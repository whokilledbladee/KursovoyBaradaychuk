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
    /// Логика взаимодействия для CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : UserControl
    {
        public CategoriesPage()
        {
            InitializeComponent();
            LoadCategories();
        }
        private void LoadCategories()
        {
            var incomeCategories = new List<Category>
            {
                new Category { Name = "Зарплата", Icon = "💼" },
                new Category { Name = "Премия", Icon = "🎁" },
                new Category { Name = "Фриланс", Icon = "💻" },
                new Category { Name = "Инвестиции", Icon = "📈" },
                new Category { Name = "Проценты по вкладу", Icon = "🏦" },
                new Category { Name = "Подарки", Icon = "🎁" },
                new Category { Name = "Возврат долга", Icon = "↩️" },
                new Category { Name = "Прочие доходы", Icon = "📥" }
            };

            var expenseCategories = new List<Category>
            {
                new Category { Name = "Продукты", Icon = "🛒" },
                new Category { Name = "Кафе и рестораны", Icon = "🍽️" },
                new Category { Name = "Транспорт", Icon = "🚗" },
                new Category { Name = "Общественный транспорт", Icon = "🚌" },
                new Category { Name = "Такси", Icon = "🚕" },
                new Category { Name = "Коммунальные услуги", Icon = "🏠" },
                new Category { Name = "Интернет и связь", Icon = "📱" },
                new Category { Name = "Одежда и обувь", Icon = "👕" },
                new Category { Name = "Здоровье", Icon = "🏥" },
                new Category { Name = "Аптека", Icon = "💊" },
                new Category { Name = "Развлечения", Icon = "🎬" },
                new Category { Name = "Кино", Icon = "🎭" },
                new Category { Name = "Путешествия", Icon = "✈️" },
                new Category { Name = "Образование", Icon = "📚" },
                new Category { Name = "Книги", Icon = "📖" },
                new Category { Name = "Подарки", Icon = "🎁" },
                new Category { Name = "Дом и ремонт", Icon = "🔧" },
                new Category { Name = "Техника", Icon = "💻" },
                new Category { Name = "Красота", Icon = "💄" },
                new Category { Name = "Спорт", Icon = "⚽" },
                new Category { Name = "Автомобиль", Icon = "🚙" },
                new Category { Name = "Налоги", Icon = "📊" },
                new Category { Name = "Страхование", Icon = "🛡️" },
                new Category { Name = "Прочие расходы", Icon = "📤" }
            };

            IncomeCategoriesList.ItemsSource = incomeCategories;
            ExpenseCategoriesList.ItemsSource = expenseCategories;
        }
    }
}
