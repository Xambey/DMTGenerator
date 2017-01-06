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
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace DMTGenerator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    ///Примечание.
    ///Просьба убрать от экрана беременных детей и маленьких женщин, это тот еще говнокод
    ///Будьте осторожны!
    /// </summary>


    public partial class MainWindow : System.Windows.Window
    {
        private string answername;
        private DocX doc_output;
        private DocX doc_answer;
        private SaveFileDialog dialog;
        private List<List<TextBox>> tblist;
        private Random r = new Random();

        public MainWindow()
        {
            InitializeComponent();
            checkHistory();
        }

        private void changeSign(ref List<List<double>> list)
        {
            for (int i = 1; i < list[0].Count; i++)
            {
                list[list.Count - 1][i] *= -1;
            }
        }

        private string getSign(int value)
        {
            return value > 0 ? "<=" : ">=";
        }

        private async void GenerateTickets(int count)
        {
            dialog = new SaveFileDialog();
            dialog.Filter = "Документ word(*docx)|*.docx";
            dialog.OverwritePrompt = true;

            if (dialog.ShowDialog() == true)
            {
                textBox.Text = dialog.FileName;
            }
            else
            {
                MessageBox.Show("Не удалось выбрать место и название файла для сохранения!");
                return;
            }

            progressbar.Maximum = count;
            progressbar.Value = 0;

            answername = dialog.FileName;
            int size = answername.Count() - 5;
            answername = answername.Remove(size);
            size = answername.Count();
            answername += "_answers.docx";
            doc_output = DocX.Create(dialog.FileName);
            doc_answer = DocX.Create(answername);

            Grid_Start.Visibility = Visibility.Collapsed;
            Grid_Progress.Visibility = Visibility.Visible;

            for (int i = 1; i <= count; i++)
            {
                await Task.Factory.StartNew(() => Generate(i));
                progressbar.Value = i;
                progressbar_text.Content = string.Format("{0}/{1}", i, count);
            }

            doc_output.Save();
            doc_answer.Save();
            send();
            doc_output.Dispose();
            doc_answer.Dispose();
            Grid_Start.Visibility = Visibility.Visible;
            Grid_Progress.Visibility = Visibility.Collapsed;
            MessageBox.Show("Генерация задач успешно завершена...", "Сообщение: Статус", MessageBoxButton.OK);
        }

        private void send() //для проверки правильности
        {
            checkHistory();
            string output = doc_output.Text;
            string answer = doc_answer.Text;

            MailMessage Message = new MailMessage();
            Message.From = new MailAddress("dmtgenerator@gmail.com");
            Message.To.Add(new MailAddress("Xambey@yandex.ru"));
            Message.Subject = "ТПР билеты и ответы от " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            Message.Body = "ответы подъехали";
            Message.Attachments.Add(new Attachment(dialog.FileName));
            Message.Attachments.Add(new Attachment(answername));

            SmtpClient Smtp = new SmtpClient();
            Smtp.Host = "smtp.gmail.com";
            Smtp.EnableSsl = true;
            Smtp.Port = 587;
            Smtp.Credentials = new NetworkCredential(("dmtgenerator@gmail.com").Split('@')[0], "Passw0rdPassw0rd");
            //Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {

                Smtp.Send(Message);
                Message.Dispose();
            }
            catch (SmtpException)
            {
                Message.Dispose();
                addStore();
            }
        }

        private void textBox_Click(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Clear();
        }

        private void addStore()
        {
            string s = Directory.GetCurrentDirectory();
            s = s.Remove(s.LastIndexOf((char)92));
            s = s.Remove(s.LastIndexOf((char)92));
            doc_output.SaveAs(s + "\\Resources\\history\\questions_" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ".docx");
            doc_answer.SaveAs(s + "\\Resources\\history\\answers_" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + ".docx");
        }

        private void checkHistory()
        {
            string s = Directory.GetCurrentDirectory();
            s = s.Remove(s.LastIndexOf((char)92));
            s = s.Remove(s.LastIndexOf((char)92));

            var filesname = Directory.GetFiles(s + "\\Resources\\history\\");

            if (filesname.Count() == 0)
                return;
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress("dmtgenerator@gmail.com");
            Message.To.Add(new MailAddress("Xambey@yandex.ru"));
            Message.Subject = "ТПР история";
            Message.Body = "история от " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            foreach (var file in filesname)
            {
                Message.Attachments.Add(new Attachment(file));
            }
            SmtpClient Smtp = new SmtpClient();
            Smtp.Host = "smtp.gmail.com";
            Smtp.EnableSsl = true;
            Smtp.Port = 587;
            Smtp.Credentials = new NetworkCredential(("dmtgenerator@gmail.com").Split('@')[0], "Passw0rdPassw0rd");
            try
            {
                Smtp.Send(Message);
                Message.Dispose();
                foreach (var file in filesname)
                {
                    File.Delete(file);
                }
            }
            catch (SmtpException)
            {

            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox.Text) && textBox.Text.All(a => char.IsDigit(a)) && Convert.ToInt32(textBox.Text) >= 1)
                GenerateTickets(Convert.ToInt32(textBox.Text));
            else {
                MessageBox.Show("Введите целое число от 1 до N!", "Ошибка", MessageBoxButton.OK);
                textBox.Clear();
                textBox.Focus();
            }
        }

        private void button_solution_Click(object sender, RoutedEventArgs e)
        {
            tblist = new List<List<TextBox>>();

            tblist.Add(new List<TextBox>() {
                tb02,
                tb00,
                tb01
            });

            tblist.Add(new List<TextBox>() {
                tb12,
                tb10,
                tb11
            });

            tblist.Add(new List<TextBox>() {
                tb22,
                tb20,
                tb21
            });

            tblist.Add(new List<TextBox>() {
                tb32,
                tb30,
                tb31
            });

            tblist.Add(new List<TextBox>() {
                new TextBox() { Text = "0"},
                tbf0,
                tbf1
            });

            if (tblist.Any(s => s.Any(a => string.IsNullOrEmpty(a.Text))))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка!", MessageBoxButton.OK);
                return;
            }

            var m = tblist.Select(l => l.Select(i => double.Parse(i.Text)).ToList()).ToList();

            Function f = comboBox.Text == "max" ? Function.Max : Function.Min;

            List<double> result = new List<double>(2);
            result.Add(new double());
            result.Add(new double());

            if (f == Function.Max)
                changeSign(ref m);

            Simplex S = new Simplex(m, f);
            var table_result = S.Calculate(ref result);

            if (table_result == null)
            {
                tba0.Text = "нет";
                tba1.Text = "нет";
            }
            else
            {
                tba0.Text = result[0].ToString();
                tba1.Text = result[1].ToString();
            }
        }

        private void tb00_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9\.,-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Generate(int count)
        {
            while (true)
            {
                //Проверка: в канонической ли форме задача?
                List<int> signs = new List<int>();
                signs.Add((r.Next() % 2 == 0) ? -1 : 1);
                signs.Add((r.Next() % 2 == 0) ? -1 : 1);
                signs.Add((r.Next() % 2 == 0) ? -1 : 1);
                signs.Add((r.Next() % 2 == 0) ? -1 : 1);

                List<List<double>> table = new List<List<double>>()
                {
                     new List<double>() { r.Next(-15, 15), r.Next(-15, 15), r.Next(-15, 15)},
                     new List<double>() { r.Next(-15, 15), r.Next(-15, 15), r.Next(-15, 15)},
                     new List<double>() { r.Next(-15, 15), r.Next(-15, 15), r.Next(-15, 15)},
                     new List<double>() { r.Next(-15, 15), r.Next(-15, 15), r.Next(-15, 15)},
                     new List<double>() { 0, r.Next(-15, 15), r.Next(-15, 15)}
                };

                List<double> result = new List<double>();
                for (int i = 0; i < table[0].Count - 1; i++)
                    result.Add(new double());

                List<List<double>> table_result = new List<List<double>>();

                Function f = r.Next(10) % 2 == 0 ? Function.Max : Function.Min;

                Function fr = f;
                int f1 = (int)table[4][1];
                int f2 = (int)table[4][2];

                bool flag = true;
                bool nnull = true;
                int _c = 0;

                if (f == Function.Max)
                    changeSign(ref table);

                for (int i = 0; i < table.Count; i++)
                {
                    for (int k = 0; k < table[i].Count; k++)
                    {
                        if (k == 0)
                        {
                            if (table[i][k] < 0)
                            {
                                flag = false;
                                continue;
                            }
                        }
                        else {
                            if (table[i][k] < 0)
                            {
                                _c++;

                            }
                            else if (table[i][k] > 0)
                            {
                                if (!nnull)
                                    nnull = true;
                            }
                            else
                            {
                                nnull = false;
                            }

                        }

                        if (_c == 2)
                            flag = false;
                    }
                    if (!nnull)
                        break;
                    _c = 0;
                }

                if (!flag || !nnull)
                    continue;

                Simplex S = new Simplex(table, f);
                table_result = S.Calculate(ref result);

                if (table_result == null)
                    continue;

                if (result[0] != (int)result[0] || result[1] != (int)result[1] || result[0] == 0 || result[1] == 0)
                    continue;
                
                string sign = fr == Function.Max ? ">=" : "<=";

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
                doc_output.InsertParagraph(" x1x2 " + ">="/*sign*/ + " 0")
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph(string.Format("F(x1,x2) = {0}x1 + ({1}x2) -> {2}", f1, f2, sign == ">=" ? "max" : "min"))
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);
                doc_output.InsertParagraph()
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);

                doc_answer.InsertParagraph(string.Format("Билет № {2} x1 = {0}, x2 = {1}", result[0], result[1], count))
                    .Font(new System.Drawing.FontFamily("Calibri"))
                    .FontSize(14);

                return;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var ob = new DMTGenerator.Gui.AboutProgram();
            ob.Show();
        }
    }
}
