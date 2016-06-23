using System;
using System.Diagnostics;

namespace m_n_k_p_q_EnginesGenerator
{
    public class ProcessInBackground
    {
        private readonly bool _input;

        public ProcessInBackground(string filename, string arguments,Action<string> callback, bool input)
        {
            _input = input;
            baseStartInfo.FileName = filename;
            baseStartInfo.Arguments = arguments;


            baseStartInfo.RedirectStandardInput= input;

            process = new Process() { StartInfo = baseStartInfo };
            
            process.OutputDataReceived += (o, e) => callback?.Invoke(e.Data);
            process.ErrorDataReceived += (o, e) => callback?.Invoke(e.Data);
            
        }

        public void Run()
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if(!_input)
                process.WaitForExit();
        }

        public void Send(string cmd)
        {
            process.StandardInput.WriteLine(cmd);
        }

        private ProcessStartInfo baseStartInfo = new ProcessStartInfo()
        {

            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,

                
        };

        private Process process;
    }
}