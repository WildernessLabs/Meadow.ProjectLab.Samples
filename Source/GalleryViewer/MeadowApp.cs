using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using Meadow.Units;
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
        RgbPwmLed led;
        MicroGraphics graphics;
        PushButton buttonLeft;
        PushButton buttonRight;
        int selectedIndex;
        string[] images = new string[3] { "image1.jpg", "image2.jpg", "image3.jpg" };

        public override Task Initialize()
        {
            led = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            led.SetColor(Color.Red);

            buttonLeft = new PushButton(Device, Device.Pins.D10, ResistorMode.InternalPullDown);
            buttonLeft.Clicked += ButtonUpClicked;

            buttonRight = new PushButton(Device, Device.Pins.D05, ResistorMode.InternalPullDown);
            buttonRight.Clicked += ButtonDownClicked;

            var config = new SpiClockConfiguration(
                speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
                clock: Device.Pins.SCK,
                copi: Device.Pins.MOSI,
                cipo: Device.Pins.MISO,
                config: config);
            var display = new St7789
            (
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.A03,
                dcPin: Device.Pins.A04,
                resetPin: Device.Pins.A05,
                width: 240, 
                height: 240, 
                displayColorMode: ColorType.Format16bppRgb565
            );

            graphics = new MicroGraphics(display);
            graphics.Rotation = RotationType._90Degrees;

            led.SetColor(Color.Green);

            return base.Initialize();
        }

        void ButtonUpClicked(object sender, EventArgs e)
        {
            led.SetColor(Color.Red);

            if (selectedIndex + 1 > 2)
                selectedIndex = 0;
            else
                selectedIndex++;

            DisplayJPG();

            led.SetColor(Color.Green);
        }

        void ButtonDownClicked(object sender, EventArgs e)
        {
            led.SetColor(Color.Red);

            if (selectedIndex - 1 < 0)
                selectedIndex = 2;
            else
                selectedIndex--;

            DisplayJPG();

            led.SetColor(Color.Green);
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