// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Elastic.Apm.NetCoreAll;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi3.App.RubricaComune.Infrastructure.EF;
using Pi3.App.RubricaComune.Infrastructure.EF.Oracle;
using Pi3.App.RubricaComune.WebApi.Handler;
using Pi3.App.RubricaComune.WebApi.Infrastructure.Services.Principal;
using Pi3.App.RubricaComune.WebApi.Middleware;
using Pi3.Core;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Services.AAC;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var elasticApmEnabled = builder.Configuration.GetValue<bool>("ElasticApmEnabled");

if (elasticApmEnabled)
    builder.Services.AddAllElasticApm();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add API Versioning to as service to your project 
builder.Services.AddApiVersioning(config =>
{
    // Advertise the API versions supported for the particular endpoint
    config.ReportApiVersions = true;
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(a =>
{
    a.CustomSchemaIds(type => type.FullName);
    a.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
});

builder.Services.AddHealthChecks()
    .AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddScoped<IClaimsPrincipalService, HttpClaimsPrincipalService>();
builder.Services.AddPi3Core();
builder.Services.AddInfrastructureLegacyRubricaComuneEFOracleDbContext(opt =>
{
    opt.ConnectionString = builder.Configuration.GetConnectionString("RUBRICACOMUNE_INSTANCE") ?? throw new Exception("Connection string RUBRICACOMUNE_INSTANCE Assente");
    opt.EnableLogging = builder.Configuration.GetSection("OracleRubricaComuneDbContextOptions:EnableLogging").Get<bool>();
    opt.UseOracleSQLCompatibility = builder.Configuration.GetSection("OracleRubricaComuneDbContextOptions:UseOracleSQLCompatibility").Get<string>();
});

if (builder.Environment.EnvironmentName.Equals("Test") || builder.Environment.EnvironmentName.Equals("Development"))
{
    builder.Services.AddAuthentication(TestAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationDefaults.AuthenticationScheme, options => { });
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(Policies.PITRE, policy =>
          policy.AddAuthenticationSchemes("Test")
          .RequireAuthenticatedUser()
          .Build()
        );
    });

}
else
{
    // Aggiunge infrastruttura di autenticazione AAC
    builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value ?? throw new Exception("Configurazione AAC Assente"));
}

var app = builder.Build();

app.UseRouting();

if (elasticApmEnabled)
    app.UseAllElasticApm(builder.Configuration);

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(a =>
{
    a.DefaultModelsExpandDepth(-1);
});

if( !builder.Environment.EnvironmentName.Equals("Test") )
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

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
    && !context.Request.Path.StartsWithSegments(metricsPattern),
    builder =>
        builder.UseMiddleware<ClaimsPrincipalActivatorMiddleware>());

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

ThreadPool.SetMinThreads(100, 100);

app.Run();
