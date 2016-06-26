using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Build.Utilities;
using Microsoft.Practices.Unity;
using _m_n_k_p_q_EngineWrapper;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace m_n_k_p_q_EnginesGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private EnginesGenerator _generator;



        public MainWindow()
        {
            InitializeComponent();
            _generator = new EnginesGenerator(async (s) => await Dispatcher.InvokeAsync(() => outputTextBox.AppendText(s + Environment.NewLine)));
            msbuildPathTextBox_Copy.Text = Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),"GeneratedEngines");

            foreach (EngineScheme value in Enum.GetValues(typeof(EngineScheme)))
            {
                batchGenerationListBox.Items.Add(value.ToEngineParameters());
            }
            batchGenerationListBox.Items.Add(new EngineParameters(8,8,4,2,1,WinCondition.K_OR_MORE_TO_WIN));
            batchGenerationListBox.Items.Add(new EngineParameters(8, 8, 4, 1, 1, WinCondition.K_OR_MORE_TO_WIN));

            batchGenerationListBox.Items.Add(new EngineParameters(5, 13, 4, 2, 1, WinCondition.K_OR_MORE_TO_WIN));
            batchGenerationListBox.Items.Add(new EngineParameters(5, 13, 4, 1, 1, WinCondition.K_OR_MORE_TO_WIN));
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            button2.IsEnabled = false;

            var engineParameters = GetEngineParametersFromUI();
            var compilerPath = msbuildPathTextBox.Text;
            var outputDir = msbuildPathTextBox_Copy.Text;
            var flags = flagsTextBox.Text;

            await Task.Run(()=> _generator.GenerateEngine(compilerPath, outputDir, flags, engineParameters));
            button2.IsEnabled = true;
        }

        private EngineParameters GetEngineParametersFromUI()
        {
            var engineParameters = new EngineParameters()
            {
                K = (ulong) kLongUpDown.Value,
                M = (ulong) mLongUpDown.Value,
                N = (ulong) nLongUpDown.Value,
                P = (ulong) pLongUpDown.Value,
                Q = (ulong) qLongUpDown.Value,
                WinCondition = (WinCondition) winingConditionComboBox.SelectedIndex,
            };
            return engineParameters;
        }

        private void button2_Copy_Click(object sender, RoutedEventArgs e)
        {
           // _generator.GenerateEngine(msbuildPathTextBox.Text, flagsTextBox.Text);
            _generator.RunEngine();
        }

        private void Button2_Copy1_OnClick(object sender, RoutedEventArgs e)
        {
            _generator.SendCommand(inputTextBox.Text);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //load scheme
            var engineScheme = (EngineScheme) comboBox1.SelectedIndex;
            var engineParameters = engineScheme.ToEngineParameters();

            kLongUpDown.Value = (long)engineParameters.K;
            mLongUpDown.Value = (long)engineParameters.M;
            nLongUpDown.Value = (long)engineParameters.N;
            pLongUpDown.Value = (long)engineParameters.P;
            qLongUpDown.Value = (long)engineParameters.Q;
            winingConditionComboBox.SelectedValue = engineParameters.WinCondition;
        }

        private void Button2_Copy2_OnClick(object sender, RoutedEventArgs e)
        {
            _generator.CleanOutput(msbuildPathTextBox.Text);
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(msbuildPathTextBox_Copy.Text);
        }

        private void Button1_Copy_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog() {SelectedPath = msbuildPathTextBox_Copy.Text,ShowNewFolderButton = true};
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                msbuildPathTextBox_Copy.Text = dialog.SelectedPath;
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();//{ CheckPathExists = true,InitialDirectory = msbuildPathTextBox.Text,  DefaultExt = "exe",FileName = msbuildPathTextBox.Text };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                msbuildPathTextBox.Text = dialog.FileName;
            }
        }

        private void AddToBatchGenerationButton_OnClick(object sender, RoutedEventArgs e)
        {
            var engineParameters = GetEngineParametersFromUI();
            batchGenerationListBox.Items.Add(engineParameters);
        }

        private async void BatchGenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            batchGenerateButton.IsEnabled = false;

            var compilerPath = msbuildPathTextBox.Text;
            var outputDir = msbuildPathTextBox_Copy.Text;
            var flags = flagsTextBox.Text;
            var engineParameters = batchGenerationListBox.Items.Cast<EngineParameters>();

            await Task.Run(() =>
            {
                foreach (var engineParameterse in engineParameters)
                {
                    _generator.GenerateEngine(compilerPath, outputDir, flags, engineParameterse);
                }
            });


            batchGenerateButton.IsEnabled = true;

        }

        private void RemoveFromBatchGeneration_OnClick(object sender, RoutedEventArgs e)
        {
            if (batchGenerationListBox.SelectedItems != null)
            {
                batchGenerationListBox.Items.RemoveAt(batchGenerationListBox.SelectedIndex);
            }
        }
    }
}
