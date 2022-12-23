using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Hardware;
using MeadowConnectedSample.Connectivity;
using MeadowConnectedSample.Controller;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MeadowConnectedSample
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        ProjectLab projLab;

        bool useWiFi = true;

        public override async Task Initialize() 
        {
            LedController.Instance.SetColor(Color.Red);

            projLab = new ProjectLab();

            DisplayController.Instance.Initialize(projLab.Display);
            DisplayController.Instance.ShowSplashScreen();

            Bh1750Controller.Instance.Initialize(projLab.LightSensor);

            Bme688Controller.Instance.Initialize(projLab.EnvironmentalSensor);

            if (useWiFi)
            {
                DisplayController.Instance.StartConnectingAnimation(useWiFi);

                try
                {
                    var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

                    await wifi.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD, TimeSpan.FromSeconds(45));

                    Console.WriteLine($"IP Address... {wifi.IpAddress}");

                    DisplayController.Instance.StopConnectingAnimation();

                    var mapleServer = new MapleServer(wifi.IpAddress, 5417, true, logger: Resolver.Log);
                    mapleServer.Start();

                    DisplayController.Instance.ShowMapleReady(wifi.IpAddress.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
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