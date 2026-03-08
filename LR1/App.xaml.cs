using LR1.Models;
using LR1.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace LR1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DataStorage Database { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Database = DataStorage.Load();
            if (Database.Users.Count == 0)
            {
                User adminUser = new User("admin", "1234", UserRole.Admin);
                adminUser.Approve();
                Database.Users.Add(adminUser);
                Database.Save();

            }
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }

}
