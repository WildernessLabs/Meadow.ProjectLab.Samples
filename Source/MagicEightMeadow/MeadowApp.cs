using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Peripherals.Leds;
using Meadow.Units;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MagicEightMeadow
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        ProjectLab projectLab;
        RgbPwmLed onboardLed;
        MicroGraphics graphics;
        bool isShaking = false;
        bool isAnsweriing = false;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                CommonType.CommonAnode);
            onboardLed.SetColor(Color.Red);

            projectLab = new ProjectLab();

            graphics = new MicroGraphics(projectLab.Display);
            graphics.Rotation = RotationType._90Degrees;

            projectLab.MotionSensor.AngularVelocity3DUpdated += MotionSensor_AngularVelocity3DUpdated;
            projectLab.MotionSensor.StartUpdating(TimeSpan.FromSeconds(1));

            onboardLed.SetColor(Color.Green);
            return base.Initialize();
        }

        private async void MotionSensor_AngularVelocity3DUpdated(object sender, IChangeResult<Meadow.Units.AngularVelocity3D> e)
        {
            if (e.New.Y.DegreesPerSecond > 0.5)
            {
                isShaking = true;
            }
            else
            {
                if (isShaking)
                {
                    isShaking = false;
                    await ShowAnswer();
                }
            }

            //Console.WriteLine($"({e.New.Y.DegreesPerSecond})");
        }

        async Task ShowAnswer() 
        {
            if (isAnsweriing)
                return;
            isAnsweriing = true;

            onboardLed.SetColor(Color.Orange);

            var rand = new Random();

            DisplayJPG(rand.Next(1, 21));

            await Task.Delay(TimeSpan.FromSeconds(5));

            DisplayJPG(21);

            onboardLed.SetColor(Color.Green);

            isAnsweriing = false;
        }

        void DisplayJPG(int answerNumber)
        {
            var jpgData = LoadResource(answerNumber);
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

        byte[] LoadResource(int answerNumber)
        {
            var assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine($"MagicEightMeadow.m8b_{answerNumber.ToString("00")}.jpg");
            var resourceName = $"MagicEightMeadow.m8b_{answerNumber.ToString("00")}.jpg";

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
            Console.WriteLine("Run...");

            DisplayJPG(21);

            return base.Run();
        }
    }
}