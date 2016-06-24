using System.Globalization;
using System.Text.RegularExpressions;

namespace _m_n_k_p_q_EngineWrapper
{
    public class PerformanceInformation
    {
        public PerformanceInformation() { }
        public PerformanceInformation(double averageAiGetMoveExecution, double averageCheckWinExecution, string aiGetMoveExecutionUnit, string checkWinExecutionUnit)
        {
            AverageAiGetMoveExecution = averageAiGetMoveExecution;
            AverageCheckWinExecution = averageCheckWinExecution;
            AiGetMoveExecutionUnit = aiGetMoveExecutionUnit;
            CheckWinExecutionUnit = checkWinExecutionUnit;
        }

        private static readonly Regex AiGetMovePerfCallbackRegex = new Regex(@".*ai.*move.+?(\d+(?:.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex GameCheckWinPerfCallbackRegex = new Regex(@".*check.*win.+?(\d+(?:.|,)?\d*)\s*(\w*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        public static bool TryParse(string str, out PerformanceInformation pi)
        {
            pi = new PerformanceInformation();
            var match = AiGetMovePerfCallbackRegex.Match(str);
            if (match.Success)
            {
                pi.AverageAiGetMoveExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.AiGetMoveExecutionUnit = match.Groups[2].Value;
            }

            match = GameCheckWinPerfCallbackRegex.Match(str);
            if (match.Success)
            {
                pi.AverageCheckWinExecution = double.Parse(match.Groups[1].Value.Replace(",", "."),
                    CultureInfo.InvariantCulture);
                pi.CheckWinExecutionUnit = match.Groups[2].Value;
                return true;
            }
            return false;
        }

        public double AverageAiGetMoveExecution { get; set; }
        public string AiGetMoveExecutionUnit { get; set; }
        public double AverageCheckWinExecution { get; set; }

        public string CheckWinExecutionUnit { get; set; }


        public override string ToString()
        {
            return $@"AIPlayer::GetMove() average execution time is {AverageAiGetMoveExecution}{AiGetMoveExecutionUnit}
Game::CheckWin() average execution time is {AverageCheckWinExecution}{CheckWinExecutionUnit}";
        }
    }
}