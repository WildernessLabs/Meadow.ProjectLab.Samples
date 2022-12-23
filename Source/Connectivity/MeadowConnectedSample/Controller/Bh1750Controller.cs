using Meadow.Foundation.Sensors.Light;
using Meadow.Units;
using MeadowConnectedSample.Connectivity;
using System;

namespace MeadowConnectedSample.Controller
{
    public class Bh1750Controller
    {
        private static readonly Lazy<Bh1750Controller> instance =
            new Lazy<Bh1750Controller>(() => new Bh1750Controller());
        public static Bh1750Controller Instance => instance.Value;

        Bh1750 bh1750;

        public Illuminance? IlluminanceReading { get; private set; }

        private Bh1750Controller() { }

        public void Initialize(Bh1750 sensor)
        {
            bh1750 = sensor;
            bh1750.Updated += Bh1750Updated;
            bh1750.StartUpdating(TimeSpan.FromSeconds(5));
        }

        private void Bh1750Updated(object sender, Meadow.IChangeResult<Illuminance> e)
        {
            IlluminanceReading = e?.New;
            if (BluetoothServer.Instance.IsInitialized)
            { 
                BluetoothServer.Instance.SetBh1750CharacteristicValue(IlluminanceReading); 
            }
        }
    }
}