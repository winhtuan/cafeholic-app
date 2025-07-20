using CAFEHOLIC.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CAFEHOLIC.ViewModel
{
    public class RoomTypeViewModel : INotifyPropertyChanged
    {
        private int _typeId;
        private string _name = string.Empty;
        private int _minCapacity;
        private int _maxCapacity;
        private string _description = string.Empty;
        private bool _isSaveEnabled;
        private readonly string _className = nameof(RoomTypeViewModel);

        public int TypeId
        {
            get => _typeId;
            set
            {
                _typeId = value;
                OnPropertyChanged();
                Logger.Info(_className, $"TypeId set to: {value}");
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

        public int MinCapacity
        {
            get => _minCapacity;
            set
            {
                _minCapacity = value;
                OnPropertyChanged();
                UpdateSaveButtonState();
                Logger.Info(_className, $"MinCapacity set to: {value}");
            }
        }

        public int MaxCapacity
        {
            get => _maxCapacity;
            set
            {
                _maxCapacity = value;
                OnPropertyChanged();
                UpdateSaveButtonState();
                Logger.Info(_className, $"MaxCapacity set to: {value}");
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
                Logger.Info(_className, $"Description set to: '{value}'");
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

        public RoomTypeViewModel()
        {
            Logger.Info(_className, "Constructor started");
            try
            {
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
                Logger.Error(_className, "Error saving room type", ex);
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave(object parameter)
        {
            bool canSave = !string.IsNullOrWhiteSpace(Name) && MinCapacity >= 0 && MaxCapacity >= MinCapacity;
            Logger.Info(_className, $"CanSave: {canSave}, Name: '{Name}', MinCapacity: {MinCapacity}, MaxCapacity: {MaxCapacity}");
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
            IsSaveEnabled = !string.IsNullOrWhiteSpace(Name) && MinCapacity >= 0 && MaxCapacity >= MinCapacity;
            SaveCommand.RaiseCanExecuteChanged();
            Logger.Info(_className, $"UpdateSaveButtonState: IsSaveEnabled={IsSaveEnabled}");
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}