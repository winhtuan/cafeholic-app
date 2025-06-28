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
    /// Interaction logic for VeriWindow.xaml
    /// </summary>
    public partial class VeriWindow : Window
    {
        private AccountDAO accountDAO;
        private string otpCode;
        public VeriWindow(AccountDAO acdao, String otp)
        {
            accountDAO = acdao;
            otpCode = otp;
            InitializeComponent();
        }

        private void btn_otp(object sender, RoutedEventArgs e)
        {

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
