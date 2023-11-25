using Meadow;
using Meadow.Cloud_Command.Hardware;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Peripherals.Leds;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Cloud_Command
{
    // Change F7CoreComputeV2 to F7FeatherV2 (or F7FeatherV1) for Feather boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainController mainController;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var hardware = new MeadowCloudCommandHardware();
            var network = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            mainController = new MainController(hardware, network);
            mainController.Initialize();

            return Task.CompletedTask;
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            mainController.Run();

            return Task.CompletedTask;
        }
    }
}