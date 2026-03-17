using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kursovoy.Views
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public AccountType Type { get; set; }
        public string Currency { get; set; } = "RUB";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string Icon
        {
            get
            {
                switch (Type)
                {
                    case AccountType.Cash: return "💵";
                    case AccountType.DebitCard: return "💳";
                    case AccountType.CreditCard: return "📇";
                    case AccountType.DigitalWallet: return "📱";
                    case AccountType.Deposit: return "🏦";
                    default: return "💰";
                }
            }
        }

        [NotMapped]
        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case AccountType.Cash: return "Наличные";
                    case AccountType.DebitCard: return "Дебетовая карта";
                    case AccountType.CreditCard: return "Кредитная карта";
                    case AccountType.DigitalWallet: return "Электронный кошелек";
                    case AccountType.Deposit: return "Депозит";
                    default: return "Другое";
                }
            }
        }
    }

    public enum AccountType
    {
        Cash,
        DebitCard,
        CreditCard,
        DigitalWallet,
        Deposit
    }
}