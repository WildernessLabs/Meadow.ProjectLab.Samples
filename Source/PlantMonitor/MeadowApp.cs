using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Grove.Sensors.Moisture;
using PlantMonitor.Controllers;
using System;
using System.Threading.Tasks;

namespace PlantMonitor
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        ProjectLab projLab;
        MoistureSensor moistureSensor;
        DisplayController displayController;

        public override Task Initialize()
        {
            projLab = new ProjectLab();

            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.HardwareRevision}");

            projLab.Led.SetColor(Color.Red);

            DisplayController.Instance.Initialize(projLab.Display);

            moistureSensor = new MoistureSensor(Device, Device.Pins.A01);
            var moistureSensorObserver = MoistureSensor.CreateObserver(
                handler: result =>
                {
                    projLab.Led.SetColor(Color.Purple);

                    displayController.Update((int)ExtensionMethods.Map(result.New.Millivolts, 0, 1500, 0, 100));

                    projLab.Led.SetColor(Color.Green);
                },
                filter: null
            );
            moistureSensor.Subscribe(moistureSensorObserver);

            projLab.Led.SetColor(Color.Green);

            return base.Initialize();
        }

        public override Task Run()
        {
            moistureSensor.StartUpdating(TimeSpan.FromMinutes(1));

            return base.Run();
        }
    }
}