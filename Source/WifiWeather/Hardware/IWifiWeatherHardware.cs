using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;

namespace WifiWeather.Hardware
{
    internal interface IWifiWeatherHardware
    {
        public IGraphicsDisplay Display { get; }

        public ITemperatureSensor TemperatureSensor { get; }

        public void Initialize();
    }
}