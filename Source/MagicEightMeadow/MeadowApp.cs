using MagicEightMeadow.Hardware;
using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace MagicEightMeadow
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        MagicEightMeadowCoordinator magicEightMeadow;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            var hardware = new MagicEightMeadowHardware();

            magicEightMeadow = new MagicEightMeadowCoordinator(hardware);
            magicEightMeadow.Initialize();

            return Task.CompletedTask;
        }

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            magicEightMeadow.Run();

            return Task.CompletedTask;
        }
    }
}