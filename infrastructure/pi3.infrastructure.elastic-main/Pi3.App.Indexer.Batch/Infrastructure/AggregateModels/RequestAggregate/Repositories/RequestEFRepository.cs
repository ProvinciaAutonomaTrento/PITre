// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.Repositories;
using Pi3.App.Indexer.Infrastructure.AggregateModels.RequestAggregate.Exceptions;
using Pi3.App.Indexer.Core.AggregateModels.RequestAggregate.ValueObjects;

namespace Pi3.App.Indexer.Infrastructure.AggregateModels.RequestAggregate.Repositories
{
    public class RequestEFRepository : ElementRepository<Request>, IRequestRepository
    {
        #region Public Members

        public RequestEFRepository(ILogger<RequestEFRepository> logger, IClaimsPrincipalService claimsPrincipal, IEventPublisher eventPublisher, IPi3DbContext dbContext)
            : base(logger, claimsPrincipal, eventPublisher)
        {
            _dbContext = dbContext;

            InitializeMapper();
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            return await _dbContext.IndexerRequests.AnyAsync(d => d.ID == id.AsLong() && d.ID_AMM == idTenant.AsLong());
        }

        protected override async Task<Request> HandleGet(Request newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var requestEntity = await _dbContext.IndexerRequests.FirstOrDefaultAsync(d => d.ID == id.AsLong() && d.ID_AMM == idTenant.AsLong());

            if (requestEntity == null)
                throw new RequestNotFoundPi3Exception(id);

            return _mapper.Map<Request>(requestEntity);
        }

        #endregion

        #region Private Members

        protected override async Task HandleUpdate(Request aggregate)
        {
            var requestEntity = await _dbContext.IndexerRequests.FirstOrDefaultAsync(d => d.ID == aggregate.Id.AsLong() && d.ID_AMM == aggregate.IdTenant.AsLong());

            if (requestEntity == null)
                throw new RequestNotFoundPi3Exception(aggregate.Id);

            if (aggregate.Handled ?? false)
            {
                // Se la richiesta è stata gestita correttamente, sposta i dati della richiesta sulla tabella dello storico

                var existing = await _dbContext.IndexerHandledRequests.FindAsync(requestEntity.ID);
                if (existing != null)
                {
                    existing.ID_OPERATION_TYPE = requestEntity.ID_OPERATION_TYPE;
                    existing.LAST_PROCESSING_DATE = aggregate.LastProcessingDate;
                    existing.LAST_PROCESSING_ELAPSED = aggregate.LastProcessingElapsed;
                    existing.WARNINGS = aggregate.Warnings;
                }
                else
                {
                    await _dbContext.IndexerHandledRequests.AddAsync(new IndexerHandledRequestEntity()
                    {
                        ID = requestEntity.ID,
                        ID_AMM = requestEntity.ID_AMM,
                        REQUEST_DATE = requestEntity.REQUEST_DATE,
                        ID_ELEMENT = requestEntity.ID_ELEMENT,
                        ID_ELEMENT_TYPE = requestEntity.ID_ELEMENT_TYPE,
                        ELEMENT_CREATION_DATE = requestEntity.ELEMENT_CREATION_DATE,
                        ID_OPERATION_TYPE = requestEntity.ID_OPERATION_TYPE,
                        LAST_PROCESSING_DATE = aggregate.LastProcessingDate,
                        LAST_PROCESSING_ELAPSED = aggregate.LastProcessingElapsed,
                        WARNINGS = aggregate.Warnings
                    });
                }

                _dbContext.IndexerRequests.Remove(requestEntity);
            }
            else
            {
                // Se la richiesta non è stata gestita correttamente, aggiorna i dati della richiesta

                requestEntity.LAST_PROCESSING_DATE = aggregate.LastProcessingDate;
                requestEntity.LAST_PROCESSING_ELAPSED = aggregate.LastProcessingElapsed;
                requestEntity.REMAINING_ATTEMPTS = aggregate.RemainingAttempts ?? 0;
                requestEntity.LAST_ERROR = aggregate.LastError;
                requestEntity.WARNINGS = aggregate.Warnings;
            }

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected override Task HandleAdd(Request element)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleDelete(Request element)
        {
            throw new NotImplementedException();
        }

        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IndexerRequestEntity, Request>()
                    .ConstructUsing(entity => new Request(
                                entity.ID.ToString(),
                                entity.ID_AMM.ToString(),
                                entity.REQUEST_DATE,
                                entity.ID_ELEMENT.ToString(),
                                (RequestElementTypesEnum)entity.ID_ELEMENT_TYPE,
                                entity.ELEMENT_CREATION_DATE,
                                (RequestOperationTypesEnum)entity.ID_OPERATION_TYPE,
                                entity.REMAINING_ATTEMPTS,
                                entity.LAST_PROCESSING_DATE,
                                entity.LAST_PROCESSING_ELAPSED,
                                entity.LAST_ERROR,
                                entity.WARNINGS))
                    .AfterMap(async (src, dest) =>
                    {
                        dest.MarkChangesAsCommitted();
                    });

                cfg.CreateMap<Request, IndexerRequestEntity>()
                    .ForMember(dest => dest.ID, src => src.MapFrom(opt => opt.Id))
                    .ForMember(dest => dest.ID_AMM, src => src.MapFrom(opt => opt.IdTenant))
                    .ForMember(dest => dest.ID_ELEMENT, src => src.MapFrom(opt => opt.IdElement))
                    .ForMember(dest => dest.ID_ELEMENT_TYPE, src => src.MapFrom(opt => opt.ElementType))
                    .ForMember(dest => dest.ID_OPERATION_TYPE, src => src.MapFrom(opt => opt.OperationType))
                    .ForMember(dest => dest.REQUEST_DATE, src => src.MapFrom(opt => opt.RequestDate))
                    .ForMember(dest => dest.LAST_PROCESSING_DATE, src => src.MapFrom(opt => opt.LastProcessingDate))
                    .ForMember(dest => dest.LAST_PROCESSING_ELAPSED, src => src.MapFrom(opt => opt.LastProcessingElapsed))
                    .ForMember(dest => dest.LAST_ERROR, src => src.MapFrom(opt => opt.LastError))
                    .ForMember(dest => dest.REMAINING_ATTEMPTS, src => src.MapFrom(opt => opt.RemainingAttempts))
                    .ForMember(dest => dest.WARNINGS, src => src.MapFrom(opt => opt.Warnings));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}