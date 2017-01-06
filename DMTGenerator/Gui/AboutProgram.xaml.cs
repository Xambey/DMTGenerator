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
using System.Windows.Shapes;

namespace DMTGenerator.Gui
{
    /// <summary>
    /// Логика взаимодействия для AboutProgram.xaml
    /// </summary>
    public partial class AboutProgram : Window
    {
        public AboutProgram()
        {
            InitializeComponent();
            textBox.Text = "Версия 0.1.4\n" +
                "Эта программа написана при помощи яп. C#(wpf), .net framework 4.5.\n" +
                "Данная программа защищена законом" +
                "об авторском праве и/или смежными законами." +
                " Разрешается использование данной программы, копирование, публикация и распространение ее копии без отчислений авторам" +
               ", при условии, что программа не модифицируется." +
               "\nАвторы: \n Разработка:\n\tПанасенко А.В ИВБО-03-14 МТУ МИРЭА@2016" +
               "\n Дизайн:\n\tКривошея М.С ИВБО-03-14 МТУ МИРЭА@2016\n" +
               " Контакты: Xambey@yandex.ru, skype: Xambey97\n Исходники: https://github.com/Xambey/DMTGenerator \n Спасибо!";
        }
    }
}
