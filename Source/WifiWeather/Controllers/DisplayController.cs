using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using System;
using System.Collections.Generic;

namespace WifiWeather.Controllers
{
    internal class DisplayController
    {
        private Color backgroundColor = Color.FromHex("10485E");
        private Color outdoorColor = Color.FromHex("C9DB31");
        private Color foregroundColor = Color.FromHex("EEEEEE");
        private Font8x16 font8x16 = new Font8x16();
        private Font6x8 font6x8 = new Font6x8();

        private int counter = 0;
        private int margin = 5;
        readonly int smallMargin = 3;
        readonly int graphHeight = 105;

        readonly int measureBoxWidth = 82;

        readonly int columnWidth = 100;

        readonly int rowHeight = 30;
        readonly int row1 = 135;
        readonly int row2 = 170;
        readonly int row3 = 205;

        Image weatherIcon = Image.LoadFromResource($"WifiWeather.Resources.w_misc.bmp");

        public LineChartSeries OutdoorSeries { get; set; }
        protected DisplayScreen DisplayScreen { get; set; }
        protected AbsoluteLayout SplashLayout { get; set; }
        protected AbsoluteLayout DataLayout { get; set; }
        protected LineChart LineChart { get; set; }
        protected Picture WifiStatus { get; set; }
        protected Picture SyncStatus { get; set; }
        protected Picture Weather { get; set; }
        protected Label Status { get; set; }
        protected Label Counter { get; set; }

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
                152,
                font8x16.Height)
            {
                Text = $"Project Lab v3",
                TextColor = Color.White,
                Font = font8x16,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(Status);

             Counter = new Label(
                228,
                margin + 2,
                44,
                14)
            {
                Text = $"00000",
                TextColor = backgroundColor,
                BackColor = Color.FromHex("EF7D3B"),
                Font = font6x8,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(Counter);

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

            LineChart = new LineChart(
                margin,
                25,
                DisplayScreen.Width - margin * 2,
                graphHeight)
            {
                BackgroundColor = Color.FromHex("082936"),
                AxisColor = foregroundColor,
                ShowYAxisLabels = true,
                Visible = false,
                AlwaysShowYOrigin = false,
            };
            OutdoorSeries = new LineChartSeries()
            {
                LineColor = outdoorColor,
                PointColor = outdoorColor,
                LineStroke = 1,
                PointSize = 2,
                ShowLines = true,
                ShowPoints = true,
            };
            LineChart.Series.Add(OutdoorSeries);
            DataLayout.Controls.Add(LineChart);

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
                ForeColor = outdoorColor
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
                ForeColor = backgroundColor
            };
            DataLayout.Controls.Add(PressureBox);
            PressureLabel = new Label(
                columnWidth + margin * 2 + smallMargin,
                row2 + smallMargin,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height)
            {
                Text = $"PRESSURE",
                TextColor = foregroundColor,
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
                TextColor = foregroundColor,
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
                ForeColor = backgroundColor
            };
            DataLayout.Controls.Add(HumidityBox);
            HumidityLabel = new Label(
                columnWidth + margin * 2 + smallMargin,
                row3 + smallMargin,
                measureBoxWidth - smallMargin * 2,
                font6x8.Height)
            {
                Text = $"HUMIDITY",
                TextColor = foregroundColor,
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
                TextColor = foregroundColor,
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
                TextColor = foregroundColor,
                Font = font6x8
            });
            FeelsLike = new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row1 + font6x8.Height + smallMargin * 2,
                columnWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"-.-C",
                TextColor = foregroundColor,
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
                TextColor = foregroundColor,
                Font = font6x8
            });
            Sunrise = new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row2 + font6x8.Height + smallMargin * 2,
                columnWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"--:-- --",
                TextColor = foregroundColor,
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
                TextColor = foregroundColor,
                Font = font6x8
            });
            Sunset = new Label(
                columnWidth * 2 + margin * 3 + smallMargin,
                row3 + font6x8.Height + smallMargin * 2,
                columnWidth - smallMargin * 2,
                font6x8.Height * 2)
            {
                Text = $"--:-- --",
                TextColor = foregroundColor,
                Font = font6x8,
                ScaleFactor = ScaleFactor.X2
            };
            DataLayout.Controls.Add(Sunset);
        }

        private void UpdateReadingType(int type)
        {
            TemperatureBox.ForeColor = PressureBox.ForeColor = HumidityBox.ForeColor = backgroundColor;
            TemperatureLabel.TextColor = PressureLabel.TextColor = HumidityLabel.TextColor = foregroundColor;
            TemperatureValue.TextColor = PressureValue.TextColor = HumidityValue.TextColor = foregroundColor;

            switch (type)
            {
                case 0:
                    TemperatureBox.ForeColor = outdoorColor;
                    TemperatureLabel.TextColor = backgroundColor;
                    TemperatureValue.TextColor = backgroundColor;
                    break;
                case 1:
                    PressureBox.ForeColor = outdoorColor;
                    PressureLabel.TextColor = backgroundColor;
                    PressureValue.TextColor = backgroundColor;
                    break;
                case 2:
                    HumidityBox.ForeColor = outdoorColor;
                    HumidityLabel.TextColor = backgroundColor;
                    HumidityValue.TextColor = backgroundColor;
                    break;
            }
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

        public void UpdateGraph(int graphType, List<double> readings)
        {
            DisplayScreen.BeginUpdate();

            UpdateReadingType(graphType);

            OutdoorSeries.Points.Clear();

            for (var p = 0; p < readings.Count; p++)
            {
                OutdoorSeries.Points.Add(p * 2, readings[p]);
            }

            DisplayScreen.EndUpdate();
        }

        public void UpdateReadings(
            int readingType,
            string icon,
            double temperature,
            double pressure,
            double humidity,
            double feelsLike,
            DateTime sunrise,
            DateTime sunset,
            List<double> outdoorReadings)
        {
            DisplayScreen.BeginUpdate();

            counter++;
            Counter.Text = $"{counter:D5}";

            UpdateReadingType(readingType);

            weatherIcon = Image.LoadFromResource(icon);
            Weather.Image = weatherIcon;

            TemperatureValue.Text = $"{temperature:N1}C";
            HumidityValue.Text = $"{humidity:N1}%";
            PressureValue.Text = $"{pressure:N2}atm";
            FeelsLike.Text = $"{feelsLike:N1}C";
            Sunrise.Text = $"{sunrise:hh:mm tt}";
            Sunset.Text = $"{sunset:hh:mm tt}";

            OutdoorSeries.Points.Clear();

            for (var p = 0; p < outdoorReadings.Count; p++)
            {
                OutdoorSeries.Points.Add(p * 2, outdoorReadings[p]);
            }

            DisplayScreen.EndUpdate();
        }
    }
}