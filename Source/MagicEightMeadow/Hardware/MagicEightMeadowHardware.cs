using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Peripherals.Leds;

namespace MagicEightMeadow.Hardware
{
    internal class MagicEightMeadowHardware : IMagicEightMeadowHardware
    {
        protected IProjectLabHardware projectLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public IRgbPwmLed RgbPwmLed { get; set; }

        public Bmi270 MotionSensor { get; set; }

        public void Initialize()
        {
            projectLab = ProjectLab.Create();

            Display = projectLab.Display;

            RgbPwmLed = projectLab.RgbLed;

            MotionSensor = projectLab.MotionSensor;
        }
    }
}