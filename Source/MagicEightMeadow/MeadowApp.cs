using MagicEightMeadow.Hardware;
using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace MagicEightMeadow
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MainCoordinator coordinator;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var hardware = new MagicEightMeadowHardware();

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