using System.Text;
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
using CAFEHOLIC.service;
using CAFEHOLIC.view.Page;
using Microsoft.Extensions.Logging;

namespace CAFEHOLIC.view
{
    public static class AppSession
    {
        public static int CurrentUserId { get; set; } = 1;
        public static readonly ILogger<ProductService> logger=new DBContext().GetLogger<ProductService>();
    }

    public partial class LoginWindown : Window
    {
        private AccountDAO accDAO;

        public LoginWindown()
        {
            InitializeComponent();
            accDAO = new AccountDAO(new DBContext().GetLogger<AccountDAO>());
            MainFrame.Navigate(new LoginPage(this, accDAO));
        }
    }

}