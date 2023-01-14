using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Thermostats;
using System;
using System.Threading.Tasks;

namespace Simon
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        private const byte thermostatAddress = 201;
        private ProjectLab projLab;
        private Tstat8 thermostat;
        private ThermostatViewModel view;

        public override async Task Initialize()
        {
            projLab = new ProjectLab();

            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.HardwareRevision}");

            projLab.Led.SetColor(Color.Red);

            projLab.UpButton.Clicked += ButtonUpClicked;
            projLab.DownButton.Clicked += ButtonDownClicked;

            Resolver.Log.Info($"Creating thermostat");
            thermostat = new Tstat8(projLab.GetModbusRtuClient(), thermostatAddress);

            Resolver.Log.Info($"Creating viewmodel");
            view = new ThermostatViewModel(projLab.Display);

            Resolver.Log.Info($"getting initial values");

            view.Update();

            projLab.Led.SetColor(Color.Green);
        }

        async void ButtonUpClicked(object sender, EventArgs e)
        {
            Resolver.Log.Info($"Increasing set point");
            var newSP = view.CurrentSetpoint + 0.5;
            await thermostat.SetOccupiedSetpoint(newSP);
        }

        async void ButtonDownClicked(object sender, EventArgs e)
        {
            Resolver.Log.Info($"Decreasing set point");
            var newSP = view.CurrentSetpoint - 0.5;
            await thermostat.SetOccupiedSetpoint(newSP);
        }

        private async void StateMonitor()
        {
            while (true)
            {
                try
                {
                    view.CurrentTemp = await thermostat.GetCurrentTemperature();
                    await Task.Delay(1000);
                    view.CurrentSetpoint = await thermostat.GetOccupiedSetpoint();
                }
                catch (Exception ex)
                {
                    Resolver.Log.Warn(ex.Message);
                }
                view.Update();

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