using Microsoft.Data.SqlClient;
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
using CAFEHOLIC.Models;
namespace CAFEHOLIC.view
{
    public partial class BookingManage : System.Windows.Controls.Page

    {
        private List<Reservation> reservations = new List<Reservation>();
        private string connectionString = "Server=localhost;Database=Cafeholic;Trusted_Connection=True;";

        public BookingManage()
        {
            InitializeComponent();
            LoadReservations();
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
            // TODO: Lọc dữ liệu theo điều kiện
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Làm mới dữ liệu
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Thêm đặt chỗ mới
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Sửa đặt chỗ đã chọn
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Xoá đặt chỗ đã chọn
        }
    }
}