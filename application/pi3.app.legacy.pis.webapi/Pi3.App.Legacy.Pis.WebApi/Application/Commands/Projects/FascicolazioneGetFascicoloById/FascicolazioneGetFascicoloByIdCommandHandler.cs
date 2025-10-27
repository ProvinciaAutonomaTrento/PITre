// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO;
using DocsPaVO.Note;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SetDataVistaSP;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetListaFascicoliDaCodice;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Globalization;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFascicoloById
{
    public class FascicolazioneGetFascicoloByIdCommandHandler : IRequestHandler<FascicolazioneGetFascicoloByIdCommand, FascicolazioneGetFascicoloByIdCommandResponse>
    {
        public FascicolazioneGetFascicoloByIdCommandHandler(ILogger<FascicolazioneGetFascicoloByIdCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }


        public async Task<FascicolazioneGetFascicoloByIdCommandResponse> Handle(FascicolazioneGetFascicoloByIdCommand request, CancellationToken cancellationToken)
        {

            DocsPaVO.fascicolazione.Fascicolo result = null;
            string idFascicolo = request.IdFascicolo;
            long idFascicoloAsLong = idFascicolo.AsLong();

            try
            {
                var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
                var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
                var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();

                var rights = await _dbContext.GetSecurityRights(idFascicolo, idUser, idGroup);
                var rigthsAsLong = Convert.ToInt32(rights);

                var f = await _dbContext.ProjectEntities.Where(x => x.SYSTEM_ID == idFascicoloAsLong && rigthsAsLong > 0 && x.CHA_TIPO_PROJ.Equals("F"))
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
                        CHA_IN_ARCHIVIO = x.CHA_IN_ARCHIVIO,
                        ACCESSRIGTHS = GetAccessRigths(rights),
                        AUTHOR = x.AUTHOR,
                        ID_RUOLO_CREATORE = x.ID_RUOLO_CREATORE,
                        ID_UO_CREATORE = x.ID_UO_CREATORE,
                        CHA_CONTROLLATO = x.CHA_CONTROLLATO,
                        CHA_COD_T_A = x.CHA_COD_T_A,
                        COD_EXT_APP = x.COD_EXT_APP,
                        ID_TITOLARIO = x.ID_TITOLARIO,
                        DTA_SCADENZA = x.DTA_SCADENZA.HasValue ? x.DTA_SCADENZA.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : String.Empty,
                        CHA_PUBBLICO = x.CHA_PUBBLICO,
                        ID_PIANO_CONSERVAZIONE = x.ID_PIANO_CONSERVAZIONE,
                        PRIVATO = x.CHA_PRIVATO

                    })
                    .Distinct()
                    .FirstOrDefaultAsync();

                if (f != null)
                {
                    result = new DocsPaVO.fascicolazione.Fascicolo()
                    {
                        systemID = f.SYSTEM_ID.ToString(),
                        apertura = f.DTA_APERTURA != null ? f.DTA_APERTURA.AsDateFormat() : string.Empty,
                        chiusura = f.DTA_CHIUSURA != null ? f.DTA_CHIUSURA.AsDateFormat() : string.Empty,
                        codice = f.VAR_CODICE?.ToString(),
                        descrizione = f.DESCRIPTION?.ToString(),
                        stato = f.CHA_STATO?.ToString(),
                        tipo = f.CHA_TIPO_FASCICOLO?.ToString(),
                        idClassificazione = f.ID_PARENT?.ToString(),
                        codUltimo = f.VAR_COD_ULTIMO?.ToString(),
                        idRegistroNodoTit = f.ID_REGISTRO?.ToString(),
                        accessRights = f.ACCESSRIGTHS,
                        dtaLF = f.DTA_UO_LF != null ? f.DTA_UO_LF.AsDateTimeFormat() : string.Empty,
                        idUoLF = f.ID_UO_LF?.ToString(),
                        controllato = f.CHA_CONTROLLATO,
                        dtaScadenza = !string.IsNullOrEmpty(f.DTA_SCADENZA) ? f.DTA_SCADENZA.AsDateTime().ToString() : string.Empty,
                        inArchivio = f.CHA_IN_ARCHIVIO,
                        idTitolario = f.ID_TITOLARIO.ToString(),
                        privato = f.PRIVATO ?? string.Empty
                    };

                    long idGroupAsLong = idGroup.AsLong();
                    long idUserAsLong = idUser.AsLong();

                    var idCorrGlobaliRuolo = this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroupAsLong).Select(x => x.SYSTEM_ID).FirstOrDefault();
                    if (idCorrGlobaliRuolo == null)
                        throw new CorrGlobaliByGroupIdNotFoundPi3Exception(idGroupAsLong);

                    result.noteFascicolo = FetchNoteFascicolo(idUserAsLong, idGroupAsLong, idCorrGlobaliRuolo, result);

                    result.codiceRegistroNodoTit = !string.IsNullOrEmpty(result.idRegistroNodoTit) ? GetCodiceRegistroBySystemId(result.idRegistroNodoTit) : string.Empty;

                    result.isFascicolazioneConsentita = (await GetChaConsentiFasc(f.ID_PARENT, f.CHA_TIPO_PROJ, f.CHA_TIPO_FASCICOLO)).Equals("1");

                    string codiceUoCreatore = await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == f.ID_UO_CREATORE).Select(x => x.VAR_CODICE).FirstOrDefaultAsync();
                    DocsPaVO.fascicolazione.CreatoreFascicolo objCreatore = new DocsPaVO.fascicolazione.CreatoreFascicolo()
                    {
                        idPeople = f.AUTHOR.ToString(),
                        idCorrGlob_Ruolo = f.ID_RUOLO_CREATORE.ToString(),
                        idCorrGlob_UO = f.ID_UO_CREATORE.ToString(),
                        uo_codiceCorrGlobali = codiceUoCreatore
                    };

                    if (f.ID_PIANO_CONSERVAZIONE != null)
                    {
                        result.pianoConservazione = await GetPianoConservazioneById(f.ID_PIANO_CONSERVAZIONE);
                    }

                    result.InAreaLavoro = await IsProjectInADLUtente(result.systemID.AsLong(), idUserAsLong, idCorrGlobaliRuolo);


                    var setDataVistaOutput = await this._mediator.Send(new SetDataVistaSPCommand()
                    {
                        InfoUtente = request.InfoUtente,
                        DocNumber = result.systemID,
                        DocOrFasc = "F"
                    });
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new()
            {
                Output = result
            };
        }


        #region Private Members

        protected readonly ILogger<FascicolazioneGetFascicoloByIdCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        private async Task<string> IsProjectInADLUtente(long systemId, long idPeople, long idCorrGlobali)
        {
            return this._dbContext.AreaLavoroEntities.Any(x => x.ID_PROJECT == systemId &&
            x.ID_PEOPLE == idPeople &&
            x.ID_RUOLO_IN_UO == idCorrGlobali) ? "1" : "0";
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

        private async Task<string> GetChaConsentiFasc(long? idParent, string? tipoProj, string? tipoFascicolo)
        {
            string result = "1";

            if (tipoProj.Equals("F") && tipoFascicolo.Equals("P"))
                result = await this._dbContext.ProjectEntities.Where(x => x.SYSTEM_ID == idParent).Select(x => x.CHA_CONSENTI_FASC).FirstOrDefaultAsync();

            return result;
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


        #endregion
    }
}
