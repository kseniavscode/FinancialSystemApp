using LR1.Interfaces;
using LR1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LR1.Services
{
    internal class ClientService : IClientService
    {
        public void CloseAccount(BankAccount acc)
        {
            if (acc == null)
            {
                MessageBox.Show("Please select an account to close!");
                return;
            }
            if (acc.Balance > 0)
            {
                MessageBox.Show("Cannot close account with positive balance! Transfer your money first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = MessageBox.Show($"Are you sure you want to close account {acc.Number}?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                App.Database.Accounts.Remove(acc);

                App.Database.Save();
            }
        }

        
    }
}
