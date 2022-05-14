using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Grove.Sensors.Moisture;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Hardware;
using Meadow.Units;
using System;

namespace PlantMonitor
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2, MeadowApp>
    {
        RgbPwmLed onboardLed;
        MoistureSensor moistureSensor;
        AnalogTemperature analogTemperature;
        DisplayController displayController;

        public MeadowApp()
        {
            Initialize();
        }

        void Initialize()
        {
            onboardLed = new RgbPwmLed(
                device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            var config = new SpiClockConfiguration(
                speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
                clock: Device.Pins.SCK,
                copi: Device.Pins.MOSI,
                cipo: Device.Pins.MISO,
                config: config);
            var display = new St7789
            (
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.A03,
                dcPin: Device.Pins.A04,
                resetPin: Device.Pins.A05,
                width: 240,
                height: 240,
                displayColorMode: ColorType.Format16bppRgb565
            );
            displayController = new DisplayController(display);

            moistureSensor = new MoistureSensor(Device, Device.Pins.A01);

            var capacitiveObserver = MoistureSensor.CreateObserver(
                handler: result =>
                {
                    onboardLed.SetColor(Color.Purple);

                    displayController.UpdateMoistureImage(result.New.Volts);
                    displayController.UpdateMoisturePercentage(result.New.Volts, result.Old.Value.Volts);

                    onboardLed.SetColor(Color.Green);
                },
                filter: null
            );
            moistureSensor.Subscribe(capacitiveObserver);

            moistureSensor.StartUpdating(TimeSpan.FromHours(1));

            analogTemperature = new AnalogTemperature(Device, Device.Pins.A00, AnalogTemperature.KnownSensorType.LM35);
            var analogTemperatureObserver = AnalogTemperature.CreateObserver(
                handler =>
                {
                    onboardLed.SetColor(Color.Purple);

                    displayController.UpdateTemperatureValue(handler.New, handler.Old.Value);

                    onboardLed.SetColor(Color.Green);
                },
                filter: null
            );
            analogTemperature.Subscribe(analogTemperatureObserver);
            analogTemperature.StartUpdating(TimeSpan.FromHours(1));

            onboardLed.SetColor(Color.Green);
        }
    }
}