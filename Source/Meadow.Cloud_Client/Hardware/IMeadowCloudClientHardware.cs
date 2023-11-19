using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;

namespace Meadow.Cloud_Client.Hardware
{
    internal interface IMeadowCloudClientHardware
    {
        public IGraphicsDisplay Display { get; }

        public RgbPwmLed RgbPwmLed { get; }

        public void Initialize();
    }
}