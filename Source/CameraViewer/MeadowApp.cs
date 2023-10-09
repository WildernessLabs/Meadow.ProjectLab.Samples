using BitMiracle.LibJpeg;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Sensors.Camera;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Simon
{
    // Change F7CoreComputeV2 to F7FeatherV2 for ProjectLab v2
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private MicroGraphics graphics;
        private IProjectLabHardware projectLab;
        private Vc0706 camera;

        private IPixelBuffer lastImage;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            projectLab = ProjectLab.Create();

            projectLab.RgbLed.SetColor(Color.Red);

            graphics = new MicroGraphics(projectLab.Display)
            {
            };
            graphics.Clear();

            projectLab.UpButton.Clicked += UpButton_Clicked;

            camera = new Vc0706(Device, projectLab.MikroBus1.SerialPortName, 38400);

            projectLab.RgbLed.SetColor(Color.Green);

            return base.Initialize();
        }

        private async void UpButton_Clicked(object sender, EventArgs e)
        {
            Resolver.Log.Info("UpButton_Clicked");
            if (!camera.SetCaptureResolution(Vc0706.ImageResolution._320x240))
            {
                Resolver.Log.Info("Set resolution failed");
            }

            var buffer = await TakePicture();

            graphics.Clear();
            graphics.DrawBuffer(0, 0, buffer);
            graphics.Show();
        }

        async Task<IPixelBuffer> TakePicture()
        {
            Resolver.Log.Info($"Image size is {camera.GetCaptureResolution()}");

            camera.CapturePhoto();

            using var jpegStream = await camera.GetPhotoStream();

            var jpeg = new JpegImage(jpegStream);
            Resolver.Log.Info($"Image decoded - width:{jpeg.Width}, height:{jpeg.Height}");

            using MemoryStream memoryStream = new MemoryStream();
            jpeg.WriteBitmap(memoryStream);
            byte[] jpegData = memoryStream.ToArray();
            jpegData = jpegData.Skip(54).ToArray();

            var buffer = new BufferRgb888(jpeg.Width, jpeg.Height, jpegData).ConvertPixelBuffer<BufferRgb565>();
            return buffer;
        }
    }
}