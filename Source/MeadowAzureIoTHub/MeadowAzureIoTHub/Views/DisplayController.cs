using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Units;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowAzureIoTHub.Views
{
    public class DisplayController
    {
        private static readonly Lazy<DisplayController> instance =
            new Lazy<DisplayController>(() => new DisplayController());
        public static DisplayController Instance => instance.Value;

        static Color backgroundColor = Color.FromHex("#23ABE3");
        static Color foregroundColor = Color.Black;

        CancellationTokenSource token;

        protected BufferRgb888 imgConnecting, imgConnected, imgRefreshing, imgRefreshed;
        protected MicroGraphics graphics;

        private DisplayController() { }

        public void Initialize(IGraphicsDisplay display)
        {
            imgConnected = LoadJpeg("img_wifi_connected.jpg");
            imgConnecting = LoadJpeg("img_wifi_connecting.jpg");
            imgRefreshing = LoadJpeg("img_refreshing.jpg");
            imgRefreshed = LoadJpeg("img_refreshed.jpg");

            graphics = new MicroGraphics(display)
            {
                CurrentFont = new Font8x12(),
                Stroke = 3,
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

        protected void DrawBackground()
        {
            var logo = LoadJpeg("img_meadow.jpg");

            graphics.Clear(backgroundColor);

            graphics.DrawBuffer(
                x: graphics.Width / 2 - logo.Width / 2,
                y: 63,
                buffer: logo);

            graphics.DrawText(graphics.Width / 2, 160, "Azure IoT Hub", Color.Black, alignmentH: HorizontalAlignment.Center);
        }

        protected byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MeadowAzureIoTHub.{filename}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        public void ShowSplashScreen()
        {
            DrawBackground();

            graphics.Show();
        }

        public async Task ShowConnectingAnimation()
        {
            token = new CancellationTokenSource();

            UpdateStatus("Connecting");

            bool alternateImg = false;
            while (!token.IsCancellationRequested)
            {
                alternateImg = !alternateImg;

                graphics.DrawBuffer(graphics.Width - imgConnected.Width - 10, 17, alternateImg ? imgConnecting : imgConnected);
                graphics.Show();

                await Task.Delay(500);
            }
        }

        public void ShowConnected()
        {
            token.Cancel();

            graphics.Clear(backgroundColor);

            graphics.DrawBuffer(graphics.Width - imgConnected.Width - 10, 17, imgConnected);

            graphics.DrawBuffer(10, 17, imgRefreshed);

            UpdateStatus("Connected");

            graphics.DrawText(10, 78, "TEMP.", foregroundColor, ScaleFactor.X2, HorizontalAlignment.Left);

            graphics.DrawText(10, 138, "HUMID.", foregroundColor, ScaleFactor.X2, HorizontalAlignment.Left);

            graphics.DrawText(10, 198, "PRESS.", foregroundColor, ScaleFactor.X2, HorizontalAlignment.Left);

            graphics.Show();
        }

        public async Task StartSyncCompletedAnimation((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) reading)
        {
            UpdateStatus("Syncing");
            graphics.DrawBuffer(10, 18, imgRefreshing);
            graphics.Show();
            await Task.Delay(TimeSpan.FromSeconds(1));

            graphics.DrawRectangle(graphics.Width / 2, 78, graphics.Width / 2, 24, backgroundColor, true);
            graphics.DrawText(graphics.Width - 10, 78, $"{reading.Temperature.Value.Celsius:N1}°C", foregroundColor, ScaleFactor.X2, HorizontalAlignment.Right);

            graphics.DrawRectangle(graphics.Width / 2, 138, graphics.Width / 2, 24, backgroundColor, true);
            graphics.DrawText(graphics.Width - 10, 138, $"{reading.Humidity.Value.Percent:N2}%", foregroundColor, ScaleFactor.X2, HorizontalAlignment.Right);

            graphics.DrawRectangle(graphics.Width / 2, 198, graphics.Width / 2, 24, backgroundColor, true);
            graphics.DrawText(graphics.Width - 10, 198, $"{reading.Pressure.Value.StandardAtmosphere:N1}Atm", foregroundColor, ScaleFactor.X2, HorizontalAlignment.Right);

            UpdateStatus("Done");
            graphics.DrawBuffer(10, 18, imgRefreshed);
            graphics.Show();
        }

        protected void UpdateStatus(string status)
        {
            graphics.DrawRectangle((graphics.Width - 150) / 2, 18, 150, 24, backgroundColor, true);
            graphics.DrawText(graphics.Width / 2, 18, $"{status}", foregroundColor, ScaleFactor.X2, HorizontalAlignment.Center);
        }
    }
}