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
using CAFEHOLIC.service;
using CAFEHOLIC.Model;
using CAFEHOLIC.view.Dialog;
using CAFEHOLIC.Utils;
namespace CAFEHOLIC.view.Page.Admin
{
    public partial class ManageStaffPage : System.Windows.Controls.Page
    {
        private readonly UserService _userService;
        private List<User> listStaff;
        public ManageStaffPage()
        {
            InitializeComponent();
            _userService = new UserService();
            Logger.Info(nameof(ManageStaffPage), "Trang quản lý nhân viên được khởi tạo");
            LoadListStaff();
        }

        private void LoadListStaff()
        {
            try
            {
                listStaff = _userService.GetStaffList();
                dataGridStaff.ItemsSource = listStaff;
                Logger.Info(nameof(ManageStaffPage), $"Tải danh sách {listStaff.Count} nhân viên thành công");
            }
            catch (Exception ex) {
                Logger.Error(nameof(ManageStaffPage), "Lỗi khi tải danh sách nhân viên", ex);
                MessageBox.Show(ex.Message, "Error to load list Staff"); 
            }
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterStaffList(txtSearch.Text.Trim());
        }

        private void FilterStaffList(string keyword)
        {
            if (listStaff == null) return;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                dataGridStaff.ItemsSource = listStaff;
            }
            else
            {
                var filtered = listStaff
                    .Where(s => (s.FullName != null && s.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                                (s.PhoneNumber != null && s.PhoneNumber.Contains(keyword)))
                    .ToList();
                dataGridStaff.ItemsSource = filtered;
                txtNoResult.Visibility = filtered.Any() ? Visibility.Collapsed : Visibility.Visible;
                Logger.Info(nameof(ManageStaffPage), $"Lọc danh sách nhân viên với từ khóa: '{keyword}', kết quả: {filtered.Count}");
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
                    Logger.Info(nameof(ManageStaffPage), $"Thêm nhân viên mới: {newUser.FullName} - {newUser.Email}");
                    LoadListStaff();
                }
            }
            catch(Exception ex) {
                Logger.Error(nameof(ManageStaffPage), "Lỗi khi thêm nhân viên mới", ex);
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
                    Logger.Info(nameof(ManageStaffPage), $"Cập nhật nhân viên ID {updatedUser.Id} - {updatedUser.FullName}");
                    LoadListStaff();
                }
            }
            catch (Exception ex) {
                Logger.Error(nameof(ManageStaffPage), "Lỗi khi cập nhật nhân viên", ex);
                MessageBox.Show(ex.Message, "Error Add"); }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var staff = (sender as Button)?.DataContext as User;
                if (staff == null) return;

                var result = MessageBox.Show($"Bạn có chắc muốn xóa nhân viên {staff.FullName}?",
                                             "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _userService.DeleteUser(staff.Id);
                    Logger.Warn(nameof(ManageStaffPage), $"Đã xóa nhân viên ID {staff.Id} - {staff.FullName}");
                    LoadListStaff();
                }
            }
            catch (Exception ex) {
                Logger.Error(nameof(ManageStaffPage), "Lỗi khi xóa nhân viên", ex);
                MessageBox.Show(ex.Message, "Error Delete"); }
        }
    }
}
