using System;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Light;
using Meadow.Gateways.Bluetooth;
using Meadow.Hardware;
using Meadow.Units;

namespace ProjLab_Demo
{
    public class ProjectLabV2_Hardware : IHardwareConfig
    {
        public IGraphicsDisplay Display { get; protected set; }

        public ISpiBus SpiBus { get; protected set; }

        public II2cBus I2cBus { get; protected set; }

        public RgbPwmLed OnboardLed { get; protected set; }

        public PiezoSpeaker Speaker { get; protected set; }

        public Bh1750? Bh1750 { get; protected set; }

        public Bme680? Bme688 { get; protected set; }

        public Bmi270? Bmi270 { get; protected set; }

        public PushButton UpButton { get; protected set; }
        public PushButton DownButton { get; protected set; }
        public PushButton LeftButton { get; protected set; }
        public PushButton RightButton { get; protected set; }

        public Mcp23008 Mcp_1 { get; protected set; }
        public Mcp23008 Mcp_2 { get; protected set; }


        public void Initialize(IF7FeatherMeadowDevice device)
        {
            //==== I2C Bus
            try
            {
                I2cBus = device.CreateI2cBus();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERR creating I2C: {e.Message}");
            }

            //==== MCP23008's

            // MCP the First.
            IDigitalInputPort mcp1_int = device.CreateDigitalInputPort(
                device.Pins.D09, InterruptMode.EdgeRising, ResistorMode.InternalPullDown);

            Mcp_1 = new Mcp23008(I2cBus, address: 0x20, mcp1_int);

            // MCP the Second.
            IDigitalInputPort mcp2_int = device.CreateDigitalInputPort(
                device.Pins.D10, InterruptMode.EdgeRising, ResistorMode.InternalPullDown);
            Mcp_2 = new Mcp23008(I2cBus, address: 0x21, mcp2_int);

            //==== SPI BUS
            var spiConfig = new SpiClockConfiguration(
                new Frequency(48000, Frequency.UnitType.Kilohertz),
                SpiClockConfiguration.Mode.Mode3);

            SpiBus = device.CreateSpiBus(
                device.Pins.SCK,
                device.Pins.MOSI,
                device.Pins.MISO,
                spiConfig);

            //==== Display
            var chipSelectPort = Mcp_1.CreateDigitalOutputPort(Mcp_1.Pins.GP5);
            var dcPort = Mcp_1.CreateDigitalOutputPort(Mcp_1.Pins.GP6);
            var resetPort = Mcp_1.CreateDigitalOutputPort(Mcp_1.Pins.GP7);

            Display = new St7789(
                spiBus: SpiBus,
                chipSelectPort: chipSelectPort,
                dataCommandPort: dcPort,
                resetPort: resetPort,
                width: 240, height: 240,
                colorMode: ColorType.Format16bppRgb565);

            //==== Onboard LED
            OnboardLed = new RgbPwmLed(device: device,
                redPwmPin: device.Pins.OnboardLedRed,
                greenPwmPin: device.Pins.OnboardLedGreen,
                bluePwmPin: device.Pins.OnboardLedBlue);
            OnboardLed.StartPulse(WildernessLabsColors.ChileanFire);

            //==== Speaker
            Speaker = new PiezoSpeaker(device, device.Pins.D11);

            //==== BH1750
            try
            {
                Bh1750 = new Bh1750(
                    i2cBus: I2cBus,
                    measuringMode: Bh1750.MeasuringModes.ContinuouslyHighResolutionMode, // the various modes take differing amounts of time.
                    lightTransmittance: 0.5, // lower this to increase sensitivity, for instance, if it's behind a semi opaque window
                    address: (byte)Bh1750.Addresses.Address_0x23
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bh1750: {e.Message}");
            }

            //==== BMI270
            try
            {
                Bmi270 = new Bmi270(
                    i2cBus: I2cBus,
                    address: (byte)Bmi270.Addresses.Address_0x68
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bmi270: {e.Message}");
            }

            //==== BME688
            try
            {
                Bme688 = new Bme680(I2cBus, (byte)Bme680.Addresses.Address_0x76);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bme688: {e.Message}");
            }

            //==== Buttons
            // TODO: consider how we want to instantiate these, given the resistors/edges, and such
            //UpButton = new PushButton(Mcp_1, Mcp_1.Pins.GP0, ResistorMode.Disabled);
            var upPort = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP0, InterruptMode.EdgeBoth);
            UpButton = new PushButton(upPort);

            var rightPort = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP1, InterruptMode.EdgeBoth);
            RightButton = new PushButton(rightPort);

            var downPort = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP3, InterruptMode.EdgeBoth);
            DownButton = new PushButton(downPort);

            var leftPort = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP2, InterruptMode.EdgeBoth);
            LeftButton = new PushButton(leftPort);

            Console.WriteLine("Hardware initialization complete");
        }
    }
}