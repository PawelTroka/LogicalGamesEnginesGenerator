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
    
    public static class RandomProvider
    {
        public static Random Generator = new Random();
    }

    public class CorrectnessTests
    {
        private readonly EngineWrapper _engine;

        private  readonly Move[] possibleMoves;

        private readonly EngineParameters engineParameters;

        public CorrectnessTests(EngineWrapper engine)
        {
            _engine = engine;
            engineParameters = _engine.GetEngineInfo();


            var possibleMovesList = new List<Move>();


          //  byte x = 1, y = 1;

            /*
            possibleMovesList.Add(new Move(x,y));



            for (ulong i = 0; i < engineParameters.N*engineParameters.M; i++)
            {
                if (x == engineParameters.N && y < engineParameters.M)
                    possibleMovesList.Add(new Move(x, ++y));
                else if (x < engineParameters.N && y == engineParameters.M)
                    possibleMovesList.Add(new Move(++x, y));
                else if (x < engineParameters.N && y < engineParameters.M)
                {
                    if (i%3 == 0)
                        possibleMovesList.Add(new Move((byte) (x+1), y));
                    else if(i % 3 == 1)
                        possibleMovesList.Add(new Move(x, (byte)(y +1)));
                    else
                        possibleMovesList.Add(new Move(++x, ++y));
                }
                else throw new Exception("generating possible moves failed");
            }*/

            for (byte x = 1; x <= engineParameters.N; x++)
            {
                for (byte y = 1; y <= engineParameters.M; y++)
                    possibleMovesList.Add(new Move(x,y));
            }

            possibleMovesList.Sort((m1,m2) => (m1.X+m1.Y).CompareTo(m2.X + m2.Y));
            possibleMoves = possibleMovesList.ToArray();
            
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


        public bool GameOverTest()
        {
            _engine.StopAsync();
            _engine.StartGame(GameType.TwoAIs);

            while (!_engine.GetGameStateSync().IsGameOver())
            {
                
            }
            return true;
        }


        public bool HumanVsHumanBlackWinTest()
        {
            _engine.StartGame(GameType.TwoHumans);


            if (engineParameters.K < Math.Max(engineParameters.M, engineParameters.N) || Math.Min(engineParameters.M, engineParameters.N)<3)//engine doesnt allow wining plus we assume m*n<=3k so min(m,n) must be at least 3
                return true;

            var blackWiningMoves = new List<Move>();

            for (ulong i = 0; i < engineParameters.K; i++)
            {
                if (engineParameters.K <= engineParameters.N)
                    blackWiningMoves.Add(new Move((byte)(engineParameters.N - i), (byte)engineParameters.M) {Player = Player.Black});
                else if (engineParameters.K <= engineParameters.M)
                    blackWiningMoves.Add(new Move((byte)(engineParameters.N), (byte)(engineParameters.M - i)) {Player = Player.Black});
            }


                var whitePossibleMoves = new List<Move>();

            foreach (var possibleMove in possibleMoves)
            {
                if (!blackWiningMoves.Any(m => m.X == possibleMove.X && m.Y == possibleMove.Y))
                {
                    whitePossibleMoves.Add(possibleMove);
                    whitePossibleMoves.Last().Player=Player.White;
                }
            }
            _engine.StopAsync();
            var whiteMove = 0;
            for (ulong blackMove = 0; blackMove < engineParameters.K; blackMove++)
            {

                while (_engine.GetCurrentPlayer() != Player.Black)
                {
                    _engine.MakeMove(whitePossibleMoves[2*whiteMove]);

                    var retMove = _engine.GetMoveSync();

                    if (retMove != whitePossibleMoves[2*whiteMove])
                        return false;

                    whiteMove++;
                }

                _engine.MakeMove(blackWiningMoves[(int) blackMove]);
                if (_engine.GetMoveSync() != blackWiningMoves[(int) blackMove])
                    return false;
            }
            return _engine.GetGameStateSync()==GameState.WinnerIsBlack;
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