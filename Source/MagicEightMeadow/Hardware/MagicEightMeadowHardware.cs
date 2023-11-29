using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;

namespace MagicEightMeadow.Hardware
{
    internal class MagicEightMeadowHardware : IMagicEightMeadowHardware
    {
        protected IProjectLabHardware projectLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public RgbPwmLed RgbPwmLed { get; set; }

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