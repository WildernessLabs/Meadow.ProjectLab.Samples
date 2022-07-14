using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace HackBoard_Test
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        private ProjLab _board;
        private RgbPwmLed _onboardLed;
        private DisplayController _displayController;

        public MeadowApp()
        {
        }

        public override Task Initialize()
        {
            Resolver.Log.Info("Initializing hardware...");

            _board = new ProjLab(Resolver.Log);

            _onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);

            _onboardLed.SetColor(Color.Red);

            _displayController = new DisplayController(_board.Display);

            try
            {
                _board.LightSensor.Updated += Bh1750Updated;
                _board.LightSensor.StartUpdating(TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Could not bring up Bh1750: {e.Message}");
            }

            try
            {
                _board.EnvironmentalSensor.Updated += Bme688Updated;
                _board.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Could not bring up Bme688: {e.Message}");
            }

            _board.UpButton.PressStarted += (s, e) => _displayController.UpButtonState = true;
            _board.DownButton.PressStarted += (s, e) => _displayController.DownButtonState = true;
            _board.LeftButton.PressStarted += (s, e) => _displayController.LeftButtonState = true;
            _board.RightButton.PressStarted += (s, e) => _displayController.RightButtonState = true;

            _board.UpButton.PressEnded += (s, e) => _displayController.UpButtonState = false;
            _board.DownButton.PressEnded += (s, e) => _displayController.DownButtonState = false;
            _board.LeftButton.PressEnded += (s, e) => _displayController.LeftButtonState = false;
            _board.RightButton.PressEnded += (s, e) => _displayController.RightButtonState = false;

            _onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> e)
        {
            Console.WriteLine($"BME688: {(int)e.New.Temperature?.Celsius:0.0}C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");
            _displayController.AtmosphericConditions = e.New;
        }

        private void Bh1750Updated(object sender, IChangeResult<Illuminance> e)
        {
            Console.WriteLine($"BH1750: {e.New.Lux}:0.0");
            _displayController.LightConditions = e.New;
        }
    }
}