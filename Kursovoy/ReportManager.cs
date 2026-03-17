using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Kursovoy.Views;

namespace Kursovoy
{
    public static class ReportManager
    {
        private static readonly Dictionary<string, string> CategoryColors = new Dictionary<string, string>
        {
            {"Зарплата", "#27AE60"},
            {"Фриланс", "#2ECC71"},
            {"Подарки", "#3498DB"},
            {"Дивиденды", "#9B59B6"},
            {"Премия", "#2ECC71"},
            {"Инвестиции", "#9B59B6"},
            {"Продукты", "#E74C3C"},
            {"Транспорт", "#E67E22"},
            {"Кафе/Рестораны", "#D35400"},
            {"Кафе и рестораны", "#D35400"},
            {"Одежда", "#F39C12"},
            {"Развлечения", "#F1C40F"},
            {"Образование", "#1ABC9C"},
            {"Здоровье", "#16A085"},
            {"Коммунальные услуги", "#2980B9"},
            {"Комунальные", "#2980B9"},
            {"Прочее", "#95A5A6"}
        };

        public static Report GenerateReport()
        {
            try
            {
                Console.WriteLine("Генерация отчета...");

                var transactions = DatabaseManager.GetAllTransactions();

                var report = new Report
                {
                    Title = $"Финансовый отчет за {DateTime.Now:dd.MM.yyyy HH:mm}",
                    CreatedDate = DateTime.Now
                };

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

                report.IncomeData = incomeGroups;
                report.ExpenseData = expenseGroups;
                report.TotalIncome = totalIncome;
                report.TotalExpenses = totalExpenses;
                report.Balance = totalIncome - totalExpenses;

                Console.WriteLine($"Отчет сгенерирован: Доходы={totalIncome}р., Расходы={totalExpenses}р.");

                return report;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка генерации отчета: {ex.Message}");
                throw;
            }
        }

