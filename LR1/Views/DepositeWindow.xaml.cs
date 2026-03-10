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
    /// Логика взаимодействия для DepositeWindow.xaml
    /// </summary>
    public partial class DepositeWindow : Window
    {
        private User current;
        public DepositeWindow(User user)
        {
            InitializeComponent();
            current = user;
            DepositeComboBox.ItemsSource = App.Database.Accounts.Where(x => x.OwnerId == current.IdUser && !x.IsBlocked);
        }

        private void DepositeButton_Click(object sender, RoutedEventArgs e)
        {
            var acc = DepositeComboBox.SelectedItem as BankAccount;
            if (acc == null || !decimal.TryParse(SumDepositeTextBox.Text, out decimal sum))
            {
                MessageBox.Show("Full all fields correctly!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if (sum < 0)
            {
                MessageBox.Show("Sum must be positive!", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            if (acc.IsBlocked)
            {
                MessageBox.Show("This account is BLOCKED by manager.");
                return;
            }
            acc.Balance += sum;
            var transaction = new TransactionAction
            {
                UserIdFrom = current.IdUser,
                Type = TransactionType.Deposit,
                Amount = sum,
                TargetAccountId = acc.BankAccountId,
                Description = $"Deposite money: {sum} to {acc.Number}"
            };
            App.Database.Transactions.Add(transaction);
            App.Database.Save();
            MessageBox.Show("Deposite successful!");
            this.Close();

        }
    }
}
