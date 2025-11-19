// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi3.App.DistributedCache.WebApi.Infrastructure.Services;
using Pi3.App.DistributedCache.WebApi.Middleware;
using Pi3.Core.Services;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddHttpContextAccessor();

// Add API Versioning to as service to your project 
builder.Services.AddApiVersioning(config =>
{
    // Advertise the API versions supported for the particular endpoint
    config.ReportApiVersions = true;
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(a =>
{
    a.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddHealthChecks()
    .AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

if (builder.Environment.EnvironmentName.Equals("Test"))
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    var redisConnectionString = builder.Configuration.GetSection("RedisOptions:ConnectionString").Get<string>();
    IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString!);
    builder.Services.AddSingleton(connectionMultiplexer);
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.ConnectionMultiplexerFactory =
            () => System.Threading.Tasks.Task.FromResult(connectionMultiplexer);
    });
    builder.Services.AddScoped<IDatabase>(cfg =>
    {
        IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConnectionString!);
        return multiplexer.GetDatabase();
    });
}

builder.Services.Configure<BasicAuthServiceOptions>(builder.Configuration.GetSection(nameof(BasicAuthServiceOptions)));
builder.Services.AddScoped<IAuthService, BasicAuthService>();

var app = builder.Build();

app.UseRouting();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(a =>
{
    a.DefaultModelsExpandDepth(-1);
});

app.UseHttpsRedirection();

const string healthCheckPattern = "/healthz";
const string readinessCheckPattern = "/readiness";
const string startupCheckPattern = "/startup";
const string metricsPattern = "/metrics";

app.MapHealthChecks(healthCheckPattern);
app.MapHealthChecks(readinessCheckPattern);
app.MapHealthChecks(startupCheckPattern);

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments(healthCheckPattern)
    && !context.Request.Path.StartsWithSegments(readinessCheckPattern)
    && !context.Request.Path.StartsWithSegments(startupCheckPattern)
    && !context.Request.Path.StartsWithSegments(metricsPattern)
    && !context.Request.Path.StartsWithSegments("/favicon.ico"),
    builder =>
        builder.UseMiddleware<BasicAuthMiddleware>());

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
