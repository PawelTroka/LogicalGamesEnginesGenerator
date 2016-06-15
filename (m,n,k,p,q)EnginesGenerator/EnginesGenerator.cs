using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

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
            _engineProjectExeFullPath = Path.Combine(_currentDirectory, _engineProjectName,"x64","Release",
    _engineProjectExeName);
        }

        private readonly string _currentDirectory =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        private const string _engineProjectName = "(m,n,k,p,q)GameEngine";

        private const string _engineProjectFullName = _engineProjectName+".vcxproj";

        private const string _engineProjectExeName = _engineProjectName + ".exe";


        private readonly string _engineProjectExeFullPath;
        private readonly string _engineProjectFullPath;
        private ProcessInBackground engine;

        public void GenerateEngine(string compilerPath, string flags= "/p:Configuration=Release /p:Platform=x64")
        {
            (new ProcessInBackground(compilerPath, _engineProjectFullPath + " " + flags, _callback,false)).Run();
        }



        public void RunEngine()
        {
            engine = (new ProcessInBackground(_engineProjectExeFullPath, "", _callback, true));
            engine.Run();
           // engine.Send("2\n");
        }

        public void SendCommand(string cmd)
        {
            engine?.Send(cmd);
        }

        private static class EngineWrapper
        {
            [DllImport(_engineProjectExeName,  CallingConvention = CallingConvention.Cdecl)/*EntryPoint = "hotFunction", CharSet = CharSet.Unicode, SetLastError = true)*/]
            public static extern double hotFunction(double x);
        }
    }

    class ProcessInBackground
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