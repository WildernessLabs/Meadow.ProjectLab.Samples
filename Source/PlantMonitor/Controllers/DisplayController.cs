using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using SimpleJpegDecoder;
using System.IO;
using System.Reflection;
using Meadow.Units;
using Meadow.Hardware;
using System;

namespace PlantMonitor.Controllers
{
    public class DisplayController
    {
        private static readonly Lazy<DisplayController> instance =
            new Lazy<DisplayController>(() => new DisplayController());
        public static DisplayController Instance => instance.Value;

        MicroGraphics graphics;

        private DisplayController() { }

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
            var display = new St7789
            (
                device: MeadowApp.Device,
                spiBus: spiBus,
                chipSelectPin: MeadowApp.Device.Pins.A03,
                dcPin: MeadowApp.Device.Pins.A04,
                resetPin: MeadowApp.Device.Pins.A05,
                width: 240,
                height: 240,
                displayColorMode: ColorType.Format16bppRgb565
            );

            graphics = new MicroGraphics(display)
            {
                IgnoreOutOfBoundsPixels = true,
                Rotation = RotationType._90Degrees,
                CurrentFont = new Font12x20(),
                Stroke = 3,
            };

            graphics.Clear();
            graphics.DrawRectangle(0, 0, graphics.Width, graphics.Height, Color.White, true);
            string plant = "Plant";
            string monitor = "Monitor";
            graphics.CurrentFont = new Font12x16();
            graphics.DrawText((240 - plant.Length * 24) / 2, 80, plant, Color.Black, ScaleFactor.X2);
            graphics.DrawText((240 - monitor.Length * 24) / 2, 130, monitor, Color.Black, ScaleFactor.X2);
            graphics.Show();
        }

        void RefreshMoistureImage(int percentage)
        {
            if (percentage >= 0 && percentage <= 25)
            {
                DrawImage(0, 42, 10);
            }
            else if (percentage > 25 && percentage <= 50)
            {
                DrawImage(1, 28, 4);
            }
            else if (percentage > 50 && percentage <= 75)
            {
                DrawImage(2, 31, 5);
            }
            else if (percentage > 75)
            {
                DrawImage(3, 35, 5);
            }

            graphics.Show();
        }

        void RefreshMoisturePercentage(int percentage)
        {
            graphics.DrawRectangle(0, 208, 96, 32, Color.White, true);
            graphics.DrawText(0, 208, $"{percentage}%", Color.Black, ScaleFactor.X2);
            graphics.Show();
        }

        void DrawImage(int index, int xOffSet, int yOffSet)
        {
            var jpgData = LoadResource($"level_{index}.jpg");
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int x = 0;
            int y = 0;
            byte r, g, b;

            graphics.DrawRectangle(0, 0, 240, 208, Color.White, true);

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                graphics.DrawPixel(x + xOffSet, y + yOffSet, Color.FromRgb(r, g, b));

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
            var resourceName = $"PlantMonitor.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public void Update(int percentage)
        {
            RefreshMoistureImage(percentage);
            RefreshMoisturePercentage(percentage);
        }

        public void UpdateTemperatureValue(Temperature newValue, Temperature oldValue)
        {
            string t = $"{(int)oldValue.Celsius}C";
            graphics.DrawText(240 - t.Length * 24, 208, t, Color.White, ScaleFactor.X2);

            t = $"{(int)newValue.Celsius}C";
            graphics.DrawText(240 - t.Length * 24, 208, t, Color.Black, ScaleFactor.X2);

            graphics.Show();
        }
    }
}