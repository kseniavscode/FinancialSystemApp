using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LR1.Models;
namespace LR1.Views
{
    /// <summary>
    /// Логика взаимодействия для TransferWindow.xaml
    /// </summary>
    public partial class TransferWindow : Window
    {
        private User current_user;
        public TransferWindow(User user)
        {
            InitializeComponent();
            current_user = user;
            FromAccountComboBox.ItemsSource = App.Database.Accounts.Where(x => x.OwnerId == current_user.IdUser && !x.IsBlocked).ToList();
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            var sourceAcc = FromAccountComboBox.SelectedItem as BankAccount;
            string targetNumber = ToAccountNumberTextBox.Text;

            if (sourceAcc == null || string.IsNullOrWhiteSpace(targetNumber) || !decimal.TryParse(SumTransferTextBox.Text, out decimal sum))
            {
                MessageBox.Show("Full all fields correctly!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }

            if (sum < 0)
            {
                MessageBox.Show("Sum must be positive!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }

            var targetAcc = App.Database.Accounts.FirstOrDefault(x => x.Number == targetNumber);
            if (targetAcc == null)
            {
                MessageBox.Show("Enter the correct number for an account!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if (sourceAcc.Number == targetAcc.Number)
            {
                MessageBox.Show("You cannot transfer money to the same account!");
                return;
            }
            if (sourceAcc.Balance < sum)
            {
                MessageBox.Show("Not enough money on balance!");
                return;
            }
            sourceAcc.Balance -= sum;
            targetAcc.Balance += sum;
            var transaction = new TransactionAction
            {
                UserIdFrom = current_user.IdUser,
                UserIdTo = targetAcc.OwnerId,
                Type = TransactionType.Transfer,
                Amount = sum,
                SourceAccountId = sourceAcc.BankAccountId, 
                TargetAccountId = targetAcc.BankAccountId,
                Description = $"Transfer from {sourceAcc.Number} to {targetAcc.Number}"
            };
            App.Database.Transactions.Add(transaction);
            
            App.Database.Save();
            MessageBox.Show("Transfer successful!");
            this.Close();
        }
    }
}
