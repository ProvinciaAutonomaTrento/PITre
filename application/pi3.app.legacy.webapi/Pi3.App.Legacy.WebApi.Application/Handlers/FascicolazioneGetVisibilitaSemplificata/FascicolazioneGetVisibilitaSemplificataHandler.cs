// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetVisibilitaSemplificata
{

    // Richiede libreria MediatR
    public class FascicolazioneGetVisibilitaSemplificataHandler : IRequestHandler<Application.Requests.FascicolazioneGetVisibilitaSemplificata, FascicolazioneGetVisibilitaSemplificataResult>
    {
        #region Public Members

        public FascicolazioneGetVisibilitaSemplificataHandler(ILogger<FascicolazioneGetVisibilitaSemplificataHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
        }

        public async Task<FascicolazioneGetVisibilitaSemplificataResult> Handle(Application.Requests.FascicolazioneGetVisibilitaSemplificata request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.fascicolazione.DirittoOggetto> listaDiritti = new List<DocsPaVO.fascicolazione.DirittoOggetto>();
            DocsPaVO.fascicolazione.InfoFascicolo infoFascicolo = request.infoFascicolo;
            bool cercaRimossi = request.cercaRimossi;
            string rootFolder = request.rootFolder;

            if (string.IsNullOrEmpty(infoFascicolo.idFascicolo))
                return new FascicolazioneGetVisibilitaSemplificataResult(null);

            long idProject = infoFascicolo.idFascicolo.AsLong();
            string IDAMM = string.Empty;

            try
            {
                #region Inserimento ruoli
                var j1Roles = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.SecurityEntities, a => a.ID_GRUPPO, b => b.PERSONORGROUP, (a, b) => new { a, b });
                var j2Roles = j1Roles.Join(this._dbContext.TipoRuoloEntities, j1 => j1.a.ID_TIPO_RUOLO, c => c.SYSTEM_ID, (j1, c) => new { a = j1.a, b = j1.b, c });
                var j3Roles = j2Roles.Join(this._dbContext.CorrGlobaliEntities, j2 => j2.a.ID_UO, d => d.SYSTEM_ID, (j2, d) => new { a = j2.a, b = j2.b, c = j2.c, d });

                var visibilityListRoles = await j3Roles.Where(x => x.b.THING == idProject)
                    .Distinct()
                    .Select(x => new
                    {
                        SYSTEM_ID = x.a.SYSTEM_ID,
                        VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                        ID_REGISTRO = x.a.ID_REGISTRO,
                        ID_AMM = x.a.ID_AMM,
                        VAR_DESC_RUOLO = x.c.VAR_DESC_RUOLO,
                        CHA_TIPO_DIRITTO = x.b.CHA_TIPO_DIRITTO,
                        ACCESSRIGHTS = x.b.ACCESSRIGHTS,
                        ID_UO = x.a.ID_UO,
                        PERSONORGROUP = x.b.PERSONORGROUP,
                        CHA_TIPO_CORR = x.a.CHA_TIPO_CORR,
                        CHA_TIPO_IE = x.a.CHA_TIPO_IE,
                        CHA_TIPO_URP = x.a.CHA_TIPO_URP,
                        VAR_DESC_CORR = x.a.VAR_DESC_CORR,
                        DESC_UO = x.d.VAR_DESC_CORR,
                        CHA_TIPO_UO = x.d.CHA_TIPO_IE,
                        VAR_COD_UO = x.d.VAR_COD_RUBRICA,
                        TS_INSERIMENTO = x.b.TS_INSERIMENTO,
                        VAR_NOTE_SEC = x.b.VAR_NOTE_SEC,
                        ShowHistory = 0,
                        ORIGINAL_ID = x.a.ORIGINAL_ID,
                        DTA_FINE = x.a.DTA_FINE
                    })
                    .OrderByDescending(x => x.ACCESSRIGHTS)
                    .ThenBy(x => x.ID_UO)
                    .ToListAsync();

                    foreach (var r in visibilityListRoles)
                    {
                        int showHistory = await GetShowHistory(r.ORIGINAL_ID);

                        DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto()
                        {
                            idObj = idProject.ToString(),
                            tipoDiritto = GetDiritto(r.CHA_TIPO_DIRITTO),
                            accessRights = Convert.ToInt32(r.ACCESSRIGHTS),
                            deleted = false,
                            personorgroup = r.PERSONORGROUP?.ToString(),
                            dtaInsSecurity = r.TS_INSERIMENTO.HasValue ? r.TS_INSERIMENTO.ToString() : string.Empty,
                            noteSecurity = r.VAR_NOTE_SEC,
                            rootFolder = rootFolder
                        };

                        DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo()
                        {
                            systemId = r.SYSTEM_ID.ToString(),
                            dta_fine = r.DTA_FINE != null && r.DTA_FINE.HasValue ? r.DTA_FINE.ToString() : string.Empty,
                            codiceRubrica = r.VAR_COD_RUBRICA ?? String.Empty,
                            ShowHistory = showHistory != 0 ? showHistory.ToString() : string.Empty,
                            descrizione = r.VAR_DESC_CORR ?? string.Empty,
                            tipoCorrispondente = r.CHA_TIPO_URP ?? string.Empty,
                            tipoIE = r.CHA_TIPO_IE ?? string.Empty,
                            idGruppo = r.PERSONORGROUP?.ToString(),

                        };

                        DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro()
                        {
                            systemId = r.ID_REGISTRO?.ToString()
                        };

                        ruolo.registri = new DocsPaVO.utente.Registro[] { reg };

                        if (r.ID_UO != null)
                        {
                            ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa()
                            {
                                systemId = r.ID_UO.ToString(),
                                idAmministrazione = r.ID_AMM != null ? r.ID_AMM.ToString() : string.Empty,
                                descrizione = r.DESC_UO ?? string.Empty,
                                tipoIE = r.CHA_TIPO_UO ?? string.Empty,
                                codiceRubrica = r.VAR_COD_UO ?? string.Empty,
                            };
                        }

                        if (r.ID_AMM != null)
                        {
                            ruolo.idAmministrazione = r.ID_AMM.ToString();
                            IDAMM = ruolo.idAmministrazione;
                        }

                        dirittoOggetto.soggetto = ruolo;

                        listaDiritti.Add(dirittoOggetto);
                    }

                #endregion

                #region Inserimento utenti
                var j1Users = this._dbContext.PeopleEntities.Join(this._dbContext.SecurityEntities, a => a.SYSTEM_ID, b => b.PERSONORGROUP, (a, b) => new { a, b });
                var j2Users = j1Users.Join(this._dbContext.CorrGlobaliEntities, j1 => j1.a.SYSTEM_ID, c => c.ID_PEOPLE, (j1, c) => new { a = j1.a, b = j1.b, c });

                var visibilityListUsers = await j2Users.Where(x => x.b.THING == idProject && !x.c.CHA_TIPO_URP.Equals("L"))
                    .Select(x => new
                    {
                        SYSTEM_ID = x.c.SYSTEM_ID,
                        ID_REGISTRO = x.c.ID_REGISTRO,
                        ID_AMM = x.c.ID_AMM,
                        VAR_NOME = x.a.VAR_NOME,
                        VAR_COGNOME = x.a.VAR_COGNOME,
                        VAR_COD_RUBRICA = x.c.VAR_COD_RUBRICA,
                        CHA_TIPO_DIRITTO = x.b.CHA_TIPO_DIRITTO,
                        PERSONORGROUP = x.b.PERSONORGROUP,
                        ACCESSRIGHTS = x.b.ACCESSRIGHTS,
                        DISABLED = x.a.DISABLED,
                        CHA_TIPO_CORR = x.c.CHA_TIPO_CORR,
                        CHA_TIPO_IE = x.c.CHA_TIPO_IE,
                        CHA_TIPO_URP = x.c.CHA_TIPO_URP,
                        VAR_DESC_CORR = x.c.VAR_DESC_CORR
                    })
                    .ToListAsync();

                foreach (var u in visibilityListUsers)
                {
                    DocsPaVO.utente.Utente utenteTemp = new DocsPaVO.utente.Utente()
                    {
                        idPeople = u.PERSONORGROUP.ToString(),
                        codiceRubrica = u.VAR_COD_RUBRICA,
                        idAmministrazione = u.ID_AMM?.ToString() ?? string.Empty,
                        descrizione = u.VAR_DESC_CORR,
                        idRegistro = u.ID_REGISTRO?.ToString() ?? string.Empty,
                        tipoCorrispondente = u.CHA_TIPO_URP?.ToString() ?? string.Empty,
                        tipoIE = u.CHA_TIPO_IE?.ToString() ?? string.Empty
                    };

                    DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto()
                    {
                        idObj = idProject.ToString(),
                        soggetto = utenteTemp,
                        tipoDiritto = GetDiritto(u.CHA_TIPO_DIRITTO),
                        accessRights = Convert.ToInt32(u.ACCESSRIGHTS),
                        deleted = false,
                        personorgroup = u.PERSONORGROUP.ToString(),
                        rootFolder = rootFolder
                    };

                    if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO))
                    {
                        dirittoOggetto.soggetto = utenteTemp;
                        dirittoOggetto.soggetto.descrizione = utenteTemp.descrizione + " sostituto di " + utenteTemp.descrizione;
                    }

                    listaDiritti.Add(dirittoOggetto);
                }

                #endregion

                #region Ruoli e utenti rimossi
                if (cercaRimossi)
                {
                    #region Ruoli rimossi
                    var j1RolesRemoved = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.DeletedSecurityEntities, a => a.ID_GRUPPO, b => b.PERSONORGROUP, (a, b) => new { a, b });
                    var j2RolesRemoved = j1RolesRemoved.Join(this._dbContext.TipoRuoloEntities, j1 => j1.a.ID_TIPO_RUOLO, c => c.SYSTEM_ID, (j1, c) => new { a = j1.a, b = j1.b, c });
                    var j3RolesRemoved = j2RolesRemoved.Join(this._dbContext.CorrGlobaliEntities, j2 => j2.a.ID_UO, d => d.SYSTEM_ID, (j2, d) => new { a = j2.a, b = j2.b, c = j2.c, d });

                    var visibilityListRolesRemoved = await j3RolesRemoved.Where(x => x.b.THING == idProject && 
                            !this._dbContext.SecurityEntities.Any(sec => sec.THING == x.b.THING &&
                            sec.PERSONORGROUP == x.b.PERSONORGROUP && sec.ACCESSRIGHTS > 20))
                        .Distinct()
                        .Select(x => new
                        {
                            SYSTEM_ID = x.a.SYSTEM_ID,
                            VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                            ID_REGISTRO = x.a.ID_REGISTRO,
                            ID_AMM = x.a.ID_AMM,
                            VAR_DESC_RUOLO = x.c.VAR_DESC_RUOLO,
                            CHA_TIPO_DIRITTO = x.b.CHA_TIPO_DIRITTO,
                            ACCESSRIGHTS = x.b.ACCESSRIGHTS,
                            ID_UO = x.a.ID_UO,
                            NOTE = x.b.NOTE,
                            PERSONORGROUP = x.b.PERSONORGROUP,
                            CHA_TIPO_CORR = x.a.CHA_TIPO_CORR,
                            CHA_TIPO_IE = x.a.CHA_TIPO_IE,
                            CHA_TIPO_URP = x.a.CHA_TIPO_URP,
                            VAR_DESC_CORR = x.a.VAR_DESC_CORR,
                            DESC_UO = x.d.VAR_DESC_CORR,
                            CHA_TIPO_UO = x.d.CHA_TIPO_IE,
                            VAR_COD_UO = x.d.VAR_COD_RUBRICA,
                            ShowHistory = 0,
                            ORIGINAL_ID = x.a.ORIGINAL_ID,
                            DTA_FINE = x.a.DTA_FINE
                        })
                        .OrderByDescending(x => x.ACCESSRIGHTS)
                        .ThenBy(x => x.ID_UO)
                        .ToListAsync();

                    foreach (var r in visibilityListRolesRemoved)
                    {
                        int showHistory = await GetShowHistory(r.ORIGINAL_ID);

                        DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto()
                        {
                            idObj = idProject.ToString(),
                            tipoDiritto = GetDiritto(r.CHA_TIPO_DIRITTO),
                            accessRights = Convert.ToInt32(r.ACCESSRIGHTS),
                            deleted = true,
                            personorgroup = r.PERSONORGROUP.ToString(),
                            noteSecurity = r.NOTE,
                            rootFolder = rootFolder
                        };

                        DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo()
                        {
                            systemId = r.SYSTEM_ID.ToString(),
                            dta_fine = r.DTA_FINE != null && r.DTA_FINE.HasValue ? r.DTA_FINE.ToString() : string.Empty,
                            codiceRubrica = r.VAR_COD_RUBRICA ?? String.Empty,
                            ShowHistory = showHistory != 0 ? showHistory.ToString() : string.Empty,
                            descrizione = r.VAR_DESC_CORR ?? string.Empty,
                            tipoCorrispondente = r.CHA_TIPO_URP ?? string.Empty,
                            tipoIE = r.CHA_TIPO_IE ?? string.Empty,
                            idGruppo = r.PERSONORGROUP.ToString()
                        };

                        DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro()
                        {
                            systemId = r.ID_REGISTRO?.ToString()
                        };

                        ruolo.registri = new DocsPaVO.utente.Registro[] { reg };

                        if (r.ID_UO != null)
                        {
                            ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa()
                            {
                                systemId = r.ID_UO.ToString(),
                                idAmministrazione = r.ID_AMM != null ? r.ID_AMM.ToString() : string.Empty,
                                descrizione = r.DESC_UO ?? string.Empty,
                                tipoIE = r.CHA_TIPO_UO ?? string.Empty,
                                codiceRubrica = r.VAR_COD_UO ?? string.Empty,
                            };
                        }

                        if (r.ID_AMM != null)
                        {
                            ruolo.idAmministrazione = r.ID_AMM.ToString();
                            IDAMM = ruolo.idAmministrazione;
                        }

                        dirittoOggetto.soggetto = ruolo;
                        listaDiritti.Add(dirittoOggetto);
                    }

                    #endregion

                    #region Utenti rimossi
                    var j1UsersRemoved = this._dbContext.PeopleEntities.Join(this._dbContext.DeletedSecurityEntities, a => a.SYSTEM_ID, b => b.PERSONORGROUP, (a, b) => new { a, b });
                    var j2UsersRemoved = j1UsersRemoved.Join(this._dbContext.CorrGlobaliEntities, j1 => j1.a.SYSTEM_ID, c => c.ID_PEOPLE, (j1, c) => new { a = j1.a, b = j1.b, c });

                    var visibilityListUsersRemoved = await j2UsersRemoved.Where(x => x.b.THING == idProject && !x.c.CHA_TIPO_URP.Equals("L"))
                        .Select(x => new
                        {
                            SYSTEM_ID = x.c.SYSTEM_ID,
                            ID_REGISTRO = x.c.ID_REGISTRO,
                            ID_AMM = x.c.ID_AMM,
                            VAR_NOME = x.a.VAR_NOME,
                            VAR_COGNOME = x.a.VAR_COGNOME,
                            VAR_COD_RUBRICA = x.c.VAR_COD_RUBRICA,
                            CHA_TIPO_DIRITTO = x.b.CHA_TIPO_DIRITTO,
                            PERSONORGROUP = x.a.SYSTEM_ID,
                            ACCESSRIGHTS = x.b.ACCESSRIGHTS,
                            DISABLED = x.a.DISABLED,
                            CHA_TIPO_CORR = x.c.CHA_TIPO_CORR,
                            CHA_TIPO_IE = x.c.CHA_TIPO_IE,
                            CHA_TIPO_URP = x.c.CHA_TIPO_URP,
                            VAR_DESC_CORR = x.c.VAR_DESC_CORR
                        })
                        .ToListAsync();

                    foreach (var u in visibilityListUsersRemoved)
                    {
                        DocsPaVO.utente.Utente utenteTemp = new DocsPaVO.utente.Utente()
                        {
                            idPeople = u.PERSONORGROUP.ToString(),
                            codiceRubrica = u.VAR_COD_RUBRICA,
                            idAmministrazione = u.ID_AMM?.ToString() ?? string.Empty,
                            descrizione = u.VAR_DESC_CORR,
                            idRegistro = u.ID_REGISTRO?.ToString() ?? string.Empty,
                            tipoCorrispondente = u.CHA_TIPO_URP?.ToString() ?? string.Empty,
                            tipoIE = u.CHA_TIPO_IE?.ToString() ?? string.Empty
                        };

                        DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto()
                        {
                            idObj = idProject.ToString(),
                            soggetto = utenteTemp,
                            tipoDiritto = GetDiritto(u.CHA_TIPO_DIRITTO),
                            accessRights = Convert.ToInt32(u.ACCESSRIGHTS),
                            deleted = true,
                            personorgroup = u.PERSONORGROUP.ToString(),
                            rootFolder = rootFolder
                        };

                        if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO))
                        {
                            dirittoOggetto.soggetto = utenteTemp;
                            dirittoOggetto.soggetto.descrizione = utenteTemp.descrizione + " sostituto di " + utenteTemp.descrizione;
                        }

                        listaDiritti.Add(dirittoOggetto);
                    }

                    #endregion
                } 
                #endregion
            }
            catch (Exception ex)
            {
                listaDiritti = null;
                _logger.LogError(ex, null, null);
            }
            return new FascicolazioneGetVisibilitaSemplificataResult(listaDiritti.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetVisibilitaSemplificataHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IMediator _mediator;

        private Task<int> GetShowHistory(long? originalId)
        {
            return this._dbContext.RoleHistoryEntities.Where(x => x.ORIGINAL_CORR_ID == originalId && !x.ACTION.Equals("C")).CountAsync();
        }
        private DocsPaVO.fascicolazione.TipoDiritto GetDiritto(string tipoDiritto)
        {
            switch (tipoDiritto)
            {
                case "P":
                    return DocsPaVO.fascicolazione.TipoDiritto.TIPO_PROPRIETARIO;
                case "T":
                    return DocsPaVO.fascicolazione.TipoDiritto.TIPO_TRASMISSIONE;
                case "S":
                    return DocsPaVO.fascicolazione.TipoDiritto.TIPO_SOSPESO;
                case "D":
                    return DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO;
                default:
                    return DocsPaVO.fascicolazione.TipoDiritto.TIPO_ACQUISITO;
            }
        }

        #endregion
    }

}
