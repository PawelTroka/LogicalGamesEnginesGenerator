// (m,n,k,p,q)EnginesGenerator Copyright © 2016 - 2016 Pawel Troka

using System;
using System.ComponentModel;
using _m_n_k_p_q_EngineWrapper;

namespace m_n_k_p_q_EnginesGenerator
{
    public enum EngineScheme
    {
        //(m,n,k,p,q)
        [Description("Tic Tac Toe")] TicTacToe, //3,3,3,1,1
        [Description("Connect6")] Connect6, //m,n,6,2,1
        [Description("Standard Gomoku")] StandardGomoku, //19,19,5,1,1, win exactly k
        [Description("Freestyle Gomoku")] FreeStyleGomoku //19,19,5,1,1, win k or more
    }

    public static class EngineSchemeExtensions
    {
        public static EngineParameters ToEngineParameters(this EngineScheme engineScheme)
        {
            var engine = new EngineParameters();
            switch (engineScheme)
            {
                case EngineScheme.TicTacToe:
                    engine.K = engine.M = engine.N = 3;
                    engine.P = engine.Q = 1;
                    break;
                case EngineScheme.Connect6:
                    engine.K = 6;
                    engine.M = engine.N = 15;
                    engine.P = 2;
                    engine.Q = 1;
                    engine.WinCondition = WinCondition.K_OR_MORE_TO_WIN;

                    break;
                case EngineScheme.StandardGomoku:
                    engine.K = 5;
                    engine.M = engine.N = 15;
                    engine.P = engine.Q = 1;
                    engine.WinCondition = WinCondition.EXACTLY_K_TO_WIN;
                    break;
                case EngineScheme.FreeStyleGomoku:
                    engine.K = 5;
                    engine.M = engine.N = 15;
                    engine.P = engine.Q = 1;
                    engine.WinCondition = WinCondition.K_OR_MORE_TO_WIN;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return engine;
        }
    }
}