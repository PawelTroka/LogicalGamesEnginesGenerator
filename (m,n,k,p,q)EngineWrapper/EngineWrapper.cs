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

        private readonly Action<GameState> _gameStateChangedCallback;


        private readonly Action<Move> _moveMadeCallback;
        private WrapperMode _mode=WrapperMode.Async;

        public void StopAsync()
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

        public Move GetMoveSync()
        {
            StopAsync();
            var line = GetLine();
            Move move;
            while (!Move.TryParse(line, out move))
            {
                line = GetLine();
            }
            return move;
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
                Move move;
                if (Move.TryParse(message, out move))
                {
                    _movesOutput.Add(move);
                    _moveMadeCallback?.Invoke(move);
                    return;
                
                }

                GameState gs;
                if (GameStateExtensions.TryParse(message,out gs))
                {
                    _gameStateChangedCallback?.Invoke(gs);
                    _gameOver = gs.IsGameOver();
                    return;
                }
            }
        }

        
        public EngineParameters GetEngineInfo()
        {
            StopAsync();
            _engine.Send("info");
            var info = GetLine();

            EngineParameters engineParameters;

            if (EngineParameters.TryParse(info, out engineParameters))
            {
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

        public PerformanceInformation GetPerformanceInformation()
        {
            StopAsync();

            _engine.Send("perf");
            var perf1 = GetLine();
            var perf2 = GetLine();

            PerformanceInformation pi;

            if (PerformanceInformation.TryParse(perf1 + perf2, out pi))
            {
                StartAsync();
                return pi;
            }
            throw  new Exception($"GetPerformanceInformation failed for {perf1} and {perf2}");
        }

        public void MakeMove(Move move)
        {
            _engine.Send($"makemove {move.X} {move.Y}");
        }

        public Task WaitForGameOver()
        {
            return Task.Run(() =>
            {
                while (!_gameOver) { Thread.Sleep(20); }
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
            while(!GetLine().Contains("game started"))
                //throw new Exception($"StartGame failed for {gameType}");
            _gameStateChangedCallback?.Invoke(GameState.Started);
            StartAsync();
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
