using System.Text.RegularExpressions;

namespace _m_n_k_p_q_EngineWrapper
{
    public enum GameState
    {
        NotStarted,
        Started,

        WinnerIsBlack,
        WinnerIsWhite,
        Draw
    }


    public static class GameStateExtensions
    {
        private static readonly Regex GameStartedCallbackRegex = new Regex(@"\s*game\s+started\s*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex GameEndedCallbackRegex = new Regex(@".*has\s+exited\s*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex DrawCallbackRegex = new Regex(@"\s*draw\s*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex WinnerIsCallbackRegex = new Regex(@"\s*winner\s+is\s+(black|white)\s*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool IsGameOver(this GameState gs)
        {
            return gs == GameState.WinnerIsBlack || gs == GameState.WinnerIsWhite || gs == GameState.Draw;
        }

        public static bool TryParse(string message, out GameState gs)
        {
            gs = default(GameState);

            if (GameStartedCallbackRegex.IsMatch(message))
            {
                gs = GameState.Started;
                return true;
            }

            var match = WinnerIsCallbackRegex.Match(message);
            if (match.Success)
            {
                Player player;
                if (PlayerExtensions.TryParse(match.Groups[1].Value, out player))
                {
                    if (player == Player.Black)
                    {
                        gs = GameState.WinnerIsBlack;
                        return true;
                    }
                    if (player == Player.White)
                    {
                        gs = GameState.WinnerIsWhite;
                        return true;
                    }
                }
            }

            match = DrawCallbackRegex.Match(message);
            if (match.Success)
            {
                gs = GameState.Draw;
                return true;
            }

            return false;
        }
    }
}