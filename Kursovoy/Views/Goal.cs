using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kursovoy.Views
{
    public class Goal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public decimal ProgressPercentage => TargetAmount > 0
            ? (CurrentAmount / TargetAmount) * 100
            : 0;

        [NotMapped]
        public double ProgressWidth => TargetAmount > 0
            ? (double)(CurrentAmount / TargetAmount) * 300
            : 0;
    }
}