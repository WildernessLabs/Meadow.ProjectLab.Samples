using Meadow.Cloud_Client.Controllers;
using Meadow.Cloud_Client.Hardware;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Cloud_Client;

internal class MainController
{
    private IMeadowCloudClientHardware hardware;
    private IWiFiNetworkAdapter network;
    private CloudController cloudController;
    private DisplayController displayController;

    private int currentGraphType = 0;

    private List<double> temperatureReadings = new List<double>();
    private List<double> pressureReadings = new List<double>();
    private List<double> humidityReadings = new List<double>();

    public MainController(IMeadowCloudClientHardware hardware, IWiFiNetworkAdapter network)
    {
        this.hardware = hardware;
        this.network = network;
    }

    public void Initialize()
    {
        hardware.Initialize();

        hardware.RightButton.Clicked += RightButtonClicked;

        hardware.LeftButton.Clicked += LeftButtonClicked;

        cloudController = new CloudController();
        displayController = new DisplayController(hardware.Display);

        displayController.ShowSplashScreen();
        Thread.Sleep(3000);
        displayController.ShowDataScreen();
    }

    private void RightButtonClicked(object sender, EventArgs e)
    {
        currentGraphType = currentGraphType + 1 > 2 ? 0 : currentGraphType + 1;

        UpdateGraph();
    }

    private void LeftButtonClicked(object sender, EventArgs e)
    {
        currentGraphType = currentGraphType - 1 < 0 ? 2 : currentGraphType - 1;

        UpdateGraph();
    }

    private void UpdateGraph()
    {
        switch (currentGraphType)
        {
            case 0:
                displayController.UpdateGraph(currentGraphType, temperatureReadings);
                break;
            case 1:
                displayController.UpdateGraph(currentGraphType, pressureReadings);
                break;
            case 2:
                displayController.UpdateGraph(currentGraphType, humidityReadings);
                break;
        }
    }

    public async Task Run()
    {
        int TIMEZONE_OFFSET = -8; // UTC-8

        while (true)
        {
            displayController.UpdateWiFiStatus(network.IsConnected);

            if (network.IsConnected)
            {
                displayController.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("dd/MM/yy hh:mm tt"));
                displayController.UpdateSyncStatus(true);

                var readings = await cloudController.GetSensorReadings();

                if (readings != null && readings.Count > 0)
                {
                    temperatureReadings.Clear();
                    pressureReadings.Clear();
                    humidityReadings.Clear();


                    // Massage data to get a record per hour
                    var formattedData = readings.GroupBy(item => new { item.record.timestamp.Year, item.record.timestamp.Month, item.record.timestamp.Day, item.record.timestamp.Hour })
                        .Select(group => group.First())
                        .Reverse()
                        .ToList();

                    Resolver.Log.Info($"========================================================================================");

                    foreach (var reading in formattedData)
                    {
                        Resolver.Log.Info($"Record: {reading.record.timestamp} | T: {reading.record.measurements.temperature}");

                        temperatureReadings.Add(double.Parse(reading.record.measurements.temperature));
                        pressureReadings.Add(double.Parse(reading.record.measurements.pressure));
                        humidityReadings.Add(double.Parse(reading.record.measurements.humidity));
                    }

                    UpdateGraph();
                }

                displayController.UpdateSyncStatus(false);

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
            else
            {
                displayController.UpdateStatus("Offline...");
                displayController.UpdateSyncStatus(true);

                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }
}