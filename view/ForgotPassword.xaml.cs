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
using CAFEHOLIC.DAO;

namespace CAFEHOLIC.view
{
    /// <summary>
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : Window
    {
        private AccountDAO accountDAO;
        public ForgotPassword(AccountDAO acc)
        {
            accountDAO = acc;
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var phoneNumber = txtPhone.Text.Trim();
            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please enter your phone number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (phoneNumber.Length != 10 || !phoneNumber.StartsWith("0"))
            {
                MessageBox.Show("Your phone number is not valid! (Phone number with length 10 and start with 0)\nPlease check again!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (accountDAO.IsPhoneNumberExists(phoneNumber))
            {
                // Gửi mã xác minh đến số điện thoại
                //string verificationCode = accountDAO.SendVerificationCode(phoneNumber);
                //if (!string.IsNullOrEmpty(verificationCode))
                //{
                //    MessageBox.Show("A verification code has been sent to your phone number.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //    // Mở cửa sổ nhập mã xác minh
                //    VerificationWindow verificationWindow = new VerificationWindow(accountDAO, phoneNumber, verificationCode);
                //    verificationWindow.Owner = this; // Đặt cửa sổ hiện tại là chủ sở hữu
                //    verificationWindow.Show();
                //    this.Hide(); // Ẩn cửa sổ ForgotPassword
                //}
                //else
                //{
                //    MessageBox.Show("Failed to send verification code. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
            }
            else
            {
                MessageBox.Show("No account found with this phone number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
