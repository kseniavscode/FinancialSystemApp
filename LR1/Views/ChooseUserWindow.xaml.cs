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
    /// Логика взаимодействия для ChooseUserWindow.xaml
    /// </summary>
    public partial class ChooseUserWindow : Window
    {
        public Models.User SelectedUser { get; private set; }
        public ChooseUserWindow()
        {
            InitializeComponent();
            UsersListManager.ItemsSource = App.Database.Users.Where(x => x.Role == Models.UserRole.Client).ToList();
        }

        private void ChooseUser_Click(object sender, RoutedEventArgs e)
        {
            SelectedUser = UsersListManager.SelectedItem as Models.User;

            if (SelectedUser == null)
            {
                MessageBox.Show("Please select a user first!");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
