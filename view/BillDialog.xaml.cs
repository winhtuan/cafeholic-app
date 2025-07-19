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
using System.Windows.Shapes;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.view.Page;

namespace CAFEHOLIC.view
{
    public partial class BillDialog : Window
    {
        private readonly OrderDAO orderDAO;
        private ObservableCollection<CartItem> CartItems { get; set; } = new ObservableCollection<CartItem>();
        private DateTime PaymentTime { get; set; }
        private User CurrentUser { get; }
        private int? createdOrderId;
        public BillDialog(ObservableCollection<CartItem> cartItems, int orderId)
        {
            InitializeComponent();
            CartItems = cartItems;
            PaymentTime = DateTime.Now;
            this.createdOrderId = orderId;
            this.orderDAO = new OrderDAO(new DBContext());
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
            "Bạn có chắc chắn muốn huỷ thanh toán không?",
            "Xác nhận huỷ",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (createdOrderId.HasValue)
                {
                    orderDAO.updateStatus("Đã hủy", createdOrderId.Value);
                }
                this.DialogResult = false;
            }
        }

        private void PayByCash_Click(object sender, RoutedEventArgs e)
        {
            CartItems.Clear();
            if (createdOrderId.HasValue)
            {
                orderDAO.updateStatus("Thành công", createdOrderId.Value);
            }
            this.DialogResult = true;
        }

        private void PayByQR_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Gọi logic thanh toán QR tại đây
        }
    }
}