using Meadow.Hardware;
using System;
using System.Threading.Tasks;
using WifiWeather.Controllers;
using WifiWeather.Hardware;

namespace WifiWeather
{
    internal class MainController
    {
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

            hardware.TemperatureSensor.TemperatureUpdated += TemperatureUpdated;
            hardware.TemperatureSensor.StartUpdating(TimeSpan.FromMinutes(10));
        }

        void TemperatureUpdated(object sender, Meadow.IChangeResult<Meadow.Units.Temperature> e)
        {
            displayController.UpdateIndoorTemperature((int)e.New.Celsius);
        }

        async Task UpdateOutdoorValues()
        {
            var outdoorConditions = await restClientController.GetWeatherForecast();
            displayController.UpdateOutdoorTemperature(outdoorConditions.Value.Item1);
            displayController.UpdateWeatherIcon(outdoorConditions.Value.Item2);
        }

        public async Task Run()
        {
            await UpdateOutdoorValues();

            while (true)
            {
                if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0 && network.IsConnected)
                {
                    await UpdateOutdoorValues();
                }

                int TimeZoneOffSet = -8; // PST
                var today = DateTime.Now.AddHours(TimeZoneOffSet);
                displayController.UpdateDateTime(today);

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}