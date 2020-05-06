using System.Net.Http;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Logging;

namespace Elastic.Apm.RabbitMQ.Samples.WebApi
{
    public class EventConsumer : IConsumeAsync<Event>
    {
        private readonly ILogger<EventConsumer> _logger;
        private readonly HttpClient _httpClient;

        public EventConsumer(ILogger<EventConsumer> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task ConsumeAsync(Event message)
        {
            _logger.LogInformation("Processing {MessageId}", message.MessageId);

            await _httpClient.GetAsync("https://google.com");
        }
    }
}
