using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _m_n_k_p_q_EngineWrapper
{
    public class EngineParameters : INotifyPropertyChanged
    {
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