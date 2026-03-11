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
    /// Логика взаимодействия для AddManagerWindow.xaml
    /// </summary>
    public partial class AddManagerWindow : Window
    {
        public AddManagerWindow()
        {
            InitializeComponent();
        }

        private void CreateManagerButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginManagerTextBox.Text;
            string password = PasswordManagerTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Enter all the information");
                return;
            }
            if (password.Contains(" ") || password.Length < 4)
            {
                MessageBox.Show("Enter normal password without spaces and more then 4 symbols");
                return;
            }
            if (!login.All(char.IsLetter) || login.Length < 4)
            {
                MessageBox.Show("Enter normal name contains only from letters, more then 4 symbols");
                return;
            }
            bool userExists = App.Database.Users.Any(u => u.Name == login);
            if (userExists)
            {
                MessageBox.Show("A manager with this login already exists!");
                return;
            }

            User manager = new User(login, password, UserRole.Manager);
            manager.Approve();
            App.Database.Users.Add(manager);
            App.Database.Save();
            MessageBox.Show("A manager was added successfully!");
            this.Close();
        }
    }
}
