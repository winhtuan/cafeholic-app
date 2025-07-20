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
using CAFEHOLIC.Model;
using CAFEHOLIC.service;
using CAFEHOLIC.view.Dialog;
using CAFEHOLIC.Utils;

namespace CAFEHOLIC.view.Page
{
    /// <summary>
    /// Interaction logic for ManageUserPage.xaml
    /// </summary>
    public partial class ManageUserPage : System.Windows.Controls.Page
    {
        private readonly UserService _userService;
        private List<User> listUser;
        public ManageUserPage()
        {
            InitializeComponent();
            _userService = new UserService();
            Logger.Info(nameof(ManageUserPage), "Trang quản lý khách hàng được khởi tạo");
            loadUserList();
        }

        private void loadUserList()
        {
            try
            {
                listUser = _userService.GetUserList();
                dataGridUser.ItemsSource = listUser;
                Logger.Info(nameof(ManageUserPage), $"Tải danh sách {listUser.Count} khách hàng thành công");
            }
            catch (Exception ex) {
                Logger.Error(nameof(ManageUserPage), "Lỗi khi tải danh sách khách hàng", ex);
                MessageBox.Show(ex.Message, "Error to load list user"); }
            
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterStaffList(txtSearch.Text.Trim());
        }

        private void FilterStaffList(string keyword)
        {
            if (listUser == null) return;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                dataGridUser.ItemsSource = listUser;
            }
            else
            {
                var filtered = listUser
                    .Where(s => (s.FullName != null && s.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                                (s.PhoneNumber != null && s.PhoneNumber.Contains(keyword)))
                    .ToList();
                dataGridUser.ItemsSource = filtered;
                txtNoResult.Visibility = filtered.Any() ? Visibility.Collapsed : Visibility.Visible;

                Logger.Info(nameof(ManageUserPage), $"Lọc danh sách khách hàng với từ khóa: '{keyword}', kết quả: {filtered.Count}");
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new UserDialog(null, "Thêm nhân viên");
                if (dialog.ShowDialog() == true)
                {
                    var newUser = dialog.User;
                    var newAccount = dialog.Account;

                    _userService.CreateUserWithAccount(newUser, newAccount);
                    Logger.Info(nameof(ManageUserPage), $"Thêm khách hàng mới: {newUser.FullName} - {newUser.Email}");

                    loadUserList();
                }
            }
            catch (Exception ex) {
                Logger.Error(nameof(ManageUserPage), "Lỗi khi thêm khách hàng", ex);
                MessageBox.Show(ex.Message, "Error Add"); }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = (sender as Button)?.DataContext as User;
                if (selected == null) return;

                var dialog = new UserDialog(selected, "Chỉnh sửa nhân viên");
                if (dialog.ShowDialog() == true)
                {
                    var updatedUser = dialog.User;
                    var updatedAccount = dialog.Account;

                    _userService.UpdateUserWithAccount(updatedUser, updatedAccount);
                    Logger.Info(nameof(ManageUserPage), $"Cập nhật khách hàng ID {updatedUser.Id} - {updatedUser.FullName}");

                    loadUserList();
                }
            }
            catch (Exception ex) {
                Logger.Error(nameof(ManageUserPage), "Lỗi khi cập nhật khách hàng", ex);
                MessageBox.Show(ex.Message, "Error Add"); }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var staff = (sender as Button)?.DataContext as User;
                if (staff == null) return;

                var result = MessageBox.Show($"Bạn có chắc muốn xóa khách hàng {staff.FullName}?",
                                             "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _userService.DeleteUser(staff.Id);
                    Logger.Warn(nameof(ManageUserPage), $"Đã xóa khách hàng ID {staff.Id} - {staff.FullName}");

                    loadUserList();
                }
            }
            catch (Exception ex) {
                Logger.Error(nameof(ManageUserPage), "Lỗi khi xóa khách hàng", ex);
                MessageBox.Show(ex.Message, "Error Delete"); }
        }
    }
}
