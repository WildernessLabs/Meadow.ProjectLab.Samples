using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using MeadowConnectedSample.Connectivity;
using MeadowConnectedSample.Controller;
using MeadowConnectedSample.Models.Logical;
using MeadowConnectedSample.Views;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample
{
    // Change F7FeatherV2 to F7CoreComputeV2 for ProjectLab v3
    public class MeadowApp : App<F7FeatherV2>
    {
        bool useWifi = true;

        IProjectLabHardware projectLab;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            projectLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            LedController.Instance.Initialize(projectLab.RgbLed);
            LedController.Instance.SetColor(Color.Red);

            MainController.Instance.Initialize(projectLab);
            MainController.Instance.UseWiFi = useWifi;

            DisplayView.Instance.Initialize(projectLab.Display);
            DisplayView.Instance.ShowSplashScreen();

            _ = DisplayView.Instance.StartConnectingAnimation(useWifi);

            if (useWifi)
            {
                var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
                wifi.NetworkConnected += WifiNetworkConnected;
            }
            else
            {
                BluetoothServer.Instance.Initialize();
                LedController.Instance.SetColor(Color.Green);
                _ = MainController.Instance.StartUpdating(TimeSpan.FromSeconds(15));
            }

            return Task.CompletedTask;
        }

        private void WifiNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            DisplayView.Instance.StopConnectingAnimation();

            _ = MainController.Instance.StartUpdating(TimeSpan.FromSeconds(15));

            var mapleServer = new MapleServer(sender.IpAddress, 5417, logger: Resolver.Log);
            mapleServer.Start();

            DisplayView.Instance.ShowMapleReady(sender.IpAddress.ToString());

            LedController.Instance.SetColor(Color.Green);
        }
    }
}