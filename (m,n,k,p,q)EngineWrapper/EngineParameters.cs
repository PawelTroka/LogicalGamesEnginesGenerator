using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace _m_n_k_p_q_EngineWrapper
{
    public class EngineParameters : INotifyPropertyChanged
    {
        private static readonly Regex _engineInfoRegex = new Regex(@"\s*\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\)\s*_?\s*(EXACTLY_K_TO_WIN|K_OR_MORE_TO_WIN)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);


        public static bool TryParse(string str, out EngineParameters engineParameters)
        {
            engineParameters = new EngineParameters();
            var match = _engineInfoRegex.Match(str);
            if (!match.Success)
                return false;

            engineParameters.M = ulong.Parse(match.Groups[1].Value);
            engineParameters.N = ulong.Parse(match.Groups[2].Value);
            engineParameters.K = ulong.Parse(match.Groups[3].Value);
            engineParameters.P = ulong.Parse(match.Groups[4].Value);
            engineParameters.Q = ulong.Parse(match.Groups[5].Value);

            engineParameters.WinCondition =
                (match.Groups[6].Value.ToLowerInvariant().Contains("EXACTLY_K_TO_WIN".ToLowerInvariant()))
                    ? WinCondition.EXACTLY_K_TO_WIN
                    : WinCondition.K_OR_MORE_TO_WIN;
            return true;
        }

        private ulong _m;
        private ulong _n;
        private ulong _k;
        private ulong _p;
        private ulong _q;
        private WinCondition _winCondition;

        public ulong M
        {
            get { return _m; }
            set { _m = value; OnPropertyChanged();}
        }

        public ulong N
        {
            get { return _n; }
            set { _n = value;OnPropertyChanged(); }
        }

        public ulong K
        {
            get { return _k; }
            set { _k = value;OnPropertyChanged(); }
        }

        public ulong P
        {
            get { return _p; }
            set { _p = value; OnPropertyChanged(); }
        }

        public ulong Q
        {
            get { return _q; }
            set { _q = value; OnPropertyChanged(); }
        }

        public WinCondition WinCondition
        {
            get { return _winCondition; }
            set { _winCondition = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}