        public static int SaveReport(Report report)
        {
            try
            {
                Console.WriteLine($"Сохранение отчета: {report.Title}");

                // ВАЖНО: Сначала проверяем и создаем таблицу, если ее нет
                CreateReportsTableIfNotExists();

                string incomeDataXml = ConvertChartDataToXml(report.IncomeData, "Income");
                string expenseDataXml = ConvertChartDataToXml(report.ExpenseData, "Expense");

                string sql = @"
                INSERT INTO Reports (Title, CreatedDate, TotalIncome, TotalExpenses, Balance, 
                                   IncomeDataXml, ExpenseDataXml)
                OUTPUT INSERTED.Id
                VALUES (@Title, @CreatedDate, @TotalIncome, @TotalExpenses, @Balance, 
                       @IncomeDataXml, @ExpenseDataXml)";

                using (var connection = new SqlConnection(DatabaseManager.GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Title", report.Title);
                        command.Parameters.AddWithValue("@CreatedDate", report.CreatedDate);
                        command.Parameters.AddWithValue("@TotalIncome", report.TotalIncome);
                        command.Parameters.AddWithValue("@TotalExpenses", report.TotalExpenses);
                        command.Parameters.AddWithValue("@Balance", report.Balance);
                        command.Parameters.AddWithValue("@IncomeDataXml",
                            string.IsNullOrEmpty(incomeDataXml) ? (object)DBNull.Value : incomeDataXml);
                        command.Parameters.AddWithValue("@ExpenseDataXml",
                            string.IsNullOrEmpty(expenseDataXml) ? (object)DBNull.Value : expenseDataXml);

                        object result = command.ExecuteScalar();
                        int newId = result != DBNull.Value && result != null ? Convert.ToInt32(result) : 0;
                        Console.WriteLine($"Отчет сохранен в БД с ID: {newId}");
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения отчета: {ex.Message}");
                throw;
            }
        }

        // ВАЖНЫЙ МЕТОД: Создает таблицу Reports, если она не существует
        private static void CreateReportsTableIfNotExists()
        {
            try
            {
                Console.WriteLine("Проверка существования таблицы Reports...");

                string checkTableSql = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Reports')
                BEGIN
                    CREATE TABLE Reports (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Title NVARCHAR(200) NOT NULL,
                        CreatedDate DATETIME NOT NULL,
                        TotalIncome DECIMAL(18,2) NOT NULL,
                        TotalExpenses DECIMAL(18,2) NOT NULL,
                        Balance DECIMAL(18,2) NOT NULL,
                        IncomeDataXml NVARCHAR(MAX),
                        ExpenseDataXml NVARCHAR(MAX),
                        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                    )
                    PRINT 'Таблица Reports создана'
                END";

                using (var connection = new SqlConnection(DatabaseManager.GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(checkTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Таблица Reports проверена/создана");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке/создании таблицы Reports: {ex.Message}");
                throw;
            }
        }

        public static List<Report> GetAllReports()
        {
            try
            {
                Console.WriteLine("Загрузка отчетов из БД...");

                // ВАЖНО: Проверяем, существует ли таблица перед загрузкой
                if (!CheckReportsTableExists())
                {
                    Console.WriteLine("Таблица Reports не существует, возвращаем пустой список");
                    return new List<Report>();
                }

                var reports = new List<Report>();

                string sql = @"
                SELECT Id, Title, CreatedDate, TotalIncome, TotalExpenses, Balance,
                       IncomeDataXml, ExpenseDataXml
                FROM Reports
                ORDER BY CreatedDate DESC";

                using (var connection = new SqlConnection(DatabaseManager.GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var report = new Report
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                TotalIncome = reader.GetDecimal(reader.GetOrdinal("TotalIncome")),
                                TotalExpenses = reader.GetDecimal(reader.GetOrdinal("TotalExpenses")),
                                Balance = reader.GetDecimal(reader.GetOrdinal("Balance"))
                            };

                            string incomeXml = reader.IsDBNull(reader.GetOrdinal("IncomeDataXml"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("IncomeDataXml"));

                            string expenseXml = reader.IsDBNull(reader.GetOrdinal("ExpenseDataXml"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("ExpenseDataXml"));

                            report.IncomeData = ConvertXmlToChartData(incomeXml);
                            report.ExpenseData = ConvertXmlToChartData(expenseXml);

                            reports.Add(report);
                        }
                    }
                }

                Console.WriteLine($"Загружено отчетов: {reports.Count}");
                return reports;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки отчетов: {ex.Message}");
                return new List<Report>();
            }
        }

        // Проверка существования таблицы Reports
        private static bool CheckReportsTableExists()
        {
            try
            {
                string sql = @"
                SELECT CASE 
                    WHEN EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Reports') 
                    THEN 1 
                    ELSE 0 
                END";

                using (var connection = new SqlConnection(DatabaseManager.GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        object result = command.ExecuteScalar();
                        return result != DBNull.Value && Convert.ToInt32(result) == 1;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private static string ConvertChartDataToXml(List<ChartData> chartData, string dataType)
        {
            if (chartData == null || !chartData.Any())
                return string.Empty;

            var xml = new StringBuilder();
            xml.AppendLine($"<{dataType}Data>");

            foreach (var data in chartData)
            {
                xml.AppendLine($"  <Item>");
                xml.AppendLine($"    <Category>{EscapeXml(data.Category)}</Category>");
                xml.AppendLine($"    <Amount>{data.Amount}</Amount>");
                xml.AppendLine($"    <Percentage>{data.Percentage}</Percentage>");
                xml.AppendLine($"    <Color>{EscapeXml(data.Color)}</Color>");
                xml.AppendLine($"  </Item>");
            }

            xml.AppendLine($"</{dataType}Data>");
            return xml.ToString();
        }

        private static List<ChartData> ConvertXmlToChartData(string xml)
        {
            var chartData = new List<ChartData>();

            if (string.IsNullOrEmpty(xml))
                return chartData;

            try
            {
                var doc = XDocument.Parse(xml);

                foreach (var item in doc.Root.Elements("Item"))
                {
                    var data = new ChartData
                    {
                        Category = item.Element("Category")?.Value ?? "",
                        Amount = decimal.Parse(item.Element("Amount")?.Value ?? "0"),
                        Percentage = double.Parse(item.Element("Percentage")?.Value ?? "0"),
                        Color = item.Element("Color")?.Value ?? "#7F8C8D"
                    };

                    chartData.Add(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга XML: {ex.Message}");
            }

            return chartData;
        }

        private static string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        private static string GetCategoryColor(string category)
        {
            return CategoryColors.ContainsKey(category)
                ? CategoryColors[category]
                : "#7F8C8D";
        }

        public static bool DeleteReport(int reportId)
        {
            try
            {
                if (!CheckReportsTableExists())
                {
                    Console.WriteLine("Таблица Reports не существует, удаление невозможно");
                    return false;
                }

                string sql = "DELETE FROM Reports WHERE Id = @Id";

                using (var connection = new SqlConnection(DatabaseManager.GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", reportId);
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Отчет ID {reportId} удален, затронуто строк: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления отчета: {ex.Message}");
                return false;
            }
        }
    }
}