using System.ComponentModel.DataAnnotations.Schema;

namespace Kursovoy.Views
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public CategoryType Type { get; set; }
        public bool IsUserCreated { get; set; }

        [NotMapped]
        public string DisplayType => Type == CategoryType.Income ? "Доход" : "Расход";
    }

    public enum CategoryType
    {
        Income = 0,
        Expense = 1
    }
}