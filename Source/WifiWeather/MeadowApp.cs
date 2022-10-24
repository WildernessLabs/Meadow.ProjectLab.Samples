using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;
using WifiWeather.Services;
using WifiWeather.ViewModels;
using WifiWeather.Views;

namespace WifiWeather
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        WeatherView displayController;
        ProjectLab projLab;

        public override async Task Initialize()
        {
            projLab = new ProjectLab();

            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.HardwareRevision}");

            projLab.Led.SetColor(Color.Red);

            var connectionResult = await Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>().Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);

            if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
            }

            displayController = new WeatherView();
            displayController.Initialize(projLab.Display);

            projLab.Led.StartPulse(Color.Green);
        }

        async Task GetTemperature()
        {
            projLab.Led.StartPulse(Color.Magenta);

            // Get indoor conditions
            var conditions = await projLab.EnvironmentalSensor.Read();

            // Get outdoor conditions
            var outdoorConditions = await WeatherService.GetWeatherForecast();

            projLab.Led.StartPulse(Color.Orange);

            // Format indoor/outdoor conditions data
            var model = new WeatherViewModel(outdoorConditions, conditions.Temperature);

            // Send formatted data to display to render
            displayController.UpdateDisplay(model);

            projLab.Led.StartPulse(Color.Green);
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