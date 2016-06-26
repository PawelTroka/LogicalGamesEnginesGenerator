using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _m_n_k_p_q_EnginesAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EnginesTester _tester;
        public MainWindow()
        {
            InitializeComponent();
            _tester = new EnginesTester(new Progress<string>((s) =>
            {
                this.OutputTextBox.AppendText(s);
            }));
        }

        private void openDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(enginesDirectoryTextBox.Text);
        }

        private void changeDirectoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = enginesDirectoryTextBox.Text, ShowNewFolderButton = true };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                enginesDirectoryTextBox.Text = dialog.SelectedPath;
            }

        }

        private async void runPerformanceTestsButton_Click(object sender, RoutedEventArgs e)
        {
            runPerformanceTestsButton.IsEnabled = false;

            var txt = enginesDirectoryTextBox.Text;
            var val = iterationsLongUpDown.Value.Value;
            await Task.Run(()=> _tester.RunPerformanceTests(txt, val));

            runPerformanceTestsButton.IsEnabled = true;
        }

        private async void runCorrectnessTestsButton_Click(object sender, RoutedEventArgs e)
        {

            runCorrectnessTestsButton.IsEnabled = false;
            var tst = enginesDirectoryTextBox.Text;
            await Task.Run(()=> _tester.RunCorrectnessTests(tst));

            runCorrectnessTestsButton.IsEnabled = true;

        }
    }



}
