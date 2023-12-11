using Meadow;
using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;
using WifiWeather.Controllers;
using WifiWeather.Hardware;

namespace WifiWeather
{
    internal class MainController
    {
        bool firstWeatherForecast = true;

        IWifiWeatherHardware hardware;
        INetworkAdapter network;
        DisplayController displayController;
        RestClientController restClientController;

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

            //hardware.TemperatureSensor.Updated += TemperatureSensorUpdated;
            //hardware.TemperatureSensor.StartUpdating(TimeSpan.FromMinutes(10));
        }

        //private void TemperatureSensorUpdated(object sender, IChangeResult<Meadow.Units.Temperature> e)
        //{
        //    displayController.UpdateIndoorTemperature((int)e.New.Celsius);
        //}

        async Task UpdateOutdoorValues()
        {
            displayController.UpdateSyncStatus(true);

            var outdoorConditions = await restClientController.GetWeatherForecast();

            if (outdoorConditions != null)
            {
                firstWeatherForecast = false;
                displayController.UpdateReadings(
                    outdoorConditions.Value.Item1,
                    outdoorConditions.Value.Item2,
                    outdoorConditions.Value.Item3,
                    outdoorConditions.Value.Item4,
                    outdoorConditions.Value.Item5,
                    outdoorConditions.Value.Item6);
                displayController.UpdateWeatherIcon(outdoorConditions.Value.Item7);
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
                    Resolver.Log.Trace("Connected!");

                    if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0 || firstWeatherForecast)
                    {
                        Resolver.Log.Trace("Getting forecast values...");

                        await UpdateOutdoorValues();

                        Resolver.Log.Trace("Forecast acquired...");
                    }

                    int TimeZoneOffSet = -8; // PST
                    var today = DateTime.Now.AddHours(TimeZoneOffSet);
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