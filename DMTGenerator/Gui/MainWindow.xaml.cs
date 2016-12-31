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

namespace DMTGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            GenerateTickets();
        }

        private static void GenerateTickets()
        {
            List<List<double>> table = new List<List<double>>()
            {
                 new List<double>() {7, 2, 2},
                 new List<double>() {3, 9, 3},
                 new List<double>() {1, 4, 1},
                 new List<double>() {5, 6, 5},
                 new List<double>() { 0, 2, 4}
            };

            List<double> result = new List<double>();
            for (int i = 0; i < table[i].Count - 1; i++)
                result.Add(new double());

            List<List<double>> table_result = new List<List<double>>();


            Simplex S = new Simplex(table, Function.Min);
            table_result = S.Calculate(ref result);

            Console.OutputEncoding = Encoding.Default;
            Console.WriteLine("Решенная симплекс-таблица:");

            foreach (List<double> item in table_result)
            {
                foreach (var t in item)
                {
                    Console.Write(t);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Решение:");
            Console.WriteLine("X[1] = " + result[0]);
            Console.WriteLine("X[2] = " + result[1]);
            Console.ReadLine();
        }
    }
}
