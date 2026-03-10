using LR1.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LR1.Views
{
    /// <summary>
    /// Логика взаимодействия для AddDepositWindow.xaml
    /// </summary>
    public partial class AddDepositWindow : Window
    {
        User current;
        public AddDepositWindow(User user)
        {
            InitializeComponent();
            current = user;
            SourceAccountComboBox.ItemsSource = App.Database.Accounts.Where(x => x.OwnerId == current.IdUser && !x.IsBlocked && x.Type != BankAccountType.Deposit).ToList();
        }


        private void OpenDepositButton_Click(object sender, RoutedEventArgs e)
        {
            var acc = SourceAccountComboBox.SelectedItem as BankAccount;

            if (acc == null || !int.TryParse(TermDepositeTextBox.Text, out int month) || month <= 0)
            {
                MessageBox.Show("Full all fields correctly!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if (!decimal.TryParse(InterestRateTextBox.Text, out decimal procent) || procent <= 0)
            {
                MessageBox.Show("Full an interest rate correctly!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if (!decimal.TryParse(AmountTextBox.Text, out decimal sum) || sum <= 0)
            {
                MessageBox.Show("Full amount correctly!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if (acc.OwnerId != current.IdUser)
            {
                MessageBox.Show("You can transfer money throught only your own accounts!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if (acc.Balance < sum)
            {
                MessageBox.Show("Not enough money in your account!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            string number;
            Random rnd = new Random();
            do
            {
                number = "DEP" + rnd.Next(10000, 99999).ToString();
                if (App.Database.Deposits.FirstOrDefault(x => x.Number == number) == null)
                {
                    break;
                }
            } while (true);

            acc.Balance -= sum;
            var deposit = new DepositAccount(number, acc.Number, acc.OwnerId, acc.BankId, BankAccountType.Deposit, procent / 100, month);
            deposit.Balance = sum;
            var transaction = new TransactionAction
            {
                UserIdFrom = current.IdUser,
                Type = TransactionType.DepositCreation,
                Amount = sum,
                SourceAccountId = acc.BankAccountId,
                TargetAccountId = deposit.BankAccountId,
                Description = $"Deposite money: {sum} to {deposit.Number} by {procent}%"
            };
            App.Database.Transactions.Add(transaction);

            App.Database.Deposits.Add(deposit);
            App.Database.Save();
            MessageBox.Show($"Deposit {number} opened successfully!\nAmount: {sum:C}\nTerm: {month} months.");
            this.Close();

        }
    }
}
