using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowConnectedSample.Views
{
    public class DisplayView
    {
        private static readonly Lazy<DisplayView> instance =
            new Lazy<DisplayView>(() => new DisplayView());
        public static DisplayView Instance => instance.Value;

        static Color backgroundColor = Color.FromHex("#23ABE3");

        CancellationTokenSource token;

        protected BufferRgb888 imgConnecting, imgConnected;
        protected MicroGraphics graphics;

        private DisplayView() { }

        public void Initialize(IGraphicsDisplay display)
        {
            graphics = new MicroGraphics(display)
            {
                CurrentFont = new Font12x20(),
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
                radius: graphics.Width / 2 - 10,
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
            imgConnected = LoadJpeg(isWiFi ? "img_wifi_connected.jpg" : "img_ble_paired.jpg");
            imgConnecting = LoadJpeg(isWiFi ? "img_wifi_connecting.jpg" : "img_ble_pairing.jpg");

            token = new CancellationTokenSource();

            graphics.DrawRectangle(
                x: (graphics.Width - 140) / 2,
                y: 115,
                width: 140,
                height: 85,
                color: backgroundColor,
                filled: true);

            bool alternateImg = false;
            while (!token.IsCancellationRequested)
            {
                alternateImg = !alternateImg;

                graphics.DrawBuffer(
                    x: graphics.Width / 2 - imgConnecting.Width / 2,
                    y: 130,
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
            graphics.DrawRectangle(
                x: (graphics.Width - 140) / 2,
                y: 120,
                width: 140,
                height: 85,
                color: backgroundColor,
                filled: true);

            graphics.CurrentFont = new Font12x16();
            graphics.DrawText(
                x: graphics.Width / 2,
                y: 128,
                text: "MAPLE",
                color: Color.Black,
                scaleFactor: ScaleFactor.X2,
                alignmentH: HorizontalAlignment.Center);

            graphics.DrawText(
                x: graphics.Width / 2,
                y: 171,
                text: $"{ipAddress}",
                color: Color.Black,
                scaleFactor: ScaleFactor.X1,
                alignmentH: HorizontalAlignment.Center);

            graphics.DrawText(
                x: graphics.Width / 2,
                y: 197,
                text: $"READY",
                color: Color.Black,
                scaleFactor: ScaleFactor.X1,
                alignmentH: HorizontalAlignment.Center);

            graphics.Show();
        }

        public void ShowBluetoothPaired()
        {
            StopConnectingAnimation();

            graphics.DrawRectangle(
                x: (graphics.Width - 140) / 2,
                y: 120,
                width: 140,
                height: 85,
                color: backgroundColor,
                filled: true);

            graphics.CurrentFont = new Font12x16();
            graphics.DrawText(
                x: graphics.Width / 2,
                y: 130,
                text: "BLUETOOTH",
                color: Color.Black,
                scaleFactor: ScaleFactor.X1,
                alignmentH: HorizontalAlignment.Center);

            graphics.DrawText(
                x: graphics.Width / 2,
                y: 160,
                text: "PAIRED",
                color: Color.Black,
                scaleFactor: ScaleFactor.X2,
                alignmentH: HorizontalAlignment.Center);

            graphics.Show();
        }
    }
}