// (m,n,k,p,q)EnginesAnalyzer Copyright © 2016 - 2016 Pawel Troka

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using _m_n_k_p_q_EngineWrapper;

namespace _m_n_k_p_q_EnginesAnalyzer
{

    public class PerformanceTests
    {
        private readonly EngineWrapper _engine;
        private readonly long _iterations;

        public PerformanceTests(EngineWrapper engine, long iterations)
        {
            _engine = engine;
            _iterations = iterations;
        }

        public void AiVsAiRandomGames()
        {
            _engine.StopAsync();
            for (long i = 0; i < _iterations; i++)
            {
                _engine.StartGame(GameType.TwoAIs);
                while (!_engine.GetGameStateSync().IsGameOver())
                {
                    Thread.Sleep(20);
                }
                _engine.ClearMessageQueue();
            }
        }

        public void GetMovesRandomMovesTest()
        {
            _engine.StopAsync();
            _engine.StartGame(GameType.TwoHumans);
            for (long i = 0; i < _iterations; i++)
            {
               // _engine.MakeMove(new Move(RandomProvider.Generator()));
                var moves = _engine.GetMovesSync();
                _engine.ClearMessageQueue();
            }
        }

        public IEnumerable<MethodInfo> GetTests()
        {
            var methods = this.GetType().GetMethods();
            foreach (var methodInfo in methods)
            {
                if (!methodInfo.IsConstructor && !methodInfo.IsAbstract && !methodInfo.GetParameters().Any() &&
                    methodInfo.ReturnType == typeof(void))
                {
                    yield return methodInfo;
                }
            }
        }
    }

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
        

        public void RunPerformanceTests(string enginesDirectory, long iterations)
        {
            _enginesPaths = Directory.GetFiles(enginesDirectory, @"*.exe");
            _progressHandler.Report($"{Environment.NewLine}---- Testing performance of engines from {enginesDirectory} ----{Environment.NewLine}");
            foreach (var path in _enginesPaths)
            {
                using (var engine = new EngineWrapper(path,null,null))
                {
                    engine.Run();
                    var performanceTests = new PerformanceTests(engine, iterations);

                    foreach (var test in performanceTests.GetTests())
                    {
                        test.Invoke(performanceTests, null);
                        _progressHandler.Report($"  {test.Name} - done!--{Environment.NewLine}");
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
            var testCount = 0;
            var successCount = 0;
            foreach (var path in _enginesPaths)
            {
                using (var engine = new EngineWrapper(path, null, null))
                {
                    _progressHandler.Report($"---- Testing engine: {engine.EngineName}{Environment.NewLine}");
                    engine.Run();

                    var correctnessTests = (new CorrectnessTests(engine));

                    foreach (var correctnessTest in correctnessTests.GetTests())
                    {
                        testCount++;
                        var result = (bool)correctnessTest.Invoke(correctnessTests, null);
                        if (result) successCount++;
                        _progressHandler.Report($"  {correctnessTest.Name} - {(result ? "Succes!" : "failed...")}--{Environment.NewLine}");
                    }
                }
            }
            _progressHandler.Report($"{Environment.NewLine}---- Correctness tests {successCount}/{testCount} succeeded!");
        }

    }
}