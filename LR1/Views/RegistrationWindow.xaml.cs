using LR1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            string login = RegLoginTextBox.Text;
            string password = RegPasswordTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Enter all the information");
                return;
            }
            bool userExists = App.Database.Users.Any(u => u.Name == login);
            if (userExists)
            {
                MessageBox.Show("A client with this login already exists!");
                return;
            }

            User client = new User(login, password, UserRole.Client);
            App.Database.Users.Add(client);
            App.Database.Save();
            MessageBox.Show("Application for registration has been submitted. Wait for the manager`s confirmation!");
            this.Close();
        }
    }
}
