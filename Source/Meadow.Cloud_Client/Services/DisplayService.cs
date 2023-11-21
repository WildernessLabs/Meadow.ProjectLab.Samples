using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using System.Collections.Generic;

namespace Meadow.Cloud_Client.Services;

internal class DisplayService
{
    private readonly int rowHeight = 40;
    private readonly int graphHeight = 150;
    private readonly int margin = 15;

    protected DisplayScreen DisplayScreen { get; set; }

    protected AbsoluteLayout SplashLayout { get; set; }

    protected AbsoluteLayout DataLayout { get; set; }

    public LineChartSeries LineChartSeries { get; set; }

    protected LineChart LineChart { get; set; }

    protected Picture WifiStatus { get; set; }

    protected Picture SyncStatus { get; set; }

    protected Label Status { get; set; }

    protected Label Temperature { get; set; }

    protected Label Pressure { get; set; }

    protected Label Humidity { get; set; }

    protected Label ConnectionErrorLabel { get; set; }

    private Meadow.Foundation.Color backgroundColor = Meadow.Foundation.Color.FromHex("#14607F");
    private Meadow.Foundation.Color foregroundColor = Meadow.Foundation.Color.White;
    private Font12x20 font12X20 = new Font12x20();

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

    private void LoadSplashLayout()
    {
        SplashLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
        {
            Visible = false
        };

        var image = Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_meadow.bmp");
        var displayImage = new Picture(0, 0, DisplayScreen.Width, DisplayScreen.Height, image)
        {
            BackColor = Meadow.Foundation.Color.FromHex("#14607F"),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        SplashLayout.Controls.Add(displayImage);
    }

    private void LoadDataLayout()
    {
        DataLayout = new AbsoluteLayout(DisplayScreen, 0, 0, DisplayScreen.Width, DisplayScreen.Height)
        {
            BackgroundColor = Meadow.Foundation.Color.FromHex("#14607F"),
            Visible = false
        };

        DataLayout.Controls.Add(new Box(0, 0, DisplayScreen.Width, rowHeight)
        {
            ForeColor = Meadow.Foundation.Color.FromHex("#10485E")
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

        LineChart = new LineChart(
            margin,
            rowHeight + margin,
            DisplayScreen.Width - margin * 2,
            graphHeight)
        {
            BackgroundColor = Meadow.Foundation.Color.FromHex("#10485E"),
            AxisColor = foregroundColor,
            ShowYAxisLabels = true,
            Visible = false,
            AlwaysShowYOrigin = false,
        };
        LineChartSeries = new LineChartSeries()
        {
            LineColor = Meadow.Foundation.Color.FromHex("EF7D3B"),
            PointColor = Meadow.Foundation.Color.FromHex("EF7D3B"),
            LineStroke = 1,
            PointSize = 2,
            ShowLines = true,
            ShowPoints = true,
        };
        LineChart.Series.Add(LineChartSeries);
        DataLayout.Controls.Add(LineChart);

        var labelFont = new Font8x12();

        Temperature = new Label(15, 205, 115, 20)
        {
            Text = $"TEMPERATURE",
            TextColor = foregroundColor,
            BackColor = Meadow.Foundation.Color.FromHex("#10485E"),
            Font = labelFont,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        DataLayout.Controls.Add(Temperature);

        Pressure = new Label(130, 205, 89, 20)
        {
            Text = $"PRESSURE",
            TextColor = foregroundColor,
            BackColor = Meadow.Foundation.Color.FromHex("#14607F"),
            Font = labelFont,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        DataLayout.Controls.Add(Pressure);

        Humidity = new Label(219, 205, 86, 20)
        {
            Text = $"HUMIDITY",
            TextColor = foregroundColor,
            BackColor = Meadow.Foundation.Color.FromHex("#14607F"),
            Font = labelFont,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        DataLayout.Controls.Add(Humidity);

        ConnectionErrorLabel = new Label(
            (int)(DisplayScreen.Width * 0.25),
            DisplayScreen.Height / 2,
            (int)(DisplayScreen.Width * 0.60),
            20)
        {
            Text = "NO NETWORK CONNECTION",
            TextColor = foregroundColor,
            BackColor = Meadow.Foundation.Color.FromHex("#10485E"),
            Font = labelFont,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        DataLayout.Controls.Add(ConnectionErrorLabel);
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

    public void UpdateGraph(int currentGraphType, List<double> readings)
    {
        Temperature.BackColor = Meadow.Foundation.Color.FromHex("14607F");
        Pressure.BackColor = Meadow.Foundation.Color.FromHex("14607F");
        Humidity.BackColor = Meadow.Foundation.Color.FromHex("14607F");

        switch (currentGraphType)
        {
            case 0:
                Temperature.BackColor = Meadow.Foundation.Color.FromHex("10485E");
                break;
            case 1:
                Pressure.BackColor = Meadow.Foundation.Color.FromHex("10485E");
                break;
            case 2:
                Humidity.BackColor = Meadow.Foundation.Color.FromHex("10485E");
                break;
        }

        LineChartSeries.Points.Clear();

        for (var p = 0; p < readings.Count; p++)
        {
            LineChartSeries.Points.Add(p * 2, readings[p]);
        }
    }

    public void UpdateWiFiStatus(bool isConnected)
    {
        var imageWiFi = isConnected
            ? Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_wifi_connected.bmp")
            : Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_wifi_connecting.bmp");
        WifiStatus.Image = imageWiFi;

        // make it clear that if there's no data, it's because we have no connection
        if (!isConnected && LineChartSeries.Points.Count == 0)
        {
            ConnectionErrorLabel.Visible = true;
        }
        else
        {
            ConnectionErrorLabel.Visible = false;
        }
    }

    public void UpdateSyncStatus(bool isSyncing)
    {
        var imageSync = isSyncing
            ? Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_refreshing.bmp")
            : Image.LoadFromResource("Meadow.Cloud_Client.Resources.img_refreshed.bmp");
        SyncStatus.Image = imageSync;
    }
}