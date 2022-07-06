using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace AmbientRoomMonitor
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        Color[] colors = new Color[4]
        {
            Color.FromHex("#67E667"),
            Color.FromHex("#00CC00"),
            Color.FromHex("#269926"),
            Color.FromHex("#008500")
        };

        MicroGraphics graphics;
        Bme680 bme;
        
        public override Task Initialize()
        {
            var onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            bme = new Bme680(Device.CreateI2cBus(), (byte)Bme680.Addresses.Address_0x76);
            bme.Updated += Bme680_Updated;

            var config = new SpiClockConfiguration(
                 speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                 mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
                clock: Device.Pins.SCK,
                copi: Device.Pins.MOSI,
                cipo: Device.Pins.MISO,
                config: config);
            var st7789 = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.A03,
                dcPin: Device.Pins.A04,
                resetPin: Device.Pins.A05,
                width: 240, 
                height: 240,
                displayColorMode: ColorType.Format16bppRgb565);

            graphics = new MicroGraphics(st7789) 
            { 
                IgnoreOutOfBoundsPixels = true 
            };
            graphics.Rotation = RotationType._90Degrees;

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }

        void Bme680_Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> e)
        {
            graphics.DrawRectangle(
                x: 150, y: 145,
                width: 84,
                height: 68,
                color: colors[colors.Length - 1],
                filled: true);
            
            graphics.DrawText(186, 145, $"{(int)e.New.Temperature.Value.Celsius}°C", Color.White);
            graphics.DrawText(150, 168, $"{(int)e.New.Pressure.Value.Millibar}mbar", Color.White);
            graphics.DrawText(198, 193, $"{(int)e.New.Humidity.Value.Percent}%", Color.White);
            
            graphics.Show();
        }

        void LoadScreen()
        {
            graphics.Clear(true);

            int radius = 225;
            int originX = graphics.Width / 2;
            int originY = graphics.Height / 2 + 130;

            graphics.Stroke = 3;
            for (int i = 1; i < 5; i++)
            {
                graphics.DrawCircle
                (
                    centerX: originX,
                    centerY: originY,
                    radius: radius,
                    color: colors[i - 1],
                    filled: true
                );
                radius -= 20;
            }

            graphics.DrawLine(0, 220, 239, 220, Color.White);
            graphics.DrawLine(0, 230, 239, 230, Color.White);

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText(6, 145, "TEMPERATURE", Color.White);
            graphics.DrawText(6, 169, "PRESSURE", Color.White);
            graphics.DrawText(6, 193, "HUMIDITY", Color.White);

            graphics.Show();
        }

        public override Task Run()
        {
            LoadScreen();

            bme.StartUpdating(TimeSpan.FromSeconds(5));

            return base.Run();
        }
    }
}