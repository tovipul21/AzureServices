using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using HelperServices;

namespace AzureServiceBus_Peek
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //await PeekSingleMessageFromServiceBusQueue();
            await PeekAllMessagesFromServiceBusQueue();
        }

        static async Task PeekSingleMessageFromServiceBusQueue()
        {
            IConfigurationBuilder _builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false, false);
            IConfigurationRoot _configurationRoot;
            _configurationRoot = _builder.Build();

            var queueName = _configurationRoot["ServiceBusQueueName"];

            KeyVaultService kvService = new KeyVaultService();
            string serviceBusConnectionString = await kvService.RetriveSecretFromVaultAsync(_configurationRoot["SecretName"]);

            ServiceBusClient _client = new(serviceBusConnectionString);

            ServiceBusReceiver _receiver = _client.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

            ServiceBusReceivedMessage _message = _receiver.ReceiveMessageAsync().GetAwaiter().GetResult();

            Console.WriteLine(_message.Body.ToString());
        }

        static async Task PeekAllMessagesFromServiceBusQueue()
        {
            IConfigurationBuilder _builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false, false);
            IConfigurationRoot _configurationRoot;
            _configurationRoot = _builder.Build();

            var queueName = _configurationRoot["ServiceBusQueueName"];

            KeyVaultService kvService = new KeyVaultService();
            string serviceBusConnectionString = await kvService.RetriveSecretFromVaultAsync(_configurationRoot["SecretName"]);

            ServiceBusClient _client = new(serviceBusConnectionString);

            ServiceBusReceiver _receiver = _client.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

            Console.WriteLine("Enter number of messages to be received");

            int messagesToBeReceivedOrPeeked = Int32.Parse(Console.ReadLine());

            var messages = _receiver.ReceiveMessagesAsync(messagesToBeReceivedOrPeeked).GetAwaiter().GetResult();

            foreach (var message in messages)
            {
                Console.WriteLine(message.SequenceNumber);
                Console.WriteLine(message.Body.ToString());
            }
        }
    }
}
