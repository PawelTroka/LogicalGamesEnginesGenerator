using System;
using System.Collections.Generic;
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


/*
 * Temat pracy dyplomowej magisterskiej (jęz. pol.)
    Implementacja i analiza wydajnościowa generatora silników gier logicznych
* Temat pracy dyplomowej magisterskiej (jęz. ang.)
    Implementation and analysis of logical games engines generator
* Opiekun pracy
    dr hab. inż. Krzysztof Giaro
* Konsultant pracy
    mgr inż. T. Goluch
* Cel pracy
    Celem pracy jest zbadanie wydajności generatorów silników gier logicznych.
* Zadania do wykonania
    1.  Przegląd i analiza dostępnych implementacji silników popularnych gier (Go, szachy, warcaby, itp.)
    2.  Implementacja generatora silników wybranej gry,  parametryzowanego takimi wartościami jak: rozmiary planszy, liczba pól aktywnych planszy, zasady gry, liczba graczy itp.
    3.  Analiza wydajnościowo, poprawnościowa zaimplementowanego generatora silników gier
 */

//architecture plans:
//1. Entry point is this generator with GUI
//2. generator creates new native game engine based on parameters and source code in C++ from another project
//3 some GUI app like WinBoard can run generated engine
//4. Tester "ChessEnginesAnalyzer" tests generated engines for performance and correctness, also tests Generator app

namespace ChessEnginesGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
