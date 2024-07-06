using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Azure.Security.KeyVault.Secrets;
using System.Globalization;

namespace AzureEventHub
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start event hub process");
            var keyVaultName = "samplekeyvaultv1";
            string secretName = "EventHub1ConnectionString";
            
            Console.WriteLine("Enter secret value");
            string secretValue = Console.ReadLine();
            
            await SetSecretIntoValutAsync(keyVaultName, secretName, secretValue);

            string connectionString = await RetriveSecretFromValutAsync(keyVaultName, secretName);
            
            await SendEnumerableOfEventAsync(connectionString);

            Console.WriteLine("Sent all events to eventhub process !!");
        }

        private static async Task<string> SetSecretIntoValutAsync(string keyVaultName, string secretName, string secretValue)
        {
            KeyVaultSecret kvs;
            
            var keyVaultURI = $"https://{keyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.SetSecretAsync(secretName, secretValue);

            return kvs.Value;
        }

        private static async Task<string> RetriveSecretFromValutAsync(string keyVaultName, string secretName)
        {
            KeyVaultSecret kvs;
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
