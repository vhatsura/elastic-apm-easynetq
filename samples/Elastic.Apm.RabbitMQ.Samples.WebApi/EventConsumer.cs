using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Logging;

namespace Elastic.Apm.RabbitMQ.Samples.WebApi
{
    public class EventConsumer : IConsumeAsync<Event>
    {
        private readonly ILogger<EventConsumer> _logger;

        public EventConsumer(ILogger<EventConsumer> logger)
        {
            _logger = logger;
        }

        public Task ConsumeAsync(Event message)
        {
            _logger.LogInformation("Processing {MessageId}", message.MessageId);

            return Task.CompletedTask;
        }
    }
}
