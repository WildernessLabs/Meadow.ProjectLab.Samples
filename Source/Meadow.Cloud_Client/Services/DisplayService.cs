using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using System;

namespace Meadow.Cloud_Client.Services
{
    internal class DisplayService
    {
        const int PointsPerSeries = 50;
        int rowHeight = 40;
        int margin = 15;

        protected DisplayScreen DisplayScreen { get; set; }

        protected AbsoluteLayout SplashLayout { get; set; }

        protected AbsoluteLayout DataLayout { get; set; }

        protected Picture WifiStatus { get; set; }

        protected Picture SyncStatus { get; set; }

        protected Label Status { get; set; }

        Color backgroundColor = Color.FromHex("#14607F");
        Color foregroundColor = Color.White;

        Font12x20 font12X20 = new Font12x20();

        public DisplayService(IGraphicsDisplay display)
        {
            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees)
            {
                BackgroundColor = backgroundColor
            };

            LoadSplashLayout();

            LoadDataLayout();

            DisplayScreen.Controls.Add(SplashLayout, DataLayout);
        }

        void LoadSplashLayout()
        {
            SplashLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
            {
                Visible = false
            };

            var image = Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_meadow.bmp");
            var displayImage = new Picture(0, 0, DisplayScreen.Width, DisplayScreen.Height, image)
            {
                BackColor = Color.FromHex("#14607F"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            SplashLayout.Controls.Add(displayImage);
        }

        void LoadDataLayout()
        {
            DataLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
            {
                BackgroundColor = Color.FromHex("#14607F"),
                Visible = false
            };

            DataLayout.Controls.Add(new Box(0, 0, DisplayScreen.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#10485E")
            });

            Status = new Label(margin, 0, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"-",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            DataLayout.Controls.Add(Status);

            var wifiImage = Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_wifi_connecting.bmp");
            WifiStatus = new Picture(DisplayScreen.Width - wifiImage.Width - margin, 0, wifiImage.Width, rowHeight, wifiImage)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(WifiStatus);

            var syncImage = Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_refreshed.bmp");
            SyncStatus = new Picture(DisplayScreen.Width - syncImage.Width - wifiImage.Width - margin * 2, 0, syncImage.Width, rowHeight, syncImage)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(SyncStatus);

            var TemperatureLineChart = new LineChart(margin * 2, rowHeight + margin, DisplayScreen.Width - margin * 4, DisplayScreen.Height - rowHeight - margin * 2)
            {
                BackgroundColor = Color.FromHex("#14607F"),
                AxisColor = foregroundColor,
                ShowYAxisLabels = true
            };
            TemperatureLineChart.Series.Add(GetTemperatureData());
            DataLayout.Controls.Add(TemperatureLineChart);
        }

        private LineChartSeries GetTemperatureData(double xScale = 4, double yScale = 1.5, double yOffset = 1.5)
        {
            var series = new LineChartSeries()
            {

                LineColor = Color.LightBlue,
                PointColor = Color.Cyan,
                LineStroke = 1,
                PointSize = 2,
                ShowLines = true,
                ShowPoints = true,
            };

            var rand = new Random();

            for (var p = 0; p < PointsPerSeries; p++)
            {
                series.Points.Add(p * 2, rand.Next(25, 35));
            }

            return series;
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
                ? Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_wifi_connected.bmp")
                : Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_wifi_connecting.bmp");
            WifiStatus.Image = imageWiFi;
        }

        public void UpdateSyncStatus(bool isSyncing)
        {
            var imageSync = isSyncing
                ? Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_refreshing.bmp")
                : Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_refreshed.bmp");
            SyncStatus.Image = imageSync;
        }
    }
}