using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;

namespace WifiWeather.Hardware
{
    internal class WifiWeatherHardware : IWifiWeatherHardware
    {
        protected IProjectLabHardware ProjLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public ITemperatureSensor TemperatureSensor { get; set; }

        public void Initialize()
        {
            ProjLab = ProjectLab.Create();

            Display = ProjLab.Display;

            TemperatureSensor = ProjLab.EnvironmentalSensor;
        }
    }
}