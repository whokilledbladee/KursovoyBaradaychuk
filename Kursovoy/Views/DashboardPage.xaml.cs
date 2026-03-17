using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kursovoy.Views
{
    public partial class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadData();
        }

        // Сделайте метод публичным
        public void LoadData()
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Загрузка данных для дашборда");

                decimal totalIncome = DatabaseManager.GetTotalIncome();
                decimal totalExpenses = DatabaseManager.GetTotalExpenses();
                var transactions = DatabaseManager.GetAllTransactions();

                Console.WriteLine($"Данные получены: Доходы={totalIncome}, Расходы={totalExpenses}");

                TotalIncomeText.Text = $"{totalIncome:N0} р.";
                TotalExpensesText.Text = $"{totalExpenses:N0} р.";

                decimal balance = totalIncome - totalExpenses;
                BalanceText.Text = $"{balance:N0} р.";

                BalanceText.Foreground = balance >= 0
                    ? new SolidColorBrush(Color.FromRgb(39, 174, 96))
                    : new SolidColorBrush(Color.FromRgb(231, 76, 60));

                // Показываем количество транзакций
                TotalTransactionsText.Text = transactions.Count.ToString();

                Console.WriteLine("Генерация данных для диаграмм...");
                GenerateChartData();

                Console.WriteLine($"Данные загружены успешно: Доходы={totalIncome}, Расходы={totalExpenses}, Баланс={balance}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА загрузки данных: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateChartData()
        {
            try
            {
                var transactions = DatabaseManager.GetAllTransactions();

                var incomeGroups = transactions
                    .Where(t => t.Type == OperationType.Income)
                    .GroupBy(t => t.Category)
                    .Select(g => new ChartData
                    {
                        Category = g.Key,
                        Amount = g.Sum(t => t.Amount),
                        Color = GetCategoryColor(g.Key)
                    })
                    .OrderByDescending(d => d.Amount)
                    .ToList();

                var expenseGroups = transactions
                    .Where(t => t.Type == OperationType.Expense)
                    .GroupBy(t => t.Category)
                    .Select(g => new ChartData
                    {
                        Category = g.Key,
                        Amount = g.Sum(t => t.Amount),
                        Color = GetCategoryColor(g.Key)
                    })
                    .OrderByDescending(d => d.Amount)
                    .ToList();

                decimal totalIncome = incomeGroups.Sum(d => d.Amount);
                foreach (var data in incomeGroups)
                {
                    data.Percentage = totalIncome > 0 ? (double)(data.Amount / totalIncome * 100) : 0;
                }

                decimal totalExpenses = expenseGroups.Sum(d => d.Amount);
                foreach (var data in expenseGroups)
                {
                    data.Percentage = totalExpenses > 0 ? (double)(data.Amount / totalExpenses * 100) : 0;
                }

                // Устанавливаем ширину столбцов
                double maxIncomeBarWidth = 150;
                double maxExpenseBarWidth = 150;

                foreach (var data in incomeGroups)
                {
                    data.BarWidth = Math.Max(data.Percentage * 2, 60);
                    data.BarWidth = Math.Min(data.BarWidth, maxIncomeBarWidth);
                }

                foreach (var data in expenseGroups)
                {
                    data.BarWidth = Math.Max(data.Percentage * 2, 60);
                    data.BarWidth = Math.Min(data.BarWidth, maxExpenseBarWidth);
                }

                IncomeChart.ItemsSource = incomeGroups;
                ExpenseChart.ItemsSource = expenseGroups;

                // Создаем легенду
                CreateLegend(IncomeLegend, incomeGroups);
                CreateLegend(ExpenseLegend, expenseGroups);

                Console.WriteLine($"Сгенерированы данные: Доходы={incomeGroups.Count} категорий, Расходы={expenseGroups.Count} категорий");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка генерации данных для диаграмм: {ex.Message}");
            }
        }

        private void CreateLegend(WrapPanel legendPanel, List<ChartData> chartData)
        {
            legendPanel.Children.Clear();

            foreach (var data in chartData)
            {
                var legendItem = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.Color)),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(12, 10, 12, 10),
                    Margin = new Thickness(8, 5, 8, 5),
                    MinWidth = 180
                };

                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;

                // Цветной квадратик
                var colorBox = new Border
                {
                    Width = 12,
                    Height = 12,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(data.Color)),
                    CornerRadius = new CornerRadius(2),
                    Margin = new Thickness(0, 0, 10, 0)
                };

                // Текст легенды
                var textBlock = new TextBlock
                {
                    Text = $"{data.Category}: {data.DisplayAmount} ({data.DisplayPercentage})",
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                };

                stackPanel.Children.Add(colorBox);
                stackPanel.Children.Add(textBlock);
                legendItem.Child = stackPanel;
                legendPanel.Children.Add(legendItem);
            }
        }

        private string GetCategoryColor(string category)
        {
            var colors = new Dictionary<string, string>
            {
                { "Зарплата", "#27AE60" },
                { "Премия", "#2ECC71" },
                { "Фриланс", "#3498DB" },
                { "Продукты", "#E74C3C" },
                { "Транспорт", "#E67E22" },
                { "Кафе и рестораны", "#D35400" },
                { "Коммунальные услуги", "#16A085" },
                { "Инвестиции", "#9B59B6" },
                { "Подарки", "#1ABC9C" },
                { "Одежда", "#F39C12" },
                { "Развлечения", "#F1C40F" },
                { "Образование", "#1ABC9C" },
                { "Здоровье", "#E74C3C" },
                { "Комунальные", "#2980B9" },
                { "Прочее", "#95A5A6" }
            };

            return colors.TryGetValue(category, out var color)
                ? color
                : GetRandomColor();
        }

        private string GetRandomColor()
        {
            var colors = new[]
            {
                "#E74C3C", "#3498DB", "#2ECC71", "#F39C12", "#9B59B6",
                "#1ABC9C", "#D35400", "#C0392B", "#8E44AD", "#27AE60"
            };
            Random rand = new Random();
            return colors[rand.Next(colors.Length)];
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var report = ReportManager.GenerateReport();

                int reportId = ReportManager.SaveReport(report);

                if (reportId > 0)
                {
                    MessageBox.Show($"✅ Отчет успешно сформирован и сохранен!\n\n" +
                                  $"📋 Название: {report.Title}\n" +
                                  $"💰 Доходы: {report.TotalIncome:N0} р.\n" +
                                  $"💸 Расходы: {report.TotalExpenses:N0} р.\n" +
                                  $"⚖️ Баланс: {report.Balance:N0} р.",
                                  "Отчет создан",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadData();
                }
                else
                {
                    MessageBox.Show("❌ Не удалось сохранить отчет в базу данных", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания отчета: {ex.Message}");

                string errorMessage = $"Ошибка создания отчета:\n{ex.Message}";

                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nДетали:\n{ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка создания отчета",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class ChartData
        {
            public string Category { get; set; }
            public decimal Amount { get; set; }
            public double Percentage { get; set; }
            public string Color { get; set; }
            public double BarWidth { get; set; }

            public string DisplayAmount => $"{Amount:N0} р.";
            public string DisplayPercentage => $"{Percentage:F1}%";
        }
    }
}