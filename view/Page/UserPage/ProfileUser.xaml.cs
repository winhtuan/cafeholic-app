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
using CAFEHOLIC.service;
using CAFEHOLIC.view.Dialog;

namespace CAFEHOLIC.view.Page.UserPage
{
    /// <summary>
    /// Interaction logic for ProfileUser.xaml
    /// </summary>
    public partial class ProfileUser : System.Windows.Controls.Page
    {
        public ProfileUser()
        {
            InitializeComponent();

            if (AppSession.CurrentUser != null)
            {
                txtFullName.Text = AppSession.CurrentUser.FullName ?? "N/A";
                txtEmail.Text = AppSession.CurrentUser.Email ?? "N/A";
                txtPhone.Text = AppSession.CurrentUser.PhoneNumber ?? "N/A";
            }
            else
            {
                txtFullName.Text = "Not logged in";
                txtEmail.Text = "Not logged in";
                txtPhone.Text = "Not logged in";
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentUser = AppSession.CurrentUser;
                if (currentUser == null)
                {
                    MessageBox.Show("Vui lòng đăng nhập trước khi chỉnh sửa thông tin!", "Thông báo");
                    return;
                }

                var dialog = new UserDialog(currentUser, "Chỉnh sửa thông tin cá nhân");
                if (dialog.ShowDialog() == true)
                {
                    var updatedUser = dialog.User;
                    var updatedAccount = dialog.Account;

                    var userService = new UserService();
                    userService.UpdateUserWithAccount(updatedUser, updatedAccount);

                    Logger.Info(nameof(ProfileUser), $"Cập nhật người dùng ID {updatedUser.Id} - {updatedUser.FullName}");

                    // Cập nhật lại thông tin hiển thị
                    txtFullName.Text = updatedUser.FullName ?? "N/A";
                    txtEmail.Text = updatedUser.Email ?? "N/A";
                    txtPhone.Text = updatedUser.PhoneNumber ?? "N/A";

                    AppSession.CurrentUser = updatedUser;

                    MessageBox.Show("Cập nhật thông tin thành công!", "Thành công");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(ProfileUser), "Lỗi khi cập nhật thông tin cá nhân", ex);
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi");
            }
        }
    }
}
