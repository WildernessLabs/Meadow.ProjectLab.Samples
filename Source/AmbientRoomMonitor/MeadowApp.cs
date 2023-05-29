using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace AmbientRoomMonitor
{
    // Change F7FeatherV2 to F7CoreComputeV2 for ProjectLab v3
    public class MeadowApp : App<F7FeatherV2>
    {
        Color[] colors = new Color[4]
        {
            Color.FromHex("#67E667"),
            Color.FromHex("#00CC00"),
            Color.FromHex("#269926"),
            Color.FromHex("#008500")
        };

        int padding = 10;

        RgbPwmLed onboardLed;
        MicroGraphics graphics;
        IProjectLabHardware projectLab;

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            projectLab = ProjectLab.Create();
            Resolver.Log.Info($"Running on ProjectLab Hardware {projectLab.RevisionString}");

            onboardLed = projectLab.RgbLed;
            onboardLed.SetColor(Color.Red);

            projectLab.EnvironmentalSensor.Updated += EnvironmentalSensor_Updated;

            projectLab.LightSensor.Updated += LightSensor_Updated;

            graphics = new MicroGraphics(projectLab.Display)
            {
                IgnoreOutOfBoundsPixels = true
            };

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }

        void LoadScreen()
        {
            graphics.Clear(true);

            int radius = (int)(graphics.Width / 2) + 175;
            int originX = graphics.Width / 2;
            int originYOffset = graphics.Width == 240 ? 85 : 105;
            int originY = graphics.Height + originYOffset;

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

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText(padding, 120, "LIGHT", Color.White);
            graphics.DrawText(padding, 145, "TEMPERATURE", Color.White);
            graphics.DrawText(padding, 170, "PRESSURE", Color.White);
            graphics.DrawText(padding, 195, "HUMIDITY", Color.White);

            graphics.DrawLine(0, 220, graphics.Width - 1, 220, Color.White);
            graphics.DrawLine(0, 230, graphics.Width - 1, 230, Color.White);

            graphics.Show();
        }

        private void LightSensor_Updated(object sender, IChangeResult<Illuminance> e)
        {
            graphics.DrawRectangle(
                x: graphics.Width * 2 / 3,
                y: 121,
                width: (graphics.Width / 3) - 1,
                height: 20,
                color: colors[colors.Length - 1],
                filled: true);

            graphics.DrawText((graphics.Width - 1) - padding, 120, $"{(int)e.New.Lux} Lx", Color.White, alignmentH: HorizontalAlignment.Right);
        }

        private void EnvironmentalSensor_Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            graphics.DrawRectangle(
                x: graphics.Width * 2 / 3,
                y: 145,
                width: (graphics.Width / 3) - 1,
                height: 68,
                color: colors[colors.Length - 1],
                filled: true);

            graphics.DrawText((graphics.Width - 1) - padding, 145, $"{(int)e.New.Temperature.Value.Celsius} °C", Color.White, alignmentH: HorizontalAlignment.Right);
            graphics.DrawText((graphics.Width - 1) - padding, 170, $"{(int)e.New.Pressure.Value.Millibar} Mb", Color.White, alignmentH: HorizontalAlignment.Right);
            graphics.DrawText((graphics.Width - 1) - padding, 195, $"{(int)e.New.Humidity.Value.Percent}  %", Color.White, alignmentH: HorizontalAlignment.Right);

            graphics.Show();
        }

        public override Task Run()
        {
            LoadScreen();

            projectLab.EnvironmentalSensor.StartUpdating(TimeSpan.FromSeconds(5));
            projectLab.LightSensor.StartUpdating(TimeSpan.FromSeconds(5));

            return base.Run();
        }
    }
}