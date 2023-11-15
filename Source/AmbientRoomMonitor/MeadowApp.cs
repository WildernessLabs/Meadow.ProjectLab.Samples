using AmbientRoomMonitor.Hardware;
using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace AmbientRoomMonitor
{
    // Change F7CoreComputeV2 to F7FeatherV2 for ProjectLab v2
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainCoordinator coordinator;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var hardware = new AmbientRoomMonitorHardware();

            coordinator = new MainCoordinator(hardware);
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
}