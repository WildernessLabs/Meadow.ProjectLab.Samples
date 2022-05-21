using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Hardware;
using Meadow.Units;
using MeadowConnectedSample.Connectivity;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample.Controller
{
    public class Bme688Controller
    {
        private static readonly Lazy<Bme688Controller> instance =
            new Lazy<Bme688Controller>(() => new Bme688Controller());
        public static Bme688Controller Instance => instance.Value;

        Bme680 bme688;

        public string AmbientReading { get; private set; }

        private Bme688Controller() { }

        public void Initialize(II2cBus i2c)
        {
            bme688 = new Bme680(i2c, (byte)Bme680.Addresses.Address_0x76);
            //bme688.Updated += Bme688Updated;
            //bme688.StartUpdating(TimeSpan.FromSeconds(5));
        }

        private void Bme688Updated(object sender, Meadow.IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> e)
        {
            AmbientReading = $"{(int)e.New.Temperature?.Celsius}°C;{(int)e.New.Humidity?.Percent}%;{(int)e.New.Pressure?.Millibar}mbar";
            BluetoothServer.Instance.SetBme688CharacteristicValue(AmbientReading);
        }

        public Task<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> Read() 
        {
            return bme688.Read();
        }
    }
}