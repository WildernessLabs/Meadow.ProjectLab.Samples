using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using System;

namespace WifiWeather.Controllers
{
    internal class DisplayController
    {
        private Color backgroundColor = Color.FromHex("10485E");
        private Color selectedColor = Color.FromHex("C9DB31");
        private Color ForegroundColor = Color.FromHex("EEEEEE");
        private Font8x16 font8x16 = new Font8x16();
        private Font6x8 font6x8 = new Font6x8();

        private int margin = 5;
        readonly int smallMargin = 3;
        readonly int graphHeight = 105;

        readonly int measureBoxWidth = 82;

        readonly int columnWidth = 100;

        readonly int rowHeight = 30;
        readonly int row1 = 135;
        readonly int row2 = 170;
        readonly int row3 = 205;

        Image weatherIcon = Image.LoadFromResource("WifiWeather.Resources.w_misc.bmp");

        public LineChartSeries LineChartSeries { get; set; }
        protected DisplayScreen DisplayScreen { get; set; }
        protected AbsoluteLayout SplashLayout { get; set; }
        protected AbsoluteLayout DataLayout { get; set; }
        protected LineChart LineChart { get; set; }
        protected Picture WifiStatus { get; set; }
        protected Picture SyncStatus { get; set; }
        protected Picture Weather { get; set; }
        protected Label Status { get; set; }

        protected Box TemperatureBox { get; set; }
        protected Label TemperatureLabel { get; set; }
        protected Label TemperatureValue { get; set; }

        protected Box PressureBox { get; set; }
        protected Label PressureLabel { get; set; }
        protected Label PressureValue { get; set; }

        protected Box HumidityBox { get; set; }
        protected Label HumidityLabel { get; set; }
        protected Label HumidityValue { get; set; }

        protected Label FeelsLike { get; set; }
        protected Label Sunrise { get; set; }
        protected Label Sunset { get; set; }

        public DisplayController(IGraphicsDisplay display)
        {
            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees)
            {
                BackgroundColor = backgroundColor
            };

            LoadSplashLayout();

            DisplayScreen.Controls.Add(SplashLayout);

            LoadDataLayout();

            DisplayScreen.Controls.Add(DataLayout);
        }

        private void LoadSplashLayout()
        {
            SplashLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
            {
                Visible = false
            };

            var image = Image.LoadFromResource("WifiWeather.Resources.img_meadow.bmp");
            var displayImage = new Picture(0, 0, DisplayScreen.Width, DisplayScreen.Height, image)
            {
                BackColor = Color.FromHex("#14607F"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            SplashLayout.Controls.Add(displayImage);
        }

        private void LoadDataLayout()
        {
            DataLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
            {
                BackgroundColor = backgroundColor,
                Visible = false
            };

            Status = new Label(
                margin,
                margin + 2,
                DisplayScreen.Width / 2,
                font8x16.Height)
            {
                Text = $"Project Lab v3",
                TextColor = Color.White,
                Font = font8x16,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(Status);

            var wifiImage = Image.LoadFromResource("WifiWeather.Resources.img_wifi_connecting.bmp");
            WifiStatus = new Picture(
                DisplayScreen.Width - wifiImage.Width - margin,
                margin,
                wifiImage.Width,
                font8x16.Height,
                wifiImage)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(WifiStatus);

            var syncImage = Image.LoadFromResource("WifiWeather.Resources.img_refreshed.bmp");
            SyncStatus = new Picture(
                DisplayScreen.Width - syncImage.Width - wifiImage.Width - margin * 2,
                margin,
                syncImage.Width,
                font8x16.Height,
                syncImage)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(SyncStatus);

            DataLayout.Controls.Add(new Box(
                5, 25, 310, 105)
            {
                ForeColor = Color.FromHex("082936"),
            });

            var weatherImage = Image.LoadFromResource("WifiWeather.Resources.w_misc.bmp");
            Weather = new Picture(
                margin,
                row1,
                100,
                100,
                weatherImage)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(Weather);

            #region TEMPERATURE
            TemperatureBox = new Box(
                columnWidth + margin * 2,
                row1,
                columnWidth,
                rowHeight)
            {
                ForeColor = selectedColor
            };
            DataLayout.Controls.Add(TemperatureBox);
            TemperatureLabel = new Label(
                columnWidth + margin * 2 + smallMargin,
                row1 + smallMargin,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height)
            {
                Text = $"TEMPERATURE",
                TextColor = backgroundColor,
                Font = font6x8
            };
            DataLayout.Controls.Add(TemperatureLabel);
            TemperatureValue = new Label(
                columnWidth + margin * 2 + smallMargin,
                row1 + font6x8.Height + smallMargin * 2,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"-.-C",
                TextColor = backgroundColor,
                Font = font6x8,
                ScaleFactor = ScaleFactor.X2
            };
            DataLayout.Controls.Add(TemperatureValue);
            #endregion

            #region PRESSURE
            PressureBox = new Box(
                columnWidth + margin * 2,
                row2,
                columnWidth,
                rowHeight)
            {
                ForeColor = selectedColor
            };
            DataLayout.Controls.Add(PressureBox);
            PressureLabel = new Label(
                columnWidth + margin * 2 + smallMargin,
                row2 + smallMargin,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height)
            {
                Text = $"PRESSURE",
                TextColor = backgroundColor,
                Font = font6x8
            };
            DataLayout.Controls.Add(PressureLabel);
            PressureValue = new Label(
                columnWidth + margin * 2 + smallMargin,
                row2 + font6x8.Height + smallMargin * 2,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"-.-hPa",
                TextColor = backgroundColor,
                Font = font6x8,
                ScaleFactor = ScaleFactor.X2
            };
            DataLayout.Controls.Add(PressureValue);
            #endregion

            #region HUMIDITY
            HumidityBox = new Box(
                columnWidth + margin * 2,
                row3,
                columnWidth,
                rowHeight)
            {
                ForeColor = selectedColor
            };
            DataLayout.Controls.Add(HumidityBox);
            HumidityLabel = new Label(
                columnWidth + margin * 2 + smallMargin,
                row3 + smallMargin,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height)
            {
                Text = $"HUMIDITY",
                TextColor = backgroundColor,
                Font = font6x8
            };
            DataLayout.Controls.Add(HumidityLabel);
            HumidityValue = new Label(
                columnWidth + margin * 2 + smallMargin,
                row3 + font6x8.Height + smallMargin * 2,
                columnWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"-.-%",
                TextColor = backgroundColor,
                Font = font6x8,
                ScaleFactor = ScaleFactor.X2
            };
            DataLayout.Controls.Add(HumidityValue);
            #endregion

            DataLayout.Controls.Add(new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row1 + smallMargin,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height)
            {
                Text = $"FEELS LIKE",
                TextColor = ForegroundColor,
                Font = font6x8
            });
            FeelsLike = new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row1 + font6x8.Height + smallMargin * 2,
                columnWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"-.-C",
                TextColor = ForegroundColor,
                Font = font6x8,
                ScaleFactor = ScaleFactor.X2
            };
            DataLayout.Controls.Add(FeelsLike);

            DataLayout.Controls.Add(new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row2 + smallMargin,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height)
            {
                Text = $"SUNRISE",
                TextColor = ForegroundColor,
                Font = font6x8
            });
            Sunrise = new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row2 + font6x8.Height + smallMargin * 2,
                columnWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"--:-- --",
                TextColor = ForegroundColor,
                Font = font6x8,
                ScaleFactor = ScaleFactor.X2
            };
            DataLayout.Controls.Add(Sunrise);

