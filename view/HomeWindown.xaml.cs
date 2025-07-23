using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CAFEHOLIC.view.Page;
using CAFEHOLIC.view.Page.UserPage;

namespace CAFEHOLIC.view
{
    public partial class HomeWindown : Window
    {
        public HomeWindown()
        {
            InitializeComponent();
            MainFrame.Navigate(new view.Page.HomePage());
        }
        private void Homepage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HomePage());
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindown login = new LoginWindown();
            login.Show();
            this.Close();
        }

        private void GoToRoomPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RoomPage());
        }

        private void GoToMenuPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MenuPage());
        }

        private void ProfilePage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfileUser());
        }
    }
}
