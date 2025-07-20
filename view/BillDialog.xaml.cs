using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;

namespace CAFEHOLIC.view
{
    public partial class BillDialog : Window
    {
        private readonly OrderDAO orderDAO;

        public ObservableCollection<CartItem> CartItems { get; set; }
        public DateTime PaymentTime { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        private int? createdOrderId;

        public BillDialog(ObservableCollection<CartItem> cartItems, int orderId, decimal total)
        {
            InitializeComponent();
            this.DataContext = this; 

            CartItems = cartItems;
            PaymentTime = DateTime.Now;
            TotalPrice = total;
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
    }
}