            DataLayout.Controls.Add(new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row3 + smallMargin,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height)
            {
                Text = $"SUNSET",
                TextColor = ForegroundColor,
                Font = font6x8
            });
            Sunset = new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row3 + font6x8.Height + smallMargin * 2,
                columnWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"--:-- --",
                TextColor = ForegroundColor,
                Font = font6x8,
                ScaleFactor = ScaleFactor.X2
            };
            DataLayout.Controls.Add(Sunset);
        }

        public void ShowSplashScreen()
        {
            DataLayout.Visible = false;
            SplashLayout.Visible = true;
        }

        public void ShowDataScreen()
        {
            SplashLayout.Visible = false;
            DataLayout.Visible = true;
        }

        public void UpdateStatus(string status)
        {
            Status.Text = status;
        }

        public void UpdateWiFiStatus(bool isConnected)
        {
            var imageWiFi = isConnected
                ? Image.LoadFromResource("WifiWeather.Resources.img_wifi_connected.bmp")
                : Image.LoadFromResource("WifiWeather.Resources.img_wifi_connecting.bmp");
            WifiStatus.Image = imageWiFi;
        }

        public void UpdateSyncStatus(bool isSyncing)
        {
            var imageSync = isSyncing
                ? Image.LoadFromResource("WifiWeather.Resources.img_refreshing.bmp")
                : Image.LoadFromResource("WifiWeather.Resources.img_refreshed.bmp");
            SyncStatus.Image = imageSync;
        }

        public void UpdateWeatherIcon(string icon)
        {
            weatherIcon = Image.LoadFromResource(icon);
            Weather.Image = weatherIcon;
        }

        public void UpdateReadings(
            double temperature,
            double pressure,
            double humidity,
            double feelsLike,
            DateTime sunrise,
            DateTime sunset)
        {
            DisplayScreen.BeginUpdate();

            TemperatureValue.Text = $"{temperature:N1}C";
            PressureValue.Text = $"{pressure:N1}hPa";
            HumidityValue.Text = $"{humidity:N0}%";
            FeelsLike.Text = $"{feelsLike:N1}C";
            Sunrise.Text = $"{sunrise:hh:mm tt}";
            Sunset.Text = $"{sunset:hh:mm tt}";

            DisplayScreen.EndUpdate();
        }
    }
}