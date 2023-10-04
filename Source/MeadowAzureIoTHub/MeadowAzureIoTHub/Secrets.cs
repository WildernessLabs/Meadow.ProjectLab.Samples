namespace MeadowAzureIoTHub
{
    public class Secrets
    {
        /// <summary>
        /// Name of the WiFi network to use.
        /// </summary>
        public const string WIFI_NAME = "WIFI_NAME";

        /// <summary>
        /// Password for the WiFi network names in WIFI_NAME.
        /// </summary>
        public const string WIFI_PASSWORD = "WIFI_PASSWORD";

        /// <summary>
        /// Name of the Azure IoT Hub created
        /// </summary>
        public const string HUB_NAME = "HUB_NAME";

        /// <summary>
        /// Name of the Azure IoT Hub created
        /// </summary>
        public const string DEVICE_ID = "DEVICE_ID";

        /// <summary>
        /// example "SharedAccessSignature sr=MeadowIoTHub ..... "
        /// 
        /// az iot hub generate-sas-token --hub-name HUB_NAME --device-id DEVICE_ID --resource-group RESOURCE_GROUP --login [Open Shared access policies -> Select iothubowner -> copy Primary connection string]
        /// </summary>
        public const string SAS_TOKEN = "SAS_TOKEN";
    }
}