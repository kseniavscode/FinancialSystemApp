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
using LR1.Models;
using LR1.Views;

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
                var myAccounts = App.Database.Accounts.Where(a => a.OwnerId == _current.IdUser).ToList();

                ClientAccountsGrid.ItemsSource = null;
                ClientAccountsGrid.ItemsSource = myAccounts;
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

        private void ManagerGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        }

        private void Transfer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}