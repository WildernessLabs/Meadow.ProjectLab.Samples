using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;
using MeadowConnectedSample.Connectivity;
using MeadowConnectedSample.Controller;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample
{
    // public class MeadowApp : App<F7FeatherV1, MeadowApp> <- If you have a Meadow F7v1.*
    public class MeadowApp : App<F7FeatherV2, MeadowApp>
    {
        public MeadowApp()
        {
            Initialize();
        }

        void Initialize() 
        {
            LedController.Instance.SetColor(Color.Red);

            DisplayController.Instance.ShowSplashScreen();

            var i2c = Device.CreateI2cBus();
            Bh1750Controller.Instance.Initialize(i2c);
            Bme688Controller.Instance.Initialize(i2c);

            InitializeBluetooth();
            //InitializeMaple().Wait();

            LedController.Instance.SetColor(Color.Green);
        }

        void InitializeBluetooth()
        {
            DisplayController.Instance.StartConnectingAnimation(isWiFi: false);

            BluetoothServer.Instance.Initialize();
        }

        async Task InitializeMaple()
        {
            DisplayController.Instance.StartConnectingAnimation(isWiFi: true);
            
            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }

            DisplayController.Instance.StopConnectingAnimation();

            MapleServer mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);
            mapleServer.Start();

            DisplayController.Instance.ShowMapleReady();
        }
    }
}