using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Leds;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GalleryViewer
{
    // Change F7CoreComputeV2 to F7FeatherV2 for ProjectLab v2
    public class MeadowApp : App<F7CoreComputeV2>
    {
        RgbPwmLed onboardLed;
        MicroGraphics graphics;
        int selectedIndex = 0;
        string[] images = new string[3] { "GalleryViewer.image1.jpg", "GalleryViewer.image2.jpg", "GalleryViewer.image3.jpg" };

        IProjectLabHardware projectLab;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            projectLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            onboardLed = projectLab.RgbLed;
            onboardLed.SetColor(Color.Red);

            projectLab.RightButton.Clicked += ButtonRightClicked;
            projectLab.LeftButton.Clicked += ButtonLeftClicked;

            graphics = new MicroGraphics(projectLab.Display)
            {
                IgnoreOutOfBoundsPixels = true
            };

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }

        void ButtonLeftClicked(object sender, EventArgs e)
        {
            onboardLed.SetColor(Color.Red);

            if (selectedIndex + 1 > 2)
                selectedIndex = 0;
            else
                selectedIndex++;

            DrawImage();

            onboardLed.SetColor(Color.Green);
        }

        void ButtonRightClicked(object sender, EventArgs e)
        {
            onboardLed.SetColor(Color.Red);

            if (selectedIndex - 1 < 0)
                selectedIndex = 2;
            else
                selectedIndex--;

            DrawImage();

            onboardLed.SetColor(Color.Green);
        }

        IPixelBuffer LoadJpeg(byte[] jpgData)
        {
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            return new BufferRgb888(decoder.Width, decoder.Height, jpg);
        }

        byte[] LoadResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        void DrawImage()
        {
            var buffer = LoadJpeg(LoadResource(images[selectedIndex]));
            graphics.DrawBuffer((graphics.Width - buffer.Width) / 2, 0, buffer);
            graphics.Show();
        }

        public override Task Run()
        {
            DrawImage();

            return base.Run();
        }
    }
}