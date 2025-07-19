using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CAFEHOLIC.Model;
using CAFEHOLIC.view.Dialog;
using CAFEHOLIC.ViewModel;

namespace CAFEHOLIC.view.Page
{
    public partial class RoomPage : System.Windows.Controls.Page
    {
        RoomDAO roomDAO = new RoomDAO(new DBContext().GetLogger<RoomDAO>());
        ReservationDAO reservationDAO = new ReservationDAO(new DBContext().GetLogger<ReservationDAO>());

        public ObservableCollection<RoomViewModel> Rooms { get; set; } = new();

        public RoomPage()
        {
            InitializeComponent();
            var vm = new RoomVM();
            vm.ShowReserveDialog = room =>
            {
                var dialog = new ReservationPopUp(room); // hoặc truyền userId từ session
                if (dialog.ShowDialog() == true && dialog.CreatedReservation is Reservation reservation)
                {
                    if (reservationDAO.InsertReservation(reservation))
                    {
                        // Load lại danh sách phòng
                        (this.DataContext as RoomVM)?.reload();

                        MessageBox.Show("✅ Reservation successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("❌ Failed to create reservation.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            };

            this.DataContext = vm;
        }

    }
}
