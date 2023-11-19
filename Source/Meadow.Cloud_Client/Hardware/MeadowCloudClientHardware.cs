using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;

namespace Meadow.Cloud_Client.Hardware
{
    internal class MeadowCloudClientHardware : IMeadowCloudClientHardware
    {
        protected IProjectLabHardware ProjLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public RgbPwmLed RgbPwmLed { get; set; }

        public void Initialize()
        {
            ProjLab = ProjectLab.Create();

            Display = ProjLab.Display;

            RgbPwmLed = ProjLab.RgbLed;
        }
    }
}