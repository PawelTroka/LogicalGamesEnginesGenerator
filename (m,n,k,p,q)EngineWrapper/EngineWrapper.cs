using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _m_n_k_p_q_EngineWrapper
{
    public class EngineWrapper
    {
        private readonly ProcessInBackground _engine;
        private readonly List<Move> _movesOutput = new List<Move>();

        private readonly Action<GameState> _gameStateChangedCallback;


        private readonly Action<Move> _moveMadeCallback;

        public string EngineName { get; }

        public EngineWrapper(string path, Action<GameState> gameStateChangedCallback, Action<Move> moveMadeCallback)
        {
            _gameStateChangedCallback = gameStateChangedCallback;
            _moveMadeCallback = moveMadeCallback;
            _engine = new ProcessInBackground(path,"", CallbackHandler,true);
            EngineName = Path.GetFileName(path);
        }

        public void CallbackHandler(string message)
        {
            var match = _moveCallbackRegex.Match(message);
            if (match.Success)
            {
                var playerStr = match.Groups[1].Value;
     
                

                var move = new Move(byte.Parse(match.Groups[2].Value), byte.Parse(match.Groups[3].Value));

                if(playerStr.ToLowerInvariant().Contains("black"))
                    move.Player=Player.Black;
                else if (playerStr.ToLowerInvariant().Contains("white"))
                    move.Player = Player.White;
                else
                    throw new Exception($"Wrong callback from engine: {message}");

                _movesOutput.Add(move);
                _moveMadeCallback?.Invoke(move);

                return;
            }
           
            match = _winnerIsCallbackRegex.Match(message);
            if (match.Success)
            {
                var playerStr = match.Groups[1].Value;

                if (playerStr.ToLowerInvariant().Contains("black"))
                    _gameStateChangedCallback?.Invoke(GameState.WinnerIsBlack);
                else if (playerStr.ToLowerInvariant().Contains("white"))
                    _gameStateChangedCallback?.Invoke(GameState.WinnerIsWhite);
                else
                    throw new Exception($"Wrong callback from engine: {message}");


                return;
            }

            match = _drawCallbackRegex.Match(message);
            if (!match.Success) return;
            _gameStateChangedCallback?.Invoke(GameState.Draw);
        }

        
        public EngineParameters GetEngineInfo()
        {
            _engine.StopAsync();

            var engineParameters = new EngineParameters();


            _engine.Send("info");
            var info = _engine.GetLine();


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



            
            _engine.StartAsync();
            return engineParameters;
        }

        public void Run()
        {
            _engine.Run();
            _gameStateChangedCallback?.Invoke(GameState.NotStarted);
        }


        private readonly Regex _engineInfoRegex = new Regex(@"\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)\s*_?\s*(EXACTLY_K_TO_WIN|K_OR_MORE_TO_WIN)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);



        private readonly Regex _drawCallbackRegex = new Regex(@"\s+draw\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly Regex _winnerIsCallbackRegex = new Regex(@"\s+winner\s+is\s+(black|white)\s+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly Regex _moveCallbackRegex = new Regex(@"\s+move\s+(black|white)\s+(\d+)\s+(\d+)", RegexOptions.IgnoreCase|RegexOptions.Compiled);

        private readonly Regex _aiGetMovePerfCallbackRegex = new Regex(@".*ai.*move.+?(\d+(?:.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly Regex _gameCheckWinPerfCallbackRegex = new Regex(@".*check.*win.+?(\d+(?:.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        
        public PerforamanceInformation GetPerformanceInformation()//TODO: make it cleaner, reafctor
        {
            var pi = new PerforamanceInformation();
            _engine.StopAsync();

            _engine.Send("perf");
            var perf1 = _engine.GetLine();
            var perf2 = _engine.GetLine();


            var match = _aiGetMovePerfCallbackRegex.Match(perf1);
            if (match.Success)
            {
                pi.AverageAiGetMoveExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.AiGetMoveExecutionUnit = match.Groups[2].Value;
            }
            match = _aiGetMovePerfCallbackRegex.Match(perf2);

            if (match.Success)
            {
                pi.AverageAiGetMoveExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.AiGetMoveExecutionUnit = match.Groups[2].Value;
            }

            match = _gameCheckWinPerfCallbackRegex.Match(perf1);
            if (match.Success)
            {
                pi.AverageCheckWinExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.CheckWinExecutionUnit = match.Groups[2].Value;
            }

            match = _gameCheckWinPerfCallbackRegex.Match(perf2);
            if (match.Success)
            {
                pi.AverageCheckWinExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.CheckWinExecutionUnit = match.Groups[2].Value;
            }

            _engine.StartAsync();
            return pi;
        }

        public void MakeMove(Move move)
        {
            _engine.Send($"makemove {move.X} {move.Y}");
        }

        public void StartGame(GameType gameType)
        {
            _engine.StopAsync();

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
            if(!_engine.GetLine().Contains("game started"))
                throw new Exception($"StartGame failed for {gameType}");
            else
            {
                _gameStateChangedCallback?.Invoke(GameState.Started);
                _engine.StartAsync();
            }

        }
    }
}
