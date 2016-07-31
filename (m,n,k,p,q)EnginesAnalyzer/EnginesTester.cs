// (m,n,k,p,q)EnginesAnalyzer Copyright © 2016 - 2016 Pawel Troka

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using _m_n_k_p_q_EngineWrapper;

namespace _m_n_k_p_q_EnginesAnalyzer
{
    public class EnginesTester
    {
        private readonly IProgress<string> _progressHandler;
        private string[] _enginesPaths;

        public EnginesTester(IProgress<string> progressHandler)
        {
            _progressHandler = progressHandler;
        }

        public DataTable PerformanceResults { get; private set; }
        public DataTable CorrectnessResults { get; private set; }

        public void RunPerformanceTests(string enginesDirectory, long iterations)
        {
            // PerformanceResults.Clear();
            PerformanceInformation performanceInformation;
            PerformanceResults = new DataTable()
            {
                Columns = {
                    { "Engine",typeof(string)},
                    { nameof(performanceInformation.AverageAiGetMoveExecution)+" ns", typeof(double) },
                    { nameof(performanceInformation.AverageGetMovesExecution)+" ns" , typeof(double) },
                    { nameof(performanceInformation.AverageCheckWinExecution)+" ns" , typeof(double) },
                }
            };

            _enginesPaths = Directory.GetFiles(enginesDirectory, @"*.exe");
            _progressHandler.Report(
                $"{Environment.NewLine}---- Testing performance of engines from {enginesDirectory} ----{Environment.NewLine}");
            foreach (var path in _enginesPaths)
            {
                using (var engine = new EngineWrapper(path, null, null))
                {
                    engine.Run();
                    var performanceTests = new PerformanceTests(engine, iterations);

                    foreach (var test in performanceTests.GetTests())
                    {
                        test.Invoke(performanceTests, null);
                        _progressHandler.Report($"  {test.Name} - done!--{Environment.NewLine}");
                    }

                    var pi = engine.GetPerformanceInformation();
                    _progressHandler.Report(
                        $"----{Environment.NewLine}{engine.EngineName}{Environment.NewLine}{pi}{Environment.NewLine}----{Environment.NewLine}");
                    PerformanceResults.Rows.Add(engine.EngineName, pi.AverageAiGetMoveExecution.Value,
                                                pi.AverageGetMovesExecution.Value, pi.AverageCheckWinExecution.Value);
                }
            }
        }

        public void RunCorrectnessTests(string enginesDirectory)
        {
            CorrectnessResults = new DataTable()
            {
                Columns = {
                    { "Engine",typeof(string)}
                }
            };
            CorrectnessResults.Columns.AddRange(CorrectnessTests.GetTests().Select(t => new DataColumn(t.Name, typeof(bool))).ToArray());

            _enginesPaths = Directory.GetFiles(enginesDirectory, @"*.exe");
            _progressHandler.Report(
                $"{Environment.NewLine}---- Testing correctness of engines from {enginesDirectory} ----{Environment.NewLine}");
            var testCount = 0;
            var successCount = 0;
            foreach (var path in _enginesPaths)
            {
                using (var engine = new EngineWrapper(path, null, null))
                {
                    _progressHandler.Report($"---- Testing engine: {engine.EngineName}{Environment.NewLine}");
                    engine.Run();
                    
                    var correctnessTests = new CorrectnessTests(engine);
                    var tests = CorrectnessTests.GetTests().ToArray();

                    var row = new List<object>() {engine.EngineName};
                    row.AddRange(Enumerable.Repeat((object)false,tests.Length));
                    CorrectnessResults.Rows.Add(row.ToArray());

                    for (int index = 0; index < tests.Length; index++)
                    {
                        var correctnessTest = tests[index];
                        testCount++;
                        var result = (bool) correctnessTest.Invoke(correctnessTests, null);
                        if (result) successCount++;
                        _progressHandler.Report(
                            $"  {correctnessTest.Name} - {(result ? "Succes!" : "failed...")}--{Environment.NewLine}");
                        CorrectnessResults.Rows[CorrectnessResults.Rows.Count - 1][index + 1] = result;

                    }
                }
            }
            _progressHandler.Report($"{Environment.NewLine}---- Correctness tests {successCount}/{testCount} succeeded!");
        }
    }
}