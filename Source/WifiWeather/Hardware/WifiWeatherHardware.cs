using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Buttons;

namespace WifiWeather.Hardware
{
    internal class WifiWeatherHardware : IWifiWeatherHardware
    {
        protected IProjectLabHardware ProjLab { get; private set; }

        public IButton UpButton { get; set; }

        public IButton DownButton { get; set; }

        public IGraphicsDisplay Display { get; set; }

        public ITemperatureSensor TemperatureSensor { get; set; }

        public void Initialize()
        {
            ProjLab = ProjectLab.Create();

            UpButton = ProjLab.UpButton;

            DownButton = ProjLab.DownButton;

            Display = ProjLab.Display;

            TemperatureSensor = (ProjLab as ProjectLabHardwareBase).EnvironmentalSensor;
        }
    }
}