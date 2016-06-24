using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Build.Utilities;
using Microsoft.Practices.Unity;
using _m_n_k_p_q_EngineWrapper;


namespace m_n_k_p_q_EnginesGenerator
{

    public class EnumerationExtension : MarkupExtension
    {
        private Type _enumType;


        public EnumerationExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            EnumType = enumType;
        }

        public Type EnumType
        {
            get { return _enumType; }
            private set
            {
                if (_enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                _enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);

            return (
              from object enumValue in enumValues
              select new EnumerationMember
              {
                  Value = enumValue,
                  Description = GetDescription(enumValue)
              }).ToArray();
        }

        private string GetDescription(object enumValue)
        {
            var descriptionAttribute = EnumType
              .GetField(enumValue.ToString())
              .GetCustomAttributes(typeof(DescriptionAttribute), false)
              .FirstOrDefault() as DescriptionAttribute;


            return descriptionAttribute != null
              ? descriptionAttribute.Description
              : enumValue.ToString();
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public object Value { get; set; }
        }
    }

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

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var engineParameters = new EngineParameters()
            {
                K= (ulong)kLongUpDown.Value,
                M= (ulong)mLongUpDown.Value,
                N= (ulong)nLongUpDown.Value,
                P= (ulong)pLongUpDown.Value,
                Q=(ulong)qLongUpDown.Value,
                WinCondition = (WinCondition)winingConditionComboBox.SelectedIndex,
            };

            _generator.GenerateEngine(msbuildPathTextBox.Text, msbuildPathTextBox_Copy.Text, flagsTextBox.Text, engineParameters);
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
