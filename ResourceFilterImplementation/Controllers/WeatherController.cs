using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResourceFilterImplementation.Filters;

namespace ResourceFilterImplementation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // Apply the API Key Validation resource filter to all actions in this controller
    // This ensures every request to any endpoint here requires a valid API key
    [ApiKeyValidationResourceFilter]
    public class WeatherController : ControllerBase
    {
        [HttpGet("forecast/{city}")]
        [ServiceFilter(typeof(WeatherCacheResourceFilter))]
        public IActionResult GetWeatherForecast(string city)
        {
            // Simulate fetching weather data for the specified city
            var weatherData = new
            {
                TemperatureC = new Random().Next(-10, 40),
                Condition = "Sunny",
                Timestamp = DateTime.UtcNow
            };

            var response = new
            {
                City = city,
                Data = weatherData
            };

            return Ok(response);
        }

        // GET api/weather/limited-forecast/{city}
        // Apply rate limiting filter and caching filter to this action, in that order
        // Rate limiting will reject excessive requests before caching logic is invoked
        [HttpGet("limited-forecast/{city}")]
        [ServiceFilter(typeof(RateLimitResourceFilter))]
        [ServiceFilter(typeof(WeatherCacheResourceFilter))]
        public IActionResult GetLimitedWeatherForecast(string city)
        {
            // Simulate fetching weather data with randomized temperature and current timestamp
            var weatherData = new
            {
                TemperatureC = new Random().Next(-10, 40),
                Condition = "Cloudy",
                Timestamp = DateTime.UtcNow
            };

            // Wrap the weather data with the city key, converting city name to lowercase
            var response = new
            {
                City = city.ToLower(),
                Data = weatherData
            };
            // Return HTTP 200 OK with the response JSON object
            return Ok(response);
        }

        // GET api/weather/open/{city}
        // This action only has API key validation from the controller level, no caching or rate limiting
        [HttpGet("open/{city}")]
        public IActionResult GetOpenWeatherData(string city)
        {
            // Simulate fetching weather data with randomized temperature and current timestamp
            var weatherData = new
            {
                TemperatureC = new Random().Next(-10, 40),
                Condition = "Partly Cloudy",
                Timestamp = DateTime.UtcNow
            };

            // Wrap the weather data with the city key, converting city name to lowercase
            var response = new
            {
                City = city.ToLower(),
                Data = weatherData
            };

            return Ok(response);
        }

    }
}
