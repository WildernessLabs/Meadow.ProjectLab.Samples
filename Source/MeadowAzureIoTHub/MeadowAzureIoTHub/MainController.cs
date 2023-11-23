using Meadow;
using Meadow.Foundation;
using Meadow.Hardware;
using Meadow.Units;
using MeadowAzureIoTHub.Controllers;
using MeadowAzureIoTHub.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowAzureIoTHub
{
    internal class MainController
    {
        int TIMEZONE_OFFSET = -8; // UTC-8

        IMeadowAzureIoTHubHardware hardware;
        IWiFiNetworkAdapter network;
        DisplayController displayController;
        IoTHubController iotHubService;

        public MainController(IMeadowAzureIoTHubHardware hardware, IWiFiNetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public async Task Initialize()
        {
            hardware.Initialize();

            displayController = new DisplayController(hardware.Display);
            displayController.ShowSplashScreen();
            Thread.Sleep(3000);
            displayController.ShowDataScreen();

            iotHubService = new IoTHubController();
            await InitializeIoTHub();

            hardware.EnvironmentalSensor.Updated += EnvironmentalSensorUpdated;
        }

        private async Task InitializeIoTHub()
        {
            while (!network.IsConnected || !iotHubService.isAuthenticated)
            {
                displayController.UpdateStatus("Authenticating...");

                bool authenticated = await iotHubService.Initialize();

                if (authenticated)
                {
                    displayController.UpdateStatus("Authenticated");
                    await Task.Delay(2000);
                    displayController.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("hh:mm tt dd/MM/yy"));
                }
                else
                {
                    displayController.UpdateStatus("Not Authenticated");
                }
            }
        }

        private async Task SendDataToIoTHub((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) data)
        {
            displayController.UpdateSyncStatus(true);
            displayController.UpdateStatus("Sending data...");

            await iotHubService.SendEnvironmentalReading(data);

            displayController.UpdateSyncStatus(false);
            displayController.UpdateStatus("Data sent!");
            Thread.Sleep(2000);
            displayController.UpdateLastUpdated(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("hh:mm tt dd/MM/yy"));

            displayController.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("hh:mm tt dd/MM/yy"));
        }

        private async void EnvironmentalSensorUpdated(object sender, Meadow.IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            Resolver.Log.Trace("EnvironmentalSensorUpdated...");

            hardware.RgbPwmLed.StartBlink(Color.Orange);

            displayController.UpdateAtmosphericConditions(
                temperature: $"{e.New.Temperature.Value.Celsius:N0}",
                pressure: $"{e.New.Pressure.Value.Millibar:N0}",
                humidity: $"{e.New.Humidity.Value.Percent:N0}");

            if (network.IsConnected && iotHubService.isAuthenticated)
            {
                await SendDataToIoTHub(e.New);
            }

            hardware.RgbPwmLed.StartBlink(Color.Green);
        }

        public async Task Run()
        {
            hardware.EnvironmentalSensor.StartUpdating(TimeSpan.FromMinutes(5));

            while (true)
            {
                displayController.UpdateWiFiStatus(network.IsConnected);

                if (network.IsConnected)
                {
                    displayController.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("hh:mm tt dd/MM/yy"));

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                else
                {
                    displayController.UpdateStatus("Offline...");

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}