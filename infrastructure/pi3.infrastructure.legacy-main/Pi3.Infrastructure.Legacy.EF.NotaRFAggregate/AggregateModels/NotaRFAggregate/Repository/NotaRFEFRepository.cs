// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.AggregateModels.NotaRFAggregate;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Resources;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Exceptions;
using Pi3.Core.AggregateModels.NotaRFAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.NotaRFAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Repository
{
    public class NotaRFEFRepository : ElementRepository<NotaRF>, INotaRFRepository
    {
        #region Public Members

        public NotaRFEFRepository(ILogger<NotaRFEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;

            InitializeMapper();
        }

        #endregion

        #region Private Members

        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ElencoNoteEntity, NotaRF>()
                    .ConstructUsing(src => new NotaRF(
                    src.SYSTEM_ID.ToString(),
                    _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true),
                    new DateTime(),
                    new TextValue(src.VAR_DESC_NOTA),
                    new TextValue(string.Format(Descriptions.NoteNameFormat)),
                    src.ID_REG_RF.HasValue ? src.ID_REG_RF.ToString() : null,
                    src.COD_REG_RF,
                    new TextValue(string.Format(Descriptions.NoteNameFormat))))
                    .AfterMap((src, dest) =>
                    {
                        dest.MarkChangesAsCommitted();
                    });

                cfg.CreateMap<NotaRF, ElencoNoteEntity>()
                   .ForMember(src => src.SYSTEM_ID, opt => opt.MapFrom(dest => dest.Id))
                   .ForMember(src => src.ID_REG_RF, opt => opt.MapFrom(dest => dest.RF.Id))
                   .ForMember(src => src.COD_REG_RF, opt => opt.MapFrom(dest => dest.RF.Codice))
                   .ForMember(src => src.VAR_DESC_NOTA, opt => opt.MapFrom(dest => dest.Name));
            });

            _mapper = configuration.CreateMapper();
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();

            return await _dbContext
                       .ElencoNoteEntities.AsNoTracking()
                    .AnyAsync(p => p.SYSTEM_ID == idAsLong);
        }

        protected override async Task<NotaRF> HandleGet(NotaRF newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {   
            var notaRFEntity = await _dbContext.ElencoNoteEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == id.AsLong());

            if (notaRFEntity != null && notaRFEntity.ID_REG_RF > 0)
                return _mapper.Map<NotaRF>(notaRFEntity);
            else
                throw new NotaRFNotFoundPi3Exception(id);
        }

        protected override async Task HandleAdd(NotaRF aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected override async Task HandleUpdate(NotaRF aggregate)
        {
            await HandleChanges(aggregate);
        }
        
        protected override async Task HandleDelete(NotaRF aggregate)
        {
            aggregate.IdTenant.AsLong();
            var idAsNumber = aggregate.Id.AsLong();
            var notaRFEntity = await _dbContext.
                ElencoNoteEntities.
                FirstOrDefaultAsync(r =>
                    r.SYSTEM_ID == idAsNumber);

            ((DbContext)_dbContext).Entry<ElencoNoteEntity>(notaRFEntity).State = EntityState.Deleted;
            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected virtual async Task HandleChanges(NotaRF aggregate)
        {
            var uncommitted = new List<dynamic>(aggregate.GetUncommittedChanges());

            foreach (var @event in uncommitted)
            {
                var handleMethod = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(m => m.Name == "Handle"
                        && m.GetParameters().Any(p => p.ParameterType == @event.GetType()));

                if (handleMethod != null)
                    await this.Handle(@event, aggregate);
            }

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected virtual async Task Handle(RFChangedEvent @event, NotaRF aggregate)
        {
            var idTenantAsNumber = aggregate.IdTenant.AsLong();
            var idAsNumber = aggregate.Id.AsLong();
            var idNewRF = @event.NewIdRF.AsLong();
            var notaRFEntity = await _dbContext.
                ElencoNoteEntities.
                FirstOrDefaultAsync(r =>
                    r.SYSTEM_ID == idAsNumber);

            if (notaRFEntity == null)
                throw new NotaRFNotFoundPi3Exception(@event.Id);

            if (string.IsNullOrEmpty(aggregate.RF.Id))
                throw new RFNotFoundPi3Exception(null);
            else
            {

                var RFEntity = await _dbContext.RegistroEntities.FindAsync(aggregate.RF.Id.AsLong());

                if (RFEntity == null)
                {
                    throw new RFNotFoundPi3Exception(aggregate.RF.Id);
                }
            }

            bool isUnica = await IsUnicaNota(aggregate);
            if (!isUnica)
            {
                throw new NotaRFAlreadyExistsPi3Exception();
            }

            notaRFEntity.ID_REG_RF = idNewRF > 0 ? idNewRF : null;
            notaRFEntity.COD_REG_RF = @event.NewCodiceRF != null ? @event.NewCodiceRF.ToString() : null;
        }

        protected virtual async Task Handle(NotaRFCreatedEvent @event, NotaRF aggregate)
        {
            if (string.IsNullOrEmpty(aggregate.RF.Id))
                throw new RFNotFoundPi3Exception(null);
            else
            {

                var RFEntity = await _dbContext.RegistroEntities.FindAsync(aggregate.RF.Id.AsLong());

                if (RFEntity == null)
                {
                    throw new RFNotFoundPi3Exception(aggregate.RF.Id);
                }
            }

            bool isUnica = await IsUnicaNota(aggregate);
            if (!isUnica)
            {
                throw new NotaRFAlreadyExistsPi3Exception();
            }

            var idRF = @event.IdRF.AsLong();
            var notaRFEntity = new ElencoNoteEntity();

            await _dbContext.ElencoNoteEntities.AddAsync(notaRFEntity);

            this.LoadAggregateFromHistory(aggregate,
                new ElementIdAssignedEvent()
                {
                    Id = notaRFEntity.SYSTEM_ID.ToString()
                });

            notaRFEntity.VAR_DESC_NOTA = @event.Name.ToString();
            notaRFEntity.ID_REG_RF = idRF > 0 ? idRF : null;
            notaRFEntity.COD_REG_RF = @event.CodiceRF != null ? @event.CodiceRF.ToString() : null;
        }

        protected virtual async Task Handle(ElementNameChangedEvent @event, NotaRF aggregate)
        {
            var idTenantAsNumber = aggregate.IdTenant.AsLong();
            var idAsNumber = aggregate.Id.AsLong();
            var notaRFEntity = await _dbContext.
                ElencoNoteEntities.
                FirstOrDefaultAsync(r =>
                    r.SYSTEM_ID == idAsNumber);

            if (notaRFEntity == null)
                throw new NotaRFNotFoundPi3Exception(@event.Id);

            bool isUnica = await IsUnicaNota(aggregate);
            if (!isUnica)
            {
                throw new NotaRFAlreadyExistsPi3Exception();
            }

            notaRFEntity.VAR_DESC_NOTA = @event.NewName.ToString();
        }

        protected async Task<bool> IsUnicaNota(NotaRF aggregate)
        {
            var notaRFEntity = await _dbContext.
                ElencoNoteEntities.
                FirstOrDefaultAsync(r =>
                    r.ID_REG_RF == aggregate.RF.Id.AsLong() && r.COD_REG_RF == aggregate.RF.Codice && r.VAR_DESC_NOTA == aggregate.Name.ToString());

            return notaRFEntity == null;
        }

        #endregion
    }
}
