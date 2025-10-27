// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO;
using DocsPaVO.fascicolazione;
using DocsPaVO.Note;
using DocsPaVO.ProspettiRiepilogativi;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFascicoloInClassifica
{
    // Richiede libreria MediatR
    public class FascicolazioneGetFascicoloInClassificaHandler : IRequestHandler<Application.Requests.FascicolazioneGetFascicoloInClassifica, FascicolazioneGetFascicoloInClassificaResult>
    {
        #region Public Members

        public FascicolazioneGetFascicoloInClassificaHandler(ILogger<FascicolazioneGetFascicoloInClassificaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<FascicolazioneGetFascicoloInClassificaResult> Handle(Application.Requests.FascicolazioneGetFascicoloInClassifica request, CancellationToken cancellationToken)
        {
            DocsPaVO.fascicolazione.Fascicolo result = null;
            string codiceFascicolo = request.codiceFascicolo;
            string idRegistro = request.idRegistro;
            bool enableUffRef = request.enableUffRef;
            long idTitolario = request.idTitolario.AsLong();
            bool enableProfilazione = request.enableProfilazione;
            string systemId = request.systemId;
            long systemIdAsLong = request.systemId.AsLong();

            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();

            try
            {
                var rights = await _dbContext.GetSecurityRights(systemId, idUser, idGroup);
                var rigthsAsLong = Convert.ToInt32(rights);

                long? idRegistroAsLong = string.IsNullOrEmpty(idRegistro) ? null : idRegistro.AsLong();

                var f = await this._dbContext.ProjectEntities.Where(x =>
                (x.ID_AMM == null || x.ID_AMM == idTenantAsLong) &&
                    rigthsAsLong > 0 &&
                    x.CHA_TIPO_PROJ.Equals("F") &&
                    x.ID_REGISTRO == idRegistroAsLong &&
                    x.VAR_CODICE.ToUpper().Equals(codiceFascicolo.ToUpper()) &&
                    x.ID_TITOLARIO == idTitolario &&
                    x.SYSTEM_ID == systemIdAsLong
                )
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
                    VAR_NOTE = x.VAR_NOTE,
                    DTA_APERTURA = x.DTA_APERTURA,
                    DTA_CHIUSURA = x.DTA_CHIUSURA,
                    CHA_STATO = x.CHA_STATO,
                    ID_TIPO_PROC = x.ID_TIPO_PROC,
                    ID_REGISTRO = x.ID_REGISTRO,
                    ID_UO_LF = x.ID_UO_LF,
                    DTA_UO_LF = x.DTA_UO_LF,
                    DTA_CREAZIONE = x.DTA_CREAZIONE,
                    ACCESSRIGTHS = GetAccessRigths(rights),
                    CARTACEO = x.CARTACEO,
                    CHA_PRIVATO = x.CHA_PRIVATO,
                    ID_TITOLARIO = x.ID_TITOLARIO,
                    DTA_SCADENZA = x.DTA_SCADENZA,
                    NUM_FASCICOLO = x.NUM_FASCICOLO,
                    AUTHOR = x.AUTHOR,
                    ID_RUOLO_CREATORE = x.ID_RUOLO_CREATORE,
                    ID_UO_CREATORE = x.ID_UO_CREATORE,
                    CHA_CONTROLLATO = x.CHA_CONTROLLATO,
                    ID_PIANO_CONSERVAZIONE = x.ID_PIANO_CONSERVAZIONE
                })
                .FirstOrDefaultAsync();

                if (f != null)
                {
                    result = new Fascicolo()
                    {
                        systemID = f.SYSTEM_ID.ToString(),
                        apertura = f.DTA_APERTURA?.ToString().Trim(),
                        chiusura = f.DTA_CHIUSURA?.ToString().Trim(),
                        codice = f.VAR_CODICE?.ToString(),
                        descrizione = f.DESCRIPTION?.ToString(),
                        stato = f.CHA_STATO?.ToString(),
                        tipo = f.CHA_TIPO_FASCICOLO?.ToString(),
                        idClassificazione = f.ID_PARENT?.ToString(),
                        codUltimo = f.VAR_COD_ULTIMO?.ToString(),
                        idRegistroNodoTit = f.ID_REGISTRO?.ToString(),
                        idTitolario = f.ID_TITOLARIO?.ToString(),
                        accessRights = f.ACCESSRIGTHS,
                        dtaLF = f.DTA_UO_LF?.ToString(),
                        idUoLF = f.ID_UO_LF?.ToString(),
                        cartaceo = Convert.ToInt32(f.CARTACEO) > 0,
                        privato = f.CHA_PRIVATO,
                        controllato = f.CHA_CONTROLLATO,
                        dtaScadenza = f.DTA_SCADENZA != null ? f.DTA_SCADENZA.AsDateFormat() : string.Empty                        
                    };

                    long idGroupAsLong = idGroup.AsLong();
                    long idUserAsLong = idUser.AsLong();

                    var idCorrGlobaliRuolo = this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroupAsLong).Select(x => x.SYSTEM_ID).FirstOrDefault();
                    if (idCorrGlobaliRuolo == null)
                        throw new CorrGlobaliByGroupIdNotFoundPi3Exception(idGroupAsLong);

                    result.noteFascicolo = FetchNoteFascicolo(idUserAsLong, idGroupAsLong, idCorrGlobaliRuolo, result);

                    string codiceUoCreatore = await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == f.ID_UO_CREATORE).Select(x => x.VAR_CODICE).FirstOrDefaultAsync();
                    DocsPaVO.fascicolazione.CreatoreFascicolo objCreatore = new DocsPaVO.fascicolazione.CreatoreFascicolo()
                    {
                        idPeople = f.AUTHOR.ToString(),
                        idCorrGlob_Ruolo = f.ID_RUOLO_CREATORE.ToString(),
                        idCorrGlob_UO = f.ID_UO_CREATORE.ToString(),
                        uo_codiceCorrGlobali = codiceUoCreatore
                    };

                    if (enableProfilazione)
                    {
                        var template = await this._mediator.Send(new Application.Requests.getTemplateFascDettagli(f.SYSTEM_ID.ToString()));
                        result.template = template.output ?? null;
                    }

                    if (f.ID_PIANO_CONSERVAZIONE != null)
                    {
                        result.pianoConservazione = await GetPianoConservazioneById(f.ID_PIANO_CONSERVAZIONE);
                    }

                    result.InAreaLavoro = await IsProjectInADLUtente(result.systemID.AsLong(), idUserAsLong, idCorrGlobaliRuolo);

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new FascicolazioneGetFascicoloInClassificaResult(result);
        }

        private async Task<string> IsProjectInADLUtente(long systemId, long idPeople, long idCorrGlobali)
        {
            return this._dbContext.AreaLavoroEntities.Any(x => x.ID_PROJECT == systemId &&
            x.ID_PEOPLE == idPeople &&
            x.ID_RUOLO_IN_UO == idCorrGlobali) ? "1" : "0";
        }

        private string GetAccessRigths(SecurityRightTypesEnum rights)
        {
            switch (rights)
            {
                case SecurityRightTypesEnum.FullControl:
                    return "255";
                    break;
                case SecurityRightTypesEnum.Write:
                    return "63";
                    break;
                case SecurityRightTypesEnum.Read:
                    return "45";
                    break;
                default:
                    return "-1";
                    break;
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetFascicoloInClassificaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

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
    }

}
