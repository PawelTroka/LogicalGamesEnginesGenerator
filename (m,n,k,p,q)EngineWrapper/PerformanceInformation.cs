using System.Globalization;
using System.Text.RegularExpressions;

namespace _m_n_k_p_q_EngineWrapper
{
    public class PerformanceInformation
    {
        private static readonly Regex AiGetMovePerfCallbackRegex = new Regex(
            @".*ai.+?move.+?(\d+(?:\.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex GameCheckWinPerfCallbackRegex =
            new Regex(@".*checkwin.+?(\d+(?:\.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex GameGetMovesPerfCallbackRegex =
            new Regex(@".*getmoves.+?(\d+(?:\.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ValueWithUnit AverageAiGetMoveExecution { get; set; }
        public ValueWithUnit AverageCheckWinExecution { get; set; }
        public ValueWithUnit AverageGetMovesExecution { get; set; }


        public static bool TryParse(string str, out PerformanceInformation pi)
        {
            pi = new PerformanceInformation();

            var match = AiGetMovePerfCallbackRegex.Match(str);
            if (!match.Success) return false;
            pi.AverageAiGetMoveExecution = new ValueWithUnit(double.Parse(match.Groups[1].Value.Replace(",", "."),
                CultureInfo.InvariantCulture), match.Groups[2].Value);

            match = GameCheckWinPerfCallbackRegex.Match(str);
            if (!match.Success) return false;
            pi.AverageCheckWinExecution = new ValueWithUnit(double.Parse(match.Groups[1].Value.Replace(",", "."),
                CultureInfo.InvariantCulture), match.Groups[2].Value);

            match = GameGetMovesPerfCallbackRegex.Match(str);
            if (!match.Success) return false;
            pi.AverageGetMovesExecution = new ValueWithUnit(double.Parse(match.Groups[1].Value.Replace(",", "."),
                CultureInfo.InvariantCulture), match.Groups[2].Value);

            return true;
        }


        public override string ToString()
        {
            return
                $@"AIPlayer::GetMove() average execution time is {AverageAiGetMoveExecution}
Game::CheckWin() average execution time is {AverageCheckWinExecution}
Game::GetMoves() average execution time is {AverageGetMovesExecution}";
        }
    }
}