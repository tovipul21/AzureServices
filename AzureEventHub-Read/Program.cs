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

            // Read messages from all partition
            //await ListenMessagesFromEventHubEventAsync(connectionString);

            // Read messages from a given partition
            await ListenMessagesFromEventHubEventByPartitionAsync(connectionString);

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

        /// <summary>
        /// Method to read messages from respective partition within Azure EventHub
        /// </summary>
        /// <param name="connectionString">Connection string to connect to Azure EventHub</param>
        /// <returns></returns>
        private static async Task ListenMessagesFromEventHubEventByPartitionAsync(string connectionString)
        {
            var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            EventHubConsumerClient consumerClient = new EventHubConsumerClient(consumerGroup, connectionString);

            var partitionId = (await consumerClient.GetPartitionIdsAsync()).First();

            //string[] allPartitions = (await consumerClient.GetPartitionIdsAsync());

            //foreach (var partitionId in allPartitions)
            //{
            IAsyncEnumerable<PartitionEvent> readEvents = consumerClient.ReadEventsFromPartitionAsync(partitionId, EventPosition.Earliest);

            await foreach (PartitionEvent _event in readEvents)
            {
                Console.WriteLine($"Partition Id - {partitionId}");
                Console.WriteLine($"Data Offset {_event.Data.Offset}");
                Console.WriteLine($"Sequence Number {_event.Data.SequenceNumber}");
                Console.WriteLine($"Partition Key {_event.Data.PartitionKey}");
                Console.WriteLine($"Data - {_event.Data.EventBody}");
            }

            //}
        }
    }
}
