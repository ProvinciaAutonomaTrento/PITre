// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;
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
using GetRoleRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRole;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRole
{
    public class GetRoleHandler : IRequestHandler<GetRoleRequest, GetRoleResult>
    {
        #region Public Members

        public GetRoleHandler(ILogger<GetRoleHandler> logger,
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

        public async Task<GetRoleResult> Handle(GetRoleRequest request, CancellationToken cancellationToken)
        {
            OrgRuolo output = null;

            var idCorrGlobali = request.idCorrGlobRuolo.AsLong();

            var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.SYSTEM_ID == idCorrGlobali && c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I" && c.DTA_FINE == null)
                .Select(c => new CorrGlobaliEntity
                {
                    SYSTEM_ID = c.SYSTEM_ID,
                    ID_GRUPPO = c.ID_GRUPPO,
                    ID_TIPO_RUOLO = c.ID_TIPO_RUOLO,
                    VAR_CODICE = c.VAR_CODICE,
                    VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                    VAR_DESC_CORR = c.VAR_DESC_CORR,
                    CHA_RIFERIMENTO = c.CHA_RIFERIMENTO,
                    ID_AMM = c.ID_AMM,
                    CHA_RESPONSABILE = c.CHA_RESPONSABILE,
                    ID_PESO_ORG = c.ID_PESO_ORG,
                    CHA_SEGRETARIO = c.CHA_SEGRETARIO,
                    CHA_DISABLED_TRASM = c.CHA_DISABLED_TRASM
                })
                .FirstOrDefaultAsync();

            if(corrGlobaliEntity != null)
                output = this._mapper.Map<OrgRuolo>(corrGlobaliEntity);

            return new GetRoleResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRoleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, OrgRuolo>()
                     .ForMember(dest => dest.IDCorrGlobale, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.IDGruppo, opt => opt.MapFrom(src => src.ID_GRUPPO))
                     .ForMember(dest => dest.IDTipoRuolo, opt => opt.MapFrom(src => src.ID_TIPO_RUOLO))
                     .ForMember(dest => dest.Codice, opt => opt.MapFrom(src => src.VAR_CODICE))
                     .ForMember(dest => dest.CodiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR))
                     .ForMember(dest => dest.DiRiferimento, opt => opt.MapFrom(src => src.CHA_RIFERIMENTO))
                     .ForMember(dest => dest.IDAmministrazione, opt => opt.MapFrom(src => src.ID_AMM))
                     .ForMember(dest => dest.Responsabile, opt => opt.MapFrom(src => src.CHA_RESPONSABILE))
                     .ForMember(dest => dest.IDPeso, opt => opt.MapFrom(src => src.ID_PESO_ORG))
                     .ForMember(dest => dest.Segretario, opt => opt.MapFrom(src => src.CHA_SEGRETARIO))
                     .ForMember(dest => dest.DisabledTrasm, opt => opt.MapFrom(src => src.CHA_DISABLED_TRASM));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
