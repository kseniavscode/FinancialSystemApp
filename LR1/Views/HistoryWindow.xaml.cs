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
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        User current; 
        public HistoryWindow(User user)
        {
            InitializeComponent();
            current = user;
            HistoryDataGrid.ItemsSource = App.Database.Transactions.Where(x => x.UserIdTo == current.IdUser || x.UserIdFrom == current.IdUser).Reverse().ToList();
        }
    }
}
