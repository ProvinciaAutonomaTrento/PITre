// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.OggettoAggregate;
using Pi3.Core.AggregateModels.OggettoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.OggettoAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.OggettoAggregate.Repository
{
    public class OggettoEFRepository : ElementRepository<Oggetto>, IOggettoRepository
    {
        #region Public Members

        public OggettoEFRepository(ILogger<OggettoEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
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
                cfg.CreateMap<OggettarioEntity, Oggetto>()
                    .ConstructUsing(src => new Oggetto(
                        src.SYSTEM_ID.ToString(),
                        _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true),
                        DateTime.Now,
                        new TextValue(src.VAR_COD_OGGETTO), //Name
                        src.ID_REGISTRO.ToString(),
                        null,
                        null,
                        new TextValue(src.VAR_DESC_OGGETTO) //Description
                        ))
                    .AfterMap((src, dest) =>
                    {
                        dest.MarkChangesAsCommitted();
                    });
            });

            _mapper = configuration.CreateMapper();
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();

            return await _dbContext
                        .OggettarioEntities
                        .AnyAsync(p =>
                                p.SYSTEM_ID == idAsLong && p.ID_AMM == idTenantAsLong);
        }

        protected override async Task<Oggetto> HandleGet(Oggetto newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var oggettoEntity = await _dbContext.OggettarioEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == id.AsLong() && r.ID_AMM == idTenant.AsLong());

            if (oggettoEntity == null)
                throw new OggettoNotFoundPi3Exception(id);

            return _mapper.Map<Oggetto>(oggettoEntity);
        }

        protected override async Task HandleAdd(Oggetto aggregate)
        {
            var oggettoEntity = new OggettarioEntity();

            oggettoEntity.VAR_COD_OGGETTO = aggregate.Name.Value;
            oggettoEntity.VAR_DESC_OGGETTO = aggregate.Description.Value;
            oggettoEntity.ID_REGISTRO = aggregate.Registro.Id.AsLong();
            oggettoEntity.ID_AMM = aggregate.IdTenant.AsLong();
            oggettoEntity.CHA_OCCASIONALE = "0";

            await _dbContext.OggettarioEntities.AddAsync(oggettoEntity);

            await ((DbContext)_dbContext).SaveChangesAsync();

            this.LoadAggregateFromHistory(aggregate, new ElementIdAssignedEvent()
            {
                Id = oggettoEntity.SYSTEM_ID.ToString()
            });
        }

        protected override async Task HandleDelete(Oggetto aggregate)
        {
            var idAsLong = aggregate.Id.AsLong();
            var idTenantAsLong = aggregate.IdTenant.AsLong();

            var oggetto = await _dbContext.OggettarioEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == idAsLong && r.ID_AMM == idTenantAsLong);
            if (oggetto != null)
            {
                oggetto.CHA_OCCASIONALE = "1";

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
        }

        protected override async Task HandleUpdate(Oggetto aggregate)
        {
            var idAsLong = Convert.ToInt64(aggregate.Id);
            var idTenantAsLong = Convert.ToInt64(aggregate.IdTenant);
            var oldOggetto = await _dbContext.OggettarioEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == idAsLong && r.ID_AMM == idTenantAsLong);
            var canUpdate = oldOggetto != null && (!aggregate.Name.Equals(oldOggetto.VAR_COD_OGGETTO) || !aggregate.Description.Equals(oldOggetto.VAR_DESC_OGGETTO));

            if (canUpdate)
            {
                oldOggetto.CHA_OCCASIONALE = "1";

                //AGGIUNGO
                var newOggettoEntity = new OggettarioEntity();
                newOggettoEntity.VAR_COD_OGGETTO = aggregate.Name?.Value ?? oldOggetto.VAR_COD_OGGETTO;
                newOggettoEntity.VAR_DESC_OGGETTO = aggregate.Description?.Value ?? oldOggetto.VAR_DESC_OGGETTO;
                newOggettoEntity.ID_REGISTRO = !string.IsNullOrEmpty(aggregate.Registro?.Id) ? aggregate.Registro.Id.AsLong() : oldOggetto.ID_REGISTRO;
                newOggettoEntity.ID_AMM = !string.IsNullOrEmpty(aggregate.IdTenant) ? aggregate.IdTenant.AsLong() : oldOggetto.ID_AMM;
                newOggettoEntity.CHA_OCCASIONALE = "0";

                await _dbContext.OggettarioEntities.AddAsync(newOggettoEntity);

                await ((DbContext)_dbContext).SaveChangesAsync();

                this.LoadAggregateFromHistory(aggregate, new ElementIdAssignedEvent()
                {
                    Id = newOggettoEntity.SYSTEM_ID.ToString()
                });
            }
        }

        #endregion
    }
}
