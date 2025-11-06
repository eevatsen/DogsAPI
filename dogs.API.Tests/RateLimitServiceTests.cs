using dogs.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace DogsHouse.Tests;

public class RateLimitServiceTests
{
    [Fact]
    public void CanMakeRequest_FirstRequest_ShouldReturnTrue()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new RateLimitOptions { RequestsPerSecond = 3 });
        var service = new RateLimitService(cache, options);

        // Act
        var result = service.CanMakeRequest("client1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanMakeRequest_WithinLimit_ShouldReturnTrue()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new RateLimitOptions { RequestsPerSecond = 3 });
        var service = new RateLimitService(cache, options);

        // Act
        var result1 = service.CanMakeRequest("client2");
        var result2 = service.CanMakeRequest("client2");
        var result3 = service.CanMakeRequest("client2");

        // Assert
        Assert.True(result1);
        Assert.True(result2);
        Assert.True(result3);
    }

    [Fact]
    public void CanMakeRequest_OverLimit_ShouldReturnFalse()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new RateLimitOptions { RequestsPerSecond = 3 });
        var service = new RateLimitService(cache, options);

        // Act
        service.CanMakeRequest("client3");
        service.CanMakeRequest("client3");
        service.CanMakeRequest("client3");
        var result = service.CanMakeRequest("client3"); // 4th request should fail

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanMakeRequest_AfterWaiting_ShouldAllowAgain()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new RateLimitOptions { RequestsPerSecond = 2 });
        var service = new RateLimitService(cache, options);

        // Act
        service.CanMakeRequest("client4");
        service.CanMakeRequest("client4");
        var result1 = service.CanMakeRequest("client4"); // Should be false

        await Task.Delay(1100); // Wait more than 1 second

        var result2 = service.CanMakeRequest("client4"); // Should be true now

        // Assert
        Assert.False(result1);
        Assert.True(result2);
    }

    [Fact]
    public void CanMakeRequest_DifferentClients_ShouldTrackSeparately()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new RateLimitOptions { RequestsPerSecond = 2 });
        var service = new RateLimitService(cache, options);

        // Act
        service.CanMakeRequest("client5");
        service.CanMakeRequest("client5");
        var result1 = service.CanMakeRequest("client5"); // Should be false for client5

        var result2 = service.CanMakeRequest("client6"); // Should be true for client6

        // Assert
        Assert.False(result1);
        Assert.True(result2);
    }
}