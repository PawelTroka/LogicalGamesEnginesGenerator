using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _m_n_k_p_q_EngineWrapper;

namespace _m_n_k_p_q_EnginesAnalyzer
{
    public class CorrectnessTests
    {
        private readonly EngineWrapper _engine;

        private readonly EngineParameters _engineParameters;

        private readonly Move[] _possibleMoves;

        public CorrectnessTests(EngineWrapper engine)
        {
            _engine = engine;
            _engineParameters = _engine.GetEngineInfo();


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

            for (byte x = 1; x <= _engineParameters.N; x++)
            {
                for (byte y = 1; y <= _engineParameters.M; y++)
                    possibleMovesList.Add(new Move(x, y));
            }

            ///////////////////////////////// possibleMovesList.Sort((m1,m2) => (m1.X+m1.Y).CompareTo(m2.X + m2.Y));
            _possibleMoves = possibleMovesList.ToArray();
        }

        public static IEnumerable<MethodInfo> GetTests()
        {
            var methods = typeof(CorrectnessTests).GetMethods();
            foreach (var methodInfo in methods)
            {
                if (!methodInfo.IsConstructor && !methodInfo.IsAbstract && !methodInfo.GetParameters().Any() &&
                    methodInfo.ReturnType == typeof(bool))
                {
                    yield return methodInfo;
                }
            }
        }


