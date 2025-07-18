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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CAFEHOLIC.DAO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using service;

namespace CAFEHOLIC.view.Page
{
    /// <summary>
    /// Interaction logic for ForgotPasswordPage.xaml
    /// </summary>
    public partial class ForgotPasswordPage : System.Windows.Controls.Page
    {
        private AccountDAO accountDAO;
        private LoginWindown mainWindow;
       

        public ForgotPasswordPage(LoginWindown login,AccountDAO acc)
        {
            accountDAO = acc;
            mainWindow = login;
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            var phoneNumber = txtPhone.Text.Trim();
            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please enter your phone number.", "Error");
                return;
            }

            if (phoneNumber.Length != 10 || !phoneNumber.StartsWith("0"))
            {
                MessageBox.Show("Phone number must start with 0 and have 10 digits.", "Error");
                return;
            }

            if (!accountDAO.IsPhoneNumberExists(phoneNumber))
            {
                MessageBox.Show("No account found with this phone number.", "Error");
                return;
            }

            // Gửi OTP bằng Twilio
            string otp = accountDAO.GenerateOTP(phoneNumber);
            string message = $"Your verification code is: {otp}";

            string toPhone = "+84" + phoneNumber.Substring(1).Trim();

            MessageBox.Show(toPhone + " " + message, "Debug Info");

            var twilio = new TwilioSMSApi(
                ConfigurationManager.AppSettings["Twilio_SID"],
                ConfigurationManager.AppSettings["Twilio_Token"],
                ConfigurationManager.AppSettings["Twilio_From"]
            );

            bool success = twilio.SendSMS(toPhone, message);

            if (success)
            {
                MessageBox.Show("✅ OTP has been sent.", "Success");
                mainWindow.MainFrame.Navigate(new VerifyPage(mainWindow, accountDAO, otp, phoneNumber, "forgot"));
            }
            else
            {
                MessageBox.Show("❌ Failed to send OTP.", "Error");
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Navigate(new LoginPage(mainWindow, accountDAO));
        }
    }
}
