using System;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace HackBoard_Test
{
    public class DisplayController
    {
        MicroGraphics graphics;
        bool buttonClicked = false;
        MeadowApp.Buttons whichButton;

        public (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)? BmeConditions 
        {
            get => bmeConditions;
            set {
                bmeConditions = value;
                Render();
            }
        } (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)? bmeConditions;

        public Illuminance? LightConditions 
        {
            get => light;
            set {
                light = value;
                Render();
            }
        } Illuminance? light;

        public DisplayController(IGraphicsDisplay display)
        {
            graphics = new MicroGraphics(display);
            graphics.Rotation = RotationType._90Degrees;

            graphics.Clear(true);
        }

        public void Render()
        {
            graphics.Clear();
            Draw();
            graphics.Show();
        }

        public void DrawButtonClick(MeadowApp.Buttons which)
        {
            buttonClicked = true;
            this.whichButton = which;
            Render();

            // reset the state machine
            buttonClicked = false;
        }

        protected void Draw()
        {
            graphics.CurrentFont = new Font12x16();
            graphics.DrawText(x: 5, y: 5, "hello, Meadow!", WildernessLabsColors.AzureBlue);

            if (buttonClicked) {
                graphics.DrawText(x: 5, y: 20, $"{whichButton} clicked.", WildernessLabsColors.ChileanFire);
            }

            if (BmeConditions is { } conditions) {
                if(conditions.Temperature is { } temp)
                graphics.DrawText(x: 5, y: 40, $"Temp: {temp.Celsius:N2}C", WildernessLabsColors.GalleryWhite);
            }

            if (LightConditions is { } light) {
                if (light is { } lightReading)
                    graphics.DrawText(x: 5, y: 60, $"Lux: {lightReading:N2}Lux", WildernessLabsColors.GalleryWhite);
            }
        }

        public void UpdateBmeData((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure) values) 
        {
            graphics.DrawText(8, 28, "BME688");
            graphics.DrawRectangle(15, 48, 200, 56, Color.Black, true);
            graphics.DrawText(16, 48, $"Temperature: {(int)values.Temperature?.Celsius}°C");
            graphics.DrawText(16, 68, $"Humidity: {(int)values.Humidity?.Percent}");
            graphics.DrawText(16, 88, $"Pressure: {(int)values.Pressure?.Millibar}");
            graphics.Show();
        }
    }
}