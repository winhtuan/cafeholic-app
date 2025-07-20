using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.view.Page;
using CAFEHOLIC.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CAFEHOLIC.ViewModel
{
    public class RoomManagementViewModel : INotifyPropertyChanged
    {
        private readonly RoomDAO _roomDAO;
        private readonly RoomTypeDAO _roomTypeDAO;
        private ObservableCollection<RoomViewModel> _pagedRooms;
        private RoomViewModel? _selectedRoom;
        private string _searchQuery = string.Empty;
        private int _currentPage = 1;
        private int _totalPages;
        private const int PageSize = 10;
        private Window _activeRoomEditPage;
        private Window _activeRoomTypeEditPage;
        private bool _isAddRoomButtonEnabled = true;
        private bool _isAddRoomTypeButtonEnabled = true;
        private readonly string _className = nameof(RoomManagementViewModel);

        public ObservableCollection<RoomViewModel> PagedRooms
        {
            get => _pagedRooms;
            set { _pagedRooms = value; OnPropertyChanged(); }
        }

        public RoomViewModel? SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
                Logger.Info(_className, $"SelectedRoom set to: {(value != null ? $"RoomId={value.RoomId}, Name={value.Name}" : "null")}");
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                Logger.Info(_className, $"SearchQuery set to: '{value}'");
                SearchCommand.Execute(null);
            }
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
                    CommandManager.InvalidateRequerySuggested();
                    Logger.Info(_className, $"CurrentPage set to: {value}");
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
                Logger.Info(_className, $"TotalPages set to: {value}");
            }
        }

        public bool IsAddRoomButtonEnabled
        {
            get => _isAddRoomButtonEnabled;
            set
            {
                _isAddRoomButtonEnabled = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
                Logger.Info(_className, $"IsAddRoomButtonEnabled set to: {value}");
            }
        }

        public bool IsAddRoomTypeButtonEnabled
        {
            get => _isAddRoomTypeButtonEnabled;
            set
            {
                _isAddRoomTypeButtonEnabled = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
                Logger.Info(_className, $"IsAddRoomTypeButtonEnabled set to: {value}");
            }
        }

        public string PageInfo => $"Trang {CurrentPage}/{TotalPages}";

        public bool CanGoToPreviousPage => CurrentPage > 1;
        public bool CanGoToNextPage => CurrentPage < TotalPages && TotalPages > 0;

        public RelayCommand<object> AddRoomCommand { get; private set; }
        public RelayCommand<object> AddRoomTypeCommand { get; private set; }
        public RelayCommand<RoomViewModel> EditRoomCommand { get; private set; }
        public RelayCommand<RoomViewModel> DeleteRoomCommand { get; private set; }
        public AsyncRelayCommand SearchCommand { get; private set; }
        public RelayCommand<object> ClearSearchCommand { get; private set; }
        public AsyncRelayCommand PreviousPageCommand { get; private set; }
        public AsyncRelayCommand NextPageCommand { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RoomManagementViewModel()
        {
            Logger.Info(_className, "Constructor started");
            try
            {
                var dbContext = new dao.DBContext();
                _roomDAO = new RoomDAO(dbContext.GetLogger<RoomDAO>());
                _roomTypeDAO = new RoomTypeDAO(dbContext.GetLogger<RoomTypeDAO>());
                _pagedRooms = new ObservableCollection<RoomViewModel>();

                AddRoomCommand = new RelayCommand<object>(AddRoom, _ => IsAddRoomButtonEnabled);
                AddRoomTypeCommand = new RelayCommand<object>(AddRoomType, _ => IsAddRoomTypeButtonEnabled);
                EditRoomCommand = new RelayCommand<RoomViewModel>(EditRoom, CanEditOrDelete);
                DeleteRoomCommand = new RelayCommand<RoomViewModel>(DeleteRoom, CanEditOrDelete);
                SearchCommand = new AsyncRelayCommand(SearchAsync);
                ClearSearchCommand = new RelayCommand<object>(ClearSearch);
                PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync, () => CanGoToPreviousPage);
                NextPageCommand = new AsyncRelayCommand(NextPageAsync, () => CanGoToNextPage);

                Logger.Info(_className, "Constructor completed successfully");
                LoadRoomsAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error in constructor", ex);
                MessageBox.Show($"Lỗi khi khởi tạo: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadRoomsAsync()
        {
            try
            {
                Logger.Info(_className, $"Loading rooms, SearchQuery: '{SearchQuery}', CurrentPage: {CurrentPage}");
                var rooms = string.IsNullOrWhiteSpace(SearchQuery)
                    ? _roomDAO.GetPagedRooms(CurrentPage, PageSize)
                    : _roomDAO.GetPagedRooms(CurrentPage, PageSize, SearchQuery);

                int totalRooms = string.IsNullOrWhiteSpace(SearchQuery)
                    ? _roomDAO.GetTotalRooms()
                    : _roomDAO.GetTotalRooms(SearchQuery);

                TotalPages = (int)Math.Ceiling((double)totalRooms / PageSize);

                if (totalRooms == 0)
                {
                    CurrentPage = 1;
                    PagedRooms.Clear();
                    Logger.Info(_className, "No rooms found");
                }
                else
                {
                    if (CurrentPage < 1) CurrentPage = 1;
                    if (CurrentPage > TotalPages) CurrentPage = TotalPages;

                    PagedRooms.Clear();
                    foreach (var room in rooms)
                    {
                        PagedRooms.Add(new RoomViewModel
                        {
                            RoomId = room.RoomId,
                            Name = room.Name ?? "Unnamed",
                            RoomType = room.RoomType?.Name ?? "Unknown",
                            Capacity = $"{room.RoomType?.MinCapacity}–{room.RoomType?.MaxCapacity}",
                            IsAvailable = room.IsAvailable ?? false
                        });
                    }
                    Logger.Info(_className, $"Loaded {rooms.Count} rooms for page {CurrentPage}");
                }

                OnPropertyChanged(nameof(PageInfo));
                OnPropertyChanged(nameof(CanGoToPreviousPage));
                OnPropertyChanged(nameof(CanGoToNextPage));
                CommandManager.InvalidateRequerySuggested();
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error loading rooms", ex);
                MessageBox.Show($"Lỗi khi tải danh sách phòng: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SearchAsync()
        {
            Logger.Info(_className, $"Searching with query: '{SearchQuery}'");
            CurrentPage = 1;
            LoadRoomsAsync();
        }

        private void ClearSearch(object parameter)
        {
            Logger.Info(_className, "Clearing search query");
            SearchQuery = string.Empty;
        }

        private async Task NextPageAsync()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
                Logger.Info(_className, $"Moving to next page, CurrentPage: {CurrentPage}");
                LoadRoomsAsync();
            }
        }

        private async Task PreviousPageAsync()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
                Logger.Info(_className, $"Moving to previous page, CurrentPage: {CurrentPage}");
                LoadRoomsAsync();
            }
        }

        private void AddRoom(object parameter)
        {
            Logger.Info(_className, "Starting AddRoom command");
            try
            {
                if (_activeRoomEditPage != null && _activeRoomEditPage.IsVisible)
                {
                    Logger.Warn(_className, "Another RoomEditPage is already open");
                    _activeRoomEditPage.Activate();
                    return;
                }

                var editViewModel = new RoomEditViewModel();
                var editPage = new RoomEditPage(editViewModel);
                _activeRoomEditPage = editPage;
                IsAddRoomButtonEnabled = false;
                Logger.Info(_className, "RoomEditPage created successfully");
                editPage.Closed += (s, e) =>
                {
                    Logger.Info(_className, $"RoomEditPage closed, DialogResult: {editPage.DialogResult}");
                    _activeRoomEditPage = null;
                    IsAddRoomButtonEnabled = true;
                    if (editPage.DialogResult == true)
                    {
                        var room = new StudyRoom
                        {
                            Name = editPage.ViewModel.Name,
                            IsAvailable = editPage.ViewModel.IsAvailable,
                            RoomTypeId = editPage.ViewModel.RoomTypeId
                        };
                        try
                        {
                            Logger.Info(_className, $"Attempting to add room: {room.Name}");
                            if (_roomDAO.AddRoom(room))
                            {
                                CurrentPage = 1;
                                LoadRoomsAsync();
                                MessageBox.Show("Thêm phòng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                                Logger.Info(_className, $"Room added successfully: {room.Name}");
                            }
                            else
                            {
                                MessageBox.Show("Không thể thêm phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                Logger.Error(_className, $"Failed to add room: {room.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(_className, $"Error adding room: {room.Name}", ex);
                            MessageBox.Show($"Lỗi khi thêm phòng: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                };
                Logger.Info(_className, "Showing RoomEditPage dialog");
                editPage.ShowDialog();
                Logger.Info(_className, "ShowDialog returned");
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error in AddRoom", ex);
                MessageBox.Show($"Lỗi khi mở trang thêm phòng: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                _activeRoomEditPage = null;
                IsAddRoomButtonEnabled = true;
            }
        }

        private void AddRoomType(object parameter)
        {
            Logger.Info(_className, "Starting AddRoomType command");
            try
            {
                if (_activeRoomTypeEditPage != null && _activeRoomTypeEditPage.IsVisible)
                {
                    Logger.Warn(_className, "Another RoomTypeEditPage is already open");
                    _activeRoomTypeEditPage.Activate();
                    return;
                }

                var roomTypeViewModel = new RoomTypeViewModel();
                var editPage = new RoomTypeEditPage(roomTypeViewModel);
                _activeRoomTypeEditPage = editPage;
                IsAddRoomTypeButtonEnabled = false;
                Logger.Info(_className, "RoomTypeEditPage created successfully");
                editPage.Closed += (s, e) =>
                {
                    Logger.Info(_className, $"RoomTypeEditPage closed, DialogResult: {editPage.DialogResult}");
                    _activeRoomTypeEditPage = null;
                    IsAddRoomTypeButtonEnabled = true;
                    if (editPage.DialogResult == true)
                    {
                        var roomType = new RoomType
                        {
                            Name = editPage.ViewModel.Name,
                            MinCapacity = editPage.ViewModel.MinCapacity,
                            MaxCapacity = editPage.ViewModel.MaxCapacity,
                            Description = editPage.ViewModel.Description
                        };
                        try
                        {
                            Logger.Info(_className, $"Attempting to add room type: {roomType.Name}");
                            if (_roomTypeDAO.AddRoomType(roomType))
                            {
                                MessageBox.Show("Thêm loại phòng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                                Logger.Info(_className, $"Room type added successfully: {roomType.Name}");
                            }
                            else
                            {
                                MessageBox.Show("Không thể thêm loại phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                Logger.Error(_className, $"Failed to add room type: {roomType.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(_className, $"Error adding room type: {roomType.Name}", ex);
                            MessageBox.Show($"Lỗi khi thêm loại phòng: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                };
                Logger.Info(_className, "Showing RoomTypeEditPage dialog");
                editPage.ShowDialog();
                Logger.Info(_className, "ShowDialog returned");
            }
            catch (Exception ex)
            {
                Logger.Error(_className, "Error in AddRoomType", ex);
                MessageBox.Show($"Lỗi khi mở trang thêm loại phòng: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                _activeRoomTypeEditPage = null;
                IsAddRoomTypeButtonEnabled = true;
            }
        }

        private void EditRoom(RoomViewModel? roomViewModel)
        {
            if (roomViewModel != null)
            {
                Logger.Info(_className, $"Starting EditRoom for RoomId: {roomViewModel.RoomId}, Name: {roomViewModel.Name}");
                try
                {
                    if (_activeRoomEditPage != null && _activeRoomEditPage.IsVisible)
                    {
                        Logger.Warn(_className, "Another RoomEditPage is already open");
                        _activeRoomEditPage.Activate();
                        return;
                    }

                    var editViewModel = new RoomEditViewModel
                    {
                        RoomId = roomViewModel.RoomId,
                        Name = roomViewModel.Name,
                        IsAvailable = roomViewModel.IsAvailable,
                        RoomTypeId = _roomDAO.GetRoomById(roomViewModel.RoomId)?.RoomTypeId ?? 0
                    };
                    var editPage = new RoomEditPage(editViewModel);
                    _activeRoomEditPage = editPage;
                    IsAddRoomButtonEnabled = false;
                    Logger.Info(_className, "RoomEditPage created successfully");
                    editPage.Closed += (s, e) =>
                    {
                        Logger.Info(_className, $"RoomEditPage closed, DialogResult: {editPage.DialogResult}");
                        _activeRoomEditPage = null;
                        IsAddRoomButtonEnabled = true;
                        if (editPage.DialogResult == true)
                        {
                            var room = new StudyRoom
                            {
                                RoomId = editPage.ViewModel.RoomId,
                                Name = editPage.ViewModel.Name,
                                IsAvailable = editPage.ViewModel.IsAvailable,
                                RoomTypeId = editPage.ViewModel.RoomTypeId
                            };
                            try
                            {
                                Logger.Info(_className, $"Attempting to update room: {room.Name}");
                                if (_roomDAO.UpdateRoom(room))
                                {
                                    LoadRoomsAsync();
                                    MessageBox.Show("Cập nhật phòng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                                    Logger.Info(_className, $"Room updated successfully: {room.Name}");
                                }
                                else
                                {
                                    MessageBox.Show("Không thể cập nhật phòng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                                    Logger.Error(_className, $"Failed to update room: {room.Name}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(_className, $"Error updating room: {room.Name}", ex);
                                MessageBox.Show($"Lỗi khi cập nhật phòng: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };
                    Logger.Info(_className, "Showing RoomEditPage dialog");
                    editPage.ShowDialog();
                    Logger.Info(_className, "ShowDialog returned");
                }
                catch (Exception ex)
                {
                    Logger.Error(_className, $"Error in EditRoom for RoomId: {roomViewModel.RoomId}", ex);
                    MessageBox.Show($"Lỗi khi mở trang sửa phòng: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    _activeRoomEditPage = null;
                    IsAddRoomButtonEnabled = true;
                }
            }
        }

        private void DeleteRoom(RoomViewModel? roomViewModel)
        {
            if (roomViewModel != null)
            {
                Logger.Info(_className, $"Attempting to delete room: {roomViewModel.Name}, RoomId: {roomViewModel.RoomId}, IsAvailable: {roomViewModel.IsAvailable}");
                if (MessageBox.Show("Bạn có chắc muốn xóa phòng này? Thao tác này không thể hoàn tác.", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (roomViewModel.IsAvailable)
                        {
                            Logger.Warn(_className, $"Cannot delete room: {roomViewModel.Name}, IsAvailable: true");
                            MessageBox.Show("Phòng đang được đặt, không thể xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (_roomDAO.DeleteRoom(roomViewModel.RoomId))
                        {
                            CurrentPage = 1;
                            LoadRoomsAsync();
                            MessageBox.Show("Xóa phòng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            Logger.Info(_className, $"Room deleted successfully: {roomViewModel.Name}");
                        }
                        else
                        {
                            MessageBox.Show("Không thể xóa phòng do lỗi hệ thống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            Logger.Error(_className, $"Failed to delete room: {roomViewModel.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(_className, $"Error deleting room: {roomViewModel.Name}", ex);
                        MessageBox.Show($"Lỗi khi xóa phòng: {ex.Message}\nChi tiết: {ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    Logger.Info(_className, $"Delete room cancelled for: {roomViewModel.Name}");
                }
            }
        }

        private bool CanEditOrDelete(RoomViewModel? room) => room != null;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute((T)parameter!);

        public void Execute(object? parameter) => _execute((T)parameter!);

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (() => true);
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => !_isExecuting && _canExecute();

        public async void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    CommandManager.InvalidateRequerySuggested();
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}