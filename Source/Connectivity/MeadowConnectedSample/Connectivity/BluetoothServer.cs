using CommonContracts.Bluetooth;
using Meadow.Gateways.Bluetooth;
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
        CharacteristicBool   ledToggleCharacteristic;
        CharacteristicBool   ledBlinkCharacteristic;
        CharacteristicBool   ledPulseCharacteristic;
        CharacteristicString bme688DataCharacteristic;
        CharacteristicString bh1750DataCharacteristic;

        private BluetoothServer() { }

        public void Initialize()
        {
            bleTreeDefinition = GetDefinition();
            MeadowApp.Device.BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);

            ledToggleCharacteristic.ValueSet += LedToggleCharacteristicValueSet;
            ledBlinkCharacteristic.ValueSet += LedBlinkCharacteristicValueSet;
            ledPulseCharacteristic.ValueSet += LedPulseCharacteristicValueSet;            
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

        public void SetBme688CharacteristicValue(string value) 
        {
            bme688DataCharacteristic.SetValue(value);
            Console.WriteLine(value);
        }

        public void SetBh1750CharacteristicValue(string value)
        {
            bh1750DataCharacteristic.SetValue(value);
            Console.WriteLine(value);
        }

        Definition GetDefinition()
        {
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
                name: "ServiceA",
                uuid: 253,
                ledToggleCharacteristic,
                ledBlinkCharacteristic,
                ledPulseCharacteristic,
                bme688DataCharacteristic,
                bh1750DataCharacteristic
            );

            return new Definition("Meadow", service);
        }
    }
}