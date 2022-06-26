using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Grove.Sensors.Moisture;
using Meadow.Foundation.Leds;
using PlantMonitor.Controllers;
using System;
using System.Threading.Tasks;

namespace PlantMonitor
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>, IApp
    {
        RgbPwmLed onboardLed;
        MoistureSensor moistureSensor;
        DisplayController displayController;

        async Task IApp.Initialize()
        {
            onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            displayController = new DisplayController();

            moistureSensor = new MoistureSensor(Device, Device.Pins.A01);
            var moistureSensorObserver = MoistureSensor.CreateObserver(
                handler: result =>
                {
                    onboardLed.SetColor(Color.Purple);

                    displayController.Update((int) ExtensionMethods.Map(result.New.Millivolts, 0, 1500, 0, 100));

                    onboardLed.SetColor(Color.Green);
                },
                filter: null
            );
            moistureSensor.Subscribe(moistureSensorObserver);
            moistureSensor.StartUpdating(TimeSpan.FromMinutes(1));

            onboardLed.SetColor(Color.Green);
        }
    }
}