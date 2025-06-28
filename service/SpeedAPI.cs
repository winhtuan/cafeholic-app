using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace service
{

    public class SpeedSMSApi
    {
        private readonly string accessToken;
        private readonly HttpClient httpClient;

        public SpeedSMSApi(string token)
        {
            accessToken = token;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.BaseAddress = new Uri("https://api.speedsms.vn/");
        }

        public async Task<string> SendSMS(string[] phones, string content, int sms_type, string sender)
        {
            var data = new
            {
                to = phones,
                content = content,
                sms_type = sms_type,
                sender = sender
            };

            var json = JsonConvert.SerializeObject(data);
            var contentData = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("index.php/sms/send", contentData);
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
