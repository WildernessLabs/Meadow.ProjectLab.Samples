using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

namespace Meadow.Cloud_Command.Controllers
{
    internal class DisplayController
    {
        private readonly int rowHeight = 60;
        private readonly int graphHeight = 115;
        private readonly int axisLabelsHeight = 15;
        private readonly int margin = 15;

        private Color backgroundColor = Color.FromHex("14607F");
        private Color foregroundColor = Color.FromHex("10485E");
        private Color chartCurveColor = Color.FromHex("EF7D3B");
        private Color TextColor = Color.White;

        private Font12x20 font12X20 = new Font12x20();
        private Font8x12 font8x12 = new Font8x12();
        private Font6x8 font6x8 = new Font6x8();

        protected DisplayScreen DisplayScreen { get; set; }

        protected AbsoluteLayout SplashLayout { get; set; }

        protected AbsoluteLayout DataLayout { get; set; }

        protected LineChartSeries LineChartSeries { get; set; }

        protected LineChart LineChart { get; set; }

        protected Picture WifiStatus { get; set; }

        protected Picture SyncStatus { get; set; }

        protected Label Status { get; set; }

        protected Label LatestReading { get; set; }

        protected Label AxisLabels { get; set; }

        protected Label Temperature { get; set; }

        protected Label Pressure { get; set; }

        protected Label Humidity { get; set; }

        protected Label ConnectionErrorLabel { get; set; }

        public DisplayController(IGraphicsDisplay display)
        {
            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees)
            {
                BackgroundColor = backgroundColor
            };

            //LoadSplashLayout();

            LoadDataLayout();

            DisplayScreen.Controls.Add(SplashLayout, DataLayout);
        }

        private void LoadSplashLayout()
        {
            SplashLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
            {
                Visible = false
            };

            var image = Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_meadow.bmp");
            var displayImage = new Picture(0, 0, DisplayScreen.Width, DisplayScreen.Height, image)
            {
                BackColor = Meadow.Foundation.Color.FromHex("14607F"),
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

            DataLayout.Controls.Add(new Box(
                0,
                0,
                DisplayScreen.Width,
                DisplayScreen.Height)
            {
                ForeColor = Meadow.Foundation.Color.Red,
                Filled = false
            });


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

        public void UpdateLatestReading(string latestUpdate)
        {
            LatestReading.Text = $"Latest reading: {latestUpdate}";
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