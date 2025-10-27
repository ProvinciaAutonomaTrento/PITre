// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.filtri.trasmissione;
using DocsPaVO.filtri;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetInfoTrasmissioniFiltered
{

    // Richiede libreria MediatR
    public class GetInfoTrasmissioniFilteredHandler : IRequestHandler<Requests.GetInfoTrasmissioniFiltered, GetInfoTrasmissioniFilteredResult>
    {
        #region Public Members

        public GetInfoTrasmissioniFilteredHandler(ILogger<GetInfoTrasmissioniFilteredHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
        }

        public async Task<GetInfoTrasmissioniFilteredResult> Handle(Requests.GetInfoTrasmissioniFiltered request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.trasmissione.InfoTrasmissione> result = new List<DocsPaVO.trasmissione.InfoTrasmissione>();
            DocsPaVO.ricerche.SearchPagingContext pagingContext = request.pagingContext;
            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri = request.objListaFiltri;
            string idDocOrFasc = request.idDocOrFasc;
            string docOrFasc = request.docOrFasc;
            DocsPaVO.utente.InfoUtente infoUtente = request.infoUtente;

            try
            {
                //    bool extendFromStruct = false;
                //    bool extendSingleTransmission = false;

                ExpressionStarter<JoinEntity> predicateInOr = null;
                ExpressionStarter<JoinEntity> predicateInOr2 = null;

                var j1 = this._dbContext.TrasmissioneEntities.Join(this._dbContext.PeopleEntities, a => a.ID_PEOPLE, p => p.SYSTEM_ID, (a, p) => new JoinEntity { a = a, p = p });
                var j2 = j1.Join(this._dbContext.CorrGlobaliEntities, j1 => j1.a.ID_RUOLO_IN_UO, g => g.SYSTEM_ID, (j1, g) => new JoinEntity { a = j1.a, p = j1.p, g = g });

                if (objListaFiltri.Any(x => x.argomento.Equals("DESTINATARIO_RUOLO") ||
                                            x.argomento.Equals("RAGIONE")))
                    j2 = j2.Join(this._dbContext.TrasmSingolaEntities, qf => qf.a.SYSTEM_ID, ts => ts.ID_TRASMISSIONE, (qf, ts) => new JoinEntity { a = qf.a, p = qf.p, g = qf.g, ts = ts });
              
                if (objListaFiltri.Any(x => x.argomento.Equals("DESTINATARIO_UTENTE") ||
                                            x.argomento.Equals("MY_RECEIVED_TRANSMISSIONS") ||
                                            x.argomento.Equals("DATA_ACCETTAZIONE_DA") ||
                                            x.argomento.Equals("DATA_RIFIUTO_DA") ||
                                            x.argomento.Equals("VISTE") ||
                                            x.argomento.Equals("PENDENTI")))
                {
                    j2 = j2.Join(this._dbContext.TrasmSingolaEntities, qf => qf.a.SYSTEM_ID, ts => ts.ID_TRASMISSIONE, (qf, ts) => new JoinEntity { a = qf.a, p = qf.p, g = qf.g, ts = ts });
                    j2 = j2.Join(this._dbContext.TrasmUtenteEntities, qf => qf.ts.SYSTEM_ID, tu => tu.ID_TRASM_SINGOLA, (qf, tu) => new JoinEntity { a = qf.a, p = qf.p, g = qf.g, ts = qf.ts, tu = tu });
                }

                var queryFilters = docOrFasc.Equals("D") ? j2.Where(x => x.a.ID_PROFILE == idDocOrFasc.AsLong()).Select(x => new JoinEntity() { a = x.a, g = x.g, p = x.p, ts = x.ts, tu = x.tu })
                    : j2.Where(x => x.a.ID_PROJECT == idDocOrFasc.AsLong()).Select(x => new JoinEntity() { a = x.a, g = x.g, p = x.p, ts = x.ts, tu = x.tu });

                DocsPaVO.filtri.FiltroRicerca f;
                FiltroRicerca filterRubDest = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.MITT_DEST_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();

                #region FILTRI 
                for (int i = 0; i < objListaFiltri.Length; i++)
                {
                    f = objListaFiltri[i];
                    TimeSpan ts = new TimeSpan(0, 0, 0);
                    if (!string.IsNullOrEmpty(f.valore))
                    {
                        switch (f.argomento)
                        {
                            case "MITTENTE_UTENTE":
                                string userNameMitt = f.valore.Split('|')[0];
                                long idAmmMitt = f.valore.Split('|')[1].AsLong();
                                long userIdMitt = await this._dbContext.PeopleEntities
                                    .Where(x => x.USER_ID.ToUpper().Equals(userNameMitt.ToUpper()) && x.ID_AMM == idAmmMitt)
                                    .Select(x => x.SYSTEM_ID)
                                    .FirstOrDefaultAsync();
                                queryFilters = queryFilters.Where(x => x.a.ID_PEOPLE == userIdMitt);
                                break;
                            case "DESTINATARIO_RUOLO":
                                var idDestAsLong = f.valore.AsLong();
                                if (filterRubDest != null && Convert.ToBoolean(filterRubDest.valore))
                                {
                                    List<long?> destStoricizzati = await GetCorrWithStoricizzati(f.valore, new List<long?>());
                                    queryFilters = queryFilters.Where(x => destStoricizzati.Contains(x.ts.ID_CORR_GLOBALE));
                                }
                                else
                                    queryFilters = queryFilters.Where(x => x.ts.ID_CORR_GLOBALE == idDestAsLong);
                                break;
                            case "DESTINATARIO_UTENTE":
                                string userNameDest = f.valore.Split('|')[0];
                                long idAmmDest = f.valore.Split('|')[1].AsLong();
                                long userIdDest = await this._dbContext.PeopleEntities
                                    .Where(x => x.USER_ID.ToUpper().Equals(userNameDest.ToUpper()) && x.ID_AMM == idAmmDest)
                                    .Select(x => x.SYSTEM_ID)
                                    .FirstOrDefaultAsync();
                                queryFilters = queryFilters.Where(x => x.tu.ID_PEOPLE == userIdDest);
                                //extendSingleTransmission = true;
                                //extendFromStruct = true;
                                break;
                            case "RAGIONE":
                                var idRagione = f.valore.AsLong();
                                queryFilters = queryFilters.Where(x => x.ts.ID_RAGIONE == idRagione);
                                //extendSingleTransmission = true;
                                break;
                            case "DTA_INVIO":
                                var dateFrom = f.valore.AsDateTime();
                                dateFrom = dateFrom.Date + ts;
                                queryFilters = queryFilters.Where(x => x.a.DTA_INVIO >= dateFrom && x.a.DTA_INVIO <= dateFrom.AddDays(1));
                                break;
                            case "MY_RECEIVED_TRANSMISSIONS":
                                List<long?> dest = await this.GetCorrWithStoricizzati(!string.IsNullOrEmpty(f.valore.Split('|')[0]) ? f.valore.Split('|')[0] : "0", new List<long?>());
                                long val1 = !string.IsNullOrEmpty(f.valore.Split('|')[1]) ? f.valore.Split('|')[1].AsLong() : 0;
                                long val2 = !string.IsNullOrEmpty(f.valore.Split('|')[2]) ? f.valore.Split('|')[2].AsLong() : 0;
                                dest.Add(val2);

                                if (predicateInOr == null)
                                    predicateInOr = PredicateBuilder.New<JoinEntity>();
                                predicateInOr = predicateInOr.Or(x => dest.Contains(x.ts.ID_CORR_GLOBALE) && x.tu.ID_PEOPLE == val1);
                                break;
                            case "EFFETTUATE_RUOLI_IN_RF":
                                long idRegAsLong = f.valore.AsLong();
                                List<long?> roleList = await this._dbContext.RuoloRegistroEntities.Where(x => x.ID_REGISTRO == idRegAsLong).Select(x => x.ID_RUOLO_IN_UO).ToListAsync();
                                if (predicateInOr == null)
                                    predicateInOr = PredicateBuilder.New<JoinEntity>();
                                predicateInOr = predicateInOr.Or(x => roleList.Contains(x.a.ID_RUOLO_IN_UO));
                                break;
                            case "MITTENTE_RUOLO":
                                var idMittAsLong = f.valore.AsLong();
                                if (predicateInOr == null)
                                    predicateInOr = PredicateBuilder.New<JoinEntity>();
                                if (filterRubDest != null && Convert.ToBoolean(filterRubDest.valore))
                                {
                                    List<long?> destStoricizzati = await GetCorrWithStoricizzati(f.valore, new List<long?>());
                                    predicateInOr = predicateInOr.Or(x => destStoricizzati.Contains(x.a.ID_RUOLO_IN_UO));
                                }
                                else
                                    predicateInOr = predicateInOr.Or(x => x.a.ID_RUOLO_IN_UO == idMittAsLong);
                                break;
                            case "DATA_ACCETTAZIONE_DA":
                                if (predicateInOr2 == null)
                                    predicateInOr2 = PredicateBuilder.New<JoinEntity>();
                                predicateInOr2 = predicateInOr2.Or(x => x.tu.DTA_ACCETTATA > "01/01/1900".AsDateTime());
                                break;
                            case "DATA_RIFIUTO_DA":
                                if (predicateInOr2 == null)
                                    predicateInOr2 = PredicateBuilder.New<JoinEntity>();
                                predicateInOr2 = predicateInOr2.Or(x => x.tu.DTA_RIFIUTATA > "01/01/1900".AsDateTime());
                                break;
                            case "VISTE":
                                if (predicateInOr2 == null)
                                    predicateInOr2 = PredicateBuilder.New<JoinEntity>();
                                var dataVista = "01/01/1900".AsDateTime();
                                dataVista = dataVista.Date + ts;
                                predicateInOr2 = predicateInOr2.Or(x => x.tu.DTA_VISTA > dataVista);
                                break;
                            case "PENDENTI":
                                if (predicateInOr2 == null)
                                    predicateInOr2 = PredicateBuilder.New<JoinEntity>();
                                predicateInOr2 = predicateInOr2.Or(x => !this._dbContext.TrasmUtenteEntities.Any(t =>  t.ID_TRASM_SINGOLA == x.ts.SYSTEM_ID && (t.DTA_ACCETTATA.HasValue || t.DTA_RIFIUTATA.HasValue)  && x.ts.ID_TRASMISSIONE == x.a.SYSTEM_ID &&
                                this._dbContext.RagioneTrasmissioneEntities.Any(r => !string.IsNullOrEmpty(r.CHA_TIPO_RAGIONE) && r.CHA_TIPO_RAGIONE.Equals("W") && x.ts.ID_RAGIONE == r.SYSTEM_ID)));

                                //NOT EXISTS(SELECT 'X' FROM DPA_TRASM_UTENTE tu, DPA_TRASM_SINGOLA ts WHERE tu.id_trasm_singola = ts.system_id and(tu.dta_accettata IS NOT NULL OR tu.dta_rifiutata IS NOT NULL) AND ts.ID_TRASMISSIONE = A.SYSTEM_ID  AND EXISTS(select 'Y' from dpa_ragione_trasm r where r.cha_tipo_ragione = 'W' and ts.id_ragione = r.system_id ))_
                                break;
                        }

                    }
                }


                if (predicateInOr != null)
                    queryFilters = queryFilters.Where(predicateInOr);

                if (predicateInOr2 != null)
                    queryFilters = queryFilters.Where(predicateInOr2);

                var queryFiltersCount =  queryFilters.GroupBy(x => x.a.SYSTEM_ID);

                pagingContext.SetRecordCount(await queryFiltersCount.CountAsync());

                if (pagingContext.RecordCount > 0)
                {
                    var resultList = await queryFilters
                        .Select(x => new
                        {
                            x.a.SYSTEM_ID,
                            RUOLO_SYSTEM_ID = x.a.ID_RUOLO_IN_UO,
                            RUOLO_MITTENTE = x.g.VAR_COD_RUBRICA,
                            USER_SYSTEM_ID = x.a.ID_PEOPLE,
                            USER_ID = x.p.FULL_NAME,
                            x.a.CHA_TIPO_OGGETTO,
                            x.a.ID_PROFILE,
                            x.a.ID_PROJECT,
                            x.a.DTA_INVIO,
                            x.a.VAR_NOTE_GENERALI,
                            x.a.CHA_SALVATA_CON_CESSIONE,
                            RUOLO = x.g.VAR_DESC_CORR,
                            US_ID = x.p.USER_ID,
                            x.a.ID_PEOPLE_DELEGATO
                        })
                        .Distinct()
                        .OrderByDescending(x => x.DTA_INVIO)
                        .ThenByDescending(x => x.SYSTEM_ID)
                        .Skip(pagingContext.StartRow - 1)
                        .Take(pagingContext.PageSize)
                        .ToListAsync();

                    foreach (var res in resultList)
                    {
                        DocsPaVO.trasmissione.InfoTrasmissione itemToAdd = new DocsPaVO.trasmissione.InfoTrasmissione()
                        {
                            IdTrasmissione = res.SYSTEM_ID.ToString(),
                            IdRuolo = res.RUOLO_SYSTEM_ID?.ToString(),
                            Ruolo = res.RUOLO,
                            IdUtente = res.USER_SYSTEM_ID?.ToString(),
                            Utente = res.USER_ID.ToString(),
                            Tipo = res.CHA_TIPO_OGGETTO,
                            IdDocumento = res.ID_PROFILE?.ToString(),
                            IdFascicolo = res.ID_PROJECT?.ToString(),
                            DataInvio = res.DTA_INVIO != null ? res.DTA_INVIO.AsDateTimeFormat() : string.Empty,
                            NoteGenerali = res.VAR_NOTE_GENERALI ?? string.Empty,
                            SalvataConCessione = res.CHA_SALVATA_CON_CESSIONE ?? string.Empty
                        };
                        if (res.ID_PEOPLE_DELEGATO != null || res.ID_PEOPLE_DELEGATO != 0)
                        {
                            string descUtDelegato = await this._dbContext.PeopleEntities
                                .Where(x => x.SYSTEM_ID == res.ID_PEOPLE_DELEGATO)
                                .Select(x => string.Concat(x.VAR_NOME, " ", x.VAR_COGNOME))
                                .FirstOrDefaultAsync();

                            itemToAdd.UtenteDelegato = descUtDelegato ?? string.Empty;
                        }
                        result.Add(itemToAdd);
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetInfoTrasmissioniFilteredResult(result.ToArray(), pagingContext);
        }

        private async Task<List<long?>> GetCorrWithStoricizzati(string valore, List<long?> outputList)
        {
            if (!string.IsNullOrEmpty(valore) && !valore.Equals("0"))
            {
                outputList.Add(valore.AsLong());
                string idOld = this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == valore.AsLong()).Select(x => x.ID_OLD != null ? x.ID_OLD : 0).FirstOrDefault().ToString();
                return await GetCorrWithStoricizzati(idOld, outputList);
            }
            return outputList;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetInfoTrasmissioniFilteredHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;

        protected class JoinEntity
        {
            public TrasmissioneEntity a { get; set; }
            public PeopleEntity p { get; set; }
            public CorrGlobaliEntity g { get; set; }
            public TrasmSingolaEntity ts { get; set; }
            public TrasmUtenteEntity tu { get; set; }
        }
        #endregion
    }

}
