// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Doc.Application.Search.DocumentoAmministrativo.Helpers;
using Doc.Search.DocumentoAmministrativo;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using pi3.Core.Contracts.DocumentoAmministrativo;
using Pi3.App.Indexer.Application;
using Pi3.App.Indexer.Application.Queries.Request;
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.Repositories;
using Pi3.App.Indexer.Infrastructure;
using Pi3.App.Indexer.Infrastructure.AggregateModels.DocumentBlobAggregate;
using Pi3.App.Indexer.Infrastructure.AggregateModels.RequestAggregate.Repositories;
using Pi3.App.Indexer.Infrastructure.Services.Configuration;
using Pi3.App.Indexer.Infrastructure.Services.Indexer;
using Pi3.App.Indexer.Infrastructure.Services.OracleDbContextFactory;
using Pi3.App.Indexer.Infrastructure.Services.Principal;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Aspose.TextExtractor;
using Pi3.Infrastructure.BouncyCastle;
using Pi3.Infrastructure.Elastic;
using Pi3.Infrastructure.Elastic.Helpers;
using Pi3.Infrastructure.Legacy.DocumentFSRepository;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.ZipFileTextExtractor;
using Serilog;
using System.Reflection;

namespace Pi3.App.Indexer
{
    public static class Runner
    {
        public async static Task Run(string[] args)
        {
            var builder = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, configuration) =>
        {
            var configurationRoot = configuration
                .AddJsonFile($"AppSettings{(args.Length > 0 ? "." + args[0] ?? "" : "").Trim()}.json", false, true)
                .Build();
        })
        .ConfigureServices((hostingContext, services) =>
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            services.AddDistributedMemoryCache();

            services.Configure<ClaimsPrincipalServiceOptions>(hostingContext.Configuration.GetSection(key: nameof(ClaimsPrincipalServiceOptions)));
            services.AddScoped<IClaimsPrincipalService, ClaimsPrincipalService>();
            services.AddInfrastructureLegacyEFServices();

            // Registra Infrastructure EF
            services.AddInfrastructureLegacyEFDocumentoAmministrativoAggregate();

            // Registra Infrastructure per accesso al repository dei documenti su file system
            services.AddInfrastructureLegacyDocumentBlobFileSystemRepository();

            // Registra Infrastructure per accesso ad Oracle tramite EF
            services.Configure<OracleDbContextFactoryServiceOptions>(hostingContext.Configuration.GetSection(key: nameof(OracleDbContextFactoryServiceOptions)));
            services.AddScoped<IOracleDbContextFactoryService, OracleDbContextFactoryService>();
            services.AddScoped<IPi3DbContext>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
            services.AddScoped<IPi3DbContextEntities>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());
            services.AddScoped<IPi3DbContextFunctions>(provider => provider.GetService<IOracleDbContextFactoryService>().CreateDbContext());

            if (args.Length > 0 && (args[0] == "Development" || args[0] == "Test"))
            {
                // Registra configuration service mock, in ambiente di debug
                services.RemoveAll<IConfigurationService>().AddScoped<IConfigurationService, MockConfigurationService>();
                services.RemoveAll<IDocumentBlobRepository>().AddScoped<IDocumentBlobRepository, DocumentBlobMockRepository>();
                services.Configure<DocumentBlobMockRepositoryOptions>(hostingContext.Configuration.GetSection(key: nameof(DocumentBlobMockRepositoryOptions)));
            }

            //services.AddAsposeOcrTextExtractor(options => options.SpoolFolder = hostingContext.Configuration.GetSection("Options:OcrTextExtractorSpoolFolder").Value);
            //services.AddAsposeWordTextExtractor();
            //services.AddAsposeSlidesTextExtractor();
            //services.AddAsposeCellTextExtractor();
            services.AddAsposePdfTextExtractor();
            services.AddInfrastructureBouncyCastle();
            services.AddZipFileTextExtractor(options => options.SpoolFolder = hostingContext.Configuration.GetSection("Options:ZipFileTextExtractorSpoolFolder").Value);

            //services.AddMsgReaderFileTextExtractor();
            //services.AddInfrastructureElasticSearch(hostingContext.Configuration.GetSection("IndexingOptions:ElasticSearchUri").Value);
            
            services.AddInfrastructureDocumentoAmministrativo();
            services.AddInfrastructureElasticSearch();
            //services.Configure<IndexingOptions>(hostingContext.Configuration.GetSection(key: nameof(IndexingOptions)));

            services.AddScoped<IRequestRepository, RequestEFRepository>();

            services.Configure<IndexingOptions>(hostingContext.Configuration.GetSection(key: nameof(IndexingOptions)));

            services.Configure<IndexerBackgroundServiceOptions>(hostingContext.Configuration.GetSection(key: nameof(IndexerBackgroundServiceOptions)));

            if (args.Length > 0 && args[0] == "Test") 
            {
                services.AddScoped<IIndexingService, MockIndexerService>();
                services.AddScoped<IIndexerBackgroundService, IndexerBackgroundService>(); 
            }
            else
            {
                services.AddScoped<IIndexingService, ElasticIndexerService>();
                services.AddHostedService<IndexerBackgroundService>();
            }

        });

            if (args.Length > 0 && args[0] == "Test")
            {
                var host = builder.Build();
                var service = host.Services.GetService<IIndexerBackgroundService>();

                var configuration = host.Services.GetService<IConfiguration>();
                var mediator = host.Services.GetService<IMediator>();

                var nTopRequests = configuration.GetSection("IndexerBackgroundServiceOptions:NTopRequests").Get<int>();
                var filterByIdTenant = configuration.GetSection("IndexerBackgroundServiceOptions:IdTenant").Get<string>();

                //queryResult
                var queryResults = await mediator.Send(new RequestQuery(nTopRequests, filterByIdTenant, RequestElementTypesEnum.DocumentoAmministrativo));

                if (!queryResults.RequestIds.Any()) throw new ApplicationException("Nessun documento da indicizzare in coda, impossibile completare il test unitario");

                if (service is not null) await service.DoWork();
            }
            else
            {
                if (args.Length > 0 && args[0] == "Development")
                {
                    builder.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
                }

                await builder.Build().RunAsync();
            }

        }      
    }
}
