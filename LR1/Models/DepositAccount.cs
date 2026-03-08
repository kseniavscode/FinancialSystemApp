using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LR1.Models
{
    internal class DepositAccount : BankAccount
    {
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public int TermMonths { get; set; }

        public DateTime EndDate => StartDate.AddMinutes(TermMonths);
        public string NumberAccount {  get; set; }
        public decimal CurrentInterest => CalculateCurrentInterest();
        public DepositAccount(string number, string numberAccount, Guid ownerId, Guid bankId, BankAccountType type, decimal procent, int termMonths) : base(number, ownerId, bankId, type)
        {
            InterestRate = procent;
            StartDate = DateTime.Now;
            TermMonths = termMonths;
            NumberAccount = numberAccount;
        }
        public DepositAccount() { }


        public decimal CalculateCurrentInterest()
        {
            if (DateTime.Now < EndDate)
            {
                return 0;
            }
            var timePassed = DateTime.Now - StartDate;
            double yearsPassed = timePassed.TotalMinutes / 12.0;
            return Math.Round(Balance * InterestRate * (decimal)yearsPassed, 2);
        }
    }
}
