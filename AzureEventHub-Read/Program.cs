using Azure.Identity;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Security.KeyVault.Secrets;

namespace AzureEventHub_Read
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Console.ReadKey();

            var keyVaultName = "sample-keyvault-v3";
            string secretName = "ReadEventHubMessageConnectionString";

            string connectionString = await RetriveSecretFromVaultAsync(keyVaultName, secretName);

            await ListenMessagesFromEventHubEventAsync(connectionString);

            //Console.WriteLine(connectionString);
            Console.WriteLine("All events read successfully !!");
            Console.ReadLine();
        }

        private static async Task<string> RetriveSecretFromVaultAsync(string keyVaultName, string secretName)
        {
            KeyVaultSecret kvs;
            var keyVaultURI = $"https://{keyVaultName}.vault.azure.net/";

            var client = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential());

            kvs = await client.GetSecretAsync(secretName);

            return kvs.Value;
        }

        private static async Task ListenMessagesFromEventHubEventAsync(string connectionString)
        {
            var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            EventHubConsumerClient consumerClient = new EventHubConsumerClient(consumerGroup, connectionString);

            IAsyncEnumerable<PartitionEvent> readEvents = consumerClient.ReadEventsAsync();

            await foreach (PartitionEvent _event in readEvents)
            {
                Console.WriteLine($"Partition Id - {_event.Partition.PartitionId}");
                Console.WriteLine($"Data Offset {_event.Data.Offset}");
                Console.WriteLine($"Sequence Number {_event.Data.SequenceNumber}");
                Console.WriteLine($"Partition Key {_event.Data.PartitionKey}");
                Console.WriteLine($"Data - {_event.Data.EventBody}");
            }
        }
    }
}
