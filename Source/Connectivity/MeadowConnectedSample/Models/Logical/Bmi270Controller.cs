using Meadow.Foundation.Sensors.Accelerometers;
using Meadow.Foundation.Sensors.Light;
using Meadow.Units;
using MeadowConnectedSample.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeadowConnectedSample.Models.Logical
{
    public class Bmi270Controller
    {
        private static readonly Lazy<Bmi270Controller> instance =
            new Lazy<Bmi270Controller>(() => new Bmi270Controller());
        public static Bmi270Controller Instance => instance.Value;

        Bmi270 bmi270;

        public Illuminance? IlluminanceReading { get; private set; }

        private Bmi270Controller() { }

        public void Initialize(Bmi270 sensor)
        {
            bmi270 = sensor;
            //bmi270.Updated += Bh1750Updated;
            //bmi270.StartUpdating(TimeSpan.FromSeconds(5));
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