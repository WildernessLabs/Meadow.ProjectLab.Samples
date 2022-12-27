using System.Text.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WifiWeather.Models;

namespace WifiWeather.Services
{
    public static class WeatherService
    {
        static string climateDataUri = "http://api.openweathermap.org/data/2.5/weather";
        static string city = $"Vancouver";
        static string apiKey = $"7d8e6485ff906aa4877c07c17c32949f";

        static WeatherService() { }

        public static async Task<WeatherReading> GetWeatherForecast()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 5, 0);

                    HttpResponseMessage response = await client.GetAsync($"{climateDataUri}?q={city}&appid={apiKey}");
            
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<WeatherReading>(json);
                    return values;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request timed out.");
                    return null;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                    return null;
                }
            }
        }
    }
}