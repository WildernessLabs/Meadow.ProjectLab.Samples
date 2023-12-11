using Meadow;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WifiWeather.DTOs;
using WifiWeather.Models;

namespace WifiWeather.Controllers
{
    internal class RestClientController
    {
        string climateDataUri = "http://api.openweathermap.org/data/2.5/weather";

        public async Task<(double, double, double, double, DateTime, DateTime, string)?> GetWeatherForecast()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 5, 0);

                    HttpResponseMessage response = await client.GetAsync($"{climateDataUri}?q={Secrets.WEATHER_CITY}&appid={Secrets.WEATHER_API_KEY}");

                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<WeatherReadingDTO>(json);

                    double outdoorTemperature = values.main.temp - 273;
                    double outdoorHumidity = values.main.humidity;
                    double outdoorPressure = values.main.pressure;
                    double feelsLikeTemperature = values.main.feels_like - 273;
                    DateTime sunrise = DateTimeOffset.FromUnixTimeSeconds(values.sys.sunrise).DateTime.AddHours(-8);
                    DateTime sunset = DateTimeOffset.FromUnixTimeSeconds(values.sys.sunset).DateTime.AddHours(-8);
                    string weatherIconFile = GetWeatherIcon(values.weather[0].id);

                    return (outdoorTemperature, outdoorHumidity, outdoorPressure, feelsLikeTemperature, sunrise, sunset, weatherIconFile);
                }
                catch (TaskCanceledException)
                {
                    Resolver.Log.Info("Request timed out.");
                    return null;
                }
                catch (Exception e)
                {
                    Resolver.Log.Info($"Request went sideways: {e.Message}");
                    return null;
                }
            }
        }

        string GetWeatherIcon(int weatherCode)
        {
            string resourceName;

            switch (weatherCode)
            {
                case int n when (n >= WeatherCodeConstants.THUNDERSTORM_LIGHT_RAIN && n <= WeatherCodeConstants.THUNDERSTORM_HEAVY_DRIZZLE):
                    resourceName = $"WifiWeather.Resources.w_storm.bmp";
                    break;
                case int n when (n >= WeatherCodeConstants.DRIZZLE_LIGHT && n <= WeatherCodeConstants.DRIZZLE_SHOWER):
                    resourceName = $"WifiWeather.Resources.w_drizzle.bmp";
                    break;
                case int n when (n >= WeatherCodeConstants.RAIN_LIGHT && n <= WeatherCodeConstants.RAIN_SHOWER_RAGGED):
                    resourceName = $"WifiWeather.Resources.w_rain.bmp";
                    break;
                case int n when (n >= WeatherCodeConstants.SNOW_LIGHT && n <= WeatherCodeConstants.SNOW_SHOWER_HEAVY):
                    resourceName = $"WifiWeather.Resources.w_snow.bmp";
                    break;
                case WeatherCodeConstants.CLOUDS_CLEAR:
                    resourceName = $"WifiWeather.Resources.w_clear.bmp";
                    break;
                case int n when (n >= WeatherCodeConstants.CLOUDS_FEW && n <= WeatherCodeConstants.CLOUDS_OVERCAST):
                    resourceName = $"WifiWeather.Resources.w_cloudy.bmp";
                    break;
                default:
                    resourceName = $"WifiWeather.Resources.w_misc.bmp";
                    break;
            }

            return resourceName;
        }
    }
}