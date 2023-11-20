using Meadow.Cloud_Client.Hardware;
using Meadow.Cloud_Client.Services;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Cloud_Client
{
    internal class MainCoordinator
    {
        IMeadowCloudClientHardware hardware;
        IWiFiNetworkAdapter network;
        CloudService cloudService;
        DisplayService displayService;

        public MainCoordinator(IMeadowCloudClientHardware hardware, IWiFiNetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

            cloudService = new CloudService();
            displayService = new DisplayService(hardware.Display);

            displayService.ShowSplashScreen();
            Thread.Sleep(3000);
            displayService.ShowDataScreen();
        }

        public async Task Run()
        {
            int TIMEZONE_OFFSET = -8; // UTC-8

            while (true)
            {
                displayService.UpdateWiFiStatus(network.IsConnected);
                displayService.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("dd/MM/yy hh:mm tt"));

                if (network.IsConnected) 
                {
                    displayService.UpdateSyncStatus(true);

                    var readings = await cloudService.GetSensorReadings();

                    var temps = new List<double>();

                    if (readings != null && readings.Count > 0)
                    {
                        foreach (var reading in readings)
                        {
                            temps.Add(reading.record.measurements.temperature);
                        }

                        displayService.GraphData(temps);
                    }

                    displayService.UpdateSyncStatus(false);
                }

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}