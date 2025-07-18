using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.Utils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CAFEHOLIC.Service;

namespace CAFEHOLIC.ViewModel
{
    public class ViewMenuVM : ObservableObject
    {
        private readonly DrinkDAO drinkDAO;
        private List<Drink> allDrinks = new();
        private string searchKeyword = string.Empty;

        public ObservableCollection<Drink> Drinks { get; set; } = new();
        public ObservableCollection<CartItem> CartItems { get; set; } = new();
        public decimal TotalPriceAll => CartItems?.Sum(item => (item.Drink.Price ?? 0) * item.Quantity) ?? 0;

        // Button
        public ICommand AddToCartCommand { get; }

        public ICommand RemoveFromCartCommand { get; }
        public ICommand VoiceOrderCommand { get; }

        public ViewMenuVM()
        {
            drinkDAO = new DrinkDAO(new DBContext(), new LoggerFactory().CreateLogger<DrinkDAO>());
            AddToCartCommand = new RelayCommand<Drink>(AddToCart);
            RemoveFromCartCommand = new RelayCommand<CartItem>(RemoveFromCart);
            LoadDrinks();

            CartItems.CollectionChanged += CartItems_CollectionChanged;
            foreach (var item in CartItems)
            {
                item.PropertyChanged += CartItem_PropertyChanged;
            }
            VoiceOrderCommand = new AsyncRelayCommand(async () => await VoiceOrder());
        }

        private async Task VoiceOrder()
        {
            var text = await SpeechToText.RecognizeOnceAsync();

            if (!string.IsNullOrWhiteSpace(text))
            {
                Logger.Info($"Người dùng nói: {text}");

                var nlp = new NLPOrder();

                try
                {
                    var orders = await nlp.AnalyzeOrderAsync(text);

                    if (orders.Count == 0)
                    {
                        Logger.Warn("Không nhận diện được món nào từ câu nói.");
                        return;
                    }

                    foreach (var (name, qty) in orders)
                    {
                        Logger.Info($"Đặt: {qty} x {name}");
                    }

                    // Lưu JSON và upload lên S3
                    string s3Url = await NLPOrder.SaveOrderToJsonAndUploadAsync(orders);
                    Logger.Info($"Đơn hàng đã lưu và upload tại: {s3Url}");
                }
                catch (Exception ex)
                {
                    Logger.Error("Lỗi khi phân tích đơn hàng", ex);
                }
            }
            else
            {
                Logger.Warn("Không nhận được nội dung từ giọng nói.");
            }
        }

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

        private void LoadDrinks()
        {
            allDrinks = drinkDAO.GetAllDrinks();
            drinkDAO.ApplySearch(searchKeyword, allDrinks, Drinks);
        }

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

        private void RemoveFromCart(CartItem? item)
        {
            if (item == null) return;

            item.PropertyChanged -= CartItem_PropertyChanged;
            CartItems.Remove(item);
            OnPropertyChanged(nameof(TotalPriceAll));
        }
    }
}