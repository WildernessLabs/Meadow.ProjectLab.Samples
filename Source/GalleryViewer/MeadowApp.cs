using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Leds;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace GalleryViewer
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        RgbPwmLed onboardLed;
        MicroGraphics graphics;
        int selectedIndex;
        string[] images = new string[3] { "image1.jpg", "image2.jpg", "image3.jpg" };

        ProjectLab projLab;

        public override Task Initialize()
        {
            projLab = new ProjectLab();

            Resolver.Log.Info($"Running on ProjectLab Hardware {projLab.RevisionString}");

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                CommonType.CommonAnode);
            onboardLed.SetColor(Color.Red);

            projLab.RightButton.Clicked += ButtonRightClicked;
            projLab.LeftButton.Clicked += ButtonLeftClicked;

            graphics = new MicroGraphics(projLab.Display);
            graphics.Rotation = RotationType._90Degrees;

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

            DisplayJPG();

            onboardLed.SetColor(Color.Green);
        }

        void ButtonRightClicked(object sender, EventArgs e)
        {
            onboardLed.SetColor(Color.Red);

            if (selectedIndex - 1 < 0)
                selectedIndex = 2;
            else
                selectedIndex--;

            DisplayJPG();

            onboardLed.SetColor(Color.Green);
        }

        void DisplayJPG()
        {
            var jpgData = LoadResource(images[selectedIndex]);
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int x = 0;
            int y = 0;
            byte r, g, b;

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                graphics.DrawPixel(x, y, Color.FromRgb(r, g, b));

                x++;
                if (x % decoder.Width == 0)
                {
                    y++;
                    x = 0;
                }
            }

            graphics.Show();
        }

        byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"GalleryViewer.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public override Task Run()
        {
            DisplayJPG();

            return base.Run();
        }
    }
}