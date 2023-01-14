using Meadow.Modbus;
using System;
using System.Threading.Tasks;

namespace Meadow.Thermostats
{
    /// <summary>
    /// Encapsulates funnctionality of a Temco Controls TSTAT8 device
    /// </summary>
    public class Tstat8
    {
        private ModbusRtuClient client;
        private byte address;

        private const ushort setPointRegister = 345; // occupied setpoint, in tenths of a degree
        private const ushort tempRegister = 121; // current temp, in tenths of a degree

        public Tstat8(ModbusRtuClient client, byte address)
        {
            this.client = client;
            this.address = address;
            client.Connect();
        }

        public async Task SetOccupiedSetpoint(double setPoint)
        {
            var spValue = (ushort)(setPoint * 10);
            await client.WriteHoldingRegister(address, setPointRegister, spValue);
        }

        public async Task<double> GetOccupiedSetpoint()
        {
            Resolver.Log.Info($"getting setpoint");
            var registers = await client.ReadHoldingRegisters(address, setPointRegister, 1);
            return registers[0] / 10d;
        }

        public async Task<double> GetCurrentTemperature()
        {
            Resolver.Log.Info($"getting temp from {address}:{tempRegister}");
            try
            {
                var registers = await client.ReadHoldingRegisters(address, tempRegister, 1);
                Resolver.Log.Info($"got registers");
                return registers[0] / 10d;
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"{ex.Message}");
                return 0d;
            }
        }
    }
}