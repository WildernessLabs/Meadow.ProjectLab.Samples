using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Grove.Relays;
using Meadow.Foundation.Leds;

namespace Meadow.Cloud_Command.Hardware
{
    internal class MeadowCloudCommandHardware : IMeadowCloudCommandHardware
    {
        protected IProjectLabHardware ProjLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public RgbPwmLed RgbPwmLed { get; set; }

        public FourChannelSpdtRelay FourChannelRelay { get; set; }

        public void Initialize()
        {
            ProjLab = ProjectLab.Create();

            Display = ProjLab.Display;

            RgbPwmLed = ProjLab.RgbLed;

            FourChannelRelay = new FourChannelSpdtRelay(ProjLab.I2cBus, 0x11);
        }
    }
}