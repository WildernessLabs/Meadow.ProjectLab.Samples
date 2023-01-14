using Meadow;
using Meadow.Foundation.Graphics;

namespace Simon
{
    public class ThermostatViewModel
    {
        private double currentTemp;
        private MicroGraphics graphics;
        private bool isDirty;
        private double currentSetpoint;

        public ThermostatViewModel(IGraphicsDisplay display)
        {
            graphics = new MicroGraphics(display)
            {
                Rotation = RotationType._90Degrees,
                Stroke = 5,
                CurrentFont = new Font12x16()
            };
        }

        public double CurrentTemp
        {
            get => currentTemp;
            set
            {
                if (value != CurrentTemp)
                {
                    currentTemp = value;
                    isDirty = true;
                }
            }
        }

        public double CurrentSetpoint
        {
            get => currentSetpoint;
            set
            {
                if (value != CurrentSetpoint)
                {
                    currentSetpoint = value;
                    isDirty = true;
                }
            }
        }

        public void Update()
        {
            Resolver.Log.Info($"updating display");
            if (isDirty)
            {
                graphics.Clear();

                graphics.DrawText(this.graphics.Width / 2, 20, currentTemp.ToString("0.0"), alignment: TextAlignment.Center);
                graphics.DrawText(this.graphics.Width / 2, 60, currentSetpoint.ToString("0.0"), alignment: TextAlignment.Center);

                graphics.Show();
            }
            isDirty = false;
        }
    }
}