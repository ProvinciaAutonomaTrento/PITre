// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Chilkat;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Pi3.App.Legacy.WebApi.ActionFilters;
using Pi3.App.Legacy.WebApi.Application.Models;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.App.Legacy.WebApi.Application.Services.Interoperability;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.App.Legacy.WebApi.Middleware;
using Pi3.App.Legacy.WebApi.Models;
using Pi3.Core;
using Pi3.Core.AggregateModels;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using Pi3.Infrastructure.Services.AAC;
using Refit;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Pi3.App.Legacy.WebApi.Application.DomainEventHandlers;
using Microsoft.AspNetCore.Authentication;
using Pi3.App.Legacy.WebApi.Handler;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Requests;
using System.Security.Claims;
using Pi3.App.Legacy.WebApi.Resources;
using System.Net.Http;
using Pi3.Infrastructure.DocumentFormat.OpenXml;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Swashbuckle.AspNetCore.Filters;
using Pi3.App.Legacy.WebApi.Services.OracleDbContextFactory;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Infrastructure.BouncyCastle;
using Pi3.App.Legacy.WebApi.Application.Handlers.ConvertVersionToPdf;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Handlers.GeneraMetadatiAGID;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.App.Legacy.WebApi.Application.Services.WebMethodLogger;
using Pi3.App.Legacy.WebApi.Application.Handlers.SpedizioneInteropPiTre;
using Pi3.App.Legacy.WebApi.Application.Services.WebMethodMockLogger;
using Pi3.Infrastructure.Graph;

using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Pi3.App.Legacy.WebApi.Application.Services.Google;
using StackExchange.Redis;
using Pi3.App.Legacy.WebApi.Application.Services.Aac;
using System.Text;
using Microsoft.Extensions.Options;
using Pi3.Infrastructure.IText.Decorator;
using Pi3.Infrastructure.IText.ReportGenerator;
using Pi3.App.Legacy.WebApi.Application.Services.Files.Converters;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.Conservazione;
using Pi3.App.Legacy.WebApi.Application.Services.DigitalPreservation;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.App.Legacy.WebApi.Application.Services.File.FirmaDigitale2;
using Pi3.Core.Services.File.FirmaRemota2;
using Pi3.App.Legacy.WebApi.Application.Services.File.FirmaRemota2;
using Pi3.Core.Services.File.MarcaTemporale;
using Pi3.App.Legacy.WebApi.Application.Services.File.MarcaTemporale;
using Pi3.Core.Services.File.SigilloElettronico;
using Pi3.App.Legacy.WebApi.Application.Services.File.SigilloElettronico;
using Elastic.Apm.NetCoreAll;


var builder = WebApplication.CreateBuilder(args);

var elasticApmEnabled = builder.Configuration.GetValue<bool>("ElasticApmEnabled");

if (elasticApmEnabled)
    builder.Services.AddAllElasticApm();

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var environment = builder.Environment.EnvironmentName;
var isTestEnvironment = environment.Equals("Test");
var isDevelopmentEnvironment = environment.Equals("Development");

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add API Versioning to as service to your project 
builder.Services.AddApiVersioning(config =>
{
    // Advertise the API versions supported for the particular endpoint
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.ReportApiVersions = true;
    config.AssumeDefaultVersionWhenUnspecified = true;
});

// Add services to the container.
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
            typeof(Pi3.App.Legacy.WebApi.Application.Behaviors.LoggerBehavior<,>).Assembly);
});

builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.TrasmissioneAggregate.Events.TrasmissioneUtenteRifiutataEvent>, TrasmissioneAccettataRifiutataEventHandler>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.TrasmissioneAggregate.Events.TrasmissioneUtenteAccettataEvent>, TrasmissioneAccettataRifiutataEventHandler>();
builder.Services.AddScoped<IEventHandler<Pi3.Core.AggregateModels.TrasmissioneAggregate.Events.TrasmissioneInviataEvent>, TrasmissioneInviataEventHandler>();

builder.Services.AddScoped<IClaimsPrincipalService, ClaimsPrincipalService>();

