using Meadow.Foundation;
using Meadow.Hardware;
using Meadow.Units;
using MeadowAzureIoTHub.Hardware;
using MeadowAzureIoTHub.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowAzureIoTHub
{
    internal class MainCoordinator
    {
        IMeadowAzureIoTHubHardware hardware;
        IWiFiNetworkAdapter network;
        DisplayService displayService;
        IoTHubService iotHubService;

        public MainCoordinator(IMeadowAzureIoTHubHardware hardware, IWiFiNetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

            displayService = new DisplayService(hardware.Display);
            displayService.ShowSplashScreen();
            Thread.Sleep(3000);
            displayService.ShowDataScreen();

            iotHubService = new IoTHubService();

            hardware.EnvironmentalSensor.Updated += EnvironmentalSensorUpdated;
        }

        private async void EnvironmentalSensorUpdated(object sender, Meadow.IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            hardware.RgbPwmLed.StartBlink(Color.Orange);

            displayService.UpdateWiFiStatus(network.IsConnected);

            displayService.UpdateAtmosphericConditions(
                pressure: $"{e.New.Pressure.Value.Millibar:N0}",
                humidity: $"{e.New.Humidity.Value.Percent:N0}",
                temperature: $"{e.New.Temperature.Value.Celsius:N0}");

            if (network.IsConnected)
            {
                if (iotHubService.isInitialized)
                {
                    await SendDataToIoTHub(e.New);
                }
                else
                {
                    await InitializeIoTHub();
                }
            }

            hardware.RgbPwmLed.StartBlink(Color.Green);
        }

        private async Task InitializeIoTHub()
        {
            displayService.UpdateStatus("Authenticating...");

            bool authenticated = await iotHubService.Initialize();

            displayService.UpdateStatus(authenticated
                ? "Authenticated"
                : "Not Authenticated");
        }

        private async Task SendDataToIoTHub((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) data)
        {
            displayService.UpdateSyncStatus(true);
            displayService.UpdateStatus("Sending data...");

            await iotHubService.SendEnvironmentalReading(data);

            displayService.UpdateSyncStatus(false);
            displayService.UpdateStatus("Data sent!");
        }

        public void Run()
        {
            hardware.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(20));
        }
    }
}