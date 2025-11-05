using dogs.Data;
using dogs.Middleware;
using dogs.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add database
builder.Services.AddDbContext<DogsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add my services
builder.Services.AddScoped<DogService>();

// Add rate limiting
builder.Services.Configure<RateLimitOptions>(
    builder.Configuration.GetSection("RateLimit"));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<RateLimitService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add rate limiting
app.UseMiddleware<RateLimitMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }