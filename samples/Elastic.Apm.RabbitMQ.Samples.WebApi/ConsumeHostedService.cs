using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Hosting;

namespace Elastic.Apm.RabbitMQ.Samples.WebApi
{
    public class ConsumeHostedService : IHostedService
    {
        private readonly IBus _bus;
        private readonly EventConsumer _eventConsumer;

        public ConsumeHostedService(IBus bus, EventConsumer eventConsumer)
        {
            _bus = bus;
            _eventConsumer = eventConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _bus.SubscribeAsync<Event>("elastic_apm_sample", message => _eventConsumer.ConsumeAsync(message));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
