using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Grove.Sensors.Moisture;
using Meadow.Foundation.Leds;
using MoistureMeter.Controllers;
using System;
using System.Threading.Tasks;

namespace MoistureMeter
{
    // Change F7MicroV2 to F7Micro for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        RgbPwmLed onboardLed;
        MoistureSensor sensor;
        IProjectLabHardware projLab;

        public override Task Initialize()
        {
            onboardLed = new RgbPwmLed(
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            projLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.RevisionString}");

            DisplayController.Instance.Initialize(projLab.Display);

            sensor = new MoistureSensor(Device.Pins.A01);

            sensor.Updated += (sender, result) =>
            {
                var percentage = (int)ExtensionMethods.Map(result.New.Millivolts, 0, 1750, 0, 100);

                DisplayController.Instance.UpdatePercentage(percentage > 100 ? 100 : percentage);
            };

            sensor.StartUpdating(TimeSpan.FromMilliseconds(1000));

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }
    }
}