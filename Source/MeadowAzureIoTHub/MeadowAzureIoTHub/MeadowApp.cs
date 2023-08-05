using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using MeadowAzureIoTHub.Azure;
using MeadowAzureIoTHub.Views;
using System;
using System.Threading.Tasks;

namespace MeadowAzureIoTHub
{
    // Change F7CoreComputeV2 to F7FeatherV2 for ProjectLab v2
    public class MeadowApp : App<F7CoreComputeV2>
    {
        RgbPwmLed onboardLed;
        IProjectLabHardware projectLab;
        IotHubManager amqpController;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += NetworkConnected;

            projectLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            onboardLed = projectLab.RgbLed;
            onboardLed.SetColor(Color.Red);

            try
            {
                amqpController = new IotHubManager();

                projectLab.EnvironmentalSensor.Updated += EnvironmentalSensorUpdated;

                DisplayController.Instance.Initialize(projectLab.Display);
                DisplayController.Instance.ShowSplashScreen();
                DisplayController.Instance.ShowConnectingAnimation();
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to Connect: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            DisplayController.Instance.ShowConnected();

            await amqpController.Initialize();

            projectLab.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(15));

            onboardLed.SetColor(Color.Green);
        }

        private async void EnvironmentalSensorUpdated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> e)
        {
            await amqpController.SendEnvironmentalReading(e.New);
            await DisplayController.Instance.StartSyncCompletedAnimation(e.New);
        }
    }
}