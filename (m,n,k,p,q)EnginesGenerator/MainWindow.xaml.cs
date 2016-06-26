using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            button2.IsEnabled = false;

            var engineParameters = new EngineParameters()
            {
                K= (ulong)kLongUpDown.Value,
                M= (ulong)mLongUpDown.Value,
                N= (ulong)nLongUpDown.Value,
                P= (ulong)pLongUpDown.Value,
                Q=(ulong)qLongUpDown.Value,
                WinCondition = (WinCondition)winingConditionComboBox.SelectedIndex,
            };
            var compilerPath = msbuildPathTextBox.Text;
            var outputDir = msbuildPathTextBox_Copy.Text;
            var flags = flagsTextBox.Text;

            await Task.Run(()=> _generator.GenerateEngine(compilerPath, outputDir, flags, engineParameters));
            button2.IsEnabled = true;
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
            switch (engineScheme)
            {
                case EngineScheme.TicTacToe:
                    kLongUpDown.Value = mLongUpDown.Value = nLongUpDown.Value = 3;
                    pLongUpDown.Value = qLongUpDown.Value = 1;
                    break;
                case EngineScheme.Connect6:
                    kLongUpDown.Value = 6;
                    mLongUpDown.Value = nLongUpDown.Value = 15;
                    pLongUpDown.Value = 2;
                    qLongUpDown.Value = 1;
                    winingConditionComboBox.SelectedValue = WinCondition.K_OR_MORE_TO_WIN;

                    break;
                case EngineScheme.StandardGomoku:
                    kLongUpDown.Value = 5;
                    mLongUpDown.Value = nLongUpDown.Value = 15;
                    pLongUpDown.Value = qLongUpDown.Value = 1;
                    winingConditionComboBox.SelectedValue = WinCondition.EXACTLY_K_TO_WIN;
                    break;
                case EngineScheme.FreeStyleGomoku:
                    kLongUpDown.Value = 5;
                    mLongUpDown.Value = nLongUpDown.Value = 15;
                    pLongUpDown.Value = qLongUpDown.Value = 1;
                    winingConditionComboBox.SelectedValue = WinCondition.K_OR_MORE_TO_WIN;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
    }
}
