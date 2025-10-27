// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using AmmGetDatiUOCorrenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetDatiUOCorrente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetDatiUOCorrente
{
    public class AmmGetDatiUOCorrenteHandler : IRequestHandler<AmmGetDatiUOCorrenteRequest, AmmGetDatiUOCorrenteResult>
    {
        #region Public Members

        public AmmGetDatiUOCorrenteHandler(ILogger<AmmGetDatiUOCorrenteHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<AmmGetDatiUOCorrenteResult> Handle(AmmGetDatiUOCorrenteRequest request, CancellationToken cancellationToken)
        {
            OrgUO output = new OrgUO();
            var idUo = request.idUO.AsLong();

            var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
              .Where(c => c.CHA_TIPO_URP == "U" && c.CHA_TIPO_IE == "I" && c.DTA_FINE == null && c.SYSTEM_ID == idUo)
              .Select(c => new CorrGlobaliEntity
              {
                  SYSTEM_ID = c.SYSTEM_ID,
                  VAR_CODICE = c.VAR_CODICE,
                  VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                  VAR_DESC_CORR = c.VAR_DESC_CORR,
                  ID_AMM =c.ID_AMM,
                  NUM_LIVELLO = c.NUM_LIVELLO,
                  ID_PARENT = c.ID_PARENT,
                  VAR_CODICE_AOO = c.VAR_CODICE_AOO,
                  CLASSIFICA_UO = c.CLASSIFICA_UO
              })
              .FirstAsync();

            if(corrGlobaliEntity != null)
            {
                output = this._mapper.Map<OrgUO>(corrGlobaliEntity);
                output.Ruoli = (await this._dbContext.CorrGlobaliEntities.AsNoTracking().CountAsync(r => r.CHA_TIPO_URP == "R" && r.CHA_TIPO_IE == "I" && r.DTA_FINE == null && r.ID_UO == corrGlobaliEntity.SYSTEM_ID)).ToString();
                output.SottoUo = (await this._dbContext.CorrGlobaliEntities.AsNoTracking().CountAsync(u => u.CHA_TIPO_URP == "U" && u.CHA_TIPO_IE == "I" && u.DTA_FINE == null && u.ID_PARENT == corrGlobaliEntity.SYSTEM_ID)).ToString();
            }

            return new AmmGetDatiUOCorrenteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetDatiUOCorrenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, OrgUO>()
                     .ForMember(dest => dest.IDCorrGlobale, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.Codice, opt => opt.MapFrom(src => src.VAR_CODICE))
                     .ForMember(dest => dest.CodiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR))
                     .ForMember(dest => dest.IDAmministrazione, opt => opt.MapFrom(src => src.ID_AMM))
                     .ForMember(dest => dest.Livello, opt => opt.MapFrom(src => src.NUM_LIVELLO))
                     .ForMember(dest => dest.IDParent, opt => opt.MapFrom(src => src.ID_PARENT))
                     .ForMember(dest => dest.CodiceRegistroInterop, opt => opt.MapFrom(src => src.VAR_CODICE_AOO))
                     .ForMember(dest => dest.Classifica, opt => opt.MapFrom(src => src.CLASSIFICA_UO));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