        public bool GetMovesShouldReturnAvalaibleMoves()
        {
            _engine.StartGame(GameType.TwoHumans);
            _engine.StopAsync();


            var possibleMovesList = new List<Move>();
            for (byte y = 1; y <= _engineParameters.M; y++)
                for (byte x = 1; x <= _engineParameters.N; x++)
                    possibleMovesList.Add(new Move(x, y));

            var moves = _engine.GetMovesSync();

            if (!possibleMovesList.SequenceEqual(moves))
                return false;

            for (ulong i = 0; i < _engineParameters.K; i++)
            {
                _engine.MakeMove(possibleMovesList[0]);
                possibleMovesList.RemoveAt(0);

                moves = _engine.GetMovesSync();
                if (!possibleMovesList.SequenceEqual(moves))
                    return false;
            }

            return true;
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

        public bool AfterQMovesItsWhitePlayerTurn()
        {
            _engine.StartGame(GameType.TwoHumans);
            _engine.StopAsync();

            //black turn
            for (ulong i = 0; i < _engineParameters.Q; i++)
            {
                if (_engine.GetCurrentPlayer() != Player.Black)
                    return false;
                _engine.MakeMove(_possibleMoves[i]);
            }

            //white turn
            for (ulong i = 0; i < _engineParameters.P; i++)
            {
                if (_engine.GetCurrentPlayer() != Player.White)
                    return false;
                _engine.MakeMove(_possibleMoves[_engineParameters.Q + i]);
            }
            //should be again black turn
            return _engine.GetCurrentPlayer() == Player.Black;
        }

        public bool TwoTimesSameMoveShouldNotBePossible()
        {
            _engine.StartGame(GameType.TwoHumans);
            _engine.StopAsync();

            var firstMove = new Move(1, 1) {Player = Player.Black};
            _engine.MakeMove(firstMove);
            if (_engine.GetMoveSync() != firstMove)
                return false;

            _engine.MakeMove(firstMove);
            return _engine.GetMoveSync() == null;
        }

        public bool AllPossibleMovesGameShouldEndTest()
        {
            _engine.StartGame(GameType.TwoHumans);
            _engine.StopAsync();

            for (ulong i = 0; i < _engineParameters.M*_engineParameters.N; i++)
                _engine.MakeMove(_possibleMoves[i]);

            return _engine.GetGameStateSync().IsGameOver();
        }

        public bool HumanVsHumanBlackWinTest()
        {
            _engine.StartGame(GameType.TwoHumans);

            if (_engineParameters.K < Math.Max(_engineParameters.M, _engineParameters.N) ||
                Math.Min(_engineParameters.M, _engineParameters.N) < 3)
                //engine doesnt allow wining plus we assume m*n<=3k so min(m,n) must be at least 3
                return true;

            var blackWiningMoves = WiningMovesFromEnd(_engineParameters.K, Player.Black);

            var whitePossibleMoves = new List<Move>();

            foreach (var possibleMove in _possibleMoves)
            {
                if (!blackWiningMoves.Any(m => m.X == possibleMove.X && m.Y == possibleMove.Y))
                {
                    whitePossibleMoves.Add(possibleMove);
                    whitePossibleMoves.Last().Player = Player.White;
                }
            }
            _engine.StopAsync();
            var whiteMove = 0;
            for (ulong blackMove = 0; blackMove < _engineParameters.K; blackMove++)
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
            return _engine.GetGameStateSync() == GameState.WinnerIsBlack;
        }

        public bool HumanVsHumanBlackWinIfKOrMoreToWinTest()
        {
            _engine.StartGame(GameType.TwoHumans);

            //we need max(m,n)>k if we wanna test K_OR_MORE vs EXACTLY_K behaviour
            if (_engineParameters.K <= Math.Max(_engineParameters.M, _engineParameters.N) ||
                Math.Min(_engineParameters.M, _engineParameters.N) < 3)
                //engine doesnt allow wining plus we assume m*n<=3k so min(m,n) must be at least 3
                return true;

            var blackWiningMoves = WiningMovesFromEnd(_engineParameters.K + 1, Player.Black);


            var whiteWiningMoves = WiningMovesFromStart(_engineParameters.K + 1, Player.White);
            var whitePossibleMoves = new List<Move>();

            foreach (var possibleMove in _possibleMoves)
            {
                if (!blackWiningMoves.Any(m => m.X == possibleMove.X && m.Y == possibleMove.Y) &&
                    !whiteWiningMoves.Any(m => m.X == possibleMove.X && m.Y == possibleMove.Y))
                {
                    whitePossibleMoves.Add(possibleMove);
                    whitePossibleMoves.Last().Player = Player.White;
                }
            }
            _engine.StopAsync();
            var whiteMove = 0;

            _engine.MakeMove(blackWiningMoves[0]);
            if (_engine.GetMoveSync() != blackWiningMoves[0])
                return false;

            //we skipped move blackWiningMoves[1] for now
            for (ulong blackMove = 2; blackMove < _engineParameters.K + 1; blackMove++)
            {
                while (_engine.GetCurrentPlayer() != Player.Black)
                {
                    if (whiteMove < (int) _engineParameters.K - 1)
                    {
                        _engine.MakeMove(whiteWiningMoves[whiteMove]);
                        var retMove = _engine.GetMoveSync();

                        if (retMove != whiteWiningMoves[whiteMove])
                            return false;
                    }
                    else
                    {
                        _engine.MakeMove(whitePossibleMoves[whiteMove - (int) _engineParameters.K + 1]);
                        var retMove = _engine.GetMoveSync();

                        if (retMove != whitePossibleMoves[whiteMove - (int) _engineParameters.K + 1])
                            return false;
                    }
                    whiteMove++;
                }

                _engine.MakeMove(blackWiningMoves[(int) blackMove]);
                if (_engine.GetMoveSync() != blackWiningMoves[(int) blackMove])
                    return false;
            }

            _engine.MakeMove(blackWiningMoves[1]);
            if (_engine.GetMoveSync() != blackWiningMoves[1])
                return false;

            if (_engineParameters.WinCondition == WinCondition.K_OR_MORE_TO_WIN)
                return _engine.GetGameStateSync() == GameState.WinnerIsBlack;
            if (_engineParameters.WinCondition == WinCondition.EXACTLY_K_TO_WIN)
            {
                _engine.MakeMove(whiteWiningMoves[(int) _engineParameters.K - 1]);
                var retMove = _engine.GetMoveSync();

                if (retMove != whiteWiningMoves[(int) _engineParameters.K - 1])
                    return false;
                return _engine.GetGameStateSync() == GameState.WinnerIsWhite;
            }
            return false;
        }

        private List<Move> WiningMovesFromEnd(ulong count, Player player)
        {
            var winingMoves = new List<Move>();

            for (ulong i = 0; i < count; i++)
            {
                if (count <= _engineParameters.N)
                    winingMoves.Add(new Move((byte) (_engineParameters.N - i), (byte) _engineParameters.M)
                    {
                        Player = player
                    });
                else if (count <= _engineParameters.M)
                    winingMoves.Add(new Move((byte) _engineParameters.N, (byte) (_engineParameters.M - i))
                    {
                        Player = player
                    });
            }
            return winingMoves;
        }

        private List<Move> WiningMovesFromStart(ulong count, Player player)
        {
            var winingMoves = new List<Move>();

            for (ulong i = 0; i < count; i++)
            {
                if (count <= _engineParameters.N)
                    winingMoves.Add(new Move((byte) i, 0)
                    {
                        Player = player
                    });
                else if (count <= _engineParameters.M)
                    winingMoves.Add(new Move(0, (byte) i)
                    {
                        Player = player
                    });
            }
            return winingMoves;
        }

        public bool FirstTurnHumanVsHumanMovesTest()
        {
            _engine.StartGame(GameType.TwoHumans);
            _engine.StopAsync();

            for (ulong i = 0; i < _engineParameters.Q + _engineParameters.P; i++)
            {
                var move = new Move(_possibleMoves[i].X, _possibleMoves[i].Y)
                {
                    Player = i < _engineParameters.Q ? Player.Black : Player.White
                };
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
            for (ulong i = 0; i < _engineParameters.Q; i++)
            {
                var move = new Move(_possibleMoves[i].X, _possibleMoves[i].Y) {Player = Player.Black};
                _engine.MakeMove(move);
                var returnedMove = _engine.GetMoveSync();
                if (move != returnedMove)
                    return false;
                movesMade.Add(returnedMove);
            }

            //ai's turn
            for (ulong i = 0; i < _engineParameters.P; i++)
            {
                var aiMove = _engine.GetMoveSync();
                if (!movesMade.Any(m => m.IsAdjacent(aiMove)))
                    return false;
                movesMade.Add(aiMove);
            }
            return true;
        }


        public bool FirstTurnAiVsHumanMovesTest()
        {
            _engine.StopAsync();

            var movesMade = new List<Move>();


            var movesAvalaible = _possibleMoves.ToList();
            _engine.StartGame(GameType.BlackAIvsWhiteHuman);


            //ai's turn
            for (ulong i = 0; i < _engineParameters.Q; i++)
            {
                var aiMove = _engine.GetMoveSync();
                if (movesMade.Count > 0 && !movesMade.Any(m => m.IsAdjacent(aiMove)))
                    return false;
                movesMade.Add(aiMove);
                movesAvalaible.RemoveAll(m => m.X == aiMove.X && m.Y == aiMove.Y);
            }

            //human's turn
            for (ulong i = 0; i < _engineParameters.P; i++)
            {
                var move = new Move(movesAvalaible[(int) i].X, movesAvalaible[(int) i].Y) {Player = Player.White};
                _engine.MakeMove(move);
                var returnedMove = _engine.GetMoveSync();
                if (move != returnedMove)
                    return false;
            }
            return true;
        }


        //checks if AIs are making right moves in first phase of the game
        public bool FirstTurnAiVsAiMovesTest()
        {
            _engine.StopAsync();

            var movesMade = new List<Move>();
            _engine.StartGame(GameType.TwoAIs);


            //ai's two turns
            for (ulong i = 0; i < _engineParameters.Q + _engineParameters.P; i++)
            {
                var aiMove = _engine.GetMoveSync();
                if (movesMade.Count > 0 && !movesMade.Any(m => m.IsAdjacent(aiMove)))
                    return false;
                movesMade.Add(aiMove);
            }
            return true;
        }
    }
}