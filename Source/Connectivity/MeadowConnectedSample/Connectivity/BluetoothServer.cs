using CommonContracts.Bluetooth;
using Meadow.Gateways.Bluetooth;
using Meadow.Units;
using MeadowConnectedSample.Controller;
using System;

namespace MeadowConnectedSample.Connectivity
{
    public class BluetoothServer
    {
        private static readonly Lazy<BluetoothServer> instance =
            new Lazy<BluetoothServer>(() => new BluetoothServer());
        public static BluetoothServer Instance => instance.Value;

        Definition bleTreeDefinition;
        CharacteristicBool   pairingCharacteristic;
        CharacteristicBool   ledToggleCharacteristic;
        CharacteristicBool   ledBlinkCharacteristic;
        CharacteristicBool   ledPulseCharacteristic;
        CharacteristicString bme688DataCharacteristic;
        CharacteristicString bh1750DataCharacteristic;

        public bool IsInitialized { get; private set; }

        private BluetoothServer() { }

        public void Initialize()
        {
            bleTreeDefinition = GetDefinition();
            pairingCharacteristic.ValueSet += PairingCharacteristicValueSet;
            ledToggleCharacteristic.ValueSet += LedToggleCharacteristicValueSet;
            ledBlinkCharacteristic.ValueSet += LedBlinkCharacteristicValueSet;
            ledPulseCharacteristic.ValueSet += LedPulseCharacteristicValueSet;
            MeadowApp.Device.BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);

            IsInitialized = true;
        }

        private void PairingCharacteristicValueSet(ICharacteristic c, object data)
        {
            if ((bool)data)
            {
                DisplayController.Instance.ShowBluetoothPaired();
            }
            else
            {
                _ = DisplayController.Instance.StartConnectingAnimation(false);
            }
        }

        private void LedToggleCharacteristicValueSet(ICharacteristic c, object data)
        {
            LedController.Instance.Toggle();
        }

        private void LedBlinkCharacteristicValueSet(ICharacteristic c, object data)
        {
            LedController.Instance.StartBlink();
        }

        private void LedPulseCharacteristicValueSet(ICharacteristic c, object data)
        {
            LedController.Instance.StartPulse();
        }

        public void SetBme688CharacteristicValue((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) value) 
        {
            bme688DataCharacteristic.SetValue($"{(int)value.Temperature?.Celsius}°C;{(int)value.Humidity?.Percent}%;{(int)value.Pressure?.Millibar}mbar; {value.GasResistance?.Ohms}ohms");
        }

        public void SetBh1750CharacteristicValue(Illuminance? value)
        {
            bh1750DataCharacteristic.SetValue($"{(int)value?.Lux}lx;");
        }

        Definition GetDefinition()
        {
            pairingCharacteristic = new CharacteristicBool(
                name: "PAIRING",
                uuid: CharacteristicsConstants.PAIRING,
                permissions: CharacteristicPermission.Read | CharacteristicPermission.Write,
                properties: CharacteristicProperty.Read | CharacteristicProperty.Write);
            ledToggleCharacteristic = new CharacteristicBool(
                name: "LED_TOGGLE",
                uuid: CharacteristicsConstants.LED_TOGGLE,
                permissions: CharacteristicPermission.Read | CharacteristicPermission.Write,
                properties: CharacteristicProperty.Read | CharacteristicProperty.Write);
            ledBlinkCharacteristic = new CharacteristicBool(
                name: "LED_BLINK",
                uuid: CharacteristicsConstants.LED_BLINK,
                permissions: CharacteristicPermission.Read | CharacteristicPermission.Write,
                properties: CharacteristicProperty.Read | CharacteristicProperty.Write);
            ledPulseCharacteristic = new CharacteristicBool(
                name: "LED_PULSE",
                uuid: CharacteristicsConstants.LED_PULSE,
                permissions: CharacteristicPermission.Read | CharacteristicPermission.Write,
                properties: CharacteristicProperty.Read | CharacteristicProperty.Write);
            bme688DataCharacteristic = new CharacteristicString(
                name: "BME688_DATA",
                uuid: CharacteristicsConstants.BME688_DATA,
                maxLength: 20,
                permissions: CharacteristicPermission.Read,
                properties: CharacteristicProperty.Read);
            bh1750DataCharacteristic = new CharacteristicString(
                name: "BH1750_DATA",
                uuid: CharacteristicsConstants.BH1750_DATA,
                maxLength: 20,
                permissions: CharacteristicPermission.Read,
                properties: CharacteristicProperty.Read);

            var service = new Service(
                name: "Service",
                uuid: 253,
                pairingCharacteristic,
                ledToggleCharacteristic,
                ledBlinkCharacteristic,
                ledPulseCharacteristic,
                bme688DataCharacteristic,
                bh1750DataCharacteristic
            );

            return new Definition("ProjectLab", service);
        }
    }
}