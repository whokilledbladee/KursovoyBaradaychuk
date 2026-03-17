using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kursovoy.Views
{
    public partial class TransactionsPage : UserControl
    {
        private List<Transaction> _allTransactions;

        public TransactionsPage()
        {
            InitializeComponent();
            LoadTransactions();
            LoadAccountFilters();
        }

        private void LoadTransactions()
        {
            try
            {
                _allTransactions = DatabaseManager.GetAllTransactions();
                TransactionsListView.ItemsSource = _allTransactions;

                Console.WriteLine($"Загружено транзакций: {_allTransactions.Count}");

                if (!_allTransactions.Any())
                {
                    MessageBox.Show("В базе данных нет транзакций. Создайте несколько операций.",
                                  "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки транзакций: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAccountFilters()
        {
            AccountFilterComboBox.Items.Clear();
            AccountFilterComboBox.Items.Add(new ComboBoxItem { Content = "Все счета", IsSelected = true });

            var accounts = DatabaseManager.GetAllAccounts();

            foreach (var account in accounts.OrderBy(a => a.Name))
            {
                AccountFilterComboBox.Items.Add(new ComboBoxItem { Content = account.Name });
            }
        }

        private void AccountFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountFilterComboBox == null || TransactionsListView == null)
                return;

            ApplyFilters();
        }

        private void TypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeFilterComboBox == null || TransactionsListView == null)
                return;

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                var filteredTransactions = DatabaseManager.GetAllTransactions();

                if (AccountFilterComboBox.SelectedItem is ComboBoxItem accountItem)
                {
                    string selectedAccount = accountItem.Content?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(selectedAccount) && selectedAccount != "Все счета")
                    {
                        filteredTransactions = filteredTransactions
                            .Where(t => t.AccountName == selectedAccount)
                            .ToList();
                    }
                }

                if (TypeFilterComboBox.SelectedItem is ComboBoxItem typeItem)
                {
                    string selectedType = typeItem.Content?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(selectedType))
                    {
                        if (selectedType == "Доходы")
                        {
                            filteredTransactions = filteredTransactions
                                .Where(t => t.Type == OperationType.Income)
                                .ToList();
                        }
                        else if (selectedType == "Расходы")
                        {
                            filteredTransactions = filteredTransactions
                                .Where(t => t.Type == OperationType.Expense)
                                .ToList();
                        }
                    }
                }

                filteredTransactions = filteredTransactions
                    .OrderByDescending(t => t.Date)
                    .ToList();

                TransactionsListView.ItemsSource = filteredTransactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка фильтрации: {ex.Message}");
                TransactionsListView.ItemsSource = DatabaseManager.GetAllTransactions();
            }
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AccountFilterComboBox != null)
                    AccountFilterComboBox.SelectedIndex = 0;

                if (TypeFilterComboBox != null)
                    TypeFilterComboBox.SelectedIndex = 0;

                ApplyFilters();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сброса фильтров: {ex.Message}");
            }
        }
    }
}