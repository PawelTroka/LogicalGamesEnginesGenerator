// (m,n,k,p,q)EnginesGenerator Copyright © 2016 - 2016 Pawel Troka

using System.ComponentModel;

namespace m_n_k_p_q_EnginesGenerator
{
    public enum EngineScheme
    {
        //(m,n,k,p,q)
        [Description("Tic Tac Toe")]
        TicTacToe,//3,3,3,1,1
        [Description("Connect6")]
        Connect6,//m,n,6,2,1
        [Description("Standard Gomoku")]
        StandardGomoku,//19,19,5,1,1, win exactly k
        [Description("Freestyle Gomoku")]
        FreeStyleGomoku,//19,19,5,1,1, win k or more
    }
}