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

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(msbuildPathTextBox_Copy.Text);
        }

        private void Button1_Copy_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog() { SelectedPath = msbuildPathTextBox_Copy.Text, ShowNewFolderButton = true };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                msbuildPathTextBox_Copy.Text = dialog.SelectedPath;
            }

        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;

            var txt = msbuildPathTextBox_Copy.Text;
            var val = iterationsLongUpDown.Value.Value;
            await _tester.RunPerformanceTests(txt, val);

            button.IsEnabled = true;
        }
    }



}
