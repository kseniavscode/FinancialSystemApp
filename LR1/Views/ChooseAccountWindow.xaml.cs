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
    /// Логика взаимодействия для ChooseAccountWindow.xaml
    /// </summary>
    public partial class ChooseAccountWindow : Window
    {
        User current;
        public ChooseAccountWindow(User current)
        {
            InitializeComponent();
            this.current = current;
            ChooseAccountComboBox.ItemsSource = App.Database.Accounts.Where(x => x.OwnerId == current.IdUser && !x.IsBlocked).ToList();
        }

        private void CloseDepositButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
