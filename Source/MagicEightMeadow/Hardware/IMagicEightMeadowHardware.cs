using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;

namespace MagicEightMeadow.Hardware
{
    internal interface IMagicEightMeadowHardware
    {
        public IGraphicsDisplay Display { get; }

        public RgbPwmLed RgbPwmLed { get; }

        public Bmi270 MotionSensor { get; }

        public void Initialize();
    }
}