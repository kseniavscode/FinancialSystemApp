using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using LR1.Models;

namespace LR1.Interfaces
{
    public interface IAdminService
    {
        void RollbackTransaction(TransactionAction transaction);
    }

    public interface IManagerService
    {
        void ApproveUser(User user);
        void RejectUser(User user);
        void ApproveSalaryInline(SalaryRequest salaryRequest);
    }
    public interface IClientService
    {
        void CloseAccount(BankAccount acc);
    }
}
