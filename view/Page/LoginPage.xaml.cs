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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;

namespace CAFEHOLIC.view.Page
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : System.Windows.Controls.Page
    {
        private AccountDAO accDAO;
        private LoginWindown LoginWindown;

        public LoginPage(LoginWindown window, AccountDAO dao)
        {
            InitializeComponent();
            LoginWindown = window;
            accDAO = dao;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Account acc = accDAO.CheckLogin(txtUsername.Text, txtPassword.Password);
            if (acc != null)
            {
                AppSession.CurrentUserId = acc.AccId;
                new HomeWindown().Show();
                LoginWindown.Close();
            }
            else
            {
                MessageBox.Show("Login failed!");
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            LoginWindown.MainFrame.Navigate(new RegisterPage(LoginWindown, accDAO));
        }

        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            LoginWindown.MainFrame.Navigate(new ForgotPasswordPage(LoginWindown, accDAO));
        }
    }

}
