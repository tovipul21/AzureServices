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
            var keyVaultName = "sample-keyvault-v3";
            string secretName = "SendEventHubMessageConnectionString";
            
            Console.WriteLine("Enter secret value");
            string secretValue = Console.ReadLine();

            // Only set the secret value in Key Vault when some one updates it using console
            if (secretValue.Trim().Length > 0)
                await SetSecretIntoVaultAsync(keyVaultName, secretName, secretValue);
            
            string connectionString = await RetriveSecretFromVaultAsync(keyVaultName, secretName);
            
            await SendMessagesToEventHubEventAsync(connectionString);

            Console.WriteLine("Sent all events to eventhub process !!");
        }

        private static async Task<string> SetSecretIntoVaultAsync(string keyVaultName, string secretName, string secretValue)
        {
            KeyVaultSecret kvs;
            
            var keyVaultURI = $"https://{keyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.SetSecretAsync(secretName, secretValue);

            return kvs.Value;
        }

        private static async Task<string> RetriveSecretFromVaultAsync(string keyVaultName, string secretName)
        {
            KeyVaultSecret kvs;
            var keyVaultURI = $"https://{keyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.GetSecretAsync(secretName);

            return kvs.Value;
        }

        private static async Task SendMessagesToEventHubEventAsync(string connectionString)
        {
            EventHubProducerClient producerClient = new EventHubProducerClient(connectionString);

            List<EventData> eventDataList = new List<EventData>();
            string eventBody;

            for (int i = 1; i <= 13; i++)
            {
                Guid _guid = Guid.NewGuid();
                eventBody = $"Add event no {i} for Guid - {_guid} at {DateTime.Now.ToString()}";

                eventDataList.Add(new EventData(eventBody));
                
                Console.WriteLine($"Added event # {i} with Guid as {_guid} at {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff",
                    CultureInfo.InvariantCulture)} to event hub");

                eventBody = string.Empty;
            }

            await producerClient.SendAsync(eventDataList);
        }
    }
}
