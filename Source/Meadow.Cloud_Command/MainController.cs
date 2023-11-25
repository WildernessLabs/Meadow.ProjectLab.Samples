using Meadow.Cloud_Command.Hardware;
using Meadow.Cloud_Command.Controllers;
using Meadow.Hardware;
using System.Threading.Tasks;
using System.Threading;

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

        public Task Run()
        {
            //while (true)
            //{

            //}

            return Task.CompletedTask;
        }
    }
}