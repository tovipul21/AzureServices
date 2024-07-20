using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunction_AzureEventHub
{
    public class EventHub
    {
        private readonly ILogger<EventHub> _logger;

        public EventHub(ILogger<EventHub> logger)
        {
            _logger = logger;
        }

        [Function("GetMessages")]
        public void Run([EventHubTrigger("dev-eventhub-v1", Connection = "eventHub_ConnectionString")] EventData[] events)
        {
            foreach (EventData @event in events)
            {
                //_logger.LogInformation($"Event Body - {@event.SequenceNumber}");
                //_logger.LogInformation($"Partition key - {@event.PartitionKey}");
                //_logger.LogInformation($"Data Offset - {@event.Offset}");
                _logger.LogInformation($"Data Offset - {@event.EventBody}");
            }
        }
    }
}
