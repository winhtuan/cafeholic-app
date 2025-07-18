using System.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace service
{
    public class LocationInfo
    {
        public string city { get; set; }
        public string country { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class WeatherCondition
    {
        public string text { get; set; }
        public string icon { get; set; }
    }

    public class CurrentWeather
    {
        public float temp_c { get; set; }
        public WeatherCondition condition { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public string country { get; set; }
    }

    public class WeatherApiResponse
    {
        public Location location { get; set; }
        public CurrentWeather current { get; set; }
    }


    public static class WeatherService
    {
        public static async Task<WeatherApiResponse> GetWeatherAsync(float lat, float lon)
        {
            string apiKey = ConfigurationManager.AppSettings["Weather_key"];
            string url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={lat},{lon}";
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<WeatherApiResponse>(response);
        }

        public static async Task<LocationInfo> GetLocationByIPAsync()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("http://ip-api.com/json/");
            return JsonSerializer.Deserialize<LocationInfo>(response);
        }
    }
}