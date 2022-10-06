using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using System;

namespace MoistureMeter.Controllers
{
    public class DisplayController
    {
        private static readonly Lazy<DisplayController> instance =
            new Lazy<DisplayController>(() => new DisplayController());
        public static DisplayController Instance => instance.Value;

        MicroGraphics graphics;

        private DisplayController() { }

        public void Initialize(IGraphicsDisplay display)
        {
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

                graphics.DrawRectangle(12, 222 - (22 * i + 12), 100, 20, color, true);
            }

            graphics.DrawText(174, 105, $"{percentage}%", ScaleFactor.X2, TextAlignment.Center);

            graphics.Show();
        }
    }
}