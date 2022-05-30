using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using Meadow.Units;
using System.Threading.Tasks;

namespace MoistureMeter.Controllers
{
    public class DisplayController
    {
        MicroGraphics graphics;

        public DisplayController()
        {
            Initialize();
        }

        void Initialize()
        {
            var config = new SpiClockConfiguration(
                new Frequency(48000, Frequency.UnitType.Kilohertz),
                SpiClockConfiguration.Mode.Mode3);
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
                Stroke = 1,
                CurrentFont = new Font12x20(),
                Rotation = RotationType._90Degrees
            };
            graphics.Clear();
        }

        public void UpdatePercentage(int percentage) 
        {
            graphics.Clear();

            Color color = Color.FromHex("004B6B");

            graphics.DrawRectangle(12, 12, 100, 218, color, true);

            int percentageGraph = (int)(percentage == 100 ? 9 : percentage / 10);
            for (int i = percentageGraph; i >= 0; i--) 
            {
                switch (i) 
                {
                    case 0:
                    case 1:
                        color = Color.FromHex("FF3535");
                        break;
                    case 2:
                    case 3:
                    case 4:
                        color = Color.FromHex("FF8251");
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        color = Color.FromHex("35FF3D");
                        break;
                    case 9:
                        color = Color.FromHex("475AFF");
                        break;
                }

                graphics.DrawRectangle(12, 222 - (22*i + 12), 100, 20, color, true);
            }

            graphics.DrawText(174, 105, $"{percentage}%", ScaleFactor.X2, TextAlignment.Center);

            graphics.Show();
        }
    }
}