using System;
using System.Diagnostics;
using System.IO;

namespace m_n_k_p_q_EnginesGenerator
{
    class EnginesGenerator
    {
        private readonly Action<string> _callback;

        public EnginesGenerator(Action<string> callback)
        {
            _callback = callback;
            _engineProjectFullPath = Path.Combine(_currentDirectory, _engineProjectName,
                _engineProjectFullName);
        }

        private readonly string _currentDirectory =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        private const string _engineProjectName = "(m,n,k,p,q)GameEngine";

        private const string _engineProjectFullName = _engineProjectName+".vcxproj";


        private readonly string _engineProjectFullPath;

        public void GenerateEngine(string compilerPath, string flags= "/p:Configuration=Release")
        {

            //* Create your Process
            Process process = new Process
            {
                StartInfo =
                {

                    FileName = compilerPath,
                    Arguments = _engineProjectFullPath +" "+flags,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            //* Set your output and error (asynchronous) handlers
            process.OutputDataReceived += (o,e) => _callback?.Invoke(e.Data);
            process.ErrorDataReceived += (o, e) => _callback?.Invoke(e.Data);
            //* Start process and handlers
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }
}