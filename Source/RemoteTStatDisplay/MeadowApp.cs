using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Thermostats;
using System;
using System.Threading.Tasks;

namespace AmbientRoomMonitor
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        private RgbPwmLed _led;
        private MicroGraphics _graphics;
        private IProjectLabHardware _projLab;
        private T8 _thermostat;

        public override Task Initialize()
        {
            _led = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);

            _led.SetColor(Color.Red);

            _projLab = ProjectLab.Create();

            Resolver.Log.Info($"Running on ProjectLab Hardware {_projLab.RevisionString}");

            _graphics = new MicroGraphics(_projLab.Display)
            {
                IgnoreOutOfBoundsPixels = true,
                Rotation = RotationType._90Degrees
            };

            _thermostat = new T8(_projLab.GetModbusRtuClient(19200), 201);

            _led.SetColor(Color.Green);

            return base.Initialize();
        }

        public override Task Run()
        {
            return Task.Run(ThermostatUpdateProc);
        }

        private async Task ThermostatUpdateProc()
        {
            while (true)
            {
                var temp = await _thermostat.GetCurrentTemperature();
                var occupiedSP = await _thermostat.GetOccupiedSetpoint();

                Resolver.Log.Debug($"Temp: {temp}");
                Resolver.Log.Debug($"SP: {occupiedSP}");

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}