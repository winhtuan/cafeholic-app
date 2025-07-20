using System;
using System.Linq;
using System.Windows;
using CAFEHOLIC.Model;
using CAFEHOLIC.service;

namespace CAFEHOLIC.view.Dialog
{
    public partial class UserDialog : Window
    {
        public User User { get; private set; }
        public Account Account { get; private set; }

        private readonly RoleService _roleService;

        public UserDialog(User user = null, string title = "User Editor")
        {
            InitializeComponent();
            txtTitle.Text = title;
            _roleService = new RoleService();

            LoadRoles();

            if (user != null)
            {
                // Edit mode
                User = user;
                Account = user.Accounts.FirstOrDefault() ?? new Account();

                txtFullName.Text = User.FullName;
                txtPhone.Text = User.PhoneNumber ?? "";
                txtEmail.Text = User.Email ?? "";
                txtPassword.Password = Account.PasswordHash ?? "";
                cbRole.SelectedValue = Account.RoleId;
            }
            else
            {
                // Add mode
                User = new User();
                Account = new Account();
            }
        }

        private void LoadRoles()
        {
            try
            {
                var roles = _roleService.GetAllRoles();
                cbRole.ItemsSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách quyền: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Password) ||
                cbRole.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Gán vào User
            User.FullName = txtFullName.Text.Trim();
            User.Email = txtEmail.Text.Trim();
            User.PhoneNumber = txtPhone.Text.Trim();

            // Gán vào Account
            Account.PhoneNumber = txtPhone.Text.Trim();
            Account.PasswordHash = txtPassword.Password;
            Account.RoleId = (int)cbRole.SelectedValue;
            Account.User = User;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
