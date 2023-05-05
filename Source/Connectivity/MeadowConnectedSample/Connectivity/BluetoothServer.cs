using CommonContracts.Bluetooth;
using Meadow.Gateways.Bluetooth;
using Meadow.Units;
using MeadowConnectedSample.Models.Logical;
using MeadowConnectedSample.Views;
using System;

namespace MeadowConnectedSample.Connectivity
{
    public class BluetoothServer
    {
        private static readonly Lazy<BluetoothServer> instance =
            new Lazy<BluetoothServer>(() => new BluetoothServer());
        public static BluetoothServer Instance => instance.Value;

        Definition bleTreeDefinition;
        CharacteristicBool pairingCharacteristic;
        CharacteristicBool ledToggleCharacteristic;
        CharacteristicBool ledBlinkCharacteristic;
        CharacteristicBool ledPulseCharacteristic;
        CharacteristicString environmentalDataCharacteristic;
        CharacteristicString lightDataCharacteristic;
        CharacteristicString motionDataCharacteristic;

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
                DisplayView.Instance.ShowBluetoothPaired();
            }
            else
            {
                _ = DisplayView.Instance.StartConnectingAnimation(false);
            }
        }

        private async void LedToggleCharacteristicValueSet(ICharacteristic c, object data)
        {
            await LedController.Instance.Toggle();
        }

        private async void LedBlinkCharacteristicValueSet(ICharacteristic c, object data)
        {
            await LedController.Instance.StartBlink();
        }

        private async void LedPulseCharacteristicValueSet(ICharacteristic c, object data)
        {
            await LedController.Instance.StartPulse();
        }

        public void SetEnvironmentalCharacteristicValue((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) value)
        {
            string stringValue = $"{(int)value.Temperature?.Celsius};{(int)value.Humidity?.Percent};{(int)value.Pressure?.Millibar}";
            Console.WriteLine(stringValue);
            environmentalDataCharacteristic.SetValue(stringValue);
        }

        public void SetLightCharacteristicValue(Illuminance? value)
        {
            lightDataCharacteristic.SetValue($"{(int)value?.Lux}lx;");
        }

        public void SetMotionCharacteristicValue((Acceleration3D? acceleration3D, AngularVelocity3D? angularVelocity3D, Temperature? temperature) value)
        {
            motionDataCharacteristic.SetValue($"{value.acceleration3D?.X}; {value.acceleration3D?.Y}; {value.acceleration3D?.Z}; {value.angularVelocity3D?.X}; {value.angularVelocity3D?.Y}; {value.angularVelocity3D?.Z}; {value.temperature}");
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
            environmentalDataCharacteristic = new CharacteristicString(
                name: "ENVIRONMENTAL_DATA",
                uuid: CharacteristicsConstants.ENVIRONMENTAL_DATA,
                maxLength: 20,
                permissions: CharacteristicPermission.Read,
                properties: CharacteristicProperty.Read);
            lightDataCharacteristic = new CharacteristicString(
                name: "LIGHT_DATA",
                uuid: CharacteristicsConstants.LIGHT_DATA,
                maxLength: 20,
                permissions: CharacteristicPermission.Read,
                properties: CharacteristicProperty.Read);
            motionDataCharacteristic = new CharacteristicString(
                name: "MOTION_DATA",
                uuid: CharacteristicsConstants.MOTION_DATA,
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
                environmentalDataCharacteristic,
                lightDataCharacteristic,
                motionDataCharacteristic
            );

            return new Definition("ProjectLab", service);
        }
    }
}