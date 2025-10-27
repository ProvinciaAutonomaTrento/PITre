// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Serilog;
using System.Text.Json.Serialization;
using Pi3.Core.Services.File.Converters;
using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pi3.Core;
using Pi3.App.Search.WebApi.Middleware;
using Pi3.App.Search.WebApi.Infrastructure.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.App.Search.WebApi.Infrastructure.Services.OracleDbContextFactory;
using Pi3.Core.Services;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Microsoft.Extensions.Options;
using Pi3.App.Search.WebApi.Application.Queries.Elements.GetElements;
using Pi3.Core.Services.Configuration;
using Pi3.App.Search.WebApi.Infrastructure.Services.Configuration;
using Pi3.Infrastructure.Services.AAC;
using Microsoft.AspNetCore.Authentication;
using Pi3.App.Search.WebApi.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (builder.Environment.IsDevelopment())
{
    builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
}

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

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(Pi3.App.Search.WebApi.Application.Behaviors.LoggerBehavior<,>));
builder.Services.AddScoped<IClaimsPrincipalService, HttpClaimsPrincipalService>();
builder.Services.AddInfrastructureLegacyEFServices();
builder.Services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();

if (builder.Environment.IsDevelopment())
    // Registra configuration service mock, in ambiente di debug
    builder.Services.RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, MockConfigurationService>();

// Registra Infrastructure per accesso ad Oracle tramite EF
builder.Services.AddScoped<IOracleDbContextFactoryServiceOptions, OracleDbContextFactoryServiceOptions>();
builder.Services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
builder.Services.AddScoped<IPi3DbContext>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextEntities>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
builder.Services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());

builder.Services.Configure<LuceneIndexingOptions>(builder.Configuration.GetSection(key: nameof(LuceneIndexingOptions)));

if (builder.Environment.EnvironmentName.Equals("Test") )
{
    builder.Services.AddAuthentication(TestAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationDefaults.AuthenticationScheme, options => { });
}
else
{
    // Aggiunge infrastruttura di autenticazione AAC
    builder.Services.AddInfrastructureAAC(builder.Configuration.GetSection("InfrastructureAACOptions:UrlJWK").Value);
}

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(a =>
{
    a.DefaultModelsExpandDepth(-1);
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ClaimsPrincipalActivatorMiddleware>();

app.MapControllers();

app.Run();