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

namespace Kursovoy.Views
{
    /// <summary>
    /// Логика взаимодействия для FeedbackPage.xaml
    /// </summary>
    public partial class FeedbackPage : UserControl
    {
        public FeedbackPage()
        {
            InitializeComponent();

            // Можно добавить дополнительную инициализацию при необходимости
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Страница обратной связи загружена");
        }
    }
}
