using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using m_n_k_p_q_EnginesGenerator;

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
            _engineExeFullPath = Path.Combine(_currentDirectory, _engineProjectName,"x64","Release",
    _engineExeName);
        }

        private readonly string _currentDirectory =
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        private const string _engineProjectName = "(m,n,k,p,q)GameEngine";

        private const string _engineProjectFullName = _engineProjectName+".vcxproj";

        private const string _engineExeName = _engineProjectName + ".exe";


        private readonly string _engineExeFullPath;
        private readonly string _engineProjectFullPath;

        ///p:DefineConstants="M 2; N 2"
        private const string BuildBasicFlags = @" /p:Configuration=Release /p:Platform=x64 /t:Build ";

        private const string CleanBasicFlags = @" /p:Configuration=Release /p:Platform=x64 /t:Clean ";
        private ProcessInBackground _engine;

        public void GenerateEngine(string compilerPath, string outputDir, string flags, EngineParameters engineParameters)
        {
            if (outputDir.Last() == '\\')
                outputDir = outputDir.Substring(0, outputDir.Length - 1);//+= @"\";


            var assemblyName = $@"({engineParameters.M},{engineParameters.N},{engineParameters.K},{engineParameters.P},{engineParameters.Q})GameEngine";

            if (engineParameters.K < Math.Max(engineParameters.M, engineParameters.N))
                assemblyName += "_" + engineParameters.WinCondition.ToString();

            var engineParametersAsCompilerFlags = $@" /p:AdditionalPreprocessorDefinitions=""_USE_GENERATOR_DEFINES;{((engineParameters.WinCondition==WinCondition.EXACTLY_K_TO_WIN) ? "EXACTLY_K_TO_WIN;" : "")}M={engineParameters.M};N={engineParameters.N};K={engineParameters.K};Q={engineParameters.Q};P={engineParameters.P};"" /p:OutDir=""{outputDir}""  /p:AssemblyName=""{assemblyName}""";// /p:AssemblyName=""{assemblyName}""





            (new ProcessInBackground(compilerPath, _engineProjectFullPath + BuildBasicFlags + engineParametersAsCompilerFlags + flags, _callback,false)).Run();
        }

        ///p:OutDir=D:\chuj\ /p:AssemblyName=Foo

        public void CleanOutput(string compilerPath)
        {
            (new ProcessInBackground(compilerPath, _engineProjectFullPath + CleanBasicFlags, _callback, false)).Run();
        }

        // /p:AdditionalPreprocessorDefinitions="_USE_LOCAL_DEFINES"
        // /p:AdditionalPreprocessorDefinitions=""M=3;N=3;K=3;Q=1;P=1;""
        public void RunEngine()
        {
            _engine = (new ProcessInBackground(_engineExeFullPath, "", _callback, true));
            _engine.Run();
           // engine.Send("2\n");
        }

        public void SendCommand(string cmd)
        {
            _engine?.Send(cmd);
        }

        private static class EngineWrapper
        {
            [DllImport(_engineExeName,  CallingConvention = CallingConvention.Cdecl)/*EntryPoint = "hotFunction", CharSet = CharSet.Unicode, SetLastError = true)*/]
            public static extern double hotFunction(double x);
        }
    }
}