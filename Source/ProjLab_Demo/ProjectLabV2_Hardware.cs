using System;
using System.Threading;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Audio;
using Meadow.Foundation.Displays;
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
        public Bme688? Bme688 { get; protected set; }
        public Bmi270? Bmi270 { get; protected set; }

        public PushButton UpButton { get; protected set; }
        public PushButton DownButton { get; protected set; }
        public PushButton LeftButton { get; protected set; }
        public PushButton RightButton { get; protected set; }

        public Mcp23008 Mcp_1 { get; protected set; }
        public Mcp23008 Mcp_2 { get; protected set; }
        public Mcp23008 Mcp_Version { get; protected set; }

        public HardwareBringupStatus Status { get; protected set; } = new HardwareBringupStatus();

        public void Initialize(IF7FeatherMeadowDevice device)
        {
            Console.WriteLine("Initialize I2cBus...");

            //==== I2C Bus
            try
            {
                I2cBus = device.CreateI2cBus();
                Status.I2c = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERR creating I2C: {e.Message}");
            }

            Console.WriteLine("Initialize MCP23008s...");

            //==== MCP23008s
            try
            {
                // MCP the First
                IDigitalInputPort mcp1_int = device.CreateDigitalInputPort(
                    device.Pins.D09, InterruptMode.EdgeRising, ResistorMode.InternalPullDown);
                IDigitalOutputPort mcp_Reset = device.CreateDigitalOutputPort(device.Pins.D14);

                Mcp_1 = new Mcp23008(I2cBus, address: 0x20, mcp1_int, mcp_Reset);
                Status.Mcp_1 = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERR creating MCP1: {e.Message}");
            }
            try
            {
                // MCP the Second
                IDigitalInputPort mcp2_int = device.CreateDigitalInputPort(
                    device.Pins.D10, InterruptMode.EdgeRising, ResistorMode.InternalPullDown);
                Mcp_2 = new Mcp23008(I2cBus, address: 0x21, mcp2_int);
                Status.Mcp_2 = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERR creating MCP2: {e.Message}");
            }
            try
            {
                Mcp_Version = new Mcp23008(I2cBus, address: 0x23);
                byte version = Mcp_Version.ReadFromPorts(Mcp23xxx.PortBank.A);
                Console.WriteLine($"Project Lab version: {version.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERR creating the MCP that has version information: {e.Message}");
            }

            Console.WriteLine("Initialize SpiBus...");

            //==== SPI BUS
            try
            {
                var spiConfig = new SpiClockConfiguration(
                new Frequency(48000, Frequency.UnitType.Kilohertz),
                SpiClockConfiguration.Mode.Mode3);

                SpiBus = device.CreateSpiBus(
                    device.Pins.SCK,
                    device.Pins.MOSI,
                    device.Pins.MISO,
                    spiConfig);
                Status.Spi = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERR creating SPI: {e.Message}");
            }

            Console.WriteLine("Initialize ST7789...");

            //==== Display
            if (Mcp_1 != null)
            {
                var chipSelectPort = Mcp_1.CreateDigitalOutputPort(Mcp_1.Pins.GP5);
                var dcPort = Mcp_1.CreateDigitalOutputPort(Mcp_1.Pins.GP6);
                var resetPort = Mcp_1.CreateDigitalOutputPort(Mcp_1.Pins.GP7);

                Thread.Sleep(50);

                Display = new St7789(
                    spiBus: SpiBus,
                    chipSelectPort: chipSelectPort,
                    dataCommandPort: dcPort,
                    resetPort: resetPort,
                    width: 240, height: 240,
                    colorMode: ColorType.Format16bppRgb565);
            }

            Console.WriteLine("Initialize LED...");

            //==== Onboard LED
            OnboardLed = new RgbPwmLed(device: device,
                redPwmPin: device.Pins.OnboardLedRed,
                greenPwmPin: device.Pins.OnboardLedGreen,
                bluePwmPin: device.Pins.OnboardLedBlue);
            OnboardLed.StartPulse(WildernessLabsColors.ChileanFire);

            Console.WriteLine("Initialize Speaker...");

            //==== Speaker
            Speaker = new PiezoSpeaker(device, device.Pins.D11);

            Console.WriteLine("Initialize BH1750...");

            //==== BH1750
            try
            {
                Bh1750 = new Bh1750(
                    i2cBus: I2cBus,
                    measuringMode: Bh1750.MeasuringModes.ContinuouslyHighResolutionMode, // the various modes take differing amounts of time.
                    lightTransmittance: 0.5, // lower this to increase sensitivity, for instance, if it's behind a semi opaque window
                    address: (byte)Bh1750.Addresses.Address_0x23
                );
                Status.Bh1750 = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bh1750: {e.Message}");
            }

            Console.WriteLine("Initialize BMI270...");

            //==== BMI270
            try
            {
                Bmi270 = new Bmi270(
                    i2cBus: I2cBus,
                    address: (byte)Bmi270.Addresses.Address_0x68
                );
                Status.Bmi270 = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bmi270: {e.Message}");
            }

            Console.WriteLine("Initialize BME688...");

            //==== BME688
            try
            {
                Bme688 = new Bme688(I2cBus, (byte)Bme688.Addresses.Address_0x76);
                Status.Bme688 = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not bring up Bme688: {e.Message}");
            }

            Console.WriteLine("Initialize Buttons...");

            //==== Buttons
            if (Mcp_1 != null)
            {
                /* 2a
                var upPort    = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP0, InterruptMode.EdgeBoth, ResistorMode.ExternalPullDown);
                var rightPort = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP1, InterruptMode.EdgeBoth, ResistorMode.ExternalPullDown);
                var downPort  = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP3, InterruptMode.EdgeBoth, ResistorMode.ExternalPullDown);
                var leftPort  = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP2, InterruptMode.EdgeBoth, ResistorMode.ExternalPullDown);
                */

                var upPort    = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP0, InterruptMode.EdgeBoth, ResistorMode.InternalPullUp);
                var rightPort = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP1, InterruptMode.EdgeBoth, ResistorMode.InternalPullUp);
                var downPort  = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP3, InterruptMode.EdgeBoth, ResistorMode.InternalPullUp);
                var leftPort  = Mcp_1.CreateDigitalInputPort(Mcp_1.Pins.GP2, InterruptMode.EdgeBoth, ResistorMode.InternalPullUp);

                UpButton = new PushButton(upPort);
                Status.BtnUp = true;
                
                RightButton = new PushButton(rightPort);
                Status.BtnRight = true;
                
                DownButton = new PushButton(downPort);
                Status.BtnDown = true;
                
                LeftButton = new PushButton(leftPort);
                Status.BtnLeft = true;
            }

            if (Status.AllGood) 
            {
                Console.WriteLine("Hardware initialized without errs.");
            }
            else
            {
                Console.WriteLine(Status);
            }
        }
    }
}