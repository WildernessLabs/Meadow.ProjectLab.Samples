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
    // Change F7FeatherV2 to F7CoreComputeV2 for ProjectLab v3
    public class MeadowApp : App<F7FeatherV2>
    {
        RgbPwmLed onboardLed;
        MoistureSensor sensor;
        IProjectLabHardware projectLab;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            projectLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            onboardLed = projectLab.RgbLed;
            onboardLed.SetColor(Color.Red);

            DisplayController.Instance.Initialize(projectLab.Display);

            sensor = new MoistureSensor(Device.Pins.A01);

            sensor.Updated += (sender, result) =>
            {
                var percentage = (int)ExtensionMethods.Map(result.New.Millivolts, 0, 1750, 0, 100);

                DisplayController.Instance.UpdatePercentage(Math.Clamp(percentage, 0, 100));
            };

            sensor.StartUpdating(TimeSpan.FromMilliseconds(1000));

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }
    }
}