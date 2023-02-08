using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Thermostats;
using System;
using System.Threading.Tasks;

namespace ModbusSample
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        private const byte thermostatAddress = 201;
        private const int DefaultBaudRate = 19200;

        private IProjectLabHardware _projLab;
        private Tstat8 _thermostat;
        private ThermostatViewModel _view;
        private RgbPwmLed _led;

        public override async Task Initialize()
        {
            _led = new RgbPwmLed(Device.Pins.OnboardLedRed, Device.Pins.OnboardLedGreen, Device.Pins.OnboardLedBlue);

            _projLab = ProjectLab.Create();

            _led.SetColor(Color.Red);

            _projLab.UpButton.Clicked += ButtonUpClicked;
            _projLab.DownButton.Clicked += ButtonDownClicked;

            Resolver.Log.Info($"Creating thermostat");
            _thermostat = new Tstat8(_projLab.GetModbusRtuClient(DefaultBaudRate), thermostatAddress);

            Resolver.Log.Info($"Creating viewmodel");
            _view = new ThermostatViewModel(_projLab.Display);

            Resolver.Log.Info($"getting initial values");

            _view.Update();

            _led.SetColor(Color.Green);
        }

        async void ButtonUpClicked(object sender, EventArgs e)
        {
            Resolver.Log.Info($"Increasing set point");
            var newSP = _view.CurrentSetpoint + 0.5;
            await _thermostat.SetOccupiedSetpoint(newSP);
        }

        async void ButtonDownClicked(object sender, EventArgs e)
        {
            Resolver.Log.Info($"Decreasing set point");
            var newSP = _view.CurrentSetpoint - 0.5;
            await _thermostat.SetOccupiedSetpoint(newSP);
        }

        private async void StateMonitor()
        {
            while (true)
            {
                try
                {
                    _view.CurrentTemp = await _thermostat.GetCurrentTemperature();
                    await Task.Delay(1000);
                    _view.CurrentSetpoint = await _thermostat.GetOccupiedSetpoint();
                }
                catch (Exception ex)
                {
                    Resolver.Log.Warn(ex.Message);
                }
                _view.Update();

                await Task.Delay(1000);
            }
        }

        public override Task Run()
        {
            Task.Run(StateMonitor);

            return base.Run();
        }
    }
}