using Meadow.Cloud_Logging.Controllers;
using Meadow.Cloud_Logging.Hardware;
using Meadow.Foundation;
using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Meadow.Cloud_Logging
{
    internal class MainController
    {
        int TIMEZONE_OFFSET = -8; // UTC-8

        IMeadowCloudLoggingHardware hardware;
        IWiFiNetworkAdapter network;
        DisplayController displayController;

        public MainController(IMeadowCloudLoggingHardware hardware, IWiFiNetworkAdapter network)
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

            displayController = new DisplayController(hardware.Display);
            displayController.ShowSplashScreen();
            Thread.Sleep(3000);
            displayController.ShowDataScreen();

            hardware.EnvironmentalSensor.Updated += EnvironmentalSensorUpdated;
        }

        private void EnvironmentalSensorUpdated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            displayController.UpdateWiFiStatus(network.IsConnected);

            hardware.RgbPwmLed.StartBlink(Color.Orange);

            displayController.UpdateAtmosphericConditions(
                pressure: $"{e.New.Pressure.Value.Millibar:N0}",
                humidity: $"{e.New.Humidity.Value.Percent:N0}",
                temperature: $"{e.New.Temperature.Value.Celsius:N0}");

            if (network.IsConnected)
            {
                displayController.UpdateSyncStatus(true);
                displayController.UpdateStatus("Sending data...");
                Thread.Sleep(2000);

                var cloudLogger = Resolver.Services.Get<CloudLogger>();
                cloudLogger.LogEvent(1000, "environment reading", new Dictionary<string, object>()
                {
                    { "pressure", $"{e.New.Pressure.Value.Millibar:N2}" },
                    { "humidity", $"{e.New.Humidity.Value.Percent:N2}" },
                    { "temperature", $"{e.New.Temperature.Value.Celsius:N2}" }
                });

                displayController.UpdateSyncStatus(false);
                displayController.UpdateStatus("Data sent!");
                Thread.Sleep(2000);

                displayController.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("dd/MM/yy hh:mm tt"));
            }
            else
            {
                displayController.UpdateStatus("Offline...");
            }

            hardware.RgbPwmLed.StartBlink(Color.Green);
        }

        public void Run()
        {
            hardware.EnvironmentalSensor.StartUpdating(TimeSpan.FromMinutes(30));
        }
    }
}