using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors.Motion;

namespace MagicEightMeadow.Hardware
{
    internal interface IMagicEightHardware
    {
        public IGraphicsDisplay Display { get; }

        public IGyroscope MotionSensor { get; }

        public void Initialize();
    }
}