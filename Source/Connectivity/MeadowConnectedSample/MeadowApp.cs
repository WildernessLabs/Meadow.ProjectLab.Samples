using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using MeadowConnectedSample.Connectivity;
using MeadowConnectedSample.Controller;
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

            DisplayController.Instance.Initialize(projLab.Display);
            DisplayController.Instance.ShowSplashScreen();

            Bh1750Controller.Instance.Initialize(projLab.LightSensor);

            Bme688Controller.Instance.Initialize(projLab.EnvironmentalSensor);

            if (useWiFi)
            {
                DisplayController.Instance.StartConnectingAnimation(useWiFi);

                var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
                wifi.NetworkConnected += WifiNetworkConnected;

                await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            }
            else
            {
                DisplayController.Instance.StartConnectingAnimation(useWiFi);

                BluetoothServer.Instance.Initialize();

                LedController.Instance.SetColor(Color.Green);
            }
        }

        private void WifiNetworkConnected(INetworkAdapter sender, NetworkConnectionEventArgs args)
        {
            DisplayController.Instance.StopConnectingAnimation();

            var mapleServer = new MapleServer(sender.IpAddress, 5417, true, logger: Resolver.Log);
            mapleServer.Start();

            DisplayController.Instance.ShowMapleReady(sender.IpAddress.ToString());

            LedController.Instance.SetColor(Color.Green);
        }
    }
}