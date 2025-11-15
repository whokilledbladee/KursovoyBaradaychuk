using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursovoy.Views
{
    public class Goal
    {
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }

        public decimal ProgressPercentage
        {
            get
            {
                if (TargetAmount > 0)
                    return (CurrentAmount / TargetAmount) * 100;
                return 0;
            }
        }

        public double ProgressWidth
        {
            get
            {
                if (TargetAmount > 0)
                    return (double)(CurrentAmount / TargetAmount) * 300;
                return 0;
            }
        }
    }
}
