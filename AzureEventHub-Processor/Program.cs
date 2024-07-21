using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using HelperServices;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace AzureEventHub_Processor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await ListenMessageFromEventHub();
        }

        static async Task ListenMessageFromEventHub()
        {
            IConfigurationBuilder _builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false, false);
            IConfigurationRoot _configurationRoot;
            _configurationRoot = _builder.Build();

            var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
            var storageaccountConnectionString = _configurationRoot["StorageAccountConnectionString"];
            var containerName = "readevents";
            KeyVaultService kvService = new KeyVaultService();
            string eventHubConnectionString = await kvService.RetriveSecretFromVaultAsync();

            BlobContainerClient blobContainerClient = new(storageaccountConnectionString, containerName);
            EventProcessorClient eventProcessorClient = new(blobContainerClient, consumerGroup, eventHubConnectionString);

            eventProcessorClient.ProcessErrorAsync += OurErrorHandler;
            eventProcessorClient.ProcessEventAsync += OurEventHandler;

            await eventProcessorClient.StartProcessingAsync();

            await Task.Delay(TimeSpan.FromSeconds(30));

            Console.ReadKey();
        }

        static async Task OurEventHandler(ProcessEventArgs eventArgs)
        {
            Console.WriteLine($"Sequence Number - {eventArgs.Data.SequenceNumber}");
            Console.WriteLine(Encoding.UTF8.GetString(eventArgs.Data.EventBody));
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task OurErrorHandler(ProcessErrorEventArgs errorEventArgs)
        {
            Console.WriteLine(errorEventArgs.Exception);
            return Task.CompletedTask;
        }
    }
}
