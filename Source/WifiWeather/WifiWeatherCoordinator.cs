using Meadow.Hardware;
using System;
using System.Threading.Tasks;
using WifiWeather.Hardware;
using WifiWeather.Services;

namespace WifiWeather
{
    internal class WifiWeatherCoordinator
    {
        IWifiWeatherHardware hardware;
        INetworkAdapter network;
        DisplayService displayService;
        WeatherService weatherService;

        public WifiWeatherCoordinator(IWifiWeatherHardware hardware, INetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

            displayService = new DisplayService(hardware.Display);
            weatherService = new WeatherService();

            hardware.TemperatureSensor.TemperatureUpdated += TemperatureUpdated;
            hardware.TemperatureSensor.StartUpdating(TimeSpan.FromMinutes(10));
        }

        void TemperatureUpdated(object sender, Meadow.IChangeResult<Meadow.Units.Temperature> e)
        {
            displayService.UpdateIndoorTemperature((int)e.New.Celsius);
        }

        async Task UpdateOutdoorValues()
        {
            var outdoorConditions = await weatherService.GetWeatherForecast();
            displayService.UpdateOutdoorTemperature(outdoorConditions.Value.Item1);
            displayService.UpdateWeatherIcon(outdoorConditions.Value.Item2);
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
                displayService.UpdateDateTime(today);

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}