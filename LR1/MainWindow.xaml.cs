using LR1.Models;
using LR1.Views;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LR1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User _current;
        private DispatcherTimer _timer;
        public MainWindow(User user)
        {
            InitializeComponent();
            _current = user;
            WelcomeTextBlock.Text = $"Welcome, {_current.Name}! Your role is {_current.Role}.";
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            RefreshClientAccounts();
            SetupInterface();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ClientDepositsGrid == null || ClientDepositsGrid.ItemsSource == null)
                return;
            int selectedIndex = ClientDepositsGrid.SelectedIndex;

            try
            {
                ClientDepositsGrid.Items.Refresh();
                if (selectedIndex >= 0 && selectedIndex < ClientDepositsGrid.Items.Count)
                {
                    ClientDepositsGrid.SelectedIndex = selectedIndex;
                }
            }
            catch (Exception) { }
        }
        private void RefreshClientAccounts()
        {
            if (_current.Role == UserRole.Client)
            {
                ClientAccountsGrid.ItemsSource = null;
                ClientAccountsGrid.ItemsSource = App.Database.Accounts.Where(a => a.OwnerId == _current.IdUser && a.Type == BankAccountType.Checking).ToList();
                ClientDepositsGrid.ItemsSource = null;
                ClientDepositsGrid.ItemsSource = App.Database.Deposits.Where(d => d.OwnerId == _current.IdUser).ToList();
            }
        }
        private void RefreshManagerData()
        {
            if (_current.Role == UserRole.Manager)
            {
                PendingUsersGrid.ItemsSource = App.Database.Users.Where(u => u.Status == ApprovalStatus.Pending).ToList();
                PendingSalaryRequestsGrid.ItemsSource = App.Database.SalaryRequests.Where(r => r.Status == ApprovalStatus.Pending).ToList();
            }
        }

        private void RefreshAdminData()
        {
            if (_current.Role == UserRole.Admin)
            {
                UsersListBox.ItemsSource = App.Database.Users.Where(u => u.IdUser != _current.IdUser).ToList();
                AdminGrid.ItemsSource = App.Database.Users.Where(x => (x.Status == ApprovalStatus.Approved || x.Status == ApprovalStatus.Rejected) && x.Role == UserRole.Client).ToList();
                AllTransactionsGrid.ItemsSource = App.Database.Transactions.OrderByDescending(x => x.DateTime).ToList();
            }
        }
        private void SetupInterface()
        {
            AdminPanel.Visibility = Visibility.Collapsed;
            ManagerPanel.Visibility = Visibility.Collapsed;
            ClientPanel.Visibility = Visibility.Collapsed;

            if (_current.Role == UserRole.Admin)
            {
                AdminPanel.Visibility = Visibility.Visible;
                RefreshAdminData();
                
            }
            if (_current.Role == UserRole.Manager)
            {
                ManagerPanel.Visibility = Visibility.Visible;
                RefreshManagerData();
            }
            if (_current.Role == UserRole.Client)
            {
                ClientPanel.Visibility = Visibility.Visible;
                BanksListBox.ItemsSource = App.Database.Banks;
                EnterprisesListBox.ItemsSource = App.Database.Enterprises;
                UpdateSalaryUI();
                RefreshClientAccounts();
            }
        }
        private void UpdateSalaryUI()
        {
            var approvedRequest = App.Database.SalaryRequests.FirstOrDefault(r => r.UserId == _current.IdUser && r.Status == ApprovalStatus.Approved);
            GetSalaryButton.Visibility = approvedRequest != null ? Visibility.Visible : Visibility.Collapsed;
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        



        private void ApproveUserInline_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;
            if (user != null)
            {
                user.Status = ApprovalStatus.Approved;
                App.Database.Save();
                RefreshManagerData();
            }
        }

        private void RejectUserInline_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;
            if (user != null)
            {
                user.Status = ApprovalStatus.Rejected;
                App.Database.Save();
                RefreshManagerData();
            }
        }

        private void ApproveSalaryInline_Click(object sender, RoutedEventArgs e)
        {
            var salaryRequest = (sender as Button).DataContext as SalaryRequest;
            if (salaryRequest != null)
            {
                salaryRequest.Status = ApprovalStatus.Approved;

                var enterprise = App.Database.Enterprises.FirstOrDefault(x => x.Id == salaryRequest.EnterpriseId);
                if (enterprise != null && !enterprise.EmployeeIds.Contains(salaryRequest.UserId))
                {
                    enterprise.EmployeeIds.Add(salaryRequest.UserId);
                }
                App.Database.Save();
                RefreshManagerData();
            }
        }

        private void RejectSalaryInline_Click(object sender, RoutedEventArgs e)
        {
            var salaryRequest = (sender as Button).DataContext as SalaryRequest;
            if (salaryRequest != null)
            {
                salaryRequest.Status = ApprovalStatus.Rejected;

                App.Database.Save();
                RefreshManagerData();
            }
        }


        
        
        private void ResetStatus_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;
            if (user != null)
            {
                user.Status = ApprovalStatus.Pending;
                App.Database.Save();
                RefreshAdminData();
                MessageBox.Show($"Status for {user.Name} reset to Pending.");
            }
        }
        private void AddManagerButton_Click(object sender, RoutedEventArgs e)
        {
            AddManagerWindow addManagerWindow = new AddManagerWindow();
            addManagerWindow.Owner = this;
            if (addManagerWindow.ShowDialog() == true)
            {
                RefreshAdminData();
            }
        }

        private void RollbackTransaction_Click(object sender, RoutedEventArgs e)
        {
            var transaction = (sender as Button).DataContext as TransactionAction;
            if (transaction == null || transaction.IsCancelled) return;
            var result = MessageBox.Show(
                $"Are you sure you want to rollback this transaction?\n" +
                $"Amount: {transaction.Amount:F2}\n" +
                $"Description: {transaction.Description}",
                "Confirm Rollback",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            var sourceAcc = App.Database.Accounts.FirstOrDefault(a => a.BankAccountId == transaction.SourceAccountId);
            var targetAcc = App.Database.Accounts.FirstOrDefault(a => a.BankAccountId == transaction.TargetAccountId);
            try
            {
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
                    if (sourceAcc != null) sourceAcc.Balance += transaction.Amount;
                    if (targetAcc != null) targetAcc.Balance -= transaction.Amount;
                }
                
                transaction.IsCancelled = true;
                transaction.Description = "[ROLLED BACK] " + transaction.Description;
                App.Database.Save();

                AllTransactionsGrid.ItemsSource = null;
                AllTransactionsGrid.ItemsSource = App.Database.Transactions.OrderByDescending(t => t.DateTime).ToList();

                MessageBox.Show("Transaction has been successfully reversed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during rollback: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void OpenAccount_Click(object sender, RoutedEventArgs e)
        {
            var selectedBank = BanksListBox.SelectedItem as Bank;
            if (selectedBank == null)
            {
                MessageBox.Show("Please, select a bank in the BANKS tab first!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string number;
            Random rnd = new Random();
            do
            {
                number = "BY" + rnd.Next(10000, 99999).ToString();
                if (App.Database.Accounts.FirstOrDefault(x => x.Number == number) == null)
                {
                    break;
                }
            } while (true);

            var new_account = new BankAccount(number, _current.IdUser, selectedBank.BankId, BankAccountType.Checking);
            App.Database.Accounts.Add(new_account);
            App.Database.Save();
            RefreshClientAccounts();
            MessageBox.Show($"Account {new_account.Number} successfully opened in {selectedBank.Name}!");
        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            TransferWindow transferWindow = new TransferWindow(_current);
            transferWindow.Owner = this;
            transferWindow.ShowDialog();
            RefreshClientAccounts();
        }

        private void DepositeAccount_Click(object sender, RoutedEventArgs e)
        {
            DepositeWindow depositeWindow = new DepositeWindow(_current);
            depositeWindow.Owner = this;
            depositeWindow.ShowDialog();
            RefreshClientAccounts();
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow historyWindow = new HistoryWindow(_current);
            historyWindow.Owner = this;
            historyWindow.ShowDialog();
            RefreshClientAccounts();
        }


        private void TransferDeposit_Click(object sender, RoutedEventArgs e)
        {

            var selectedDeposit = ClientDepositsGrid.SelectedItem as DepositAccount;
            if (selectedDeposit == null)
            {
                MessageBox.Show("Please select a deposit to close!");
                return;
            }

            if (DateTime.Now < selectedDeposit.EndDate)
            {
                var result = MessageBox.Show(
                    $"The term is not over yet! If you close now, you will LOSE all interest.\n" +
                    $"Expected end: {selectedDeposit.EndDate:HH:mm:ss}\n" +
                    "Do you want to proceed?",
                    "Early Closure",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No) return;
            }
            decimal profit = selectedDeposit.CalculateCurrentInterest();
            decimal finalAmount = selectedDeposit.Balance + profit;

            var mainAccount = App.Database.Accounts.FirstOrDefault(a => a.OwnerId == _current.IdUser && a.Number == selectedDeposit.NumberAccount);
            if (mainAccount == null)
            {
                MessageBox.Show("Main account not found! Cannot transfer funds. Choose another account");
                ChooseAccountWindow chooseAccountWindow = new ChooseAccountWindow(_current);
                chooseAccountWindow.Owner = this;
                if (chooseAccountWindow.ShowDialog() == true || chooseAccountWindow.ChooseAccountComboBox.SelectedItem != null)
                {
                    mainAccount = chooseAccountWindow.ChooseAccountComboBox.SelectedItem as BankAccount;
                }
                if (mainAccount == null)
                {
                    MessageBox.Show("Transfer cancelled. No target account selected.");
                    return;
                }
            }


            mainAccount.Balance += finalAmount;
            App.Database.Deposits.Remove(selectedDeposit);
            App.Database.Transactions.Add(new TransactionAction
            {
                UserIdTo = _current.IdUser,
                Type = TransactionType.TransferDeposit,
                SourceAccountId = selectedDeposit.BankAccountId,
                TargetAccountId = mainAccount.BankAccountId,
                Amount = finalAmount,
                Description = $"Close deposit {selectedDeposit.Number}. Profit: {profit:C}"
            });

            App.Database.Save();

            RefreshClientAccounts();
            MessageBox.Show($"Deposit closed!\nTransferred to account {mainAccount.Number}: {finalAmount:C}\n(Profit: {profit:C})");
        }



        private void OpenDeposite_Click(object sender, RoutedEventArgs e)
        {
            AddDepositWindow addDepositWindow = new AddDepositWindow(_current);
            addDepositWindow.Owner = this;
            addDepositWindow.ShowDialog();
            RefreshClientAccounts();
        }

        private void CloseAccount_Click(object sender, RoutedEventArgs e)
        {
            var acc = ClientAccountsGrid.SelectedItem as BankAccount;
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
                RefreshClientAccounts();
            }
        }

        private void ApplyForSalary_Click(object sender, RoutedEventArgs e)
        {
            var enterprise = EnterprisesListBox.SelectedItem as Enterprise;
            if (enterprise == null)
            {
                MessageBox.Show("Select an enterprise!");
                return;
            }
            if (App.Database.SalaryRequests.Any(r => r.UserId == _current.IdUser && r.EnterpriseId == enterprise.Id))
            {
                MessageBox.Show("Application already submitted!");
                return;
            }
            App.Database.SalaryRequests.Add(new SalaryRequest(_current.IdUser, enterprise.Id));
            App.Database.Save();
            MessageBox.Show("Application sent to manager!");
        }

        private void GetSalary_Click(object sender, RoutedEventArgs e)
        {
            var approvedRequest = App.Database.SalaryRequests.FirstOrDefault(r => r.UserId == _current.IdUser && r.Status == ApprovalStatus.Approved);

            if (approvedRequest == null)
            {
                MessageBox.Show("Application sent to manager! Just wait.");
                return;
            }
            var enterprise = App.Database.Enterprises.FirstOrDefault(ent => ent.Id == approvedRequest.EnterpriseId);
            if (enterprise == null) return;


            ChooseAccountWindow chooseWin = new ChooseAccountWindow(_current);
            if (chooseWin.ShowDialog() == true)
            {
                var targetAcc = chooseWin.ChooseAccountComboBox.SelectedItem as BankAccount;
                if (targetAcc != null)
                {
                    Random rnd = new Random();

                    double randomFactor = rnd.NextDouble();
                    decimal range = enterprise.MaxSalary - enterprise.MinSalary;
                    decimal calculatedSalary = enterprise.MinSalary + (range * (decimal)randomFactor);
                    calculatedSalary = Math.Round(calculatedSalary, 2);
                    targetAcc.Balance += calculatedSalary;

                    App.Database.Transactions.Add(new TransactionAction
                    {
                        UserIdTo = _current.IdUser,
                        Type = TransactionType.SalaryPayment,
                        Amount = calculatedSalary,
                        TargetAccountId = targetAcc.BankAccountId,
                        Description = $"Salary from {enterprise.Name} to {targetAcc.Number}"
                    });

                    App.Database.Save();
                    RefreshClientAccounts();

                    MessageBox.Show($"Congratulations!\nYour salary from '{enterprise.Name}' this month is: {calculatedSalary:C}\nCredited to: {targetAcc.Number}");
                }

               

            }
        }

        
    }
}