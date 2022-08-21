using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Light;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace ProjLab_Demo
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        DisplayController displayController;
        IHardwareConfig hardware;

        public override Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            // get the correct hardware config depending on board version
#if V1_PROJLAB
            hardware = new ProjectLabV1_Hardware();
#elif V2_PROJLAB
            hardware = new ProjectLabV2_Hardware();
#endif
            // Initialize the board specific hardware
            hardware.Initialize(Device);

            displayController = new DisplayController(hardware.Display);

            //---- BH1750 Light Sensor
            if (hardware.Bh1750 is { } bh1750) {
                bh1750.Updated += Bh1750Updated;
                bh1750.StartUpdating(TimeSpan.FromSeconds(5));
            }

            //---- BME688 Atmospheric sensor
            if (hardware.Bme688 is { } bme688)
            {
                bme688.Updated += Bme688Updated;
                bme688.StartUpdating(TimeSpan.FromSeconds(5));
            }

            //---- BMI270 Accel/IMU
            if(hardware.Bmi270 is { } bmi270)
            {
                bmi270.Updated += Bmi270Updated;
                bmi270.StartUpdating(TimeSpan.FromSeconds(5));
            }

            //---- buttons
            hardware.UpButton.PressStarted += (s, e) => displayController.UpButtonState = true;
            Console.WriteLine("UP");
            hardware.LeftButton.PressStarted += (s, e) => displayController.LeftButtonState = true;
            Console.WriteLine("Left");
            hardware.RightButton.PressStarted += (s, e) => displayController.RightButtonState = true;
            Console.WriteLine("Right");

            hardware.UpButton.PressEnded += (s, e) => displayController.UpButtonState = false;
            hardware.LeftButton.PressEnded += (s, e) => displayController.LeftButtonState = false;
            hardware.RightButton.PressEnded += (s, e) => displayController.RightButtonState = false;

#if V2_PROJLAB
            hardware.DownButton.PressStarted += (s, e) => displayController.DownButtonState = true;
            Console.WriteLine("Down");
            hardware.DownButton.PressEnded += (s, e) => displayController.RightButtonState = false;
#endif
            //---- heartbeat
            hardware.OnboardLed.StartPulse(WildernessLabsColors.PearGreen);

            return base.Initialize();
        }

        private void Bmi270Updated(object sender, IChangeResult<(Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature)> e)
        {
            Console.WriteLine($"BMI270: {e.New.Acceleration3D.Value.X.Gravity:0.0},{e.New.Acceleration3D.Value.Y.Gravity:0.0},{e.New.Acceleration3D.Value.Z.Gravity:0.0}g");
            displayController.AccelerationConditions = e.New;
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> e)
        {
            Console.WriteLine($"BME688: {(int)e.New.Temperature?.Celsius}°C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");
            displayController.AtmosphericConditions = e.New;
        }

        private void Bh1750Updated(object sender, IChangeResult<Illuminance> e)
        {
            Console.WriteLine($"BH1750: {e.New.Lux}");
            displayController.LightConditions = e.New;
        }


        public override Task Run()
        {
            displayController.Update();

            Console.WriteLine("starting da blink");
            hardware.OnboardLed.StartBlink(WildernessLabsColors.PearGreen, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(2000), 0.5f);

            return base.Run();
        }

        
    }
}