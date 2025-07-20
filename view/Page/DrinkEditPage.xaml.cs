using CAFEHOLIC.ViewModel;
using System;
using System.Windows;

namespace CAFEHOLIC.view.Page
{
    public partial class DrinkEditPage : Window
    {
        public DrinkEditViewModel ViewModel { get; set; }

        public DrinkEditPage(DrinkViewModel drink)
        {
            System.Diagnostics.Debug.WriteLine("[DrinkEditPage] Constructor started");
            try
            {
                if (drink == null) throw new ArgumentNullException(nameof(drink));
                if (App.Configuration == null) throw new InvalidOperationException("App.Configuration is not initialized");
                InitializeComponent();
                ViewModel = new DrinkEditViewModel(drink, App.Configuration);
                DataContext = ViewModel;
                System.Diagnostics.Debug.WriteLine("[DrinkEditPage] Constructor completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DrinkEditPage] Error in constructor: {ex.Message}, InnerException: {ex.InnerException?.Message}, StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Lỗi khởi tạo DrinkEditPage: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }
}