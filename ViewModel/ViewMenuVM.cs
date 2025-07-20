using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.Utils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CAFEHOLIC.Service;
using CAFEHOLIC.view;
using CAFEHOLIC.view.Page;

namespace CAFEHOLIC.ViewModel
{
    public class ViewMenuVM : ObservableObject
    {
        private readonly DrinkDAO drinkDAO;
        private readonly OrderDAO orderDAO;
        private List<Drink> allDrinks = new();
        private string searchKeyword = string.Empty;

        public ObservableCollection<Drink> Drinks { get; set; } = new();
        public ObservableCollection<CartItem> CartItems { get; set; } = new();
        public decimal TotalPriceAll => CartItems?.Sum(item => (item.Drink.Price ?? 0) * item.Quantity) ?? 0;
        public int CurrentUserID { get; }

        // Button
        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand VoiceOrderCommand { get; }
        public ICommand CheckoutCommand { get; }
        // Constructor
        public ViewMenuVM()
        {
            this.CurrentUserID = AppSession.CurrentUserId;
            drinkDAO = new DrinkDAO(new DBContext(), new LoggerFactory().CreateLogger<DrinkDAO>());
            orderDAO = new OrderDAO(new DBContext());
            AddToCartCommand = new RelayCommand<Drink>(AddToCart);
            RemoveFromCartCommand = new RelayCommand<CartItem>(RemoveFromCart);
            VoiceOrderCommand = new AsyncRelayCommand(async () => await VoiceOrder());
            CheckoutCommand = new AsyncRelayCommand(async () => await Checkout());
            LoadDrinks();

            CartItems.CollectionChanged += CartItems_CollectionChanged;
            foreach (var item in CartItems)
            {
                item.PropertyChanged += CartItem_PropertyChanged;
            }
        }

        public Action? NavigateToRoomPageAction { get; set; }
        private int? createdOrderId = null;
        private async Task Checkout()
        {
            // 1. Tạo đơn hàng
            Order createdOrder = await orderDAO.CreateOrderFromCart(CartItems.ToList(), CurrentUserID);
            var dialog = new BillDialog(this.CartItems, createdOrder.OrderId, this.TotalPriceAll);
            dialog.ShowDialog();
        }

        // Voice order functionality
        private async Task VoiceOrder()
        {
            var text = await SpeechToText.RecognizeOnceAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                Logger.Info(nameof(ViewMenuVM), $"Người dùng nói: {text}");
                var nlp = new NLPOrder();
                try
                {
                    var orders = await nlp.AnalyzeOrderAsync(text);
                    if (orders.Count == 0)
                    {
                        Logger.Warn(nameof(ViewMenuVM), "Không nhận diện được món nào từ câu nói.");
                        return;
                    }
                    Logger.Info(nameof(ViewMenuVM), $"Tìm thấy {orders.Count} món:");
                    foreach (var (name, qty) in orders)
                    {
                        Logger.Info(nameof(ViewMenuVM), $"  - {name} x{qty}");
                    }
                    // Add items to cart
                    foreach (var (name, qty) in orders)
                    {
                        var drink = allDrinks.FirstOrDefault(d =>
                            string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
                        if (drink != null)
                        {
                            var existing = CartItems.FirstOrDefault(c => c.Drink.DrinkId == drink.DrinkId);
                            if (existing != null)
                            {
                                existing.Quantity += qty;
                                Logger.Info(nameof(ViewMenuVM), $"Cập nhật số lượng {drink.Name}: {existing.Quantity}");
                            }
                            else
                            {
                                var item = new CartItem { Drink = drink, Quantity = qty };
                                item.PropertyChanged += CartItem_PropertyChanged;
                                CartItems.Add(item);
                                Logger.Info(nameof(ViewMenuVM), $"Thêm mới vào giỏ hàng: {drink.Name} x{qty}");
                            }
                        }
                        else
                        {
                            Logger.Warn(nameof(ViewMenuVM), $"Không tìm thấy đồ uống: {name}");
                        }
                    }
                    // Update UI
                    OnPropertyChanged(nameof(CartItems));
                    OnPropertyChanged(nameof(TotalPriceAll));
                }
                catch (Exception ex)
                {
                    Logger.Error(nameof(ViewMenuVM), "Lỗi khi phân tích đơn hàng", ex);
                }
            }
            else
            {
                Logger.Warn(nameof(ViewMenuVM), "Không nhận được nội dung từ giọng nói.");
            }
        }

        // Search functionality
        public string SearchKeyword
        {
            get => searchKeyword;
            set
            {
                if (SetProperty(ref searchKeyword, value))
                {
                    drinkDAO.ApplySearch(searchKeyword, allDrinks, Drinks);
                }
            }
        }
        // Load all drinks from the database
        private void LoadDrinks()
        {
            allDrinks = drinkDAO.GetAllDrinks();
            drinkDAO.ApplySearch(searchKeyword, allDrinks, Drinks);
        }
        // Event handlers for CartItems and CartItem changes
        private void CartItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.Quantity) || e.PropertyName == nameof(CartItem.Drink))
            {
                OnPropertyChanged(nameof(TotalPriceAll));
            }
        }

        private void CartItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CartItem item in e.NewItems)
                    item.PropertyChanged += CartItem_PropertyChanged;
            }
            if (e.OldItems != null)
            {
                foreach (CartItem item in e.OldItems)
                    item.PropertyChanged -= CartItem_PropertyChanged;
            }
            OnPropertyChanged(nameof(TotalPriceAll));
        }

        private void AddToCart(Drink? drink)
        {
            if (drink == null) return;

            var existing = CartItems.FirstOrDefault(c => c.Drink.DrinkId == drink.DrinkId);
            if (existing != null)
            {
                existing.Quantity += 1;
                OnPropertyChanged(nameof(CartItems));
                OnPropertyChanged(nameof(TotalPriceAll));
            }
            else
            {
                var item = new CartItem { Drink = drink };
                item.PropertyChanged += CartItem_PropertyChanged;
                CartItems.Add(item);
            }
        }
        // Remove an item from the cart
        private void RemoveFromCart(CartItem? item)
        {
            if (item == null) return;

            item.PropertyChanged -= CartItem_PropertyChanged;
            CartItems.Remove(item);
            OnPropertyChanged(nameof(TotalPriceAll));
        }
    }
}