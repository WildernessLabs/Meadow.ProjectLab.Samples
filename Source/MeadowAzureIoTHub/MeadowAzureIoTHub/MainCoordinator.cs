using Meadow.Foundation;
using Meadow.Hardware;
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

        public async Task Initialize()
        {
            hardware.Initialize();

            displayService = new DisplayService(hardware.Display);
            displayService.ShowSplashScreen();
            Thread.Sleep(3000);
            displayService.ShowDataScreen();

            iotHubService = new IoTHubService();

            if (network.IsConnected)
            {
                displayService.UpdateWiFiStatus(network.IsConnected);
                displayService.UpdateStatus("Authenticating...");
                bool authenticated = await iotHubService.Initialize();
                displayService.UpdateStatus(authenticated
                    ? "Authenticated"
                    : "Not Authenticated");
            }
            else
            {
                network.NetworkConnected += NetworkConnected;
            }

            hardware.EnvironmentalSensor.Updated += EnvironmentalSensorUpdated;
        }

        private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            displayService.UpdateWiFiStatus(network.IsConnected);
            displayService.UpdateStatus("-Authenticating...");
            bool authenticated = await iotHubService.Initialize();
            displayService.UpdateStatus(authenticated
                ? "-Authenticated"
                : "-Not Authenticated");
        }

        private void EnvironmentalSensorUpdated(object sender, Meadow.IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> e)
        {
            hardware.RgbPwmLed.StartBlink(Color.Orange);

            displayService.UpdateAtmosphericConditions(
                pressure: $"{e.New.Pressure.Value.Millibar:N0}",
                humidity: $"{e.New.Humidity.Value.Percent:N0}",
                temperature: $"{e.New.Temperature.Value.Celsius:N0}");

            if (network != null && network.IsConnected && iotHubService.isInitialized)
            {
                displayService.UpdateStatus("Syncing...");
                displayService.UpdateWiFiStatus(network.IsConnected);
                //    iotHubService.SendEnvironmentalReading(e.New);
                displayService.UpdateStatus("Done!");
            }

            hardware.RgbPwmLed.StartBlink(Color.Green);
        }

        public void Run()
        {
            hardware.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(20));
        }
    }
}