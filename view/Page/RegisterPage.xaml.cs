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
using CAFEHOLIC.Model;
using Newtonsoft.Json;
using Twilio.TwiML.Messaging;

namespace CAFEHOLIC.view.Page
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : System.Windows.Controls.Page
    {
        private AccountDAO accDAO;
        private UserDAO cusDAO;
        private LoginWindown mainWindow;

        public RegisterPage(LoginWindown login,AccountDAO accdao)
        {
            mainWindow = login;
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
                MessageBox.Show("Had have account with Phone number: " + phoneNumber + ".\n You can get your passowrd if forgot in login screen.", "Lỗi");
                return;
            }

            string otp = accDAO.GenerateOTP(phoneNumber);

            // Gửi SMS OTP
            string toPhone = "+84" + phoneNumber.Substring(1).Trim();
            string content = $"Your verification code is: {otp}. Please use this code to verify your phone number.";

            // Gọi SpeedSMS (hoặc Twilio nếu bạn dùng)
            var twilio = new TwilioSMSApi(
                ConfigurationManager.AppSettings["Twilio_SID"],
                ConfigurationManager.AppSettings["Twilio_Token"],
                ConfigurationManager.AppSettings["Twilio_From"]
            );

            bool status = twilio.SendSMS(toPhone, content);

            if (status)
            {
                MessageBox.Show("✅ OTP has been sent. Please verify your phone number.", "SMS Sent");

                // Mở VerifyPage để nhập OTP
                mainWindow.MainFrame.Navigate(new VerifyPage(mainWindow, accDAO, otp, phoneNumber, "register",Name,password));
                return;
            }
            else
            {
                MessageBox.Show($"❌ Failed to send OTP", "Error");
                return;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Navigate(new LoginPage(mainWindow, accDAO));
        }
    }
}
