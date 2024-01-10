using Meadow.Foundation.Graphics;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Peripherals.Leds;

namespace MeadowAzureIoTHub.Hardware
{
    internal interface IMeadowAzureIoTHubHardware
    {
        public IGraphicsDisplay Display { get; }

        public Bme68x EnvironmentalSensor { get; }

        public IRgbPwmLed RgbPwmLed { get; }

        public void Initialize();
    }
}