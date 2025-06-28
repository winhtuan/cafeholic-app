using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CAFEHOLIC.Model;
using CAFEHOLIC.service;

namespace CAFEHOLIC.ViewModel
{
    public class RecommendDrinkViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Drink> _recommendedDrinks;
        public ObservableCollection<Drink> RecommendedDrinks
        {
            get => _recommendedDrinks;
            set
            {
                _recommendedDrinks = value;
                OnPropertyChanged();
            }
        }

        private string _suggestionText = string.Empty;
        public string SuggestionText
        {
            get => _suggestionText;
            set
            {
                _suggestionText = value;
                OnPropertyChanged();
            }
        }

        public RecommendDrinkViewModel()
        {
            RecommendedDrinks = new ObservableCollection<Drink>();
            LoadRecommendedDrinksAsync();
        }

        private async void LoadRecommendedDrinksAsync()
        {
            int userId = AppSession.CurrentUserId;
            ProductService service = new ProductService();
            var result = await service.GetRecommentDrink(userId);
            SuggestionText = result.SuggestionText;
            RecommendedDrinks = new ObservableCollection<Drink>(result.Drinks);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
