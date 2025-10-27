// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Pi3.App.StampaRegistri.Batch.Infrastructure.AggregateModels.DocumentBlobAggregate;
using Pi3.App.StampaRegistri.Batch.Infrastructure.Services.Configuration;
using Pi3.App.StampaRegistri.Batch.Infrastructure.Services.DigitalPreservation;
using Pi3.App.StampaRegistri.Batch.Infrastructure.Services.Email.Sender;
using Pi3.App.StampaRegistri.Batch.Infrastructure.Services.OracleDbContextFactory;
using Pi3.App.StampaRegistri.Batch.Infrastructure.Services.StampaRegistri;
using Pi3.App.StampaRegistri.Batch.Services.Principal;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Conservazione;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.BouncyCastle;
using Pi3.Infrastructure.BouncyCastle.Services.File.CAdES;
using Pi3.Infrastructure.Chilkat;
using Pi3.Infrastructure.IText.ReportGenerator;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.ParER;
using Pi3.Infrastructure.ParER.Services.DigitalPreservation.ValueObjects;
using Serilog;

namespace Pi3.App.StampaRegistri.Batch
{
    public static class Runner
    {
        public async static Task Run(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");

            var builder = Host.CreateDefaultBuilder()
                    .ConfigureAppConfiguration((hostingContext, configuration) =>
                    {
                        var suffix = (!string.IsNullOrWhiteSpace(environment) ?
                           $".{environment}" : string.Empty);

                        var configurationRoot = configuration
                            .AddJsonFile($"AppSettings{suffix}.json", false, true)
                            .Build();
                    })
                    .ConfigureServices((hostingContext, services) =>
                    {
                        services.AddMediatR(config =>
                        {
                            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                        });
                        
                        services.AddDistributedMemoryCache();
                        
                        services.AddScoped<IClaimsPrincipalService, ClaimsPrincipalService>();

                        // Registra Infrastructure EF
                        services.AddInfrastructureLegacyEFServices();
                        services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();
                        services.AddInfrastructureLegacyEFAggregazioneDocumentaleAggregate();

                        // Registra Infrastructure per accesso al repository dei documenti su file system
                        services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();

                        // Registra Infrastructure per accesso ad Oracle tramite EF
                        services.AddScoped<IInstanceProvider, CommandLineArgsInstanceProvider>();
                        services.Configure<OracleDbContextFactoryServiceOptions>(hostingContext.Configuration.GetSection(key: nameof(OracleDbContextFactoryServiceOptions)));
                        services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
                        services.AddScoped<IPi3DbContext>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
                        services.AddScoped<IPi3DbContextEntities>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
                        services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());

                        services.AddScoped<ICAdESService, BouncyCastleCAdESService>();

                        // Registra Infrastructure per generazione report
                        services.AddInfrastructureITextReportGenerator();

                        services.AddInfrastructureChilkat(opt =>
                        {
                            opt.LicenseKey = hostingContext.Configuration.GetSection("ChilkatOptions:LicenseKey").Value;
                        });

                        // Registra Infrastructure per conservazione ParER
                        services.AddInfrastructureParERDigitalPreservation(hostingContext.Configuration.GetSection("ParEROptions:Url").Value, opt =>
                        {
                            opt.UserName = hostingContext.Configuration.GetSection("ParEROptions:UserName").Value;
                            opt.Password = hostingContext.Configuration.GetSection("ParEROptions:Password").Value;
                            opt.Version = hostingContext.Configuration.GetSection("ParEROptions:Version").Value;
                            opt.Ambiente = hostingContext.Configuration.GetSection("ParEROptions:Ambiente").Value;
                        });

                        if (environment == "Development")
                        {
                            services.RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, MockConfigurationService>();
                        }
                        else if (environment == "Test")
                        {
                            services.RemoveAll<IDocumentBlobRepository>().AddScoped<IDocumentBlobRepository, DocumentBlobMockRepository>();
                            services.RemoveAll<IInstanceProvider>().AddScoped<IInstanceProvider, MockCommandLineArgsInstanceProvider>();
                            services.RemoveAll<ISIPService>().AddScoped<ISIPService, MockSIPService>();
                            services.RemoveAll<IEmailSenderService>().AddScoped<IEmailSenderService, MockEmailSenderService>();
                        }

                        services.AddScoped<IStampaRegistriService, StampaRegistriService>();
                    });


            builder.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

            if (environment == "Test")
            {
                var host = builder.Build();
                var service = host.Services.GetRequiredService<IStampaRegistriService>();

                var configuration = host.Services.GetService<IConfiguration>();
                var mediator = host.Services.GetService<IMediator>();

                await service.DoWork();
            }
            else
            {
                var host = builder.Build();
                var service = host.Services.GetRequiredService<IStampaRegistriService>();

                await service.DoWork();

                Environment.Exit(0);
            }
        }
    }
}
