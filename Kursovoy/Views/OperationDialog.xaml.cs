using System;
using System.Windows;
using System.Windows.Controls;

namespace Kursovoy.Views
{
    public partial class OperationDialog : Window
    {
        public decimal Amount { get; private set; }
        public OperationType Type { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }

        public string AccountInfo { get; set; }

        public OperationDialog(Account account)
        {
            InitializeComponent();

            this.DataContext = this;

            AccountInfo = $"Счет: {account.Name}\nБаланс: {account.Balance:N0} р.";
            OperationTypeComboBox.SelectedIndex = 0;
            Type = OperationType.Income;

            InitializeCategories();

            AmountTextBox.Focus();
            AmountTextBox.SelectAll();
        }

        private void InitializeCategories()
        {
            CategoryComboBox.Items.Add("Зарплата");
            CategoryComboBox.Items.Add("Фриланс");
            CategoryComboBox.Items.Add("Дивиденды");
            CategoryComboBox.Items.Add("Подарки");

            CategoryComboBox.Items.Add("Продукты");
            CategoryComboBox.Items.Add("Транспорт");
            CategoryComboBox.Items.Add("Кафе/Рестораны");
            CategoryComboBox.Items.Add("Одежда");
            CategoryComboBox.Items.Add("Развлечения");
            CategoryComboBox.Items.Add("Образование");
            CategoryComboBox.Items.Add("Здоровье");
            CategoryComboBox.Items.Add("Комунальные");
            CategoryComboBox.Items.Add("Прочее");

            CategoryComboBox.SelectedIndex = 0;
        }

        private void OperationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OperationTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string tag = selectedItem.Tag.ToString();
                if (tag == "Income")
                {
                    Type = OperationType.Income;
                }
                else if (tag == "Expense")
                {
                    Type = OperationType.Expense;
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out decimal amount) && amount > 0)
            {
                Amount = amount;
                Description = DescriptionTextBox.Text;
                Category = CategoryComboBox.SelectedItem?.ToString() ?? "Без категории";

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}