// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.ricerche;
using DocsPaVO.trasmissione;
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
using GetTrasmissioniPendentiConWorkflowRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetTrasmissioniPendentiConWorkflow;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTrasmissioniPendentiConWorkflow
{

    public class GetTrasmissioniPendentiConWorkflowHandler : IRequestHandler<GetTrasmissioniPendentiConWorkflowRequest, GetTrasmissioniPendentiConWorkflowResult>
    {
        #region Public Members

        public GetTrasmissioniPendentiConWorkflowHandler(ILogger<GetTrasmissioniPendentiConWorkflowHandler> logger,
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

        public async Task<GetTrasmissioniPendentiConWorkflowResult> Handle(GetTrasmissioniPendentiConWorkflowRequest request, CancellationToken cancellationToken)
        {
            var output = new List<InfoTrasmissione>();
            var idTrasmSingola = new List<string>();
            var pagingContext = request.pagingContext;

            try
            {
                var idPeopleAsLong = request.idPeople.AsLong();
                var idRuoloInUOAsLong = request.idRuoloInUO.AsLong();
                var idDocOrFascAsLong = request.idDocOrFasc.AsLong();

                var queryablePendentiConWorkflow = this._dbContext.TrasmissioneEntities.AsNoTracking()
                    .Join(this._dbContext.TrasmSingolaEntities.AsNoTracking(), t => t.SYSTEM_ID, s => s.ID_TRASMISSIONE, (t, s) => new { t, s })
                    .Join(this._dbContext.TrasmUtenteEntities.AsNoTracking(), j => j.s.SYSTEM_ID, u => u.ID_TRASM_SINGOLA, (j, u) => new { j.t, j.s, u })
                    .Join(this._dbContext.RagioneTrasmissioneEntities.AsNoTracking(), j => j.s.ID_RAGIONE, r => r.SYSTEM_ID, (j, r) => new { j.t, j.s, j.u, r })
                    .Where(j => j.t.DTA_INVIO.HasValue && j.s.ID_CORR_GLOBALE == idRuoloInUOAsLong && j.r.CHA_TIPO_RAGIONE == "W" && j.t.CHA_TIPO_OGGETTO == request.docOrFasc
                           && j.u.CHA_ACCETTATA == "0" && j.u.CHA_RIFIUTATA == "0" && j.u.CHA_VALIDA == "1" && j.u.ID_PEOPLE == idPeopleAsLong);

                queryablePendentiConWorkflow = request.docOrFasc.Equals("D") ? queryablePendentiConWorkflow.Where(j => j.t.ID_PROFILE == idDocOrFascAsLong) : queryablePendentiConWorkflow.Where(j => j.t.ID_PROJECT == idDocOrFascAsLong);

                var idTrasmSingolaAsLong = await queryablePendentiConWorkflow.Select(j => j.s.SYSTEM_ID).ToListAsync();
                
                pagingContext.SetRecordCount(idTrasmSingolaAsLong.Count);

                if(pagingContext.RecordCount > 0)
                {
                    var infoTrasmissioniEntities = await queryablePendentiConWorkflow
                        .Join(this._dbContext.PeopleEntities, j => j.t.ID_PEOPLE, p => p.SYSTEM_ID, (j, p) => new { j.t, j.s, j.u, j.r, p })
                        .Join(this._dbContext.CorrGlobaliEntities, j => j.t.ID_RUOLO_IN_UO, g => g.SYSTEM_ID, (j, g) => new { j.t, j.s, j.u, j.r, j.p, g })
                        .GroupJoin(this._dbContext.PeopleEntities, j => j.t.ID_PEOPLE_DELEGATO, d => d.SYSTEM_ID, (j, d) => new { j.t, j.s, j.u, j.r, j.p, j.g, d })
                        .SelectMany(j => j.d.DefaultIfEmpty(), (j, d) => new InfoTrasmissioneEntity
                        {
                            SYSTEM_ID = j.t.SYSTEM_ID,
                            ID_TRASM_SINGOLA = j.s.SYSTEM_ID,
                            RUOLO_SYSTEM_ID = j.t.ID_RUOLO_IN_UO,
                            RUOLO_MITTENTE = j.g.VAR_COD_RUBRICA,
                            USER_SYSTEM_ID = j.t.ID_PEOPLE,
                            USER_DESCRIPTION = j.p.FULL_NAME,
                            USER_ID = j.p.FULL_NAME,
                            CHA_TIPO_OGGETTO = j.t.CHA_TIPO_OGGETTO,
                            ID_PROFILE = j.t.ID_PROFILE,
                            ID_PROJECT = j.t.ID_PROJECT,
                            DTA_INVIO = j.t.DTA_INVIO,
                            VAR_NOTE_GENERALI = j.t.VAR_NOTE_GENERALI,
                            CHA_SALVATA_CON_CESSIONE = j.t.CHA_SALVATA_CON_CESSIONE,
                            RUOLO = j.g.VAR_DESC_CORR,
                            ID_PEOPLE_DELEGATO = j.t.ID_PEOPLE_DELEGATO,
                            UTENTE_DELEGATO = d.FULL_NAME
                        })
                        .OrderByDescending(j => j.DTA_INVIO)
                        .Skip(pagingContext.StartRow - 1)
                        .Take(pagingContext.PageSize)
                        .ToListAsync();

                    output = this._mapper.Map<InfoTrasmissione[]>(infoTrasmissioniEntities).ToList();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetTrasmissioniPendentiConWorkflowResult(output, idTrasmSingola, pagingContext);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTrasmissioniPendentiConWorkflowHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected class InfoTrasmissioneEntity
        {
            public long? SYSTEM_ID { get; set; }
            public DateTime? DTA_INVIO { get; set; }
            public long? ID_TRASM_SINGOLA { get; set; }
            public long? RUOLO_SYSTEM_ID { get; set; }
            public string? RUOLO_MITTENTE { get; set; }
            public long? USER_SYSTEM_ID { get; set; }
            public string? USER_ID { get; set; }
            public string? CHA_TIPO_OGGETTO { get; set; }
            public long? ID_PROFILE { get; set; }
            public long? ID_PROJECT { get; set; }
            public string? VAR_NOTE_GENERALI { get; set; }
            public string? CHA_SALVATA_CON_CESSIONE { get; set; }
            public string? RUOLO { get; set; }
            public string? USER_DESCRIPTION { get; set; }
            public long? ID_PEOPLE_DELEGATO { get; set; }
            public string? UTENTE_DELEGATO { get; set; }

        }

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InfoTrasmissioneEntity, InfoTrasmissione>()
                     .ForMember(dest => dest.IdTrasmissione, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.idTrasmSingola, opt => opt.MapFrom(src => src.ID_TRASM_SINGOLA))
                     .ForMember(dest => dest.IdRuolo, opt => opt.MapFrom(src => src.RUOLO_SYSTEM_ID))
                     .ForMember(dest => dest.Ruolo, src => src.MapFrom(src => src.RUOLO))
                     .ForMember(dest => dest.IdUtente, src => src.MapFrom(src => src.USER_SYSTEM_ID))
                     .ForMember(dest => dest.Utente, opt => opt.MapFrom(src => src.USER_DESCRIPTION))
                     .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.CHA_TIPO_OGGETTO))
                     .ForMember(dest => dest.IdDocumento, opt => opt.MapFrom(src => src.ID_PROFILE))
                     .ForMember(dest => dest.IdFascicolo, opt => opt.MapFrom(src => src.ID_PROJECT))
                     .ForMember(dest => dest.NoteGenerali, opt => opt.MapFrom(src => src.VAR_NOTE_GENERALI))
                     .ForMember(dest => dest.SalvataConCessione, opt => opt.MapFrom(src => src.CHA_SALVATA_CON_CESSIONE))
                     .ForMember(dest => dest.IdDocumento, opt => opt.MapFrom(src => src.ID_PROFILE))
                     .ForMember(dest => dest.IdFascicolo, opt => opt.MapFrom(src => src.ID_PROJECT))
                     .ForMember(dest => dest.UtenteDelegato, opt => opt.MapFrom(src => src.UTENTE_DELEGATO ?? string.Empty));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
