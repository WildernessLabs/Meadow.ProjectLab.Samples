using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonContracts.Bluetooth;

namespace MobileMaple.ViewModel
{
    public class BluetoothViewModel : BaseViewModel
    {
        int listenTimeout = 5000;

        ushort DEVICE_ID = 253;

        IAdapter adapter;
        IService service;

        ICharacteristic pairingCharacteristic;
        ICharacteristic ledToggleCharacteristic;
        ICharacteristic ledBlinkCharacteristic;
        ICharacteristic ledPulseCharacteristic;
        ICharacteristic bme688DataCharacteristic;
        ICharacteristic bh1750DataCharacteristic;

        public ObservableCollection<IDevice> DeviceList { get; set; }

        IDevice deviceSelected;
        public IDevice DeviceSelected
        {
            get => deviceSelected;
            set { deviceSelected = value; OnPropertyChanged(nameof(DeviceSelected)); }
        }

        bool isScanning;
        public bool IsScanning
        {
            get => isScanning;
            set { isScanning = value; OnPropertyChanged(nameof(IsScanning)); }
        }

        bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set { isConnected = value; OnPropertyChanged(nameof(IsConnected)); }
        }

        bool isDeviceListEmpty;
        public bool IsDeviceListEmpty
        {
            get => isDeviceListEmpty;
            set { isDeviceListEmpty = value; OnPropertyChanged(nameof(IsDeviceListEmpty)); }
        }

        public ICommand CmdToggleConnection { get; set; }

        public ICommand CmdSearchForDevices { get; set; }

        // Onboard RGB LED
        string ledStatus;
        public string LedStatus
        {
            get => ledStatus;
            set { ledStatus = value; OnPropertyChanged(nameof(LedStatus)); }
        }
        public ICommand CmdSetOnboardLed { get; private set; }

        // BME688
        string temperature;
        public string Temperature
        {
            get => temperature;
            set { temperature = value; OnPropertyChanged(nameof(Temperature)); }
        }
        string humidity;
        public string Humidity
        {
            get => humidity;
            set { humidity = value; OnPropertyChanged(nameof(Humidity)); }
        }
        string pressure;
        public string Pressure
        {
            get => pressure;
            set { pressure = value; OnPropertyChanged(nameof(Pressure)); }
        }
        public ICommand CmdGetBme688Data { get; private set; }

        // BH1750
        string illuminance;
        public string Illuminance
        {
            get => illuminance;
            set { illuminance = value; OnPropertyChanged(nameof(Illuminance)); }
        }
        public ICommand CmdGetBh1750Data { get; private set; }

        public BluetoothViewModel()
        {
            DeviceList = new ObservableCollection<IDevice>();

            adapter = CrossBluetoothLE.Current.Adapter;
            adapter.ScanTimeout = listenTimeout;
            adapter.ScanMode = ScanMode.LowLatency;
            adapter.DeviceConnected += AdapterDeviceConnected;
            adapter.DeviceDiscovered += AdapterDeviceDiscovered;
            adapter.DeviceDisconnected += AdapterDeviceDisconnected;

            CmdToggleConnection = new Command(async () => await ToggleConnection());

            CmdSearchForDevices = new Command(async () => await SearchForDevices());

            CmdSetOnboardLed = new Command(async (obj) => await SetOnboardLed(obj as string));

            CmdGetBme688Data = new Command(async () => await GetBme688Data());

            CmdGetBh1750Data = new Command(async () => await GetBh1750Data());
        }

        void AdapterDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            IsConnected = false;
        }

        async void AdapterDeviceConnected(object sender, DeviceEventArgs e)
        {
            IsConnected = true;

            IDevice device = e.Device;

            var services = await device.GetServicesAsync();

            foreach (var serviceItem in services)
            {
                if (UuidToUshort(serviceItem.Id.ToString()) == DEVICE_ID)
                {
                    service = serviceItem;
                }
            }

            pairingCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.PAIRING));
            ledToggleCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.LED_TOGGLE));
            ledBlinkCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.LED_BLINK));
            ledPulseCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.LED_PULSE));
            bme688DataCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.BME688_DATA));
            bh1750DataCharacteristic = await service.GetCharacteristicAsync(Guid.Parse(CharacteristicsConstants.BH1750_DATA));

            SetPairingStatus();
        }

        async void AdapterDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if (DeviceList.FirstOrDefault(x => x.Name == e.Device.Name) == null &&
                !string.IsNullOrEmpty(e.Device.Name))
            {
                DeviceList.Add(e.Device);
            }

            if (e.Device.Name == "ProjectLab")
            {
                await adapter.StopScanningForDevicesAsync();
                IsDeviceListEmpty = false;
                DeviceSelected = e.Device;
            }
        }

        async Task ScanTimeoutTask()
        {
            await Task.Delay(listenTimeout);
            await adapter.StopScanningForDevicesAsync();
            IsScanning = false;
        }

        async Task ToggleConnection()
        {
            try
            {
                if (IsConnected)
                {
                    IsConnected = false;
                    await SetPairingStatus();
                    await adapter.DisconnectDeviceAsync(DeviceSelected);

                }
                else
                {
                    await adapter.ConnectToDeviceAsync(DeviceSelected);
                    IsConnected = true;
                }
            }
            catch (DeviceConnectionException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        async Task SearchForDevices()
        {
            try
            {
                IsScanning = true;

                var tasks = new Task[]
                {
                    ScanTimeoutTask(),
                    adapter.StartScanningForDevicesAsync()
                };

                await Task.WhenAny(tasks);
            }
            catch (DeviceConnectionException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        async Task SetOnboardLed(string command)
        {
            byte[] array = new byte[1];

            switch (command)
            {
                case "toggle":
                    array[0] = 1;
                    await ledToggleCharacteristic.WriteAsync(array);
                    LedStatus = "Toggled";
                    break;

                case "blink":
                    await ledBlinkCharacteristic.WriteAsync(array);
                    LedStatus = "Blinking";
                    break;

                case "pulse":
                    await ledPulseCharacteristic.WriteAsync(array);
                    LedStatus = "Pulsing";
                    break;
            }
        }

        async Task GetBme688Data()
        {
            var value = System.Text.Encoding.Default.GetString(await bme688DataCharacteristic.ReadAsync()).Split(';');

            Temperature = value[0];
            Humidity = value[1];
            Pressure = value[2];
        }

        async Task GetBh1750Data()
        {
            Illuminance = System.Text.Encoding.Default.GetString(await bh1750DataCharacteristic.ReadAsync()).Split(';')[0];
        }

        async Task SetPairingStatus()
        {
            byte[] array = new byte[1];
            array[0] = IsConnected ? (byte)1 : (byte)0;

            await pairingCharacteristic.WriteAsync(array);
        }

        protected int UuidToUshort(string uuid)
        {
            return int.Parse(uuid.Substring(4, 4), System.Globalization.NumberStyles.HexNumber); ;
        }
    }
}