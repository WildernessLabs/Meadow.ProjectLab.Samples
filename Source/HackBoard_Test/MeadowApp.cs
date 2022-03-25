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
        ISpiBus spi;
        II2cBus i2c;
        St7789 display;
        DisplayController displayController;
        Bh1750? bh1750;
        PushButton buttonUp;
        PushButton buttonRight;
        PushButton buttonLeft;
        PushButton buttonDown;
        Bme680 bme;
        Apa102 rgbLedStrip;
        int numberOfLeds = 9;
        float maxBrightness = 0.25f;

        public MeadowApp()
        {
            Initialize().Wait();
            SetInitialRgbLedColors();
            ReadLightSensor().Wait();
            displayController.Render();
            StartUpdating();
            CycleColors(1000);
        }

        async Task Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);

            noize = new PiezoSpeaker(Device, Device.Pins.D11);

            i2c = Device.CreateI2cBus();

            var config = new SpiClockConfiguration(new Frequency(48000, Frequency.UnitType.Kilohertz), SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(Device.Pins.SCK, Device.Pins.MOSI, Device.Pins.MISO, config);

            display = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.A03,
                dcPin: Device.Pins.A04,
                resetPin: Device.Pins.A05,
                width: 240, height: 240, displayColorMode: ColorType.Format16bppRgb565)
            {
                IgnoreOutOfBoundsPixels = true
            };
            displayController = new DisplayController(display);

            // HD107S RGB LEDs
            rgbLedStrip = new Apa102(spi, numberOfLeds, Apa102.PixelOrder.BGR);

            // Bh1750
            try 
            {
                bh1750 = new Bh1750(
                    i2cBus: i2c,
                    measuringMode: Bh1750.MeasuringModes.ContinuouslyHighResolutionMode, // the various modes take differing amounts of time.
                    lightTransmittance: 1, // lower this to increase sensitivity, for instance, if it's behind a semi opaque window
                    address: (byte)Bh1750.Addresses.Default
                );
            }
            catch (Exception e) 
            {
                Console.WriteLine($"Could not bring up Bh1750: {e.Message}");
            }

            try 
            {
                Console.WriteLine("instantiating the BME");
                bme = new Bme680(i2c, (byte)Bme680.Addresses.Address_0x76);
                Console.WriteLine("Bme up");

                var conditions = await bme.Read();
                Console.WriteLine("Initial Readings:");
                Console.WriteLine($"  Temperature: {conditions.Temperature?.Celsius:N2}C");
                Console.WriteLine($"  Pressure: {conditions.Pressure?.Bar:N2}hPa");
                Console.WriteLine($"  Relative Humidity: {conditions.Humidity?.Percent:N2}%");

                displayController.BmeConditions = conditions;

                Bme680.CreateObserver(result => { displayController.BmeConditions = result.New; }, null);
            } 
            catch (Exception e) 
            {
                Console.WriteLine($"Could not bring up Bme680: {e.Message}");
            }

            // buttons
            buttonUp = new PushButton(Device, Device.Pins.D15, ResistorMode.InternalPullDown);
            buttonUp.Clicked += (s, e) => ButtonClicked(Buttons.Up);
            buttonRight = new PushButton(Device, Device.Pins.D05, ResistorMode.InternalPullDown);
            buttonRight.Clicked += (s, e) => ButtonClicked(Buttons.Right);
            buttonDown = new PushButton(Device, Device.Pins.D02, ResistorMode.InternalPullDown);
            buttonDown.Clicked += (s, e) => ButtonClicked(Buttons.Down);
            buttonLeft = new PushButton(Device, Device.Pins.D10, ResistorMode.InternalPullDown);
            buttonLeft.Clicked += (s, e) => ButtonClicked(Buttons.Left);
        }

        public enum Buttons
        {
            Up,
            Right,
            Down,
            Left
        }

        void ButtonClicked(Buttons whichButton)
        {
            Console.WriteLine($"Button Clicked: {whichButton}");
            displayController.DrawButtonClick(whichButton);
        }

        void StartUpdating()
        {
            // TODO: Throws a NRE for some reason.
            //bme.StartUpdating();
        }

        async Task ReadLightSensor()
        {
            if (bh1750 != null) {
                var result = await bh1750.Read();
                Console.WriteLine("Initial Readings:");
                Console.WriteLine($"   Light: {result.Lux:N2}Lux");

                displayController.LightConditions = result;
            }
        }

        void SetInitialRgbLedColors()
        {
            //rgbLedStrip.Clear();

            //rgbLedStrip.SetLed(index: 0, color: Color.Red, brightness: 0.5f);
            //rgbLedStrip.SetLed(index: 1, color: Color.Purple, brightness: 0.6f);
            //rgbLedStrip.SetLed(index: 2, color: Color.Blue, brightness: 0.7f);
            //rgbLedStrip.SetLed(index: 2, color: Color.Green, brightness: 0.8f);
            //rgbLedStrip.SetLed(index: 2, color: Color.Yellow, brightness: 0.9f);
            //rgbLedStrip.SetLed(index: 2, color: Color.Orange, brightness: 1.0f);

            //rgbLedStrip.Show();
        }

        void CycleColors(int duration)
        {
            Console.WriteLine("Cycle colors...");

            while (true) {
                Console.WriteLine("Playing a tone.");
                noize.PlayTone(440, 500);

                ShowColorPulse(Color.Blue, duration);
                ShowColorPulse(Color.Cyan, duration);
                ShowColorPulse(Color.Green, duration);
                ShowColorPulse(Color.GreenYellow, duration);
                ShowColorPulse(Color.Yellow, duration);
                ShowColorPulse(Color.Orange, duration);
                ShowColorPulse(Color.OrangeRed, duration);
                ShowColorPulse(Color.Red, duration);
                ShowColorPulse(Color.MediumVioletRed, duration);
                ShowColorPulse(Color.Purple, duration);
                ShowColorPulse(Color.Magenta, duration);
                ShowColorPulse(Color.Pink, duration);
            }
        }

        void ShowColorPulse(Color color, int duration = 1000)
        {
            onboardLed.StartPulse(color, duration / 2);
            Thread.Sleep(duration);
            onboardLed.Stop();
        }

        void ShowColor(Color color, int duration = 1000)
        {
            Console.WriteLine($"Color: {color}");
            onboardLed.SetColor(color);
            Thread.Sleep(duration);
            onboardLed.Stop();
        }
    }
}