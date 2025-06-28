using System;
using System.Collections.Generic;
using System.Configuration;
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
using Newtonsoft.Json;
using service;

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

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var phoneNumber = txtPhone.Text.Trim();
            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please enter your phone number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (phoneNumber.Length != 10 || !phoneNumber.StartsWith("0"))
            {
                MessageBox.Show("Phone number must start with 0 and have 10 digits.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!accountDAO.IsPhoneNumberExists(phoneNumber))
            {
                MessageBox.Show("No account found with this phone number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Gửi OTP
            string[] phoneNumbersArray = new string[] { "84334354406" };
            string OTP = accountDAO.GenerateOTP(phoneNumber);
            string content = $"Your verification code is: {OTP}. Please use this code to reset your password.";

            var smsApi = new SpeedSMSApi(ConfigurationManager.AppSettings["SMS_key"]);
            string responseJson = await smsApi.SendSMS(phoneNumbersArray, content, 2, "");

            dynamic result = JsonConvert.DeserializeObject(responseJson);
            string status = result.status;

            if (status == "success")
            {
                // Hiển thị thành công & mở cửa sổ nhập mã
                MessageBox.Show("✅ OTP has been sent to your phone number.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                VeriWindow verificationWindow = new VeriWindow(accountDAO, OTP);
                verificationWindow.Owner = this;
                verificationWindow.Show();
                this.Hide();
            }
            else
            {
                string message = result.message;
                MessageBox.Show($"❌ Failed to send SMS: {message}", "SMS Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
