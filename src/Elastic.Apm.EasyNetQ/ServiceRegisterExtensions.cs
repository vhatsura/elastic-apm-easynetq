using System;
using EasyNetQ;
using EasyNetQ.Consumer;
using EasyNetQ.DI;
using EasyNetQ.Interception;
using EasyNetQ.Logging;
using EasyNetQ.Producer;

namespace Elastic.Apm.EasyNetQ
{
    public static class ServiceRegisterExtensions
    {
        public static IInterceptorRegistrator EnableElasticApmInterceptor(this IInterceptorRegistrator interceptorRegister)
        {
            if (interceptorRegister == null) throw new ArgumentNullException(nameof(interceptorRegister));

            interceptorRegister.Add(new ElasticApmInterceptor());

            return interceptorRegister;
        }

        public static IServiceRegister EnableElasticApmTracing(this IServiceRegister serviceRegister)
        {
            if (serviceRegister == null)
            {
                throw new ArgumentNullException(nameof(serviceRegister));
            }

            serviceRegister.Register<IAdvancedBus>(c =>
                new ElasticApmAdvancedBus(c.Resolve<IConnectionFactory>(), c.Resolve<IConsumerFactory>(),
                    c.Resolve<IClientCommandDispatcherFactory>(), c.Resolve<IPublishConfirmationListener>(),
                    c.Resolve<IEventBus>(), c.Resolve<IHandlerCollectionFactory>(),
                    c.Resolve<IServiceResolver>(), c.Resolve<ConnectionConfiguration>(),
                    c.Resolve<IProduceConsumeInterceptor>(), c.Resolve<IMessageSerializationStrategy>(),
                    c.Resolve<IConventions>(), c.Resolve<AdvancedBusEventHandlers>(),
                    c.Resolve<IPersistentConnectionFactory>(), c.Resolve<ILogProvider>()));

            return serviceRegister;
        }
    }
}
