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
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        IProjectLabHardware projLab;

        bool useWiFi = true;

        public override async Task Initialize()
        {
            LedController.Instance.SetColor(Color.Red);

            projLab = ProjectLab.Create();

            MainController.Instance.Initialize(projLab);

            DisplayView.Instance.Initialize(projLab.Display);
            DisplayView.Instance.ShowSplashScreen();
            DisplayView.Instance.StartConnectingAnimation(useWiFi);

            if (useWiFi)
            {
                var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
                wifi.NetworkConnected += WifiNetworkConnected;

                await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            }
            else
            {
                BluetoothServer.Instance.Initialize();

                LedController.Instance.SetColor(Color.Green);
            }
        }

        private void WifiNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            DisplayView.Instance.StopConnectingAnimation();

            MainController.Instance.StartUpdating(TimeSpan.FromSeconds(15));

            var mapleServer = new MapleServer(sender.IpAddress, 5417, true, logger: Resolver.Log);
            mapleServer.Start();

            DisplayView.Instance.ShowMapleReady(sender.IpAddress.ToString());

            LedController.Instance.SetColor(Color.Green);
        }
    }
}