using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Peripherals.Leds;

namespace MagicEightMeadow.Hardware
{
    internal interface IMagicEightMeadowHardware
    {
        public IGraphicsDisplay Display { get; }

        public IRgbPwmLed RgbPwmLed { get; }

        public Bmi270 MotionSensor { get; }

        public void Initialize();
    }
}