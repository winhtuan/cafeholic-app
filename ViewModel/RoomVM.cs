using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using CAFEHOLIC.dao;
using CAFEHOLIC.DAO;
namespace CAFEHOLIC.ViewModel
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string RoomType { get; set; }
        public string Capacity { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class RoomVM : INotifyPropertyChanged
    {
        private readonly RoomDAO roomDAO;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<RoomViewModel> Rooms { get; set; }

        public ICommand ReserveCommand { get; }

        private string keyword;

        public string Keyword
        {
            get => keyword;
            set
            {
                if (keyword != value)
                {
                    keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                    ApplyFilter();
                }
            }
        }

        public RoomVM()
        {
            roomDAO = new RoomDAO(new DBContext().GetLogger<RoomDAO>());
            Rooms = new ObservableCollection<RoomViewModel>();
            LoadRooms();
            ApplyFilter();
            ReserveCommand = new RelayCommand<RoomViewModel>(OnReserve, CanReserve);
        }


        public ObservableCollection<RoomViewModel> FilteredRooms { get; set; } = new();

        public void reload()
        {
            LoadRooms();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = Rooms
                .Where(r => string.IsNullOrEmpty(Keyword) ||
                            r.Name.Contains(Keyword, StringComparison.OrdinalIgnoreCase) ||
                            r.RoomType.Contains(Keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            FilteredRooms.Clear();
            foreach (var room in filtered)
                FilteredRooms.Add(room);
        }

        private void FilterAvailable()
        {
            var filtered = Rooms.Where(r => r.IsAvailable).ToList();
            FilteredRooms.Clear();
            foreach (var room in filtered)
                FilteredRooms.Add(room);
        }

        public ICommand FilterAvailableCommand => new RelayCommand<object>(_ => FilterAvailable());
        public ICommand ShowAllCommand => new RelayCommand<object>(_ => { Keyword = null; ApplyFilter(); });
        public ICommand RefreshCommand => new RelayCommand<object>(_ =>
        {
            Keyword = null;
            LoadRooms();
            ApplyFilter();
        });

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand CompleteCommand => new RelayCommand<RoomViewModel>(OnComplete, CanComplete);

        private bool CanComplete(RoomViewModel room) => room != null && !room.IsAvailable;

        private void OnComplete(RoomViewModel room)
        {
            var reservationDAO = new ReservationDAO(new DBContext().GetLogger<ReservationDAO>());
            var roomDAO = new RoomDAO(new DBContext().GetLogger<RoomDAO>());

            // 1. Cập nhật EndTime cho reservation hiện tại
            if (reservationDAO.EndCurrentReservation(room.RoomId, DateTime.Now))
            {
                // 2. Mở lại phòng
                roomDAO.UpdateRoomStatus(room.RoomId, true);
                reload();
                System.Windows.MessageBox.Show("✅ Room marked as completed!", "Success");
            }
            else
            {
                System.Windows.MessageBox.Show("❌ Failed to update reservation.", "Error");
            }
        }


        private void LoadRooms()
        {
            var rooms = roomDAO.GetAllRooms();

            Rooms.Clear();
            foreach (var r in rooms)
            {
                Rooms.Add(new RoomViewModel
                {
                    RoomId = r.RoomId,
                    Name = r.Name ?? "Unnamed",
                    RoomType = r.RoomType?.Name ?? "Unknown",
                    Capacity = $"{r.RoomType?.MinCapacity}–{r.RoomType?.MaxCapacity}",
                    IsAvailable = r.IsAvailable ?? false
                });
            }
        }

        public Action<RoomViewModel>? ShowReserveDialog { get; set; }

        private void OnReserve(RoomViewModel room)
        {
            ShowReserveDialog?.Invoke(room);
        }

        private bool CanReserve(RoomViewModel room)
        {
            return room != null && room.IsAvailable;
        }

        public class BoolToStatusConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool available = (bool)value;
                return available ? "Available" : "Occupied";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();
        }
        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Predicate<T>? _canExecute;

            public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object? parameter) => _canExecute == null || _canExecute((T)parameter!);

            public void Execute(object? parameter) => _execute((T)parameter!);
        }
    }
    
}
