using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using CAFEHOLIC.Utils;

namespace CAFEHOLIC.ViewModel
{
    public class DrinkViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _price;
        private bool _isAvailable;
        private string _img = string.Empty;
        private int _drinkId;
        private readonly string _className = nameof(DrinkViewModel);

        public int DrinkId
        {
            get => _drinkId;
            set
            {
                _drinkId = value;
                OnPropertyChanged();
                Logger.Info(_className, $"DrinkId set to: {value}");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                Logger.Info(_className, $"Name set to: '{value}'");
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

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                Logger.Info(_className, $"Price set to: {value}");
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

        public string Img
        {
            get => _img;
            set
            {
                _img = value;
                OnPropertyChanged();
                Logger.Info(_className, $"Img set to: '{value}'");
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand UploadImageCommand { get; }

        public DrinkViewModel()
        {
            Logger.Info(_className, "Constructor started");
            IsAvailable = true; // Giá trị mặc định
            SaveCommand = new RelayCommand<object>(Save, CanSave);
            CancelCommand = new RelayCommand<object>(Cancel);
            UploadImageCommand = new RelayCommand<object>(UploadImage);
            Logger.Info(_className, "Constructor completed successfully");
        }

        private bool CanSave(object parameter)
        {
            bool canSave = !string.IsNullOrEmpty(Name) && Price > 0 && !string.IsNullOrEmpty(Img);
            Logger.Info(_className, $"CanSave: {canSave}, Name: '{Name}', Price: {Price}, Img: '{Img}'");
            return canSave;
        }

        private void Save(object parameter)
        {
            Logger.Info(_className, "Starting Save command");
            try
            {
                if (string.IsNullOrEmpty(Name) || Price <= 0 || string.IsNullOrEmpty(Img))
                {
                    Logger.Warn(_className, "Validation failed: Name is empty, Price <= 0, or Img is empty");
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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
                Logger.Error(_className, "Error saving drink", ex);
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                MessageBox.Show($"Lỗi khi hủy: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UploadImage(object parameter)
        {
            Logger.Info(_className, "Starting UploadImage command");
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    Logger.Info(_className, $"Selected file: {filePath}");
                    string destinationPath = Path.Combine("Images", Path.GetFileName(filePath));
                    Directory.CreateDirectory("Images"); // Tạo thư mục Images nếu chưa tồn tại
                    File.Copy(filePath, destinationPath, true);
                    Img = destinationPath; // Lưu đường dẫn tương đối
                    Logger.Info(_className, $"Image copied to: {destinationPath}");
                }
                else
                {
                    Logger.Info(_className, "File dialog cancelled");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error uploading image", ex);
                MessageBox.Show($"Lỗi khi upload ảnh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Triển khai IDataErrorInfo để validation thời gian thực
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;
                switch (columnName)
                {
                    case nameof(Name):
                        if (string.IsNullOrEmpty(Name))
                            error = "Tên đồ uống là bắt buộc.";
                        else if (Name.Length > 100)
                            error = "Tên đồ uống không được vượt quá 100 ký tự.";
                        break;
                    case nameof(Description):
                        if (!string.IsNullOrEmpty(Description) && Description.Length > 500)
                            error = "Mô tả không được vượt quá 500 ký tự.";
                        break;
                    case nameof(Price):
                        if (Price <= 0)
                            error = "Giá phải lớn hơn 0.";
                        break;
                    case nameof(Img):
                        if (string.IsNullOrEmpty(Img))
                            error = "Đường dẫn ảnh là bắt buộc.";
                        break;
                }
                if (error != null)
                {
                    Logger.Warn(_className, $"Validation error on {columnName}: {error}");
                }
                return error;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName != nameof(Error))
            {
                (SaveCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
            }
        }
    }
}