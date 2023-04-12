using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Units;
using MeadowAzureServo.Azure;
using MeadowAzureServo.Models;
using MeadowAzureServo.Views;
using System;
using System.Threading.Tasks;

namespace MeadowAzureServo
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        RgbPwmLed onboardLed;
        IoTHubManager amqpController;
        IProjectLabHardware projectLab;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");
            onboardLed = new RgbPwmLed(
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            try
            {
                amqpController = new IoTHubManager();

                var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
                wifi.NetworkConnected += NetworkConnected;

                projectLab = ProjectLab.Create();
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to Connect: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            await amqpController.Initialize();
            amqpController.StartSweeping += StartSweeping;
            amqpController.StopSweeping += StopSweeping;
            amqpController.RotateTo += RotateTo;

            projectLab.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(15));

            onboardLed.SetColor(Color.Green);
        }

        private void StartSweeping(object sender, EventArgs e)
        {
            ServoController.Instance.StartSweep();
        }

        private void StopSweeping(object sender, EventArgs e)
        {
            ServoController.Instance.StopSweep();
        }

        private void RotateTo(object sender, RotateToEventArgs e)
        {
            ServoController.Instance.RotateTo(new Angle(e.AngleInDegrees));
        }
    }
}