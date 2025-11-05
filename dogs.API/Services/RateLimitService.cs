using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace dogs.Services;

// conf for rate limiting
public class RateLimitOptions
{
    public int RequestsPerSecond { get; set; } = 10;
}

// service to check if too many requests
public class RateLimitService
{
    private readonly IMemoryCache _cache;
    private readonly int _maxRequests;

    public RateLimitService(IMemoryCache cache, IOptions<RateLimitOptions> options)
    {
        _cache = cache;
        _maxRequests = options.Value.RequestsPerSecond;
    }

    // check if client can make a request
    public bool CanMakeRequest(string clientId)
    {
        string cacheKey = $"rate_limit_{clientId}";
        DateTime now = DateTime.UtcNow;

        // get list of recent requests 
        if (!_cache.TryGetValue(cacheKey, out List<DateTime>? requestTimes))
        {
            requestTimes = new List<DateTime>();
        }

        // remove old requests 
        requestTimes = requestTimes!.Where(t => (now - t).TotalSeconds < 1).ToList();

        // check if too many
        if (requestTimes.Count >= _maxRequests)
        {
            return false;
        }

        // add current 
        requestTimes.Add(now);

        // save to cache
        _cache.Set(cacheKey, requestTimes, TimeSpan.FromSeconds(2));

        return true;
    }
}