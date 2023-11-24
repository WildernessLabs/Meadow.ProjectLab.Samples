using Meadow.Hardware;
using System.Threading.Tasks;

namespace Meadow.Cloud_Command
{
    internal class MainController
    {
        int TIMEZONE_OFFSET = -8; // UTC-8

        private IMeadowCloudCommandHardware hardware;
        private IWiFiNetworkAdapter network;
        private DisplayController displayController;

        public MainController(IMeadowCloudClientHardware hardware, IWiFiNetworkAdapter network)
        {
            this.hardware = hardware;
            this.network = network;
        }

        public void Initialize()
        {
            hardware.Initialize();

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