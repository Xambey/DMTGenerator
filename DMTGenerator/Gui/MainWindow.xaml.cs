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
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;

namespace DMTGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : System.Windows.Window
    {
        private StringBuilder buffer;
        private Word.Find findobject;
        private Word.Range rng;

        public MainWindow()
        {
            InitializeComponent();
            checkBox.IsEnabled = false;
            checkBox_Copy.IsEnabled = false;
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

            //проверка существования шаблона
            stream = File.OpenRead("../Resources/template_word.docx");

            if (stream == null)
            {
                MessageBox.Show("Файл шаблона template_word.docx не найден!");
                return;
            }
            else
            {
                using (StreamWriter nstream = new System.IO.StreamWriter(dialog.FileName, false))
                {
                    nstream.WriteLine("");
                    nstream.Close();
                }
            }
            stream.Close();

            while (count != 0)
            {
                Random r = new Random();
                List<List<double>> table = new List<List<double>>()
                {
                     new List<double>() {(int)r.Next(-15,15), (int)r.Next(-15, 15), (int)r.Next(-15, 15) },
                     new List<double>() { (int)r.Next(-15, 15), (int)r.Next(-15, 15), (int)r.Next(-15, 15) },
                     new List<double>() { (int)r.Next(-15, 15), (int)r.Next(-15, 15), (int)r.Next(-15, 15) },
                     new List<double>() { (int)r.Next(-15, 15), (int)r.Next(-15, 15), (int)r.Next(-15, 15) },
                     new List<double>() { 0, (int)r.Next(-15, 15), (int)r.Next(-15, 15) }
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
                

                Word.Application word = new Microsoft.Office.Interop.Word.Application();
                Word.Document document = word.Documents.Open("../Resources/template_word.docx");

                if(document == null)
                {
                    MessageBox.Show("Файл шаблона template_word.docx не найден!");
                    return;
                }

                word.Application.Visible = false;

               // findobject = word.Application.Selection.Find;
                rng = document.Content;

                count--;
            }
        }

        private void SearchReplace(int index, string text, string ntext)
        {
            //findobject.ClearFormatting();
            //findobject.Text = text;
            //findobject.Replacement.ClearFormatting();
            //findobject.Replacement.Text = ntext;

            //var replace = Word.WdReplace.wdReplaceOne;
            //findobject.Execute(Replace: replace);

            rng.Find.ClearFormatting();
            rng.Find.Forward = true;
            rng.Find.Text = text;

            rng.Find.Replacement.ClearFormatting();
            rng.Find.Replacement.Text = ntext;

            object replace = Word.WdReplace.wdReplaceOne;

            int i = 0;

            rng.Find.Execute();

            while(rng.Find.Found)
            {
                if (index == i)
                    rng.Find.Execute(Replace: replace);
                i++;
                rng.Find.Execute();
            }
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
