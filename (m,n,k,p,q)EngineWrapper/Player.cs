namespace _m_n_k_p_q_EngineWrapper
{
    public enum Player
    {
        Black,
        White,
    }

    public static class PlayerExtensions
    {
        public static bool TryParse(string str, out Player player)
        {
            player = default(Player);
            if (str.ToLowerInvariant().Contains("black"))
            {
                player = Player.Black;
                return true;
            }
            else if (str.ToLowerInvariant().Contains("white"))
            {
                player = Player.White;
                return true;
            }
            return false;
        }
    }
}