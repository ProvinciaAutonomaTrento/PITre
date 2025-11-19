// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmGetListaRuoliAOORequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListaRuoliAOO;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListaRuoliAOO
{

    public class AmmGetListaRuoliAOOHandler : IRequestHandler<AmmGetListaRuoliAOORequest, AmmGetListaRuoliAOOResult>
    {
        #region Public Members

        public AmmGetListaRuoliAOOHandler(ILogger<AmmGetListaRuoliAOOHandler> logger, 
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

        public async Task<AmmGetListaRuoliAOOResult> Handle(AmmGetListaRuoliAOORequest request, CancellationToken cancellationToken)
        {
            long idRegistroAsLong = request.idRegistro.AsLong();

            var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(this._dbContext.RuoloRegistroEntities.AsNoTracking(), corrGlobali => corrGlobali.SYSTEM_ID, ruoloRegistro => ruoloRegistro.ID_RUOLO_IN_UO, (corrGlobali, ruoloRegistro) => new
                {
                    SYSTEM_ID = corrGlobali.SYSTEM_ID,
                    VAR_COD_RUBRICA = corrGlobali.VAR_COD_RUBRICA,
                    VAR_DESC_CORR = corrGlobali.VAR_DESC_CORR,
                    ID_GRUPPO = corrGlobali.ID_GRUPPO,
                    ID_AMM = corrGlobali.ID_AMM,
                    ruoloRegistro.ID_REGISTRO
                })
                .Where(a => a.ID_REGISTRO == idRegistroAsLong)
                .Select(a => new CorrGlobaliEntity
                {
                    SYSTEM_ID = a.SYSTEM_ID,
                    VAR_COD_RUBRICA = a.VAR_COD_RUBRICA,
                    VAR_DESC_CORR = a.VAR_DESC_CORR,
                    ID_GRUPPO = a.ID_GRUPPO,
                    ID_AMM = a.ID_AMM,
                })
                .ToListAsync();

            var output = _mapper.Map<OrgRuolo[]>(corrGlobaliEntity);

            return new AmmGetListaRuoliAOOResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetListaRuoliAOOHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, OrgRuolo>()
                    .ForMember(dest => dest.IDCorrGlobale, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.IDGruppo, src => src.MapFrom(opt => opt.ID_GRUPPO))
                    .ForMember(dest => dest.CodiceRubrica, src => src.MapFrom(opt => opt.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.IDAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_CORR));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
