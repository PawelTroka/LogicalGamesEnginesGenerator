using System.Text.RegularExpressions;

namespace _m_n_k_p_q_EngineWrapper
{
    public enum Player
    {
        Black,
        White
    }

    public static class PlayerExtensions
    {
        public static bool TryParse(string str, out Player player)
        {
            player = default(Player);
            str = Regex.Replace(str.ToLowerInvariant(), @"\s+", "");
            if (str == "black")
            {
                player = Player.Black;
                return true;
            }
            if (str == "white")
            {
                player = Player.White;
                return true;
            }
            return false;
        }
    }
}