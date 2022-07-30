using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Gateway.WiFi;
using MeadowConnectedSample.Connectivity;
using MeadowConnectedSample.Controller;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        bool useBluetooth = true;

        public override async Task Initialize() 
        {
            LedController.Instance.SetColor(Color.Red);

            DisplayController.Instance.ShowSplashScreen();

            var i2c = Device.CreateI2cBus();
            Bh1750Controller.Instance.Initialize(i2c);
            Bme688Controller.Instance.Initialize(i2c);

            if (useBluetooth)
            {
                DisplayController.Instance.StartConnectingAnimation(isWiFi: false);

                BluetoothServer.Instance.Initialize();
            }
            else
            {
                DisplayController.Instance.StartConnectingAnimation(isWiFi: true);

                var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
                if (result.ConnectionStatus != ConnectionStatus.Success)
                {
                    throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
                }

                DisplayController.Instance.StopConnectingAnimation();
            }

            LedController.Instance.SetColor(Color.Green);
        }

        public override Task Run()
        {
            if (useBluetooth)
            {
                
            }
            else 
            {
                var mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);
                mapleServer.Start();

                DisplayController.Instance.ShowMapleReady();
            }

            return base.Run();
        }
    }
}