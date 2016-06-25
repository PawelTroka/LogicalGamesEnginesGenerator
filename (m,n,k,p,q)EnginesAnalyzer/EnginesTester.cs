// (m,n,k,p,q)EnginesAnalyzer Copyright © 2016 - 2016 Pawel Troka

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using _m_n_k_p_q_EngineWrapper;

namespace _m_n_k_p_q_EnginesAnalyzer
{
    public class EnginesTester
    {
       // private readonly string _enginesDirectory;
        private string[] _enginesPaths;

        private readonly IProgress<string> _progressHandler;

        public EnginesTester(IProgress<string> progressHandler)
        {
           
          //  _enginesDirectory = enginesDirectory;
            _progressHandler = progressHandler;


            // _enginesPaths.Select(path => new EngineWrapper(path,gs => ))


        }
        

        public async Task RunPerformanceTests(string enginesDirectory, long iterations)
        {
            _enginesPaths = Directory.GetFiles(enginesDirectory, @"*.exe");
            _progressHandler.Report($"{Environment.NewLine}---- Testing performance of engines from {enginesDirectory} ----{Environment.NewLine}");
            foreach (var path in _enginesPaths)
            {
                using (var engine = new EngineWrapper(path,null,null))
                {
                    engine.Run();
                    for (long i = 0; i < iterations; i++)
                    {
                        engine.StartGame(GameType.TwoAIs);
                        await engine.WaitForGameOver();
                    }
                    var pi = engine.GetPerformanceInformation();
                    _progressHandler.Report($"----{Environment.NewLine}{engine.EngineName}{Environment.NewLine}{pi}{Environment.NewLine}----{Environment.NewLine}");
                }
            }
        }

        public void RunCorrectnessTests(string enginesDirectory)
        {
            _enginesPaths = Directory.GetFiles(enginesDirectory, @"*.exe");
            _progressHandler.Report($"{Environment.NewLine}---- Testing correctness of engines from {enginesDirectory} ----{Environment.NewLine}");
            foreach (var path in _enginesPaths)
            {
                using (var engine = new EngineWrapper(path, null, null))
                {
                    _progressHandler.Report($"---- Testing engine: {engine.EngineName}{Environment.NewLine}");
                    engine.Run();

                    var correctnessTests = (new CorrectnessTests(engine));

                    foreach (var correctnessTest in correctnessTests.GetTests())
                    {
                        var result = (bool)correctnessTest.Invoke(correctnessTests, null);
                        _progressHandler.Report($"  {correctnessTest.Name} - {(result ? "Succes!" : "failed...")}--{Environment.NewLine}");
                    }



                }
            }
        }

    }
    
    public static class RandomProvider
    {
        public static Random Generator = new Random();
    }

    public class CorrectnessTests
    {
        private readonly EngineWrapper _engine;

        private static readonly Move[] possibleMoves= new Move[]
        {
            new Move(1,1),new Move(1,2),new Move(2,1),new Move(2,2),new Move(2,3),new Move(3,2),new Move(3,3),       
        };

        private EngineParameters engineParameters;

        public CorrectnessTests(EngineWrapper engine)
        {
            _engine = engine;
            engineParameters = _engine.GetEngineInfo();
        }

        public IEnumerable<MethodInfo> GetTests()
        {
            var methods = this.GetType().GetMethods();
            foreach (var methodInfo in methods)
            {
                if (!methodInfo.IsConstructor && !methodInfo.IsAbstract && !methodInfo.GetParameters().Any() &&
                    methodInfo.ReturnType == typeof(bool))
                {
                    yield return methodInfo;
                }
            }
        }

        public bool FirstTurnHumanVsHumanMovesTest()
        {
            _engine.StartGame(GameType.TwoHumans);
            _engine.StopAsync();

            for (ulong i = 0; i < engineParameters.Q+engineParameters.P; i++)
            {
                var move = new Move(possibleMoves[i].X, possibleMoves[i].Y) {Player = (i<engineParameters.Q) ? Player.Black : Player.White};
                _engine.MakeMove(move);
                var returnedMove = _engine.GetMoveSync();
                if (move != returnedMove)
                    return false;
            }

            return true;

        }

        public bool FirstTurnHumanVsAiMovesTest()
        {
            _engine.StartGame(GameType.BlackHumanVsWhiteAi);
            _engine.StopAsync();

            var movesMade = new List<Move>();

            //human's turn
            for (ulong i = 0; i < engineParameters.Q; i++)
            {
                var move = new Move(possibleMoves[i].X, possibleMoves[i].Y) { Player = Player.Black };
                _engine.MakeMove(move);
                var returnedMove = _engine.GetMoveSync();
                if (move != returnedMove)
                    return false;
                movesMade.Add(returnedMove);
            }

            //ai's turn
            for (ulong i = 0; i < engineParameters.P; i++)
            {
                var aiMove = _engine.GetMoveSync();
                if (!movesMade.Any(m => m.IsAdjacent(aiMove)))
                    return false;
                movesMade.Add(aiMove);
            }


            return true;

        }
    }
}