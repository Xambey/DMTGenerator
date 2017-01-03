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
using Novacode;

namespace DMTGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : System.Windows.Window
    {
        private StringBuilder buffer;

        private string template_wname = @"..\..\Resources\template_word.docx";

        public MainWindow()
        {
            InitializeComponent();
            checkBox.IsEnabled = false;
            checkBox_Copy.IsEnabled = false;
        }

        private void GenerateTickets(int count)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Документ word(*docx)|*.docx";
            dialog.OverwritePrompt = true;
            
            if (dialog.ShowDialog() == true) {
                textBox.Text = dialog.FileName;
            }
            else
            {
                MessageBox.Show("Не удалось выбрать место и название файла для сохранения!");
                return;
            }

            //проверка существования шаблона
            if(!File.Exists(template_wname))
            { 
                MessageBox.Show("Файл шаблона template_word.docx не найден!");
                return;
            }

            var doc_output = DocX.Create(dialog.FileName);

            int c = 1;

            Random r = new Random();
            while (c <= count)
            {
                List<List<double>> table = new List<List<double>>()
                {
                     new List<double>() {r.Next(-15,15), r.Next(-15, 15), r.Next(-15, 15) },
                     new List<double>() { r.Next(-15, 15), r.Next(-15, 15), r.Next(-15, 15) },
                     new List<double>() { r.Next(-15, 15), r.Next(-15, 15), r.Next(-15, 15) },
                     new List<double>() { r.Next(-15, 15), r.Next(-15, 15), r.Next(-15, 15) },
                     new List<double>() { 0, r.Next(-15, 15), r.Next(-15, 15) }
                };


                List<double> result = new List<double>();
                for (int i = 0; i < table[i].Count - 1; i++)
                    result.Add(new double());

                List<List<double>> table_result = new List<List<double>>();

                Function f = r.Next(10) % 1 == 0 ? Function.Max : Function.Min;

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

                if (table[4][1] != (int)table[4][1] || table[4][2] != (int)table[4][2])
                    continue;

                string sign = f == Function.Max ? ">=" : "<=";

                doc_output.InsertParagraph("Билет №" + count.ToString())
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14)
                    .Alignment = Alignment.center;
                doc_output.InsertParagraph("Решить симплексным методом, с использованием симплексной таблицы при следующих условиях: ")
                    .SpacingAfter(0)
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph(string.Format(" {0}x1+({1}x2) <= {2}", table[0][1], table[0][2], table[0][0]))
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph(string.Format(" {0}x1+({1}x2) <= {2}", table[1][1], table[1][2], table[1][0]))
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph(string.Format(" {0}x1+({1}x2) <= {2}", table[2][1], table[2][2], table[2][0]))
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph(string.Format(" {0}x1+({1}x2) <= {2}", table[3][1], table[3][2], table[3][0]))
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph(" x1x2 " + sign + " 0")
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph(string.Format("F(x1,x2) = {0}x1 + ({1}x2) -> {2}", table[4][1], table[4][2], sign == ">=" ? "max" : "min"))
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph()
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);

                count--;
            }

            doc_output.Save();
        }

        private void SearchReplace(int index, string text, string ntext)
        {
            
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
