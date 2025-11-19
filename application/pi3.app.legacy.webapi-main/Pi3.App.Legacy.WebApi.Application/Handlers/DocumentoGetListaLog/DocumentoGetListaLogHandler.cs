// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
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
using DocumentoGetListaLogRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetListaLog;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetListaLog
{
    public class DocumentoGetListaLogHandler : IRequestHandler<DocumentoGetListaLogRequest, DocumentoGetListaLogResult>
    {
        #region Public Members

        public DocumentoGetListaLogHandler(ILogger<DocumentoGetListaLogHandler> logger, 
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

        public async Task<DocumentoGetListaLogResult> Handle(DocumentoGetListaLogRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.LogDocumento[] output = null;

            try
            {
                long idOggetto = request.idOggetto.AsLong();
                string varOggetto = request.varOggetto;

                string[] varOggettoFilters = null;
                if (varOggetto.Equals("DOCUMENTO") || varOggetto.Equals("ALLEGATO"))
                    varOggettoFilters = varOggetto.Equals("ALLEGATO") ? new string[] { "DOCUMENTO", "ALLEGATO" } : new string[] { "DOCUMENTO" };
                else
                    varOggettoFilters = new string[] { "FOLDER", "FASCICOLO" };

                var logEntities = await this._dbContext.LogEntities
                .Join(this._dbContext.AnagraficaLogEntities, log => log.VAR_COD_AZIONE, anagrafica => anagrafica.VAR_CODICE, (log, anagrafica) => new { log, anagrafica })
                .Join(this._dbContext.LogAttivatoEntities, log => log.anagrafica.SYSTEM_ID, logAttivato => logAttivato.SYSTEM_ID_ANAGRAFICA, (log, anagrafica) => new
                {
                    SYSTEM_ID = log.log.SYSTEM_ID,
                    USERID_OPERATORE = log.log.USERID_OPERATORE,
                    ID_PEOPLE_OPERATORE = log.log.ID_PEOPLE_OPERATORE,
                    DTA_AZIONE = log.log.DTA_AZIONE,
                    ID_GRUPPO_OPERATORE = log.log.ID_GRUPPO_OPERATORE,
                    ID_AMM = log.log.ID_AMM,
                    VAR_DESC_OGGETTO = log.log.VAR_DESC_OGGETTO,
                    VAR_COD_AZIONE = log.log.VAR_COD_AZIONE,
                    CHA_ESITO = log.log.CHA_ESITO,
                    VAR_DESC_AZIONE = log.log.VAR_DESC_AZIONE,
                    DESC_PRODUCER = log.log.DESC_PRODUCER,
                    ID_OGGETTO = log.log.ID_OGGETTO,
                    VAR_OGGETTO = log.log.VAR_OGGETTO
                })
                .Join(this._dbContext.PeopleEntities, log => log.ID_PEOPLE_OPERATORE, people => people.SYSTEM_ID, (log, people) => new { Log = log, PeopleOperatore = people.FULL_NAME })
                .Join(this._dbContext.GroupEntities, log => log.Log.ID_GRUPPO_OPERATORE, group => group.SYSTEM_ID, (log, group) => new { log.Log, log.PeopleOperatore, GroupOperatore = group.GROUP_NAME })
                .Where(l => l.Log.ID_OGGETTO == idOggetto && l.Log.VAR_COD_AZIONE != "OPEN_DET_DOC" && varOggettoFilters.Contains(l.Log.VAR_OGGETTO))
                 .Concat(
                    this._dbContext.LogStoricoEntities
                    .Join(this._dbContext.AnagraficaLogEntities, log => log.VAR_COD_AZIONE, anagrafica => anagrafica.VAR_CODICE, (log, anagrafica) => new { log, anagrafica })
                    .Join(this._dbContext.LogAttivatoEntities, log => log.anagrafica.SYSTEM_ID, logAttivato => logAttivato.SYSTEM_ID_ANAGRAFICA, (log, anagrafica) => new
                    {
                        SYSTEM_ID = log.log.SYSTEM_ID,
                        USERID_OPERATORE = log.log.USERID_OPERATORE,
                        ID_PEOPLE_OPERATORE = log.log.ID_PEOPLE_OPERATORE,
                        DTA_AZIONE = log.log.DTA_AZIONE,
                        ID_GRUPPO_OPERATORE = log.log.ID_GRUPPO_OPERATORE,
                        ID_AMM = log.log.ID_AMM,
                        VAR_DESC_OGGETTO = log.log.VAR_DESC_OGGETTO,
                        VAR_COD_AZIONE = log.log.VAR_COD_AZIONE,
                        CHA_ESITO = log.log.CHA_ESITO,
                        VAR_DESC_AZIONE = log.log.VAR_DESC_AZIONE,
                        DESC_PRODUCER = log.log.DESC_PRODUCER,
                        ID_OGGETTO = log.log.ID_OGGETTO,
                        VAR_OGGETTO = log.log.VAR_OGGETTO
                    })
                    .Join(this._dbContext.PeopleEntities, log => log.ID_PEOPLE_OPERATORE, people => people.SYSTEM_ID, (log, people) => new { Log = log, PeopleOperatore = people.FULL_NAME })
                    .Join(this._dbContext.GroupEntities, log => log.Log.ID_GRUPPO_OPERATORE, group => group.SYSTEM_ID, (log, group) => new { log.Log, log.PeopleOperatore, GroupOperatore = group.GROUP_NAME })
                    .Where(l => l.Log.ID_OGGETTO == idOggetto && l.Log.VAR_COD_AZIONE != "OPEN_DET_DOC" && varOggettoFilters.Contains(l.Log.VAR_OGGETTO))
                    )
                 .Distinct()
                 .ToListAsync();

                if (logEntities != null)
                {
                    var logs = new List<LogDocumento>();
                    logEntities.OrderByDescending(l => l.Log.SYSTEM_ID).ToList().ForEach(e =>
                        {
                            var log = new LogDocumento();
                            log.chaEsito = e.Log.CHA_ESITO;
                            log.codAzione = e.Log.VAR_COD_AZIONE;
                            log.dataAzione = e.Log.DTA_AZIONE.AsDateTimeFormat();
                            log.descrOggetto = e.Log.VAR_DESC_OGGETTO;
                            log.idAmm = e.Log.ID_AMM.GetValueOrDefault().ToString();
                            log.idPeopleOPeratore = e.Log.ID_PEOPLE_OPERATORE.ToString();
                            log.idGruppoOperatore = e.Log.ID_GRUPPO_OPERATORE.ToString();
                            log.descProduttore = e.Log.DESC_PRODUCER;
                            log.userIdOperatore = e.PeopleOperatore;
                            log.idGruppoOperatore = e.GroupOperatore;
                            logs.Add(log);
                        }
                    );
                    output = logs.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new DocumentoGetListaLogResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetListaLogHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
       
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LogEntity, LogDocumento>()
                    .ForMember(dest => dest.chaEsito, src => src.MapFrom(opt => opt.CHA_ESITO))
                    .ForMember(dest => dest.codAzione, src => src.MapFrom(opt => opt.VAR_COD_AZIONE))
                    .ForMember(dest => dest.dataAzione, src => src.MapFrom(opt => opt.DTA_AZIONE))
                    .ForMember(dest => dest.descrOggetto, src => src.MapFrom(opt => opt.VAR_DESC_OGGETTO))
                    .ForMember(dest => dest.idAmm, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.idPeopleOPeratore, src => src.MapFrom(opt => opt.ID_PEOPLE_OPERATORE))
                    .ForMember(dest => dest.idGruppoOperatore, src => src.MapFrom(opt => opt.ID_GRUPPO_OPERATORE))
                    .ForMember(dest => dest.descProduttore, src => src.MapFrom(opt => opt.DESC_PRODUCER));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
