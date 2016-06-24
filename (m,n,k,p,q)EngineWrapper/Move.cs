namespace _m_n_k_p_q_EngineWrapper
{
    public class Move
    {
        public Move(byte x, byte y)
        {
            this.X = x;
            this.Y = y;
            //this.Player = player;
        }

        public byte X { get; set; }
        public byte Y { get; set; }

        public Player Player { get; set; }
    }
}