using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;

namespace Meadow.Cloud_Logging.Hardware
{
    internal interface IMeadowCloudLoggingHardware
    {
        public IGraphicsDisplay Display { get; }

        public Bme68x EnvironmentalSensor { get; }

        public RgbPwmLed RgbPwmLed { get; }

        public void Initialize();
    }
}