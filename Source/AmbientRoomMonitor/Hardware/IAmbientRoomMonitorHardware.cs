using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Peripherals.Leds;
using Meadow.Peripherals.Sensors.Light;

namespace AmbientRoomMonitor.Hardware
{
    internal interface IAmbientRoomMonitorHardware
    {
        public IGraphicsDisplay Display { get; }

        public ILightSensor LightSensor { get; }

        public Bme68x EnvironmentalSensor { get; }

        public IRgbPwmLed RgbPwmLed { get; }

        public void Initialize();
    }
}