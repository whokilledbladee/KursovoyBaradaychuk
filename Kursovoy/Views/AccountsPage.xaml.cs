using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient; 

namespace Kursovoy.Views
{
    public partial class AccountsPage : UserControl
    {
        private List<Account> _accounts;

        public AccountsPage()
        {
            InitializeComponent();
            LoadAccounts();
        }

        private void LoadAccounts()
        {
            try
            {
                _accounts = DatabaseManager.GetAllAccounts();

                if (!_accounts.Any())
                {
                    _accounts = new List<Account>
                    {
                        new Account { Name = "Наличные", Balance = 5000, Type = AccountType.Cash },
                        new Account { Name = "Сбербанк", Balance = 25000, Type = AccountType.DebitCard },
                        new Account { Name = "Тинькофф", Balance = 15000, Type = AccountType.CreditCard },
                        new Account { Name = "Копилка", Balance = 10000, Type = AccountType.Deposit }
                    };

                    foreach (var account in _accounts)
                    {
                        DatabaseManager.AddAccount(account);
                    }
                }

                AccountsListBox.ItemsSource = _accounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки счетов: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                LoadLocalAccounts();
            }
        }

        private void LoadLocalAccounts()
        {
            _accounts = new List<Account>
            {
                new Account { Name = "Наличные", Balance = 0, Type = AccountType.Cash },
                new Account { Name = "Сбербанк", Balance = 0, Type = AccountType.DebitCard },
                new Account { Name = "Тинькофф", Balance = 0, Type = AccountType.CreditCard },
                new Account { Name = "Копилка", Balance = 0, Type = AccountType.Deposit }
            };

            AccountsListBox.ItemsSource = _accounts;
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AccountDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    DatabaseManager.AddAccount(dialog.Account);
                    _accounts = DatabaseManager.GetAllAccounts();
                    AccountsListBox.ItemsSource = _accounts;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения счета: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsListBox.SelectedItem is Account selectedAccount)
            {
                var dialog = new AccountDialog(selectedAccount);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        DatabaseManager.UpdateAccount(dialog.Account);
                        _accounts = DatabaseManager.GetAllAccounts();
                        AccountsListBox.ItemsSource = _accounts;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка обновления счета: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите счет для редактирования", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsListBox.SelectedItem is Account selectedAccount)
            {
                var result = MessageBox.Show($"Удалить счет '{selectedAccount.Name}'?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DatabaseManager.DeleteAccount(selectedAccount.Id);
                        _accounts = DatabaseManager.GetAllAccounts();
                        AccountsListBox.ItemsSource = _accounts;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления счета: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите счет для удаления", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Добавьте этот метод в класс AccountsPage
        private void RefreshAllData()
        {
            try
            {
                Console.WriteLine("Обновление всех данных...");

                // Обновляем список счетов
                _accounts = DatabaseManager.GetAllAccounts();
                AccountsListBox.ItemsSource = _accounts;

                // Обновляем дашборд
                UpdateDashboard();

                Console.WriteLine("Данные успешно обновлены");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления данных: {ex.Message}");
            }
        }

        // Обновите метод QuickIncome_Click (измените конец метода):
        private void QuickIncome_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Нажата кнопка Доход");

            if (((Button)sender).Tag is Account account)
            {
                Console.WriteLine($"Выбран счет: {account.Name}, ID: {account.Id}, Баланс: {account.Balance}");

                var dialog = new OperationDialog(account);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        Console.WriteLine($"Операция: {dialog.Amount}р., категория: {dialog.Category}, тип: {dialog.Type}");

                        account.Balance += dialog.Amount;
                        Console.WriteLine($"Новый баланс счета: {account.Balance}");

                        DatabaseManager.UpdateAccount(account);
                        Console.WriteLine("Баланс обновлен в базе");

                        var transaction = new Transaction
                        {
                            AccountId = account.Id,
                            AccountName = account.Name,
                            Amount = dialog.Amount,
                            Type = dialog.Type,
                            Category = dialog.Category,
                            Description = string.IsNullOrWhiteSpace(dialog.Description)
                                ? "Без описания"
                                : dialog.Description,
                            Date = DateTime.Now,
                            CategoryId = GetCategoryId(dialog.Category)
                        };

                        Console.WriteLine($"Создана транзакция: ID категории={transaction.CategoryId}");

                        int transactionId = DatabaseManager.AddTransaction(transaction);

                        if (transactionId > 0)
                        {
                            transaction.Id = transactionId;
                            Console.WriteLine($"Транзакция сохранена, ID: {transactionId}");
                        }
                        else
                        {
                            Console.WriteLine("ОШИБКА: Транзакция не сохранена!");
                        }

                        // Обновляем список счетов
                        _accounts = DatabaseManager.GetAllAccounts();
                        AccountsListBox.ItemsSource = _accounts;

                        // Пытаемся обновить дашборд, если он открыт
                        UpdateDashboard();

                        MessageBox.Show($"Доход {dialog.Amount:N0} р. добавлен на счет '{account.Name}'",
                                      "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ОШИБКА: {ex.Message}");
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Console.WriteLine("Диалог отменен");
                }
            }
        }

        private void QuickExpense_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Tag is Account account)
            {
                var dialog = new OperationDialog(account);
                if (dialog.ShowDialog() == true)
                {
                    if (account.Balance >= dialog.Amount)
                    {
                        try
                        {
                            account.Balance -= dialog.Amount;
                            DatabaseManager.UpdateAccount(account);

                            var transaction = new Transaction
                            {
                                AccountId = account.Id,
                                AccountName = account.Name,
                                Amount = dialog.Amount,
                                Type = OperationType.Expense,
                                Category = dialog.Category,
                                Description = string.IsNullOrWhiteSpace(dialog.Description)
                                    ? "Без описания"
                                    : dialog.Description,
                                Date = DateTime.Now,
                                CategoryId = GetCategoryId(dialog.Category)
                            };

                            int transactionId = DatabaseManager.AddTransaction(transaction);

                            if (transactionId > 0)
                            {
                                transaction.Id = transactionId;
                                Console.WriteLine($"Транзакция сохранена, ID: {transactionId}");
                            }

                            // Обновляем список счетов
                            _accounts = DatabaseManager.GetAllAccounts();
                            AccountsListBox.ItemsSource = _accounts;

                            // Пытаемся обновить дашборд, если он открыт
                            UpdateDashboard();

                            MessageBox.Show($"Расход {dialog.Amount:N0} р. списан со счета '{account.Name}'",
                                          "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                                          MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Недостаточно средств на счете!", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // Обновите метод UpdateDashboard
        private void UpdateDashboard()
        {
            try
            {
                Console.WriteLine("Обновление дашборда...");

                // Не пытаемся обновлять главное окно напрямую
                // Вместо этого обновляем данные в базе и ждем, пока пользователь переключится на дашборд

                // Просто логируем обновление
                decimal totalIncome = DatabaseManager.GetTotalIncome();
                decimal totalExpenses = DatabaseManager.GetTotalExpenses();
                Console.WriteLine($"Данные обновлены. Доходы: {totalIncome}р., Расходы: {totalExpenses}р.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления дашборда: {ex.Message}");
            }
        }
        private int GetCategoryId(string categoryName)
        {
            try
            {
                var categories = DatabaseManager.GetAllCategories();
                var category = categories.FirstOrDefault(c => c.Name == categoryName);
                return category?.Id ?? 0; 
            }
            catch
            {
                return 0;
            }
        }
    }
}