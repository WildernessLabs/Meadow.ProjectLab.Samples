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
    public class MeadowApp : App<F7FeatherV2, MeadowApp>
    {
        MoistureSensor sensor;

        public MeadowApp()
        {
            Initialize();
        }

        void Initialize()
        {
            var onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            var displayController = new DisplayController();

            sensor = new MoistureSensor(Device, Device.Pins.A01);

            sensor.Updated += (sender, result) => 
            {
                var percentage = (int)ExtensionMethods.Map(result.New.Millivolts, 0, 1750, 0, 100);

                displayController.UpdatePercentage(percentage > 100 ? 100 : percentage);
                //Console.WriteLine($"RAW: {result.New.Millivolts:N2}mV - PERCENTAGE: {percentage}%");
            };

            sensor.StartUpdating(TimeSpan.FromMilliseconds(1000));

            onboardLed.SetColor(Color.Green);
        }
    }
}