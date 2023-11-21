using Meadow.Cloud_Client.Hardware;
using Meadow.Devices;
using Meadow.Hardware;
using System.Threading.Tasks;

namespace Meadow.Cloud_Client;

// Change F7CoreComputeV2 to F7FeatherV2 (or F7FeatherV1) for Feather boards
public class MeadowApp : App<F7CoreComputeV2>
{
    MainCoordinator coordinator;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        var hardware = new MeadowCloudClientHardware();
        var network = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

        coordinator = new MainCoordinator(hardware, network);
        coordinator.Initialize();

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        coordinator.Run();

        return Task.CompletedTask;
    }
}