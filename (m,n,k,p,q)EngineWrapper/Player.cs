namespace _m_n_k_p_q_EngineWrapper
{
    public enum Player
    {
        Black,
        White
    }

    public static class PlayerExtensions
    {
        // private static readonly Regex BlackPlayerRegex = new Regex(@"\s*black\s*",RegexOptions.Compiled|RegexOptions.IgnoreCase);

        //  private static readonly Regex WhitePlayerRegex = new Regex(@"\s*white\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static bool TryParse(string str, out Player player)
        {
            player = default(Player);
            if (str.ToLowerInvariant().Replace(" ", "").Replace("\n", " ").Replace("\r", "") == "black")
            {
                player = Player.Black;
                return true;
            }
            if (str.ToLowerInvariant().Replace(" ", "").Replace("\n", " ").Replace("\r", "") == "white")
            {
                player = Player.White;
                return true;
            }
            return false;
        }
    }
}