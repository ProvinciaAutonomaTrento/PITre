// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2013.Word;
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
using GetTrasmissioneByIdTrasmSingRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetTrasmissioneByIdTrasmSing;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTrasmissioneByIdTrasmSing
{
    public class GetTrasmissioneByIdTrasmSingHandler : IRequestHandler<GetTrasmissioneByIdTrasmSingRequest, GetTrasmissioneByIdTrasmSingResult>
    {
        #region Public Members

        public GetTrasmissioneByIdTrasmSingHandler(ILogger<GetTrasmissioneByIdTrasmSingHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetTrasmissioneByIdTrasmSingResult> Handle(GetTrasmissioneByIdTrasmSingRequest request, CancellationToken cancellationToken)
        {
            Trasmissione trasmissione = new Trasmissione();

            var idTrasmissioneSingola = request.idTrasmSing.AsLong();

            var idTrasmissione = await _dbContext.TrasmSingolaEntities.AsNoTracking()
                .Where(s => s.SYSTEM_ID == idTrasmissioneSingola)
                .Select(s => s.ID_TRASMISSIONE)
                .FirstOrDefaultAsync();

            if(idTrasmissione != null)
                trasmissione = (await _mediator.Send(new Requests.GetTrasmissioneById(request.oggettoTrasmesso, request.utente, request.ruolo, idTrasmissione.ToString()))).output;

            /*
            var queryable = _dbContext.TrasmissioneEntities
                .Join(_dbContext.TrasmSingolaEntities,
                    t => t.SYSTEM_ID,
                    s => s.ID_TRASMISSIONE,
                    (t, s) => new { t, s })
                .Join(_dbContext.TrasmUtenteEntities,
                    j => j.s.SYSTEM_ID,
                    u => u.ID_TRASM_SINGOLA,
                    (j, u) => new { j.t, j.s, u })
                .Join(_dbContext.CorrGlobaliEntities,
                    j => j.s.ID_CORR_GLOBALE,
                    z => z.SYSTEM_ID,
                    (j, z) => new { j.t, j.s, j.u, z })
                .Join(_dbContext.PeopleEntities,
                    j => j.t.ID_PEOPLE,
                    e => e.SYSTEM_ID,
                    (j, e) => new { j.t, j.s, j.u, j.z, e })
                .Join(_dbContext.CorrGlobaliEntities,
                    j => j.t.ID_RUOLO_IN_UO,
                    f => f.SYSTEM_ID,
                    (j, f) => new { j.t, j.s, j.u, j.z, j.e, f })
                .Join(_dbContext.RagioneTrasmissioneEntities,
                    j => j.s.ID_RAGIONE,
                    r => r.SYSTEM_ID,
                    (j, r) => new { j.t, j.s, j.u, j.z, j.e, j.f, r })
                .Join(_dbContext.PeopleEntities,
                    j => j.u.ID_PEOPLE,
                    h => h.SYSTEM_ID,
                    (j, h) => new { j.t, j.s, j.u, j.z, j.e, j.f, j.r, h })
                .Where(j => j.t.SYSTEM_ID == idTrasmissione);

            if (request.oggettoTrasmesso.infoDocumento != null)
            {
                var idProfile = request.oggettoTrasmesso.infoDocumento.idProfile.AsLong();
                queryable = queryable.Where(j => j.t.ID_PROFILE == idProfile);
            }

            if (request.oggettoTrasmesso.infoFascicolo != null)
            {
                var idProject = request.oggettoTrasmesso.infoFascicolo.idFascicolo.AsLong();
                queryable = queryable.Where(j => j.t.ID_PROJECT == idProject);
            }

            var trasmissioneEntities = await queryable
                .AsNoTracking()
                .Select(j => new
                {
                    j.t.ID_RUOLO_IN_UO,
                    j.t.ID_PEOPLE,
                    j.t.CHA_TIPO_OGGETTO,
                    j.t.ID_PROFILE,
                    j.t.ID_PROJECT,
                    j.t.DTA_INVIO,
                    j.t.VAR_NOTE_GENERALI,
                    j.t.ID_PEOPLE_DELEGATO,
                    j.t.CHA_SALVATA_CON_CESSIONE,
                    j.s.ID_RAGIONE,
                    j.s.ID_TRASMISSIONE,
                    j.s.ID_TRASM_UTENTE,
                    j.s.CHA_TIPO_DEST,
                    j.s.ID_CORR_GLOBALE,
                    j.s.HIDE_DOC_VERSIONS,
                    j.s.VAR_NOTE_SING,
                    j.s.CHA_TIPO_TRASM,
                    j.s.DTA_SCADENZA,
                    ID_TRASMISSIONE_UTENTE = j.u.SYSTEM_ID,
                    ID_DESTINATARIO = j.u.ID_PEOPLE,
                    j.u.DTA_VISTA,
                    j.u.CHA_VISTA,
                    j.u.DTA_ACCETTATA,
                    j.u.CHA_ACCETTATA,
                    j.u.DTA_RIFIUTATA,
                    j.u.CHA_RIFIUTATA,
                    j.u.VAR_NOTE_RIF,
                    j.u.VAR_NOTE_ACC,
                    j.u.ID_TRASM_SINGOLA,
                    j.u.CHA_VALIDA,
                    DTA_RIMOSSA_TDL = j.u.DTA_RIMOZIONE_TODOLIST,
                    DELEGATO_UTENTE = j.u.ID_PEOPLE_DELEGATO,
                    ACCETTATA_DELEGATO = j.u.CHA_ACCETTATA_DELEGATO,
                    VISTA_DELEGATO = j.u.CHA_VISTA_DELEGATO,
                    RIFIUTATA_DELEGATO = j.u.CHA_RIFIUTATA_DELEGATO,
                    RIMOSSA_DELEGATO = j.u.CHA_RIMOZIONE_DELEGATO,
                    ID_SEGN_CODFASC = IPi3DbContextMappedFunctions.VarDescribe((long)j.t.ID_PROFILE, "SEGNATURA_CODFASC") ?? IPi3DbContextMappedFunctions.VarDescribe((long)j.t.ID_PROJECT, "CODFASC"),
                    DATA_DOC_FASC = IPi3DbContextMappedFunctions.VarDescribe((long)j.t.ID_PROFILE, "DATADOC") ?? IPi3DbContextMappedFunctions.VarDescribe((long)j.t.ID_PROJECT, "DATA_CREAZ"),
                    j.z.VAR_DESC_CORR,
                    j.z.CHA_TIPO_URP,
                    j.e.FULL_NAME,
                    DESC_RUOLO_MITT = j.f.VAR_DESC_CORR,
                    j.r.CHA_TIPO_RAGIONE,
                    j.r.VAR_DESC_RAGIONE,
                    j.r.CHA_EREDITA,
                    j.r.CHA_TIPO_DIRITTI,
                    DESC_DESTINATARIO = j.h.FULL_NAME,
                    ID_PEOPLE_DESTINATARIO = j.h.SYSTEM_ID,
                    COD_RUOLO_MITT = j.f.VAR_CODICE,
                    j.e.USER_ID,
                    j.r.CHA_RISPOSTA,
                    TIPO_DEST_RAG = j.r.CHA_TIPO_DEST,
                    j.r.VAR_NOTE,
                    j.r.CHA_TIPO_RISPOSTA,
                    j.r.VAR_NOTIFICA_TRASM,
                    j.r.CHA_CEDE_DIRITTI,
                    j.r.CHA_MANTIENI_LETT,
                    j.r.CHA_PROC_RES

                })
                .OrderByDescending(j => j.DTA_INVIO)
                .ThenByDescending(j => j.ID_TRASMISSIONE)
                .ThenBy(j => j.ID_TRASM_UTENTE)
                .ToListAsync();

            if (trasmissioneEntities != null && trasmissioneEntities.Count > 0)
            {
                var trasmissioneEntity = trasmissioneEntities[0];

                trasmissione.systemId = trasmissioneEntity.ID_TRASMISSIONE.ToString();
                trasmissione.dataInvio = trasmissioneEntity.DTA_INVIO.HasValue ? trasmissioneEntity.DTA_INVIO.AsDateTimeFormat() : string.Empty;
                trasmissione.daAggiornare = false;
                trasmissione.noteGenerali = trasmissioneEntity.VAR_NOTE_GENERALI;
                trasmissione.tipoOggetto = trasmissioneEntity.CHA_TIPO_OGGETTO == "D" ? TipoOggetto.DOCUMENTO : TipoOggetto.FASCICOLO;
                trasmissione.salvataConCessione = trasmissioneEntity.CHA_SALVATA_CON_CESSIONE == "1";
                trasmissione.infoDocumento = request.oggettoTrasmesso.infoDocumento;
                trasmissione.infoFascicolo = request.oggettoTrasmesso.infoFascicolo;

                if (request.utente.idPeople.Equals(trasmissioneEntity.ID_PEOPLE.ToString()))
                {
                    trasmissione.utente = new DocsPaVO.utente.Utente()
                    {
                        idPeople = trasmissioneEntity.ID_PEOPLE.ToString(),
                        descrizione = trasmissioneEntity.FULL_NAME,
                        userId = trasmissioneEntity.USER_ID
                    };
                }

                trasmissione.ruolo = request.ruolo.systemId.Equals(trasmissioneEntity.ID_RUOLO_IN_UO) ? request.ruolo :
                    new DocsPaVO.utente.Ruolo()
                    {
                        systemId = trasmissioneEntity.ID_RUOLO_IN_UO.ToString(),
                        descrizione = trasmissioneEntity.DESC_RUOLO_MITT,
                        codice = trasmissioneEntity.COD_RUOLO_MITT,
                    };

                trasmissione.delegato = trasmissioneEntity.ID_PEOPLE_DELEGATO == null ? string.Empty :
                    await _dbContext.PeopleEntities.AsNoTracking().Where(p => p.SYSTEM_ID == trasmissioneEntity.ID_PEOPLE_DELEGATO).Select(p => p.FULL_NAME).FirstOrDefaultAsync();

                List<TrasmissioneSingola> trasmissioniSingole = new List<TrasmissioneSingola>();
                List<TrasmissioneUtente> trasmissioneUtente = null;
                foreach (var trasmSingolaEntity in trasmissioneEntities)
                {
                    if (!trasmissioniSingole.Any(s => s.systemId == trasmSingolaEntity.ID_TRASM_SINGOLA.ToString()))
                    {
                        TrasmissioneSingola trasmSingola = new TrasmissioneSingola();
                        trasmissioneUtente = new List<TrasmissioneUtente>();

                        trasmSingola.systemId = trasmSingolaEntity.ID_TRASM_SINGOLA.ToString();
                        trasmSingola.noteSingole = trasmissioneEntity.VAR_NOTE_SING;
                        trasmSingola.tipoTrasm = trasmSingolaEntity.CHA_TIPO_TRASM;
                        trasmSingola.idTrasmUtente = trasmSingolaEntity.ID_TRASMISSIONE_UTENTE.ToString();
                        trasmSingola.dataScadenza = trasmissioneEntity.DTA_SCADENZA.HasValue ? trasmissioneEntity.DTA_SCADENZA.AsDateFormat() : string.Empty;
                        trasmSingola.hideDocumentPreviousVersions = trasmSingolaEntity.HIDE_DOC_VERSIONS == "1";

                        trasmSingola.ragione = new RagioneTrasmissione()
                        {
                            systemId = trasmSingolaEntity.ID_RAGIONE.ToString(),
                            descrizione = trasmSingolaEntity.VAR_DESC_RAGIONE,
                            tipo = trasmSingolaEntity.CHA_TIPO_RAGIONE,
                            risposta = trasmSingolaEntity.CHA_RISPOSTA,
                            note = trasmSingolaEntity.VAR_NOTE,
                            eredita = trasmSingolaEntity.CHA_EREDITA,
                            tipoRisposta = trasmSingolaEntity.CHA_TIPO_RISPOSTA ?? string.Empty
                        };
                        trasmSingola.tipoDest = trasmSingolaEntity.CHA_TIPO_DEST == "R" ? TipoDestinatario.RUOLO : TipoDestinatario.UTENTE;
                        trasmSingola.corrispondenteInterno = await GetCorrispondente(trasmSingolaEntity.ID_CORR_GLOBALE.Value);

                        //Aggiungo le trasmissioni utente
                        foreach (var trasmUtenteEntity in trasmissioneEntities.Where(u => u.ID_TRASM_SINGOLA == trasmissioneEntity.ID_TRASM_SINGOLA))
                        {
                            TrasmissioneUtente trasmUtente = new TrasmissioneUtente();
                            trasmUtente.systemId = trasmUtenteEntity.ID_TRASMISSIONE_UTENTE.ToString();
                            trasmUtente.dataVista = trasmUtenteEntity.DTA_VISTA.HasValue ? trasmUtenteEntity.DTA_VISTA.AsDateFormat() : string.Empty;
                            trasmUtente.dataAccettata = trasmUtenteEntity.DTA_ACCETTATA.HasValue ? trasmUtenteEntity.DTA_ACCETTATA.AsDateFormat() : string.Empty;
                            trasmUtente.dataRifiutata = trasmUtenteEntity.DTA_RIFIUTATA.HasValue ? trasmUtenteEntity.DTA_RIFIUTATA.AsDateFormat() : string.Empty;
                            trasmUtente.noteRifiuto = trasmUtenteEntity.VAR_NOTE_RIF ?? string.Empty;
                            trasmUtente.noteAccettazione = trasmUtenteEntity.VAR_NOTE_ACC ?? string.Empty;
                            trasmUtente.valida = trasmUtenteEntity.CHA_VALIDA;
                            trasmUtente.idPeopleDelegato = trasmUtenteEntity.DELEGATO_UTENTE == null ? string.Empty :
                                await _dbContext.PeopleEntities.AsNoTracking().Where(p => p.SYSTEM_ID == trasmUtenteEntity.DELEGATO_UTENTE).Select(p => p.FULL_NAME).FirstOrDefaultAsync();
                            trasmUtente.cha_accettata_delegato = trasmUtenteEntity.ACCETTATA_DELEGATO ?? string.Empty;
                            trasmUtente.cha_vista_delegato = trasmUtenteEntity.VISTA_DELEGATO ?? string.Empty;
                            trasmUtente.cha_rifiutata_delegato = trasmUtenteEntity.RIFIUTATA_DELEGATO ?? string.Empty;
                            trasmUtente.cha_rimossa_delegato = trasmUtenteEntity.RIMOSSA_DELEGATO ?? string.Empty;
                            trasmUtente.dataRimossaTDL = trasmUtenteEntity.DTA_RIMOSSA_TDL.HasValue ? trasmUtenteEntity.DTA_RIMOSSA_TDL.AsDateFormat() : string.Empty;

                            trasmUtente.utente = new DocsPaVO.utente.Utente()
                            {
                                systemId = trasmUtenteEntity.ID_DESTINATARIO.ToString(),
                                descrizione = trasmUtenteEntity.DESC_DESTINATARIO,
                                idPeople = trasmUtenteEntity.ID_PEOPLE_DESTINATARIO.ToString()
                            };
                            trasmissioneUtente.Add(trasmUtente);
                        }
                        trasmSingola.trasmissioneUtente = trasmissioneUtente.ToArray();

                        trasmissioniSingole.Add(trasmSingola);
                    }
                }

                trasmissione.trasmissioniSingole = trasmissioniSingole.ToArray();
            }
            */

            return new GetTrasmissioneByIdTrasmSingResult(trasmissione);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTrasmissioneByIdTrasmSingHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected async Task<Corrispondente> GetCorrispondente(long idCorrGlobali)
        {
            Corrispondente corrispondente = null;
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.SYSTEM_ID == idCorrGlobali)
                .Select(c => new
                {
                    c.SYSTEM_ID,
                    c.ID_PEOPLE,
                    c.ID_GRUPPO,
                    c.VAR_DESC_CORR,
                    c.CHA_TIPO_URP
                })
                .FirstAsync();

            if(corrGlobaliEntity.CHA_TIPO_URP == "P")
            {
                corrispondente = new Utente()
                {
                    systemId = corrGlobaliEntity.SYSTEM_ID.ToString(),
                    idPeople = corrGlobaliEntity.ID_PEOPLE.ToString(),
                    descrizione = corrGlobaliEntity.VAR_DESC_CORR,
                    tipoCorrispondente = corrGlobaliEntity.CHA_TIPO_URP
                };
            }
            else
            {
                corrispondente = new Ruolo()
                {
                    systemId = corrGlobaliEntity.SYSTEM_ID.ToString(),
                    idGruppo = corrGlobaliEntity.ID_GRUPPO.ToString(),
                    descrizione = corrGlobaliEntity.VAR_DESC_CORR,
                    tipoCorrispondente = corrGlobaliEntity.CHA_TIPO_URP
                };
            }

            return corrispondente;
        }
        #endregion
    }
}