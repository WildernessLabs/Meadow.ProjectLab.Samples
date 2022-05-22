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
using System.Threading;
using System.Threading.Tasks;

namespace HackBoard_Test
{
    // Change F7MicroV2 to F7Micro for V1.x boards
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        RgbPwmLed onboardLed;
        PiezoSpeaker noize;
        II2cBus i2c;
        St7789 display;
        DisplayController displayController;
        Bh1750? bh1750;
        PushButton buttonUp;
        PushButton buttonRight;
        PushButton buttonLeft;
        PushButton buttonDown;
        Bme680 bme688;

        public enum Buttons
        {
            Up,
            Right,
            Down,
            Left
        }

        public MeadowApp()
        {
            Initialize();
            displayController.Render();
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            noize = new PiezoSpeaker(Device, Device.Pins.D11);

            var config = new SpiClockConfiguration(
                new Frequency(48000, Frequency.UnitType.Kilohertz), 
                SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
                Device.Pins.SCK, 
                Device.Pins.MOSI, 
                Device.Pins.MISO, 
                config);

            display = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.A03,
                dcPin: Device.Pins.A04,
                resetPin: Device.Pins.A05,
                width: 240, height: 240, 
                displayColorMode: ColorType.Format16bppRgb565)
            {
                IgnoreOutOfBoundsPixels = true
            };
            displayController = new DisplayController(display);

            i2c = Device.CreateI2cBus();

            try 
            {
                bh1750 = new Bh1750(
                    i2cBus: i2c,
                    measuringMode: Bh1750.MeasuringModes.ContinuouslyHighResolutionMode, // the various modes take differing amounts of time.
                    lightTransmittance: 0.5, // lower this to increase sensitivity, for instance, if it's behind a semi opaque window
                    address: (byte)Bh1750.Addresses.Address_0x23
                );
                bh1750.Updated += Bh1750Updated;
                bh1750.StartUpdating(TimeSpan.FromSeconds(5));
            }
            catch (Exception e) 
            {
                Console.WriteLine($"Could not bring up Bh1750: {e.Message}");
            }

            try 
            {
                bme688 = new Bme680(i2c, (byte)Bme680.Addresses.Address_0x76);
                bme688.Updated += Bme688Updated;
                bme688.StartUpdating(TimeSpan.FromSeconds(5));
            } 
            catch (Exception e) 
            {
                Console.WriteLine($"Could not bring up Bme680: {e.Message}");
            }

            buttonUp = new PushButton(Device, Device.Pins.D15, ResistorMode.InternalPullDown);
            buttonUp.Clicked += (s, e) => ButtonClicked(Buttons.Up);
            buttonRight = new PushButton(Device, Device.Pins.D05, ResistorMode.InternalPullDown);
            buttonRight.Clicked += (s, e) => ButtonClicked(Buttons.Right);
            buttonDown = new PushButton(Device, Device.Pins.D02, ResistorMode.InternalPullDown);
            buttonDown.Clicked += (s, e) => ButtonClicked(Buttons.Down);
            buttonLeft = new PushButton(Device, Device.Pins.D10, ResistorMode.InternalPullDown);
            buttonLeft.Clicked += (s, e) => ButtonClicked(Buttons.Left);

            onboardLed.SetColor(Color.Green);
        }

        private void Bme688Updated(object sender, IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> e)
        {
            Console.WriteLine($"BME688: {(int)e.New.Temperature?.Celsius}°C - {(int)e.New.Humidity?.Percent}% - {(int)e.New.Pressure?.Millibar}mbar");
            //displayController.UpdateBmeData(e.New); //ToDo: investigate why this stops the triggering this event after the first call
        }

        private void Bh1750Updated(object sender, IChangeResult<Illuminance> e)
        {
            Console.WriteLine($"BH1750: {e.New.Lux}");
        }

        void ButtonClicked(Buttons whichButton)
        {
            Console.WriteLine($"Button Clicked: {whichButton}");
            displayController.DrawButtonClick(whichButton);
        }
    }
}