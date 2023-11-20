using Meadow.Cloud_Logging.Hardware;
using Meadow.Cloud_Logging.Services;
using Meadow.Foundation;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Meadow.Cloud_Logging
{
    internal class MainCoordinator
    {
        IMeadowCloudLoggingHardware hardware;
        IWiFiNetworkAdapter network;
        DisplayService displayService;

        public MainCoordinator(IMeadowCloudLoggingHardware hardware, IWiFiNetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

            var cloudLogger = new CloudLogger();
            Resolver.Log.AddProvider(cloudLogger);
            Resolver.Services.Add(cloudLogger);

            displayService = new DisplayService(hardware.Display);
            displayService.ShowSplashScreen();
            Thread.Sleep(3000);
            displayService.ShowDataScreen();

            hardware.EnvironmentalSensor.Updated += EnvironmentalSensorUpdated;
        }

        private void EnvironmentalSensorUpdated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            displayService.UpdateWiFiStatus(network.IsConnected);

            hardware.RgbPwmLed.StartBlink(Color.Orange);

            displayService.UpdateAtmosphericConditions(
                pressure: $"{e.New.Pressure.Value.Millibar:N0}",
                humidity: $"{e.New.Humidity.Value.Percent:N0}",
                temperature: $"{e.New.Temperature.Value.Celsius:N0}");

            if (network.IsConnected)
            {
                displayService.UpdateSyncStatus(true);
                displayService.UpdateStatus("Sending data...");

                var cloudLogger = Resolver.Services.Get<CloudLogger>();
                cloudLogger.LogEvent(1000, "environment reading", new Dictionary<string, object>()
                {
                    { "pressure", $"{e.New.Pressure.Value.Millibar:N2}" },
                    { "humidity", $"{e.New.Humidity.Value.Percent:N2}" },
                    { "temperature", $"{e.New.Temperature.Value.Celsius:N2}" }
                });

                displayService.UpdateSyncStatus(false);
                displayService.UpdateStatus("Data sent!");
            }

            hardware.RgbPwmLed.StartBlink(Color.Green);
        }

        public void Run()
        {
            hardware.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(20));
        }
    }
}