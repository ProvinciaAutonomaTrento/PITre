// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.OracleDbContextFactory;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;
using Pi3.App.InteropPitre.WebApi.Middleware;
using Pi3.App.InteropPitre.WebApi.Models;
using Microsoft.OpenApi.Models;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core.Services.Configuration;
using Refit;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.CommonAddressBook;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.AuthToken;
using Microsoft.AspNetCore.Authentication;
using Pi3.App.InteropPitre.WebApi.Handler;
using static Pi3.App.InteropPitre.WebApi.Handler.TestAuthenticationHandler;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.SendProof;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Mock;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Repository;
using Pi3.Infrastructure.Services.AAC;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MediatR;
using Pi3.App.InteropPitre.WebApi.Extensions;
using Pi3.Core.Services;
using Prometheus;
using Pi3.Infrastructure.Chilkat;
using Elastic.Apm.NetCoreAll;
using Pi3.App.InteropPitre.WebApi.Application.Services.RabbitMQ;
using Microsoft.Extensions.Options;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.InviaEmailTrasmissione;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
});

builder.Services.AddHealthChecks()
    .AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.Configure<SuperadminUserOptions>(builder.Configuration.GetSection("SuperadminUserOptions"));
//builder.Services.AddScoped<IClaimsPrincipalService, HttpClaimsPrincipalService>();
builder.Services.AddScoped<HttpClaimsPrincipalService>();
builder.Services.AddScoped<ClaimsPrincipalService>();

builder.Services.AddScoped<IClaimsPrincipalService>(provider =>
{
    var httpContextAccessor = provider.GetService<IHttpContextAccessor>();

    // Verifica se il contesto HTTP esiste
    if (httpContextAccessor?.HttpContext != null)
    {
        return provider.GetRequiredService<HttpClaimsPrincipalService>();
    }
    else
    {
        return provider.GetRequiredService<ClaimsPrincipalService>();
    }
});

builder.Services.AddInfrastructureLegacyEFServices();
builder.Services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();
builder.Services.AddInfrastructureLegacyEFTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFNotaAggregate();
builder.Services.AddInfrastructureLegacyEFUOCorrispondenteAggregate();
builder.Services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();
builder.Services.AddInfrastructureChilkat(opt =>
{
    opt.LicenseKey = builder.Configuration.GetSection("ChilkatOptions:LicenseKey").Value;
    opt.SpoolFolder = builder.Configuration.GetSection("ChilkatOptions:SpoolFolder").Value;
});

// Registra Infrastructure per accesso ad Oracle tramite EF
builder.Services.AddScoped<IOracleDbContextFactoryServiceOptions, OracleDbContextFactoryServiceOptions>();
builder.Services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
builder.Services.AddScoped<IPi3DbContext>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextEntities>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IAuthToken, AuthTokenService>();
builder.Services.AddScoped<IDbUtilsService, DbUtilsService>();

if (builder.Environment.EnvironmentName.Equals("Test"))
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

    // Mock per test unitari
    builder.Services.AddScoped<ICorrispondenti, MockCorrispondentiService>();
    builder.Services.AddScoped<IInteroperabilityService, MockSendProofService>();

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddInfrastructureMockRabbitMQ();
}
else
{
    // Aggiunge infrastruttura di autenticazione AAC
    builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value);

    // Aggiunta client Refit per Rubrica Comune e servizio Interoperabilit�

    builder.Services.AddRefitClient<ICorrispondenti>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("AddressBookOptions:Url").Value));

    builder.Services.AddRefitClient<IInteroperabilityService>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("InteroperabilityServiceOptions:Url").Value));

    builder.Services.AddDistributedMemoryCache();

    builder.Services.AddInfrastructureRabbitMQ(builder.Configuration);
    //var redisConnectionString = builder.Configuration.GetSection("RedisOptions:ConnectionString").Get<string>();
    //builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // a.SwaggerDoc()
    options.CustomSchemaIds(type => type.FullName);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "JWT Bearer access token for Api",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (!builder.Environment.EnvironmentName.Equals("Test"))
{
    var rabbitMQOptions = app.Services.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
    if (rabbitMQOptions.EnableSubscribers ?? true)
    {
        var rabbitMQService = app.Services.GetRequiredService<IRabbitMQService>();
        rabbitMQService?.Subscribe<InviaEmailTrasmissioneRequest>();
    }
}

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
    && !context.Request.Path.ToString().Contains(startupCheckPattern)
    && !context.Request.Path.StartsWithSegments(metricsPattern),
    builder =>
        builder.UseMiddleware<ClaimsPrincipalActivatorMiddleware>());

app.UseHttpMetrics();
app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
    endpoints.MapControllers();
});

ThreadPool.SetMinThreads(100, 100);

app.Run();
