using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kursovoy
{
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush _selectedButtonColor = new SolidColorBrush(Color.FromRgb(52, 152, 219));

        public MainWindow()
        {
            InitializeComponent();
            BtnDashboard_Click(null, null);
        }

        // Добавьте этот публичный метод в MainWindow
        public void RefreshDashboard()
        {
            // Если текущая страница - дашборд, обновляем его
            if (MainContentFrame.Content is Views.DashboardPage dashboardPage)
            {
                dashboardPage.LoadData();
                Console.WriteLine("Дашборд обновлен");
            }
        }

        // Обновите метод BtnDashboard_Click, чтобы использовать RefreshDashboard
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnDashboard.Background = _selectedButtonColor;

            var dashboardPage = new Views.DashboardPage();
            MainContentFrame.Content = dashboardPage;
        }

        // Обновите метод BtnTransactions_Click
        private void BtnTransactions_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnTransactions.Background = _selectedButtonColor;

            // Создаем новую страницу транзакций (она загрузит свежие данные из БД)
            var transactionsPage = new Views.TransactionsPage();
            MainContentFrame.Content = transactionsPage;
        }

        private void BtnAccounts_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnAccounts.Background = _selectedButtonColor;
            MainContentFrame.Content = new Views.AccountsPage();
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnCategories.Background = _selectedButtonColor;
            MainContentFrame.Content = new Views.CategoriesPage();
        }

        private void BtnGoals_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnGoals.Background = _selectedButtonColor;
            MainContentFrame.Content = new Views.GoalsPage();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnReports.Background = _selectedButtonColor;

            MainContentFrame.Content = new Views.ReportsPage();
        }

        // Измените метод BtnSettings на BtnFeedback
        private void BtnFeedback_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnFeedback.Background = _selectedButtonColor;
            MainContentFrame.Content = new Views.FeedbackPage();
        }

        private void ResetNavButtons()
        {
            var buttons = new[] {
                BtnDashboard, BtnTransactions, BtnAccounts, BtnCategories,
                BtnGoals, BtnReports, BtnFeedback
            };

            foreach (var button in buttons)
            {
                button.Background = Brushes.Transparent;
            }
        }
    }
}