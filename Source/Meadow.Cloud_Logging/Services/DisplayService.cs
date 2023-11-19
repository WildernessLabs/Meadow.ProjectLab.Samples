using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

namespace Meadow.Cloud_Logging.Services
{
    internal class DisplayService
    {
        int rowHeight = 60;
        int rowMargin = 15;

        protected DisplayScreen DisplayScreen { get; set; }

        protected AbsoluteLayout SplashLayout { get; set; }

        protected AbsoluteLayout DataLayout { get; set; }

        protected Picture WifiStatus { get; set; }

        protected Picture SyncStatus { get; set; }

        protected Label Status { get; set; }

        protected Label Pressure { get; set; }

        protected Label Humidity { get; set; }

        protected Label Temperature { get; set; }

        Color backgroundColor = Color.FromHex("#F3F7FA");
        Color foregroundColor = Color.Black;

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

            var image = Image.LoadFromResource("Meadow.Cloud_Logging.Resources.img_meadow.bmp");
            var displayImage = new Picture(0, 0, DisplayScreen.Width, DisplayScreen.Height, image)
            {
                BackColor = Color.FromHex("#C9DB31"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            SplashLayout.Controls.Add(displayImage);
        }

        void LoadDataLayout()
        {
            DataLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
            {
                Visible = false
            };

            DataLayout.Controls.Add(new Box(0, 0, DisplayScreen.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#CADC32")
            });
            DataLayout.Controls.Add(new Box(0, rowHeight, DisplayScreen.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#D3E255")
            });
            DataLayout.Controls.Add(new Box(0, rowHeight * 2, DisplayScreen.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#DCE878")
            });
            DataLayout.Controls.Add(new Box(0, rowHeight * 3, DisplayScreen.Width, rowHeight)
            {
                ForeColor = Color.FromHex("#E5EE9B")
            });

            var wifiImage = Image.LoadFromResource("Meadow.Cloud_Logging.Resources.img_wifi_connecting.bmp");
            WifiStatus = new Picture(DisplayScreen.Width - wifiImage.Width - rowMargin, 0, wifiImage.Width, rowHeight, wifiImage)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(WifiStatus);

            var syncImage = Image.LoadFromResource("Meadow.Cloud_Logging.Resources.img_refreshed.bmp");
            SyncStatus = new Picture(DisplayScreen.Width - syncImage.Width - wifiImage.Width - 10 - rowMargin, 0, syncImage.Width, rowHeight, syncImage)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            DataLayout.Controls.Add(SyncStatus);

            DataLayout.Controls.Add(new Label(rowMargin, rowHeight, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"PRESSURE",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });
            DataLayout.Controls.Add(new Label(rowMargin, rowHeight * 2, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"HUMIDITY",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });
            DataLayout.Controls.Add(new Label(rowMargin, rowHeight * 3, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"TEMPERATURE",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });

            Status = new Label(rowMargin, 0, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"-",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            DataLayout.Controls.Add(Status);

            Pressure = new Label(DisplayScreen.Width / 2 - rowMargin, rowHeight, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"- Mb",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DataLayout.Controls.Add(Pressure);

            Humidity = new Label(DisplayScreen.Width / 2 - rowMargin, rowHeight * 2, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"- % ",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DataLayout.Controls.Add(Humidity);

            Temperature = new Label(DisplayScreen.Width / 2 - rowMargin, rowHeight * 3, DisplayScreen.Width / 2, rowHeight)
            {
                Text = $"- °C",
                TextColor = foregroundColor,
                Font = font12X20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            DataLayout.Controls.Add(Temperature);
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
                ? Image.LoadFromResource("Meadow.Cloud_Logging.Resources.img_wifi_connected.bmp")
                : Image.LoadFromResource("Meadow.Cloud_Logging.Resources.img_wifi_connecting.bmp");
            WifiStatus.Image = imageWiFi;
        }

        public void UpdateSyncStatus(bool isSyncing)
        {
            var imageSync = isSyncing
                ? Image.LoadFromResource("Meadow.Cloud_Logging.Resources.img_refreshing.bmp")
                : Image.LoadFromResource("Meadow.Cloud_Logging.Resources.img_refreshed.bmp");
            SyncStatus.Image = imageSync;
        }

        public void UpdateAtmosphericConditions(string pressure, string humidity, string temperature)
        {
            DisplayScreen.BeginUpdate();

            Pressure.Text = $"{pressure} mb";
            Humidity.Text = $"{humidity} % ";
            Temperature.Text = $"{temperature} °C";

            DisplayScreen.EndUpdate();
        }
    }
}