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
    // Change F7FeatherV2 to F7CoreComputeV2 for ProjectLab v3
    public class MeadowApp : App<F7CoreComputeV2>
    {
        RgbPwmLed onboardLed;
        WeatherView displayController;
        IProjectLabHardware projectLab;

        public override async Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            projectLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            onboardLed = projectLab.RgbLed;
            onboardLed.SetColor(Color.Red);

            displayController = new WeatherView();
            displayController.Initialize(projectLab.Display);

            var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
            await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));

            onboardLed.SetColor(Color.Green);
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

        public override async Task Run()
        {
            await GetTemperature();

            while (true)
            {
                if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    await GetTemperature();
                }

                displayController.UpdateDateTime();
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}