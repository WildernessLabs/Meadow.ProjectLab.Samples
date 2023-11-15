using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using MeadowAzureIoTHub.Hardware;
using System.Threading.Tasks;

namespace MeadowAzureIoTHub
{
    // Change F7CoreComputeV2 to F7FeatherV2 for ProjectLab v2
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainCoordinator coordinator;

        public override async Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var hardware = new MeadowAzureIoTHubHardware();
            var network = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

            coordinator = new MainCoordinator(hardware, network);
            await coordinator.Initialize();
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            coordinator.Run();

            return Task.CompletedTask;
        }
    }
}