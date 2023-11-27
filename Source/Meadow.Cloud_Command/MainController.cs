using Meadow.Cloud_Command.Controllers;
using Meadow.Cloud_Command.Hardware;
using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Cloud_Command
{
    internal class MainController
    {
        int TIMEZONE_OFFSET = -8; // UTC-8

        private IMeadowCloudCommandHardware hardware;
        private IWiFiNetworkAdapter network;
        private DisplayController displayController;

        public MainController(IMeadowCloudCommandHardware hardware, IWiFiNetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

            displayController = new DisplayController(hardware.Display);
            displayController.ShowSplashScreen();
            Thread.Sleep(3000);
            displayController.ShowDataScreen();
        }

        public async Task Run()
        {
            while (true)
            {
                displayController.UpdateWiFiStatus(network.IsConnected);

                if (network.IsConnected)
                {
                    displayController.UpdateStatus(DateTime.Now.AddHours(TIMEZONE_OFFSET).ToString("hh:mm tt dd/MM/yy"));

                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
                else
                {
                    displayController.UpdateStatus("Offline...");

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }
    }
}