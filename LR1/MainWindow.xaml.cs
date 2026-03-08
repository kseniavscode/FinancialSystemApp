using LR1.Models;
using LR1.Views;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LR1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private User _current;
        public MainWindow(User user)
        {
            InitializeComponent();
            _current = user;
            WelcomeTextBlock.Text = $"Welcome, {_current.Name}! Your role is {_current.Role}.";
            SetupInterface();
        }
        private void RefreshClientAccounts()
        {
            ClientAccountsGrid.ItemsSource = null;
            ClientAccountsGrid.ItemsSource = App.Database.Accounts.Where(a => a.OwnerId == _current.IdUser).ToList(); ;
        }
        private void SetupInterface()
        {
            AdminPanel.Visibility = Visibility.Collapsed;
            ManagerPanel.Visibility = Visibility.Collapsed;
            ClientPanel.Visibility = Visibility.Collapsed;

            if (_current.Role == UserRole.Admin)
            {
                AdminPanel.Visibility = Visibility.Visible;
                UsersListBox.ItemsSource = App.Database.Users;
                AdminGrid.ItemsSource = App.Database.Users.Where(x => (x.Status == ApprovalStatus.Approved || x.Status == ApprovalStatus.Rejected) && x.Role != UserRole.Admin && x.Role != UserRole.Manager).ToList();
            }
            if (_current.Role == UserRole.Manager)
            {
                ManagerPanel.Visibility = Visibility.Visible;
                ManagerGrid.ItemsSource = App.Database.Users.Where(x => x.Status == ApprovalStatus.Pending).ToList();
            }
            if (_current.Role == UserRole.Client)
            {
                ClientPanel.Visibility = Visibility.Visible;
                BanksListBox.ItemsSource = App.Database.Banks;
                RefreshClientAccounts();
            }
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void AddManagerButton_Click(object sender, RoutedEventArgs e)
        {
            AddManagerWindow addManagerWindow = new AddManagerWindow();
            addManagerWindow.Owner = this;
            addManagerWindow.ShowDialog();
            UsersListBox.Items.Refresh();
        }


        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;
            if (user != null)
            {
                user.Status = ApprovalStatus.Approved;
                App.Database.Save();
                ManagerGrid.ItemsSource = App.Database.Users.Where(x => x.Status == ApprovalStatus.Pending).ToList();
            }

        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;
            if (user != null)
            {
                user.Status = ApprovalStatus.Rejected;
                App.Database.Save();
                ManagerGrid.ItemsSource = App.Database.Users.Where(x => x.Status == ApprovalStatus.Pending).ToList();
            }
        }

        private void ShowUsersListButton_Click(object sender, RoutedEventArgs e)
        {
            UsersListBox.Visibility = Visibility.Visible;
            AdminGrid.Visibility = Visibility.Hidden;
        }

        private void ShowActionsButton_Click(object sender, RoutedEventArgs e)
        {
            UsersListBox.Visibility = Visibility.Hidden;
            AdminGrid.Visibility =Visibility.Visible;
        }

        private void ResetStatus_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as User;
            if (user != null)
            {
                user.Status = ApprovalStatus.Pending;
                App.Database.Save();
                AdminGrid.ItemsSource = null;
                AdminGrid.ItemsSource = App.Database.Users.Where(x => (x.Status == ApprovalStatus.Approved || x.Status == ApprovalStatus.Rejected) && x.Role != UserRole.Admin && x.Role != UserRole.Manager).ToList(); ;
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
            do {
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
    }
}