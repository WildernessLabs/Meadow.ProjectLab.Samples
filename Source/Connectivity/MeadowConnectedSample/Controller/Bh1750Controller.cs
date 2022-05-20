using Meadow.Foundation.Sensors.Light;
using Meadow.Hardware;
using Meadow.Units;
using MeadowConnectedSample.Connectivity;
using System;
using System.Threading.Tasks;

namespace MeadowConnectedSample.Controller
{
    public class Bh1750Controller
    {
        private static readonly Lazy<Bh1750Controller> instance =
            new Lazy<Bh1750Controller>(() => new Bh1750Controller());
        public static Bh1750Controller Instance => instance.Value;

        Bh1750 bh1750;

        public string LuminanceReading { get; private set; }

        private Bh1750Controller() { }

        public void Initialize(II2cBus i2c)
        {
            bh1750 = new Bh1750(
                    i2cBus: i2c,
                    measuringMode: Bh1750.MeasuringModes.ContinuouslyHighResolutionMode, 
                    lightTransmittance: 1, 
                    address: (byte)Bh1750.Addresses.Address_0x23);
            bh1750.Updated += Bh1750Updated;
            bh1750.StartUpdating(TimeSpan.FromSeconds(5));
        }

        private void Bh1750Updated(object sender, Meadow.IChangeResult<Illuminance> e)
        {
            LuminanceReading = $"{e.New.Lux}lx";
            BluetoothServer.Instance.SetBh1750CharacteristicValue(LuminanceReading);
        }

        public Task<Illuminance> Read()
        {
            return bh1750.Read();
        }
    }
}