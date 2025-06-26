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
using CAFEHOLIC.dao;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;

namespace CAFEHOLIC.view
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private AccountDAO accDAO;
        private UserDAO cusDAO;
        public Register(AccountDAO accdao)
        {
            accDAO = accdao;
            cusDAO = new UserDAO(new DBContext(), new DBContext().GetLogger<UserDAO>());
            InitializeComponent();
        }

        private void btn_register(object sender, RoutedEventArgs e)
        {
            String phoneNumber = txtPhone.Text.Trim();
            String username = txtUsername.Text.Trim();
            String password = txtPassword.Password.Trim();
            String confirmPassword = txtConfirmPassword.Password.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill all in register form!", "Notify");
                return;
            }
            if (password != confirmPassword)
            {
                MessageBox.Show("Your PASSWORD and CONFIRM PASSWORD are not same!", "Notify");
                return;
            }
            if (phoneNumber.Length != 10 || !phoneNumber.StartsWith("0"))
            {
                MessageBox.Show("Your phone number is not valid!(Phone number with length 10 and start with 0)\n Please check again!", "Notify");
                return;
            }
            if (accDAO.CheckLogin(phoneNumber, password) != null)
            {
                MessageBox.Show("Had have account with Phone number: "+phoneNumber+".\n You can get your passowrd if forgot in login screen.", "Lỗi");
                return;
            }
            User newUser= new UserDAO(new DBContext(), new DBContext().GetLogger<UserDAO>()).CreateUser(phoneNumber, username, password);
            Account newAccount = new AccountDAO(new DBContext(), new DBContext().GetLogger<AccountDAO>()).CreateAccount(phoneNumber, password,newUser.Id);
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng RegisterWindow

            if (this.Owner != null)
            {
                this.Owner.Show(); // Hiện lại MainWindow
            }
        }
    }
}
