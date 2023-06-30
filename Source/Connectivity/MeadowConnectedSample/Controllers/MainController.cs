using Meadow.Devices;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Light;
using Meadow.Units;
using MeadowConnectedSample.Connectivity;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample.Controller
{
    public class MainController
    {
        private static readonly Lazy<MainController> instance =
            new Lazy<MainController>(() => new MainController());
        public static MainController Instance => instance.Value;

        private Bme688 environmentalSensor;

        private Bh1750 lightSensor;

        private Bmi270 motionSensor;

        public bool UseWiFi { get; set; } = true;

        public (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) EnvironmentalReading { get; private set; }

        public Illuminance? LightReading { get; private set; }

        public (Acceleration3D? acceleration3D, AngularVelocity3D? angularVelocity3D, Temperature? temperature) MotionReading { get; private set; }

        public MainController() { }

        public void Initialize(IProjectLabHardware hardware)
        {
            lightSensor = hardware.LightSensor;
            motionSensor = hardware.MotionSensor;
            environmentalSensor = hardware.EnvironmentalSensor;
        }

        public async Task StartUpdating(TimeSpan updateInterval)
        {
            while (true)
            {
                Console.Write("Reading...");

                EnvironmentalReading = environmentalSensor.Read().Result;
                LightReading = lightSensor.Read().Result;
                MotionReading = motionSensor.Read().Result;

                if (!UseWiFi)
                {
                    BluetoothServer.Instance.SetEnvironmentalCharacteristicValue(EnvironmentalReading);
                    BluetoothServer.Instance.SetLightCharacteristicValue(LightReading);
                    BluetoothServer.Instance.SetMotionCharacteristicValue(MotionReading);
                }

                Console.WriteLine("Done");

                await Task.Delay(updateInterval);
            }
        }
    }
}