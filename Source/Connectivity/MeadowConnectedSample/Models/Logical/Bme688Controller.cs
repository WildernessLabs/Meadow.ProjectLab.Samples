﻿using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Hardware;
using Meadow.Units;
using MeadowConnectedSample.Connectivity;
using System;

namespace MeadowConnectedSample.Models.Logical
{
    public class Bme688Controller
    {
        private static readonly Lazy<Bme688Controller> instance =
            new Lazy<Bme688Controller>(() => new Bme688Controller());
        public static Bme688Controller Instance => instance.Value;

        Bme688 bme688;

        public (Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) AmbientReading { get; private set; }

        private Bme688Controller() { }

        public void Initialize(Bme688 sensor)
        {
            bme688 = sensor;
            bme688.Updated += Bme688Updated;
            bme688.StartUpdating(TimeSpan.FromSeconds(5));
        }

        private void Bme688Updated(object sender, Meadow.IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance)> e)
        {
            AmbientReading = e.New;
            if (BluetoothServer.Instance.IsInitialized)
            {
                BluetoothServer.Instance.SetBme688CharacteristicValue(AmbientReading);
            }
        }
    }
}