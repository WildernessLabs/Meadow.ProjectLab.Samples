using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using WifiWeather.Models;
using WifiWeather.ViewModels;

namespace WifiWeather.Views
{
    public class WeatherView
    {
        MicroGraphics graphics;
        int x_padding = 5;

        public WeatherView() { }

        public void Initialize(IGraphicsDisplay display)
        {
            graphics = new MicroGraphics(display)
            {
                Stroke = 1,
                CurrentFont = new Font12x20(),
            };

            x_padding = display.Width > 240 ? 30 : 5;

            graphics.Clear();
        }

        public void UpdateDateTime()
        {
            int TimeZoneOffSet = -7; // PST
            var today = DateTime.Now.AddHours(TimeZoneOffSet);

            graphics.DrawRectangle(graphics.Width / 2, 24, graphics.Width, 82, Color.White, true);

            graphics.DrawText(graphics.Width - x_padding - 10, 25, today.ToString("MM/dd/yy"), Color.Black, alignmentH: HorizontalAlignment.Right);
            graphics.DrawText(graphics.Width - x_padding, 60, today.ToString("hh:mm"), Color.Black, ScaleFactor.X2, alignmentH: HorizontalAlignment.Right);

            graphics.Show();
        }

        public void UpdateDisplay(WeatherViewModel model)
        {
            graphics.Clear(Color.White);

            DisplayJPG(model.WeatherCode, x_padding, 15);

            graphics.DrawText(graphics.Width - x_padding - 5, 140, "Outdoor", Color.Black, alignmentH: HorizontalAlignment.Right);
            graphics.DrawText(graphics.Width - x_padding, 175, $"{model.OutdoorTemperature}°C", Color.Black, ScaleFactor.X2, alignmentH: HorizontalAlignment.Right);

            graphics.DrawText(x_padding + 10, 140, "Indoor", Color.Black);
            graphics.DrawText(x_padding, 175, $"{model.IndoorTemperature}°C", Color.Black, ScaleFactor.X2);

            graphics.Show();
        }

        void DisplayJPG(int weatherCode, int xOffset, int yOffset)
        {
            var jpgData = LoadResource(weatherCode);
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int x = 0;
            int y = 0;
            byte r, g, b;

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                graphics.DrawPixel(x + xOffset, y + yOffset, Color.FromRgb(r, g, b));

                x++;
                if (x % decoder.Width == 0)
                {
                    y++;
                    x = 0;
                }
            }
        }

        byte[] LoadResource(int weatherCode)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName;

            switch (weatherCode)
            {
                case int n when (n >= WeatherConstants.THUNDERSTORM_LIGHT_RAIN && n <= WeatherConstants.THUNDERSTORM_HEAVY_DRIZZLE):
                    resourceName = $"WifiWeather.w_storm.jpg";
                    break;
                case int n when (n >= WeatherConstants.DRIZZLE_LIGHT && n <= WeatherConstants.DRIZZLE_SHOWER):
                    resourceName = $"WifiWeather.w_drizzle.jpg";
                    break;
                case int n when (n >= WeatherConstants.RAIN_LIGHT && n <= WeatherConstants.RAIN_SHOWER_RAGGED):
                    resourceName = $"WifiWeather.w_rain.jpg";
                    break;
                case int n when (n >= WeatherConstants.SNOW_LIGHT && n <= WeatherConstants.SNOW_SHOWER_HEAVY):
                    resourceName = $"WifiWeather.w_snow.jpg";
                    break;
                case WeatherConstants.CLOUDS_CLEAR:
                    resourceName = $"WifiWeather.w_clear.jpg";
                    break;
                case int n when (n >= WeatherConstants.CLOUDS_FEW && n <= WeatherConstants.CLOUDS_OVERCAST):
                    resourceName = $"WifiWeather.w_cloudy.jpg";
                    break;
                default:
                    resourceName = $"WifiWeather.w_misc.jpg";
                    break;
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}