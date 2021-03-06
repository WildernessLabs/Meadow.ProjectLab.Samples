using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnalogClockFace
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2, MeadowApp>
    {
        readonly Color WatchBackgroundColor = Color.White;

        MicroGraphics graphics;
        int tick;

        public MeadowApp()
        {
            Initialize().Wait();

            DrawClock();
        }

        async Task Initialize()
        {
            var onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            var connectionResult = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
            if (connectionResult.ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
            }

            var config = new SpiClockConfiguration(
                speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
                clock: Device.Pins.SCK,
                copi: Device.Pins.MOSI,
                cipo: Device.Pins.MISO,
                config: config);
            var st7789 = new St7789
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

            graphics = new MicroGraphics(st7789);
            graphics.Rotation = RotationType._90Degrees;

            onboardLed.SetColor(Color.Green);
        }

        void DrawClock()
        {
            graphics.Clear(true);

            DrawWatchFace();
            while (true)
            {
                tick++;
                Thread.Sleep(1000);
                UpdateClock(second: tick % 60);
            }
        }
        void DrawWatchFace()
        {
            graphics.Clear();
            int hour = 12;
            int xCenter = graphics.Width / 2;
            int yCenter = graphics.Height / 2;
            int x, y;

            graphics.DrawRectangle(0, 0, graphics.Width, graphics.Height, Color.White);
            graphics.DrawRectangle(5, 5, graphics.Width - 10, graphics.Height - 10, Color.White);

            graphics.CurrentFont = new Font12x20();
            graphics.DrawCircle(xCenter, yCenter, 100, WatchBackgroundColor, true);
            for (int i = 0; i < 60; i++)
            {
                x = (int)(xCenter + 80 * Math.Sin(i * Math.PI / 30));
                y = (int)(yCenter - 80 * Math.Cos(i * Math.PI / 30));

                if (i % 5 == 0)
                {
                    graphics.DrawText(hour > 9 ? x - 10 : x - 5, y - 5, hour.ToString(), Color.Black);
                    if (hour == 12) hour = 1; else hour++;
                }
            }

            graphics.Show();
        }
        void UpdateClock(int second = 0)
        {
            int xCenter = graphics.Width / 2;
            int yCenter = graphics.Height / 2;

            int TimeZoneOffSet = -7; // PST
            var today = DateTime.Now.AddHours(TimeZoneOffSet);
            int minute = today.Minute;
            int hour = today.Hour > 12 ? today.Hour - 12 : today.Hour;
            Console.WriteLine($"{hour}:{minute}");

            int x, y, xT, yT;

            if (second == 0)
            {
                minute++;
                if (minute == 60)
                {
                    minute = 0;
                    hour++;
                    if (hour == 12)
                    {
                        hour = 0;
                    }
                }
            }

            graphics.Stroke = 3;

            //remove previous hour
            int previousHour = (hour - 1) < -1 ? 11 : (hour - 1);
            x = (int)(xCenter + 43 * Math.Sin(previousHour * Math.PI / 6));
            y = (int)(yCenter - 43 * Math.Cos(previousHour * Math.PI / 6));
            xT = (int)(xCenter + 3 * Math.Sin((previousHour - 3) * Math.PI / 6));
            yT = (int)(yCenter - 3 * Math.Cos((previousHour - 3) * Math.PI / 6));
            graphics.DrawLine(xT, yT, x, y, WatchBackgroundColor);
            xT = (int)(xCenter + 3 * Math.Sin((previousHour + 3) * Math.PI / 6));
            yT = (int)(yCenter - 3 * Math.Cos((previousHour + 3) * Math.PI / 6));
            graphics.DrawLine(xT, yT, x, y, WatchBackgroundColor);
            //current hour
            x = (int)(xCenter + 43 * Math.Sin(hour * Math.PI / 6));
            y = (int)(yCenter - 43 * Math.Cos(hour * Math.PI / 6));
            xT = (int)(xCenter + 3 * Math.Sin((hour - 3) * Math.PI / 6));
            yT = (int)(yCenter - 3 * Math.Cos((hour - 3) * Math.PI / 6));
            graphics.DrawLine(xT, yT, x, y, Color.Black);
            xT = (int)(xCenter + 3 * Math.Sin((hour + 3) * Math.PI / 6));
            yT = (int)(yCenter - 3 * Math.Cos((hour + 3) * Math.PI / 6));
            graphics.DrawLine(xT, yT, x, y, Color.Black);
            //remove previous minute
            int previousMinute = minute - 1 < -1 ? 59 : (minute - 1);
            x = (int)(xCenter + 55 * Math.Sin(previousMinute * Math.PI / 30));
            y = (int)(yCenter - 55 * Math.Cos(previousMinute * Math.PI / 30));
            xT = (int)(xCenter + 3 * Math.Sin((previousMinute - 15) * Math.PI / 6));
            yT = (int)(yCenter - 3 * Math.Cos((previousMinute - 15) * Math.PI / 6));
            graphics.DrawLine(xT, yT, x, y, WatchBackgroundColor);
            xT = (int)(xCenter + 3 * Math.Sin((previousMinute + 15) * Math.PI / 6));
            yT = (int)(yCenter - 3 * Math.Cos((previousMinute + 15) * Math.PI / 6));
            graphics.DrawLine(xT, yT, x, y, WatchBackgroundColor);
            //current minute
            x = (int)(xCenter + 55 * Math.Sin(minute * Math.PI / 30));
            y = (int)(yCenter - 55 * Math.Cos(minute * Math.PI / 30));
            xT = (int)(xCenter + 3 * Math.Sin((minute - 15) * Math.PI / 6));
            yT = (int)(yCenter - 3 * Math.Cos((minute - 15) * Math.PI / 6));
            graphics.DrawLine(xT, yT, x, y, Color.Black);
            xT = (int)(xCenter + 3 * Math.Sin((minute + 15) * Math.PI / 6));
            yT = (int)(yCenter - 3 * Math.Cos((minute + 15) * Math.PI / 6));
            graphics.DrawLine(xT, yT, x, y, Color.Black);
            //remove previous second
            int previousSecond = second - 1 < -1 ? 59 : (second - 1);
            x = (int)(xCenter + 70 * Math.Sin(previousSecond * Math.PI / 30));
            y = (int)(yCenter - 70 * Math.Cos(previousSecond * Math.PI / 30));
            graphics.DrawLine(xCenter, yCenter, x, y, WatchBackgroundColor);
            //current second
            x = (int)(xCenter + 70 * Math.Sin(second * Math.PI / 30));
            y = (int)(yCenter - 70 * Math.Cos(second * Math.PI / 30));
            graphics.DrawLine(xCenter, yCenter, x, y, Color.Red);
            graphics.Show();
        }
    }
}