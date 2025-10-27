// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Principal;
using Pi3.Core;
using System.Text.Json.Serialization;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Behaviors;
using System.Reflection;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.OracleDbContextFactory;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Serilog;
using Pi3.Core.Services;
using Pi3.App.Legacy.Pis.WebApi.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi3.Infrastructure.BouncyCastle;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core.Services.Configuration;
using Refit;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.WebMethodLogger;
using Pi3.Infrastructure.Chilkat;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Interoperability;
using DocsPaVO.Mobile.Requests;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.LibroFirma;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SpedizioneInteropPiTre;
using Pi3.Infrastructure.DocumentFormat.OpenXml;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.DistributedCache;
using System.Text;
using Microsoft.Net.Http.Headers;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Aac;
using Microsoft.Extensions.Options;
using Pi3.App.Legacy.Pis.WebApi;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.InviaEmailTrasmissione;
using Pi3.Core.SeedWork;
using Pi3.App.Legacy.Pis.WebApi.Application.DomainEventHandlers.TrasmissioneInviataEvent;
using Pi3.Infrastructure.Graph;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentMetadata.DocumentMetadataManagement;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Infrastructure.Legacy.UploaderFS;
using Elastic.Apm.NetCoreAll;
using System.Text.Json.Serialization.Metadata;
using Pi3.Infrastructure.IText.Decorator;
using Pi3.Core.Services.File.Converters;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.File.Converters;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.File.FirmaDigitale2;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.File.FirmaRemota2;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.File.MarcaTemporale;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.File.SigilloElettronico;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.File.FirmaRemota2;
using Pi3.Core.Services.File.MarcaTemporale;
using Pi3.Core.Services.File.SigilloElettronico;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
var elasticApmEnabled = builder.Configuration.GetValue<bool>("ElasticApmEnabled");

if (elasticApmEnabled)
    builder.Services.AddAllElasticApm();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 80 * 1024 * 1024;
});

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Pi3.App.Legacy.Pis.WebApi.Application.Behaviors.LoggerBehavior<,>), ServiceLifetime.Scoped);
});
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.TrasmissioneAggregate.Events.TrasmissioneInviataEvent>, TrasmissioneInviataEventHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.MaxDepth = 64;
    options.JsonSerializerOptions.Converters.Add(new Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.FilterTypeConverter());
    options.JsonSerializerOptions.Converters.Add(new Pi3.App.Legacy.Pis.WebApi.Infrastructure.Converters.NullValuesConverter<bool>());
    options.JsonSerializerOptions.Converters.Add(new Pi3.App.Legacy.Pis.WebApi.Infrastructure.Converters.NullValuesConverter<int>());
    options.JsonSerializerOptions.Converters.Add(new Pi3.App.Legacy.Pis.WebApi.Infrastructure.Converters.NullValuesConverter<long>());

});

builder.Services.AddSingleton<IRoutingUtils, RoutingUtils>();
builder.Services.AddScoped<IFileConverterService, MockFileConverterService>();
builder.Services.AddScoped<IFirmaDigitale2Service, MockFirmaDigitale2Service>();
builder.Services.AddScoped<IFirmaRemota2Service, MockFirmaRemota2Service>();
builder.Services.AddScoped<IMarcaTemporaleService, MockMarcaTemporaleService>();
builder.Services.AddScoped<ISigilloElettronicoService, MockSigilloElettronicoService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(a =>
{
    a.CustomSchemaIds(type => type.FullName);
    var xmlSwagger = Path.Combine(AppContext.BaseDirectory, "Pi3.App.Legacy.Pis.WebApi.xml");
    a.IncludeXmlComments(xmlSwagger);
});

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

builder.Services.AddPi3Core();

if (builder.Environment.EnvironmentName.Equals("Test"))
{
    builder.Services.AddDistributedMemoryCache();
}
else
{
    var redisConnectionString = builder.Configuration.GetSection("RedisOptions:ConnectionString").Get<string>();
    builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnectionString);
}

