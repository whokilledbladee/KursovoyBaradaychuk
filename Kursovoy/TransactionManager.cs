using System;
using System.Collections.Generic;
using System.Linq;
using Kursovoy.Views;

namespace Kursovoy
{
    public static class TransactionManager
    {
        public static List<Transaction> GetAllTransactions()
        {
            try
            {
                return DatabaseManager.GetAllTransactions();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки транзакций: {ex.Message}");
                return new List<Transaction>();
            }
        }

        public static int AddTransaction(Transaction transaction)
        {
            try
            {
                return DatabaseManager.AddTransaction(transaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения транзакции: {ex.Message}");
                throw;
            }
        }

        public static List<Transaction> GetTransactionsByAccount(string accountName)
        {
            try
            {
                var allTransactions = DatabaseManager.GetAllTransactions();
                return allTransactions
                    .Where(t => t.AccountName == accountName)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка фильтрации транзакций: {ex.Message}");
                return new List<Transaction>();
            }
        }

        public static List<Transaction> GetTransactionsByType(OperationType type)
        {
            try
            {
                var allTransactions = DatabaseManager.GetAllTransactions();
                return allTransactions
                    .Where(t => t.Type == type)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка фильтрации транзакций: {ex.Message}");
                return new List<Transaction>();
            }
        }

        public static List<string> GetUniqueAccountNames()
        {
            try
            {
                var transactions = GetAllTransactions();
                return transactions
                    .Select(t => t.AccountName)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения уникальных счетов: {ex.Message}");
                return new List<string>();
            }
        }
    }
}