using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CAFEHOLIC.ViewModel
{
    public class RoomEditViewModel : INotifyPropertyChanged
    {
        private readonly RoomTypeDAO _roomTypeDAO;
        private int _roomId;
        private string _name = string.Empty;
        private bool _isAvailable;
        private int _roomTypeId;
        private ObservableCollection<RoomType> _roomTypes;
        private bool _isSaveEnabled;
        private readonly string _className = nameof(RoomEditViewModel);

        public int RoomId
        {
            get => _roomId;
            set
            {
                _roomId = value;
                OnPropertyChanged();
                Logger.Info(_className, $"RoomId set to: {value}");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                UpdateSaveButtonState();
                Logger.Info(_className, $"Name set to: '{value}'");
            }
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                OnPropertyChanged();
                Logger.Info(_className, $"IsAvailable set to: {value}");
            }
        }

        public int RoomTypeId
        {
            get => _roomTypeId;
            set
            {
                _roomTypeId = value;
                OnPropertyChanged();
                UpdateSaveButtonState();
                Logger.Info(_className, $"RoomTypeId set to: {value}");
            }
        }

        public ObservableCollection<RoomType> RoomTypes
        {
            get => _roomTypes;
            set
            {
                _roomTypes = value;
                OnPropertyChanged();
                Logger.Info(_className, $"RoomTypes set, count: {value?.Count ?? 0}");
            }
        }

        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            set
            {
                _isSaveEnabled = value;
                OnPropertyChanged();
                Logger.Info(_className, $"IsSaveEnabled set to: {value}");
            }
        }

        public RelayCommand<object> SaveCommand { get; private set; }
        public RelayCommand<object> CancelCommand { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RoomEditViewModel()
        {
            Logger.Info(_className, "Constructor started");
            try
            {
                _roomTypeDAO = new RoomTypeDAO(new dao.DBContext().GetLogger<RoomTypeDAO>());
                _roomTypes = new ObservableCollection<RoomType>(_roomTypeDAO.GetAllRoomTypes());
                SaveCommand = new RelayCommand<object>(Save, CanSave);
                CancelCommand = new RelayCommand<object>(Cancel, _ => true);
                UpdateSaveButtonState();
                Logger.Info(_className, "Constructor completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error in constructor", ex);
                MessageBox.Show($"Lỗi khi khởi tạo: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void Save(object parameter)
        {
            Logger.Info(_className, "Starting Save command");
            try
            {
                if (parameter is Window window)
                {
                    Logger.Info(_className, "Setting DialogResult to true and closing window");
                    window.DialogResult = true;
                    window.Close();
                }
                else
                {
                    Logger.Error(_className, "Parameter is not a Window");
                    MessageBox.Show("Lỗi hệ thống: Không thể lưu dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error saving room", ex);
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave(object parameter)
        {
            bool canSave = !string.IsNullOrWhiteSpace(Name) && RoomTypeId > 0;
            Logger.Info(_className, $"CanSave: {canSave}, Name: '{Name}', RoomTypeId: {RoomTypeId}");
            return canSave;
        }

        private void Cancel(object parameter)
        {
            Logger.Info(_className, "Starting Cancel command");
            try
            {
                if (parameter is Window window)
                {
                    Logger.Info(_className, "Setting DialogResult to false and closing window");
                    window.DialogResult = false;
                    window.Close();
                }
                else
                {
                    Logger.Error(_className, "Parameter is not a Window");
                    MessageBox.Show("Lỗi hệ thống: Không thể hủy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error cancelling", ex);
                MessageBox.Show($"Lỗi khi hủy: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateSaveButtonState()
        {
            IsSaveEnabled = !string.IsNullOrWhiteSpace(Name) && RoomTypeId > 0;
            SaveCommand.RaiseCanExecuteChanged();
            Logger.Info(_className, $"UpdateSaveButtonState: IsSaveEnabled={IsSaveEnabled}");
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}