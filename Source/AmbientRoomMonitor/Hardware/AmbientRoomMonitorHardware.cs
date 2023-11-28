using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Peripherals.Sensors.Light;

namespace AmbientRoomMonitor.Hardware
{
    internal class AmbientRoomMonitorHardware : IAmbientRoomMonitorHardware
    {
        protected IProjectLabHardware ProjLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public RgbPwmLed RgbPwmLed { get; set; }

        public ILightSensor LightSensor { get; set; }

        public Bme68x EnvironmentalSensor { get; set; }

        public void Initialize()
        {
            ProjLab = ProjectLab.Create();

            Display = ProjLab.Display;

            RgbPwmLed = ProjLab.RgbLed;

            LightSensor = ProjLab.LightSensor;

            EnvironmentalSensor = ProjLab.EnvironmentalSensor;
        }
    }
}