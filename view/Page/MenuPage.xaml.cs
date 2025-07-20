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

namespace CAFEHOLIC.view.Page
{
<<<<<<< HEAD
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : System.Windows.Controls.Page
    {
        public MenuPage()
        {
            InitializeComponent();
=======
    public partial class MenuPage : System.Windows.Controls.Page
    {
        private readonly ViewModel.ViewMenuVM viewModel;

        public MenuPage()
        {
            InitializeComponent();
            viewModel = new ViewModel.ViewMenuVM();
            this.DataContext = viewModel;
>>>>>>> origin/develop
        }
    }
}
