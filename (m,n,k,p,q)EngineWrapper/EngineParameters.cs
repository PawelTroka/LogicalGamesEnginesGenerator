using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace _m_n_k_p_q_EngineWrapper
{
    public class EngineParameters : INotifyPropertyChanged, IEquatable<EngineParameters>
    {

        private static readonly Regex EngineParametersRegex =
            new Regex(
                @"\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)\s*_?\s*(EXACTLY_K_TO_WIN|K_OR_MORE_TO_WIN)?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private ulong _k;

        private ulong _m;
        private ulong _n;
        private ulong _p;
        private ulong _q;
        private WinCondition _winCondition;

        public EngineParameters()
        {
        }

        public EngineParameters(ulong m, ulong n, ulong k, ulong p, ulong q, WinCondition w)
        {
            M = m;
            N = n;
            K = k;
            P = p;
            Q = q;
            WinCondition = w;
        }

        public ulong M
        {
            get { return _m; }
            set
            {
                _m = value;
                OnPropertyChanged();
            }
        }

        public ulong N
        {
            get { return _n; }
            set
            {
                _n = value;
                OnPropertyChanged();
            }
        }

        public ulong K
        {
            get { return _k; }
            set
            {
                _k = value;
                OnPropertyChanged();
            }
        }

        public ulong P
        {
            get { return _p; }
            set
            {
                _p = value;
                OnPropertyChanged();
            }
        }

        public ulong Q
        {
            get { return _q; }
            set
            {
                _q = value;
                OnPropertyChanged();
            }
        }

        public WinCondition WinCondition
        {
            get { return _winCondition; }
            set
            {
                _winCondition = value;
                OnPropertyChanged();
            }
        }

        public bool Equals(EngineParameters other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _m == other._m && _n == other._n && _k == other._k && _p == other._p && _q == other._q &&
                   _winCondition == other._winCondition;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((EngineParameters) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _m.GetHashCode();
                hashCode = (hashCode*397) ^ _n.GetHashCode();
                hashCode = (hashCode*397) ^ _k.GetHashCode();
                hashCode = (hashCode*397) ^ _p.GetHashCode();
                hashCode = (hashCode*397) ^ _q.GetHashCode();
                hashCode = (hashCode*397) ^ (int) _winCondition;
                return hashCode;
            }
        }

        public static bool operator ==(EngineParameters left, EngineParameters right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EngineParameters left, EngineParameters right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            var ret = $@"({M},{N},{K},{P},{Q})GameEngine";
            if (Math.Max(M, N) > K)
            {
                ret += $"_{WinCondition}";
            }
            return ret;
        }

        public static bool TryParse(string str, out EngineParameters engineParameters)
        {
            engineParameters = new EngineParameters();
            var match = EngineParametersRegex.Match(str);
            if (!match.Success)
                return false;

            engineParameters.M = ulong.Parse(match.Groups[1].Value);
            engineParameters.N = ulong.Parse(match.Groups[2].Value);
            engineParameters.K = ulong.Parse(match.Groups[3].Value);
            engineParameters.P = ulong.Parse(match.Groups[4].Value);
            engineParameters.Q = ulong.Parse(match.Groups[5].Value);

            engineParameters.WinCondition =
                match.Groups[6].Value.ToLowerInvariant().Contains("EXACTLY_K_TO_WIN".ToLowerInvariant())
                    ? WinCondition.EXACTLY_K_TO_WIN
                    : WinCondition.K_OR_MORE_TO_WIN;
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}