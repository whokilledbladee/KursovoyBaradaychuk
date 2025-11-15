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
using System.Windows.Shapes;

namespace Kursovoy.Views
{
    public partial class GoalDialog : Window
    {
        public Goal Goal { get; private set; }

        public GoalDialog()
        {
            InitializeComponent();
            Goal = new Goal();
        }

        public GoalDialog(Goal existingGoal) : this()
        {
            Goal = existingGoal;
            NameTextBox.Text = existingGoal.Name;
            TargetAmountTextBox.Text = existingGoal.TargetAmount.ToString("F2");
            CurrentAmountTextBox.Text = existingGoal.CurrentAmount.ToString("F2");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название цели", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(TargetAmountTextBox.Text, out decimal targetAmount) || targetAmount <= 0)
            {
                MessageBox.Show("Введите корректную целевую сумму", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(CurrentAmountTextBox.Text, out decimal currentAmount) || currentAmount < 0)
            {
                MessageBox.Show("Введите корректную текущую сумму", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Goal.Name = NameTextBox.Text;
            Goal.TargetAmount = targetAmount;
            Goal.CurrentAmount = currentAmount;

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