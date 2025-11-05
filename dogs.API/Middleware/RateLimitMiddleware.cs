using dogs.Services;

namespace dogs.Middleware;

// middleware to check rate limits
public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimitService _rateLimitService;

    public RateLimitMiddleware(RequestDelegate next, RateLimitService rateLimitService)
    {
        _next = next;
        _rateLimitService = rateLimitService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // get client IP 
        string clientId = GetClientIp(context);

        // check if client can make request
        if (!_rateLimitService.CanMakeRequest(clientId))
        {
            context.Response.StatusCode = 429; 
            await context.Response.WriteAsync("Too many requests. Please try again later.");
            return;
        }

        await _next(context);
    }

    private string GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // get direct IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}