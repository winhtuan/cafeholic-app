using CAFEHOLIC.ViewModel;
using System.Windows;

namespace CAFEHOLIC.view.Page
{
    public partial class RoomTypeEditPage : Window
    {
        public RoomTypeViewModel ViewModel => (RoomTypeViewModel)DataContext;

        public RoomTypeEditPage(RoomTypeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}