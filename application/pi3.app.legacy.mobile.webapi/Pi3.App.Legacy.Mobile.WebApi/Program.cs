// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Asp.Versioning;
using DocsPaVO.Settings;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Pi3.App.Legacy.Mobile.ApiHandler.Models;
using Pi3.App.Legacy.Mobile.Data;
using Pi3.App.Legacy.Mobile.Data.Context;
using Pi3.App.Legacy.Mobile.Data.Services;
using Pi3.App.Legacy.Mobile.Models;
using Pi3.App.Legacy.Mobile.WebApi;
using Pi3.App.Legacy.Mobile.WebApi.Helpers;
using Pi3.App.Legacy.Mobile.WebApi.Middleware;
using Pi3.Core;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.BouncyCastle;
using Pi3.Infrastructure.Chilkat;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Services.AAC;
using Pi3.Infrastructure.Tibco;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
//using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Reflection;
using HANDLERS = Pi3.App.Legacy.Mobile.ApiHandler.Handlers;
using Pi3.App.Legacy.Mobile.WebApi.Services.RabbitMQ;
using Pi3.App.Legacy.Mobile.WebApi.Requests;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.IText.Decorator;
using Pi3.App.Legacy.Mobile.WebApi.Services.RubricaComune;
using Refit;
using Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService;
using Pi3.App.Legacy.Mobile.WebApi.Services.AAC;
using Pi3.App.Legacy.Mobile.WebApi.Helpers.Spedizione.InteroperabilitaSemplificata;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Infrastructure.IText.ReportGenerator;
using System.Linq;
using System;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var elasticApmEnabled = builder.Configuration.GetValue<bool>("ElasticApmEnabled");

if (elasticApmEnabled)
    builder.Services.AddAllElasticApm();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>("CustomHealthCheck");

//builder.Services.AddAutoMapper(typeof(MapperProfileHandler));
//builder.Services.AddAutoMapper(typeof(MapperProfileData));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.MaxDepth = 64;
        options.JsonSerializerOptions.IncludeFields = true;
    });

var parametri = new AppSettings();
builder.Configuration.GetSection("Parametri").Bind(parametri);
AppSettings.Instance = parametri;
AppSettings.Instance.ConnectionStrings = builder.Configuration.GetConnectionStrings();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(HANDLERS.QueryHandlers.Documents.GetDocumentInfoHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(HANDLERS.QueryHandlers.Documents.GetFileByIdDocumentHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(HANDLERS.QueryHandlers.Users.GetUserHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(HANDLERS.QueryHandlers.Deleghe.GetDelegheHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(HANDLERS.QueryHandlers.Instances.GetInstancesHandler).Assembly);
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
})
.AddApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.OperationFilter<SwaggerDefaultValues>();
    //options.OperationFilter<SwaggerUserResourceFilter>();
});

// Aggiunge client Refit per servizi Rubrica comune
builder.Services.AddRefitClient<IRubricaComuneService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("AddressBookOptions:Url").Value));

#region SWAGGER
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
var apiVersion = Environment.GetEnvironmentVariable("API_VERSION") ?? "v1";

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc($"{apiVersion}", new OpenApiInfo
    {
        Title = "Pi3.App.Legacy.Mobile.WebApi",
        Version = $"{apiVersion}"
    });
    c.EnableAnnotations();
    c.OperationFilter<SwaggerUserResourceFilter>();

    OpenApiSecurityScheme instanceScheme = new()
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "Instance",
        Scheme = "Instance",
        Description = "Global header for specifying the instance",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Instance"
        }
    };
    OpenApiSecurityRequirement instanceRequirement = [];
    instanceRequirement.Add(instanceScheme, ["Instance"]);
    c.AddSecurityDefinition("Instance", instanceScheme);
    c.AddSecurityRequirement(instanceRequirement);

    OpenApiSecurityScheme authTokenScheme = new()
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Name = "AuthToken",
        Scheme = "AuthToken",
        Description = "Global header for specifying the AuthToken",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "AuthToken"
        }
    };
    OpenApiSecurityRequirement authTokenRequirement = [];
    authTokenRequirement.Add(authTokenScheme, ["AuthToken"]);
    c.AddSecurityDefinition("AuthToken", authTokenScheme);
    c.AddSecurityRequirement(authTokenRequirement);

});
#endregion




builder.Services.AddDistributedMemoryCache();

#region Integrazione Pi3
builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<IClaimsPrincipalService, HttpClaimsPrincipalService>();

builder.Services.AddScoped<HttpClaimsPrincipalService>();
builder.Services.AddScoped<ClaimsPrincipalService>();
builder.Services.AddScoped<IWebMethodLoggerService>();

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
// Registra Infrastructure EF
builder.Services.AddInfrastructureLegacyEFServices();
builder.Services.AddInfrastructureITextFileDecorator();
builder.Services.AddInfrastructureITextReportGenerator();

builder.Services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();
builder.Services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();


builder.Services.AddInfrastructureChilkat(opt =>
{
    opt.LicenseKey = builder.Configuration.GetSection("ChilkatOptions:LicenseKey").Value;
    opt.SpoolFolder = builder.Configuration.GetSection("ChilkatOptions:SpoolFolder").Value;
});
builder.Services.AddInfrastructureBouncyCastle();

