// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Authentication;
using Pi3.App.AggregazioneDocumentale.WebApi.Handlers;
using Pi3.App.AggregazioneDocumentale.WebApi.Infrastructure.Services.Principal;
using Pi3.App.AggregazioneDocumentale.WebApi.Middleware;
using Pi3.Core.Services.Principal;
using Pi3.Core;
using Pi3.Infrastructure.Services.AAC;
using System.Reflection;
using System.Text.Json.Serialization;
using Serilog;
using Pi3.App.AggregazioneDocumentale.WebApi.Infrastructure.Services.OracleDbContextFactory;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.App.AggregazioneDocumentale.WebApi.Services.OracleDbContextFactory;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Elastic.Apm.NetCoreAll;

var builder = WebApplication.CreateBuilder(args);

var elasticApmEnabled = builder.Configuration.GetValue<bool>("ElasticApmEnabled");

if (elasticApmEnabled)
    builder.Services.AddAllElasticApm();

var environment = builder.Environment.EnvironmentName;
var isTestEnvironment = environment.Equals("Test");
var isDevelopmentEnvironment = environment.Equals("Development");
var apiPrefix = !isDevelopmentEnvironment ? "/AggregazioneDocumentale" : "";


// Add services to the container.
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

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
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

if (!isTestEnvironment)
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.FullName);
        options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
        options.ExampleFilters();
        options.EnableAnnotations();
        options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
        {
            Url = $"{builder.Configuration.GetSection("SwaggerOptions:Server").Value}{apiPrefix}"
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "OAuth2.0 Auth Code with PKCE",
            Name = "oauth2",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(builder.Configuration.GetSection("SwaggerOptions:AuthorizationUrl").Value!),
                    TokenUrl = new Uri(builder.Configuration.GetSection("SwaggerOptions:TokenUrl").Value!)
                }
            }
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                },
                "profile email user.roles.me openid".Split(" ")
            }
        });
    });

    builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
}

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddHealthChecks()
    .AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Pi3.App.AggregazioneDocumentale.WebApi.Application.Behaviors.LoggerBehavior<,>), ServiceLifetime.Scoped);
});

builder.Services.AddScoped<IClaimsPrincipalService, HttpClaimsPrincipalService>();
builder.Services.AddPi3Core();

// Registra Infrastructure per accesso ad Oracle tramite EF
builder.Services.Configure<OracleDbContextFactoryServiceOptions>(builder.Configuration.GetSection(key: nameof(OracleDbContextFactoryServiceOptions)));
builder.Services.AddScoped<IInstanceProvider, HttpContextAccessorInstanceProvider>();
builder.Services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
builder.Services.AddScoped<IPi3DbContext>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextEntities>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());

builder.Services.AddInfrastructureLegacyEFAggregazioneDocumentaleAggregate();
builder.Services.AddInfrastructureLegacyEFNotaAggregate();
builder.Services.AddInfrastructureLegacyEFTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFModelloTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFServices();

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

    builder.Services.AddDistributedMemoryCache();
}
else
{
    // Aggiunge infrastruttura di autenticazione AAC
    builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value ?? throw new Exception("Configurazione AAC Assente"));

    var redisConnectionString = builder.Configuration.GetSection("RedisOptions:ConnectionString").Get<string>();
    builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);
}

var app = builder.Build();

app.UseRouting();

if (elasticApmEnabled)
    app.UseAllElasticApm(builder.Configuration);

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseMiddleware<ErrorHandlerMiddleware>();

if (!isTestEnvironment)
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/swagger/{documentname}/swagger.json";
    });
}

app.UseStaticFiles();

if (!isTestEnvironment)
{
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);
        options.DisplayOperationId();
        options.InjectJavascript($"{apiPrefix}/swagger-ui/cors.js");
        options.OAuthClientId(builder.Configuration.GetSection("SwaggerOptions:OAuthClientId").Value!);
        options.OAuthUsePkce();
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

const string healthCheckPattern = "/healthz";
const string readinessCheckPattern = "/readiness";
const string startupCheckPattern = "/startup";

app.MapHealthChecks(healthCheckPattern);
app.MapHealthChecks(readinessCheckPattern);
app.MapHealthChecks(startupCheckPattern);

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments(healthCheckPattern)
    && !context.Request.Path.StartsWithSegments(readinessCheckPattern)
    && !context.Request.Path.StartsWithSegments(startupCheckPattern)
    && !context.Request.Path.ToString().Contains(startupCheckPattern)
    && !context.Request.Path.StartsWithSegments("/swagger-ui/cors.js"),
    builder =>
        builder.UseMiddleware<ClaimsPrincipalActivatorMiddleware>());


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

ThreadPool.SetMinThreads(100, 100);

app.Run();