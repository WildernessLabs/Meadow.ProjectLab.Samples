using Meadow.Devices;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Light;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample.Controller
{
    public class MainController
    {
        private static readonly Lazy<MainController> instance =
            new Lazy<MainController>(() => new MainController());
        public static MainController Instance => instance.Value;

        private Bh1750 lightSensor;

        private Bmi270 motionSensor;

        private Bme688 environmentalSensor;

        public Illuminance? IlluminanceReading { get; private set; }

        public (Acceleration3D? acceleration3D, AngularVelocity3D? angularVelocity3D, Temperature? temperature) MotionReading { get; private set; }

        public (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) AmbientReading { get; private set; }

        public MainController() { }

        public void Initialize(IProjectLabHardware hardware)
        {
            lightSensor = hardware.LightSensor;
            motionSensor = hardware.MotionSensor;
            environmentalSensor = hardware.EnvironmentalSensor;
        }

        public Task StartUpdating(TimeSpan updateInterval)
        {
            Task.Run(async () =>
            {

                while (true)
                {
                    IlluminanceReading = await lightSensor.Read();
                    MotionReading = await motionSensor.Read();
                    AmbientReading = await environmentalSensor.Read();

                    await Task.Delay(updateInterval);
                }
            });

            return Task.CompletedTask;
        }
    }
}