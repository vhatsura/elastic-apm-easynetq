using System;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.DI;
using EasyNetQ.Interception;
using EasyNetQ.Logging;
using EasyNetQ.Producer;
using EasyNetQ.Topology;
using Elastic.Apm.Api;

namespace Elastic.Apm.EasyNetQ
{
    public class ElasticApmAdvancedBus : RabbitAdvancedBus
    {
        private readonly ILogProvider _logProvider;

        public ElasticApmAdvancedBus(IConnectionFactory connectionFactory, IConsumerFactory consumerFactory,
            IClientCommandDispatcherFactory clientCommandDispatcherFactory,
            IPublishConfirmationListener confirmationListener, IEventBus eventBus,
            IHandlerCollectionFactory handlerCollectionFactory, IServiceResolver container,
            ConnectionConfiguration connectionConfiguration, IProduceConsumeInterceptor produceConsumeInterceptor,
            IMessageSerializationStrategy messageSerializationStrategy, IConventions conventions,
            AdvancedBusEventHandlers advancedBusEventHandlers, IPersistentConnectionFactory persistentConnectionFactory,
            ILogProvider logProvider)
            : base(connectionFactory, consumerFactory, clientCommandDispatcherFactory, confirmationListener, eventBus,
                handlerCollectionFactory, container, connectionConfiguration, produceConsumeInterceptor,
                messageSerializationStrategy, conventions, advancedBusEventHandlers, persistentConnectionFactory)
        {
            _logProvider = logProvider;
        }

        public override IDisposable Consume(IQueue queue,
            Func<byte[], MessageProperties, MessageReceivedInfo, Task> onMessage,
            Action<IConsumerConfiguration> configure)
        {
            async Task NewOnMessage(byte[] body, MessageProperties properties, MessageReceivedInfo receivedInfo)
            {
                // MDC (mapped diagnostic context) definition: https://www.baeldung.com/mdc-in-log4j-2-logback
                // NDC (nested diagnostic context) definition: https://www.baeldung.com/java-logging-ndc-log4j
                // using var disposable = _logProvider.OpenMappedContext("CorrelationId",
                //     properties.CorrelationIdPresent ? properties.CorrelationId : Guid.NewGuid().ToString());

                if (!Agent.IsConfigured)
                {
                    await onMessage(body, properties, receivedInfo).ConfigureAwait(false);

                    return;
                }

                DistributedTracingData tracingData = null;

                if (properties.Headers.ContainsKey("traceparent") &&
                    properties.Headers["traceparent"] is string traceparent)
                {
                    tracingData = DistributedTracingData.TryDeserializeFromString(traceparent);
                }

                var transaction = Agent.Tracer.StartTransaction(receivedInfo.Queue, "event", tracingData);

                try
                {
                    await onMessage(body, properties, receivedInfo).ConfigureAwait(false);
                }
                finally
                {
                    transaction.End();
                }
            }

            return base.Consume(queue, NewOnMessage, configure);
        }
    }
}
