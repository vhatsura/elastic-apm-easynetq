using System;
using System.Text;
using EasyNetQ.Interception;
using Elastic.Apm.Api;

namespace Elastic.Apm.EasyNetQ
{
    public class ElasticApmInterceptor : IProduceConsumeInterceptor
    {
        public RawMessage OnProduce(RawMessage rawMessage)
        {
            if (!Agent.IsConfigured || Agent.Tracer.CurrentTransaction == null) return rawMessage;

            var currentExecutionSegment =
                Agent.Tracer.CurrentSpan ?? (IExecutionSegment) Agent.Tracer.CurrentTransaction;

            //currentExecutionSegment.OutgoingDistributedTracingData

            if (!rawMessage.Properties.Headers.ContainsKey("traceparent"))
            {
                rawMessage.Properties.Headers.Add("traceparent",
                    currentExecutionSegment.OutgoingDistributedTracingData.SerializeToString());
            }

            // todo: tracestate

            return rawMessage;
        }

        public RawMessage OnConsume(RawMessage rawMessage)
        {
            if (!Agent.IsConfigured) return rawMessage;

            if (rawMessage.Properties.Headers.ContainsKey("traceparent") &&
                rawMessage.Properties.Headers["traceparent"] is byte[] bytes)
            {
                var @string = Encoding.UTF8.GetString(bytes);
                rawMessage.Properties.Headers["traceparent"] = @string;
            }

            return rawMessage;
        }
    }
}
