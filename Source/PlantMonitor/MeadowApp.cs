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
    public class MeadowApp : App<F7FeatherV2>
    {
        RgbPwmLed onboardLed;
        IProjectLabHardware projLab;
        MoistureSensor moistureSensor;
        DisplayController displayController;

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

            moistureSensor = new MoistureSensor(Device, Device.Pins.A01);
            var moistureSensorObserver = MoistureSensor.CreateObserver(
                handler: result =>
                {
                    onboardLed.SetColor(Color.Purple);

                    displayController.Update((int)ExtensionMethods.Map(result.New.Millivolts, 0, 1500, 0, 100));

                    onboardLed.SetColor(Color.Green);
                },
                filter: null
            );
            moistureSensor.Subscribe(moistureSensorObserver);

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }

        public override Task Run()
        {
            moistureSensor.StartUpdating(TimeSpan.FromMinutes(1));

            return base.Run();
        }
    }
}