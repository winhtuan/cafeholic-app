using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
using CAFEHOLIC.dao;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using Twilio.Types;

namespace CAFEHOLIC.view.Page
{
    /// <summary>
    /// Interaction logic for VerifyPage.xaml
    /// </summary>
    public partial class VerifyPage : System.Windows.Controls.Page
    {
        private AccountDAO accountDAO;
        private string otpCode;
        private LoginWindown mainWindow;
        private String phone;
        private String state;
        private String name;
        private String pass;
        public VerifyPage(LoginWindown login,AccountDAO acdao, String otp,String phone,String state)
        {
            accountDAO = acdao;
            otpCode = otp;
            mainWindow = login;
            this.phone = phone;
            this.state = state;
            InitializeComponent();
        }
        public VerifyPage(LoginWindown login, AccountDAO acdao, String otp, String phone, String state, String name, String pass)
        {
            accountDAO = acdao;
            otpCode = otp;
            mainWindow = login;
            this.phone = phone;
            this.state = state;
            this.name = name;
            this.pass = pass;
            InitializeComponent();
        }

        private void btn_otp(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtOTP.Text))
            {
                MessageBox.Show("Please enter the OTP code.", "Error");
                return;
            }
            if (txtOTP.Text == otpCode)
            {
                MessageBox.Show("OTP verified successfully!", "Success");
                if (state == "register")
                {
                    User newUser = new UserDAO(new DBContext(), new DBContext().GetLogger<UserDAO>()).CreateUser(phone, name, pass);
                    Account newAccount = new AccountDAO(new DBContext(), new DBContext().GetLogger<AccountDAO>()).CreateAccount(phone, pass, newUser.Id);
                    if (newAccount != null)
                    {
                        MessageBox.Show("Register succcessfull!", "Notify");
                        btnCancel_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Register fail!", "Lỗi");
                    }
                }
                else if (state == "forgot")
                    mainWindow.MainFrame.Navigate(new ResetPasswordPage(mainWindow, accountDAO, phone));
            }
            else
            {
                MessageBox.Show("Invalid OTP code. Please try again.", "Error");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Navigate(new LoginPage(mainWindow, accountDAO));
        }
    }
}
