using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace service
{
    public class GeminiService
    {
        private static readonly HttpClient client = new HttpClient();
        private static string ApiKey = ConfigurationManager.AppSettings["Gemini_key"];

        public static async Task<string> AskGemini(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                new {
                    parts = new[] {
                        new { text = prompt }
                    }
                }
            }
            };

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={ApiKey}";
            string json = JsonSerializer.Serialize(requestBody);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, httpContent);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"API lỗi: {response.StatusCode} - {result}");

            // Đọc content từ JSON response
            using var doc = JsonDocument.Parse(result);
            var content = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return content;
        }
    }
}