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

        }
    }
}
