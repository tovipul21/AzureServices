using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Globalization;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using Microsoft.Azure.Amqp.Framing;
using System.Collections;

namespace AzureEventHub
{ 
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start event hub process");
            string secretName = "EventHub1ConnectionString";
            
            string connectionString = await RetriveSecretFromValutAsync(secretName);
            
            await SendEnumerableOfEventAsync(connectionString);

            Console.WriteLine($"{secretName} = {connectionString}");
            Console.WriteLine("Sent all events to eventhub process !!");
        }

        private static async Task<string> RetriveSecretFromValutAsync(string secretName)
        {
            KeyVaultSecret kvs;
            var keyVaultName = "";
            var keyVaultURI = $"https://{keyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.GetSecretAsync(secretName);

            return kvs.Value;
        }

        private static async Task SendEnumerableOfEventAsync(string connectionString)
        {
            EventHubProducerClient producerClient = new EventHubProducerClient(connectionString);

            List<EventData> eventDataList = new List<EventData>();
            string eventBody;

            for (int i = 1; i <= 1750; i++)
            {
                eventBody = $"Add event no {i} at {DateTime.Now.ToString()}";

                eventDataList.Add(new EventData(eventBody));
                
                Console.WriteLine($"Added event # {i} at {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)} to event hub");

                eventBody = string.Empty;
            }

            await producerClient.SendAsync(eventDataList);
        }
    }
}
