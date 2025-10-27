// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.Exceptions;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Handlers.DocumentoGetVisibilitaSemplificata
{

    // Richiede libreria MediatR
    public class DocumentoGetVisibilitaSemplificataHandler : IRequestHandler<Application.Requests.DocumentoGetVisibilitaSemplificata, DocumentoGetVisibilitaSemplificataResult>
    {
        #region Public Members

        public DocumentoGetVisibilitaSemplificataHandler(
            ILogger<DocumentoGetVisibilitaSemplificataHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext, 
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<DocumentoGetVisibilitaSemplificataResult> Handle(Application.Requests.DocumentoGetVisibilitaSemplificata request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.documento.DirittoOggetto> listaDiritti = new List<DirittoOggetto>();

            long idProfile = request.idProfile.AsLong();
            bool cercaRimossi = request.cercaRimossi;
            string idAmm = string.Empty;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            //1-- inserimento ruoli
            try
            {
                var q1 = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.SecurityEntities, cg1 => cg1.ID_GRUPPO, s => s.PERSONORGROUP, (cg1, s) => new { cg1, s });
                var q2 = q1.Join(this._dbContext.TipoRuoloEntities, q1 => q1.cg1.ID_TIPO_RUOLO, tr => tr.SYSTEM_ID, (q1, tr) => new { q1.cg1, q1.s, tr });
                var q3 = q2.Join(this._dbContext.CorrGlobaliEntities, q2 => q2.cg1.ID_UO, cg2 => cg2.SYSTEM_ID, (q2, cg2) => new { q2.cg1, q2.s, q2.tr, cg2 });

                var roleList = await q3.Where(x => !this._dbContext.ProfileEntities.Any(p => p.SYSTEM_ID == x.s.THING && p.CHA_IN_CESTINO != null && p.CHA_IN_CESTINO.Equals("1")) &&
                    x.s.THING == idProfile &&
                    (x.s.ACCESSRIGHTS >= 0 || x.s.CHA_TIPO_DIRITTO.Equals("P")))
                    .Distinct()
                    .Select(x => new
                    {
                        SYSTEM_ID = x.cg1.SYSTEM_ID,
                        VAR_COD_RUBRICA = x.cg1.VAR_COD_RUBRICA,
                        ID_REGISTRO = x.cg1.ID_REGISTRO,
                        ID_AMM = x.cg1.ID_AMM,
                        VAR_DESC_RUOLO = x.tr.VAR_DESC_RUOLO,
                        CHA_TIPO_DIRITTO = x.s.CHA_TIPO_DIRITTO,
                        PERSONORGROUP = x.s.PERSONORGROUP,
                        HIDE_DOC_VERSIONS = x.s.HIDE_DOC_VERSIONS,
                        ACCESSRIGHTS = x.s.ACCESSRIGHTS,
                        ID_UO = x.cg1.ID_UO,
                        CHA_TIPO_CORR = x.cg1.CHA_TIPO_CORR,
                        CHA_TIPO_IE = x.cg1.CHA_TIPO_IE,
                        CHA_TIPO_URP = x.cg1.CHA_TIPO_URP,
                        VAR_DESC_CORR = x.cg1.VAR_DESC_CORR,
                        DESC_UO = x.cg2.VAR_DESC_CORR,
                        CHA_TIPO_UO = x.cg2.CHA_TIPO_IE,
                        VAR_COD_UO = x.cg2.VAR_COD_RUBRICA,
                        TS_INSERIMENTO = x.s.TS_INSERIMENTO,
                        VAR_NOTE_SEC = x.s.VAR_NOTE_SEC,
                        ORIGINAL_ID = x.cg1.ORIGINAL_ID,
                        //SHOWHISTORY = GetShowHistory(x.cg1.ORIGINAL_ID).Result,
                        DTA_FINE = x.cg1.DTA_FINE
                    })
                    .OrderByDescending(x => x.ACCESSRIGHTS)
                    .ThenBy(x => x.ID_UO)
                    .ToListAsync();

                foreach (var r in roleList)
                {
                    DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto()
                    {
                        idObj = idProfile.ToString(),
                        tipoDiritto = GetDiritto(r.CHA_TIPO_DIRITTO),
                        accessRights = Convert.ToInt32(r.ACCESSRIGHTS),
                        deleted = false,
                        personorgroup = r.PERSONORGROUP?.ToString(),
                        hideDocVersions = !string.IsNullOrEmpty(r.HIDE_DOC_VERSIONS) && r.HIDE_DOC_VERSIONS.ToString().Equals("1"),
                        dtaInsSecurity = r.TS_INSERIMENTO.HasValue ? r.TS_INSERIMENTO.ToString() : string.Empty,
                        noteSecurity = r.VAR_NOTE_SEC
                    };

                    int showHistory = await GetShowHistory(r.ORIGINAL_ID);

                    DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo()
                    {
                        systemId = r.SYSTEM_ID.ToString(),
                        dta_fine = r.DTA_FINE != null && r.DTA_FINE.HasValue ? r.DTA_FINE.ToString() : string.Empty,
                        codiceRubrica = r.VAR_COD_RUBRICA ?? String.Empty,
                        ShowHistory = showHistory != 0 ? showHistory.ToString() : string.Empty,
                        //idAmministrazione = x.ID_AMM != null ? x.ID_AMM.ToString() : string.Empty,
                        descrizione = r.VAR_DESC_CORR ?? string.Empty,
                        tipoCorrispondente = r.CHA_TIPO_URP ?? string.Empty,
                        tipoIE = r.CHA_TIPO_IE ?? string.Empty,
                        idGruppo = r.PERSONORGROUP?.ToString()
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
                        idAmm = ruolo.idAmministrazione;
                    }

                    dirittoOggetto.soggetto = ruolo;

                    listaDiritti.Add(dirittoOggetto);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Errore nella gestione DIRITTI_RUOLI SEMPLIFICATI: {0}", ex);
                return null;
            }

            //2 Inserimento utenti
            try
            {
                if (string.IsNullOrEmpty(idAmm))
                    idAmm = idTenant.ToString();

                var q1 = this._dbContext.PeopleEntities.Join(this._dbContext.SecurityEntities, p => p.SYSTEM_ID, s => s.PERSONORGROUP, (p, s) => new { p, s });
                var q2 = q1.Join(this._dbContext.CorrGlobaliEntities, q1 => q1.p.SYSTEM_ID, cg => cg.ID_PEOPLE, (q1, cg) => new { q1.p, q1.s, cg });

                var userList = await q2
                    .Where(x => x.s.THING == idProfile && (x.s.ACCESSRIGHTS > 0 || x.s.CHA_TIPO_DIRITTO.Equals("P")) && x.cg.ID_AMM == idAmm.AsLong())
                    .Select(x => new
                    {
                        SYSTEM_ID = x.cg.SYSTEM_ID,
                        ID_REGISTRO = x.cg.ID_REGISTRO,
                        ID_AMM = x.cg.ID_AMM,
                        VAR_NOME = x.p.VAR_NOME,
                        VAR_COGNOME = x.p.VAR_COGNOME,
                        VAR_COD_RUBRICA = x.cg.VAR_COD_RUBRICA,
                        CHA_TIPO_DIRITTO = x.s.CHA_TIPO_DIRITTO,
                        ACCESSRIGHTS = x.s.ACCESSRIGHTS,
                        PERSONORGROUP = x.p.SYSTEM_ID,
                        HIDE_DOC_VERSIONS = x.s.HIDE_DOC_VERSIONS,
                        CHA_TIPO_CORR = x.cg.CHA_TIPO_CORR,
                        CHA_TIPO_IE = x.cg.CHA_TIPO_IE,
                        CHA_TIPO_URP = x.cg.CHA_TIPO_URP,
                        VAR_DESC_CORR = x.cg.VAR_DESC_CORR
                    })
                    .Distinct()
                    .ToListAsync();

                userList.ForEach(u =>
                {
                    DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente()
                    {
                        codiceRubrica = u.VAR_COD_RUBRICA,
                        idRegistro = u.ID_REGISTRO?.ToString(),
                        idAmministrazione = u.ID_AMM?.ToString(),
                        descrizione = u.VAR_DESC_CORR?.ToString(),
                        tipoCorrispondente = u.CHA_TIPO_URP?.ToString(),
                        tipoIE = u.CHA_TIPO_IE?.ToString(),
                        idPeople = u.PERSONORGROUP.ToString(),

                    };

                    DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto()
                    {
                        idObj = idProfile.ToString(),
                        soggetto = utente,
                        tipoDiritto = GetDiritto(u.CHA_TIPO_DIRITTO),
                        accessRights = Convert.ToInt32(u.ACCESSRIGHTS),
                        deleted = false,
                        personorgroup = u.PERSONORGROUP.ToString(),
                        hideDocVersions = !string.IsNullOrEmpty(u.HIDE_DOC_VERSIONS) && u.HIDE_DOC_VERSIONS.Equals("1")
                    };

                    if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO))
                        dirittoOggetto.soggetto.descrizione = utente.descrizione + " sostituto di " + utente.descrizione; //?????? Sul vecchio backend è così... controlla

                    listaDiritti.Add(dirittoOggetto);
                });

            }
            catch (Exception ex)
            {
                _logger.LogError("Errore nella gestione DIRITTI_UTENTI, {0}", ex);
                return null;
            }

            //gestione diritti rimossi
            if (cercaRimossi)
            {
                //3-- inserimento ruoli rimossi
                try
                {
                    var q1 = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.DeletedSecurityEntities, cg1 => cg1.ID_GRUPPO, s => s.PERSONORGROUP, (cg1, s) => new { cg1, s });
                    var q2 = q1.Join(this._dbContext.TipoRuoloEntities, q1 => q1.cg1.ID_TIPO_RUOLO, tr => tr.SYSTEM_ID, (q1, tr) => new { q1.cg1, q1.s, tr });
                    var q3 = q2.Join(this._dbContext.CorrGlobaliEntities, q2 => q2.cg1.ID_UO, cg2 => cg2.SYSTEM_ID, (q2, cg2) => new { q2.cg1, q2.s, q2.tr, cg2 });

                    var rolesList = await q3
                        .Where(x => x.s.THING == idProfile &&
                         (x.s.ACCESSRIGHTS >= 0 || x.s.CHA_TIPO_DIRITTO.Equals("P")) &&
                         !this._dbContext.SecurityEntities.Any(sec => sec.THING == x.s.THING &&
                            sec.PERSONORGROUP == x.s.PERSONORGROUP && sec.ACCESSRIGHTS > 20)
                         )
                        .Select(x => new
                        {
                            SYSTEM_ID = x.cg1.SYSTEM_ID,
                            VAR_COD_RUBRICA = x.cg1.VAR_COD_RUBRICA,
                            ID_REGISTRO = x.cg1.ID_REGISTRO,
                            ID_AMM = x.cg1.ID_AMM,
                            VAR_DESC_RUOLO = x.tr.VAR_DESC_RUOLO,
                            CHA_TIPO_DIRITTO = x.s.CHA_TIPO_DIRITTO,
                            NOTE = x.s.NOTE,
                            PERSONORGROUP = x.s.PERSONORGROUP,
                            HIDE_DOC_VERSIONS = x.s.HIDE_DOC_VERSIONS,
                            ACCESSRIGHTS = x.s.ACCESSRIGHTS,
                            ID_UO = x.cg1.ID_UO,
                            CHA_TIPO_CORR = x.cg1.CHA_TIPO_CORR,
                            CHA_TIPO_IE = x.cg1.CHA_TIPO_IE,
                            CHA_TIPO_URP = x.cg1.CHA_TIPO_URP,
                            VAR_DESC_CORR = x.cg1.VAR_DESC_CORR,
                            DESC_UO = x.cg2.VAR_DESC_CORR,
                            CHA_TIPO_UO = x.cg2.CHA_TIPO_IE,
                            VAR_COD_UO = x.cg2.VAR_COD_RUBRICA
                        })
                        .Distinct()
                        .OrderByDescending(x => x.ACCESSRIGHTS)
                        .ThenBy(x => x.ID_UO)
                        .ToListAsync();

                    rolesList.ForEach(r =>
                    {
                        DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo()
                        {
                            codiceRubrica = r.VAR_COD_RUBRICA ?? string.Empty,
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

                        if (r.ID_AMM != null)
                        {
                            ruolo.idAmministrazione = r.ID_AMM.ToString();
                            idAmm = ruolo.idAmministrazione;
                        }

                        if (r.ID_UO != null)
                        {
                            ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa()
                            {
                                systemId = r.ID_UO.ToString(),
                                idAmministrazione = idAmm,
                                descrizione = r.DESC_UO ?? string.Empty,
                                tipoIE = r.CHA_TIPO_UO ?? string.Empty,
                                codiceRubrica = r.VAR_COD_UO ?? string.Empty,
                            };
                        }

                        DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto()
                        {
                            idObj = idProfile.ToString(),
                            soggetto = ruolo,
                            tipoDiritto = GetDiritto(r.CHA_TIPO_DIRITTO),
                            accessRights = Convert.ToInt32(r.ACCESSRIGHTS),
                            deleted = true,
                            personorgroup = r.PERSONORGROUP.ToString(),
                            hideDocVersions = !string.IsNullOrEmpty(r.HIDE_DOC_VERSIONS) && r.HIDE_DOC_VERSIONS.ToString().Equals("1")
                        };

                        listaDiritti.Add(dirittoOggetto);
                    });

                }
                catch (Exception ex)
                {
                    _logger.LogError("Errore nella gestione DIRITTI_RUOLI_DELETED, {0}", ex);
                    return null;
                }

                //4-- inserimento utenti rimossi
                try
                {
                    if (string.IsNullOrEmpty(idAmm))
                        idAmm = idTenant.ToString();

                    ArrayList utentiInt = new ArrayList();
                    DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                    DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();

                    var q1 = this._dbContext.PeopleEntities.Join(this._dbContext.DeletedSecurityEntities, p => p.SYSTEM_ID, s => s.PERSONORGROUP, (p, s) => new { p, s });
                    var q2 = q1.Join(this._dbContext.CorrGlobaliEntities, q1 => q1.p.SYSTEM_ID, cg => cg.ID_PEOPLE, (q1, cg) => new { q1.p, q1.s, cg });

                    var usersList = await q2
                        .Where(x => x.s.THING == idProfile && (x.s.ACCESSRIGHTS > 0 || x.s.CHA_TIPO_DIRITTO.Equals("P")) && x.cg.ID_AMM == idAmm.AsLong())
                        .Select(x => new
                        {
                            SYSTEM_ID = x.cg.SYSTEM_ID,
                            ID_REGISTRO = x.cg.ID_REGISTRO,
                            ID_AMM = x.cg.ID_AMM,
                            VAR_NOME = x.p.VAR_NOME,
                            VAR_COGNOME = x.p.VAR_COGNOME,
                            VAR_COD_RUBRICA = x.cg.VAR_COD_RUBRICA,
                            CHA_TIPO_DIRITTO = x.s.CHA_TIPO_DIRITTO,
                            ACCESSRIGHTS = x.s.ACCESSRIGHTS,
                            NOTE = x.s.NOTE,
                            PERSONORGROUP = x.s.PERSONORGROUP,
                            HIDE_DOC_VERSIONS = x.s.HIDE_DOC_VERSIONS
                        })
                        .ToListAsync();

                    foreach (var u in usersList)
                    {
                        var corrInt = await this._dbContext.CorrGlobaliEntities
                            .Where(x => x.CHA_TIPO_IE.Equals("I") && x.CHA_TIPO_CORR.Equals("S") &&
                            (x.ID_AMM == null || x.ID_AMM == idAmm.AsLong()) && x.VAR_COD_RUBRICA.Equals(u.VAR_COD_RUBRICA))
                            .FirstOrDefaultAsync();

                        if (corrInt != null)
                        {
                            var q1U = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.PeopleGroupEntities, a => a.ID_PEOPLE, b => b.PEOPLE_SYSTEM_ID, (a, b) => new { a, b });
                            var q2U = q1U.Join(this._dbContext.CorrGlobaliEntities, q1 => q1.b.GROUPS_SYSTEM_ID, c => c.ID_GRUPPO, (q1, c) => new { q1.a, q1.b, c });
                            var q3 = q2U.Join(this._dbContext.PeopleEntities, q2 => q2.a.ID_PEOPLE, e => e.SYSTEM_ID, (q2, e) => new { q2.a, q2.b, q2.c, e });
                            var q4 = q3.Join(this._dbContext.TipoRuoloEntities, q3 => q3.c.ID_TIPO_RUOLO, d => d.SYSTEM_ID, (q3, d) => new { q3.a, q3.b, q3.c, q3.e, d });

                            var corrList = await q4.Where(x => x.a.VAR_COD_RUBRICA.Equals(u.VAR_COD_RUBRICA) && (x.a.ID_AMM == null || x.a.ID_AMM == idAmm.AsLong()) && x.a.CHA_TIPO_CORR.Equals("S"))
                                .Select(x => new
                                {
                                    SYSTEM_ID = x.a.SYSTEM_ID,
                                    ID_PEOPLE = x.a.ID_PEOPLE,
                                    ID_REGISTRO = x.a.ID_REGISTRO,
                                    ID_AMM = x.a.ID_AMM,
                                    VAR_NOME = x.e.VAR_NOME,
                                    VAR_COGNOME = x.e.VAR_COGNOME,
                                    EMAIL_ADDRESS = x.e.EMAIL_ADDRESS,
                                    CHA_NOTIFICA = x.e.CHA_NOTIFICA,
                                    VAR_TELEFONO = x.e.VAR_TELEFONO,
                                    VAR_DESC_CORR = x.a.VAR_DESC_CORR,
                                    VAR_CODICE = x.a.VAR_CODICE,
                                    VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                                    CHA_DETTAGLI = x.a.CHA_DETTAGLI,
                                    CHA_TIPO_URP = x.a.CHA_TIPO_URP,
                                    VAR_SMTP = x.a.VAR_SMTP,
                                    NUM_PORTA_SMTP = x.a.NUM_PORTA_SMTP,
                                    VAR_CODICE_AMM = x.a.VAR_CODICE_AMM,
                                    VAR_CODICE_AOO = x.a.VAR_CODICE_AOO,
                                    DTA_FINE = x.a.DTA_FINE,
                                    RUOLO_SYSTEM_ID = x.c.SYSTEM_ID,
                                    RUOLO_DESC = x.d.VAR_DESC_RUOLO,
                                    RUOLO_CODICE = x.c.VAR_CODICE,
                                    RUOLO_ID_UO = x.c.ID_UO,
                                    RUOLO_COD_RUBRICA = x.c.VAR_COD_RUBRICA,
                                    RUOLO_DETTAGLI = x.c.CHA_DETTAGLI,
                                    CHA_NOTIFICA_CON_ALLEGATO = x.e.CHA_NOTIFICA_CON_ALLEGATO,
                                    DISABLED = x.e.DISABLED,
                                    VAR_SEDE = x.e.VAR_SEDE
                                }).ToListAsync();

                            string useConnectByPriorOrWith = await this._configurationService.GetValue<string>("USA_CONNECTBYPRIOR_OR_WITH");

                            foreach (var c in corrList)
                            {
                                List<UnitaOrganizzativa> uoList = new List<UnitaOrganizzativa>();
                                UnitaOrganizzativa uo = new UnitaOrganizzativa();

                                if (!string.IsNullOrEmpty(useConnectByPriorOrWith) && useConnectByPriorOrWith.Equals("1"))
                                {
                                    var tmp = this._dbContext.CorrGlobaliEntities
                                            .Where(x => x.CHA_TIPO_URP.Equals("U") && x.CHA_TIPO_IE.Equals("I") && x.ID_AMM == idAmm.AsLong());

                                    var list = await GetUoHierarchy(c.RUOLO_ID_UO, new List<CorrGlobaliEntity>(), tmp);

                                    foreach (var x in list)
                                    {
                                        uoList.Add(new UnitaOrganizzativa()
                                        {
                                            systemId = x.SYSTEM_ID.ToString(),
                                            descrizione = x.VAR_DESC_CORR,
                                            codiceCorrispondente = x.VAR_CODICE,
                                            codiceRubrica = x.VAR_COD_RUBRICA,
                                            livello = x.NUM_LIVELLO.ToString(),
                                            codiceAOO = x.VAR_CODICE_AOO,
                                            codiceAmm = x.VAR_CODICE_AMM,
                                            idRegistro = x.ID_REGISTRO.ToString(),
                                            codiceIstat = x.VAR_CODICE_ISTAT,
                                            idAmministrazione = x.ID_AMM.ToString(),
                                            dettagli = !string.IsNullOrEmpty(x.CHA_DETTAGLI) && x.CHA_DETTAGLI.Equals("1"),
                                            tipoIE = "I",
                                            tipoCorrispondente = "P",
                                            email = x.VAR_EMAIL,
                                            interoperante = !string.IsNullOrEmpty(x.CHA_PA) && x.CHA_PA.Equals("1"),
                                            classificaUO = x.CLASSIFICA_UO ?? string.Empty,
                                            parent = new UnitaOrganizzativa()
                                            {
                                                systemId = x.ID_PARENT != null ? x.ID_PARENT.ToString() :string.Empty
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    uoList = await this._dbContext.CorrGlobaliEntities.SelectMany(a => this._dbContext.CorrGlobaliEntities, (a, b) => new { a, b })
                                        .Where(x => x.a.CHA_TIPO_URP.Equals("U") && x.a.CHA_TIPO_IE.Equals("I") &&
                                            x.b.SYSTEM_ID == c.RUOLO_ID_UO && x.a.NUM_LIVELLO <= x.b.NUM_LIVELLO && x.a.ID_AMM == idAmm.AsLong())

                                        .Select(x => new UnitaOrganizzativa()
                                        {
                                            systemId = x.a.SYSTEM_ID.ToString(),
                                            descrizione = x.a.VAR_DESC_CORR,
                                            codiceCorrispondente = x.a.VAR_CODICE,
                                            codiceRubrica = x.a.VAR_COD_RUBRICA,
                                            livello = x.a.NUM_LIVELLO.ToString(),
                                            codiceAOO = x.a.VAR_CODICE_AOO,
                                            codiceAmm = x.a.VAR_CODICE_AMM,
                                            idRegistro = x.a.ID_REGISTRO.ToString(),
                                            codiceIstat = x.a.VAR_CODICE_ISTAT,
                                            idAmministrazione = x.a.ID_AMM.ToString(),
                                            dettagli = !string.IsNullOrEmpty(x.a.CHA_DETTAGLI) && x.a.CHA_DETTAGLI.Equals("1"),
                                            tipoIE = "I",
                                            tipoCorrispondente = "P",
                                            email = x.a.VAR_EMAIL,
                                            interoperante = !string.IsNullOrEmpty(x.a.CHA_PA) && x.a.CHA_PA.Equals("1"),
                                            classificaUO = x.a.CLASSIFICA_UO ?? string.Empty,
                                            parent = new UnitaOrganizzativa()
                                            {
                                                systemId = x.a.ID_PARENT != null ? x.a.ID_PARENT.ToString() : string.Empty
                                            }
                                        })
                                        .OrderBy(x => x.descrizione)
                                    .ToListAsync();
                                }

                                uo = GetUoParent(c.RUOLO_ID_UO, uoList, new UnitaOrganizzativa());

                                DocsPaVO.utente.Utente corrispondenteUtente = new DocsPaVO.utente.Utente()
                                {
                                    systemId = c.SYSTEM_ID.ToString(),
                                    idPeople = c.ID_PEOPLE.ToString(),
                                    descrizione = c.VAR_DESC_CORR,
                                    codiceCorrispondente = c.VAR_CODICE,
                                    codiceRubrica = c.VAR_COD_RUBRICA,
                                    dta_fine = c.DTA_FINE.HasValue ? c.DTA_FINE.Value.ToString() : string.Empty,
                                    disabilitato = c.DISABLED,
                                    idAmministrazione = c.ID_AMM.ToString(),
                                    codiceAOO = c.VAR_CODICE_AOO,
                                    codiceAmm = c.VAR_CODICE_AMM,
                                    idRegistro = c.ID_REGISTRO?.ToString(),
                                    dettagli = !string.IsNullOrEmpty(c.CHA_DETTAGLI) && c.CHA_DETTAGLI.Equals("1"),
                                    email = c.EMAIL_ADDRESS,
                                    notifica = c.CHA_NOTIFICA,
                                    telefono = c.VAR_TELEFONO,
                                    tipoIE = "I",
                                    tipoCorrispondente = "P",
                                    notificaConAllegato = !string.IsNullOrEmpty(c.CHA_NOTIFICA_CON_ALLEGATO) && c.CHA_NOTIFICA_CON_ALLEGATO.Equals("1"),
                                    sede = c.VAR_SEDE ?? string.Empty
                                };

                                corrispondenteUtente.serverPosta = new DocsPaVO.utente.ServerPosta()
                                {
                                    serverSMTP = c.VAR_SMTP,
                                    portaSMTP = c.NUM_PORTA_SMTP.ToString()
                                };

                                DocsPaVO.utente.Ruolo ruoloUtente = new DocsPaVO.utente.Ruolo()
                                {
                                    systemId = c.RUOLO_SYSTEM_ID.ToString(),
                                    descrizione = c.RUOLO_DESC,
                                    codiceCorrispondente = c.RUOLO_CODICE,
                                    codiceRubrica = c.RUOLO_COD_RUBRICA,
                                    dettagli = !string.IsNullOrEmpty(c.RUOLO_DETTAGLI) && c.RUOLO_DETTAGLI.Equals("1"),
                                    uo = uo
                                };

                                corrispondenteUtente.ruoli = new Ruolo[] { ruoloUtente };

                                utentiInt.Add(corrispondenteUtente);
                            }

                            if (utentiInt != null && utentiInt.Count > 0)
                            {
                                utente = (DocsPaVO.utente.Utente)utentiInt[0];
                                dirittoOggetto.idObj = idProfile.ToString();
                                dirittoOggetto.soggetto = utente;
                                dirittoOggetto.tipoDiritto = GetDiritto(u.CHA_TIPO_DIRITTO.ToString());
                                dirittoOggetto.accessRights = Convert.ToInt32(u.ACCESSRIGHTS);
                                dirittoOggetto.deleted = true;
                                dirittoOggetto.note = u.NOTE.ToString();
                                dirittoOggetto.personorgroup = u.PERSONORGROUP.ToString();
                                dirittoOggetto.hideDocVersions = !string.IsNullOrEmpty(u.HIDE_DOC_VERSIONS) && u.HIDE_DOC_VERSIONS.Equals("1");

                                listaDiritti.Add(dirittoOggetto);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Errore nella gestione DIRITTI_UTENTI_DELETED, {0}", ex);
                    return null;
                }
            }

            return new DocumentoGetVisibilitaSemplificataResult(listaDiritti.ToArray());
        }

        private async Task<List<CorrGlobaliEntity>> GetUoHierarchy(long? idUo, List<CorrGlobaliEntity> uoListToReturn, IQueryable<CorrGlobaliEntity> uoList)
        {
            if (uoList.Any())
            {
                var uo = uoList.Where(x => x.SYSTEM_ID == idUo).FirstOrDefault();
                if (uo != null)
                {
                    uoListToReturn.Add(uo);
                    return await GetUoHierarchy(uo.ID_PARENT, uoListToReturn, uoList);
                }
            }
            return uoListToReturn;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetVisibilitaSemplificataHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        private UnitaOrganizzativa GetUoParent(long? idParent, List<UnitaOrganizzativa> uoList, UnitaOrganizzativa uoToReturn)
        {
            UnitaOrganizzativa uo = new UnitaOrganizzativa();

            if (uoList.Any())
            {
                uo = uoList.Where(x => x.systemId.Equals(idParent.ToString())).FirstOrDefault();

                if (uo != null && !uo.parent.systemId.Equals("0"))
                {
                    uoToReturn = new DocsPaVO.utente.UnitaOrganizzativa()
                    {
                        systemId = uo.systemId,
                        descrizione = uo.descrizione,
                        codiceCorrispondente = uo.codiceCorrispondente,
                        codiceRubrica = uo.codiceRubrica,
                        livello = uo.livello,
                        codiceAOO = uo.codiceAOO,
                        codiceAmm = uo.codiceAmm,
                        idRegistro = uo.idRegistro,
                        codiceIstat = uo.codiceIstat,
                        idAmministrazione = uo.idAmministrazione,
                        dettagli = uo.dettagli,
                        tipoIE = "I",
                        tipoCorrispondente = "P",
                        email = uo.email,
                        interoperante = uo.interoperante,
                        classificaUO = uo.classificaUO,
                        serverPosta = uo.serverPosta,
                        parent = new UnitaOrganizzativa()
                        {
                            systemId = uo.parent.systemId
                        }
                    };

                    return GetUoParent(uoToReturn.parent.systemId.AsLong(), uoList, uoToReturn);
                }
            }
            return uoToReturn;
        }

        private TipoDiritto GetDiritto(string tipoDiritto)
        {
            switch (tipoDiritto)
            {
                case "P":
                    return DocsPaVO.documento.TipoDiritto.TIPO_PROPRIETARIO;
                case "T":
                    return DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE;
                case "F":
                    return DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO;
                case "S":
                    return DocsPaVO.documento.TipoDiritto.TIPO_SOSPESO;
                case "D":
                    return DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO;
                case "C":
                    return DocsPaVO.documento.TipoDiritto.TIPO_CONSERVAZIONE;
                default:
                    return DocsPaVO.documento.TipoDiritto.TIPO_ACQUISITO;
            }
        }

        private Task<int> GetShowHistory(long? originalId)
        {
            return this._dbContext.RoleHistoryEntities.Where(x => x.ORIGINAL_CORR_ID == originalId && !x.ACTION.Equals("C")).CountAsync();
        }

        #endregion
    }
}
