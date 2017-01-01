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
using System.IO;
using Microsoft.Win32;

namespace DMTGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private StringBuilder buffer;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateTickets(int count)
        {
            Stream stream;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Документ word(*.doc;*docx)|.doc;*.docx";
            dialog.CheckFileExists = true;
            dialog.OverwritePrompt = true;

            if (dialog.ShowDialog() == true) {
                textBox.Text = dialog.FileName;
            }
            else
            {
                MessageBox.Show("Не удалось выбрать место и название файла для сохранения!");
                return;
            }
             
            while (count != 0)
            {
                Random r = new Random();
                List<List<double>> table = new List<List<double>>()
                {
                     new List<double>() {(int)r.Next(-15,40), (int)r.Next(-15, 40), (int)r.Next(-15,40)},
                     new List<double>() { (int)r.Next(-15, 40), (int)r.Next(-15, 40), (int)r.Next(-15,40)},
                     new List<double>() { (int)r.Next(-15, 40), (int)r.Next(-15, 40), (int)r.Next(-15,40)},
                     new List<double>() { (int)r.Next(-15, 40), (int)r.Next(-15, 40), (int)r.Next(-15,40)},
                     new List<double>() { 0, (int)r.Next(-15, 40), (int)r.Next(-15, 40) }
                };

                List<double> result = new List<double>();
                for (int i = 0; i < table[i].Count - 1; i++)
                    result.Add(new double());

                List<List<double>> table_result = new List<List<double>>();

                Function f = r.Next(1)  == 1 ? Function.Max : Function.Min;

                Simplex S = new Simplex(table, f);
                table_result = S.Calculate(ref result);

                double func = 0;
                int j = 0;
                for (int i = 1; i < table[i].Count; i++, j++)
                {
                    func += result[j] * table[table.Count - 1][i];
                }


                switch (f)
                {
                    case Function.Min:
                        if (!(func <= 0))
                            continue;
                        break;
                    case Function.Max:
                        if (!(func >= 0))
                            continue;
                        break;
                    default:
                        MessageBox.Show("Стремление функции неизвестно!");
                        return;
                }



                //Console.OutputEncoding = Encoding.Default;
                //Console.WriteLine("Решенная симплекс-таблица:");

                //foreach (List<double> item in table_result)
                //{
                //    foreach (var t in item)
                //    {
                //        Console.Write(t);
                //        Console.Write(" ");
                //    }
                //    Console.WriteLine();
                //}

                //Console.WriteLine();
                //Console.WriteLine("Решение:");
                //Console.WriteLine("X[1] = " + result[0]);
                //Console.WriteLine("X[2] = " + result[1]);
                //Console.ReadLine(); 

                string str = "";

                str += "\t\t\t Задача №" + count.ToString() + "\n"; 

                count--;
            }

            if((stream = dialog.OpenFile()) == null)
            {
                MessageBox.Show("Файл не был создан!");
                return;
            }



            stream.Close();
        }

        

        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox.Clear();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text.All(a => char.IsDigit(a)) && Convert.ToInt32(textBox.Text) >= 1)
                GenerateTickets(Convert.ToInt32(textBox.Text));
            else {
                MessageBox.Show("Введите целое число от 1 до N!", "Ошибка", MessageBoxButton.OK);
                textBox.Clear();
                textBox.Focus();
            }
        }
    }
}
