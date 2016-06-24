using System;
using System.Text.RegularExpressions;

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

        private static readonly Regex MoveCallbackRegex = new Regex(@"\s*move\s+(black|white)\s+(\d+)\s+(\d+)\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool TryParse(string str, out Move move)
        {
            var match = MoveCallbackRegex.Match(str);
            if (match.Success)
            {
                move = new Move(byte.Parse(match.Groups[2].Value), byte.Parse(match.Groups[3].Value));
                Player player;
                if (PlayerExtensions.TryParse(match.Groups[1].Value, out player))
                {
                    move.Player = player;
                    return true;
                }
                //throw new Exception($"Move.TryParse() could not parse: {str}");
            }
            move = null;
            return false;
        }
    }
}