// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmGetListTipiRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListTipiRuolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListTipiRuolo
{

    // Richiede libreria MediatR
    public class AmmGetListTipiRuoloHandler : IRequestHandler<AmmGetListTipiRuoloRequest, AmmGetListTipiRuoloResult>
    {
        #region Public Members

        public AmmGetListTipiRuoloHandler(ILogger<AmmGetListTipiRuoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<AmmGetListTipiRuoloResult> Handle(AmmGetListTipiRuoloRequest request, CancellationToken cancellationToken)
        {
            OrgTipoRuolo[] output = null;
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var tipoRuoloEntity = await this._dbContext.TipoRuoloEntities
                .AsNoTracking()
                .Where(t => t.ID_AMM == idTenantAsLong)
                .OrderBy(x => x.VAR_DESC_RUOLO)
                .ToListAsync();

            output = _mapper.Map<OrgTipoRuolo[]>(tipoRuoloEntity);

            return new AmmGetListTipiRuoloResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetListTipiRuoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoRuoloEntity, OrgTipoRuolo>()
                    .ForMember(dest => dest.IDTipoRuolo, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_RUOLO))
                    .ForMember(dest => dest.Livello, src => src.MapFrom(opt => opt.NUM_LIVELLO))
                    .ForMember(dest => dest.IDAmministrazione, src => src.MapFrom(opt => opt.ID_AMM));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
