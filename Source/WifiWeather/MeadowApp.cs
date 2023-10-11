using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;
using WifiWeather.Services;
using WifiWeather.ViewModels;
using WifiWeather.Views;

namespace WifiWeather
{
    // Change F7CoreComputeV2 to F7FeatherV2 for ProjectLab v2
    public class MeadowApp : App<F7CoreComputeV2>
    {
        RgbPwmLed onboardLed;
        WeatherView displayController;
        IProjectLabHardware projectLab;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            wifi.NetworkConnected += NetworkConnected;

            return Task.CompletedTask;
        }

        private async void NetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            Resolver.Log.Info("Connected...");

            projectLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            onboardLed = projectLab.RgbLed;
            onboardLed.SetColor(Color.Red);

            displayController = new WeatherView();
            displayController.Initialize(projectLab.Display);

            onboardLed.SetColor(Color.Green);

            await GetTemperature();

            while (true)
            {
                if (projectLab != null)
                {
                    if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                    {
                        await GetTemperature();
                    }

                    int TimeZoneOffSet = -7; // PST
                    var today = DateTime.Now.AddHours(TimeZoneOffSet);
                    displayController.UpdateDateTime(today);
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        async Task GetTemperature()
        {
            onboardLed.StartPulse(Color.Magenta);

            // Get indoor conditions
            var conditions = await projectLab.EnvironmentalSensor.Read();

            // Get outdoor conditions
            var outdoorConditions = await WeatherService.GetWeatherForecast();

            onboardLed.StartPulse(Color.Orange);

            // Format indoor/outdoor conditions data
            var model = new WeatherViewModel(outdoorConditions, conditions.Temperature);

            // Send formatted data to display to render
            displayController.UpdateDisplay(model);

            onboardLed.StartPulse(Color.Green);
        }
    }
}