using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Sensors.Hid;
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
                Rotation = RotationType._90Degrees
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

        void Draw()
        {
            graphics.CurrentFont = new Font12x16();
            graphics.DrawText(x: 5, y: 5, "Hello PROJ LAB!", WildernessLabsColors.AzureBlue);


            if (AtmosphericConditions is { } conditions)
            {
                if(conditions.Temperature is { } temp)
                {
                    graphics.DrawText(x: 5, y:  40, $"Temperature: {temp.Celsius:N1}C", WildernessLabsColors.GalleryWhite);
                }

                if (conditions.Pressure is { } pressure)
                {
                    graphics.DrawText(x: 5, y:  60, $"Pressure:    {pressure.StandardAtmosphere:N1}atm", WildernessLabsColors.GalleryWhite);
                }

                if (conditions.Humidity is { } humidity)
                {
                    graphics.DrawText(x: 5, y:  80, $"Humidity:    {humidity.Percent:N1}%", WildernessLabsColors.GalleryWhite);
                }
            }

            if (LightConditions is { } light) 
            {
                if (light is { } lightReading)
                {
                    graphics.DrawText(x: 5, y: 100, $"Lux:         {lightReading:N0}Lux", WildernessLabsColors.GalleryWhite);
                }
            }

            graphics.DrawText(x: 5, y: 140, $"Up:    {(UpButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire);
            graphics.DrawText(x: 5, y: 160, $"Down:  {(DownButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire);
            graphics.DrawText(x: 5, y: 180, $"Left:  {(LeftButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire);
            graphics.DrawText(x: 5, y: 200, $"Right: {(RightButtonState ? "pressed" : "released")}", WildernessLabsColors.ChileanFire);
        }
    }
}