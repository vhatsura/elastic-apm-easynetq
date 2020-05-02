using System;
using System.Net.Http;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Elastic.Apm.RabbitMQ.Samples.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IBus _bus;
        private readonly HttpClient _httpClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBus bus,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _bus = bus;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rng = new Random();

            await _httpClient.GetAsync("https://google.com");

            await _bus.PublishAsync(new Event
            {
                Forecast = new WeatherForecast
                {
                    Date = DateTime.Now,
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                }
            });

            return Ok();
        }
    }
}
