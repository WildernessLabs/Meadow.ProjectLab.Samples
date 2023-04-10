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
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        RgbPwmLed onboardLed;
        IProjectLabHardware projectLab;
        AmqpController amqpController;

        public override async Task Initialize()
        {
            Console.WriteLine("Initializing...");
            onboardLed = new RgbPwmLed(
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            try
            {
                amqpController = new AmqpController();

                var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
                wifi.NetworkConnected += NetworkConnected;

                projectLab = ProjectLab.Create();
                projectLab.EnvironmentalSensor.Updated += EnvironmentalSensorUpdated;

                DisplayController.Instance.Initialize(projectLab.Display);
                DisplayController.Instance.ShowSplashScreen();
                DisplayController.Instance.StartConnectingAnimation();
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to Connect: {ex.Message}");
            }
        }

        private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Console.WriteLine("Connected...");
            DisplayController.Instance.StopConnectingAnimation();

            await amqpController.Initialize();

            projectLab.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(5));

            onboardLed.SetColor(Color.Green);
        }

        private void EnvironmentalSensorUpdated(object sender, IChangeResult<(Meadow.Units.Temperature? Temperature, Meadow.Units.RelativeHumidity? Humidity, Meadow.Units.Pressure? Pressure, Meadow.Units.Resistance? GasResistance)> e)
        {
            amqpController.SendEnvironmentalReading(e.New);
        }
    }
}