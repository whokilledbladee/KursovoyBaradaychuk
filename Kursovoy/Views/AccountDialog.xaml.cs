using System;
using System.Windows;
using System.Windows.Controls;

namespace Kursovoy.Views
{
    public partial class AccountDialog : Window
    {
        public Account Account { get; private set; }

        public AccountDialog()
        {
            InitializeComponent();
            Account = new Account();
            InitializeComboBox();
        }

        public AccountDialog(Account existingAccount) : this()
        {
            Account = existingAccount;
            NameTextBox.Text = existingAccount.Name;
            BalanceTextBox.Text = existingAccount.Balance.ToString("F2");

            foreach (ComboBoxItem item in TypeComboBox.Items)
            {
                if (item.Content.ToString() == GetTypeName(existingAccount.Type))
                {
                    TypeComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private string GetTypeName(AccountType type)
        {
            switch (type)
            {
                case AccountType.Cash: return "Наличные";
                case AccountType.DebitCard: return "Дебетовая карта";
                case AccountType.CreditCard: return "Кредитная карта";
                case AccountType.DigitalWallet: return "Электронный кошелек";
                case AccountType.Deposit: return "Депозит";
                default: return "Наличные";
            }
        }

        private void InitializeComboBox()
        {
            TypeComboBox.Items.Add(new ComboBoxItem { Content = "Наличные" });
            TypeComboBox.Items.Add(new ComboBoxItem { Content = "Дебетовая карта" });
            TypeComboBox.Items.Add(new ComboBoxItem { Content = "Кредитная карта" });
            TypeComboBox.Items.Add(new ComboBoxItem { Content = "Электронный кошелек" });
            TypeComboBox.Items.Add(new ComboBoxItem { Content = "Депозит" });
            TypeComboBox.SelectedIndex = 0;
        }

        private AccountType GetAccountTypeFromString(string typeName)
        {
            switch (typeName)
            {
                case "Наличные": return AccountType.Cash;
                case "Дебетовая карта": return AccountType.DebitCard;
                case "Кредитная карта": return AccountType.CreditCard;
                case "Электронный кошелек": return AccountType.DigitalWallet;
                case "Депозит": return AccountType.Deposit;
                default: return AccountType.Cash;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название счета", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(BalanceTextBox.Text, out decimal balance) || balance < 0)
            {
                MessageBox.Show("Введите корректный баланс", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Account.Name = NameTextBox.Text;
            Account.Balance = balance;

            if (TypeComboBox.SelectedItem is ComboBoxItem selectedType)
            {
                Account.Type = GetAccountTypeFromString(selectedType.Content.ToString());
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}