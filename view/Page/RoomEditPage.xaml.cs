using CAFEHOLIC.ViewModel;
using System.Windows;

namespace CAFEHOLIC.view.Page
{
    public partial class RoomEditPage : Window
    {
        public RoomEditViewModel ViewModel => (RoomEditViewModel)DataContext;

        public RoomEditPage(RoomEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}