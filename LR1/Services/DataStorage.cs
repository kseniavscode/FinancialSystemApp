using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using LR1.Models;
using System.IO;

namespace LR1.Services
{
    public class DataStorage
    {
        private const string FileName = "data.json";
        public List<User> Users { get; set; } = new List<User>();
        public List<Bank> Banks { get; set; } = new List<Bank>();
        public List<BankAccount> Accounts { get; set; } = new List<BankAccount>();
        public List<Enterprise> Enterprises { get; set; } = new List<Enterprise>();
        public List<TransactionAction> Transactions { get; set; } = new List<TransactionAction>();

        public void Save()
        {
            
            var options = new JsonSerializerOptions { WriteIndented = true};
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(FileName, json);
        }
        public static DataStorage Load()
        {
            DataStorage storage;
            if (!File.Exists(FileName))
            {
                storage = new DataStorage();
            }
            else
            {
                try
                {
                    string json = File.ReadAllText(FileName);
                    storage = JsonSerializer.Deserialize<DataStorage>(json) ?? new DataStorage();
                }
                catch
                {
                    storage = new DataStorage();
                }
            }
            if (storage.Banks.Count == 0)
            {
                storage.Banks.Add(new Bank("Альфа-Банк"));
                storage.Banks.Add(new Bank("Беларусбанк"));

                storage.Enterprises.Add(new Enterprise("Техно-Старт"));
                storage.Enterprises.Add(new Enterprise("Глобал АйТи"));

                storage.Save();
            }
            return storage;
        }
    }
}
