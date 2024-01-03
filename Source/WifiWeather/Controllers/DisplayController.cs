using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using System;

namespace WifiWeather.Controllers
{
    internal class DisplayController
    {
        Image weatherIcon = Image.LoadFromResource("WifiWeather.Resources.w_misc.bmp");

        protected DisplayScreen DisplayScreen { get; set; }

        protected Picture Weather { get; set; }

        protected Label Date { get; set; }

        protected Label Time { get; set; }

        protected Label IndoorTemperature { get; set; }

        protected Label OutdoorTemperature { get; set; }

        Color backgroundColor = Color.FromHex("#F3F7FA");
        Color foregroundColor = Color.Black;

        Font12x20 font12X20 = new Font12x20();

        public DisplayController(IGraphicsDisplay display)
        {
            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees)
            {
                BackgroundColor = backgroundColor
            };

            Weather = new Picture(30, 15, 100, 100, weatherIcon);
            DisplayScreen.Controls.Add(Weather);

            Date = new Label(DisplayScreen.Width / 2, 25, DisplayScreen.Width / 2, font12X20.Height)
            {
                Text = $"--/--/--",
                TextColor = foregroundColor,
                BackColor = backgroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            DisplayScreen.Controls.Add(Date);

            Time = new Label(DisplayScreen.Width / 2, 60, DisplayScreen.Width / 2, font12X20.Height * 2)
            {
                Text = $"--:--",
                TextColor = foregroundColor,
                BackColor = backgroundColor,
                Font = font12X20,
                ScaleFactor = ScaleFactor.X2,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            DisplayScreen.Controls.Add(Time);

            DisplayScreen.Controls.Add(new Label(0, 140, DisplayScreen.Width / 2, font12X20.Height)
            {
                Text = $"Indoor",
                TextColor = foregroundColor,
                BackColor = backgroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            IndoorTemperature = new Label(0, 175, DisplayScreen.Width / 2, font12X20.Height * 2)
            {
                Text = $"-°C",
                TextColor = foregroundColor,
                BackColor = backgroundColor,
                Font = font12X20,
                ScaleFactor = ScaleFactor.X2,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            DisplayScreen.Controls.Add(IndoorTemperature);

            DisplayScreen.Controls.Add(new Label(DisplayScreen.Width / 2, 140, DisplayScreen.Width / 2, font12X20.Height)
            {
                Text = $"Outdoor",
                TextColor = foregroundColor,
                BackColor = backgroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            OutdoorTemperature = new Label(DisplayScreen.Width / 2, 175, DisplayScreen.Width / 2, font12X20.Height * 2)
            {
                Text = $"-°C",
                TextColor = foregroundColor,
                BackColor = backgroundColor,
                Font = font12X20,
                ScaleFactor = ScaleFactor.X2,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            DisplayScreen.Controls.Add(OutdoorTemperature);
        }

        public void UpdateWeatherIcon(string icon)
        {
            weatherIcon = Image.LoadFromResource(icon);
            Weather.Image = weatherIcon;
        }

        public void UpdateDateTime(DateTime today)
        {
            Date.Text = today.ToString("MM/dd/yy");
            Time.Text = today.ToString("hh:mm");
        }

        public void UpdateIndoorTemperature(int temperature)
        {
            IndoorTemperature.Text = $"{temperature}°C";
        }

        public void UpdateOutdoorTemperature(int temperature)
        {
            OutdoorTemperature.Text = $"{temperature}°C";
        }
    }
}