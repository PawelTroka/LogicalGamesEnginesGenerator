// (m,n,k,p,q)EnginesAnalyzer Copyright © 2016 - 2016 Pawel Troka

using System;
using System.IO;
using System.Linq;
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
                    engine.Run();



                    _progressHandler.Report($"----");
                }
            }
        }

    }

    public class CorrectnessTests
    {
        private EngineWrapper _engine;

        public CorrectnessTests(EngineWrapper engine)
        {
            _engine = engine;
        }

        public bool Test1()
        {
            //_engine.StartGame(GameType.BlackHumanVsWhiteAi);
            return true;

        }
    }
}