using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors;
using Meadow.Peripherals.Sensors.Buttons;

namespace WifiWeather.Hardware
{
    internal interface IWifiWeatherHardware
    {
        IButton UpButton { get; }

        IButton DownButton { get; }

        IGraphicsDisplay Display { get; }

        ITemperatureSensor TemperatureSensor { get; }

        void Initialize();
    }
}