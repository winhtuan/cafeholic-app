using System.Windows;
using System.Diagnostics;
using CAFEHOLIC.ViewModel;

namespace CAFEHOLIC.view.Page
{
    public partial class DrinkManagementPage : System.Windows.Controls.Page
    {
        public DrinkManagementPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"[DrinkManagementPage] DataContext: {DataContext?.GetType().Name}");
            if (DataContext is DrinkManagementViewModel vm)
            {
                Debug.WriteLine($"[DrinkManagementPage] AddDrinkCommand is null: {vm.AddDrinkCommand == null}");
            }
        }

        private void AddDrinkButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[DrinkManagementPage] AddDrinkButton_Click triggered");
            if (DataContext is DrinkManagementViewModel vm && vm.AddDrinkCommand != null)
            {
                Debug.WriteLine("[DrinkManagementPage] Executing AddDrinkCommand");
                vm.AddDrinkCommand.Execute(null);
            }
            else
            {
                Debug.WriteLine("[DrinkManagementPage] AddDrinkCommand is null or DataContext is invalid");
            }
        }
    }
}