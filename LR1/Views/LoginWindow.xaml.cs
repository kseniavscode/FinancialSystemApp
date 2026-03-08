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

namespace LR1.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            var user = App.Database.Users.FirstOrDefault(x => x.Name == login && x.Password == password);

            if (user != null)
            {
                if (user.Status == Models.ApprovalStatus.Pending)
                {
                    MessageBox.Show("Your registration has not been confirmed!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if(user.Status == Models.ApprovalStatus.Rejected)
                {
                    MessageBox.Show("You have been denied registartion!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Not correct login or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.ShowDialog();
        }
    }
}