builder.Services.AddInfrastructureRabbitMQ(builder.Configuration);

// Aggiunge client Refit per servizi Interoperabilit�
builder.Services.AddRefitClient<IInteroperabilityService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("InteroperabilityServiceOptions:Url").Value));

//Aggiunge client Refit per servizi AAC per scan Google
builder.Services.AddRefitClient<IAacFetcherService>(new RefitSettings()
{
    ContentSerializer = new SystemTextJsonContentSerializer()
}).ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(builder.Configuration.GetSection("AACOptions:Url").Value);
});

// Registra Infrastructure per accesso ad Oracle tramite EF
builder.Services.Configure<OracleDbContextFactoryServiceOptions>(options =>
{
    options.EnableLogging = true;
});
builder.Services.Configure<OracleDbContextFactoryServiceOptions>(builder.Configuration.GetSection(key: nameof(OracleDbContextFactoryServiceOptions)));
builder.Services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
builder.Services.AddScoped<IPi3DbContext>(provider =>
{
    return provider?.GetService<IOracleDbContextFactoryService>()?.CreateDbContext() ?? throw new Exception("Provider Pi3Context non configurato");
});
builder.Services.AddScoped<IPi3DbContextEntities>(provider =>
{
    return provider?.GetService<IOracleDbContextFactoryService>()?.CreateDbContext() ?? throw new Exception("Provider Pi3Context (Entities) non configurato");
});
builder.Services.AddScoped<IPi3DbContextFunctions>(provider => {
    return provider?.GetService<IOracleDbContextFactoryService>()?.CreateDbContext() ?? throw new Exception("Provider Pi3Context (Functions) non configurato");
});

#endregion

#region URL Rewrite
if (builder.Configuration.GetValue<bool>("EnableUrlRewrite"))
{
    builder.Services.AddSingleton<Pi3.App.Legacy.Mobile.Shared.Helpers.PathResolutionService>();
}
#endregion

#region AAC

builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value ?? throw new Exception("Configurazione AAC Assente"));

/*
switch (builder.Environment.EnvironmentName)
{
    case "Development":
    case "Test":
        //builder.Services.AddAuthentication(TestAuthenticationDefaults.AuthenticationScheme)
        //            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
        //                TestAuthenticationDefaults.AuthenticationScheme, options => { });

        // in caso questo
        //builder.Services.AddAuthentication(TestAuthenticationDefaults.AuthenticationScheme)
        //            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
        //                TestAuthenticationDefaults.AuthenticationScheme, options => { });
        //builder.Services.AddAuthorization(options =>
        //{
        //    options.AddPolicy(Policies.PITRE, policy =>
        //      policy.AddAuthenticationSchemes("Test")
        //      .RequireAuthenticatedUser()
        //      .Build()
        //    );
        //});

        builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value ?? throw new Exception("Configurazione AAC Assente"));
        break;
    default:
        builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value ?? throw new Exception("Configurazione AAC Assente"));
        break;
}
*/

#endregion

builder.Services.AddHandlerRepositories();
builder.Services.AddInfrastructureLegacyEFTrasmissioneAggregate();
builder.Services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();
app.UseStaticFiles();

if (elasticApmEnabled)
    app.UseAllElasticApm(builder.Configuration);

var rabbitMQOptions = app.Services.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
if (rabbitMQOptions.EnableSubscribers ?? true)
{
    var rabbitMQService = app.Services.GetRequiredService<RabbitMQService>();
    rabbitMQService?.Subscribe<LibroFirmaRequest>();
    rabbitMQService?.Subscribe<SpedizioneInteropPiTreRequest>();
}

var supportedCultures = new[] { "en-US", "it-IT" };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("it-IT"),
    SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList(),
    SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList()
};

app.UseRequestLocalization(localizationOptions);


app.UseMiddleware<ReadHeaderMiddleware>();

#region URL Rewrite
if (app.Configuration.GetValue<bool>("EnableUrlRewrite"))
{
    app.UseMiddleware<RewriteRouteMiddleware>();
}
#endregion


app.UseSerilogRequestLogging();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<AuthenticationTokenMiddleware>();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI(options =>
    //{
    //    var basePath = Environment.GetEnvironmentVariable("ASPNETCORE_BASEPATH");
    //    if( !String.IsNullOrWhiteSpace(basePath) )
    //    {
    //        app.UsePathBase($"/{basePath}");
    //    }
    //    options.SwaggerEndpoint($"{version}/swagger.json", "Pi3.App.Legacy.Mobile.WebApi");
    //});
    app.UseSwaggerUI(
    options =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}

app.UseRouting();
app.UseAuthorization();

app.UseRouting();
app.MapControllers();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = CustomHealthCheck.WriteHealthCheckResponse
});
app.MapHealthChecks("/readiness", new HealthCheckOptions
{
    ResponseWriter = CustomHealthCheck.WriteHealthCheckResponse
});
app.MapHealthChecks("/startup", new HealthCheckOptions
{
    ResponseWriter = CustomHealthCheck.WriteHealthCheckResponse
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
