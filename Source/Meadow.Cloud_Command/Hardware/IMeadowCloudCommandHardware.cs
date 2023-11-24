using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Leds;

namespace Meadow.Cloud_Command.Hardware
{
    internal interface IMeadowCloudCommandHardware
    {
        public IGraphicsDisplay Display { get; }

        public IRgbPwmLed RgbPwmLed { get; }

        public void Initialize();
    }
}