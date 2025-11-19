// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.StatoInvio;
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
using GetListaSpedizioniRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetListaSpedizioni;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetListaSpedizioni
{
    public class GetListaSpedizioniHandler : IRequestHandler<GetListaSpedizioniRequest, GetListaSpedizioniResult>
    {
        #region Public Members

        public GetListaSpedizioniHandler(ILogger<GetListaSpedizioniHandler> logger,
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

        public async Task<GetListaSpedizioniResult> Handle(GetListaSpedizioniRequest request, CancellationToken cancellationToken)
        {
            StatoInvio[] output = null;

            try
            {
                var idProfile = request.idProfile.AsLong();

                var listaSpedizioneEntities = await this._dbContext.StatoInvioEntities
                    .Join(this._dbContext.CorrGlobaliEntities, stato => stato.ID_CORR_GLOBALE, corr => corr.SYSTEM_ID, (stato, corr) => new { stato, corr })
                    .Where(j => j.stato.ID_PROFILE == idProfile && j.stato.DTA_SPEDIZIONE != null)
                    .Select(j => new ListaSpedizioneEntity()
                    {
                        VAR_INDIRIZZO = j.stato.VAR_INDIRIZZO,
                        VAR_CODICE_AOO = j.stato.VAR_CODICE_AOO,
                        VAR_CODICE_AMM = j.stato.VAR_CODICE_AMM,
                        DTA_SPEDIZIONE = j.stato.DTA_SPEDIZIONE,
                        VAR_DESC_CORR = j.corr.VAR_DESC_CORR,
                        ID_CORR_GLOBALE = j.stato.ID_CORR_GLOBALE
                    })
                    .AsNoTracking()
                    .ToListAsync();

                output = this._mapper.Map<StatoInvio[]>(listaSpedizioneEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetListaSpedizioniResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetListaSpedizioniHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ListaSpedizioneEntity, StatoInvio>()
                    .ForMember(dest => dest.indirizzo, opt => opt.MapFrom(src => src.VAR_INDIRIZZO))
                    .ForMember(dest => dest.codiceAmm, opt => opt.MapFrom(src => src.VAR_CODICE_AMM))
                    .ForMember(dest => dest.codiceAOO, opt => opt.MapFrom(src => src.VAR_CODICE_AOO))
                    .ForMember(dest => dest.dataSpedizione, opt => opt.MapFrom(src => src.DTA_SPEDIZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.destinatario, opt => opt.MapFrom(src => src.VAR_DESC_CORR));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class ListaSpedizioneEntity
        {
            public string? VAR_INDIRIZZO { get; set; }
            public string? VAR_CODICE_AOO { get; set; }
            public string? VAR_CODICE_AMM { get; set; }
            public string? VAR_DESC_CORR { get; set; }
            public long? ID_CORR_GLOBALE { get; set; }
            public DateTime? DTA_SPEDIZIONE { get; set; }

        }
        #endregion
    }
}
