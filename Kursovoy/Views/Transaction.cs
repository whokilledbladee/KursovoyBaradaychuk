using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kursovoy.Views
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public OperationType Type { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; } 

        [NotMapped]
        public string DisplayType => Type == OperationType.Income ? "Доход" : "Расход";

        [NotMapped]
        public string DisplayAmount => Type == OperationType.Income
            ? $"+{Amount:N0} р."
            : $"-{Amount:N0} р.";

        [NotMapped]
        public string DisplayDate => Date.ToString("dd.MM.yyyy HH:mm");
    }

    public enum OperationType
    {
        Income,
        Expense
    }
}