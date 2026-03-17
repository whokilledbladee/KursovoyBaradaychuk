using System;
using System.Windows;

namespace Kursovoy
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Инициализируем базу данных
                DatabaseManager.InitializeDatabase();

                Console.WriteLine("Приложение успешно запущено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске приложения: {ex.Message}");

                // Показываем пользователю, но продолжаем работу
                MessageBox.Show($"Внимание: {ex.Message}\n\nПриложение будет работать в ограниченном режиме.",
                              "Предупреждение",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }



    }


}