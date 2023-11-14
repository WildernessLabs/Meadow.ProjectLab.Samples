using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors.Motion;

namespace MagicEightMeadow.Hardware
{
    internal class MagicEightHardware : IMagicEightHardware
    {
        protected IProjectLabHardware projectLab { get; private set; }

        public IGraphicsDisplay Display { get; set; }

        public IGyroscope MotionSensor { get; set; }

        public void Initialize() 
        {
            projectLab = ProjectLab.Create();

            Display = projectLab.Display;

            MotionSensor = projectLab.MotionSensor;
        }
    }
}
