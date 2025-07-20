using CAFEHOLIC.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CAFEHOLIC.view
{
    public partial class EditBookingWindow : Window
    {
        private Reservation currentReservation;

        public Reservation UpdatedReservation { get; private set; }

        public EditBookingWindow(Reservation reservation)
        {
            InitializeComponent();
            currentReservation = reservation;

            if (reservation != null)
            {
                currentReservation = reservation;

                txtUserId.Text = reservation.UserId.ToString();
                txtRoomId.Text = reservation.RoomId.ToString();
                dpStartTime.SelectedDate = reservation.StartTime;
                dpEndTime.SelectedDate = reservation.EndTime;

                foreach (ComboBoxItem item in cmbStatus.Items)
                {
                    if ((string)item.Content == reservation.Status)
                    {
                        cmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                // Nếu là thêm mới, khởi tạo một reservation trống
                currentReservation = new Reservation();
                cmbStatus.SelectedIndex = 0; // chọn mặc định
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra đầu vào (có thể mở rộng validate thêm)
                if (string.IsNullOrWhiteSpace(txtUserId.Text) ||
                    string.IsNullOrWhiteSpace(txtRoomId.Text) ||
                    dpStartTime.SelectedDate == null ||
                    dpEndTime.SelectedDate == null ||
                    cmbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Thiếu dữ liệu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tạo Reservation mới từ dữ liệu nhập
                UpdatedReservation = new Reservation
                {
                    ReservationId = currentReservation.ReservationId, // sẽ là 0 nếu thêm mới
                    UserId = int.Parse(txtUserId.Text),
                    RoomId = int.Parse(txtRoomId.Text),
                    StartTime = dpStartTime.SelectedDate.Value,
                    EndTime = dpEndTime.SelectedDate.Value,
                    Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString()
                };

                // Đóng cửa sổ và trả kết quả true
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu đặt phòng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
