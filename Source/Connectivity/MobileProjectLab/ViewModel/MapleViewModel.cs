using CommonContracts.Models;
using Meadow.Foundation.Web.Maple.Client;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace MobileProjectLab.ViewModel
{
    public class MapleViewModel : BaseViewModel
    {
        public MapleClient client { get; private set; }

        int _serverPort;
        public int ServerPort
        {
            get => _serverPort;
            set { _serverPort = value; OnPropertyChanged(nameof(ServerPort)); }
        }

        bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set { _isScanning = value; OnPropertyChanged(nameof(IsScanning)); }
        }

        bool _isServerListEmpty;
        public bool IsServerListEmpty
        {
            get => _isServerListEmpty;
            set { _isServerListEmpty = value; OnPropertyChanged(nameof(IsServerListEmpty)); }
        }

        string ipAddress;
        public string IpAddress
        {
            get => ipAddress;
            set { ipAddress = value; OnPropertyChanged(nameof(IpAddress)); }
        }

        ServerModel _selectedServer;
        public ServerModel SelectedServer
        {
            get => _selectedServer;
            set
            {
                if (value == null) return;
                _selectedServer = value;
                IpAddress = _selectedServer.IpAddress;
                OnPropertyChanged(nameof(SelectedServer));
            }
        }

        public ObservableCollection<ServerModel> HostList { get; set; }

        public ICommand CmdSearchServers { get; private set; }

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

        public MapleViewModel()
        {
            HostList = new ObservableCollection<ServerModel>();

            ServerPort = 5417;

            client = new MapleClient();
            client.Servers.CollectionChanged += ServersCollectionChanged;

            CmdSearchServers = new Command(async () => await GetServers());

            CmdSetOnboardLed = new Command(async (obj) => await SetOnboardLed(obj as string));

            CmdGetBme688Data = new Command(async () => await GetBme688Data());

            CmdGetBh1750Data = new Command(async () => await GetBh1750Data());
        }

        void ServersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ServerModel server in e.NewItems)
                    {
                        HostList.Add(new ServerModel() { Name = $"{server.Name} ({server.IpAddress})", IpAddress = server.IpAddress });
                        Console.WriteLine($"'{server.Name}' @ ip:[{server.IpAddress}]");
                    }
                    break;
            }
        }

        async Task GetServers()
        {
            if (IsScanning)
                return;
            IsScanning = true;

            try
            {
                IsServerListEmpty = false;

                await client.StartScanningForAdvertisingServers();

                //HostList.Add(new ServerModel() { Name = "Meadow (192.168.1.81)", IpAddress = "192.168.1.81" });

                if (HostList.Count == 0)
                {
                    IsServerListEmpty = true;
                }
                else
                {
                    IsServerListEmpty = false;
                    SelectedServer = HostList[0];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsScanning = false;
            }
        }

        async Task SetOnboardLed(string command)
        {
            if (SelectedServer == null && string.IsNullOrEmpty(IpAddress))
                return;

            try
            {
                bool response = await client.PostAsync(SelectedServer != null ? SelectedServer.IpAddress : IpAddress, ServerPort, command, string.Empty);

                if (response)
                {
                    switch (command)
                    {
                        case "toggle": LedStatus = "Toggled"; break;
                        case "blink": LedStatus = "Blinking"; break;
                        case "pulse": LedStatus = "Pulsing"; break;
                    }
                }
                else
                {
                    Console.WriteLine("Request failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task GetBme688Data()
        {
            if (SelectedServer == null && string.IsNullOrEmpty(IpAddress))
                return;

            try
            {
                var response = await client.GetAsync(SelectedServer != null ? SelectedServer.IpAddress : IpAddress, ServerPort, "getbme688data", null, null);

                if (response == null)
                {
                    return;
                }

                var value = System.Text.Json.JsonSerializer.Deserialize<ClimateModel>(response);

                Temperature = value.Temperature;
                Humidity = value.Humidity;
                Pressure = value.Pressure;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        async Task GetBh1750Data()
        {
            if (SelectedServer == null && string.IsNullOrEmpty(IpAddress))
                return;

            try
            {
                var response = await client.GetAsync(SelectedServer != null ? SelectedServer.IpAddress : IpAddress, ServerPort, "getbh1750data", null, null);

                if (response == null)
                    return;

                var value = System.Text.Json.JsonSerializer.Deserialize<IlluminanceModel>(response);

                Illuminance = value.Illuminance;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}