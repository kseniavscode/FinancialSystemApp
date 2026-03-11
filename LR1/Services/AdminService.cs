using LR1.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using LR1.Models;

namespace LR1.Services
{
    internal class AdminService : IAdminService
    {
        public void RollbackTransaction(TransactionAction transaction)
        {
            var sourceAcc = App.Database.Accounts.FirstOrDefault(a => a.BankAccountId == transaction.SourceAccountId);
            var targetAcc = App.Database.Accounts.FirstOrDefault(a => a.BankAccountId == transaction.TargetAccountId);

            if (transaction.Type == TransactionType.Transfer || transaction.Type == TransactionType.TransferDeposit)
            {
                if (sourceAcc != null) sourceAcc.Balance += transaction.Amount;
                if (targetAcc != null) targetAcc.Balance -= transaction.Amount;
            }
            if (transaction.Type == TransactionType.Deposit || transaction.Type == TransactionType.SalaryPayment)
            {
                if (targetAcc != null) targetAcc.Balance -= transaction.Amount;
            }
            if (transaction.Type == TransactionType.Withdrawal)
            {
                if (sourceAcc != null) sourceAcc.Balance += transaction.Amount;
            }
            if (transaction.Type == TransactionType.DepositCreation)
            {
                targetAcc = App.Database.Deposits.FirstOrDefault(a => a.BankAccountId == transaction.TargetAccountId);
                if (sourceAcc != null) sourceAcc.Balance += transaction.Amount;
                if (targetAcc != null)
                {
                    targetAcc.Balance -= transaction.Amount;
                    App.Database.Deposits.Remove((DepositAccount)targetAcc);
                }

            }

            transaction.IsCancelled = true;
            transaction.Description = "[ROLLED BACK] " + transaction.Description;
            App.Database.Save();

            
        }
    }
}
