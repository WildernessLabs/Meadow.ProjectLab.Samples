using Meadow;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WifiWeather.Controllers;
using WifiWeather.Hardware;

namespace WifiWeather
{
    internal class MainController
    {
        bool firstWeatherForecast = true;

        private IWifiWeatherHardware hardware;
        private INetworkAdapter network;
        private DisplayController displayController;
        private RestClientController restClientController;

        private List<double> temperatureReadings = new List<double>();
        private List<double> pressureReadings = new List<double>();
        private List<double> humidityReadings = new List<double>();

        public MainController(IWifiWeatherHardware hardware, INetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

            displayController = new DisplayController(hardware.Display);
            restClientController = new RestClientController();

            displayController.ShowSplashScreen();
            Thread.Sleep(3000);
            displayController.ShowDataScreen();
        }

        async Task UpdateOutdoorValues()
        {
            displayController.UpdateSyncStatus(true);

            var outdoorConditions = await restClientController.GetWeatherForecast();

            if (outdoorConditions != null)
            {
                firstWeatherForecast = false;



                temperatureReadings.Add(outdoorConditions.Value.Item2);
                pressureReadings.Add(outdoorConditions.Value.Item3);
                humidityReadings.Add(outdoorConditions.Value.Item4);

                if (temperatureReadings.Count > 10)
                {
                    temperatureReadings.RemoveAt(0);
                    pressureReadings.RemoveAt(0);
                    humidityReadings.RemoveAt(0);
                }

                displayController.UpdateReadings(
                    icon: outdoorConditions.Value.Item1,
                    temperature: outdoorConditions.Value.Item2,
                    humidity: outdoorConditions.Value.Item3,
                    pressure: outdoorConditions.Value.Item4,
                    feelsLike: outdoorConditions.Value.Item5,
                    sunrise: outdoorConditions.Value.Item6,
                    sunset: outdoorConditions.Value.Item7,
                    temperatureReadings);
            }

            displayController.UpdateSyncStatus(false);
        }

        public async Task Run()
        {
            while (true)
            {
                displayController.UpdateWiFiStatus(network.IsConnected);

                if (network.IsConnected)
                {
                    int TimeZoneOffSet = -8; // PST
                    var today = DateTime.Now.AddHours(TimeZoneOffSet);

                    Resolver.Log.Trace("Connected!");

                    if (today.Minute == 0 ||
                        today.Minute == 10 ||
                        today.Minute == 20 ||
                        today.Minute == 30 ||
                        today.Minute == 40 ||
                        today.Minute == 50 ||
                        firstWeatherForecast)
                    {
                        Resolver.Log.Trace("Getting forecast values...");

                        await UpdateOutdoorValues();

                        Resolver.Log.Trace("Forecast acquired!");
                    }

                    displayController.UpdateStatus(today.ToString("hh:mm tt | dd/MM/yyyy"));

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                else
                {
                    Resolver.Log.Trace("Not connected, checking in 10s");

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}