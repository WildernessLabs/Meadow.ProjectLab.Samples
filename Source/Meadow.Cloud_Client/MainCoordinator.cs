using Meadow.Cloud_Client.Hardware;
using Meadow.Cloud_Client.Services;
using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Cloud_Client
{
    internal class MainCoordinator
    {
        IMeadowCloudClientHardware hardware;
        IWiFiNetworkAdapter network;
        DisplayService displayService;

        public MainCoordinator(IMeadowCloudClientHardware hardware, IWiFiNetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

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

                await Task.Delay(TimeSpan.FromMinutes(1));
            }

        }
    }
}