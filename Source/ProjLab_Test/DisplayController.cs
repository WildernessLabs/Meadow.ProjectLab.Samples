using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace HackBoard_Test
{
    public class DisplayController
    {
        MicroGraphics graphics;

        public (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)? AtmosphericConditions
        {
            get => atmosphericConditions;
            set 
            {
                atmosphericConditions = value;
                Update();
            }
        }
        (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)? atmosphericConditions;

        public Illuminance? LightConditions
        {
            get => lightConditions;
            set
            {
                lightConditions = value;
                Update();
            }
        }
        Illuminance? lightConditions;

        public bool UpButtonState
        {
            get => upButtonState;
            set
            {
                upButtonState = value;
                Update();
            }
        }
        bool upButtonState = false;


        public bool DownButtonState
        {
            get => downButtonState;
            set
            {
                downButtonState = value;
                Update();
            }
        }
        bool downButtonState = false;

        public bool LeftButtonState
        {
            get => leftButtonState;
            set
            {
                leftButtonState = value;
                Update();
            }
        }
        bool leftButtonState = false;

        public bool RightButtonState
        {
            get => rightButtonState;
            set
            {
                rightButtonState = value;
                Update();
            }
        }
        bool rightButtonState = false;


        bool isUpdating = false;
        bool needsUpdate = false;

        public DisplayController(IGraphicsDisplay display)
        {
            graphics = new MicroGraphics(display)
            {
                Rotation = RotationType._90Degrees,
                CurrentFont = new Font12x16()
            };

            graphics.Clear(true);
        }

        public void Update()
        {
            if (isUpdating)
            { 
                needsUpdate = true;
                return;
            }

            isUpdating = true;
            graphics.Clear();
            Draw();
            graphics.Show();

            isUpdating = false; 

            if(needsUpdate)
            {
                needsUpdate = false;
                Update();
            }
        }

        void DrawStatus(string label, string value, Color color, int yPosition)
        {
            graphics.DrawText(x: 5, y: yPosition, label, color: color);
            graphics.DrawText(x: 240, y: yPosition, value, alignment: TextAlignment.Right, color: color);
        }

        void Draw()
        {
            graphics.DrawText(x: 5, y: 5, "Hello PROJ LAB!", WildernessLabsColors.AzureBlue);

            if (AtmosphericConditions is { } conditions)
            {
                if(conditions.Temperature is { } temp)
                {
                    DrawStatus("Temperature:", $"{temp.Celsius:N1}C", WildernessLabsColors.GalleryWhite, 40);
                }

                if (conditions.Pressure is { } pressure)
                {
                    DrawStatus("Pressure:", $"{pressure.StandardAtmosphere:N1}atm", WildernessLabsColors.GalleryWhite, 60);
                }

                if (conditions.Humidity is { } humidity)
                {
                    DrawStatus("Humidity:", $"{humidity.Percent:N1}%", WildernessLabsColors.GalleryWhite, 80);
                }
            }

            if (LightConditions is { } light) 
            {
                if (light is { } lightReading)
                {
                    DrawStatus("Lux:", $"{lightReading:N0}Lux", WildernessLabsColors.GalleryWhite, 100);
                }
            }

            DrawStatus("Up:", $"{(UpButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire, 140);
            DrawStatus("Down:", $"{(DownButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire, 160);
            DrawStatus("Left:", $"{(LeftButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire, 180);
            DrawStatus("Right:", $"{(RightButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire, 200);
        }
    }
}