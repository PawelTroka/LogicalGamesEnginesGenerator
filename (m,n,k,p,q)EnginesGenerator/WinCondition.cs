using System.ComponentModel;

namespace m_n_k_p_q_EnginesGenerator
{
    public enum WinCondition
    {
        [Description("exactly k-in-a-row")]
        // ReSharper disable once InconsistentNaming
        EXACTLY_K_TO_WIN,
        [Description("k-in-a-row or more")]
        // ReSharper disable once InconsistentNaming
        K_OR_MORE_TO_WIN,
    }
}