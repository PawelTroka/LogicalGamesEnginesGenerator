using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Build.Utilities;
using Microsoft.Practices.Unity;


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

            _generator.GenerateEngine(msbuildPathTextBox.Text, flagsTextBox.Text, engineParameters);
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
                    winingConditionComboBox.SelectedValue = WinCondition.KOrMore;

                    break;
                case EngineScheme.StandardGomoku:
                    kLongUpDown.Value = 5;
                    mLongUpDown.Value = nLongUpDown.Value = 15;
                    pLongUpDown.Value = qLongUpDown.Value = 1;
                    winingConditionComboBox.SelectedValue = WinCondition.ExactlyK;
                    break;
                case EngineScheme.FreeStyleGomoku:
                    kLongUpDown.Value = 5;
                    mLongUpDown.Value = nLongUpDown.Value = 15;
                    pLongUpDown.Value = qLongUpDown.Value = 1;
                    winingConditionComboBox.SelectedValue = WinCondition.KOrMore;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
