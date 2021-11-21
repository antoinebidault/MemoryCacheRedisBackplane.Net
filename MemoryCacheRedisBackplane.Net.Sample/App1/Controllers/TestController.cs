using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MemoryCacheRedisBackplane.Net.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<TestController> _logger;
    private readonly IMemoryCache _cache;
    private IMemoryCacheInvalidator _cacheInvalidator;
    private string _cacheKey;

    public TestController(ILogger<TestController> logger, IMemoryCache cache, IMemoryCacheInvalidator cacheInvalidator)
    {
        _logger = logger;
        _cache = cache;
        _cacheInvalidator = cacheInvalidator;
        _cacheKey = "test";
    }


    [HttpGet("invalidate")]
    public IActionResult Invalidate()
    {
        _cacheInvalidator.InvalidateAsync(_cacheKey);
        return Ok();
    }

    [HttpGet("fromcache")]
    public IActionResult Get()
    {
        return Ok(GetFromCache());

    }

    private object GetFromCache()
    {
        return _cache.GetOrCreate(this._cacheKey, c =>
        {
            c.AbsoluteExpiration = DateTime.UtcNow.AddDays(1);
            return new
            {
                Data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToArray(),
                CacheTime = DateTime.Now
            };
        });
    }
}
