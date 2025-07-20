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
using CAFEHOLIC.Utils;
namespace CAFEHOLIC.view.Page.Admin
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : System.Windows.Controls.Page
    {
        public AdminPage()
        {
            InitializeComponent();
            Logger.Info(nameof(AdminPage), "AdminPage khởi tạo thành công");
        }

        private void ManageUser_Checked(object sender, RoutedEventArgs e)
        {
            // Điều hướng đến trang quản lý user
            Logger.Info(nameof(AdminPage), "Điều hướng đến trang ManageUserPage");
            this.NavigationService?.Navigate(new ManageUserPage());
        }

        private void ManageStaff_Checked(object sender, RoutedEventArgs e)
        {
            // Điều hướng đến trang quản lý staff
            Logger.Info(nameof(AdminPage), "Điều hướng đến trang ManageStaffPage");
            this.NavigationService?.Navigate(new ManageStaffPage());
        }

        private void ManageRoom_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void ManageDrink_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void Booking_Checked(object sender, RoutedEventArgs e)
        {

        }

    }
}
