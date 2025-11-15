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
using Kursovoy.Views;

namespace Kursovoy
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush _selectedButtonColor = new SolidColorBrush(Color.FromRgb(52, 152, 219));

        public MainWindow()
        {
            InitializeComponent();
            BtnDashboard_Click(null, null);
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnDashboard.Background = _selectedButtonColor;
            ShowPlaceholder("Диаграммы - здесь будет обзор ваших финансов");
        }

        private void BtnTransactions_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnTransactions.Background = _selectedButtonColor;
            ShowPlaceholder("Операции - управление доходами и расходами");
        }

        private void BtnAccounts_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnAccounts.Background = _selectedButtonColor;
            ShowPlaceholder("Счета - управление банковскими счетами и наличными");
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnCategories.Background = _selectedButtonColor;
            ShowPlaceholder("Категории - настройка категорий доходов и расходов");
        }

        private void BtnBudgets_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnBudgets.Background = _selectedButtonColor;
            ShowPlaceholder("Бюджеты - планирование и контроль лимитов");
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
            ShowPlaceholder("Отчеты - аналитика и визуализация данных");
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            ResetNavButtons();
            BtnSettings.Background = _selectedButtonColor;
            ShowPlaceholder("Настройки - конфигурация приложения");
        }

        private void ResetNavButtons()
        {
            var buttons = new[] {
                BtnDashboard, BtnTransactions, BtnAccounts, BtnCategories,
                BtnBudgets, BtnGoals, BtnReports, BtnSettings
            };

            foreach (var button in buttons)
            {
                button.Background = Brushes.Transparent;
            }
        }

        private void ShowPlaceholder(string content)
        {
            var textBlock = new TextBlock
            {
                Text = content,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18,
                Foreground = Brushes.Gray,
                TextAlignment = TextAlignment.Center
            };

            MainContentFrame.Content = textBlock;
        }
    }
}