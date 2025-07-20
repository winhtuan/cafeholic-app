using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CAFEHOLIC.Models;
using CAFEHOLIC.Utils;
using CAFEHOLIC.view;

namespace CAFEHOLIC.view
{
    public partial class BookingManage : System.Windows.Controls.Page
    {
        private List<Reservation> reservations = new List<Reservation>();
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=Cafeholic;User Id=sa;Password=123;TrustServerCertificate=True;";

        public BookingManage()
        {
            try
            {
                InitializeComponent();
                LoadReservations();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void LoadReservations()
        {
            reservations.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Reservation";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reservations.Add(new Reservation
                    {
                        ReservationId = (int)reader["ReservationId"],
                        UserId = (int)reader["UserId"],
                        RoomId = (int)reader["RoomId"],
                        StartTime = (DateTime)reader["StartTime"],
                        EndTime = (DateTime)reader["EndTime"],
                        Status = reader["Status"].ToString()
                    });
                }
            }

            dgBookings.ItemsSource = null;
            dgBookings.ItemsSource = reservations;
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = dpFilterDate.SelectedDate;
            string selectedStatus = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filtered = reservations;

            if (selectedDate.HasValue)
            {
                filtered = filtered.FindAll(r => r.StartTime.HasValue && r.StartTime.Value.Date == selectedDate.Value.Date);
            }

            if (selectedStatus != "Tất cả")
            {
                filtered = filtered.FindAll(r => r.Status == selectedStatus);
            }

            dgBookings.ItemsSource = null;
            dgBookings.ItemsSource = filtered;

            Logger.Info("BookingManage", "Đã lọc danh sách đặt phòng");
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            dpFilterDate.SelectedDate = null;
            cbStatusFilter.SelectedIndex = 0;
            LoadReservations();
            Logger.Info("BookingManage", "Làm mới danh sách đặt phòng");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Truyền null hoặc tạo Reservation trống để báo hiệu "Thêm mới"
            var addWindow = new EditBookingWindow(null);

            if (addWindow.ShowDialog() == true)
            {
                Reservation newReservation = addWindow.UpdatedReservation;

                AddReservationToDatabase(newReservation);

                LoadReservations();
                Logger.Info("BookingManage", $"Đã thêm đặt phòng mới (UserID: {newReservation.UserId}, RoomID: {newReservation.RoomId})");
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (dgBookings.SelectedItem is Reservation selected)
            {
                var editWindow = new EditBookingWindow(selected);
                if (editWindow.ShowDialog() == true)
                {
                    Reservation updated = editWindow.UpdatedReservation;
                    UpdateReservationInDatabase(updated);
                    LoadReservations();
                    Logger.Info("BookingManage", $"Cập nhật đặt phòng ID {updated.ReservationId}");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để sửa", "Thông báo");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (dgBookings.SelectedItem is Reservation selected)
            {
                var result = MessageBox.Show($"Xác nhận xoá đặt phòng ID {selected.ReservationId}?", "Xoá", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Reservation WHERE ReservationId = @id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selected.ReservationId);
                        cmd.ExecuteNonQuery();
                    }

                    LoadReservations();
                    Logger.Info("BookingManage", $"Đã xoá đặt phòng ID {selected.ReservationId}");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một dòng để xoá", "Thông báo");
            }
        }
        private void UpdateReservationInDatabase(Reservation reservation)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            UPDATE Reservation
            SET 
                UserId = @UserId,
                RoomId = @RoomId,
                StartTime = @StartTime,
                EndTime = @EndTime,
                Status = @Status
            WHERE ReservationId = @ReservationId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", reservation.UserId);
                cmd.Parameters.AddWithValue("@RoomId", reservation.RoomId);
                cmd.Parameters.AddWithValue("@StartTime", reservation.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", reservation.EndTime);
                cmd.Parameters.AddWithValue("@Status", reservation.Status);
                cmd.Parameters.AddWithValue("@ReservationId", reservation.ReservationId);

                cmd.ExecuteNonQuery();
            }
        }
        private void AddReservationToDatabase(Reservation reservation)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            INSERT INTO Reservation (UserId, RoomId, StartTime, EndTime, Status)
            VALUES (@UserId, @RoomId, @StartTime, @EndTime, @Status)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", reservation.UserId);
                cmd.Parameters.AddWithValue("@RoomId", reservation.RoomId);
                cmd.Parameters.AddWithValue("@StartTime", reservation.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", reservation.EndTime);
                cmd.Parameters.AddWithValue("@Status", reservation.Status);

                cmd.ExecuteNonQuery();
            }
        }

    }
}