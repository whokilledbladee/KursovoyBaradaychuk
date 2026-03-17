using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kursovoy.Views
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ChartData> IncomeData { get; set; }
        public List<ChartData> ExpenseData { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance { get; set; }

        [NotMapped]
        public string DisplayDate => CreatedDate.ToString("dd.MM.yyyy HH:mm");

        public Report()
        {
            IncomeData = new List<ChartData>();
            ExpenseData = new List<ChartData>();
            CreatedDate = DateTime.Now;
        }
    }

    public class ChartData
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; }

        [NotMapped]
        public string DisplayAmount => $"{Amount:N0} р.";
        [NotMapped]
        public string DisplayPercentage => $"{Percentage:F1}%";
    }
}