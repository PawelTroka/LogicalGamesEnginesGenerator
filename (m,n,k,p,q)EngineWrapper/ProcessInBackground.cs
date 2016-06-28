using System;
using System.Diagnostics;

namespace _m_n_k_p_q_EngineWrapper
{
    public class ProcessInBackground
    {
        private readonly ProcessStartInfo _baseStartInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        private readonly bool _input;

        private readonly Process _process;

        public ProcessInBackground(string filename, string arguments, Action<string> callback, bool input)
        {
            _input = input;
            _baseStartInfo.FileName = filename;
            _baseStartInfo.Arguments = arguments;


            _baseStartInfo.RedirectStandardInput = input;

            _process = new Process {StartInfo = _baseStartInfo};

            _process.OutputDataReceived += (o, e) => callback?.Invoke(e.Data);
            _process.ErrorDataReceived += (o, e) => callback?.Invoke(e.Data);
        }

        public void Run()
        {
            _process.Start();

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            if (!_input)
                _process.WaitForExit();
        }

        public void Send(string cmd)
        {
            _process.StandardInput.WriteLine(cmd);
        }
    }
}