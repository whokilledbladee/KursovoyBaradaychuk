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

namespace Kursovoy.Views
{
    public partial class QuickOperationDialog : Window
    {
        public decimal Amount { get; private set; }
        public string AccountInfo { get; }

        public QuickOperationDialog(Account account, OperationType operationType)
        {
            InitializeComponent();

            AccountInfo = $"{account.Name}\n{operationType.ToString().ToUpper()}";
            Title = operationType == OperationType.Income ? "Доход" : "Расход";

            AmountTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out decimal amount) && amount > 0)
            {
                Amount = amount;
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