// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.trasmissione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Logger.CodAzione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetListaLogFilter
{
    // Richiede libreria MediatR
    public class DocumentoGetListaLogFilterHandler : IRequestHandler<Application.Requests.DocumentoGetListaLogFilter, DocumentoGetListaLogFilterResult>
    {
        #region Public Members

        public DocumentoGetListaLogFilterHandler(ILogger<DocumentoGetListaLogFilterHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoGetListaLogFilterResult> Handle(Application.Requests.DocumentoGetListaLogFilter request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.LogDocumento[] res = null;
            var logs = new List<LogDocumento>();
            DocsPaVO.filtri.FilterVisibility[] filters = request.filter;
            string varOggetto = request.varOggetto;
            long idOggetto = !string.IsNullOrEmpty(request.idOggetto) ? request.idOggetto.AsLong() : 0;
            long idFolder = !string.IsNullOrEmpty(request.idFolder) ? request.idFolder.AsLong() : 0;

            try
            {
                if (varOggetto.Equals("DOCUMENTO")) //Documento
                {
                    var q1 = this._dbContext.LogEntities
                        .Join(this._dbContext.AnagraficaLogEntities, log => log.VAR_COD_AZIONE, al => al.VAR_CODICE, (log, al) => new { log, al });
                    var q2 = q1
                        .Join(this._dbContext.LogAttivatoEntities, q1 => q1.al.SYSTEM_ID, la => Convert.ToInt32(la.SYSTEM_ID_ANAGRAFICA), (q1, la) => new { q1.log, q1.al, la });

                    var union1 = q2.Where(x => x.log.ID_OGGETTO == idOggetto && x.log.VAR_OGGETTO.Equals("DOCUMENTO") && !x.log.VAR_COD_AZIONE.Equals("OPEN_DET_DOC"))
                        .Select(x => new
                        {
                            SYSTEM_ID = x.log.SYSTEM_ID,
                            USERID_OPERATORE = x.log.USERID_OPERATORE,
                            ID_PEOPLE_OPERATORE = x.log.ID_PEOPLE_OPERATORE,
                            DTA_AZIONE = x.log.DTA_AZIONE,
                            ID_GRUPPO_OPERATORE = x.log.ID_GRUPPO_OPERATORE,
                            ID_AMM = x.log.ID_AMM,
                            VAR_DESC_OGGETTO = x.log.VAR_DESC_OGGETTO,
                            VAR_COD_AZIONE = x.log.VAR_COD_AZIONE,
                            CHA_ESITO = x.log.CHA_ESITO,
                            VAR_DESC_AZIONE = x.log.VAR_DESC_AZIONE,
                            DESC_PRODUCER = x.log.DESC_PRODUCER
                        });


                    var q3 = this._dbContext.LogStoricoEntities
                        .Join(this._dbContext.AnagraficaLogEntities, ls => ls.VAR_COD_AZIONE, al => al.VAR_CODICE, (ls, al) => new { ls, al });
                    var q4 = q3
                        .Join(this._dbContext.LogAttivatoEntities, q1 => q1.al.SYSTEM_ID, la => Convert.ToInt32(la.SYSTEM_ID_ANAGRAFICA), (q1, la) => new { q1.ls, q1.al, la });

                    var union2 = q4.Where(x => x.ls.ID_OGGETTO == idOggetto && x.ls.VAR_OGGETTO.Equals("DOCUMENTO") && !x.ls.VAR_COD_AZIONE.Equals("OPEN_DET_DOC"))
                        .Select(x => new
                        {
                            SYSTEM_ID = x.ls.SYSTEM_ID,
                            USERID_OPERATORE = x.ls.USERID_OPERATORE,
                            ID_PEOPLE_OPERATORE = x.ls.ID_PEOPLE_OPERATORE,
                            DTA_AZIONE = x.ls.DTA_AZIONE,
                            ID_GRUPPO_OPERATORE = x.ls.ID_GRUPPO_OPERATORE,
                            ID_AMM = x.ls.ID_AMM,
                            VAR_DESC_OGGETTO = x.ls.VAR_DESC_OGGETTO,
                            VAR_COD_AZIONE = x.ls.VAR_COD_AZIONE,
                            CHA_ESITO = x.ls.CHA_ESITO,
                            VAR_DESC_AZIONE = x.ls.VAR_DESC_AZIONE,
                            DESC_PRODUCER = x.ls.DESC_PRODUCER
                        });

                    if (filters != null && filters.Length > 0)
                    {
                        foreach (FilterVisibility filter in filters)
                        {
                            DateTime value;
                            DateTime dateFrom;
                            DateTime dateTo;
                            switch (filter.Type)
                            {
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE:
                                    value = filter.Value.AsDateTime();
                                    dateFrom = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                                    dateTo = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
                                    union1 = union1.Where(x =>
                                        x.DTA_AZIONE >= dateFrom &&
                                        x.DTA_AZIONE <= dateTo);
                                    union2 = union2.Where(x =>
                                        x.DTA_AZIONE >= dateFrom &&
                                        x.DTA_AZIONE <= dateTo);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_FROM:
                                    value = filter.Value.AsDateTime();
                                    dateFrom = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                                    union1 = union1.Where(x => x.DTA_AZIONE >= dateFrom);
                                    union2 = union2.Where(x => x.DTA_AZIONE >= dateFrom);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_TO:
                                    value = filter.Value.AsDateTime();
                                    dateTo = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
                                    union1 = union1.Where(x => x.DTA_AZIONE <= dateTo);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= dateTo);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_WEEK:
                                    union1 = union1.Where(x => x.DTA_AZIONE <= DateTime.Now.AddDays(-7) && x.DTA_AZIONE <= DateTime.Now);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= DateTime.Now.AddDays(-7) && x.DTA_AZIONE <= DateTime.Now);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_MONTH:
                                    union1 = union1.Where(x => x.DTA_AZIONE <= DateTime.Now.AddMonths(-1) && x.DTA_AZIONE <= DateTime.Now);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= DateTime.Now.AddMonths(-1) && x.DTA_AZIONE <= DateTime.Now);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.USER:
                                    var idPeople = this._dbContext.CorrGlobaliEntities.FirstOrDefault(x => x.SYSTEM_ID == Convert.ToInt64(filter.Value)).ID_PEOPLE;
                                    union1 = union1.Where(x => x.ID_PEOPLE_OPERATORE == idPeople);
                                    union2 = union2.Where(x => x.ID_PEOPLE_OPERATORE == idPeople);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.ROLE:
                                    var idGroup = this._dbContext.CorrGlobaliEntities.FirstOrDefault(x => x.SYSTEM_ID == Convert.ToInt64(filter.Value)).ID_GRUPPO;
                                    union1 = union1.Where(x => x.ID_GRUPPO_OPERATORE == idGroup);
                                    union2 = union2.Where(x => x.ID_GRUPPO_OPERATORE == idGroup);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.CAUSE:
                                    union1 = union1.Where(x => x.VAR_COD_AZIONE.Equals(filter.Value));
                                    union2 = union2.Where(x => x.VAR_COD_AZIONE.Equals(filter.Value));
                                    break;
                            }
                        }
                    }

                    var logsEntities = union1.Concat(union2)
                        .Distinct()
                        .OrderByDescending(x => x.SYSTEM_ID)
                        .ToList();

                    logsEntities.ForEach(x =>
                    {
                        LogDocumento log = new LogDocumento
                        {
                            chaEsito = x.CHA_ESITO,
                            codAzione = x.VAR_COD_AZIONE,
                            dataAzione = x.DTA_AZIONE.AsDateTimeFormat(),
                            descrOggetto = x.VAR_DESC_OGGETTO,
                            idAmm = x.ID_AMM.GetValueOrDefault().ToString(),
                            idPeopleOPeratore = x.ID_PEOPLE_OPERATORE.ToString(),
                            idGruppoOperatore = this._dbContext.CorrGlobaliEntities.Where(p => p.ID_GRUPPO == x.ID_GRUPPO_OPERATORE).ToList().FirstOrDefault().VAR_DESC_CORR, //x.RUOLO_DESC,
                            descProduttore = x.DESC_PRODUCER,
                            userIdOperatore = this._dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == x.ID_PEOPLE_OPERATORE).ToList().FirstOrDefault().FULL_NAME//x.PEOPLE_DESC
                        };
                        logs.Add(log);
                    });
                }
                else if (varOggetto.Equals("ALLEGATO")) //Allegato
                {
                    var q1 = this._dbContext.LogEntities
                        .Join(this._dbContext.AnagraficaLogEntities, log => log.VAR_COD_AZIONE, al => al.VAR_CODICE, (log, al) => new { log, al });
                    var q2 = q1
                        .Join(this._dbContext.LogAttivatoEntities, q1 => q1.al.SYSTEM_ID, la => Convert.ToInt32(la.SYSTEM_ID_ANAGRAFICA), (q1, la) => new { q1.log, q1.al, la });

                    var union1 = q2.Where(x => x.log.ID_OGGETTO == idOggetto && (x.log.VAR_OGGETTO.Equals("ALLEGATO") || x.log.VAR_OGGETTO.Equals("DOCUMENTO")) && !x.log.VAR_COD_AZIONE.Equals("OPEN_DET_DOC"))
                        .Select(x => new
                        {
                            SYSTEM_ID = x.log.SYSTEM_ID,
                            USERID_OPERATORE = x.log.USERID_OPERATORE,
                            ID_PEOPLE_OPERATORE = x.log.ID_PEOPLE_OPERATORE,
                            DTA_AZIONE = x.log.DTA_AZIONE,
                            ID_GRUPPO_OPERATORE = x.log.ID_GRUPPO_OPERATORE,
                            ID_AMM = x.log.ID_AMM,
                            VAR_DESC_OGGETTO = x.log.VAR_DESC_OGGETTO,
                            VAR_COD_AZIONE = x.log.VAR_COD_AZIONE,
                            CHA_ESITO = x.log.CHA_ESITO,
                            VAR_DESC_AZIONE = x.log.VAR_DESC_AZIONE,
                            DESC_PRODUCER = x.log.DESC_PRODUCER
                        });

                    var q3 = this._dbContext.LogStoricoEntities
                        .Join(this._dbContext.AnagraficaLogEntities, ls => ls.VAR_COD_AZIONE, al => al.VAR_CODICE, (ls, al) => new { ls, al });
                    var q4 = q3
                        .Join(this._dbContext.LogAttivatoEntities, q1 => q1.al.SYSTEM_ID, la => Convert.ToInt32(la.SYSTEM_ID_ANAGRAFICA), (q1, la) => new { q1.ls, q1.al, la });

                    var union2 = q4.Where(x => x.ls.ID_OGGETTO == idOggetto && (x.ls.VAR_OGGETTO.Equals("ALLEGATO") || x.ls.VAR_OGGETTO.Equals("DOCUMENTO")) && !x.ls.VAR_COD_AZIONE.Equals("OPEN_DET_DOC"))
                        .Select(x => new
                        {
                            SYSTEM_ID = x.ls.SYSTEM_ID,
                            USERID_OPERATORE = x.ls.USERID_OPERATORE,
                            ID_PEOPLE_OPERATORE = x.ls.ID_PEOPLE_OPERATORE,
                            DTA_AZIONE = x.ls.DTA_AZIONE,
                            ID_GRUPPO_OPERATORE = x.ls.ID_GRUPPO_OPERATORE,
                            ID_AMM = x.ls.ID_AMM,
                            VAR_DESC_OGGETTO = x.ls.VAR_DESC_OGGETTO,
                            VAR_COD_AZIONE = x.ls.VAR_COD_AZIONE,
                            CHA_ESITO = x.ls.CHA_ESITO,
                            VAR_DESC_AZIONE = x.ls.VAR_DESC_AZIONE,
                            DESC_PRODUCER = x.ls.DESC_PRODUCER
                        });

                    if (filters != null && filters.Length > 0)
                    {
                        foreach (FilterVisibility filter in filters)
                        {
                            DateTime value;
                            DateTime dateFrom;
                            DateTime dateTo;
                            switch (filter.Type)
                            {
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE:
                                    value = filter.Value.AsDateTime();
                                    dateFrom = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                                    dateTo = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
                                    union1 = union1.Where(x =>
                                        x.DTA_AZIONE >= dateFrom &&
                                        x.DTA_AZIONE <= dateTo);
                                    union2 = union2.Where(x =>
                                        x.DTA_AZIONE >= dateFrom &&
                                        x.DTA_AZIONE <= dateTo);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_FROM:
                                    value = filter.Value.AsDateTime();
                                    dateFrom = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                                    union1 = union1.Where(x => x.DTA_AZIONE >= dateFrom);
                                    union2 = union2.Where(x => x.DTA_AZIONE >= dateFrom);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_TO:
                                    value = filter.Value.AsDateTime();
                                    dateTo = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
                                    union1 = union1.Where(x => x.DTA_AZIONE <= dateTo);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= dateTo);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_WEEK:
                                    union1 = union1.Where(x => x.DTA_AZIONE <= DateTime.Now.AddDays(-7) && x.DTA_AZIONE <= DateTime.Now);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= DateTime.Now.AddDays(-7) && x.DTA_AZIONE <= DateTime.Now);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_MONTH:
                                    union1 = union1.Where(x => x.DTA_AZIONE <= DateTime.Now.AddMonths(-1) && x.DTA_AZIONE <= DateTime.Now);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= DateTime.Now.AddMonths(-1) && x.DTA_AZIONE <= DateTime.Now);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.USER:
                                    var idPeople = this._dbContext.CorrGlobaliEntities.FirstOrDefault(x => x.SYSTEM_ID == Convert.ToInt64(filter.Value)).ID_PEOPLE;
                                    union1 = union1.Where(x => x.ID_PEOPLE_OPERATORE == idPeople);
                                    union2 = union2.Where(x => x.ID_PEOPLE_OPERATORE == idPeople);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.ROLE:
                                    var idGroup = this._dbContext.CorrGlobaliEntities.FirstOrDefault(x => x.SYSTEM_ID == Convert.ToInt64(filter.Value)).ID_GRUPPO;
                                    union1 = union1.Where(x => x.ID_GRUPPO_OPERATORE == idGroup);
                                    union2 = union2.Where(x => x.ID_GRUPPO_OPERATORE == idGroup);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.CAUSE:
                                    union1 = union1.Where(x => x.VAR_COD_AZIONE.Equals(filter.Value));
                                    union2 = union2.Where(x => x.VAR_COD_AZIONE.Equals(filter.Value));
                                    break;
                            }
                        }
                    }


                    var logsEntities = union1.Concat(union2)
                        .Distinct()
                        .OrderByDescending(x => x.SYSTEM_ID)
                        .ToList();

                    logsEntities.ForEach(x =>
                    {
                        LogDocumento log = new LogDocumento
                        {
                            chaEsito = x.CHA_ESITO,
                            codAzione = x.VAR_COD_AZIONE,
                            dataAzione = x.DTA_AZIONE.AsDateTimeFormat(),
                            descrOggetto = x.VAR_DESC_OGGETTO,
                            idAmm = x.ID_AMM.GetValueOrDefault().ToString(),
                            idPeopleOPeratore = x.ID_PEOPLE_OPERATORE.ToString(),
                            idGruppoOperatore = this._dbContext.CorrGlobaliEntities.Where(p => p.ID_GRUPPO == x.ID_GRUPPO_OPERATORE).ToList().FirstOrDefault().VAR_DESC_CORR, //x.RUOLO_DESC,
                            descProduttore = x.DESC_PRODUCER,
                            userIdOperatore = this._dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == x.ID_PEOPLE_OPERATORE).ToList().FirstOrDefault().FULL_NAME//x.PEOPLE_DESC
                        };
                        logs.Add(log);
                    });
                }
                else //Fascicolo
                {
                    var q1 = this._dbContext.LogEntities
                        .Join(this._dbContext.AnagraficaLogEntities, log => log.VAR_COD_AZIONE, al => al.VAR_CODICE, (log, al) => new { log, al });
                    var q2 = q1
                        .Join(this._dbContext.LogAttivatoEntities, q1 => q1.al.SYSTEM_ID, la => Convert.ToInt32(la.SYSTEM_ID_ANAGRAFICA), (q1, la) => new { q1.log, q1.al, la });

                    var union1 = q2.Where(x => (x.log.ID_OGGETTO == idOggetto || x.log.ID_OGGETTO == idFolder) && (x.log.VAR_OGGETTO.Equals("FOLDER") || x.log.VAR_OGGETTO.Equals("FASCICOLO")))
                        .Select(x => new
                        {
                            SYSTEM_ID = x.log.SYSTEM_ID,
                            USERID_OPERATORE = x.log.USERID_OPERATORE,
                            ID_PEOPLE_OPERATORE = x.log.ID_PEOPLE_OPERATORE,
                            DTA_AZIONE = x.log.DTA_AZIONE,
                            ID_GRUPPO_OPERATORE = x.log.ID_GRUPPO_OPERATORE,
                            ID_AMM = x.log.ID_AMM,
                            VAR_DESC_OGGETTO = x.log.VAR_DESC_OGGETTO,
                            VAR_COD_AZIONE = x.log.VAR_COD_AZIONE,
                            CHA_ESITO = x.log.CHA_ESITO,
                            VAR_DESC_AZIONE = x.log.VAR_DESC_AZIONE,
                            DESC_PRODUCER = x.log.DESC_PRODUCER
                        });

                    var q3 = this._dbContext.LogStoricoEntities
                        .Join(this._dbContext.AnagraficaLogEntities, ls => ls.VAR_COD_AZIONE, al => al.VAR_CODICE, (ls, al) => new { ls, al });
                    var q4 = q3
                        .Join(this._dbContext.LogAttivatoEntities, q1 => q1.al.SYSTEM_ID, la => Convert.ToInt32(la.SYSTEM_ID_ANAGRAFICA), (q1, la) => new { q1.ls, q1.al, la });

                    var union2 = q4.Where(x => (x.ls.ID_OGGETTO == idOggetto || x.ls.ID_OGGETTO == idFolder) && (x.ls.VAR_OGGETTO.Equals("FOLDER") || x.ls.VAR_OGGETTO.Equals("FASCICOLO")))
                        .Select(x => new
                        {
                            SYSTEM_ID = x.ls.SYSTEM_ID,
                            USERID_OPERATORE = x.ls.USERID_OPERATORE,
                            ID_PEOPLE_OPERATORE = x.ls.ID_PEOPLE_OPERATORE,
                            DTA_AZIONE = x.ls.DTA_AZIONE,
                            ID_GRUPPO_OPERATORE = x.ls.ID_GRUPPO_OPERATORE,
                            ID_AMM = x.ls.ID_AMM,
                            VAR_DESC_OGGETTO = x.ls.VAR_DESC_OGGETTO,
                            VAR_COD_AZIONE = x.ls.VAR_COD_AZIONE,
                            CHA_ESITO = x.ls.CHA_ESITO,
                            VAR_DESC_AZIONE = x.ls.VAR_DESC_AZIONE,
                            DESC_PRODUCER = x.ls.DESC_PRODUCER
                        });

                    if (filters != null && filters.Length > 0)
                    {
                        foreach (FilterVisibility filter in filters)
                        {
                            DateTime value;
                            DateTime dateFrom;
                            DateTime dateTo;
                            switch (filter.Type)
                            {
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE:
                                    value = filter.Value.AsDateTime();
                                    dateFrom = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                                    dateTo = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
                                    union1 = union1.Where(x =>
                                        x.DTA_AZIONE >= dateFrom &&
                                        x.DTA_AZIONE <= dateTo);
                                    union2 = union2.Where(x =>
                                        x.DTA_AZIONE >= dateFrom &&
                                        x.DTA_AZIONE <= dateTo);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_FROM:
                                    value = filter.Value.AsDateTime();
                                    dateFrom = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                                    union1 = union1.Where(x => x.DTA_AZIONE >= dateFrom);
                                    union2 = union2.Where(x => x.DTA_AZIONE >= dateFrom);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_TO:
                                    value = filter.Value.AsDateTime();
                                    dateTo = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
                                    union1 = union1.Where(x => x.DTA_AZIONE <= dateTo);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= dateTo);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_WEEK:
                                    union1 = union1.Where(x => x.DTA_AZIONE <= DateTime.Now.AddDays(-7) && x.DTA_AZIONE <= DateTime.Now);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= DateTime.Now.AddDays(-7) && x.DTA_AZIONE <= DateTime.Now);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.DATE_MONTH:
                                    union1 = union1.Where(x => x.DTA_AZIONE <= DateTime.Now.AddMonths(-1) && x.DTA_AZIONE <= DateTime.Now);
                                    union2 = union2.Where(x => x.DTA_AZIONE <= DateTime.Now.AddMonths(-1) && x.DTA_AZIONE <= DateTime.Now);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.USER:
                                    var idPeople = this._dbContext.CorrGlobaliEntities.FirstOrDefault(x => x.SYSTEM_ID == Convert.ToInt64(filter.Value)).ID_PEOPLE;
                                    union1 = union1.Where(x => x.ID_PEOPLE_OPERATORE == idPeople);
                                    union2 = union2.Where(x => x.ID_PEOPLE_OPERATORE == idPeople);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.ROLE:
                                    var idGroup = this._dbContext.CorrGlobaliEntities.FirstOrDefault(x => x.SYSTEM_ID == Convert.ToInt64(filter.Value)).ID_GRUPPO;
                                    union1 = union1.Where(x => x.ID_GRUPPO_OPERATORE == idGroup);
                                    union2 = union2.Where(x => x.ID_GRUPPO_OPERATORE == idGroup);
                                    break;
                                case DocsPaVO.filtri.TypeFilterVisibility.CAUSE:
                                    union1 = union1.Where(x => x.VAR_COD_AZIONE.Equals(filter.Value));
                                    union2 = union2.Where(x => x.VAR_COD_AZIONE.Equals(filter.Value));
                                    break;
                            }
                        }
                    }

                    var logsEntities = union1.Concat(union2)
                        .Distinct()
                        .OrderByDescending(x => x.SYSTEM_ID)
                        .ToList();

                    logsEntities.ForEach(x =>
                    {
                        LogDocumento log = new LogDocumento
                        {
                            chaEsito = x.CHA_ESITO,
                            codAzione = x.VAR_COD_AZIONE,
                            dataAzione = x.DTA_AZIONE.AsDateTimeFormat(),
                            descrOggetto = x.VAR_DESC_OGGETTO,
                            idAmm = x.ID_AMM.GetValueOrDefault().ToString(),
                            idPeopleOPeratore = x.ID_PEOPLE_OPERATORE.ToString(),
                            idGruppoOperatore = this._dbContext.CorrGlobaliEntities.Where(p => p.ID_GRUPPO == x.ID_GRUPPO_OPERATORE).ToList().FirstOrDefault().VAR_DESC_CORR, //x.RUOLO_DESC,
                            descProduttore = x.DESC_PRODUCER,
                            userIdOperatore = this._dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == x.ID_PEOPLE_OPERATORE).ToList().FirstOrDefault().FULL_NAME//x.PEOPLE_DESC
                        };
                        logs.Add(log);
                    });
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new DocumentoGetListaLogFilterResult(logs.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetListaLogFilterHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
