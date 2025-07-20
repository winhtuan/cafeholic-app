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
using CAFEHOLIC.view.Page.Admin;

namespace CAFEHOLIC.view
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AdminPage());
        }
        private void AdminPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AdminPage());
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindown login = new LoginWindown();
            login.Show();
            this.Close();
        }
    }
}
