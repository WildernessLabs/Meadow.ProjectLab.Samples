using System;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace HackBoard_Test
{
    public class DisplayController
    {
        MicroGraphics canvas;
        bool buttonClicked = false;
        MeadowApp.Buttons whichButton;
        

        public (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)? BmeConditions {
            get => bmeConditions;
            set {
                bmeConditions = value;
                Render();
            }
        }
        (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)? bmeConditions;

        public Illuminance? LightConditions {
            get => light;
            set {
                light = value;
                Render();
            }
        } Illuminance? light;

        public DisplayController(IGraphicsDisplay display)
        {
            canvas = new MicroGraphics(display);
            canvas.Rotation = RotationType._90Degrees;

            canvas.Clear(true);
        }

        public void Render()
        {
            canvas.Clear();
            DrawShit();
            canvas.Show();
        }

        public void DrawButtonClick(MeadowApp.Buttons which)
        {
            buttonClicked = true;
            this.whichButton = which;
            Render();

            // reset the state machine
            buttonClicked = false;
        }

        protected void DrawShit()
        {
            canvas.CurrentFont = new Font12x20();
            canvas.DrawText(x: 5, y: 5, "hello, Meadow!", WildernessLabsColors.AzureBlue);

            if (buttonClicked) {
                canvas.DrawText(x: 5, y: 20, $"{whichButton} clicked.", WildernessLabsColors.ChileanFire);
            }

            if (BmeConditions is { } conditions) {
                if(conditions.Temperature is { } temp)
                canvas.DrawText(x: 5, y: 40, $"Temp: {temp.Celsius:N2}C", WildernessLabsColors.GalleryWhite);
            }

            if (LightConditions is { } light) {
                if (light is { } lightReading)
                    canvas.DrawText(x: 5, y: 60, $"Lux: {lightReading:N2}Lux", WildernessLabsColors.GalleryWhite);
            }
        }
    }
}
