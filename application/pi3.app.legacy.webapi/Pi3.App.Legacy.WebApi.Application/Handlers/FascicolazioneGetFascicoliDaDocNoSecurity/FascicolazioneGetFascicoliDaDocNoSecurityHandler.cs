// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO;
using DocsPaVO.fascicolazione;
using DocsPaVO.Note;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFascicoliDaDocNoSecurity
{
    // Richiede libreria MediatR
    public class FascicolazioneGetFascicoliDaDocNoSecurityHandler : IRequestHandler<Application.Requests.FascicolazioneGetFascicoliDaDocNoSecurity, FascicolazioneGetFascicoliDaDocNoSecurityResult>
    {
        #region Public Members

        public FascicolazioneGetFascicoliDaDocNoSecurityHandler(ILogger<FascicolazioneGetFascicoliDaDocNoSecurityHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<FascicolazioneGetFascicoliDaDocNoSecurityResult> Handle(Application.Requests.FascicolazioneGetFascicoliDaDocNoSecurity request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.fascicolazione.Fascicolo> result = new List<DocsPaVO.fascicolazione.Fascicolo>();
            string idProfile = request.idProfile;
            long idProfileAsLong = idProfile.AsLong();
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idUserAsString = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
            var idGroupAsString = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            bool extAppControlEnabled = _dbContext.ChiaviConfigurazioneEntities.Where(a => a.VAR_CODICE.Equals("ENABLE_GEST_EXT_APPS") && a.ID_AMM == 0).Select(a => a.VAR_VALORE).FirstOrDefault().Equals("1"); 

            try
            {
                var idCorrGlobaliRuolo = this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstOrDefault();
                if (idCorrGlobaliRuolo == null)
                    throw new CorrGlobaliByGroupIdNotFoundPi3Exception(idGroup);

                var idFascicoloList = this._dbContext.
                        ProjectComponentEntities.Where(x => x.LINK == idProfileAsLong)
                        .Join(this._dbContext.ProjectEntities, pc => pc.PROJECT_ID, p => p.SYSTEM_ID, (pc, p) => p.ID_FASCICOLO)
                        .ToList();

                var fascicoloList = this._dbContext
                    .ProjectEntities
                    .Where(x => x.CHA_TIPO_PROJ.Equals("F") && idFascicoloList.Contains(x.SYSTEM_ID))
                    .Select(x => new
                    {
                        SYSTEM_ID = x.SYSTEM_ID,
                        DESCRIPTION = x.DESCRIPTION,
                        CHA_TIPO_PROJ = x.CHA_TIPO_PROJ,
                        VAR_CODICE = x.VAR_CODICE,
                        ID_AMM = x.ID_AMM,
                        NUM_LIVELLO = x.NUM_LIVELLO,
                        CHA_TIPO_FASCICOLO = x.CHA_TIPO_FASCICOLO,
                        ID_FASCICOLO = x.ID_FASCICOLO,
                        ID_PARENT = x.ID_PARENT,
                        VAR_COD_ULTIMO = x.VAR_COD_ULTIMO,
                        SICUREZZA = _dbContext.HasSecurityRights(x.SYSTEM_ID.ToString(), idUserAsString, idGroupAsString).Result ? "1" : "0",
                        DTA_APERTURA = x.DTA_APERTURA,
                        DTA_CHIUSURA = x.DTA_CHIUSURA,
                        CHA_STATO = x.CHA_STATO,
                        ID_TIPO_PROC = x.ID_TIPO_PROC,
                        VAR_NOTE = x.VAR_NOTE,
                        ID_REGISTRO = x.ID_REGISTRO,
                        ACCESSRIGHTS = _dbContext.GetSecurity(x.SYSTEM_ID.ToString(), idUserAsString, idGroupAsString).Result.ACCESSRIGHTS,
                        CARTACEO = x.CARTACEO,
                        ID_TITOLARIO = x.ID_TITOLARIO,
                        DTA_SCADENZA = x.DTA_SCADENZA,
                        NUM_FASCICOLO = x.NUM_FASCICOLO,
                        AUTHOR = x.AUTHOR,
                        ID_RUOLO_CREATORE = x.ID_RUOLO_CREATORE,
                        ID_UO_CREATORE = x.ID_UO_CREATORE,
                        CHA_CONSENTI_CLASS = x.CHA_CONSENTI_CLASS,
                        CHA_CONSENTI_FASC = x.CHA_CONSENTI_FASC,
                        COD_EXT_APP = x.COD_EXT_APP,
                        ID_PIANO_CONSERVAZIONE = x.ID_PIANO_CONSERVAZIONE
                    })
                    .AsNoTracking()
                    .ToList()
                    .OrderByDescending(x => x.SICUREZZA)
                    .ThenBy(x => x.SYSTEM_ID);


                foreach (var f in fascicoloList)
                {
                    DocsPaVO.fascicolazione.Fascicolo fasc = new DocsPaVO.fascicolazione.Fascicolo()
                    {
                        systemID = f.SYSTEM_ID.ToString(),
                        apertura = f.DTA_APERTURA.ToString(),
                        chiusura = f.DTA_CHIUSURA.ToString(),
                        codice = f.VAR_CODICE,
                        descrizione = f.DESCRIPTION,
                        stato = f.CHA_STATO,
                        tipo = f.CHA_TIPO_FASCICOLO,
                        idClassificazione = f.ID_PARENT.ToString(),
                        codUltimo = f.VAR_COD_ULTIMO,
                        idRegistroNodoTit = f.ID_REGISTRO.ToString(),
                        idTitolario = f.ID_TITOLARIO.ToString(),
                        accessRights = f.ACCESSRIGHTS?.ToString(),
                        cartaceo = Convert.ToInt32(f.CARTACEO) > 0,
                        controllato = "0",
                        isFascConsentita = f.CHA_CONSENTI_CLASS,
                        isFascicolazioneConsentita = f.CHA_CONSENTI_FASC != "0",
                        dtaScadenza = f.DTA_SCADENZA.AsDateFormat(),
                        numFascicolo = f.NUM_FASCICOLO.ToString(),
                        sicurezzaUtente = f.SICUREZZA.ToString(),
                        codiceApplicazione = f.COD_EXT_APP,
                    };

                    fasc.isFascPrimaria = GetFascPrimaria(idProfileAsLong, fasc.systemID.AsLong());

                    fasc.codiceRegistroNodoTit = !string.IsNullOrEmpty(fasc.idRegistroNodoTit) ? GetCodiceRegistroBySystemId(fasc.idRegistroNodoTit) : string.Empty;

                    fasc.noteFascicolo = FetchNoteFascicolo(idUserAsString.AsLong(), idGroupAsString.AsLong(), idCorrGlobaliRuolo, fasc);

                    fasc.creatoreFascicolo = new DocsPaVO.fascicolazione.CreatoreFascicolo()
                    {
                        idPeople = f.AUTHOR.ToString(),
                        idCorrGlob_Ruolo = f.ID_RUOLO_CREATORE.ToString(),
                        idCorrGlob_UO = f.ID_UO_CREATORE.ToString()
                    };

                    //if (string.Compare(fasc.creatoreFascicolo.idCorrGlob_UO.Trim(), string.Empty, true) != 0)
                    if (!string.IsNullOrEmpty(fasc.creatoreFascicolo.idCorrGlob_UO.Trim()))
                    {
                        fasc.creatoreFascicolo.uo_codiceCorrGlobali = GetUoById(fasc.creatoreFascicolo.idCorrGlob_UO); //recupera il nome della UO
                    }

                    //In area lavoro
                    fasc.InAreaLavoro = await IsProjectInADLUtente(fasc.systemID.AsLong(), idUser, idCorrGlobaliRuolo);

                    if (extAppControlEnabled && (!string.IsNullOrEmpty(fasc.codiceApplicazione)) && !string.IsNullOrEmpty(request.infoUtente.codWorkingApplication))
                    {
                        fasc.accessRights = (string.Compare(fasc.codiceApplicazione, request.infoUtente.codWorkingApplication) == 0 ? fasc.accessRights : "45");
                    }

                    if (f.ID_PIANO_CONSERVAZIONE != null)
                    {
                        fasc.pianoConservazione = await GetPianoConservazioneById(f.ID_PIANO_CONSERVAZIONE);
                    }

                    result.Add(fasc);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }
            return new FascicolazioneGetFascicoliDaDocNoSecurityResult(result.ToArray());
        }

        private async Task<string> IsProjectInADLUtente(long systemId, long idPeople, long idCorrGlobali)
        {
            return this._dbContext.AreaLavoroEntities.Any(x => x.ID_PROJECT == systemId &&
            x.ID_PEOPLE == idPeople &&
            x.ID_RUOLO_IN_UO == idCorrGlobali) ? "1" : "0";
        }

        private string GetUoById(string idCorrGlob_UO)
        {
            return this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == idCorrGlob_UO.AsLong()).Select(x => x.VAR_CODICE).FirstOrDefault();
        }

        private InfoNota[] FetchNoteFascicolo(long idUser, long idGroup, long idCorrGlobaliRuolo, DocsPaVO.fascicolazione.Fascicolo fasc)
        {
            List<InfoNota> note = new List<InfoNota>();

            var idRfAssociato = this._dbContext.RuoloRegistroEntities.Where(x => x.ID_RUOLO_IN_UO != null && x.ID_RUOLO_IN_UO == idCorrGlobaliRuolo).Select(x => x.ID_REGISTRO).FirstOrDefault();

            // Reperimento ultima nota visibile per il documento
            var q1 = this._dbContext.NoteEntities.Join(this._dbContext.PeopleEntities, n => n.IDUTENTECREATORE, p => p.SYSTEM_ID, (n, p) => new { n, p });
            var q2 = q1.Join(_dbContext.GroupEntities, q1 => q1.n.IDRUOLOCREATORE, g => g.SYSTEM_ID, (q1, g) => new { q1.n, q1.p, g });
            
            var listaNoteResult = q2
                .Where(x => x.n.TIPOOGGETTOASSOCIATO.Equals("F") &&
                x.n.IDOGGETTOASSOCIATO == fasc.systemID.AsLong() &&
                (x.n.TIPOVISIBILITA.Equals("T") ||
                x.n.TIPOVISIBILITA.Equals("F") && x.n.IDRFASSOCIATO == idRfAssociato ||
                x.n.TIPOVISIBILITA.Equals("P") && x.n.IDUTENTECREATORE == idUser ||
                x.n.TIPOVISIBILITA.Equals("R") && x.n.IDRUOLOCREATORE == idGroup)
                )
                .ToList()
                .OrderByDescending(x => x.n.DATACREAZIONE);

            foreach (var nota in listaNoteResult)
            {

                InfoNota item = new InfoNota()
                {
                    Id = nota.n.SYSTEM_ID.ToString(),
                    Testo = nota.n.TESTO,
                    DataCreazione = nota.n.DATACREAZIONE,
                    TipoVisibilita = GetTipoVisibilita(nota.n.TIPOVISIBILITA),
                    SolaLettura = !(nota.n.IDUTENTECREATORE.Equals(idUser)),
                    IdRfAssociato = nota.n.IDRFASSOCIATO.ToString(),
                    IdPeopleDelegato = nota.n.IDPEOPLEDELEGATO.ToString() ?? string.Empty,
                };

                item.UtenteCreatore = new InfoUtenteCreatoreNota()
                {
                    IdUtente = nota.n.IDUTENTECREATORE.ToString(),
                    DescrizioneUtente = nota.p.USER_ID,
                    IdRuolo = nota.n.IDRUOLOCREATORE.ToString(),
                    DescrizioneRuolo = nota.g.GROUP_NAME
                };

                item.DescrPeopleDelegato = !string.IsNullOrEmpty(item.IdPeopleDelegato) && !item.IdPeopleDelegato.Equals("0") ? GetDescrizioneUtente(item.IdPeopleDelegato) : string.Empty;

                note.Add(item);
            }

            return note.ToArray();
        }

        private string GetDescrizioneUtente(string idPeopleDelegato)
        {
            var user = this._dbContext.PeopleEntities.Where(x => x.SYSTEM_ID == idPeopleDelegato.AsLong()).Select(x => new { NOME = x.VAR_NOME, COGNOME = x.VAR_COGNOME }).FirstOrDefault();
            return string.Concat(user.COGNOME, " ", user.NOME);
        }

        private TipiVisibilitaNotaEnum GetTipoVisibilita(string visibilita)
        {
            if (visibilita.Equals("T"))
                return TipiVisibilitaNotaEnum.Tutti;
            else if (visibilita.Equals("F"))
                return TipiVisibilitaNotaEnum.RF;
            else if (visibilita.Equals("R"))
                return TipiVisibilitaNotaEnum.Ruolo;
            else if (visibilita.Equals("P"))
                return TipiVisibilitaNotaEnum.Personale;
            else
                return TipiVisibilitaNotaEnum.Tutti;
        }

        private string GetCodiceRegistroBySystemId(string idRegistroNodoTit)
        {
            return this._dbContext.RegistroEntities.Where(x => x.SYSTEM_ID == idRegistroNodoTit.AsLong()).Select(x => x.VAR_CODICE).FirstOrDefault();
        }

        private string GetFascPrimaria(long idProfile, long idFascicolo)
        {
            return _dbContext.ProjectComponentEntities.Join(_dbContext.ProjectEntities, pc => pc.PROJECT_ID, p => p.SYSTEM_ID, (pc, p) => new { pc, p })
                    .Any(x => x.pc.LINK == idProfile && x.pc.CHA_FASC_PRIMARIA.Equals("1") && x.p.ID_FASCICOLO == idFascicolo) ? "1" : "0";
        }

        private async Task<PianoConservazione> GetPianoConservazioneById(long? idPianoConservazione)
        {
            return await this._dbContext.PianoConservazioneEntities.Where(x => x.SYSTEM_ID == idPianoConservazione)
                .Select(x => new PianoConservazione()
                {
                    SystemId = x.SYSTEM_ID.ToString(),
                    IdClassificazione = x.ID_CLASSIFICAZIONE != null ? x.ID_CLASSIFICAZIONE.ToString() : string.Empty,
                    CodiceClassificazione = x.CODICE_CLASSIFICAZIONE,
                    NumeroProcedimento = x.NUMERO_PROCEDIMENTO,
                    TipologiaFascicolo = x.TIPOLOGIA_FASCICOLO,
                    TempoConservazione = x.TEMPO_CONSERVAZIONE,
                    VoceProcedimento = x.VOCE_PROCEDIMENTO,
                    NoteChiusuraFascicolo = x.NOTE_CHIUSURA_FASCICOLO,
                    NoteScartabilitaDocumenti = x.NOTE_SCARTABILITA_DOC,
                    NoteDocumenti = x.NOTE_DOCUMENTI
                })
                .FirstOrDefaultAsync();
        }
        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetFascicoliDaDocNoSecurityHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;



        #endregion
    }

}
