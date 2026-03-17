using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursovoy.Views
{
    public partial class ReportsPage : UserControl
    {
        public ReportsPage()
        {
            InitializeComponent();
            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                var reports = ReportManager.GetAllReports();

                if (reports.Any())
                {
                    ReportsListBox.ItemsSource = reports;
                    Console.WriteLine($"Загружено {reports.Count} отчетов");
                }
                else
                {
                    // Показываем дружественное сообщение
                    ReportsListBox.ItemsSource = new List<string>
            {
                "📭 Нет сохраненных отчетов",
                "Чтобы создать отчет:",
                "1. Перейдите на вкладку 'Диаграммы'",
                "2. Нажмите '📋 Сформировать отчет'"
            };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки отчетов: {ex.Message}");

                // Показываем информацию пользователю
                ReportsListBox.ItemsSource = new List<string>
        {
            "⚠️ Не удалось загрузить отчеты",
            "Причина: " + ex.Message,
            "",
            "Попробуйте создать новый отчет"
        };
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadReports();
        }

        private void DeleteReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsListBox.SelectedItem is Report selectedReport)
            {
                var result = MessageBox.Show($"Удалить отчет '{selectedReport.Title}'?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = ReportManager.DeleteReport(selectedReport.Id);

                    if (success)
                    {
                        MessageBox.Show("Отчет успешно удален", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadReports();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить отчет", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите отчет для удаления", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}