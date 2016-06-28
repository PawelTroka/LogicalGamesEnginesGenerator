using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using _m_n_k_p_q_EngineWrapper;

namespace _m_n_k_p_q_EnginesAnalyzer
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EnginesTester _tester;

        public MainWindow()
        {
            InitializeComponent();
            _tester = new EnginesTester(new Progress<string>(s => { OutputTextBox.AppendText(s); }));

            // performanceDataGrid.Columns.Add(new DataGridTextColumn() {Header = "chuj"});
            //   performanceDataGrid.Rows


            //correctnessDataGrid.Columns.Add(new Column() {Title = "dads"});
            //correctnessDataGrid.
        }

        private void openDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(enginesDirectoryTextBox.Text);
        }

        private void changeDirectoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                SelectedPath = enginesDirectoryTextBox.Text,
                ShowNewFolderButton = true
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                enginesDirectoryTextBox.Text = dialog.SelectedPath;
            }
        }

        private async void runPerformanceTestsButton_Click(object sender, RoutedEventArgs e)
        {
            runPerformanceTestsButton.IsEnabled = false;

            var enginesDirectory = enginesDirectoryTextBox.Text;
            var iterations = iterationsLongUpDown.Value.Value;
            await Task.Run(() => _tester.RunPerformanceTests(enginesDirectory, iterations));

            runPerformanceTestsButton.IsEnabled = true;
            SetPerformanceDataGrid();
        }

        private void SetCorrectnessDataGrid()
        {
            var dt = new DataTable();
            PerformanceInformation pi;

            dt.Columns.Add(new DataColumn(null, typeof(string)) { ColumnName = "Engine" });
            dt.Columns.Add(new DataColumn(null, typeof(double))
            {
                ColumnName =
                    nameof(pi.AverageAiGetMoveExecution) +
                    $" {_tester.PerformanceResults.Values.First().AverageAiGetMoveExecution.Unit}"
            });
            dt.Columns.Add(new DataColumn(null, typeof(double))
            {
                ColumnName =
                    nameof(pi.AverageGetMovesExecution) +
                    $" {_tester.PerformanceResults.Values.First().AverageGetMovesExecution.Unit}"
            });
            dt.Columns.Add(new DataColumn(null, typeof(double))
            {
                ColumnName =
                    nameof(pi.AverageCheckWinExecution) +
                    $" {_tester.PerformanceResults.Values.First().AverageCheckWinExecution.Unit}"
            });

            foreach (var performanceInformation in _tester.PerformanceResults)
                dt.Rows.Add(performanceInformation.Key, performanceInformation.Value.AverageAiGetMoveExecution.Value,
                    performanceInformation.Value.AverageGetMovesExecution.Value,
                    performanceInformation.Value.AverageCheckWinExecution.Value);
            performanceDataGrid.ItemsSource = dt.DefaultView;
        }

        private void SetPerformanceDataGrid()
        {
            var dt = new DataTable();
            PerformanceInformation pi;

            dt.Columns.Add(new DataColumn(null, typeof(string)) {ColumnName = "Engine"});
            dt.Columns.Add(new DataColumn(null, typeof(double))
            {
                ColumnName =
                    nameof(pi.AverageAiGetMoveExecution) +
                    $" {_tester.PerformanceResults.Values.First().AverageAiGetMoveExecution.Unit}"
            });
            dt.Columns.Add(new DataColumn(null, typeof(double))
            {
                ColumnName =
                    nameof(pi.AverageGetMovesExecution) +
                    $" {_tester.PerformanceResults.Values.First().AverageGetMovesExecution.Unit}"
            });
            dt.Columns.Add(new DataColumn(null, typeof(double))
            {
                ColumnName =
                    nameof(pi.AverageCheckWinExecution) +
                    $" {_tester.PerformanceResults.Values.First().AverageCheckWinExecution.Unit}"
            });

            foreach (var performanceInformation in _tester.PerformanceResults)
                dt.Rows.Add(performanceInformation.Key, performanceInformation.Value.AverageAiGetMoveExecution.Value,
                    performanceInformation.Value.AverageGetMovesExecution.Value,
                    performanceInformation.Value.AverageCheckWinExecution.Value);
            performanceDataGrid.ItemsSource = dt.DefaultView;
        }

        private async void runCorrectnessTestsButton_Click(object sender, RoutedEventArgs e)
        {
            runCorrectnessTestsButton.IsEnabled = false;
            var enginesDirectory = enginesDirectoryTextBox.Text;
            await Task.Run(() => _tester.RunCorrectnessTests(enginesDirectory));

            runCorrectnessTestsButton.IsEnabled = true;
        }
    }
}