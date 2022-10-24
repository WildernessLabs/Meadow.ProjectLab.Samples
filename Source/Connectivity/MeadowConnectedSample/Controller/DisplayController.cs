using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Hardware;
using Meadow.Units;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowConnectedSample.Controller
{
    public class DisplayController
    {
        private static readonly Lazy<DisplayController> instance =
            new Lazy<DisplayController>(() => new DisplayController());
        public static DisplayController Instance => instance.Value;

        static Color backgroundColor = Color.FromHex("#23ABE3");

        CancellationTokenSource token;

        protected BufferRgb888 imgConnecting, imgConnected;
        protected MicroGraphics graphics;

        private DisplayController()
        {
            Initialize();
        }

        public void Initialize()
        {
            var config = new SpiClockConfiguration(
                speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = MeadowApp.Device.CreateSpiBus(
                clock: MeadowApp.Device.Pins.SCK,
                copi: MeadowApp.Device.Pins.MOSI,
                cipo: MeadowApp.Device.Pins.MISO,
                config: config);
            var st7789 = new St7789
            (
                device: MeadowApp.Device,
                spiBus: spiBus,
                chipSelectPin: MeadowApp.Device.Pins.A03,
                dcPin: MeadowApp.Device.Pins.A04,
                resetPin: MeadowApp.Device.Pins.A05,
                width: 240,
                height: 240,
                colorMode: ColorType.Format16bppRgb565
            );

            graphics = new MicroGraphics(st7789)
            {
                CurrentFont = new Font12x20(),
                Stroke = 3,
                Rotation = RotationType._90Degrees
            };

            graphics.Clear(true);
        }

        BufferRgb888 LoadJpeg(string fileName)
        {
            var jpgData = LoadResource(fileName);
            var decoder = new JpegDecoder();
            decoder.DecodeJpeg(jpgData);
            return new BufferRgb888(decoder.Width, decoder.Height, decoder.GetImageData());
        }

        void DrawBackground()
        {
            var logo = LoadJpeg("img_meadow.jpg");

            graphics.Clear(backgroundColor);

            graphics.DrawBuffer(
                x: graphics.Width / 2 - logo.Width / 2,
                y: 34,
                buffer: logo);

            graphics.DrawCircle(
                centerX: graphics.Width / 2,
                centerY: graphics.Height / 2,
                radius: (graphics.Width / 2) - 10,
                color: Color.Black,
                filled: false);
        }

        public void ShowSplashScreen()
        {
            DrawBackground();

            graphics.Show();
        }

        protected byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MeadowConnectedSample.{filename}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public async Task StartConnectingAnimation(bool isWiFi) 
        {            
            imgConnected = LoadJpeg(isWiFi? "img_wifi_connected.jpg" : "img_ble_paired.jpg");
            imgConnecting = LoadJpeg(isWiFi? "img_wifi_connecting.jpg" : "img_ble_pairing.jpg");

            token = new CancellationTokenSource();

            graphics.DrawRectangle(44, 132, 146, 63, backgroundColor, true);


            bool alternateImg = false;
            while (!token.IsCancellationRequested)
            {
                alternateImg = !alternateImg;

                graphics.DrawBuffer(
                    x: graphics.Width / 2 - imgConnecting.Width / 2,
                    y: 134,
                    buffer: alternateImg ? imgConnecting : imgConnected);
                graphics.Show();

                await Task.Delay(500);
            }
        }

        public void StopConnectingAnimation() 
        {
            token.Cancel();
        }

        public void ShowMapleReady(string ipAddress) 
        {
            graphics.DrawRectangle(77, 134, 86, 74, backgroundColor, true);

            graphics.CurrentFont = new Font12x16();
            graphics.DrawText(120, 128, "MAPLE", ScaleFactor.X2, TextAlignment.Center);

            graphics.DrawText(120, 171, $"{ipAddress}", ScaleFactor.X1, TextAlignment.Center);

            graphics.DrawText(120, 197, $"READY", ScaleFactor.X1, TextAlignment.Center);

            graphics.Show();
        }

        public void ShowBluetoothPaired() 
        {
            StopConnectingAnimation();

            graphics.DrawRectangle(77, 134, 86, 74, backgroundColor, true);

            graphics.CurrentFont = new Font12x16();
            graphics.DrawText(120, 132, "BLUETOOTH", ScaleFactor.X1, TextAlignment.Center);

            graphics.DrawText(120, 163, "PAIRED", ScaleFactor.X2, TextAlignment.Center);

            graphics.Show();
        }
    }
}