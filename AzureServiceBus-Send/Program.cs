using Microsoft.Extensions.Configuration;
using HelperServices;
using Azure.Messaging.ServiceBus;

namespace AzureServiceBus_Send
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await SendMessageToServiceBusQueue();
        }

        static async Task SendMessageToServiceBusQueue()
        {
            IConfigurationBuilder _builder = new ConfigurationBuilder().AddJsonFile("appSettings.json", false, false);
            IConfigurationRoot _configurationRoot;
            _configurationRoot = _builder.Build();

            var queueName = _configurationRoot["ServiceBusQueueName"];
            KeyVaultService kvService = new KeyVaultService();
            
            string eventHubConnectionString = await kvService.RetriveSecretFromVaultAsync();

            Console.WriteLine("Enter the number of messages to be pushed into the queue");
            int messageCountToBePushed = Int32.Parse(Console.ReadLine());

            // Add messages into the queue
            List<Order> orders = new List<Order>();

            for (int i = 0; i < messageCountToBePushed; i++)
            {
                orders.Add(new Order()
                { 
                    OrderId = new Random().Next(0, 100), 
                    Quantity = new Random().Next(0, 10),
                    UnitPrice = new Decimal(new Random().NextDouble()) 
                });
            }

            ServiceBusClient _client = new(eventHubConnectionString);

            ServiceBusSender _sender = _client.CreateSender(queueName);

            foreach (var order in orders)
            {
                ServiceBusMessage _message = new ServiceBusMessage(order.ToString());

                _message.ContentType = "application/json";
                _sender.SendMessageAsync(_message).GetAwaiter().GetResult();
            }

            Console.WriteLine($"All the {orders.Count} messages has been sent.");
        }
    }
}
