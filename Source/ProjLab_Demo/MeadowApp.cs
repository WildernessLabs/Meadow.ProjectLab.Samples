using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
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
            if (hardware.Bmi270 is { } bmi270)
            {
                bmi270.Updated += Bmi270Updated;
                bmi270.StartUpdating(TimeSpan.FromSeconds(5));
            }

            //---- buttons
            if (hardware.LeftButton is { } leftButton) {
                leftButton.PressStarted += (s, e) => displayController.LeftButtonState = true;
                leftButton.PressEnded += (s, e) => displayController.LeftButtonState = false;
            }

            if (hardware.RightButton is { } rightButton)
            {
                rightButton.PressStarted += (s, e) => displayController.RightButtonState = true;
                rightButton.PressEnded += (s, e) => displayController.RightButtonState = false;
            }

#if V2_PROJLAB

            hardware.UpButton.PressStarted += (s, e) => displayController.UpButtonState = true;
            hardware.UpButton.PressEnded += (s, e) => displayController.UpButtonState = false;

            hardware.DownButton.PressStarted += (s, e) => displayController.DownButtonState = true;
            hardware.DownButton.PressEnded += (s, e) => displayController.DownButtonState = false;
#endif
            //---- heartbeat
            hardware.OnboardLed.StartPulse(WildernessLabsColors.PearGreen);

            Console.WriteLine("Initialization complete");

            return base.Initialize();
        }

        private void Bmi270Updated(object sender, IChangeResult<(Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D, Temperature? Temperature)> e)
        {
            Console.WriteLine($"BMI270: {e.New.Acceleration3D.Value.X.Gravity:0.0},{e.New.Acceleration3D.Value.Y.Gravity:0.0},{e.New.Acceleration3D.Value.Z.Gravity:0.0}g");
            displayController.AccelerationConditions = e.New;
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
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
            Console.WriteLine("Run...");

            displayController.Update();

            Console.WriteLine("starting blink");
            hardware.OnboardLed.StartBlink(WildernessLabsColors.PearGreen, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(2000), 0.5f);

            return base.Run();
        }
    }
}