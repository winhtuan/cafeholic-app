using System;
using CAFEHOLIC.dao;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using service;

namespace CAFEHOLIC.service
{
    public class DrinkRecommendationResult
    {
        public string SuggestionText { get; set; } = string.Empty;
        public List<Drink> Drinks { get; set; } = new List<Drink>();
    }
    public class GeminiDrinkSuggestion
    {
        public string suggestionText { get; set; }
        public List<string> drinks { get; set; }
    }

    public class ProductService
    {
        public async Task<DrinkRecommendationResult> GetRecommentDrink(int userID)
        {
            // 1. Lấy danh sách tất cả đồ uống hiện có
            List<Drink> allDrinks = new DrinkDAO(new DBContext(), new DBContext().GetLogger<DrinkDAO>()).GetAllDrinks();
            var drinkNames = allDrinks
                .Where(d => d.IsAvailable == true && !string.IsNullOrWhiteSpace(d.Name))
                .Select(d => d.Name)
                .ToList();

            // 2. Lấy lịch sử đơn hàng
            OrderDAO orderDAO = new OrderDAO(new DBContext(), new DBContext().GetLogger<OrderDAO>());
            List<Order> orders = orderDAO.GetOrderByUserID(userID);

            // 3. Thống kê tần suất đồ uống đã mua
            Dictionary<int, int> drinkFrequency = new Dictionary<int, int>();
            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.DrinkId != null)
                    {
                        int drinkId = item.DrinkId.Value;
                        if (!drinkFrequency.ContainsKey(drinkId))
                            drinkFrequency[drinkId] = 0;
                        drinkFrequency[drinkId] += item.Quantity ?? 1;
                    }
                }
            }

            // 4. Lấy tên các đồ uống được người dùng gọi nhiều nhất
            var topDrinkNames = drinkFrequency.OrderByDescending(x => x.Value)
                                              .Take(3)
                                              .Select(x => allDrinks.FirstOrDefault(d => d.DrinkId == x.Key)?.Name)
                                              .Where(name => !string.IsNullOrEmpty(name))
                                              .ToList();

            // 5. Lấy thời tiết
            var location = await WeatherService.GetLocationByIPAsync();
            var weather = await WeatherService.GetWeatherAsync(location.lat, location.lon);

            // 6. Tạo prompt cho Gemini
            string prompt = $"Bạn là trợ lý gợi ý đồ uống cho khách hàng. Nhiệm vụ của bạn là đề xuất 3 món đồ uống phù hợp nhất dựa trên thông tin được cung cấp.\n\n" +
                $"Thông tin khách hàng:\n" +
                $"- Các món yêu thích: {string.Join(", ", topDrinkNames)}\n" +
                $"Thông tin thời tiết hiện tại:\n" +
                $"- Điều kiện: {weather.current.condition.text}\n" +
                $"- Nhiệt độ: {weather.current.temp_c}°C\n\n" +
                $"Danh sách đồ uống có sẵn của quán:\n" +
                $"{string.Join(", ", drinkNames)}\n\n" +
                $"Yêu cầu:\n" +
                $"1. Gợi ý 3 món đồ uống từ danh sách trên, ưu tiên phù hợp với sở thích của khách hàng và điều kiện thời tiết hiện tại.\n" +
                $"2. Viết thêm một câu dẫn ngắn gọn (bằng tiếng Việt) để giới thiệu danh sách gợi ý này đến người dùng.\n\n" +
                $"Định dạng phản hồi **bắt buộc phải là JSON** theo cấu trúc sau:\n" +
                "{\n" +
                "  \"suggestionText\": \"[Câu dẫn của bạn]\",\n" +
                "  \"drinks\": [\"[Tên đồ uống 1]\", \"[Tên đồ uống 2]\", \"[Tên đồ uống 3]\"]\n" +
                "}";
            AppSession.logger.LogInformation("Gửi prompt đến Gemini: {Prompt}", prompt);
            // 7. Gửi prompt đến Gemini để lấy đề xuất
            var geminiReply = await GeminiService.AskGemini(prompt);
            string json = ExtractJsonFromGeminiReply(geminiReply);

            GeminiDrinkSuggestion geminiResult = JsonConvert.DeserializeObject<GeminiDrinkSuggestion>(json);

            var recommendedDrinks = allDrinks
            .Where(d => geminiResult.drinks.Any(name =>
            string.Equals(name, d.Name, StringComparison.OrdinalIgnoreCase)))
             .ToList();

            string suggestionText = geminiResult?.suggestionText ?? "Dưới đây là các gợi ý đồ uống:";
           
            return new DrinkRecommendationResult
            {
                SuggestionText = suggestionText,
                Drinks = recommendedDrinks
            };
        }
        public static string ExtractJsonFromGeminiReply(string reply)
        {
            int start = reply.IndexOf('{');
            int end = reply.LastIndexOf('}');

            if (start >= 0 && end > start)
                return reply.Substring(start, end - start + 1);

            throw new FormatException("Không tìm thấy đoạn JSON hợp lệ trong phản hồi Gemini.");
        }

    }

}
