﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using _m_n_k_p_q_EngineWrapper;

namespace m_n_k_p_q_EnginesGenerator
{
    internal class EnginesGenerator
    {
        private const string EngineProjectName = "(m,n,k,p,q)GameEngine";
        private const string EngineProjectFullName = EngineProjectName + ".vcxproj";

        /// p:DefineConstants="M 2; N 2"
        private const string BuildBasicFlags = @" /p:Configuration=Release /p:Platform=x64 /t:Build ";
        private const string CleanBasicFlags = @" /p:Configuration=Release /p:Platform=x64 /t:Clean ";
        private readonly Action<string> _callback;

        private readonly string _currentDirectory =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private readonly string _engineProjectFullPath;
        private ProcessInBackground _engine;
        private string _engineAssemblyName;

        private string _engineExeFullPath;

        public EnginesGenerator(Action<string> callback)
        {
            _callback = callback;
            _engineProjectFullPath = Path.Combine(_currentDirectory, EngineProjectName,
                EngineProjectFullName);
        }

        public void GenerateEngine(string compilerPath, string outputDir, string flags,
            EngineParameters engineParameters)
        {
            if (outputDir.Last() == '\\')
                outputDir = outputDir.Substring(0, outputDir.Length - 1);

            _engineAssemblyName = engineParameters.ToString();

            _engineExeFullPath = Path.Combine(outputDir, _engineAssemblyName);

            var engineParametersAsCompilerFlags =
                $@" /p:AdditionalPreprocessorDefinitions=""_USE_GENERATOR_DEFINES;{(engineParameters.WinCondition ==
                                                                                    WinCondition.EXACTLY_K_TO_WIN
                    ? "EXACTLY_K_TO_WIN;"
                    : "")}M={engineParameters.M};N={engineParameters.N};K={engineParameters.K};Q={engineParameters.Q};P={engineParameters
                        .P};"" /p:OutDir=""{outputDir}""  /p:AssemblyName=""{_engineAssemblyName}""";

            new ProcessInBackground(compilerPath,
                "\""+_engineProjectFullPath+ "\"" + BuildBasicFlags + engineParametersAsCompilerFlags + flags, _callback, false)
                .Run();
        }

        /// p:OutDir=D:\location\ /p:AssemblyName=Foo
        public void CleanOutput(string compilerPath)
        {
            new ProcessInBackground(compilerPath, "\"" + _engineProjectFullPath + "\"" + CleanBasicFlags, _callback, false).Run();
        }

        // /p:AdditionalPreprocessorDefinitions="_USE_LOCAL_DEFINES"
        // /p:AdditionalPreprocessorDefinitions=""M=3;N=3;K=3;Q=1;P=1;""
        public void RunEngine()
        {
            _engine = new ProcessInBackground(_engineExeFullPath, "", _callback, true);
            _engine.Run();
            _callback.Invoke($@"{_engineAssemblyName}.exe started");
        }

        public void SendCommand(string cmd)
        {
            _engine?.Send(cmd);
        }
    }
}