using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Meadow.Cloud_Client.DTOs;

namespace Meadow.Cloud_Client.Services
{
    internal class CloudService
    {
        public async Task<List<QueryResponse>> GetSensorReadings()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"apikey {Secrets.API_KEY}");
                    client.Timeout = new TimeSpan(0, 5, 0);

                    HttpResponseMessage response = await client.GetAsync($"{Secrets.MEADOW_CLOUD_URL}/api/orgs/{Secrets.ORGANIZATION_ID}/search/source:event deviceId:{Secrets.DEVICE_ID} eventId:1000 size:10");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        //Resolver.Log.Info("Response Content: " + jsonString);

                        var root = JsonSerializer.Deserialize<Root>(jsonString);

                        return root?.data?.queryResponses;
                    }
                    else
                    {
                        Resolver.Log.Error("Error: " + response.StatusCode);
                        return new List<QueryResponse>();
                    }
                }
                catch (TaskCanceledException)
                {
                    Resolver.Log.Error("Request timed out.");
                    return new List<QueryResponse>();
                }
                catch (Exception e)
                {
                    Resolver.Log.Error($"Request went sideways: {e.Message}");
                    return new List<QueryResponse>();
                }
            }
        }
    }
}