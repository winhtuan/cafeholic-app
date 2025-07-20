using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.Utils;
using Microsoft.Extensions.Logging;
using System.IO;

public class NLPOrder
{
    private const string ClassName = nameof(NLPOrder);

    private List<Drink> allDrinks = new DrinkDAO(new DBContext(), new LoggerFactory().CreateLogger<DrinkDAO>()).GetAllDrinks();
    private static readonly string AIKey = AppConfig.Get("AzureOpenAIClient:Key");
    private static readonly string AIendpoint = AppConfig.Get("AzureOpenAIClient:Endpoint");
    private static readonly string AIModel = AppConfig.Get("AzureOpenAIClient:DeploymentName");
    private static readonly string version = AppConfig.Get("AzureOpenAIClient:ApiVersion");
    private static readonly Dictionary<string, string> keywordMapping = AppConfig.LoadKeywordMapping();

    static NLPOrder()
    {
        Logger.Info(ClassName, $"Loaded {keywordMapping.Count} keyword mappings");
        foreach (var kvp in keywordMapping.Take(5))
        {
            Logger.Info(ClassName, $"Keyword: '{kvp.Key}' -> '{kvp.Value}'");
        }
    }

    private string NormalizeName(string inputName)
    {
        foreach (var kvp in keywordMapping)
        {
            if (inputName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        foreach (var drink in allDrinks)
        {
            if (inputName.Contains(drink.Name, StringComparison.OrdinalIgnoreCase))
                return drink.Name;
        }

        return inputName;
    }

    public async Task<List<(string name, int quantity)>> AnalyzeOrderAsync(string text)
    {
        // First try local matching for better performance
        var localResult = AnalyzeOrderWithLocalMatching(text);
        if (localResult.Count > 0)
        {
            Logger.Info(ClassName, "Using local matching result");
            return localResult;
        }

        // If local matching fails, try Azure OpenAI
        try
        {
            Logger.Info(ClassName, "Local matching failed, trying Azure OpenAI...");
            return await AnalyzeOrderWithAzureOpenAI(text);
        }
        catch (Exception ex)
        {
            Logger.Warn(ClassName, $"Azure OpenAI failed: {ex.Message}");
            Logger.Info(ClassName, "Returning empty result from local matching");
            return localResult; // Return the empty result from local matching
        }
    }

    private async Task<List<(string name, int quantity)>> AnalyzeOrderWithAzureOpenAI(string text)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AIKey);

        Logger.Info(ClassName, $"Config - Endpoint: {AIendpoint}, Model: {AIModel}, Version: {version}");

            var baseUri = AIendpoint.TrimEnd('/');
            var requestUri = $"{baseUri}/openai/deployments/{AIModel}/chat/completions?api-version={version}";

        Logger.Info(ClassName, $"Request URI: {requestUri}");

            var requestBody = new
            {
                messages = new[]
                {
                new { role = "system", content = "Bạn là trợ lý quán cà phê. Trích xuất các món uống và số lượng từ câu tiếng Việt, trả về dạng JSON với key là 'orders', mỗi phần tử gồm 'name' và 'quantity'." },
                new { role = "user", content = text }
            },
                temperature = 0.2,
                max_tokens = 300
            };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        Logger.Info(ClassName, $"Request body: {jsonContent}");

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
            Logger.Error(ClassName, $"Azure OpenAI call failed: {response.StatusCode}, Body: {errorBody}");
            throw new Exception($"Azure OpenAI call failed: {response.StatusCode} - {errorBody}");
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(responseStream);
            var message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

        Logger.Info(ClassName, $"AI response: {message}");

            var result = new List<(string name, int quantity)>();
            try
            {
                var orders = JsonDocument.Parse(message).RootElement.GetProperty("orders");
                foreach (var item in orders.EnumerateArray())
                {
                    var rawName = item.GetProperty("name").GetString();
                    var normalized = NormalizeName(rawName ?? "");
                    var qty = item.GetProperty("quantity").GetInt32();
                    result.Add((normalized, qty));
                }
            }
            catch (Exception parseEx)
            {
            Logger.Error(ClassName, "JSON parse failed", parseEx);
                throw new Exception("Failed to parse AI response", parseEx);
            }

            return result;
        }

    private List<(string name, int quantity)> AnalyzeOrderWithLocalMatching(string text)
    {
        var result = new List<(string name, int quantity)>();
        var lowerText = text.ToLower().Trim();

        // Loại bỏ dấu câu
        lowerText = Regex.Replace(lowerText, @"[.,!?]", "");

        Logger.Info(ClassName, $"Analyzing text: '{text}'");
        Logger.Info(ClassName, $"Available keywords: {string.Join(", ", keywordMapping.Keys.Take(10))}");

        // Remove common words that don't affect the order
        var filteredText = lowerText;
        var commonWords = new[] { "ly", "cốc", "chén", "tách", "cho", "tôi", "một", "hai", "ba", "bốn", "năm" };
        foreach (var word in commonWords)
        {
            filteredText = filteredText.Replace(word, " ").Replace("  ", " ").Trim();
        }
        
        Logger.Info(ClassName, $"Filtered text: '{filteredText}'");

        // Split by common conjunctions to handle multiple items
        var itemParts = SplitByConjunctions(filteredText);
        Logger.Info(ClassName, $"Split into {itemParts.Count} parts: {string.Join(" | ", itemParts)}");

        foreach (var part in itemParts)
        {
            var itemResult = ParseSingleItem(part.Trim());
            if (itemResult.HasValue)
    {
                result.Add(itemResult.Value);
                Logger.Info(ClassName, $"Parsed item: {itemResult.Value.name} x{itemResult.Value.quantity}");
    }
        }

        if (result.Count == 0)
        {
            Logger.Warn(ClassName, $"No matches found for text: '{text}'");
        }

        return result;
    }

    private List<string> SplitByConjunctions(string text)
    {
        var conjunctions = new[] { "và", "với", "cùng", "thêm", "nữa", "đồng thời" };
        var parts = new List<string> { text };

        foreach (var conj in conjunctions)
        {
            var newParts = new List<string>();
            foreach (var part in parts)
            {
                var split = part.Split(new[] { conj }, StringSplitOptions.RemoveEmptyEntries);
                newParts.AddRange(split);
            }
            parts = newParts;
        }

        return parts.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();
    }

    private (string name, int quantity)? ParseSingleItem(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        // Extract quantity (default to 1 if not specified)
        int quantity = 1;
        var remainingWords = new List<string>();

        foreach (var word in words)
        {
            if (int.TryParse(word, out int qty) && qty > 0)
            {
                quantity = qty;
            }
            else
            {
                remainingWords.Add(word);
            }
        }

        var itemText = string.Join(" ", remainingWords);
        if (string.IsNullOrWhiteSpace(itemText))
        {
            return null;
        }

        // Try keyword mapping first
        foreach (var kvp in keywordMapping)
        {
            if (string.IsNullOrEmpty(kvp.Value)) continue;
            
            if (itemText.Contains(kvp.Key.ToLower()))
            {
                return (kvp.Value, quantity);
            }
        }

        // Try direct drink name matching
        foreach (var drink in allDrinks)
        {
            if (itemText.Contains(drink.Name.ToLower()))
            {
                return (drink.Name, quantity);
            }
        }

        // Try partial matching for better results
        foreach (var drink in allDrinks)
        {
            var drinkWords = drink.Name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var drinkWord in drinkWords)
            {
                if (drinkWord.Length > 2 && itemText.Contains(drinkWord))
                {
                    return (drink.Name, quantity);
                }
            }
        }

        return null;
    }

}

