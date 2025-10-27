// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Pi3.App.Conservazione.Batch.Infrastructure.AggregateModels.DocumentBlobAggregate;
using Pi3.App.Conservazione.Batch.Infrastructure.Services.Conservazione;
using Pi3.App.Conservazione.Batch.Infrastructure.Services.DigitalPreservation;
using Pi3.App.Conservazione.Batch.Infrastructure.Services.OracleDbContextFactory;
using Pi3.App.Conservazione.Batch.Infrastructure.Services.Principal;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Services.Conservazione;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.BouncyCastle.Services.File.CAdES;
using Pi3.Infrastructure.IText.ReportGenerator;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.ParER;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Conservazione.Batch
{
    public static class Runner
    {
        public async static Task Run(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");

            //if (args.Count() < 2) throw new ArgumentNullException();

            var builder = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    var suffix = !string.IsNullOrWhiteSpace(environment) ? $".{environment}" : string.Empty;

                    var configurationRoot = configuration
                        .AddJsonFile($"AppSettings{suffix}.json", false, true)
                        .Build();

                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddDistributedMemoryCache();

                    //ClaimsPrincipalService
                    services.AddScoped<IClaimsPrincipalService, ClaimsPrincipalService>();

                    // Registra Infrastructure EF
                    services.AddInfrastructureLegacyEFServices();
                    services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();
                    services.AddInfrastructureLegacyEFAggregazioneDocumentaleAggregate();

                    // Registra Infrastructure per accesso al repository dei documenti su file system
                    services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();

                    // Registra Infrastructure per invio trasmissioni
                    services.AddInfrastructureLegacyEFTrasmissioneAggregate();

                    // Registra Infrastructure per accesso ad Oracle tramite EF
                    services.AddScoped<IInstanceProvider, CommandLineArgsInstanceProvider>();
                    services.Configure<OracleDbContextFactoryServiceOptions>(hostingContext.Configuration.GetSection(key: nameof(OracleDbContextFactoryServiceOptions)));
                    services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
                    services.AddScoped<IPi3DbContext>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
                    services.AddScoped<IPi3DbContextEntities>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
                    services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());

                    services.AddScoped<ICAdESService, BouncyCastleCAdESService>();


                    // Registra Infrastructure per conservazione ParER
                    services.AddInfrastructureParERDigitalPreservation(hostingContext.Configuration.GetSection("ParEROptions:Url").Value, opt =>
                    {
                        opt.UserName = hostingContext.Configuration.GetSection("ParEROptions:UserName").Value;
                        opt.Password = hostingContext.Configuration.GetSection("ParEROptions:Password").Value;
                        opt.Version = hostingContext.Configuration.GetSection("ParEROptions:Version").Value;
                    });

                    // Registra Infrastructure per generazione report
                    services.AddInfrastructureITextReportGenerator();

                    if (environment == "Test")
                    {
                        services.RemoveAll<IDocumentBlobRepository>().AddScoped<IDocumentBlobRepository, DocumentBlobMockRepository>();
                        services.RemoveAll<IInstanceProvider>().AddScoped<IInstanceProvider, MockCommandLineArgsInstanceProvider>();
                        services.RemoveAll<ISIPService>().AddScoped<ISIPService, MockSIPService>();
                    }

                    services.AddScoped<IConservazioneService, ConservazioneService>();
                });

            builder.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

            if (environment == "Test")
            {
                var host = builder.Build();
                var service = host.Services.GetRequiredService<IConservazioneService>();

                var configuration = host.Services.GetRequiredService<IConfiguration>();

                var operationType = args[1];

                await service.DoWork(operationType);
            
                await host.StopAsync();
            }
            else
            {
                var host = builder.Build();
                var service = host.Services.GetRequiredService<IConservazioneService>();

                var operationType = args[1];

                await service.DoWork(operationType);
                
                await host.StopAsync();
            }
        }
    }
}
