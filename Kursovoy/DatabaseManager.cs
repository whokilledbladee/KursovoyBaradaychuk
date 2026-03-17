using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Kursovoy
{
    public static class DatabaseManager
    {
        private static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public static string GetConnectionString()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["FinanceDBConnection"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=FinanceManagerDB;Integrated Security=True;Connect Timeout=30";
                    Log("Используется резервная строка подключения");
                }

                Log($"Строка подключения: {connectionString}");
                return connectionString;
            }
            catch (Exception ex)
            {
                Log($"Ошибка получения строки подключения: {ex.Message}");
                throw;
            }
        }

        public static bool CheckDatabaseConnection()
        {
            try
            {
                Log("Проверка подключения к базе данных...");
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    Log("Подключение успешно");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка подключения к базе данных: {ex.Message}");
                return false;
            }
        }

        public static T ExecuteScalar<T>(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    var result = command.ExecuteScalar();
                    return result != DBNull.Value ? (T)Convert.ChangeType(result, typeof(T)) : default(T);
                }
            }
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    return command.ExecuteNonQuery();
                }
            }
        }

        private static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                        command.Parameters.AddRange(parameters);

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }


        public static List<Views.Account> GetAllAccounts()
        {
            Log("Загрузка всех счетов");
            var accounts = new List<Views.Account>();

            try
            {
                string sql = "SELECT * FROM Accounts ORDER BY Name";
                var dataTable = ExecuteQuery(sql);

                Log($"Найдено счетов: {dataTable.Rows.Count}");

                foreach (DataRow row in dataTable.Rows)
                {
                    accounts.Add(new Views.Account
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Name = row["Name"].ToString(),
                        Balance = Convert.ToDecimal(row["Balance"]),
                        Type = (Views.AccountType)Convert.ToInt32(row["Type"]),
                        Currency = row["Currency"].ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка загрузки счетов: {ex.Message}");
                throw;
            }

            return accounts;
        }

        public static int AddAccount(Views.Account account)
        {
            Log($"Добавление счета: {account.Name}, баланс: {account.Balance}");

            string sql = @"
            INSERT INTO Accounts (Name, Balance, Type, Currency) 
            OUTPUT INSERTED.Id
            VALUES (@Name, @Balance, @Type, @Currency)";

            var parameters = new[]
            {
                new SqlParameter("@Name", account.Name),
                new SqlParameter("@Balance", account.Balance),
                new SqlParameter("@Type", (int)account.Type),
                new SqlParameter("@Currency", account.Currency)
            };

            int newId = ExecuteScalar<int>(sql, parameters);
            Log($"Счет добавлен, ID: {newId}");
            return newId;
        }

        public static void UpdateAccount(Views.Account account)
        {
            Log($"Обновление счета ID {account.Id}: {account.Name}, новый баланс: {account.Balance}");

            string sql = @"
            UPDATE Accounts 
            SET Name = @Name, 
                Balance = @Balance, 
                Type = @Type, 
                Currency = @Currency 
            WHERE Id = @Id";

            var parameters = new[]
            {
                new SqlParameter("@Id", account.Id),
                new SqlParameter("@Name", account.Name),
                new SqlParameter("@Balance", account.Balance),
                new SqlParameter("@Type", (int)account.Type),
                new SqlParameter("@Currency", account.Currency)
            };

            int rowsAffected = ExecuteNonQuery(sql, parameters);
            Log($"Счет обновлен, затронуто строк: {rowsAffected}");
        }

        public static void DeleteAccount(int id)
        {
            Log($"Удаление счета ID: {id}");

            string sql = "DELETE FROM Accounts WHERE Id = @Id";
            var parameter = new SqlParameter("@Id", id);

            int rowsAffected = ExecuteNonQuery(sql, parameter);
            Log($"Счет удален, затронуто строк: {rowsAffected}");
        }

        public static List<Views.Category> GetAllCategories()
        {
            Log("Загрузка всех категорий");
            var categories = new List<Views.Category>();

            string sql = "SELECT * FROM Categories ORDER BY CategoryType, Name";
            var dataTable = ExecuteQuery(sql);

            Log($"Найдено категорий: {dataTable.Rows.Count}");

            foreach (DataRow row in dataTable.Rows)
            {
                categories.Add(new Views.Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Icon = row["Icon"].ToString(),
                    Type = (Views.CategoryType)Convert.ToInt32(row["CategoryType"]),
                    IsUserCreated = Convert.ToBoolean(row["IsUserCreated"])
                });
            }

            return categories;
        }

        public static List<Views.Category> GetIncomeCategories()
        {
            Log("Загрузка категорий доходов");
            var categories = new List<Views.Category>();

            string sql = "SELECT * FROM Categories WHERE CategoryType = 0 ORDER BY Name";
            var dataTable = ExecuteQuery(sql);

            Log($"Найдено категорий доходов: {dataTable.Rows.Count}");

            foreach (DataRow row in dataTable.Rows)
            {
                categories.Add(new Views.Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Icon = row["Icon"].ToString(),
                    Type = Views.CategoryType.Income,
                    IsUserCreated = Convert.ToBoolean(row["IsUserCreated"])
                });
            }

            return categories;
        }

        public static List<Views.Category> GetExpenseCategories()
        {
            Log("Загрузка категорий расходов");
            var categories = new List<Views.Category>();

            string sql = "SELECT * FROM Categories WHERE CategoryType = 1 ORDER BY Name";
            var dataTable = ExecuteQuery(sql);

            Log($"Найдено категорий расходов: {dataTable.Rows.Count}");

            foreach (DataRow row in dataTable.Rows)
            {
                categories.Add(new Views.Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Icon = row["Icon"].ToString(),
                    Type = Views.CategoryType.Expense,
                    IsUserCreated = Convert.ToBoolean(row["IsUserCreated"])
                });
            }

            return categories;
        }

        public static int AddCategory(Views.Category category)
        {
            Log($"Добавление категории: {category.Name}, тип: {category.Type}");

            string sql = @"
            INSERT INTO Categories (Name, Icon, CategoryType, IsUserCreated)
            OUTPUT INSERTED.Id
            VALUES (@Name, @Icon, @CategoryType, @IsUserCreated)";

            var parameters = new[]
            {
                new SqlParameter("@Name", category.Name),
                new SqlParameter("@Icon", category.Icon),
                new SqlParameter("@CategoryType", (int)category.Type),
                new SqlParameter("@IsUserCreated", category.IsUserCreated)
            };

            int newId = ExecuteScalar<int>(sql, parameters);
            Log($"Категория добавлена, ID: {newId}");
            return newId;
        }

        public static void UpdateCategory(Views.Category category)
        {
            Log($"Обновление категории ID {category.Id}: {category.Name}");

            string sql = @"
            UPDATE Categories 
            SET Name = @Name, 
                Icon = @Icon, 
                CategoryType = @CategoryType
            WHERE Id = @Id";

            var parameters = new[]
            {
                new SqlParameter("@Id", category.Id),
                new SqlParameter("@Name", category.Name),
                new SqlParameter("@Icon", category.Icon),
                new SqlParameter("@CategoryType", (int)category.Type)
            };

            int rowsAffected = ExecuteNonQuery(sql, parameters);
            Log($"Категория обновлена, затронуто строк: {rowsAffected}");
        }

        public static void DeleteCategory(int id)
        {
            Log($"Удаление категории ID: {id}");

            string sql = "DELETE FROM Categories WHERE Id = @Id AND IsUserCreated = 1";
            var parameter = new SqlParameter("@Id", id);

            int rowsAffected = ExecuteNonQuery(sql, parameter);
            Log($"Категория удалена, затронуто строк: {rowsAffected}");
        }
        public static int AddTransaction(Views.Transaction transaction)
        {
            try
            {
                Console.WriteLine($"Добавление транзакции: {transaction.Amount}р., {transaction.Category}");

                string sql = @"
                    INSERT INTO Transactions (Date, AccountName, Amount, Type, Category, 
                                             Description, AccountId, CategoryId) 
                    OUTPUT INSERTED.Id
                    VALUES (@Date, @AccountName, @Amount, @Type, @Category, 
                   @Description, @AccountId, @CategoryId)";

                var parameters = new[]
                {
            new SqlParameter("@Date", transaction.Date),
            new SqlParameter("@AccountName", transaction.AccountName),
            new SqlParameter("@Amount", transaction.Amount),
            new SqlParameter("@Type", (int)transaction.Type),
            new SqlParameter("@Category", transaction.Category),
            new SqlParameter("@Description", transaction.Description ?? (object)DBNull.Value),
            new SqlParameter("@AccountId", transaction.AccountId),
            new SqlParameter("@CategoryId", transaction.CategoryId > 0 ? (object)transaction.CategoryId : DBNull.Value)
        };

                int newId = ExecuteScalar<int>(sql, parameters);
                Console.WriteLine($"Транзакция добавлена, ID: {newId}");
                return newId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении транзакции: {ex.Message}");
                throw;
            }
        }
        public static List<Views.Transaction> GetAllTransactions()
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Загрузка всех транзакций из БД");

                var transactions = new List<Views.Transaction>();

                string sql = @"
            SELECT Id, Date, AccountName, Amount, Type, Category, 
                   Description, AccountId, CategoryId 
            FROM Transactions 
            ORDER BY Date DESC";

                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var transaction = new Views.Transaction
                            {
                                Id = reader.GetInt32(0),
                                Date = reader.GetDateTime(1),
                                AccountName = reader.GetString(2),
                                Amount = reader.GetDecimal(3),
                                Type = (Views.OperationType)reader.GetInt32(4),
                                Category = reader.GetString(5),
                                Description = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                AccountId = reader.GetInt32(7),
                                CategoryId = reader.IsDBNull(8) ? 0 : reader.GetInt32(8)
                            };

                            transactions.Add(transaction);
                        }
                    }
                }

                Console.WriteLine($"Загружено {transactions.Count} транзакций из БД");
                return transactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки транзакций: {ex.Message}");
                throw;
            }
        }

        public static List<Views.Goal> GetAllGoals()
        {
            Log("Загрузка всех целей");
            var goals = new List<Views.Goal>();

            string sql = "SELECT * FROM Goals ORDER BY CreatedAt DESC";
            var dataTable = ExecuteQuery(sql);

            Log($"Найдено целей: {dataTable.Rows.Count}");

            foreach (DataRow row in dataTable.Rows)
            {
                goals.Add(new Views.Goal
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    TargetAmount = Convert.ToDecimal(row["TargetAmount"]),
                    CurrentAmount = Convert.ToDecimal(row["CurrentAmount"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"])
                });
            }

            return goals;
        }

        public static int AddGoal(Views.Goal goal)
        {
            Log($"Добавление цели: {goal.Name}, цель: {goal.TargetAmount}р., текущая: {goal.CurrentAmount}р.");

            string sql = @"
            INSERT INTO Goals (Name, TargetAmount, CurrentAmount)
            OUTPUT INSERTED.Id
            VALUES (@Name, @TargetAmount, @CurrentAmount)";

            var parameters = new[]
            {
                new SqlParameter("@Name", goal.Name),
                new SqlParameter("@TargetAmount", goal.TargetAmount),
                new SqlParameter("@CurrentAmount", goal.CurrentAmount)
            };

            int newId = ExecuteScalar<int>(sql, parameters);
            Log($"Цель добавлена, ID: {newId}");
            return newId;
        }

        public static void UpdateGoal(Views.Goal goal)
        {
            Log($"Обновление цели ID {goal.Id}: {goal.Name}, прогресс: {goal.CurrentAmount}/{goal.TargetAmount}");

            string sql = @"
            UPDATE Goals 
            SET Name = @Name, 
                TargetAmount = @TargetAmount, 
                CurrentAmount = @CurrentAmount 
            WHERE Id = @Id";

            var parameters = new[]
            {
                new SqlParameter("@Id", goal.Id),
                new SqlParameter("@Name", goal.Name),
                new SqlParameter("@TargetAmount", goal.TargetAmount),
                new SqlParameter("@CurrentAmount", goal.CurrentAmount)
            };

            int rowsAffected = ExecuteNonQuery(sql, parameters);
            Log($"Цель обновлена, затронуто строк: {rowsAffected}");
        }

        public static void DeleteGoal(int id)
        {
            Log($"Удаление цели ID: {id}");

            string sql = "DELETE FROM Goals WHERE Id = @Id";
            var parameter = new SqlParameter("@Id", id);

            int rowsAffected = ExecuteNonQuery(sql, parameter);
            Log($"Цель удалена, затронуто строк: {rowsAffected}");
        }

        public static decimal GetTotalBalance()
        {
            Log("Расчет общего баланса");

            string sql = "SELECT SUM(Balance) FROM Accounts";
            decimal result = ExecuteScalar<decimal>(sql);

            Log($"Общий баланс: {result}р.");
            return result;
        }

        public static decimal GetTotalIncome()
        {
            try
            {
                string sql = "SELECT SUM(Amount) FROM Transactions WHERE Type = 0";
                decimal result = ExecuteScalar<decimal>(sql);
                return result;
            }
            catch
            {
                return 0;
            }
        }

        public static decimal GetTotalExpenses()
        {
            try
            {
                string sql = "SELECT SUM(Amount) FROM Transactions WHERE Type = 1";
                decimal result = ExecuteScalar<decimal>(sql);
                return result;
            }
            catch
            {
                return 0;
            }
        }



        public static List<Views.Transaction> GetTransactionsByAccount(int accountId)
        {
            Log($"Загрузка транзакций для счета ID: {accountId}");
            var transactions = new List<Views.Transaction>();

            string sql = "SELECT * FROM Transactions WHERE AccountId = @AccountId ORDER BY Date DESC";
            var parameter = new SqlParameter("@AccountId", accountId);

            var dataTable = ExecuteQuery(sql, parameter);

            Log($"Найдено транзакций для счета {accountId}: {dataTable.Rows.Count}");

            foreach (DataRow row in dataTable.Rows)
            {
                transactions.Add(new Views.Transaction
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Date = Convert.ToDateTime(row["Date"]),
                    AccountName = row["AccountName"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    Type = (Views.OperationType)Convert.ToInt32(row["Type"]),
                    Category = row["Category"].ToString(),
                    Description = row["Description"].ToString(),
                    AccountId = accountId,
                    CategoryId = row["CategoryId"] != DBNull.Value ? Convert.ToInt32(row["CategoryId"]) : 0
                });
            }

            return transactions;
        }

        public static List<Views.Category> SearchCategories(string searchText)
        {
            Log($"Поиск категорий по запросу: {searchText}");
            var categories = new List<Views.Category>();

            string sql = "SELECT * FROM Categories WHERE Name LIKE @SearchText ORDER BY Name";
            var parameter = new SqlParameter("@SearchText", $"%{searchText}%");

            var dataTable = ExecuteQuery(sql, parameter);

            Log($"Найдено категорий: {dataTable.Rows.Count}");

            foreach (DataRow row in dataTable.Rows)
            {
                categories.Add(new Views.Category
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Icon = row["Icon"].ToString(),
                    Type = (Views.CategoryType)Convert.ToInt32(row["CategoryType"]),
                    IsUserCreated = Convert.ToBoolean(row["IsUserCreated"])
                });
            }

            return categories;
        }

        public static void DebugDatabaseInfo()
        {
            try
            {
                Log("инфа о бд");

                // Таблицы
                string tablesSql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                var tables = ExecuteQuery(tablesSql);
                Log($"Таблицы в базе: {tables.Rows.Count}");
                foreach (DataRow row in tables.Rows)
                {
                    Log($"  - {row["TABLE_NAME"]}");
                }

                if (tables.Rows.Count > 0)
                {
                    foreach (DataRow tableRow in tables.Rows)
                    {
                        string tableName = tableRow["TABLE_NAME"].ToString();
                        try
                        {
                            string countSql = $"SELECT COUNT(*) as Count FROM {tableName}";
                            var countTable = ExecuteQuery(countSql);
                            if (countTable.Rows.Count > 0)
                            {
                                int count = Convert.ToInt32(countTable.Rows[0]["Count"]);
                                Log($"  Таблица {tableName}: {count} записей");
                            }
                        }
                        catch
                        {
                            Log($"  Таблица {tableName}: не удалось получить количество");
                        }
                    }
                }

                Log("=== КОНЕЦ ИНФОРМАЦИИ ===");
            }
            catch (Exception ex)
            {
                Log($"Ошибка при получении информации о базе: {ex.Message}");
            }
        }

        public static void InitializeDatabase()
        {
            try
            {
                Log("Начало инициализации базы данных...");

                if (!CheckDatabaseExists())
                {
                    Log("База данных не найдена. Создаем новую...");
                    CreateDatabase();
                }

                if (!CheckTablesExist())
                {
                    Log("Таблицы не найдены. Создаем структуру...");
                    CreateTables();
                }

                CreateReportsTable();

                CheckAndInsertDefaultData();

                Log("Инициализация базы данных завершена успешно");
            }
            catch (Exception ex)
            {
                Log($"Ошибка при инициализации базы данных: {ex.Message}");
                throw;
            }
        }

        private static bool CheckDatabaseExists()
        {
            try
            {
                Log("Проверка существования базы данных...");

                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                SELECT COUNT(*) 
                FROM sys.databases 
                WHERE name = 'FinanceManagerDB'";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        bool exists = count > 0;
                        Log($"База данных существует: {exists}");
                        return exists;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при проверке базы данных: {ex.Message}");
                return false;
            }
        }

        private static void CreateDatabase()
        {
            try
            {
                Log("Создание базы данных...");

                var connectionStringBuilder = new SqlConnectionStringBuilder(GetConnectionString());
                string databaseName = connectionStringBuilder.InitialCatalog;
                connectionStringBuilder.InitialCatalog = "master";
                string masterConnectionString = connectionStringBuilder.ToString();

                using (var connection = new SqlConnection(masterConnectionString))
                {
                    connection.Open();

                    string createDbSql = $"CREATE DATABASE [{databaseName}]";

                    using (var command = new SqlCommand(createDbSql, connection))
                    {
                        command.ExecuteNonQuery();
                        Log("База данных создана успешно");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при создании базы данных: {ex.Message}");

                CreateDatabaseLocalDB();
            }
        }

        private static void CreateDatabaseLocalDB()
        {
            try
            {
                Log("Попытка создания базы данных через LocalDB...");

                string localDbConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30";

                using (var connection = new SqlConnection(localDbConnectionString))
                {
                    connection.Open();

                    string createDbSql = "CREATE DATABASE [FinanceManagerDB]";

                    using (var command = new SqlCommand(createDbSql, connection))
                    {
                        command.ExecuteNonQuery();
                        Log("База данных создана успешно через LocalDB");
                    }
                }

                UpdateConnectionStringForLocalDB();
            }
            catch (Exception ex)
            {
                Log($"Ошибка при создании базы данных через LocalDB: {ex.Message}");
                throw new Exception("Не удалось создать базу данных. Убедитесь, что установлен SQL Server LocalDB.", ex);
            }
        }

        private static void UpdateConnectionStringForLocalDB()
        {
            try
            {
                Log("Обновление строки подключения для LocalDB...");

                Log("Рекомендуется обновить строку подключения в App.config на: Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=FinanceManagerDB;Integrated Security=True;Connect Timeout=30");
            }
            catch (Exception ex)
            {
                Log($"Ошибка при обновлении строки подключения: {ex.Message}");
            }
        }

        private static bool CheckTablesExist()
        {
            try
            {
                Log("Проверка существования таблиц...");

                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string sql = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME IN ('Accounts', 'Categories', 'Transactions', 'Goals')";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        int count = (int)command.ExecuteScalar();
                        bool allTablesExist = count == 4;
                        Log($"Все таблицы существуют: {allTablesExist} (найдено {count} из 4)");
                        return allTablesExist;
                    }
                }
            }


            catch (Exception ex)
            {
                Log($"Ошибка при проверке таблиц: {ex.Message}");
                return false;
            }
        }

        private static void CreateTables()
        {
            try
            {
                Log("Создание таблиц...");

                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string createAccountsTable = @"
                CREATE TABLE Accounts (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100) NOT NULL,
                    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
                    Type INT NOT NULL DEFAULT 0,
                    Currency NVARCHAR(10) NOT NULL DEFAULT 'RUB',
                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                )";

                    string createCategoriesTable = @"
                CREATE TABLE Categories (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100) NOT NULL,
                    Icon NVARCHAR(50) NOT NULL DEFAULT 'Default',
                    CategoryType INT NOT NULL,
                    IsUserCreated BIT NOT NULL DEFAULT 1,
                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                )";

                    string createTransactionsTable = @"
                CREATE TABLE Transactions (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Date DATETIME NOT NULL DEFAULT GETDATE(),
                    AccountName NVARCHAR(100) NOT NULL,
                    Amount DECIMAL(18,2) NOT NULL,
                    Type INT NOT NULL,
                    Category NVARCHAR(100) NOT NULL,
                    Description NVARCHAR(500),
                    AccountId INT NOT NULL,
                    CategoryId INT,
                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                )";

                    string createGoalsTable = @"
                CREATE TABLE Goals (
                    Id INT PRIMARY KEY IDENTITY(1,1),
                    Name NVARCHAR(100) NOT NULL,
                    TargetAmount DECIMAL(18,2) NOT NULL,
                    CurrentAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                )";

                    using (var command = new SqlCommand(createAccountsTable, connection))
                    {
                        command.ExecuteNonQuery();
                        Log("Таблица Accounts создана");
                    }

                    using (var command = new SqlCommand(createCategoriesTable, connection))
                    {
                        command.ExecuteNonQuery();
                        Log("Таблица Categories создана");
                    }

                    using (var command = new SqlCommand(createTransactionsTable, connection))
                    {
                        command.ExecuteNonQuery();
                        Log("Таблица Transactions создана");
                    }

                    using (var command = new SqlCommand(createGoalsTable, connection))
                    {
                        command.ExecuteNonQuery();
                        Log("Таблица Goals создана");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при создании таблиц: {ex.Message}");
                throw;
            }
        }

        public static void CreateReportsTable()
        {
            try
            {
                Log("Создание таблицы Reports...");

                string sql = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reports]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Reports] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Title] NVARCHAR(200) NOT NULL,
                            [CreatedDate] DATETIME NOT NULL,
                            [TotalIncome] DECIMAL(18,2) NOT NULL,
                            [TotalExpenses] DECIMAL(18,2) NOT NULL,
                            [Balance] DECIMAL(18,2) NOT NULL,
                            [IncomeDataXml] NVARCHAR(MAX),
                            [ExpenseDataXml] NVARCHAR(MAX),
                            [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
                        )
                        PRINT 'Таблица Reports создана'
                    END";

                ExecuteNonQuery(sql);
                Log("Таблица Reports проверена/создана");
            }
            catch (Exception ex)
            {
                Log($"Ошибка создания таблицы Reports: {ex.Message}");
                throw;
            }
        }


        private static void CheckAndInsertDefaultData()
        {
            try
            {
                Log("Проверка и добавление базовых данных...");

                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();

                    string checkCategoriesSql = "SELECT COUNT(*) FROM Categories";
                    using (var command = new SqlCommand(checkCategoriesSql, connection))
                    {
                        int count = (int)command.ExecuteScalar();

                        if (count == 0)
                        {
                            Log("Добавляем базовые категории...");

                            string insertIncomeCategories = @"
                                INSERT INTO Categories (Name, Icon, CategoryType, IsUserCreated) VALUES
                                ('Зарплата', 'Money', 0, 0),
                                ('Фриланс', 'Briefcase', 0, 0),
                                ('Инвестиции', 'TrendingUp', 0, 0),
                                ('Подарки', 'Gift', 0, 0),
                                ('Возврат долга', 'DollarSign', 0, 0)";

                            string insertExpenseCategories = @"
                                INSERT INTO Categories (Name, Icon, CategoryType, IsUserCreated) VALUES
                                ('Продукты', 'ShoppingCart', 1, 0),
                                ('Транспорт', 'Car', 1, 0),
                                ('Жилье', 'Home', 1, 0),
                                ('Развлечения', 'Film', 1, 0),
                                ('Здоровье', 'Heart', 1, 0),
                                ('Одежда', 'ShoppingBag', 1, 0),
                                ('Рестораны', 'Utensils', 1, 0),
                                ('Образование', 'Book', 1, 0)";

                            using (var cmd = new SqlCommand(insertIncomeCategories, connection))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmd = new SqlCommand(insertExpenseCategories, connection))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            Log("Базовые категории добавлены");
                        }
                    }

                    string checkAccountsSql = "SELECT COUNT(*) FROM Accounts";
                    using (var command = new SqlCommand(checkAccountsSql, connection))
                    {
                        int count = (int)command.ExecuteScalar();

                        if (count == 0)
                        {
                            Log("Добавляем базовый счет...");

                            string insertDefaultAccount = @"
                        INSERT INTO Accounts (Name, Balance, Type, Currency) 
                        VALUES ('Основной счет', 0, 0, 'RUB')";

                            using (var cmd = new SqlCommand(insertDefaultAccount, connection))
                            {
                                cmd.ExecuteNonQuery();
                            }

                            Log("Базовый счет добавлен");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Ошибка при добавлении базовых данных: {ex.Message}");
            }
        }
    }
}