using Meadow.Foundation.Sensors.Light;
using Meadow.Hardware;
using Meadow.Units;
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

        private Bh1750Controller() { }

        public void Initialize(II2cBus i2c)
        {
            bh1750 = new Bh1750(
                    i2cBus: i2c,
                    measuringMode: Bh1750.MeasuringModes.ContinuouslyHighResolutionMode, // the various modes take differing amounts of time.
                    lightTransmittance: 1, // lower this to increase sensitivity, for instance, if it's behind a semi opaque window
                    address: (byte)Bh1750.Addresses.Address_0x23
                );
        }

        public Task<Illuminance> Read()
        {
            return bh1750.Read();
        }
    }
}