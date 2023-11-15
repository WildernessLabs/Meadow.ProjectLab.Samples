using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Threading.Tasks;
using WifiWeather.Hardware;

namespace WifiWeather
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainCoordinator coordinator;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var hardware = new WifiWeatherHardware();
            var network = Device.NetworkAdapters.Primary<INetworkAdapter>();

            coordinator = new MainCoordinator(hardware, network);
            coordinator.Initialize();

            return Task.CompletedTask;
        }

        public override async Task Run()
        {
            Resolver.Log.Info("Run...");

            await coordinator.Run();
        }
    }
}