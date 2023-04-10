using Amqp;
using Meadow;
using Meadow.Units;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MeadowAzureIoTHub.Azure
{
    /// <summary>
    /// You'll need to create an IoT Hub - https://learn.microsoft.com/en-us/azure/iot-hub/iot-hub-create-through-portal
    /// Create a device within your IoT Hub
    /// And then generate a SAS token - this can be done via the Azure CLI 
    /// 
    /// Example
    /// az iot hub generate-sas-token
    /// --hub-name HUB_NAME 
    /// --device-id DEVICE_ID 
    /// --resource-group RESOURCE_GROUP 
    /// --login [Open Shared access policies -> Select iothubowner -> copy Primary connection string]
    /// </summary>
    public class AmqpController
    {
        private static readonly Random rand = new Random();

        private const string HubName = Secrets.HUB_NAME;
        private const string SasToken = Secrets.SAS_TOKEN;
        private const string DeviceId = Secrets.DEVICE_ID;

        private Connection connection;
        private SenderLink sender;
        private ReceiverLink receiver;

        public AmqpController() { }

        public async Task Initialize()
        {
            string hostName = HubName + ".azure-devices.net";
            string userName = DeviceId + "@sas." + HubName;
            string senderAddress = "devices/" + DeviceId + "/messages/events";
            string receiverAddress = "devices/" + DeviceId + "/messages/deviceBound";

            Resolver.Log.Info("Create connection factory...");
            var factory = new ConnectionFactory();

            Resolver.Log.Info("Create connection ...");
            connection = await factory.CreateAsync(new Address(hostName, 5671, userName, SasToken));

            Resolver.Log.Info("Create session ...");
            var session = new Session(connection);

            Resolver.Log.Info("Create SenderLink ...");
            sender = new SenderLink(session, "send-link", senderAddress);

            Resolver.Log.Info("Create ReceiverLink ...");
            receiver = new ReceiverLink(session, "receive-link", receiverAddress);
            receiver.Start(100, OnMessage);
        }

        public Task SendEnvironmentalReading((Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure, Resistance? GasResistance) reading)
        {
            try
            {
                Resolver.Log.Info("Create payload");
                string messagePayload = $"{{\"Temperature\":{reading.Temperature.Value.Celsius}," +
                    $"\"Humidity\":{reading.Humidity.Value.Percent}," +
                    $"\"Pressure\":{reading.Pressure.Value.Millibar}}}";

                Resolver.Log.Info("Create message");
                var message = new Message(Encoding.UTF8.GetBytes(messagePayload));
                message.ApplicationProperties = new Amqp.Framing.ApplicationProperties();

                Resolver.Log.Info("Send message");
                sender.Send(message, null, null);

                Resolver.Log.Info($"*** DATA SENT - Temperature - {reading.Temperature.Value.Celsius}, Humidity - {reading.Humidity.Value.Percent}, Pressure - {reading.Pressure.Value.Millibar} ***");
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"-- D2C Error - {ex.Message} --");
            }

            return Task.CompletedTask;
        }

        private static void OnMessage(IReceiverLink receiver, Message message)
        {
            Resolver.Log.Info("Message received");

            try
            {
                //double.TryParse((string)message.ApplicationProperties["setlat"], out latitude);
                //double.TryParse((string)message.ApplicationProperties["setlon"], out longitude);
                //Resolver.Log.Info($"== Received new Location setting: Lat - {latitude}, Lon - {longitude} ==");
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"-- C2D Error - {ex.Message} --");
            }
        }
    }
}