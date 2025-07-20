using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CAFEHOLIC.view.Page
{
    public partial class HomePage : System.Windows.Controls.Page
    {
        public HomePage()
        {
            InitializeComponent();
            //this.DataContext = new ViewModel.RecommendDrinkViewModel();
        }

        private void GoToMenuPage_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new MenuPage());
        }

//         private void GoToDrinkManagementPage_Click(object sender, RoutedEventArgs e)
//         {
//             this.NavigationService?.Navigate(new DrinkManagementPage());
//         }

//         private void GoToRoomManagementPage_Click(object sender, RoutedEventArgs e)
//         {
//             this.NavigationService?.Navigate(new RoomManagementPage());
//         }
    }
}