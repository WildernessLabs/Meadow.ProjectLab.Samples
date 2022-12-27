using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;
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
        RgbPwmLed onboardLed;
        WeatherView displayController;
        ProjectLab projLab;

        public override Task Initialize()
        {
            onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            projLab = new ProjectLab();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.RevisionString}");

            Device.NetworkConnected += DeviceNetworkConnected;

            displayController = new WeatherView();
            displayController.Initialize(projLab.Display);

            onboardLed.StartPulse(Color.Green);

            return Task.CompletedTask;
        }

        private async void DeviceNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
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

        async Task GetTemperature()
        {
            onboardLed.StartPulse(Color.Magenta);

            // Get indoor conditions
            var conditions = await projLab.EnvironmentalSensor.Read();

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