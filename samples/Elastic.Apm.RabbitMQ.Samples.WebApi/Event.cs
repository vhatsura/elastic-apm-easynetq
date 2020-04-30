using System;

namespace Elastic.Apm.RabbitMQ.Samples.WebApi
{
    public class Event
    {
        public Guid MessageId { get; } = Guid.NewGuid();
        
        public WeatherForecast Forecast { get; set; }
    }
}
