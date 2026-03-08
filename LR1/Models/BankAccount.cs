using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LR1.Models
{
    public class BankAccount
    {
        public Guid BankAccountId { get; set; }
        public string Number { get; set; }
        public decimal Balance { get; set; }
        public Guid OwnerId { get; set; } 
        public Guid BankId { get; set; }  
        public bool IsBlocked { get; set; } = false; 
        public BankAccountType Type { get; set; }

        public BankAccount() { }
        public BankAccount(string number, Guid ownerId, Guid bankId, BankAccountType type)
        {
            Number = number;
            BankAccountId = Guid.NewGuid();
            OwnerId = ownerId;
            BankId = bankId;
            Type = type;
            Balance = 0;
        }
        [JsonIgnore]
        public string BankName => App.Database.Banks.FirstOrDefault(b => b.BankId == BankId)?.Name ?? "Unknown";
        [JsonIgnore]
        public string DisplayInfo => $"{Number} (Balance: {Balance:C})";
    }
}
