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
    public partial class GoalsPage : UserControl
    {
        private List<Goal> _goals;
        public GoalsPage()
        {
            InitializeComponent();
            LoadGoals();
        }

        private void LoadGoals() //временные тестовые цели для наглядности
        {
            _goals = new List<Goal>();
            _goals.Add(new Goal
            {
                Name = "Диплом",
                TargetAmount = 100000,
                CurrentAmount = 25000
            });

            _goals.Add(new Goal
            {
                Name = "Отпуск в Соль-Илецке",
                TargetAmount = 150000,
                CurrentAmount = 75000
            });

            GoalsListBox.ItemsSource = _goals;
        }

        private void AddGoal_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GoalDialog();
            if (dialog.ShowDialog() == true)
            {
                _goals.Add(dialog.Goal);
                GoalsListBox.Items.Refresh();
            }
        }

        private void EditGoal_Click(object sender, RoutedEventArgs e)
        {
            if (GoalsListBox.SelectedItem is Goal selectedGoal)
            {
                var dialog = new GoalDialog(selectedGoal);
                if (dialog.ShowDialog() == true)
                {
                    GoalsListBox.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Выберите цель для редактирования", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteGoal_Click(object sender, RoutedEventArgs e)
        {
            if (GoalsListBox.SelectedItem is Goal selectedGoal)
            {
                var result = MessageBox.Show($"Удалить цель '{selectedGoal.Name}'?",
                 "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _goals.Remove(selectedGoal);
                    GoalsListBox.Items.Refresh(); 
                }
            }
            else
            {
                MessageBox.Show("Выберите цель для удаления", "Внимание",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}