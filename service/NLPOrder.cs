using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.Utils;
using Microsoft.Extensions.Logging;
using System.IO;

public class NLPOrder
{
    private List<Drink> allDrinks = new DrinkDAO(new DBContext(), new LoggerFactory().CreateLogger<DrinkDAO>()).GetAllDrinks();
    private static readonly string AI_Key1 = AppConfig.Get("NLP:Key1");
    private static readonly string region = AppConfig.Get("NLP:Region");
    private static readonly string endpoint = AppConfig.Get("NLP:Endpoint");

    private static readonly string logPath = Path.Combine(AppContext.BaseDirectory, "log.txt");

    private static void Log(string message)
    {
        try
        {
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
        }
        catch
        {
            // Ignore logging failure
        }
    }

    public async Task<List<(string name, int quantity)>> AnalyzeOrderAsync(string text)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AI_Key1);

        try
        {
            var requestUri = $"{endpoint}/openai/deployments/gpt35/chat/completions?api-version=2024-03-01";
            var requestBody = new
            {
                messages = new[]
                {
                    new { role = "system", content = "Bạn là trợ lý quán cà phê. Trích xuất các món uống và số lượng từ câu tiếng Việt, trả về dạng JSON." },
                    new { role = "user", content = text }
                },
                temperature = 0.2,
                max_tokens = 300
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(requestUri, content);

            if (!response.IsSuccessStatusCode)
            {
                Log($"❌ Azure OpenAI call failed: {response.StatusCode}");
                throw new Exception($"Azure OpenAI call failed: {response.StatusCode}");
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(responseStream);
            var message = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            Log("✅ AI response: " + message);

            // Tìm JSON trong câu trả lời
            var result = new List<(string name, int quantity)>();
            try
            {
                var orders = JsonDocument.Parse(message).RootElement.GetProperty("orders");
                foreach (var item in orders.EnumerateArray())
                {
                    var name = item.GetProperty("name").GetString();
                    var qty = item.GetProperty("quantity").GetInt32();
                    result.Add((name, qty));
                }
            }
            catch (Exception parseEx)
            {
                Log("❌ JSON parse failed: " + parseEx.Message);
                throw new Exception("Failed to parse AI response", parseEx);
            }

            return result;
        }
        catch (Exception ex)
        {
            Log("❌ Tổng lỗi: " + ex.Message + "\n" + ex.StackTrace);
            throw;
        }
    }

    public static async Task<string> SaveOrderToJsonAndUploadAsync(List<(string name, int quantity)> orders)
    {
        var s3Client = new S3Client(
            AppConfig.Get("S3:accessKeyId"),
            AppConfig.Get("S3:secretAccessKey"),
            AppConfig.Get("S3:bucketName"),
            AppConfig.Get("S3:folder")
        );

        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CafeholicOrders");
        Directory.CreateDirectory(folderPath);

        int index = 1;
        string filePath;
        do
        {
            filePath = Path.Combine(folderPath, $"Order_{index}.json");
            index++;
        } while (File.Exists(filePath));

        var orderData = new
        {
            datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            items = orders.Select(o => new { name = o.name, quantity = o.quantity })
        };

        string json = JsonSerializer.Serialize(orderData, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json);
        Log("✅ Saved order JSON to: " + filePath);

        string uploadedUrl = await s3Client.UploadFileAsync(filePath);
        Log("✅ Uploaded to S3: " + uploadedUrl);

        return uploadedUrl;
    }
}
