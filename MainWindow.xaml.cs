using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CAFEHOLIC.dao;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.service;
using CAFEHOLIC.view;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC
{
    public static class AppSession
    {
        public static int CurrentUserId { get; set; } = 1;
        public static readonly ILogger<ProductService> logger=new DBContext().GetLogger<ProductService>();
    }

    public partial class MainWindow : Window
    {
        // Khởi tạo DAO cho Customer
        private AccountDAO accDAO;

        public MainWindow()
        {
            accDAO= new AccountDAO(new DBContext(),new DBContext().GetLogger<AccountDAO>());
            InitializeComponent();
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
                HomeWindown homeWindow = new HomeWindown();
                homeWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Đăng nhập thất bại! Vui lòng kiểm tra lại thông tin đăng nhập.", "Lỗi");
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var registerWindow = new Register(accDAO);
            registerWindow.Owner = this;
            registerWindow.ShowDialog();

            this.Show();
        }

        private void btnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            var frogotWindow = new ForgotPassword(accDAO);
            frogotWindow.Owner = this;
            frogotWindow.ShowDialog();

            this.Show();
        }

    }
}