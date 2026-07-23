using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddCarter();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(NormalizePostgresConnectionString(builder.Configuration.GetConnectionString("Database"))!);
}).UseLightweightSessions();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = NormalizeRedisConnectionString(
        builder.Configuration.GetConnectionString("Redis"));
});

builder.Services.AddScoped<BasketRepository>();
builder.Services.AddScoped<IBasketRepository>(provider =>
{
    var repository = provider.GetRequiredService<BasketRepository>();
    var cache = provider.GetRequiredService<IDistributedCache>();
    return new CachedBasketRepository(repository, cache);
});

builder.Services.AddHealthChecks()
    .AddNpgSql(NormalizePostgresConnectionString(builder.Configuration.GetConnectionString("Database"))!)
    .AddRedis(NormalizeRedisConnectionString(builder.Configuration.GetConnectionString("Redis"))!);

var app = builder.Build();

app.UseExceptionHandler(_ => { });
app.UseCors("AllowAll");
app.MapCarter();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

static string? NormalizePostgresConnectionString(string? connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return connectionString;
    }

    if (!Uri.TryCreate(connectionString, UriKind.Absolute, out var uri) ||
        (uri.Scheme != "postgres" && uri.Scheme != "postgresql"))
    {
        return connectionString;
    }

    var userInfo = uri.UserInfo.Split(':', 2);
    var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty;
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
    var database = uri.AbsolutePath.TrimStart('/');
    var parts = new List<string>
    {
        $"Host={uri.Host}",
        $"Port={uri.Port}",
        $"Database={database}",
        $"Username={username}",
        $"Password={password}"
    };

    if (uri.Query.Contains("sslmode=require", StringComparison.OrdinalIgnoreCase))
    {
        parts.Add("SSL Mode=Require");
        parts.Add("Trust Server Certificate=true");
    }

    return string.Join(";", parts);
}

static string? NormalizeRedisConnectionString(string? connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        return connectionString;
    }

    if (!Uri.TryCreate(connectionString, UriKind.Absolute, out var uri) ||
        (uri.Scheme != "redis" && uri.Scheme != "rediss"))
    {
        return connectionString;
    }

    var parts = new List<string>
    {
        $"{uri.Host}:{uri.Port}",
        "abortConnect=false"
    };

    if (!string.IsNullOrWhiteSpace(uri.UserInfo))
    {
        var userInfo = uri.UserInfo.Split(':', 2);
        var password = userInfo.Length == 2 ? userInfo[1] : userInfo[0];
        parts.Add($"password={Uri.UnescapeDataString(password)}");
    }

    if (uri.Scheme == "rediss")
    {
        parts.Add("ssl=True");
    }

    return string.Join(",", parts);
}
