using System.ComponentModel;

namespace m_n_k_p_q_EnginesGenerator
{
    public enum WinCondition
    {
        [Description("exactly k-in-a-row")]
        ExactlyK,
        [Description("k-in-a-row or more")]
        KOrMore,
    }
}