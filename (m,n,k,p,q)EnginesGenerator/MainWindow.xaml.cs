using System;
using System.Windows;
using Microsoft.Build.Utilities;


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
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _generator.GenerateEngine(msbuildPathTextBox.Text, flagsTextBox.Text);
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
    }
}
