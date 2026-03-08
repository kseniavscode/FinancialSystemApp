using System;
using System.Collections.Generic;
using System.Text;

namespace LR1.Models
{
    public enum UserRole
    {
        Client,
        Manager,
        Admin
    }
    public enum BankAccountType
    {
        Checking,
        Deposit

    }
    public enum TransactionType
    {
        Transfer,       
        AccountOpening, 
        DepositCreation,
        SalaryPayment,  
        Blocking        
    }
}
