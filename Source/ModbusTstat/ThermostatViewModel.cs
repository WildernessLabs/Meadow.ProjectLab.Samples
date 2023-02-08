using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace ModbusSample
{
    public class ThermostatViewModel
    {
        private Temperature _currentTemp;
        private MicroGraphics _graphics;
        private bool _isDirty;
        private double _currentSetpoint;

        public ThermostatViewModel(IGraphicsDisplay display)
        {
            _graphics = new MicroGraphics(display)
            {
                Rotation = RotationType._90Degrees,
                Stroke = 5,
                CurrentFont = new Font12x16()
            };
        }

        public Temperature CurrentTemp
        {
            get => _currentTemp;
            set
            {
                if (value != CurrentTemp)
                {
                    _currentTemp = value;
                    _isDirty = true;
                }
            }
        }

        public double CurrentSetpoint
        {
            get => _currentSetpoint;
            set
            {
                if (value != CurrentSetpoint)
                {
                    _currentSetpoint = value;
                    _isDirty = true;
                }
            }
        }

        public void Update()
        {
            Resolver.Log.Info($"updating display");
            if (_isDirty)
            {
                _graphics.Clear();

                _graphics.DrawText(this._graphics.Width / 2, 20, _currentTemp.Fahrenheit.ToString("0.0"), alignmentH: HorizontalAlignment.Center);
                _graphics.DrawText(this._graphics.Width / 2, 60, _currentSetpoint.ToString("0.0"), alignmentH: HorizontalAlignment.Center);

                _graphics.Show();
            }
            _isDirty = false;
        }
    }
}