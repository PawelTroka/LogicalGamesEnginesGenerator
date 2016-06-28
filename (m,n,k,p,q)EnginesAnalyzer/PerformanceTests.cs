using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
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
            var methods = GetType().GetMethods();
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
}