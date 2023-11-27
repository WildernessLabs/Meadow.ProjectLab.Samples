using Meadow.Foundation.Graphics;
using Meadow.Foundation.Grove.Relays;
using Meadow.Peripherals.Leds;

namespace Meadow.Cloud_Command.Hardware
{
    internal interface IMeadowCloudCommandHardware
    {
        public IGraphicsDisplay Display { get; }

        public IRgbPwmLed RgbPwmLed { get; }

        public FourChannelSpdtRelay FourChannelRelay { get; }

        public void Initialize();
    }
}