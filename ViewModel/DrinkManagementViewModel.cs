using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.Service;
using CAFEHOLIC.Utils;
using CAFEHOLIC.view.Page;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CAFEHOLIC.ViewModel
{
    public class DrinkManagementViewModel : INotifyPropertyChanged
    {
        private readonly DrinkService _drinkService;
        private ObservableCollection<DrinkViewModel> _pagedDrinks;
        private DrinkViewModel? _selectedDrink;
        private int _currentPage;
        private int _totalPages;
        private string _searchQuery = string.Empty;
        private const int PageSize = 10;
        private Window _activeEditPage;
        private bool _isAddDrinkButtonEnabled = true;
        private readonly string _className = nameof(DrinkManagementViewModel);

        public ObservableCollection<DrinkViewModel> PagedDrinks
        {
            get => _pagedDrinks;
            set { _pagedDrinks = value; OnPropertyChanged(); }
        }

        public DrinkViewModel? SelectedDrink
        {
            get => _selectedDrink;
            set { _selectedDrink = value; OnPropertyChanged(); }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(PageInfo));
                    OnPropertyChanged(nameof(CanGoToPreviousPage));
                    OnPropertyChanged(nameof(CanGoToNextPage));
                    (PreviousPageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                    (NextPageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                    Logger.Info(_className, $"CurrentPage changed to: {CurrentPage}");
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set { _totalPages = value; OnPropertyChanged(); }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                Logger.Info(_className, $"SearchQuery changed to: '{SearchQuery}'");
                // Tự động tìm kiếm khi người dùng nhập
                SearchCommand.Execute(null);
            }
        }

        public bool IsAddDrinkButtonEnabled
        {
            get => _isAddDrinkButtonEnabled;
            set
            {
                _isAddDrinkButtonEnabled = value;
                OnPropertyChanged();
                AddDrinkCommand.RaiseCanExecuteChanged();
                Logger.Info(_className, $"IsAddDrinkButtonEnabled changed to: {IsAddDrinkButtonEnabled}");
            }
        }

        public string PageInfo => $"Trang {CurrentPage}/{TotalPages}";

        public bool CanGoToPreviousPage => CurrentPage > 1;
        public bool CanGoToNextPage => CurrentPage < TotalPages && TotalPages > 0;

        public RelayCommand<object> AddDrinkCommand { get; private set; }
        public RelayCommand<DrinkViewModel> EditDrinkCommand { get; private set; }
        public RelayCommand<DrinkViewModel> DeleteDrinkCommand { get; private set; }
        public AsyncRelayCommand PreviousPageCommand { get; private set; }
        public AsyncRelayCommand NextPageCommand { get; private set; }
        public AsyncRelayCommand SearchCommand { get; private set; }
        public RelayCommand<object> ClearSearchCommand { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public DrinkManagementViewModel()
        {
            Logger.Info(_className, "Constructor started");
            try
            {
                var dbContext = new DAO.DBContext();
                var logger = App.LoggerFactory.CreateLogger<DrinkDAO>();
                var drinkDAO = new DrinkDAO(dbContext, logger);
                _drinkService = new DrinkService(drinkDAO, App.LoggerFactory.CreateLogger<DrinkService>());
                _pagedDrinks = new ObservableCollection<DrinkViewModel>();
                _selectedDrink = null;
                _currentPage = 1;

                AddDrinkCommand = new RelayCommand<object>(AddDrink, _ => IsAddDrinkButtonEnabled);
                EditDrinkCommand = new RelayCommand<DrinkViewModel>(EditDrink, CanEditOrDelete);
                DeleteDrinkCommand = new RelayCommand<DrinkViewModel>(DeleteDrink, CanEditOrDelete);
                PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync, () => CanGoToPreviousPage);
                NextPageCommand = new AsyncRelayCommand(NextPageAsync, () => CanGoToNextPage);
                SearchCommand = new AsyncRelayCommand(SearchAsync);
                ClearSearchCommand = new RelayCommand<object>(ClearSearch);

                Logger.Info(_className, "Constructor completed successfully");
                Init();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error in constructor", ex);
                MessageBox.Show($"Lỗi khi khởi tạo: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Init()
        {
            await LoadDrinksAsync();
        }

        private async Task LoadDrinksAsync()
        {
            try
            {
                Logger.Info(_className, $"Loading drinks, SearchQuery: '{SearchQuery}', CurrentPage: {CurrentPage}");
                var drinks = string.IsNullOrWhiteSpace(SearchQuery)
                    ? _drinkService.GetPagedDrinks(CurrentPage, PageSize)
                    : _drinkService.GetPagedDrinks(CurrentPage, PageSize, SearchQuery);

                int totalDrinks = string.IsNullOrWhiteSpace(SearchQuery)
                    ? _drinkService.GetTotalDrinks()
                    : _drinkService.GetTotalDrinks(SearchQuery);

                TotalPages = (int)Math.Ceiling((double)totalDrinks / PageSize);

                if (totalDrinks == 0)
                {
                    CurrentPage = 1;
                    PagedDrinks.Clear();
                    Logger.Info(_className, "No drinks found");
                }
                else
                {
                    if (CurrentPage < 1) CurrentPage = 1;
                    if (CurrentPage > TotalPages) CurrentPage = TotalPages;

                    PagedDrinks.Clear();
                    foreach (var drink in drinks)
                    {
                        PagedDrinks.Add(new DrinkViewModel
                        {
                            DrinkId = drink.DrinkId,
                            Name = drink.Name ?? string.Empty,
                            Description = drink.Description ?? string.Empty,
                            Price = drink.Price ?? 0,
                            IsAvailable = drink.IsAvailable ?? false,
                            Img = drink.img ?? string.Empty
                        });
                    }
                    Logger.Info(_className, $"Loaded {drinks.Count()} drinks for page {CurrentPage}");
                }

                OnPropertyChanged(nameof(PageInfo));
                OnPropertyChanged(nameof(CanGoToPreviousPage));
                OnPropertyChanged(nameof(CanGoToNextPage));
                (PreviousPageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (NextPageCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error loading drinks", ex);
                MessageBox.Show($"Lỗi khi tải danh sách đồ uống: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SearchAsync()
        {
            Logger.Info(_className, $"Searching with query: '{SearchQuery}'");
            CurrentPage = 1; // Đặt lại về trang 1 khi tìm kiếm
            await LoadDrinksAsync();
        }

        private void ClearSearch(object parameter)
        {
            Logger.Info(_className, "Clearing search query");
            SearchQuery = string.Empty; // Tự động gọi LoadDrinksAsync qua setter
        }

        private async Task NextPageAsync()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
                await LoadDrinksAsync();
            }
        }

        private async Task PreviousPageAsync()
        {
            try
            {
                if (CanGoToPreviousPage)
                {
                    CurrentPage--;
                    Logger.Info(_className, $"Moving to previous page, CurrentPage: {CurrentPage}");
                    await LoadDrinksAsync();
                }
                else
                {
                    Logger.Info(_className, $"Cannot go to previous page, CurrentPage: {CurrentPage}, CanGoToPreviousPage: {CanGoToPreviousPage}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error moving to previous page", ex);
                MessageBox.Show($"Lỗi khi chuyển trang trước: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddDrink(object parameter)
        {
            Logger.Info(_className, "Starting AddDrink command");
            try
            {
                if (_activeEditPage != null && _activeEditPage.IsVisible)
                {
                    Logger.Warn(_className, "Another DrinkEditPage is already open");
                    _activeEditPage.Activate();
                    return;
                }

                Logger.Info(_className, "Creating new DrinkViewModel");
                var drinkViewModel = new DrinkViewModel();
                Logger.Info(_className, $"DrinkViewModel created: {(drinkViewModel != null)}");
                Logger.Info(_className, "Creating DrinkEditPage");
                var editPage = new DrinkEditPage(drinkViewModel);
                _activeEditPage = editPage;
                IsAddDrinkButtonEnabled = false;
                Logger.Info(_className, "DrinkEditPage created successfully");
                editPage.Closed += async (s, e) =>
                {
                    Logger.Info(_className, $"DrinkEditPage closed, DialogResult: {editPage.DialogResult}");
                    _activeEditPage = null;
                    IsAddDrinkButtonEnabled = true;
                    if (editPage.DialogResult == true)
                    {
                        var drink = new Drink
                        {
                            Name = editPage.ViewModel.Drink.Name,
                            Description = editPage.ViewModel.Drink.Description,
                            Price = editPage.ViewModel.Drink.Price,
                            IsAvailable = editPage.ViewModel.Drink.IsAvailable,
                            img = editPage.ViewModel.Drink.Img
                        };
                        try
                        {
                            Logger.Info(_className, $"Attempting to add drink: {drink.Name}");
                            if (_drinkService.AddDrink(drink))
                            {
                                CurrentPage = 1;
                                await LoadDrinksAsync();
                                MessageBox.Show("Thêm đồ uống thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                                Logger.Info(_className, "Drink added successfully");
                            }
                            else
                            {
                                MessageBox.Show("Không thể thêm đồ uống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                Logger.Error(_className, "Failed to add drink");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(_className, "Error adding drink", ex);
                            MessageBox.Show($"Lỗi khi thêm đồ uống: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                };
                Logger.Info(_className, "Showing DrinkEditPage dialog");
                editPage.ShowDialog();
                Logger.Info(_className, "ShowDialog returned");
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error in AddDrink", ex);
                MessageBox.Show($"Lỗi khi mở trang thêm đồ uống: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                _activeEditPage = null;
                IsAddDrinkButtonEnabled = true;
            }
        }

        private void EditDrink(DrinkViewModel? drinkViewModel)
        {
            if (drinkViewModel != null)
            {
                Logger.Info(_className, $"Starting EditDrink for DrinkId: {drinkViewModel.DrinkId}");
                try
                {
                    if (_activeEditPage != null && _activeEditPage.IsVisible)
                    {
                        Logger.Warn(_className, "Another DrinkEditPage is already open");
                        _activeEditPage.Activate();
                        return;
                    }

                    var editPage = new DrinkEditPage(new DrinkViewModel
                    {
                        DrinkId = drinkViewModel.DrinkId,
                        Name = drinkViewModel.Name,
                        Description = drinkViewModel.Description,
                        Price = drinkViewModel.Price,
                        IsAvailable = drinkViewModel.IsAvailable,
                        Img = drinkViewModel.Img
                    });
                    _activeEditPage = editPage;
                    IsAddDrinkButtonEnabled = false;
                    Logger.Info(_className, "DrinkEditPage created successfully");
                    editPage.Closed += async (s, e) =>
                    {
                        Logger.Info(_className, $"DrinkEditPage closed, DialogResult: {editPage.DialogResult}");
                        _activeEditPage = null;
                        IsAddDrinkButtonEnabled = true;
                        if (editPage.DialogResult == true)
                        {
                            var drink = new Drink
                            {
                                DrinkId = editPage.ViewModel.Drink.DrinkId,
                                Name = editPage.ViewModel.Drink.Name,
                                Description = editPage.ViewModel.Drink.Description,
                                Price = editPage.ViewModel.Drink.Price,
                                IsAvailable = editPage.ViewModel.Drink.IsAvailable,
                                img = editPage.ViewModel.Drink.Img
                            };
                            try
                            {
                                Logger.Info(_className, $"Attempting to update drink: {drink.Name}");
                                if (_drinkService.UpdateDrink(drink))
                                {
                                    await LoadDrinksAsync();
                                    MessageBox.Show("Cập nhật đồ uống thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                                    Logger.Info(_className, "Drink updated successfully");
                                }
                                else
                                {
                                    MessageBox.Show("Không thể cập nhật đồ uống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                    Logger.Error(_className, "Failed to update drink");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(_className, "Error updating drink", ex);
                                MessageBox.Show($"Lỗi khi cập nhật đồ uống: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };
                    Logger.Info(_className, "Showing DrinkEditPage dialog");
                    editPage.ShowDialog();
                    Logger.Info(_className, "ShowDialog returned");
                }
                catch (Exception ex)
                {
                    Logger.Error(_className, "Error creating DrinkEditPage", ex);
                    MessageBox.Show($"Lỗi khi mở trang sửa đồ uống: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    _activeEditPage = null;
                    IsAddDrinkButtonEnabled = true;
                }
            }
        }

        private void DeleteDrink(DrinkViewModel? drinkViewModel)
        {
            if (drinkViewModel != null)
            {
                Logger.Info(_className, $"Attempting to delete drink: {drinkViewModel.Name}, DrinkId: {drinkViewModel.DrinkId}");
                if (MessageBox.Show("Bạn có chắc muốn xóa đồ uống này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (_drinkService.DeleteDrink(drinkViewModel.DrinkId))
                        {
                            Logger.Info(_className, $"Drink deleted successfully: {drinkViewModel.Name}");
                            _ = LoadDrinksAsync();
                            MessageBox.Show("Xóa đồ uống thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            Logger.Error(_className, $"Failed to delete drink: {drinkViewModel.Name}");
                            MessageBox.Show("Không thể xóa đồ uống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, $"Error deleting drink: {drinkViewModel.Name}", ex);
                        MessageBox.Show($"Lỗi khi xóa đồ uống: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Logger.Info(_className, $"Delete drink cancelled for: {drinkViewModel.Name}");
                }
            }
        }

        private bool CanEditOrDelete(DrinkViewModel? drink) => drink != null;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}