using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Units;
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

        private Bme688Controller() { }

        public void Initialize()
        {
            bme688 = new Bme680(MeadowApp.Device.CreateI2cBus(), (byte)Bme680.Addresses.Address_0x76);
        }

        public Task<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> Read() 
        {
            return bme688.Read();
        }
    }
}