if (isTestEnvironment)
{
    builder.Services.AddInfrastructureMockRabbitMQ();

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

    builder.Services.AddScoped<IRubricaComuneService, RubricaComuneMockService>();
}
else
{
    builder.Services.AddInfrastructureRabbitMQ(builder.Configuration);

    // Aggiunge infrastruttura di autenticazione AAC
    builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value);

    // Aggiunge client Refit per servizi Rubrica comune
    builder.Services.AddRefitClient<IRubricaComuneService>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("AddressBookOptions:Url").Value));

    // Aggiunge client Refit per servizi Interoperabilit�
    builder.Services.AddRefitClient<IInteroperabilityService>()
        .ConfigureHttpClient(c =>
        {
            c.BaseAddress = new Uri(builder.Configuration.GetSection("InteroperabilityServiceOptions:Url").Value);
            c.Timeout = TimeSpan.FromMinutes(20);
        });

    //Aggiunge client Refit per servizi AAC per scan Google
    builder.Services.AddRefitClient<IAacFetcherService>(new RefitSettings()
    {
        ContentSerializer = new SystemTextJsonContentSerializer()
    }).ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration.GetSection("AACOptions:Url").Value);
    });

    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddInfrastructureLegacyEFServices();
builder.Services.AddInfrastructureLegacyEFOracleDbContextFactory();
builder.Services.AddInfrastructureLegacyEFAggregazioneDocumentaleAggregate();
builder.Services.AddInfrastructureLegacyEFDelegaAggregate();
builder.Services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();
builder.Services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();
builder.Services.AddInfrastructureLegacyEFKeywordAggregate();
builder.Services.AddInfrastructureLegacyEFListaDistribuzioneAggregate();
builder.Services.AddInfrastructureLegacyEFMezzoSpedizioneAggregate();
builder.Services.AddInfrastructureLegacyEFModelloTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFNotaAggregate();
builder.Services.AddInfrastructureLegacyEFNotaRFAggregate();
builder.Services.AddInfrastructureLegacyEFOggettoAggregate();
builder.Services.AddInfrastructureLegacyEFPersonaCorrispondenteAggregate();
builder.Services.AddInfrastructureLegacyEFRagioneTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFRuoloCorrispondenteAggregate();
builder.Services.AddInfrastructureLegacyEFTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFUOCorrispondenteAggregate();
builder.Services.AddInfrastructureITextFileDecorator();
builder.Services.AddSessionRepositoryService();
builder.Services.AddInfrastructureGraph();
builder.Services.AddInfrastructureChilkat(opt =>
{
    opt.LicenseKey = builder.Configuration.GetSection("ChilkatOptions:LicenseKey").Value;
    opt.SpoolFolder = builder.Configuration.GetSection("ChilkatOptions:SpoolFolder").Value;
});

builder.Services.AddInfrastructureITextReportGenerator();
builder.Services.AddInfrastructureOpenXmlSpreadsheetService();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ClaimsPrincipalActivatorActionFilter));
});

builder.Services.AddScoped<IFileConverterService, MockFileConverterService>();
builder.Services.AddScoped<IFirmaDigitale2Service, MockFirmaDigitale2Service>();
builder.Services.AddScoped<IFirmaRemota2Service, MockFirmaRemota2Service>();
builder.Services.AddScoped<IMarcaTemporaleService, MockMarcaTemporaleService>();
builder.Services.AddScoped<ISigilloElettronicoService, MockSigilloElettronicoService>();

builder.Services.AddInfrastructureBouncyCastle();

builder.Services.AddScoped<ISIPService, MockSIPService>();

if (isDevelopmentEnvironment)
{
    // Registra configuration service mock, in ambiente di debug
    builder.Services.RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, MockConfigurationService>();
}

builder.Services.RemoveAll<IWebMethodLoggerService>().AddScoped<IWebMethodLoggerService, WebMethodLoggerEFAgidService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
});

builder.Services.AddInfrastructureGoogle();

builder.Services.AddHealthChecks().AddCheck("BaseHealthCheck", () => HealthCheckResult.Healthy());

var app = builder.Build();

app.UseRouting();

if (elasticApmEnabled)
    app.UseAllElasticApm(builder.Configuration);

if (!isTestEnvironment)
{
    var rabbitMQOptions = app.Services.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
    if (rabbitMQOptions.EnableSubscribers ?? true)
    {
        var rabbitMQService = app.Services.GetRequiredService<IRabbitMQService>();
        rabbitMQService?.Subscribe<InviaEmailTrasmissioneRequest>();
        rabbitMQService?.Subscribe<ConvertVersionToPdfRequest>();
        rabbitMQService?.Subscribe<LibroFirmaRequest>();
        rabbitMQService?.Subscribe<GeneraMetadatiAGIDRequest>();
        rabbitMQService?.Subscribe<SpedizioneInteropPiTreRequest>();
    }
}

app.UseSerilogRequestLogging();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

if (!isTestEnvironment)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

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

ThreadPool.SetMinThreads(100, 100);

app.Run();