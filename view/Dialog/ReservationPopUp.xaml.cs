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
using CAFEHOLIC.Model;
using CAFEHOLIC.ViewModel;

namespace CAFEHOLIC.view.Dialog
{
    public partial class ReservationPopUp : Window
    {
        public string RoomName { get; set; }
        public string RoomType { get; set; }
        public string Capacity { get; set; }
        public string PhoneNumber { get; set; } = "";

        private readonly AccountDAO accountDAO = new(new DBContext().GetLogger<AccountDAO>());

        public Reservation? CreatedReservation { get; private set; }

        private readonly int roomId;

        public ReservationPopUp(RoomViewModel room)
        {
            InitializeComponent();
            RoomName = $"Reserve Room: {room.Name}";
            RoomType = room.RoomType;
            Capacity = room.Capacity;
            roomId = room.RoomId;
            this.DataContext = this;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            var account = accountDAO.GetAccountByPhone(PhoneNumber);
            if (account == null)
            {
                MessageBox.Show("❌ Phone number not found!", "Invalid User", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CreatedReservation = new Reservation
            {
                RoomId = roomId,
                UserId = account.AccId,
                StartTime = DateTime.Now,
                EndTime = null,
                Status = "Pending"
            };
            new RoomDAO( new DBContext().GetLogger<RoomDAO>()).UpdateRoomStatus(roomId, false);
            this.DialogResult = true;
        }
    }

}
