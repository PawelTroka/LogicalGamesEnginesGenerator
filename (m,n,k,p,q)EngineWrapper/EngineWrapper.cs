using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace _m_n_k_p_q_EngineWrapper
{

    public enum WrapperMode
    {
        Async,
        Sync
    }
    public class EngineWrapper : IDisposable //TODO: handle duration callbacks
    {
        private readonly ProcessInBackground _engine;
        private readonly List<Move> _movesOutput = new List<Move>();

        private readonly List<string> _engineOutputs = new List<string>();

        private Action<GameState> _gameStateChangedCallback;


        private readonly Action<Move> _moveMadeCallback;
        private WrapperMode _mode=WrapperMode.Async;

        private void StopAsync()
        {
            _mode=WrapperMode.Sync;
        }

        private void StartAsync()
        {
            _mode = WrapperMode.Async;
        }

        private string _lastLine = null;

        public string GetLine()
        {
            while (_lastLine == null)
            {
                
            }
            var ret = _lastLine;
            _lastLine = null;

            return ret;
        }

        public string EngineName { get; }

        public EngineWrapper(string path, Action<GameState> gameStateChangedCallback, Action<Move> moveMadeCallback)
        {
            _gameStateChangedCallback = gameStateChangedCallback;
            _moveMadeCallback = moveMadeCallback;
            _engine = new ProcessInBackground(path,"", CallbackHandler,true);
            EngineName = Path.GetFileName(path);
        }

        public void Close()
        {
            StopAsync();
            _engine.Send("quit");
            var response = GetLine();//TODO: fix code below
          //  if(!response.ToLowerInvariant().Contains("has exited"))
            //    throw new Exception($"Engine didnt exit properly {response}");
        }

        public void CallbackHandler(string message)
        {
            _engineOutputs.Add(message);

            if (_mode == WrapperMode.Sync)
            {
                while (_lastLine != null)
                {

                }
                _lastLine = message;
            }
            else
            {
                var match = MoveCallbackRegex.Match(message);
                if (match.Success)
                {
                    var playerStr = match.Groups[1].Value;



                    var move = new Move(byte.Parse(match.Groups[2].Value), byte.Parse(match.Groups[3].Value));

                    if (playerStr.ToLowerInvariant().Contains("black"))
                        move.Player = Player.Black;
                    else if (playerStr.ToLowerInvariant().Contains("white"))
                        move.Player = Player.White;
                    else
                        throw new Exception($"Wrong callback from engine: {message}");

                    _movesOutput.Add(move);
                    _moveMadeCallback?.Invoke(move);

                    return;
                }

                match = WinnerIsCallbackRegex.Match(message);
                if (match.Success)
                {
                    var playerStr = match.Groups[1].Value;

                    if (playerStr.ToLowerInvariant().Contains("black"))
                    {
                        _gameStateChangedCallback?.Invoke(GameState.WinnerIsBlack);
                        _gameOver = true;
                    }
                    else if (playerStr.ToLowerInvariant().Contains("white"))
                    {
                        _gameStateChangedCallback?.Invoke(GameState.WinnerIsWhite);
                        _gameOver = true;
                    }
                    else
                        throw new Exception($"Wrong callback from engine: {message}");


                    return;
                }

                match = DrawCallbackRegex.Match(message);
                if (!match.Success) return;

                _gameStateChangedCallback?.Invoke(GameState.Draw);
                _gameOver = true;
            }
        }

        
        public EngineParameters GetEngineInfo()
        {
            StopAsync();

            var engineParameters = new EngineParameters();


            _engine.Send("info");
            var info = GetLine();


            var match = _engineInfoRegex.Match(info);
            if(!match.Success)
                throw new Exception($"engine info wrong {info}");

            engineParameters.M = ulong.Parse(match.Groups[1].Value);
            engineParameters.N = ulong.Parse(match.Groups[2].Value);
            engineParameters.K = ulong.Parse(match.Groups[3].Value);
            engineParameters.P = ulong.Parse(match.Groups[4].Value);
            engineParameters.Q = ulong.Parse(match.Groups[5].Value);

            engineParameters.WinCondition =
                (match.Groups[6].Value.ToLowerInvariant().Contains("EXACTLY_K_TO_WIN".ToLowerInvariant()))
                    ? WinCondition.EXACTLY_K_TO_WIN
                    : WinCondition.K_OR_MORE_TO_WIN;



            
            StartAsync();
            return engineParameters;
        }

        public void Run()
        {
            _engine.Run();
            _gameStateChangedCallback?.Invoke(GameState.NotStarted);
        }


        private readonly Regex _engineInfoRegex = new Regex(@"\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)\s*_?\s*(EXACTLY_K_TO_WIN|K_OR_MORE_TO_WIN)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);



        private static readonly Regex DrawCallbackRegex = new Regex(@"\s*draw\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex WinnerIsCallbackRegex = new Regex(@"\s*winner\s+is\s+(black|white)\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex MoveCallbackRegex = new Regex(@"\s*move\s+(black|white)\s+(\d+)\s+(\d+)\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex AiGetMovePerfCallbackRegex = new Regex(@".*ai.*move.+?(\d+(?:.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex GameCheckWinPerfCallbackRegex = new Regex(@".*check.*win.+?(\d+(?:.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);



        public PerformanceInformation GetPerformanceInformation()//TODO: make it cleaner, reafctor
        {
            var pi = new PerformanceInformation();
            StopAsync();

            _engine.Send("perf");
            var perf1 = GetLine();
            var perf2 = GetLine();


            var match = AiGetMovePerfCallbackRegex.Match(perf1);
            if (match.Success)
            {
                pi.AverageAiGetMoveExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.AiGetMoveExecutionUnit = match.Groups[2].Value;
            }
            match = AiGetMovePerfCallbackRegex.Match(perf2);

            if (match.Success)
            {
                pi.AverageAiGetMoveExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.AiGetMoveExecutionUnit = match.Groups[2].Value;
            }

            match = GameCheckWinPerfCallbackRegex.Match(perf1);
            if (match.Success)
            {
                pi.AverageCheckWinExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.CheckWinExecutionUnit = match.Groups[2].Value;
            }

            match = GameCheckWinPerfCallbackRegex.Match(perf2);
            if (match.Success)
            {
                pi.AverageCheckWinExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.CheckWinExecutionUnit = match.Groups[2].Value;
            }

            StartAsync();
            return pi;
        }

        public void MakeMove(Move move)
        {
            _engine.Send($"makemove {move.X} {move.Y}");
        }

        public Task WaitForGameOver()
        {
            return Task.Run(() =>
            {
                while (!_gameOver) { Thread.Sleep(100); }
            });
        }

        private bool _gameOver = false;
        public void StartGame(GameType gameType)
        {
            StopAsync();
            _gameOver = false;

            switch (gameType)
            {
                case GameType.TwoHumans:
                    _engine.Send("newgame black human white human");
                    break;
                case GameType.BlackHumanVsWhiteAi:
                    _engine.Send("newgame black human white ai");
                    break;
                case GameType.BlackAIvsWhiteHuman:
                    _engine.Send("newgame black ai white human");
                    break;
                case GameType.TwoAIs:
                    _engine.Send("newgame black ai white ai");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
            if(!GetLine().Contains("game started"))
                throw new Exception($"StartGame failed for {gameType}");
            else
            {
                _gameStateChangedCallback?.Invoke(GameState.Started);
                StartAsync();
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                
            }
            // free native resources if there are any.
            Close();
        }
    }
}
