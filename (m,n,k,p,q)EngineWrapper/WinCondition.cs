using System.ComponentModel;

namespace _m_n_k_p_q_EngineWrapper
{
    public enum WinCondition
    {
        [Description("exactly k-in-a-row")]
        // ReSharper disable once InconsistentNaming
        EXACTLY_K_TO_WIN,

        [Description("k-in-a-row or more")]
        // ReSharper disable once InconsistentNaming
        K_OR_MORE_TO_WIN
    }
}