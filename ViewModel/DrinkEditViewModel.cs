using CAFEHOLIC.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CAFEHOLIC.ViewModel
{
    public class DrinkEditViewModel : INotifyPropertyChanged
    {
        private DrinkViewModel _drink;
        private readonly S3Client _s3Client;
        private readonly string _className = nameof(DrinkEditViewModel);

        public DrinkViewModel Drink
        {
            get => _drink;
            set
            {
                _drink = value ?? new DrinkViewModel();
                OnPropertyChanged();
                Logger.Info(_className, $"Drink property set: Name={_drink.Name}, Price={_drink.Price}, Img={_drink.Img}");
            }
        }

        public ICommand SelectImageCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public DrinkEditViewModel(IConfiguration configuration)
        {
            Logger.Info(_className, "Constructor (IConfiguration) started");
            try
            {
                if (configuration == null)
                    throw new ArgumentNullException(nameof(configuration), "S3 configuration is missing.");
                var s3Config = configuration.GetSection("S3") ?? throw new ArgumentNullException(nameof(configuration), "S3 configuration section is missing.");
                string accessKey = s3Config["accessKeyId"] ?? throw new ArgumentNullException("accessKeyId", "S3 accessKeyId is missing in appsettings.json.");
                string secretKey = s3Config["secretAccessKey"] ?? throw new ArgumentNullException("secretAccessKey", "S3 secretAccessKey is missing in appsettings.json.");
                string bucketName = s3Config["bucketName"] ?? throw new ArgumentNullException("bucketName", "S3 bucketName is missing in appsettings.json.");
                string folderPrefix = s3Config["folder"] ?? "order-json";
                _s3Client = new S3Client(accessKey, secretKey, bucketName, folderPrefix);
                _drink = new DrinkViewModel();
                SelectImageCommand = new AsyncRelayCommand(SelectImageAsync);
                SaveCommand = new RelayCommand<object>(Save, CanSave);
                CancelCommand = new RelayCommand<object>(Cancel);
                Logger.Info(_className, "Constructor (IConfiguration) completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error in constructor", ex);
                throw;
            }
        }

        public DrinkEditViewModel(DrinkViewModel drink, IConfiguration configuration) : this(configuration)
        {
            Logger.Info(_className, "Constructor (DrinkViewModel, IConfiguration) started");
            try
            {
                Drink = drink ?? new DrinkViewModel();
                Logger.Info(_className, "Constructor (DrinkViewModel, IConfiguration) completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error in constructor", ex);
                throw;
            }
        }

        private async Task SelectImageAsync()
        {
            try
            {
                Logger.Info(_className, "Opening file dialog");
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    Logger.Info(_className, $"Selected file: {openFileDialog.FileName}");
                    string url = await _s3Client.UploadFileAsync(openFileDialog.FileName);
                    Drink.Img = url;
                    Logger.Info(_className, $"Uploaded to S3, URL: {url}");
                    OnPropertyChanged(nameof(Drink));
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

        private void Save(object parameter)
        {
            Logger.Info(_className, "Starting Save command");
            try
            {
                if (string.IsNullOrEmpty(Drink.Name) || Drink.Price <= 0)
                {
                    Logger.Warn(_className, "Validation failed: Name is empty or Price <= 0");
                    MessageBox.Show("Vui lòng nhập tên đồ uống và giá hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (parameter is not Window window)
                {
                    Logger.Error(_className, "Parameter is not a Window");
                    MessageBox.Show("Lỗi hệ thống: Không thể lưu dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Logger.Info(_className, "Setting DialogResult to true and closing window");
                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error saving drink", ex);
                MessageBox.Show($"Lỗi khi lưu đồ uống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave(object parameter)
        {
            bool canSave = !string.IsNullOrEmpty(Drink.Name) && Drink.Price > 0;
            Logger.Info(_className, $"CanSave: {canSave}, Name: {Drink.Name}, Price: {Drink.Price}");
            return canSave;
        }

        private void Cancel(object parameter)
        {
            Logger.Info(_className, "Starting Cancel command");
            try
            {
                if (parameter is not Window window)
                {
                    Logger.Error(_className, "Parameter is not a Window");
                    MessageBox.Show("Lỗi hệ thống: Không thể hủy.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Logger.Info(_className, "Setting DialogResult to false and closing window");
                window.DialogResult = false;
                window.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error cancelling", ex);
                MessageBox.Show($"Lỗi khi hủy: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}