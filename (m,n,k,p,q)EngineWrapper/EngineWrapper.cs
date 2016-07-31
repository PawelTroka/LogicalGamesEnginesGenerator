using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace _m_n_k_p_q_EngineWrapper
{
    public class EngineWrapper : IDisposable
    {
        private static readonly Regex GetMovesRegex = new Regex(@"\s*moves:\s*");
        private static readonly Regex MoveFromGetMovesRegex = new Regex(@"\s*\(\s*(\d+)\s+(\d+)\s*\)\s*");

        private readonly ProcessInBackground _engine;
        
        private readonly Action<GameState> _gameStateChangedCallback;
        private readonly Action<Move> _moveMadeCallback;
        
        private bool _gameOver;
        private WrapperMode _mode = WrapperMode.Async;

        private ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();

        private readonly List<string> _engineOutputs = new List<string>();
        private readonly List<Move> _movesOutput = new List<Move>();

        public string EngineName { get; }

        public EngineWrapper(string path, Action<GameState> gameStateChangedCallback, Action<Move> moveMadeCallback)
        {
            _gameStateChangedCallback = gameStateChangedCallback;
            _moveMadeCallback = moveMadeCallback;
            _engine = new ProcessInBackground(path, "", CallbackHandler, true);
            EngineName = Path.GetFileName(path);
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void StopAsync()
        {
            _mode = WrapperMode.Sync;
        }

        public void StartAsync()
        {
            _mode = WrapperMode.Async;
        }

        public string GetLine()
        {
            string ret;
            while (!_messages.TryDequeue(out ret))
            {
                Thread.Sleep(20);
            }

            return ret;
        }


        public GameState GetGameStateSync()
        {
            StopAsync();
            var line = GetLine();
            GameState state;
            while (!GameStateExtensions.TryParse(line, out state))
            {
                line = GetLine();
            }
            return state;
        }

        public Move GetMoveSync()
        {
            StopAsync();
            Move move;
            while (!Move.TryParse(GetLine(), out move))
            {
            }
            return move;
        }

        public IEnumerable<Move> GetMovesSync()
        {
            var restoreAsync = false;
            if (_mode == WrapperMode.Async)
            {
                restoreAsync = true;
                StopAsync();
            }
            _engine.Send("getmoves");

            var line = "";

            while (!GetMovesRegex.IsMatch(line))
            {
                line = GetLine();
            }

            var moves = new List<Move>();

            var match = MoveFromGetMovesRegex.Match(line);
            while (match.Success)
            {
                moves.Add(new Move(byte.Parse(match.Groups[1].Value), byte.Parse(match.Groups[2].Value)));
                match = match.NextMatch();
            }

            if (restoreAsync)
                StartAsync();
            return moves;
        }

        public void Close()
        {
            StopAsync();
            _engine.Send("quit");
            while (!GetLine().ToLowerInvariant().Contains("has exited"))
            {
            }
        }

        public void ClearMessageQueue()
        {
            _messages = new ConcurrentQueue<string>();
        }

        public void CallbackHandler(string message)
        {
            _engineOutputs.Add(message);

            if (_mode == WrapperMode.Sync)
            {
                _messages.Enqueue(message);
            }
            else
            {
                Move move;
                if (Move.TryParse(message, out move))
                {
                    _movesOutput.Add(move);
                    _moveMadeCallback?.Invoke(move);
                    return;
                }

                GameState gs;
                if (GameStateExtensions.TryParse(message, out gs))
                {
                    _gameStateChangedCallback?.Invoke(gs);
                    _gameOver = gs.IsGameOver();
                }
            }
        }


        public EngineParameters GetEngineInfo()
        {
            var restoreAsync = false;
            if (_mode == WrapperMode.Async)
            {
                restoreAsync = true;
                StopAsync();
            }

            _engine.Send("info");
            var info = GetLine();

            EngineParameters engineParameters;

            if (EngineParameters.TryParse(info, out engineParameters))
            {
                if (restoreAsync)
                    StartAsync();
                return engineParameters;
            }
            throw new Exception($"engine info wrong {info}");
        }

        public void Run()
        {
            _engine.Run();
            _gameStateChangedCallback?.Invoke(GameState.NotStarted);
        }

        public Player GetCurrentPlayer()
        {
            var restoreAsync = false;
            if (_mode == WrapperMode.Async)
            {
                restoreAsync = true;
                StopAsync();
            }
            _engine.Send("getplayer");
            Player player;

            while (!PlayerExtensions.TryParse(GetLine(), out player)) { }

            if (restoreAsync)
                StartAsync();

            return player;
        }


        public PerformanceInformation GetPerformanceInformation()
        {
            var restoreAsync = false;
            if (_mode == WrapperMode.Async)
            {
                restoreAsync = true;
                StopAsync();
            }
            _engine.Send("perf");
            var perf = GetLine() + GetLine() + GetLine();


            PerformanceInformation pi;

            if (PerformanceInformation.TryParse(perf, out pi))
            {
                if (restoreAsync)
                    StartAsync();
                return pi;
            }
            throw new Exception($"GetPerformanceInformation failed for {perf}");
        }

        public void MakeMove(Move move)
        {
            _engine.Send($"makemove {move.X} {move.Y}{Environment.NewLine}");
        }

        public Task WaitForGameOver()
        {
            return Task.Run(() =>
            {
                while (!_gameOver)
                {
                    Thread.Sleep(20);
                }
            });
        }

        public void StartGame(GameType gameType)
        {
            _messages = new ConcurrentQueue<string>(); /////////////////???????????????????????????????????????

            var restoreAsync = false;
            if (_mode == WrapperMode.Async)
            {
                restoreAsync = true;
                StopAsync();
            }

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
            while (!GetLine().Contains("game started")) { }
                //throw new Exception($"StartGame failed for {gameType}");
                _gameStateChangedCallback?.Invoke(GameState.Started);

            if (!restoreAsync)
                StartAsync();
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