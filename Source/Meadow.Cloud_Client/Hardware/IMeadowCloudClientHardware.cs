using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Sensors.Buttons;

namespace Meadow.Cloud_Client.Hardware
{
    internal interface IMeadowCloudClientHardware
    {
        public IGraphicsDisplay Display { get; }

        public IButton RightButton { get; }

        public IButton LeftButton { get; }

        public RgbPwmLed RgbPwmLed { get; }

        public void Initialize();
    }
}