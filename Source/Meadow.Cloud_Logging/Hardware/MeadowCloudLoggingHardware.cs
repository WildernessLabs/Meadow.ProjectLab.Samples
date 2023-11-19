using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;

namespace Meadow.Cloud_Logging.Hardware
{
    internal class MeadowCloudLoggingHardware : IMeadowCloudLoggingHardware
    {
        protected IProjectLabHardware ProjLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public Bme68x EnvironmentalSensor { get; set; }

        public RgbPwmLed RgbPwmLed { get; set; }

        public void Initialize()
        {
            ProjLab = ProjectLab.Create();

            Display = ProjLab.Display;

            RgbPwmLed = ProjLab.RgbLed;

            EnvironmentalSensor = ProjLab.EnvironmentalSensor;
        }
    }
}