using System;
using System.Text.RegularExpressions;

namespace _m_n_k_p_q_EngineWrapper
{
    public class Move : IEquatable<Move>
    {
        private static readonly Regex MoveCallbackRegex = new Regex(@"\s*move\s+(black|white)\s+(\d+)\s+(\d+)\s*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public Move(byte x, byte y)
        {
            X = x;
            Y = y;
            //this.Player = player;
        }

        public byte X { get; set; }
        public byte Y { get; set; }

        public Player Player { get; set; }

        public bool Equals(Move other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return X == other.X && Y == other.Y && Player == other.Player;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Move) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode*397) ^ Y.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Player;
                return hashCode;
            }
        }

        public static bool operator ==(Move left, Move right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !Equals(left, right);
        }

        public bool IsAdjacent(Move move)
        {
            // ReSharper disable once RedundantCast
            return Math.Abs((int) X - move.X) <= 1 && Math.Abs(Y - move.Y) <= 1;
        }

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

            if (str.ToLowerInvariant().Contains("invalid move"))
                return true;

            return false;
        }
    }
}