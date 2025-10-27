// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2

using Asp.Versioning;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Pi3.App.Legacy.Uploader.WebApi.Handlers;
using Pi3.App.Uploader.WebApi.Infrastructure.Services.Configuration;
using Pi3.App.Uploader.WebApi.Infrastructure.Services.OracleDbContextFactory;
using Pi3.App.Uploader.WebApi.Infrastructure.Services.Principal;
using Pi3.App.Uploader.WebApi.Middleware;
using Pi3.Core.AggregateModels;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.UploaderFS;
using Pi3.Infrastructure.Services.AAC;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
var apiPrefix = !environment.Equals("Development") ? "/Uploader" : "";

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
})
.AddApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    options.ExampleFilters();
    options.EnableAnnotations();

    //if (!environment.Equals("Test"))
    //{
        options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
        {
            Url = $"{builder.Configuration.GetSection("SwaggerOptions:Server").Value}{apiPrefix}"
        });

        //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

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
    //}
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddHealthChecks()
    .AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

builder.Services.AddScoped<IClaimsPrincipalService, ClaimsPrincipalService>();
builder.Services.AddScoped<IEventPublisher, SyncEventPublisher>();

builder.Services.AddInfrastructureLegacyEFServices();

// Registra Infrastructure per accesso ad Oracle tramite EF
builder.Services.Configure<OracleDbContextFactoryServiceOptions>(builder.Configuration.GetSection(key: nameof(OracleDbContextFactoryServiceOptions)));
builder.Services.AddScoped<IInstanceProvider, ClaimsPrincipalScopedInstanceProvider>();
builder.Services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
builder.Services.AddScoped<IPi3DbContext>(provider => provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextEntities>(provider => provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());

if (environment.Equals("Test"))
{
    builder.Services.AddDistributedMemoryCache();

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
    var redisConnectionString = builder.Configuration.GetSection("RedisOptions:ConnectionString").Get<string>();
    builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);

    // Aggiunge infrastruttura di autenticazione AAC
    builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value!);
}

if (environment.Equals("Development") || environment.Equals("Test"))
{
    // Registra configuration service mock, in ambiente di debug
    builder.Services.RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, MockConfigurationService>();
}


builder.Services.AddInfrastructureFSUploaderService();

var app = builder.Build();

//var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseRouting();

if (elasticApmEnabled)
    app.UseAllElasticApm(builder.Configuration);

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseSwagger(options =>
{
    options.RouteTemplate = "/swagger/{documentname}/swagger.json";
});

app.UseStaticFiles();

app.UseSwaggerUI(options =>
{
    options.DefaultModelsExpandDepth(-1);
    options.DisplayOperationId();
    options.InjectJavascript("/swagger-ui/cors.js");
    options.OAuthClientId(builder.Configuration.GetSection("SwaggerOptions:OAuthClientId").Value!);
    options.OAuthUsePkce();
    options.RoutePrefix = "swagger";
});

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