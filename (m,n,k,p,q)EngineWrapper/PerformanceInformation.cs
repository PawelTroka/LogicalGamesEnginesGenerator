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