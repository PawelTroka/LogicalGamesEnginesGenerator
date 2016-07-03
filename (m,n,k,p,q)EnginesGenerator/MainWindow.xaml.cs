using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using _m_n_k_p_q_EngineWrapper;

namespace m_n_k_p_q_EnginesGenerator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EnginesGenerator _generator;
        private readonly Action<string> _callback;
        public MainWindow()
        {
            InitializeComponent();

            _callback = async s => await Dispatcher.InvokeAsync(() => outputTextBox.AppendText(s + Environment.NewLine));
            _generator = new EnginesGenerator(_callback);

            outputPathTextBox.Text = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GeneratedEngines");

            foreach (EngineScheme value in Enum.GetValues(typeof(EngineScheme)))
            {
                batchGenerationListBox.Items.Add(value.ToEngineParameters());
            }
            batchGenerationListBox.Items.Add(new EngineParameters(8, 8, 4, 2, 1, WinCondition.K_OR_MORE_TO_WIN));
            batchGenerationListBox.Items.Add(new EngineParameters(8, 8, 4, 1, 1, WinCondition.K_OR_MORE_TO_WIN));

            batchGenerationListBox.Items.Add(new EngineParameters(5, 13, 4, 2, 1, WinCondition.K_OR_MORE_TO_WIN));
            batchGenerationListBox.Items.Add(new EngineParameters(5, 13, 4, 1, 1, WinCondition.K_OR_MORE_TO_WIN));
        }

        private async void generateButton_Click(object sender, RoutedEventArgs e)
        {
            generateButton.IsEnabled = false;

            var engineParameters = GetEngineParametersFromUI();
            var compilerPath = msbuildPathTextBox.Text;
            var outputDir = outputPathTextBox.Text;
            var flags = flagsTextBox.Text;

            await Task.Run(() => _generator.GenerateEngine(compilerPath, outputDir, flags, engineParameters));

            generateButton.IsEnabled = true;
        }

        private EngineParameters GetEngineParametersFromUI()
        {
            var engineParameters = new EngineParameters
            {
                K = (ulong) kLongUpDown.Value,
                M = (ulong) mLongUpDown.Value,
                N = (ulong) nLongUpDown.Value,
                P = (ulong) pLongUpDown.Value,
                Q = (ulong) qLongUpDown.Value,
                WinCondition = (WinCondition) winingConditionComboBox.SelectedIndex
            };
            return engineParameters;
        }

        private void runGeneratedEngineButton_Click(object sender, RoutedEventArgs e)
        {
            _generator.RunEngine();
        }

        private void sendInputButton_OnClick(object sender, RoutedEventArgs e)
        {
            _generator.SendCommand(inputTextBox.Text);
        }

        private void loadEngineSchemeButton_Click(object sender, RoutedEventArgs e)
        {
            //load scheme
            var engineScheme = (EngineScheme) engineSchemeComboBox.SelectedIndex;
            var engineParameters = engineScheme.ToEngineParameters();

            kLongUpDown.Value = (long) engineParameters.K;
            mLongUpDown.Value = (long) engineParameters.M;
            nLongUpDown.Value = (long) engineParameters.N;
            pLongUpDown.Value = (long) engineParameters.P;
            qLongUpDown.Value = (long) engineParameters.Q;
            winingConditionComboBox.SelectedValue = engineParameters.WinCondition;
        }

        private void cleanOutputButton_OnClick(object sender, RoutedEventArgs e)
        {
            _generator.CleanOutput(msbuildPathTextBox.Text);
        }

        private void openEnginesDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(outputPathTextBox.Text);
        }

        private void changeEnginesOutputDirectoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog {SelectedPath = outputPathTextBox.Text, ShowNewFolderButton = true};
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputPathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void changeCompilerPathButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
                //{ CheckPathExists = true,InitialDirectory = msbuildPathTextBox.Text,  DefaultExt = "exe",FileName = msbuildPathTextBox.Text };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                msbuildPathTextBox.Text = dialog.FileName;
            }
        }

        private void AddToBatchGenerationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var engineParameters = GetEngineParametersFromUI();
            if(!batchGenerationListBox.Items.Contains(engineParameters))
                batchGenerationListBox.Items.Add(engineParameters);
        }

        private async void BatchGenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            batchGenerateButton.IsEnabled = measurePerformanceCheckBox.IsEnabled = false;

            var compilerPath = msbuildPathTextBox.Text;
            var outputDir = outputPathTextBox.Text;
            var flags = flagsTextBox.Text;
            var engineParameters = batchGenerationListBox.Items.Cast<EngineParameters>();
            var measurePerformance = measurePerformanceCheckBox.IsChecked.Value;

            var performanceDict = new Dictionary<EngineParameters,double>();

            await Task.Run(() =>
            {
                foreach (var engine in engineParameters)
                    if (measurePerformance)
                    {
                        _generator.CleanOutput(compilerPath);
                        var stw = Stopwatch.StartNew();
                        _generator.GenerateEngine(compilerPath, outputDir, flags, engine);
                        stw.Stop();
                        performanceDict.Add(engine, stw.Elapsed.TotalMilliseconds);
                    }
                    else
                        _generator.GenerateEngine(compilerPath, outputDir, flags, engine);

                if (!measurePerformance) return;
                _callback("Generation times:");
                foreach (var key in performanceDict.Keys)
                    _callback($"  {key}: {performanceDict[key]}ms");
                _callback($" average: {performanceDict.Values.Average()}ms");
            });

            batchGenerateButton.IsEnabled = measurePerformanceCheckBox.IsEnabled = true;
        }

        private void RemoveFromBatchGeneration_OnClick(object sender, RoutedEventArgs e)
        {
            if (batchGenerationListBox.SelectedItems != null && batchGenerationListBox.SelectedIndex>=0)
            {
                batchGenerationListBox.Items.RemoveAt(batchGenerationListBox.SelectedIndex);
            }
        }
    }
}