builder.Services.AddInfrastructureITextFileDecorator();
builder.Services.AddInfrastructureLegacyEFServices();
builder.Services.AddInfrastructureLegacyEFAggregazioneDocumentaleAggregate();
builder.Services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();
builder.Services.AddInfrastructureLegacyEFModelloTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFTrasmissioneAggregate();
builder.Services.AddInfrastructureBouncyCastle();
builder.Services.AddInfrastructureLegacyEFNotaAggregate();
builder.Services.AddInfrastructureLegacyEFNotaRFAggregate();
builder.Services.AddInfrastructureOpenXmlReportGeneratorService();

// Registra Infrastructure per accesso ad Oracle tramite EF
builder.Services.AddInfrastructureLegacyEFOracleDbContextFactory();

// Registra Infrastructure per accesso al repository dei documenti su file system
builder.Services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();
builder.Services.AddInfrastructureGraph();
builder.Services.AddInfrastructureChilkat(opt =>
{
    opt.LicenseKey = builder.Configuration.GetSection("ChilkatOptions:LicenseKey").Value;
    opt.SpoolFolder = builder.Configuration.GetSection("ChilkatOptions:SpoolFolder").Value;
});

builder.Services.AddInfrastructureRabbitMQ(builder.Configuration);
builder.Services.AddRefitClient<IRubricaComuneService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("AddressBookOptions:Url").Value));

builder.Services.AddRefitClient<IDistributedCacheService>(new RefitSettings()
{
    AuthorizationHeaderValueGetter = (Func<HttpRequestMessage, CancellationToken, Task<string>>)((a, b) =>
    Task.FromResult(Convert.ToBase64String(Encoding.UTF8.GetBytes($"{builder.Configuration.GetSection("DistributedCacheServiceOptions:BasicAuthUserName").Value}:{builder.Configuration.GetSection("DistributedCacheServiceOptions:BasicAuthPassword").Value}"))))
}).ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(builder.Configuration.GetSection("DistributedCacheServiceOptions:Url").Value);
});

builder.Services.AddRefitClient<IAacFetcherService>(new RefitSettings()
{
    ContentSerializer = new SystemTextJsonContentSerializer()
}).ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(builder.Configuration.GetSection("AACOptions:Url").Value);
});

// Aggiunge client Refit per servizi Interoperabilit�
builder.Services.AddRefitClient<IInteroperabilityService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("InteroperabilityServiceOptions:Url").Value));

if (builder.Environment.EnvironmentName.Equals("Development"))
{
    // Registra configuration service mock, in ambiente di debug
    builder.Services.RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Configuration.MockConfigurationService>();
}

builder.Services.AddInfrastructureFSUploaderService();

// healthcheck
builder.Services.AddHealthChecks().AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

builder.Services.RemoveAll<IWebMethodLoggerService>().AddScoped<IWebMethodLoggerService, WebMethodLoggerAgidLibroFirmaEFService>();


var app = builder.Build();

app.UseRouting();

if (elasticApmEnabled)
    app.UseAllElasticApm(builder.Configuration);

var rabbitMQOptions = app.Services.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
if (rabbitMQOptions.EnableSubscribers ?? true)
{
    var rabbitMQService = app.Services.GetRequiredService<RabbitMQService>();
    rabbitMQService?.Subscribe<InviaEmailTrasmissioneCommand>();
    rabbitMQService?.Subscribe<LibroFirmaCommand>();
    rabbitMQService?.Subscribe<SpedizioneInteropPiTreCommand>();
    rabbitMQService?.Subscribe<DocumentMetadataManagementCommand>();
}

// rabbitMQService?.Subscribe<>(); Sottoscrittori

app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(a =>
{
    a.DefaultModelsExpandDepth(-1);
});

app.UseHttpsRedirection();

// si basa su logica differente
app.UseMiddleware<ClaimsPrincipalActivatorMiddleware>();

const string healthCheckPattern = "/healthz";
const string readinessCheckPattern = "/readiness";
const string startupCheckPattern = "/startup";

app.MapHealthChecks(healthCheckPattern);
app.MapHealthChecks(readinessCheckPattern);
app.MapHealthChecks(startupCheckPattern);

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

