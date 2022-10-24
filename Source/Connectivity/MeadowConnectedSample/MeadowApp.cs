using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using MeadowConnectedSample.Connectivity;
using MeadowConnectedSample.Controller;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV1>
    {
        bool useWiFi = true;

        public override async Task Initialize() 
        {
            LedController.Instance.SetColor(Color.Red);

            DisplayController.Instance.ShowSplashScreen();

            var i2c = Device.CreateI2cBus();
            Bh1750Controller.Instance.Initialize(i2c);
            Bme688Controller.Instance.Initialize(i2c);

            if (useWiFi)
            {
                DisplayController.Instance.StartConnectingAnimation(useWiFi);

                var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

                var connectionResult = await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));
                if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
                {
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }

                DisplayController.Instance.StopConnectingAnimation();

                var mapleServer = new MapleServer(wifi.IpAddress, 5417, false);
                mapleServer.Start();

                DisplayController.Instance.ShowMapleReady(wifi.IpAddress.ToString());
            }
            else
            {
                DisplayController.Instance.StartConnectingAnimation(useWiFi);

                BluetoothServer.Instance.Initialize();
            }

            LedController.Instance.SetColor(Color.Green);
        }
    }
}