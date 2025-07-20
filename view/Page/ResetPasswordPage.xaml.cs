using System.Windows;
using System.Windows.Controls;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
namespace CAFEHOLIC.view.Page
{
    public partial class ResetPasswordPage : System.Windows.Controls.Page
    {
        private LoginWindown mainWindow;
        private AccountDAO accountDAO;
        private string phone;

        public ResetPasswordPage(LoginWindown login, AccountDAO accDao, string phone)
        {
            InitializeComponent();
            mainWindow = login;
            accountDAO = accDao;
            this.phone = phone;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtNewPassword.Password.Trim();
            string confirmPassword = txtConfirmPassword.Password.Trim();

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please enter both password fields.", "Error");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error");
                return;
            }

            // Cập nhật mật khẩu
            bool updated = accountDAO.UpdatePasswordByPhone(phone, newPassword);
            if (updated)
            {
                MessageBox.Show("Password reset successfully!", "Success");
                mainWindow.MainFrame.Navigate(new LoginPage(mainWindow, accountDAO));
            }
            else
            {
                MessageBox.Show("Reset failed. Please try again.", "Error");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Navigate(new LoginPage(mainWindow, accountDAO));
        }
    }
}
