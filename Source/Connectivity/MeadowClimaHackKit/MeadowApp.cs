using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Gateway.WiFi;
using MeadowClimaHackKit.Connectivity;
using MeadowClimaHackKit.Controller;
using System;
using System.Threading.Tasks;

namespace MeadowClimaHackKit
{
    // public class MeadowApp : App<F7Micro, MeadowApp> <- If you have a Meadow F7v1.*
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        public MeadowApp()
        {
            Initialize();
        }

        void Initialize() 
        {
            LedController.Instance.SetColor(Color.Red);

            DisplayController.Instance.ShowSplashScreen();

            //InitializeBluetooth();
            InitializeMaple().Wait();
        }

        void InitializeBluetooth()
        {
            BluetoothServer.Instance.Initialize();
        }

        async Task InitializeMaple()
        {
            DisplayController.Instance.StartWifiConnectingAnimation();
            
            var result = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (result.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {result.ConnectionStatus}");
            }

            DisplayController.Instance.StopWifiConnectingAnimation();

            MapleServer mapleServer = new MapleServer(Device.WiFiAdapter.IpAddress, 5417, false);
            mapleServer.Start();

            Bme688Controller.Instance.Initialize();

            DisplayController.Instance.ShowMapleReady();

            LedController.Instance.SetColor(Color.Green);
        }
    }
}