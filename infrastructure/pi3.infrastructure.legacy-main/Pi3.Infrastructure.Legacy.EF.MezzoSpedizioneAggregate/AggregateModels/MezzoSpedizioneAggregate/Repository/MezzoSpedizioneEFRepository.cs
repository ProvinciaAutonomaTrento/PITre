// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.MezzoSpedizioneAggregate;
using Pi3.Core.AggregateModels.MezzoSpedizioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.MezzoSpedizioneAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.MezzoSpedizioneAggregate.Resources;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.MezzoSpedizioneAggregate.Repository
{
    public class MezzoSpedizioneEFRepository : ElementRepository<MezzoSpedizione>, IMezzoSpedizioneRepository
    {
        public MezzoSpedizioneEFRepository(ILogger<MezzoSpedizioneEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;
            InitializeMapper();
        }

        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected override async Task HandleAdd(MezzoSpedizione element)
        {
            var documentTypeEntity = _mapper.Map<DocumentTypesEntity>(element);
            _dbContext.DocumentTypesEntities.AddAsync(documentTypeEntity);
            await ((DbContext)_dbContext).SaveChangesAsync();
            element.AssignId(documentTypeEntity.SYSTEM_ID.ToString());
        }

        protected override async Task HandleDelete(MezzoSpedizione element)
        {
            var documentTypeEntity = await _dbContext.DocumentTypesEntities.FindAsync(element.Id.Trim().AsLong());

            if (documentTypeEntity == null)
                throw new MezzoSpedizioneNotFoundPi3Exception(element.Id);

            if (documentTypeEntity.DELETED != null)
            {
                throw new MezzoSpedizioneNotSupportedPi3Exception(ErrorDescriptions.MezzoDiSpedizioneGiaEliminato);
            }
            else
            {
                documentTypeEntity.DELETED = "Y";
                await ((DbContext)_dbContext).SaveChangesAsync();
            }
        }

        protected async override Task<bool> HandleExists(string idTenant, string id)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();
            var documentTypeEntity = await _dbContext
                        .DocumentTypesEntities
                        .FirstOrDefaultAsync(p =>
                                p.SYSTEM_ID == idAsLong);
            if (documentTypeEntity != null && documentTypeEntity.DELETED != "Y")
                return true;
            else
                throw new MezzoSpedizioneNotFoundPi3Exception(id);
        }

        protected override async Task<MezzoSpedizione> HandleGet(MezzoSpedizione newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var documentTypeEntity = await _dbContext.DocumentTypesEntities.FindAsync(id.AsLong());

            if (documentTypeEntity != null && documentTypeEntity.DELETED != "Y")
                return _mapper.Map<MezzoSpedizione>(documentTypeEntity);
            else
                throw new MezzoSpedizioneNotFoundPi3Exception(id);
        }

        protected override async Task HandleUpdate(MezzoSpedizione element)
        {
            var documentTypeEntity = await _dbContext.DocumentTypesEntities.FindAsync(element.Id.Trim().AsLong());

            if (documentTypeEntity == null)
                throw new MezzoSpedizioneNotFoundPi3Exception(element.Id);

            if (documentTypeEntity.DELETED != null)
            {
                throw new MezzoSpedizioneNotSupportedPi3Exception(ErrorDescriptions.ModificaMezzoSpedizioneNonConsentita);
            }
            else
            {
                documentTypeEntity.TYPE_ID = !string.IsNullOrWhiteSpace(element.Name.ToString()) ? element.Name.ToString() : null;
                documentTypeEntity.DESCRIPTION = !string.IsNullOrWhiteSpace(element.Description.ToString()) ? element.Description.ToString() : null;
                documentTypeEntity.CHA_TIPO_CANALE = !string.IsNullOrWhiteSpace(element.Description.ToString()) ? element.Name.ToString().Substring(0, 1) : null;
                await ((DbContext)_dbContext).SaveChangesAsync();
            }
        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DocumentTypesEntity, MezzoSpedizione>()
                    .ConstructUsing(src => new MezzoSpedizione(
                    src.SYSTEM_ID.ToString(),
                    _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true),
                    DateTime.Now,
                    new TextValue(src.TYPE_ID),
                    new TextValue(src.DESCRIPTION)))
                    .AfterMap((src, dest) =>
                    {
                        dest.MarkChangesAsCommitted();
                    });

                cfg.CreateMap<MezzoSpedizione, DocumentTypesEntity>()
                    .ForMember(src => src.SYSTEM_ID, opt => opt.MapFrom(dest => dest.Id))
                    .ForMember(src => src.CHA_TIPO_CANALE, opt => opt.MapFrom(dest => dest.Name.ToString().Substring(0, 1)))
                    .ForMember(src => src.DESCRIPTION, opt => opt.MapFrom(dest => dest.Description))
                    .ForMember(src => src.TYPE_ID, opt => opt.MapFrom(dest => dest.Name));
            });

            _mapper = configuration.CreateMapper();
        }
    }
}
