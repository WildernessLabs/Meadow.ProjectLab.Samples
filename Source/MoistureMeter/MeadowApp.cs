using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Grove.Sensors.Moisture;
using MoistureMeter.Controllers;
using System;
using System.Threading.Tasks;

namespace MoistureMeter
{
    // Change F7MicroV2 to F7Micro for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        MoistureSensor sensor;
        ProjectLab projLab;

        public override Task Initialize()
        {
            projLab = new ProjectLab();

            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.HardwareRevision}");

            projLab.Led.SetColor(Color.Red);

            DisplayController.Instance.Initialize(projLab.Display);

            sensor = new MoistureSensor(Device, Device.Pins.A01);

            sensor.Updated += (sender, result) =>
            {
                var percentage = (int)ExtensionMethods.Map(result.New.Millivolts, 0, 1750, 0, 100);

                DisplayController.Instance.UpdatePercentage(percentage > 100 ? 100 : percentage);
            };

            sensor.StartUpdating(TimeSpan.FromMilliseconds(1000));

            projLab.Led.SetColor(Color.Green);

            return base.Initialize();
        }
    }
}