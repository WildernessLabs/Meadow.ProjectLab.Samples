using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Threading.Tasks;

namespace ModbusSample;

// Change F7FeatherV2 to F7FeatherV1 for V1.x boards
public class MeadowApp : App<F7CoreComputeV2>
{
    public override async Task Run()
    {
        var projLab = ProjectLab.Create() as ProjectLabHardwareV3;

        var rst1 = projLab.MikroBus1.Pins.RST.CreateDigitalOutputPort(false);
        var rst2 = projLab.MikroBus2.Pins.RST.CreateDigitalOutputPort(true);

        while (true)
        {
            rst1.State = !(rst2.State = true);
            await Task.Delay(20);
            rst2.State = !(rst1.State = true);
            await Task.Delay(20);
        }
    }

    /*
    private const byte thermostatAddress = 201;
    private const int DefaultBaudRate = 19200;

    private IProjectLabHardware _projLab;
    private Tstat8 _thermostat;
    private ThermostatViewModel _view;
    private RgbPwmLed? _led;

    public override async Task Initialize()
    {
        _projLab = ProjectLab.Create();

        var pl = (_projLab as ProjectLabHardwareV3).MikroBus1.Pins.AN.CreateAnalogInputPort(1);
        var pl2 = ((_projLab as ProjectLabHardwareV3).Connectors[0] as MikroBusConnector).Pins.AN.CreateAnalogInputPort(1);

        if (_projLab.RgbLed != null)
        {
            _led = _projLab.RgbLed;

            // TODO: create the on-board if we're an F7 Feather (early PL boards)
            //            _led = new RgbPwmLed(Device.Pins.OnboardLedRed, Device.Pins.OnboardLedGreen, Device.Pins.OnboardLedBlue);
        }

        _led?.SetColor(Color.Red);

        _projLab.UpButton.Clicked += ButtonUpClicked;
        _projLab.DownButton.Clicked += ButtonDownClicked;

        Resolver.Log.Info($"Creating thermostat");

        var modbus = _projLab.GetModbusRtuClient(DefaultBaudRate);
        _thermostat = new Tstat8(modbus, thermostatAddress);

        Resolver.Log.Info($"Creating viewmodel");
        _view = new ThermostatViewModel(_projLab.Display);

        Resolver.Log.Info($"getting initial values");

        _view.Update();

        _led?.SetColor(Color.Green);
    }

    private async void ButtonUpClicked(object sender, EventArgs e)
    {
        Resolver.Log.Info($"Increasing set point");
        var newSP = _view.CurrentSetpoint + 0.5;
        await _thermostat.SetOccupiedSetpoint(newSP);
    }

    private async void ButtonDownClicked(object sender, EventArgs e)
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
                _view.Update();
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to get thermostat info: {ex.Message}");
            }

            await Task.Delay(1000);
        }
    }

    public override Task Run()
    {
        Task.Run(StateMonitor);

        return base.Run();
    }
    */
}