// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.KeywordAggregate;
using Pi3.Core.AggregateModels.KeywordAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.KeywordAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.KeywordAggregate.Repository
{
    public class KeywordEFRepository : ElementRepository<Keyword>, IKeywordRepository
    {
        #region Public Members

        public KeywordEFRepository(ILogger<KeywordEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;

            InitializeMapper();
        }

        protected async override Task<bool> HandleExists(string idTenant, string id)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();
            var paroleEnttry = await _dbContext
                        .ParolaEntities
                        .FirstOrDefaultAsync(p =>
                                p.SYSTEM_ID == idAsLong);
            if (paroleEnttry != null)
                return true;
            else
                throw new KeywordNotFoundPi3Exception(id);
        }

        protected override async Task<Keyword> HandleGet(Keyword aggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var keywordEntity = await _dbContext.ParolaEntities.FindAsync(id.AsLong());

            if (keywordEntity == null)
                throw new KeywordNotFoundPi3Exception(id);

            return _mapper.Map<Keyword>(keywordEntity);                
        }

        protected override async Task HandleAdd(Keyword element)
        {
            if (!_dbContext.ParolaEntities.Where(w => w.ID_AMM == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true)
                && w.VAR_DESC_PAROLA.Equals(element.Name.ToString())).Any() && !string.IsNullOrWhiteSpace(element.Name.ToString()))
            {
                var keywordEntity = _mapper.Map<ParolaEntity>(element);
                
                await _dbContext.ParolaEntities.AddAsync(keywordEntity);

                await ((DbContext)_dbContext).SaveChangesAsync();
                
                element.AssignId(keywordEntity.SYSTEM_ID.ToString());
            }
            else
                throw new KeywordAlreadyExistPi3Exception();
        }

        protected override Task HandleDelete(Keyword element)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleUpdate(Keyword element)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Members

        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ParolaEntity, Keyword>()
                    .ConstructUsing(src => new Keyword(
                    src.SYSTEM_ID.ToString(),
                    _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true),
                    DateTime.Now,
                    new TextValue(src.VAR_DESC_PAROLA),
                    null))
                    .AfterMap((src, dest) =>
                    {
                        dest.MarkChangesAsCommitted();
                    });

                cfg.CreateMap<Keyword, ParolaEntity>()
                    .ForMember(src => src.SYSTEM_ID, opt => opt.MapFrom(dest => dest.Id))
                    .ForMember(src => src.VAR_DESC_PAROLA, opt => opt.MapFrom(dest => dest.Name))
                    .ForMember(src => src.ID_AMM, opt => opt.MapFrom(dest => dest.IdTenant));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
