// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Doc.Application.Search.DocumentoAmministrativo.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate;
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.Repositories;
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.ValueObjects;
using Pi3.App.Indexer.Infrastructure;
using Pi3.Core.SeedWork;
using System.Text.Json;

namespace Pi3.App.Indexer
{
    internal class IndexerBackgroundService : BackgroundService, IIndexerBackgroundService
    {
        #region Public Members

        public IndexerBackgroundService(
            ILogger<IndexerBackgroundService> logger,
            IServiceProvider services,
            IOptions<IndexerBackgroundServiceOptions> options)
        {
            this._logger = logger;
            this._services = services;
            this._options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._logger.LogDebug($"StopExecution: {this._options.Value.StopExecution}");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!this._options.Value.StopExecution ?? false)
                {
                    await this.DoWork();

                    await Task.Delay(TimeSpan.FromSeconds(this._options.Value.SecondsDelay), stoppingToken);
                }
            }
        }

        public virtual async Task DoWork()
        {
            var logger = this._logger;
            var configuration = this._services.GetService<IConfiguration>();
            var requestRepository = this._services.GetService<IRequestRepository>();

            var mediator = this._services.GetService<IMediator>();

            var instance = this._options.Value.Instance;
            logger.LogInformation($"instance: {instance}");

            var nTopRequests = this._options.Value.NTopRequests;
            logger.LogInformation($"nTopRequests: {nTopRequests}");

            var filterByIdTenant = this._options.Value.IdTenant;
            logger.LogInformation($"filterByIdTenant: {filterByIdTenant}");

            var filterByIdElementType = Enum.Parse<Pi3.App.Indexer.Application.Queries.Request.RequestElementTypesEnum>(this._options.Value.IdElementType);
            logger.LogInformation($"filterByIdElementType: {filterByIdElementType}");

            var filterByElementCreationDateFrom = this._options.Value.ElementCreationDateFrom;
            logger.LogInformation($"filterByElementCreationDateFrom: {filterByElementCreationDateFrom}");

            var filterByElementCreationDateTo = this._options.Value.ElementCreationDateTo;
            logger.LogInformation($"filterByElementCreationDateTo: {filterByElementCreationDateTo}");

            logger.LogInformation($"Estrazione richieste in corso...");

            var queryResults = await mediator.Send(new Application.Queries.Request.RequestQuery(nTopRequests, filterByIdTenant, filterByIdElementType, filterByElementCreationDateFrom, filterByElementCreationDateTo));

            logger.LogInformation($"Estratte {queryResults.RequestIds.Count} richieste.");

            int i = 1;

            foreach (var idAggregate in queryResults.RequestIds)
            {
                logger.LogInformation($"Gestione richiesta {i} di  {queryResults.RequestIds.Count} in corso...");

                Request requestAggregate = null;

                if (true /*await requestRepository.Exists(filterByIdTenant, idAggregate)*/)
                {
                    logger.LogInformation($"Richiesta con Id '{idAggregate}'.");

                    //Pi3.App.Indexer.Application.Commands.RequestStatus requestStatus = null;
                    Pi3.Search.DocumentoAmministrativo.Models.RequestStatus requestStatus = null;

                    try
                    {
                        //requestAggregate = await requestRepository.Get(filterByIdTenant, idAggregate);

                        requestStatus = await IndexDocument("361", idAggregate, RequestOperationTypesEnum.AddOrUpdate);

                        //switch (requestAggregate.ElementType)
                        //{
                        //    case RequestElementTypesEnum.Document:
                        //        {
                        //            requestStatus = await IndexDocument(requestAggregate.IdTenant, requestAggregate.IdElement, requestAggregate.OperationType);
                        //        }
                        //        break;
                        //    default:
                        //        throw new NotSupportedPi3Exception();
                        //}

                        logger.LogInformation($"Richiesta con Id '{idAggregate}' completata: {JsonSerializer.Serialize<Pi3.Search.DocumentoAmministrativo.Models.RequestStatus>(requestStatus)}");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, ex.Message);
                    }
                    finally
                    {
                        if (requestAggregate is not null && requestStatus is not null)
                        {
                            requestAggregate.UpdateRequestStatus(requestStatus.Handled, requestStatus.ProcessingDate, requestStatus.ProcessingElapsed, requestStatus.Error, requestStatus.Warnings);

                            await requestRepository.Update(requestAggregate);
                        }
                    }
                }
                else
                {
                    logger.LogInformation($"Richiesta con Id '{idAggregate}' non trovata.");
                }

                i++;
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IndexerBackgroundService> _logger;
        protected readonly IServiceProvider _services;
        protected readonly IOptions<IndexerBackgroundServiceOptions> _options;

        protected virtual async Task<Pi3.Search.DocumentoAmministrativo.Models.RequestStatus> IndexDocument(string idTenant, string idDocument, RequestOperationTypesEnum operationType)
        {
            Pi3.Search.DocumentoAmministrativo.Models.RequestStatus requestStatus = null;
            var mediator = this._services.GetService<IMediator>();

            switch (operationType)
            {
                case RequestOperationTypesEnum.AddOrUpdate:
                    requestStatus = (await mediator.Send(new AddOrUpdateDocumentoAmministrativoCommand(idTenant, idDocument))).RequestStatus;

                    break;

                case RequestOperationTypesEnum.Delete:
                    requestStatus = (await mediator.Send(new DeleteDocumentoAmministrativoCommand(idTenant, idDocument))).RequestStatus;

                    break;
            }

            return requestStatus;
        }

        #endregion
    }
}
