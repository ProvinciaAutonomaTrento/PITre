// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.Procedimento;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.InteroperabilitaAggiornamentoConferma
{

    // Richiede libreria MediatR
    public class InteroperabilitaAggiornamentoConfermaHandler : IRequestHandler<Application.Requests.InteroperabilitaAggiornamentoConferma, InteroperabilitaAggiornamentoConfermaResult>
    {
        #region Public Members

        public InteroperabilitaAggiornamentoConfermaHandler(ILogger<InteroperabilitaAggiornamentoConfermaHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<InteroperabilitaAggiornamentoConfermaResult> Handle(Application.Requests.InteroperabilitaAggiornamentoConferma request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.idProfile))
                return null;

            var idProfile = request.idProfile.AsLong();
            var idCorrGlobali = request.corrispondente.systemId.AsLong();

            var statoInvioEntities = await _dbContext.StatoInvioEntities
                .Where(w => w.ID_PROFILE == idProfile && w.ID_CORR_GLOBALE == idCorrGlobali)
                .ToListAsync();

            var protocolloDestinatario = _mapper.Map<ProtocolloDestinatario[]>(statoInvioEntities);

            return new InteroperabilitaAggiornamentoConfermaResult(protocolloDestinatario.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InteroperabilitaAggiornamentoConfermaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StatoInvioEntity, ProtocolloDestinatario>()
                     .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.VAR_CODICE_AMM))
                     .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.VAR_CODICE_AOO))
                     .ForMember(dest => dest.dta_spedizione, opt => opt.MapFrom(src => src.DTA_SPEDIZIONE.HasValue ? src.DTA_SPEDIZIONE.AsDateTimeFormat() : string.Empty))
                     .ForMember(dest => dest.protocolloDestinatario, opt => opt.MapFrom(src => src.VAR_PROTO_DEST))
                     .ForMember(dest => dest.dataProtocolloDestinatario, opt => opt.MapFrom(src => src.DTA_PROTO_DEST.HasValue ? src.DTA_PROTO_DEST.AsDateTimeFormat() : string.Empty))
                     .ForMember(dest => dest.documentType, opt => opt.MapFrom(src => src.ID_DOCUMENTTYPE))
                     .ForMember(dest => dest.descrizioneCorr, opt => opt.MapFrom(src => src.ID_CORR_GLOBALE))
                     .ForMember(dest => dest.annullato, opt => opt.MapFrom(src => src.CHA_ANNULLATO))
                     .ForMember(dest => dest.motivo, opt => opt.MapFrom(src => src.VAR_MOTIVO_ANNULLA))
                     .ForMember(dest => dest.provvedimento, opt => opt.MapFrom(src => src.VAR_PROVVEDIMENTO));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
