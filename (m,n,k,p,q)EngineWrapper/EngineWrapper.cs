using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _m_n_k_p_q_EngineWrapper
{
    public class EngineWrapper
    {
        private ProcessInBackground _engine;

        public EngineWrapper(string filename)
        {
            _engine = new ProcessInBackground(filename,"", CallbackHandler,true);
        }

        public void CallbackHandler(string message)
        {
            
        }

        public PerfInfo GetPerformanceInformation()
        {
            return new PerfInfo();
        }

        public Move MakeMove(Move move)
        {
            return new Move();
        }
    }

    public struct PerfInfo
    {
        
    }

    public struct EngineCallback
    {
        public GameState State { get; set; }
        public Move Move { get; set; }
    }

    public enum GameState
    {
        NotStarted,
        Started,

        WinnerIsBlack,
        WinnerIsWhite,
        Draw,
    }
    public struct Move
    {
        public Move(byte x, byte y)
        {
            this.X = x;
            this.Y = y;
        }
        public byte X { get; set; }
        public byte Y { get; set; }
    }
}
