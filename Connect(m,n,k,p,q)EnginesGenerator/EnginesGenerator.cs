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
            _engineProjectDllFullPath = Path.Combine(_currentDirectory, _engineProjectName,"Release",
    _engineProjectDllName);
        }

        private readonly string _currentDirectory =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        private const string _engineProjectName = "(m,n,k,p,q)GameEngine";

        private const string _engineProjectFullName = _engineProjectName+".vcxproj";

        private const string _engineProjectDllName = _engineProjectName + ".dll";


        private readonly string _engineProjectDllFullPath;
        private readonly string _engineProjectFullPath;

        public void GenerateEngine(string compilerPath, string flags= "/p:Configuration=Release")
        {
            dllLoaded = false;
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

        private bool dllLoaded;

        public void RunEngine()
        {
            if (!dllLoaded)
            {
                EmbeddedDllClass.LoadDll(_engineProjectDllFullPath);
                dllLoaded = true;
            }
            var res = EngineWrapper.hotFunction(2);
            _callback?.Invoke($"Output from engine: {res}");
        }


        private static class EngineWrapper
        {
            [DllImport(_engineProjectDllName,  CallingConvention = CallingConvention.Cdecl)/*EntryPoint = "hotFunction", CharSet = CharSet.Unicode, SetLastError = true)*/]
            public static extern double hotFunction(double x);
        }
    }



}