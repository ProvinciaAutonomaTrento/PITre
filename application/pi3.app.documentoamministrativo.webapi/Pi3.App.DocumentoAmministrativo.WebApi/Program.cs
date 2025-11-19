// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Elastic.Apm.NetCoreAll;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.DomainEventHandlers;
using Pi3.App.DocumentoAmministrativo.WebApi.Controllers;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.Configuration;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.File.Converters;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.OracleDbContextFactory;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.Principal;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.App.DocumentoAmministrativo.WebApi.Infrastructure.Services.Uploader;
using Pi3.App.DocumentoAmministrativo.WebApi.Middleware;
using Pi3.App.Legacy.DocumentoAmministrativo.WebApi.Handler;
using Pi3.Core.AggregateModels;
using Pi3.Core.SeedWork;
using Pi3.Core.Services;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.Uploader;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Adobe;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.UploaderFS;
using Pi3.Infrastructure.Services.AAC;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var elasticApmEnabled = builder.Configuration.GetValue<bool>("ElasticApmEnabled");

if (elasticApmEnabled)
    builder.Services.AddAllElasticApm();

var environment = builder.Environment.EnvironmentName;
var isTestEnvironment = environment.Equals("Test");
var isDevelopmentEnvironment = environment.Equals("Development");
var apiPrefix = !isDevelopmentEnvironment ? "/DocumentoAmministrativo" : "";

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddHealthChecks()
    .AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

builder.Services.AddScoped<IClaimsPrincipalService, ClaimsPrincipalService>();
builder.Services.AddScoped<IEventPublisher, SyncEventPublisher>();

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Pi3.App.DocumentoAmministrativo.WebApi.Application.Behaviors.LoggerBehavior<,>), ServiceLifetime.Scoped);
});

builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events.DocumentoAmministrativoCreatedEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events.RegistrazioneRichiestaEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.ContentElementAggregate.Events.ContentElementClassificationAddedEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.ContentElementAggregate.Events.ContentElementClassificationRemovedEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentAggregate.Events.DocumentAddedInRecycleBinEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentAggregate.Events.DocumentRestoredEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events.OggettoDelDocumentoChangedEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events.AggFascicoloAddedEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events.AggFascicoloRemovedEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events.DocumentoConsolidatoEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events.MezzoSpedizioneAssignedEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events.AnnullatoEvent>, DocumentoAmministrativoEventHandlers>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.ElementAggregate.Events.ElementProfileFieldAddedEvent>, DocumentoAmministrativoEventHandlers>();

builder.Services.AddInfrastructureLegacyEFServices();
builder.Services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();
builder.Services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();
builder.Services.AddInfrastructureLegacyEFNotaAggregate();
builder.Services.AddInfrastructureLegacyEFTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFModelloTrasmissioneAggregate();

if (isDevelopmentEnvironment)
{
    // Registra configuration service mock, in ambiente di debug
    builder.Services.RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, MockConfigurationService>();
}


// Registra Infrastructure per accesso ad Oracle tramite EF
builder.Services.Configure<OracleDbContextFactoryServiceOptions>(builder.Configuration.GetSection(key: nameof(OracleDbContextFactoryServiceOptions)));
builder.Services.AddScoped<IInstanceProvider, ClaimsPrincipalScopedInstanceProvider>();
builder.Services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
builder.Services.AddScoped<IPi3DbContext>(provider => provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextEntities>(provider => provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetRequiredService<IOracleDbContextFactoryService>().CreateDbContext());

if (isTestEnvironment)
{
    builder.Services.AddScoped<IFileConverterService, MockFileConverterService>();

    builder.Services.AddDistributedMemoryCache();

    builder.Services.AddScoped<IRabbitMQService, MockRabbitMQService>();

    builder.Services.AddScoped<IUploaderService, MockFileUploaderService>();

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
    // Registra Pi3.Core e il servizio di conversione tramite Adobe
    builder.Services.AddInfrastructureAdobeFileConverter(cfg =>
    {
        cfg.ServiceUrl = builder.Configuration.GetSection("AdobePdfFileConverterServiceOptions:ServiceUrl").Value;
        cfg.ServiceCredentialsUserName = builder.Configuration.GetSection("AdobePdfFileConverterServiceOptions:ServiceCredentialsUserName").Value;
        cfg.ServiceCredentialsPassword = builder.Configuration.GetSection("AdobePdfFileConverterServiceOptions:ServiceCredentialsPassword").Value;
        cfg.PdfSettings = builder.Configuration.GetSection("AdobePdfFileConverterServiceOptions:PdfSettings").Value;
        cfg.FileTypeSettings = builder.Configuration.GetSection("AdobePdfFileConverterServiceOptions:FileTypeSettings").Value;
        cfg.SecuritySettings = builder.Configuration.GetSection("AdobePdfFileConverterServiceOptions:SecuritySettings").Value;
    });

    var redisConnectionString = builder.Configuration.GetSection("RedisOptions:ConnectionString").Get<string>();
    builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);

    builder.Services.AddInfrastructureRabbitMQ(builder.Configuration);
    builder.Services.AddInfrastructureFSUploaderService();

    // Aggiunge infrastruttura di autenticazione AAC
    builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value!);

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

var app = builder.Build();

if (elasticApmEnabled)
    app.UseAllElasticApm(builder.Configuration);

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseRouting();

if (!isTestEnvironment)
{
    var rabbitMQOptions = app.Services.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
    if (rabbitMQOptions.EnableSubscribers ?? true)
    {
        var rabbitMQService = app.Services.GetRequiredService<IRabbitMQService>();
        rabbitMQService?.Subscribe<Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.ConvertVersionToPdf.ConvertToPdfCommand>();
    }
}

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