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
        public VerifyPage(LoginWindown login,AccountDAO acdao, String otp)
        {
            accountDAO = acdao;
            otpCode = otp;
            mainWindow = login;
            InitializeComponent();
        }

        private void btn_otp(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Navigate(new LoginPage(mainWindow, accountDAO));
        }
    }
}
