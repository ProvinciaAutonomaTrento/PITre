// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.filtri.trasmissione;
using DocsPaVO.InstanceAccess.Metadata;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrasmissioneGetQueryEffettuatePagingLiteRequest = Pi3.App.Legacy.WebApi.Application.Requests.TrasmissioneGetQueryEffettuatePagingLite;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneGetQueryEffettuatePagingLite
{
    public class TrasmissioneGetQueryEffettuatePagingLiteHandler : IRequestHandler<TrasmissioneGetQueryEffettuatePagingLiteRequest, TrasmissioneGetQueryEffettuatePagingLiteResult>
    {
        #region Public Members

        public TrasmissioneGetQueryEffettuatePagingLiteHandler(ILogger<TrasmissioneGetQueryEffettuatePagingLiteHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._configurationService = configurationService;
        }

        public async Task<TrasmissioneGetQueryEffettuatePagingLiteResult> Handle(TrasmissioneGetQueryEffettuatePagingLiteRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            DocsPaVO.trasmissione.OggettoTrasm oggettoTrasmesso = request.oggettoTrasmesso;
            DocsPaVO.filtri.FiltroRicerca[] listaFiltri = request.listaFiltri;
            DocsPaVO.utente.Utente utente = request.utente;
            DocsPaVO.utente.Ruolo ruolo = request.ruolo;
            int pageNumber = request.pageNumber;
            bool excel = request.excel;
            int pageSize = request.pageSize;
            List<Trasmissione> result = new List<Trasmissione>();
            int totalPageNumber = 0;
            int recordCount = 0;

            try
            {
                FiltroRicerca documentOrFolder = listaFiltri.Where(e => e.argomento == "TIPO_OGGETTO").FirstOrDefault();

                switch (documentOrFolder.valore)
                {
                    case "D":
                        (result, totalPageNumber, recordCount) = await this.GetQueryEffettuateMethodPagingInternalLiteDoc(oggettoTrasmesso, utente, ruolo, listaFiltri, true, pageNumber, excel, pageSize);
                        break;
                    default:
                        (result, totalPageNumber, recordCount) = await this.GetQueryEffettuateMethodPagingInternalLiteFasc(oggettoTrasmesso, utente, ruolo, listaFiltri, true, pageNumber, excel, pageSize);
                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return new TrasmissioneGetQueryEffettuatePagingLiteResult(result.ToArray(), totalPageNumber, recordCount);


        }


        private async Task<(List<Trasmissione> result, int totalPageNumber, int recordCount)> GetQueryEffettuateMethodPagingInternalLiteDoc(OggettoTrasm objOggettoTrasmesso, Utente objUtente, Ruolo objRuolo, FiltroRicerca[] objListaFiltri, bool ricercaCondizioniVisibilitaUtente, int pageNumber, bool excel, int pageSize)
        {
            List<Trasmissione> lista = new List<Trasmissione>();
            int totalPageNumber = 0; int recordCount = 0;

            ExpressionStarter<JoinProfileEntity> predicate = PredicateBuilder.New<JoinProfileEntity>(); // PredicateBuilder.New<JoinProfileEntity>();
            var queryJoin = this._dbContext.TrasmissioneEntities
                .Join(this._dbContext.TrasmSingolaEntities, a => a.SYSTEM_ID, b => b.ID_TRASMISSIONE, (a, b) => new { a, b })
                .Join(this._dbContext.TrasmUtenteEntities, j1 => j1.b.SYSTEM_ID, c => c.ID_TRASM_SINGOLA, (j1, c) => new { a = j1.a, b = j1.b, c })
                .Join(this._dbContext.RagioneTrasmissioneEntities, j2 => j2.b.ID_RAGIONE, g => g.SYSTEM_ID, (j2, g) => new { a = j2.a, b = j2.b, c = j2.c, g })
                .Join(this._dbContext.ProfileEntities, j3 => j3.b.ID_RAGIONE, pr => pr.SYSTEM_ID, (j3, pr) => new JoinProfileEntity { a = j3.a, b = j3.b, c = j3.c, g = j3.g, pr = pr });


            if (objOggettoTrasmesso.infoDocumento != null && objOggettoTrasmesso.infoFascicolo != null)
            {
                var profileList = await this._dbContext.ProfileEntities.Where(x => x.CHA_IN_CESTINO == null || x.CHA_IN_CESTINO.Equals("0")).Select(x => x.SYSTEM_ID).ToListAsync();
                predicate = predicate.And(x => profileList.Contains((long)x.a.ID_PROFILE));
                predicate = predicate.And(x => x.a.ID_PROFILE == objOggettoTrasmesso.infoDocumento.idProfile.AsLong() || x.a.ID_PROJECT == objOggettoTrasmesso.infoFascicolo.idFascicolo.AsLong());
            }
            else if (objOggettoTrasmesso.infoDocumento != null)
            {
                var profileList = await this._dbContext.ProfileEntities.Where(x => x.CHA_IN_CESTINO == null || x.CHA_IN_CESTINO.Equals("0")).Select(x => x.SYSTEM_ID).ToListAsync();
                predicate = predicate.And(x => profileList.Contains((long)x.a.ID_PROFILE));
                predicate = predicate.And(x => x.a.ID_PROFILE == objOggettoTrasmesso.infoDocumento.idProfile.AsLong());
            }
            else if (objOggettoTrasmesso.infoFascicolo != null)
            {
                predicate = predicate.And(x => x.a.ID_PROJECT == objOggettoTrasmesso.infoFascicolo.idFascicolo.AsLong());
            }

            if (objUtente != null && objRuolo != null)
                predicate = await this.GetCondizioniVisibilitaEffettuate(predicate, objOggettoTrasmesso, objUtente, objRuolo, objListaFiltri);



            var entities = queryJoin
                .Where(predicate)
                .Select(x => new
                {
                    ID_RUOLO_IN_UO = x.a.ID_RUOLO_IN_UO,
                    ID_PEOPLE = x.a.ID_PEOPLE,
                    CHA_TIPO_OGGETTO = x.a.CHA_TIPO_OGGETTO,
                    ID_PROFILE = x.a.ID_PROFILE,
                    ID_PROJECT = x.a.ID_PROJECT,
                    DTA_INVIO_F = string.Empty,
                    VAR_NOTE_GENERALI = x.a.VAR_NOTE_GENERALI,
                    ID_RAGIONE = x.b.ID_RAGIONE,
                    ID_TRASMISSIONE = x.b.ID_TRASMISSIONE,
                    ID_TRASM_UTENTE = x.b.ID_TRASM_UTENTE,
                    CHA_TIPO_DEST = x.b.CHA_TIPO_DEST,
                    ID_CORR_GLOBALE = x.b.ID_CORR_GLOBALE,
                    HIDE_DOC_VERSIONS = x.b.HIDE_DOC_VERSIONS,
                    VAR_NOTE_SING = x.b.VAR_NOTE_SING,
                    CHA_TIPO_TRASM = x.b.CHA_TIPO_TRASM,
                    DTA_SCADENZA = string.Empty,
                    ID_TRASMISSIONE_UTENTE = x.c.SYSTEM_ID,
                    ID_DESTINATARIO = x.c.ID_PEOPLE,
                    DTA_VISTA = string.Empty,
                    CHA_VISTA = x.c.CHA_VISTA,
                    DTA_ACCETTATA = string.Empty,
                    CHA_ACCETTATA = x.c.CHA_ACCETTATA,
                    DTA_RIFIUTATA = string.Empty,
                    CHA_RIFIUTATA = x.c.CHA_RIFIUTATA,
                    VAR_NOTE_RIF = x.c.VAR_NOTE_RIF,
                    VAR_NOTE_ACC = x.c.VAR_NOTE_ACC,
                    ID_TRASM_SINGOLA = x.c.ID_TRASM_SINGOLA,
                    CHA_VALIDA = x.c.CHA_VALIDA,
                    DTA_RIMOSSA_TDL = x.c.DTA_RIMOZIONE_TODOLIST,
                    CHA_SALVATA_CON_CESSIONE = x.a.CHA_SALVATA_CON_CESSIONE,
                    ID_PEOPLE_DELEGATO = x.a.ID_PEOPLE_DELEGATO,
                    DELEGATO_UTENTE = x.c.ID_PEOPLE_DELEGATO,
                    ACCETTATA_DELEGATO = x.c.CHA_ACCETTATA_DELEGATO,
                    VISTA_DELEGATO = x.c.CHA_VISTA_DELEGATO,
                    RIFIUTATA_DELEGATO = x.c.CHA_RIFIUTATA_DELEGATO,
                    ID_SEGN_CODFASC = !string.IsNullOrWhiteSpace(IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "SEGNATURA_CODFASC")) ? IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "SEGNATURA_CODFASC") : IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "CODFASC"),
                    DATA_DOC_FASC = !string.IsNullOrWhiteSpace(IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "DATADOC")) ? IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "DATADOC") : IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "DATA_CREAZ"),
                    VAR_DESC_CORR = IPi3DbContextMappedFunctions.GetDescCorr((long)x.b.ID_CORR_GLOBALE),
                    FULL_NAME = IPi3DbContextMappedFunctions.GetPeopleName((long)x.a.ID_PEOPLE),
                    DESC_RUOLO_MITT = IPi3DbContextMappedFunctions.GetPeopleUserId((long)x.a.ID_RUOLO_IN_UO),
                    //MITTENTI_PROTO = IPi3DbContextMappedFunctions.CorrCatByTipo((long)s.p.SYSTEM_ID, s.p.CHA_TIPO_PROTO, "M"),
                    //DESTINATARI_PROTO = IPi3DbContextMappedFunctions.CorrCatByTipo((long)s.p.SYSTEM_ID, s.p.CHA_TIPO_PROTO, "D"),
                    //COUNTER_REPERTORY = IPi3DbContextMappedFunctions.GetSegnaturaRepertorio(s.p.SYSTEM_ID, idAmm)
                    CHA_TIPO_RAGIONE = x.g.CHA_TIPO_RAGIONE,
                    VAR_DESC_RAGIONE = x.g.VAR_DESC_RAGIONE,
                    CHA_EREDITA = x.g.CHA_EREDITA,
                    CHA_TIPO_DIRITTI = x.g.CHA_TIPO_DIRITTI,
                    DESC_DESTINATARIO = IPi3DbContextMappedFunctions.GetPeopleName((long)x.c.ID_PEOPLE),
                    COD_RUOLO_MITT = IPi3DbContextMappedFunctions.GetCodRuoloByIdCorr((long)x.a.ID_RUOLO_IN_UO),
                    USER_ID = IPi3DbContextMappedFunctions.GetPeopleUserId((long)x.a.ID_PEOPLE),
                    CHA_RISPOSTA = x.g.CHA_TIPO_RISPOSTA,
                    VAR_NOTIFICA_TRASM = x.g.VAR_NOTIFICA_TRASM,
                    VAR_TESTO_MSG_NOTIFICA_DOC = x.g.VAR_TESTO_MSG_NOTIFICA_DOC,
                    VAR_TESTO_MSG_NOTIFICA_FASC = x.g.VAR_TESTO_MSG_NOTIFICA_FASC,
                    CHA_CEDE_DIRITTI = x.g.CHA_CEDE_DIRITTI,
                    CHA_MANTIENI_LETT = x.g.CHA_MANTIENI_LETT,
                    MITTENTI_PROTO = IPi3DbContextMappedFunctions.CorrCatByTipo((long)x.pr.SYSTEM_ID, x.pr.CHA_TIPO_PROTO, "M"),
                    DESTINATARI_PROTO = IPi3DbContextMappedFunctions.CorrCatByTipo((long)x.pr.SYSTEM_ID, x.pr.CHA_TIPO_PROTO, "D"),
                    OGGETTO_PROTO = x.pr.VAR_PROF_OGGETTO,
                    DATA_PROTO = x.pr.DTA_PROTO.AsDateFormat(),
                    NUM_PROTO = x.pr.NUM_PROTO,
                    DOCNUMBER = x.pr.DOCNUMBER,
                    VAR_SEGNATURA = x.pr.VAR_SEGNATURA,
                    ID_REGISTRO = x.pr.ID_REGISTRO,
                    CHA_TIPO_PROTO = x.pr.CHA_TIPO_PROTO
                });


            return (lista, totalPageNumber, recordCount);
        }

        private async Task<ExpressionStarter<JoinProfileEntity>> GetCondizioniVisibilitaEffettuate(ExpressionStarter<JoinProfileEntity> predicate, OggettoTrasm objOggettoTrasmesso, Utente objUtente, Ruolo objRuolo, FiltroRicerca[] objListaFiltri)
        {
            bool cercaInferiori = objListaFiltri.Where(x => x.argomento.Equals("NO_CERCA_INFERIORI") && x.valore.ToUpper().Equals("TRUE")).FirstOrDefault() != null;
            string ruolo_sottoposto = string.Empty;
            string persona_sottoposta = string.Empty;
            bool al_mio_ruolo = false;
            String filterHistoricized = null;

            var idCorrispondente = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_PEOPLE == objUtente.idPeople.AsLong()).Select(x => x.SYSTEM_ID).ToListAsync();//getQueryEffMet1(objUtente.idPeople);

            DocsPaVO.filtri.FiltroRicerca f;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.argomento.Equals("RUOLO_SOTTOPOSTO"))
                    ruolo_sottoposto = f.valore;
                if (f.argomento.Equals("PERSONA_SOTTOPOSTA"))
                    persona_sottoposta = f.valore;
                if (f.argomento.Equals("AL_MIO_RUOLO"))
                    al_mio_ruolo = true;
            }

            FiltroRicerca extendToHist = objListaFiltri.Where(e => e.argomento == listaArgomentiNascosti.RUOLO_EXTEND_TO_HISTORICIZED.ToString()).FirstOrDefault();
            List<long> roleChain = new List<long> { ruolo_sottoposto.AsLong() };
            if (extendToHist != null && Convert.ToBoolean(extendToHist.valore))
                roleChain = await this.GetRoleChain(roleChain, ruolo_sottoposto.AsLong());

            if (persona_sottoposta.Equals("tutti")) //Tutti gli utenti del ruolo
                predicate = predicate.And(x => roleChain.Contains((long)x.a.ID_RUOLO_IN_UO));
            else
            {
                if (persona_sottoposta.Equals("altri")) //Solo gli altri utenti tranne me
                    predicate = predicate.And(x => x.a.ID_PEOPLE != objUtente.idPeople.AsLong());
                else //singolo utente selezionato
                    predicate = predicate.And(x => x.a.ID_PEOPLE == persona_sottoposta.AsLong());
            }

            if (!al_mio_ruolo)
            {
                List<long> ruoliInf = new List<long>();
                if (cercaInferiori)
                {
                    if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoDocumento != null)
                        ruoliInf = await this.GetGerarchiaInf(objRuolo, objOggettoTrasmesso.infoDocumento.idRegistro, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO);
                    else if (objOggettoTrasmesso != null && objOggettoTrasmesso.infoFascicolo != null)
                        ruoliInf = await this.GetGerarchiaInf(objRuolo, objOggettoTrasmesso.infoFascicolo.idRegistro, objOggettoTrasmesso.infoFascicolo.idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO);
                    else
                        ruoliInf = await this.GetGerarchiaInf(objRuolo, null, null, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO);
                }

                if (ruoliInf.Any())
                {
                    predicate = predicate.And(x => ruoliInf.Contains((long)x.a.ID_RUOLO_IN_UO));
                }
                else
                {
                    //Per non trovare niente se non ho ruoli sottoposti e si seleziona il flag
                    predicate = predicate.And(x => x.a.SYSTEM_ID == null);
                    //queryWhere += " a.system_id is null ";
                }
            }
            return predicate;
        }


        private async Task<List<long>> GetGerarchiaInf(Ruolo objRuolo, string idRegistro, string idNodoTitolario, TipoOggetto tipoOggetto)
        {
            List<Ruolo> toReturn = new List<Ruolo>();
            var uoInf = await this._dbContext.CorrGlobaliEntities
                .Where(x => x.CHA_TIPO_URP.Equals("U") && x.NUM_LIVELLO >= objRuolo.uo.livello.AsLong())
                .Select(x => new UoInfo
                {
                    SYSTEM_ID = x.SYSTEM_ID,
                    ID_PARENT = x.ID_PARENT
                })
                .ToListAsync();

            List<long> childrenUO = new List<long>() { };
            childrenUO = await this.GetChildrenUo(objRuolo.uo.systemId.AsLong(), uoInf, childrenUO);

            childrenUO.Add(objRuolo.uo.systemId.AsLong());

            var queryBase = this._dbContext.CorrGlobaliEntities
                .Join(this._dbContext.TipoRuoloEntities, a => a.ID_TIPO_RUOLO, b => b.SYSTEM_ID, (a, b) => new { a, b });

            (string? noFiltroAooKey, bool foundKey1) = await this._configurationService.TryGetValue<string>("NO_FILTRO_AOO");
            (string? estSupPariLivKey, bool foundKey2) = await this._configurationService.TryGetValue<string>("EST_VIS_SUP_PARI_LIV");

            bool isFiltroAooEnabled = noFiltroAooKey != null && noFiltroAooKey.Equals("1");
            bool estSupPariLiv = estSupPariLivKey != null && estSupPariLivKey.Equals("1");
            if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                if (idRegistro != null && idRegistro != "" && !isFiltroAooEnabled)
                {
                    var predicate = PredicateBuilder.New<JoinCorrGlobaliTipoRuoloLRuoloRegEntity>();
                    var query = queryBase.Join(this._dbContext.RuoloRegistroEntities, q1 => q1.a.SYSTEM_ID, c => c.ID_RUOLO_IN_UO, (q1, c) => new JoinCorrGlobaliTipoRuoloLRuoloRegEntity { a = q1.a, b = q1.b, c = c });
                    predicate = predicate.And(x => x.c.ID_REGISTRO == idRegistro.AsLong());
                    if (objRuolo.idAmministrazione != null && !objRuolo.idAmministrazione.ToString().Equals(""))
                        predicate = predicate.And(x => x.a.ID_AMM == objRuolo.idAmministrazione.AsLong());
                    if (!estSupPariLiv && childrenUO != null && childrenUO.Count > 0)
                        predicate = predicate.And(x => childrenUO.Contains((long)x.a.ID_UO));
                    else
                    {
                        if (childrenUO != null && childrenUO.Count > 1)
                        {
                            predicate = predicate.And(x => childrenUO.Contains((long)x.a.ID_UO));
                            predicate = predicate.And(x => x.b.NUM_LIVELLO > objRuolo.livello.AsLong());
                            predicate = predicate.Or(x => x.a.ID_UO == objRuolo.uo.systemId.AsLong() && x.b.NUM_LIVELLO > objRuolo.livello.AsLong());
                        }
                        else
                        {
                            predicate = predicate.And(x => x.a.ID_UO == objRuolo.uo.systemId.AsLong());
                        }
                    }
                    var entities = query.Where(predicate).Select(x => new
                    {
                        SYSTEM_ID = x.a.SYSTEM_ID,
                        ID_GRUPPO = x.a.ID_GRUPPO,
                        NUM_LIVELLO = x.b.NUM_LIVELLO,
                        VAR_DESC_RUOLO = x.a.VAR_DESC_CORR,
                        VAR_CODICE = x.a.VAR_CODICE,
                        VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                        ID_PARENT = x.a.ID_PARENT,
                        ID_UO = x.a.ID_UO
                    });
                    foreach (var entity in entities)
                    {
                        Ruolo r = new Ruolo()
                        {
                            systemId = entity.SYSTEM_ID.ToString(),
                            descrizione = entity.VAR_DESC_RUOLO,
                            codiceCorrispondente = entity.VAR_CODICE,
                            codiceRubrica = entity.VAR_COD_RUBRICA,
                            idGruppo = entity.ID_GRUPPO.ToString()
                        };
                        toReturn.Add(r);
                    }
                }
            }
            else
            {
                if (idRegistro != null && idRegistro != "")
                {
                    var predicate = PredicateBuilder.New<JoinCorrGlobaliTipoRuoloSecurityLRuoloRegEntity>();
                    var query = queryBase.Join(this._dbContext.SecurityEntities, q1 => q1.a.ID_GRUPPO, c => c.PERSONORGROUP, (q1, c) => new JoinCorrGlobaliTipoRuoloSecurityEntity { a = q1.a, b = q1.b, c = c })
                        .Join(this._dbContext.RuoloRegistroEntities, q1 => q1.a.SYSTEM_ID, d => d.ID_RUOLO_IN_UO, (q1, d) => new JoinCorrGlobaliTipoRuoloSecurityLRuoloRegEntity { a = q1.a, b = q1.b, c = q1.c, d = d });
                    predicate = predicate.And(x => x.c.THING == idNodoTitolario.AsLong() && x.c.ACCESSRIGHTS > 0);
                    predicate = predicate.And(x => x.d.ID_REGISTRO == idRegistro.AsLong());
                    if (objRuolo.idAmministrazione != null && !objRuolo.idAmministrazione.ToString().Equals(""))
                        predicate = predicate.And(x => x.a.ID_AMM == objRuolo.idAmministrazione.AsLong());
                    if (!estSupPariLiv && childrenUO != null && childrenUO.Count > 0)
                        predicate = predicate.And(x => childrenUO.Contains((long)x.a.ID_UO));
                    else
                    {
                        if (childrenUO != null && childrenUO.Count > 1)
                        {
                            predicate = predicate.And(x => childrenUO.Contains((long)x.a.ID_UO));
                            predicate = predicate.And(x => x.b.NUM_LIVELLO > objRuolo.livello.AsLong());
                            predicate = predicate.Or(x => x.a.ID_UO == objRuolo.uo.systemId.AsLong() && x.b.NUM_LIVELLO > objRuolo.livello.AsLong());
                        }
                        else
                        {
                            predicate = predicate.And(x => x.a.ID_UO == objRuolo.uo.systemId.AsLong());
                        }
                    }
                    var entities = query.Where(predicate).Select(x => new
                    {
                        SYSTEM_ID = x.a.SYSTEM_ID,
                        ID_GRUPPO = x.a.ID_GRUPPO,
                        NUM_LIVELLO = x.b.NUM_LIVELLO,
                        VAR_DESC_RUOLO = x.a.VAR_DESC_CORR,
                        VAR_CODICE = x.a.VAR_CODICE,
                        VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                        ID_PARENT = x.a.ID_PARENT,
                        ID_UO = x.a.ID_UO
                    });
                    foreach (var entity in entities)
                    {
                        Ruolo r = new Ruolo()
                        {
                            systemId = entity.SYSTEM_ID.ToString(),
                            descrizione = entity.VAR_DESC_RUOLO,
                            codiceCorrispondente = entity.VAR_CODICE,
                            codiceRubrica = entity.VAR_COD_RUBRICA,
                            idGruppo = entity.ID_GRUPPO.ToString()
                        };
                        toReturn.Add(r);
                    }
                }
                else
                {
                    var predicate = PredicateBuilder.New<JoinCorrGlobaliTipoRuoloSecurityEntity>();
                    var query = queryBase.Join(this._dbContext.SecurityEntities, q1 => q1.a.ID_GRUPPO, c => c.PERSONORGROUP, (q1, c) => new JoinCorrGlobaliTipoRuoloSecurityEntity { a = q1.a, b = q1.b, c = c });
                    predicate = predicate.And(x => x.c.THING == idNodoTitolario.AsLong() && x.c.ACCESSRIGHTS > 0);
                    if (objRuolo.idAmministrazione != null && !objRuolo.idAmministrazione.ToString().Equals(""))
                        predicate = predicate.And(x => x.a.ID_AMM == objRuolo.idAmministrazione.AsLong());
                    if (!estSupPariLiv && childrenUO != null && childrenUO.Count > 0)
                        predicate = predicate.And(x => childrenUO.Contains((long)x.a.ID_UO));
                    else
                    {
                        if (childrenUO != null && childrenUO.Count > 1)
                        {
                            predicate = predicate.And(x => childrenUO.Contains((long)x.a.ID_UO));
                            predicate = predicate.And(x => x.b.NUM_LIVELLO > objRuolo.livello.AsLong());
                            predicate = predicate.Or(x => x.a.ID_UO == objRuolo.uo.systemId.AsLong() && x.b.NUM_LIVELLO > objRuolo.livello.AsLong());
                        }
                        else
                        {
                            predicate = predicate.And(x => x.a.ID_UO == objRuolo.uo.systemId.AsLong());
                        }
                    }
                    var entities = query.Where(predicate).Select(x => new
                    {
                        SYSTEM_ID = x.a.SYSTEM_ID,
                        ID_GRUPPO = x.a.ID_GRUPPO,
                        NUM_LIVELLO = x.b.NUM_LIVELLO,
                        VAR_DESC_RUOLO = x.a.VAR_DESC_CORR,
                        VAR_CODICE = x.a.VAR_CODICE,
                        VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                        ID_PARENT = x.a.ID_PARENT,
                        ID_UO = x.a.ID_UO
                    });
                    foreach (var entity in entities)
                    {
                        Ruolo r = new Ruolo()
                        {
                            systemId = entity.SYSTEM_ID.ToString(),
                            descrizione = entity.VAR_DESC_RUOLO,
                            codiceCorrispondente = entity.VAR_CODICE,
                            codiceRubrica = entity.VAR_COD_RUBRICA,
                            idGruppo = entity.ID_GRUPPO.ToString()
                        };
                        toReturn.Add(r);
                    }
                }
            }

            return toReturn.Select(x => x.systemId.AsLong()).ToList();
        }

        private async Task<List<long>> GetChildrenUo(long idUo, List<UoInfo> uoInf, List<long> childrenUO)
        {
            List<UoInfo> childrenUoNewList = uoInf.Where(x => x.ID_PARENT == idUo).ToList();
            if (childrenUoNewList != null)
            {
                foreach (UoInfo childUoNew in childrenUoNewList)
                {
                    childrenUO.Add(childUoNew.SYSTEM_ID);
                    childrenUO = await GetChildrenUo(childUoNew.SYSTEM_ID, uoInf, childrenUO);
                }
            }

            return childrenUO;
        }

        private async Task<List<long>> GetRoleChain(List<long> roleChain, long oldRuoloSottoposto)
        {
            var newRuoloSottoposto = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_OLD == oldRuoloSottoposto).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
            if (newRuoloSottoposto != null)
            {
                roleChain.Add(newRuoloSottoposto);
                return await GetRoleChain(roleChain, newRuoloSottoposto);
            }
            else
                return roleChain;
        }

        private async Task<(List<Trasmissione> result, int totalPageNumber, int recordCount)> GetQueryEffettuateMethodPagingInternalLiteFasc(OggettoTrasm objOggettoTrasmesso, Utente objUtente, Ruolo objRuolo, FiltroRicerca[] objListaFiltri, bool ricercaCondizioniVisibilitaUtente, int pageNumber, bool excel, int pageSize)
        {
            List<Trasmissione> lista = new List<Trasmissione>();
            int totalPageNumber = 0; int recordCount = 0;

            var predicate = PredicateBuilder.New<JoinProjectEntity>();
            var queryJoin = this._dbContext.TrasmissioneEntities
                .Join(this._dbContext.TrasmSingolaEntities, a => a.SYSTEM_ID, b => b.ID_TRASMISSIONE, (a, b) => new { a, b })
                .Join(this._dbContext.TrasmUtenteEntities, j1 => j1.b.SYSTEM_ID, c => c.ID_TRASM_SINGOLA, (j1, c) => new { a = j1.a, b = j1.b, c })
                .Join(this._dbContext.RagioneTrasmissioneEntities, j2 => j2.b.ID_RAGIONE, g => g.SYSTEM_ID, (j2, g) => new { a = j2.a, b = j2.b, c = j2.c, g })
                .Join(this._dbContext.ProjectEntities, j3 => j3.b.ID_RAGIONE, pj => pj.SYSTEM_ID, (j3, pj) => new JoinProjectEntity { a = j3.a, b = j3.b, c = j3.c, g = j3.g, pj = pj });

            // condizione sui documenti
            if (objOggettoTrasmesso.infoFascicolo != null)
                predicate = predicate.And(x => x.a.ID_PROFILE == objOggettoTrasmesso.infoDocumento.idProfile.AsLong());

            var entities = queryJoin
                .Where(predicate)
                .Select(x => new
                {
                    ID_RUOLO_IN_UO = x.a.ID_RUOLO_IN_UO,
                    ID_PEOPLE = x.a.ID_PEOPLE,
                    CHA_TIPO_OGGETTO = x.a.CHA_TIPO_OGGETTO,
                    ID_PROFILE = x.a.ID_PROFILE,
                    ID_PROJECT = x.a.ID_PROJECT,
                    DTA_INVIO_F = string.Empty,
                    VAR_NOTE_GENERALI = x.a.VAR_NOTE_GENERALI,
                    ID_RAGIONE = x.b.ID_RAGIONE,
                    ID_TRASMISSIONE = x.b.ID_TRASMISSIONE,
                    ID_TRASM_UTENTE = x.b.ID_TRASM_UTENTE,
                    CHA_TIPO_DEST = x.b.CHA_TIPO_DEST,
                    ID_CORR_GLOBALE = x.b.ID_CORR_GLOBALE,
                    HIDE_DOC_VERSIONS = x.b.HIDE_DOC_VERSIONS,
                    VAR_NOTE_SING = x.b.VAR_NOTE_SING,
                    CHA_TIPO_TRASM = x.b.CHA_TIPO_TRASM,
                    DTA_SCADENZA = string.Empty,
                    ID_TRASMISSIONE_UTENTE = x.c.SYSTEM_ID,
                    ID_DESTINATARIO = x.c.ID_PEOPLE,
                    DTA_VISTA = string.Empty,
                    CHA_VISTA = x.c.CHA_VISTA,
                    DTA_ACCETTATA = string.Empty,
                    CHA_ACCETTATA = x.c.CHA_ACCETTATA,
                    DTA_RIFIUTATA = string.Empty,
                    CHA_RIFIUTATA = x.c.CHA_RIFIUTATA,
                    VAR_NOTE_RIF = x.c.VAR_NOTE_RIF,
                    VAR_NOTE_ACC = x.c.VAR_NOTE_ACC,
                    ID_TRASM_SINGOLA = x.c.ID_TRASM_SINGOLA,
                    CHA_VALIDA = x.c.CHA_VALIDA,
                    DTA_RIMOSSA_TDL = x.c.DTA_RIMOZIONE_TODOLIST,
                    CHA_SALVATA_CON_CESSIONE = x.a.CHA_SALVATA_CON_CESSIONE,
                    ID_PEOPLE_DELEGATO = x.a.ID_PEOPLE_DELEGATO,
                    DELEGATO_UTENTE = x.c.ID_PEOPLE_DELEGATO,
                    ACCETTATA_DELEGATO = x.c.CHA_ACCETTATA_DELEGATO,
                    VISTA_DELEGATO = x.c.CHA_VISTA_DELEGATO,
                    RIFIUTATA_DELEGATO = x.c.CHA_RIFIUTATA_DELEGATO,
                    ID_SEGN_CODFASC = !string.IsNullOrWhiteSpace(IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "SEGNATURA_CODFASC")) ? IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "SEGNATURA_CODFASC") : IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "CODFASC"),
                    DATA_DOC_FASC = !string.IsNullOrWhiteSpace(IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "DATADOC")) ? IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "DATADOC") : IPi3DbContextMappedFunctions.VarDescribe((long)x.a.ID_PROFILE, "DATA_CREAZ"),
                    VAR_DESC_CORR = IPi3DbContextMappedFunctions.GetDescCorr((long)x.b.ID_CORR_GLOBALE),
                    FULL_NAME = IPi3DbContextMappedFunctions.GetPeopleName((long)x.a.ID_PEOPLE),
                    DESC_RUOLO_MITT = IPi3DbContextMappedFunctions.GetPeopleUserId((long)x.a.ID_RUOLO_IN_UO),
                    //MITTENTI_PROTO = IPi3DbContextMappedFunctions.CorrCatByTipo((long)s.p.SYSTEM_ID, s.p.CHA_TIPO_PROTO, "M"),
                    //DESTINATARI_PROTO = IPi3DbContextMappedFunctions.CorrCatByTipo((long)s.p.SYSTEM_ID, s.p.CHA_TIPO_PROTO, "D"),
                    //COUNTER_REPERTORY = IPi3DbContextMappedFunctions.GetSegnaturaRepertorio(s.p.SYSTEM_ID, idAmm)
                    CHA_TIPO_RAGIONE = x.g.CHA_TIPO_RAGIONE,
                    VAR_DESC_RAGIONE = x.g.VAR_DESC_RAGIONE,
                    CHA_EREDITA = x.g.CHA_EREDITA,
                    CHA_TIPO_DIRITTI = x.g.CHA_TIPO_DIRITTI,
                    DESC_DESTINATARIO = IPi3DbContextMappedFunctions.GetPeopleName((long)x.c.ID_PEOPLE),
                    COD_RUOLO_MITT = IPi3DbContextMappedFunctions.GetCodRuoloByIdCorr((long)x.a.ID_RUOLO_IN_UO),
                    USER_ID = IPi3DbContextMappedFunctions.GetPeopleUserId((long)x.a.ID_PEOPLE),
                    CHA_RISPOSTA = x.g.CHA_TIPO_RISPOSTA,
                    VAR_NOTIFICA_TRASM = x.g.VAR_NOTIFICA_TRASM,
                    VAR_TESTO_MSG_NOTIFICA_DOC = x.g.VAR_TESTO_MSG_NOTIFICA_DOC,
                    VAR_TESTO_MSG_NOTIFICA_FASC = x.g.VAR_TESTO_MSG_NOTIFICA_FASC,
                    CHA_CEDE_DIRITTI = x.g.CHA_CEDE_DIRITTI,
                    CHA_MANTIENI_LETT = x.g.CHA_MANTIENI_LETT,
                    VAR_CODICE = x.pj.VAR_CODICE,
                    DESCRIPTION = x.pj.DESCRIPTION,
                    DTA_APERTURA = x.pj.DTA_APERTURA.AsDateFormat(),
                    ID_REGISTRO = x.pj.ID_REGISTRO
                });


            return (lista, totalPageNumber, recordCount);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneGetQueryEffettuatePagingLiteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }

    internal class JoinCorrGlobaliTipoRuoloSecurityEntity
    {
        public CorrGlobaliEntity a { get; set; }
        public TipoRuoloEntity b { get; set; }
        public SecurityEntity c { get; set; }
    }

    internal class JoinCorrGlobaliTipoRuoloSecurityLRuoloRegEntity
    {
        public CorrGlobaliEntity a { get; set; }
        public TipoRuoloEntity b { get; set; }
        public SecurityEntity c { get; set; }
        public RuoloRegistroEntity d { get; set; }
    }

    internal class JoinCorrGlobaliTipoRuoloLRuoloRegEntity
    {
        public CorrGlobaliEntity a { get; set; }
        public TipoRuoloEntity b { get; set; }
        public RuoloRegistroEntity c { get; set; }
    }

    internal class JoinCorrGlobaliTipoRuoloEntity
    {
        public CorrGlobaliEntity a { get; set; }
        public TipoRuoloEntity b { get; set; }
    }

    internal class JoinProfileEntity
    {
        public TrasmissioneEntity a { get; set; }
        public TrasmSingolaEntity b { get; set; }
        public TrasmUtenteEntity c { get; set; }
        public RagioneTrasmissioneEntity g { get; set; }
        public ProfileEntity pr { get; set; }
    }

    internal class JoinProjectEntity
    {
        public TrasmissioneEntity a { get; set; }
        public TrasmSingolaEntity b { get; set; }
        public TrasmUtenteEntity c { get; set; }
        public RagioneTrasmissioneEntity g { get; set; }
        public ProjectEntity pj { get; set; }
    }

    internal class UoInfo
    {
        public long SYSTEM_ID { get; set; }
        public long? ID_PARENT { get; set; }
    }
}