// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.rubrica;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using CallTypes = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType;
using RubricaGetElementiRubricaRequest = Pi3.App.Legacy.WebApi.Application.Requests.rubricaGetElementiRubrica;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.rubricaGetElementiRubrica
{
    public class RubricaGetElementiRubricaHandler : IRequestHandler<RubricaGetElementiRubricaRequest, rubricaGetElementiRubricaResult>
    {
        #region Public members
        public RubricaGetElementiRubricaHandler(ILogger<RubricaGetElementiRubricaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, 
            IPi3DbContext dbContext, IRubricaComuneService rubricaComuneService, IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._rubricaComuneService = rubricaComuneService;
            this._httpContextAccessor = httpContextAccessor;

        }
        public async Task<rubricaGetElementiRubricaResult> Handle(RubricaGetElementiRubricaRequest request, CancellationToken cancellationToken)
        {
            List<ElementoRubrica> output = null;
            try
            {
                //A volte l'idGruppo non corrisponde all'idGruppo ma all'idCorrGlobali...pezza
                if(!string.IsNullOrEmpty(request.u.idCorrGlobali) && !string.IsNullOrEmpty(request.u.idGruppo) && request.u.idGruppo == request.u.idCorrGlobali)
                {
                    var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                    request.u.idGruppo = idGruppo != null && idGruppo > 0 ? idGruppo.ToString() : (await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == request.u.idCorrGlobali.AsLong())
                        .Select(c => c.ID_GRUPPO)
                        .FirstOrDefaultAsync())
                        ?.ToString();
                }

                output = await this.RicercaInRubricaStandard(request.qc, request.u);

                if (string.IsNullOrWhiteSpace(request.qc.localita) &&
                    request.qc.tipoIE != DocsPaVO.addressbook.TipoUtente.INTERNO &&
                    request.qc.doRubricaComune
                    )
                {
                    var rubricaComuneElements = await this.RicercaInRubricaComune(request.qc,request.u);

                    if (rubricaComuneElements.Any()) 
                        output.AddRange(rubricaComuneElements);

                    output = output.OrderBy(a => a.descrizione.ToUpper().Trim()).ToList();

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new rubricaGetElementiRubricaResult(output?.ToArray());
        }
        #endregion

        #region Private members
        protected ILogger<RubricaGetElementiRubricaHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IRubricaComuneService _rubricaComuneService;
        protected IHttpContextAccessor _httpContextAccessor;

        private readonly string BearerPrefix = "Bearer ";

        protected string? GetAuthToken()
        {
            //if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            //var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];
            var authorizationHeader = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            return authorizationHeader;
        }

        protected async Task<List<ElementoRubrica>> RicercaInRubricaStandard(ParametriRicercaRubrica qc, InfoUtente u)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant) ?? u.idAmministrazione;
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup) != null ? this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup) : u.idGruppo.AsLong();

            var elementiRubrica = new List<ElementoRubrica>();

            var predicate = PredicateBuilder.New<CorrGlobaliEntity>();
            bool notOnlyList = false;
            var listePredicate = PredicateBuilder.New<CorrGlobaliEntity>();
            bool notOnlyRf = false;
            var rfPredicate = PredicateBuilder.New<CorrGlobaliEntity>();

            var entities = new List<CorrWithCodReg>();

            if (!string.IsNullOrWhiteSpace( qc.parent))
            {
                var tipoCorr = string.Empty;

                if (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) tipoCorr = "I";
                else if (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO) tipoCorr = "E";

                var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID_AMM == idTenant.AsLong()
                    && x.CHA_TIPO_IE == tipoCorr
                    && x.DTA_FINE == null
                    && x.VAR_COD_RUBRICA == qc.parent);

                if (corrGlobaliEntity?.CHA_TIPO_URP == "U")
                {
                    if (qc.doUo)
                        entities.AddRange(await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(x => x.CHA_TIPO_URP == "U"
                            && x.ID_PARENT == corrGlobaliEntity.SYSTEM_ID
                            && x.DTA_FINE == null).Select(c => new CorrWithCodReg()
                            {
                                Corr = c,
                                CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(c.ID_REGISTRO.GetValueOrDefault())
                            })
                            .ToListAsync());
                    if (qc.doRuoli)
                        entities.AddRange(await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(x => x.CHA_TIPO_URP == "R"
                            && x.ID_UO == corrGlobaliEntity.SYSTEM_ID
                            && x.DTA_FINE == null)
                            .Select(c => new CorrWithCodReg()
                            {
                                Corr = c,
                                CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(c.ID_REGISTRO.GetValueOrDefault())
                            }).ToListAsync());
                }
                else
                {
                    if (qc.doUtenti)
                    {
                        var peopleEntities = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                            .Where(x => x.GROUPS_SYSTEM_ID == corrGlobaliEntity.ID_GRUPPO
                            && x.DTA_FINE == null)
                            .Select(x => x.PEOPLE_SYSTEM_ID)
                            .ToListAsync();

                        entities.AddRange(await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(x => peopleEntities.Any(y => y == x.ID_PEOPLE)
                            && x.CHA_TIPO_URP != "L"
                            && x.DTA_FINE == null).Select(c => new CorrWithCodReg()
                            {
                                Corr = c,
                                CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(c.ID_REGISTRO.GetValueOrDefault())
                            }).ToListAsync());
                    }
                }
            }
            else
            {
                predicate.And(x => x.CHA_TIPO_CORR != "C" && (x.ID_AMM == idTenant.AsLong() || x.ID_AMM == null));
                switch (qc.tipoIE)
                {
                    case DocsPaVO.addressbook.TipoUtente.INTERNO:
                        if (qc.doListe || qc.doRF)
                            predicate.And(x => x.CHA_TIPO_IE == null || x.CHA_TIPO_IE == "I");
                        else
                            predicate.And(x => x.CHA_TIPO_IE == "I");
                        break;
                    case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                        predicate.And(x => x.CHA_TIPO_IE == "E");
                        break;
                }

                await this.ApplyBasicFilters(predicate, qc);

                if (!string.IsNullOrWhiteSpace(qc.systemId))
                    predicate.And(x => x.SYSTEM_ID == qc.systemId.AsLong());

                if (!string.IsNullOrWhiteSpace(qc.noteEmailPISREST))
                    predicate.And(x => this._dbContext.MailCorrEsterniEntities.AsNoTracking().Any
                        (mc => mc.VAR_NOTE.ToUpper().Contains(qc.noteEmailPISREST.ToUpper())
                            && mc.ID_CORR == x.SYSTEM_ID));

                if (!string.IsNullOrWhiteSpace(qc.emailsPISREST))
                    predicate.And(x => this._dbContext.MailCorrEsterniEntities.AsNoTracking().Any(
                        mc => mc.VAR_EMAIL.ToUpper().Contains(qc.emailsPISREST.ToUpper())
                            && mc.ID_CORR == x.SYSTEM_ID));

                if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doRF)
                {
                    var urpTypes = new List<string>();
                    if (qc.doUo) urpTypes.Add("U");
                    if (qc.doRuoli) urpTypes.Add("R");
                    if (qc.doUtenti) urpTypes.Add("P");
                    if (qc.doRF) urpTypes.Add("F");

                    predicate.And(x => urpTypes.Any(y => y == x.CHA_TIPO_URP));
                }

                if (!CheckCalltypesRicercaRuoliDisabilitati(qc.calltype))
                    predicate.And(x => x.DTA_FINE == null);

                if (!qc.extSystems && qc.calltype != CallTypes.CALLTYPE_MITT_MODELLO_TRASM)
                    predicate.And(x => !(x.CHA_SYSTEM_ROLE == "1"));

                if ((qc.calltype == CallTypes.CALLTYPE_RICERCA_CORRISPONDENTE ||
                    qc.calltype == CallTypes.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO) && qc.caller is not null && !string.IsNullOrWhiteSpace(qc.caller.filtroRegistroPerRicerca))
                {
                    var regIds = new List<long>();
                    qc.caller.filtroRegistroPerRicerca.Split(',').ForEach(x => regIds.Add(x.AsLong()));
                    if (regIds.Any()) predicate.And(x => x.ID_REGISTRO == null || regIds.Contains((long)x.ID_REGISTRO));
                }

                if ((CheckCalltypesFiltroRegistro(qc.calltype) &&
                    !(qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) &&
                    qc.caller is not null && !string.IsNullOrWhiteSpace(qc.caller.IdRegistro) &&
                    (qc.doUo || qc.doRuoli || qc.doUtenti))
                    || CheckCallTypesGestioneRubrica(qc.calltype)
                    )
                {
                    var regIds = new List<long>();
                    qc.caller?.filtroRegistroPerRicerca?.Split(',').ForEach(x => regIds.Add(x.AsLong()));

                    bool isRf = false;

                    if (regIds.Count == 1) isRf = (await this._dbContext.RegistroEntities.FirstAsync(x => x.SYSTEM_ID == regIds.First())).CHA_RF == "1";

                    if (regIds.Any() && !isRf)
                        predicate.And(x => x.ID_REGISTRO == null || regIds.Contains((long)x.ID_REGISTRO));
                    if (regIds.Any() && isRf)
                        predicate.And(x => x.ID_REGISTRO == regIds.First());
                    if (!regIds.Any())
                        predicate.And(x => x.ID_REGISTRO == null);
                }

                if ((CheckCallTypesFiltroModelliTrasm(qc.calltype) || CheckCallTypesFiltroRuoloInteropNoMail(qc.calltype))
                    && qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO
                    && qc.caller is not null && !string.IsNullOrWhiteSpace(qc.caller.IdRegistro))
                {
                    var ruoloRegItems = await this._dbContext.RuoloRegistroEntities.AsNoTracking()
                        .Where(x => x.ID_REGISTRO == qc.caller.IdRegistro.AsLong())
                        .Select(x => x.ID_RUOLO_IN_UO).ToListAsync();

                    predicate.And(x => ruoloRegItems.Any(y => y == x.SYSTEM_ID));
                }

                if (qc.calltype == CallTypes.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                {
                    var roleEntities = await this._dbContext.GetChildren(qc.caller.IdRuolo);

                    if (roleEntities.Any())
                        predicate.And(x => roleEntities.Any(y => y.SYSTEM_ID == x.SYSTEM_ID));
                    else
                        predicate.And(x => false);
                }

                if (qc.doListe)
                {
                    if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doRF)
                    {
                        ApplyFilterListeDistribuzione(
                            predicate,
                            idTenant,
                            qc.caller is not null ? qc.caller.IdPeople : string.Empty,
                            u.idGruppo);
                    }
                    else
                    {
                        notOnlyList = true;

                        ApplyFilterListeDistribuzione(
                            listePredicate,
                            idTenant,
                            qc.caller is not null ? qc.caller.IdPeople : string.Empty,
                            u.idGruppo);

                        await this.ApplyBasicFilters(listePredicate, qc);
                    }
                }

                if (qc.doRF)
                {
                    if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doListe)
                    {
                        predicate.And(x => x.ID_AMM == idTenant.AsLong());
                    }
                    else
                    {
                        notOnlyRf = true;
                        rfPredicate.And(x => x.CHA_TIPO_URP == "F" && x.ID_AMM == idTenant.AsLong());

                        await this.ApplyBasicFilters(rfPredicate, qc);
                    }
                }

                try
                {
                    var queryable = this._dbContext.CorrGlobaliEntities.Where(predicate);
                    if (notOnlyList)
                        queryable = queryable.Union(this._dbContext.CorrGlobaliEntities.Where(listePredicate));
                    if (notOnlyRf)
                        queryable = queryable.Union(this._dbContext.CorrGlobaliEntities.Where(rfPredicate));

                    entities = await queryable.OrderByDescending(x => x.CHA_TIPO_URP)
                                .ThenBy(x => x.VAR_DESC_CORR).Select(c => new CorrWithCodReg()
                                {
                                    Corr = c,
                                    CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(c.ID_REGISTRO.GetValueOrDefault())
                                }).ToListAsync();
                }
                catch (Exception ex)
                {
                    throw ;
                }
            }

            foreach (var x in entities)
                elementiRubrica.Add(
                    await this.GetElementoRubrica(x, !string.IsNullOrWhiteSpace(qc.parent)));

            if (CheckCallTypesCheckboxVisibility(qc.calltype))
                elementiRubrica.ForEach(x => x.isVisibile = !x.disabledTrasm);

            return elementiRubrica;
        }

        protected class CorrWithCodReg
        {
            public CorrGlobaliEntity Corr{ get; set; }
            public string? CodRegRf { get; set; }
        }

        protected async Task<List<ElementoRubrica>> RicercaInRubricaComune(ParametriRicercaRubrica qc,InfoUtente user)
        {
            var elementiRubrica = new List<ElementoRubrica>();

            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = qc.codice, TipoRicercaParola = qc.queryCodiceEsatta ? TipiRicercaParolaEnum.ParolaIntera : TipiRicercaParolaEnum.ParteDellaParola  },
                new CriterioRicerca { Campo = CampiRicercaEnum.Denominazione, Valore = qc.descrizione },
                new CriterioRicerca { Campo = CampiRicercaEnum.Citta, Valore = qc.citta },
                //new CriterioRicerca { Campo = CampiRicercaEnum.Email, Valore = qc.email },
                new CriterioRicerca { Campo = CampiRicercaEnum.CodiceFiscale, Valore = qc.codiceFiscale },
                new CriterioRicerca { Campo = CampiRicercaEnum.PartitaIva, Valore = qc.partitaIva }
            };

            if(qc.rubricaEsterna is not null && qc.rubricaEsterna.Any())
            {
                criteriRicerca.Add(new CriterioRicerca
                {
                    Campo = CampiRicercaEnum.RubricaEsterna,
                    Valore = string.Join("@", qc.rubricaEsterna)
                });
            }
            List<Services.RubricaComune.Corrispondente> rc = new();

            var response = await this._rubricaComuneService.Search(this.GetAuthToken(), new SearchRequest
            {
                CriteriRicerca = criteriRicerca,
                ElementiPerPagina = 50,
                Pagina = 0
            });

            if (response.Corrispondenti.Any())
            {
                rc.AddRange(response.Corrispondenti);
            }

            var regs = await this.GetListaRegistriRfRuolo(user.idCorrGlobali);

            foreach (var c in rc)
            {
                if (c != null!)
                {
                    if (!await this.InternoInAoo(regs, c, user))
                    {
                        elementiRubrica.Add(new ElementoRubrica
                        {
                            codice = c.Codice,
                            descrizione = c.Denominazione,
                            interno = false,
                            tipo = c.Tipo == Tipi.RaggruppamentoFunzionale ? "F" : "U",
                            has_children = false,
                            canale = c.Canale,
                            isRubricaComune = true,
                            rubricaComune = new DocsPaVO.RubricaComune.InfoElementoRubricaComune { IdRubricaComune = int.Parse(c.Id) },
                            rubricaEsterna = c.RubricaEsterna
                        });
                    }
                }
            }

            return elementiRubrica;
        }
        private async Task<bool> InternoInAoo(List<string> regs, Services.RubricaComune.Corrispondente corr, InfoUtente infoUtente)
        {
            bool output = false;

            if (!string.IsNullOrEmpty(corr.AOO))
            {
                output = (from r in regs where r.Equals(corr.AOO) select r).FirstOrDefault() != null;
            }
            return output;
        }

        private async Task<List<string>> GetListaRegistriRfRuolo(string idRuolo)
        {
            long? idRole = string.IsNullOrEmpty(idRuolo) ? null : idRuolo.AsLong();

            var codRegs = await (from b in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                 from a in this._dbContext.RegistroEntities.AsNoTracking()
                                 where a.SYSTEM_ID == b.ID_REGISTRO && b.ID_RUOLO_IN_UO == idRole && a.CHA_RF != null && a.CHA_RF.Equals("0")
                                 select a.VAR_CODICE
             ).ToListAsync();

            return codRegs;
        }

        protected static bool CheckCalltypesRicercaRuoliDisabilitati(CallTypes c) => !(
                c == CallTypes.CALLTYPE_FIND_ROLE
                || c == CallTypes.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO
                || c == CallTypes.CALLTYPE_RICERCA_CREATOR
                || c == CallTypes.CALLTYPE_OWNER_AUTHOR
                || c == CallTypes.CALLTYPE_DEP_OSITO
                || c == CallTypes.CALLTYPE_RICERCA_TRASM_TODOLIST
                || c == CallTypes.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT
                || c == CallTypes.CALLTYPE_RICERCA_DOCUMENTI
                || c == CallTypes.CALLTYPE_RICERCA_TRASM
                || c == CallTypes.CALLTYPE_RICERCA_COMPLETAMENTO
                || c == CallTypes.CALLTYPE_RICERCA_ESTESA
                || c == CallTypes.CALLTYPE_RICERCA_MITTINTERMEDIO
                || c == CallTypes.CALLTYPE_RICERCA_MITTDEST
                || c == CallTypes.CALLTYPE_DEST_FOR_SEARCH_MODELLI
                || c == CallTypes.CALLTYPE_CORR_NO_FILTRI
                || c == CallTypes.CALLTYPE_RICERCA_CORRISPONDENTE
                || c == CallTypes.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                || c == CallTypes.CALLTYPE_CORR_INT_CON_DISABILITATI
                || c == CallTypes.CALLTYPE_CORR_EST_CON_DISABILITATI);

        protected static bool CheckCalltypesFiltroRegistro(CallTypes c) => (c == CallTypes.CALLTYPE_PROTO_IN
                || c == CallTypes.CALLTYPE_PROTO_INGRESSO
                || c == CallTypes.CALLTYPE_PROTO_OUT
                || c == CallTypes.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                || c == CallTypes.CALLTYPE_MITT_MULTIPLI
                || c == CallTypes.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO
                || c == CallTypes.CALLTYPE_PROTO_OUT_ESTERNI
                || c == CallTypes.CALLTYPE_CORR_INT
                || c == CallTypes.CALLTYPE_CORR_EST
                || c == CallTypes.CALLTYPE_CORR_INT_EST
                || c == CallTypes.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                || c == CallTypes.CALLTYPE_CORR_INT_CON_DISABILITATI
                || c == CallTypes.CALLTYPE_CORR_EST_CON_DISABILITATI);

        protected static bool CheckCallTypesFiltroModelliTrasm(CallTypes c) => (c == CallTypes.CALLTYPE_MITT_MODELLO_TRASM
                || c == CallTypes.CALLTYPE_REPLACE_ROLE
                || c == CallTypes.CALLTYPE_FIND_ROLE);

        protected static bool CheckCallTypesFiltroRuoloInteropNoMail(CallTypes c) => (c == CallTypes.CALLTYPE_RUOLO_REG_NOMAIL
                || c == CallTypes.CALLTYPE_RUOLO_RESP_REG);

        protected static bool CheckCallTypesGestioneRubrica(CallTypes c) => (c == CallTypes.CALLTYPE_MANAGE
                || c == CallTypes.CALLTYPE_ESTERNI_AMM
                || c == CallTypes.CALLTYPE_RICERCA_ESTESA
                || c == CallTypes.CALLTYPE_RICERCA_MITTDEST
                || c == CallTypes.CALLTYPE_RICERCA_MITTINTERMEDIO
                || c == CallTypes.CALLTYPE_RICERCA_COMPLETAMENTO
                || c == CallTypes.CALLTYPE_LISTE_DISTRIBUZIONE
                || c == CallTypes.CALLTYPE_CORR_EST
                || c == CallTypes.CALLTYPE_CORR_INT
                || c == CallTypes.CALLTYPE_CORR_INT_EST
                || c == CallTypes.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                || c == CallTypes.CALLTYPE_CORR_INT_CON_DISABILITATI
                || c == CallTypes.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                || c == CallTypes.CALLTYPE_CORR_NO_FILTRI
                || c == CallTypes.CALLTYPE_CORR_INT_NO_UO);

        protected static bool CheckCallTypesCheckboxVisibility(CallTypes c) => (c == CallTypes.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO
                || c == CallTypes.CALLTYPE_MODELLI_TRASM_ALL
                || c == CallTypes.CALLTYPE_MODELLI_TRASM_INF
                || c == CallTypes.CALLTYPE_MODELLI_TRASM_SUP
                || c == CallTypes.CALLTYPE_MODELLI_TRASM_PARILIVELLO
                || c == CallTypes.CALLTYPE_DEST_MODELLO_TRASM
                || c == CallTypes.CALLTYPE_TRASM_PARILIVELLO
                || c == CallTypes.CALLTYPE_TRASM_ALL
                || c == CallTypes.CALLTYPE_TRASM_SUP
                || c == CallTypes.CALLTYPE_TRASM_INF
                || c == CallTypes.CALLTYPE_ORGANIGRAMMA_INTERNO
                || c == CallTypes.CALLTYPE_REPLACE_ROLE);

        protected async Task ApplyBasicFilters(IQueryable<CorrGlobaliEntity> entities, ParametriRicercaRubrica qc)
        {
            if (!string.IsNullOrWhiteSpace(qc.codice))
                entities = (qc.queryCodiceEsatta) ?
                    entities.Where(row => !string.IsNullOrEmpty(row.VAR_COD_RUBRICA) && EF.Functions.Like(row.VAR_COD_RUBRICA.ToUpper(), $"{qc.codice.ToUpper().Replace("'", "''")}")) :
                    entities.Where(row => !string.IsNullOrEmpty(row.VAR_COD_RUBRICA) && EF.Functions.Like(row.VAR_COD_RUBRICA.ToUpper(), $"%{qc.codice.ToUpper().Replace("'", "''")}%"));

            if (!string.IsNullOrWhiteSpace(qc.descrizione))
                entities = entities.Where(x => x.VAR_DESC_CORR.ToUpper().Contains(qc.descrizione.ToUpper()));

            if (!string.IsNullOrWhiteSpace(qc.email))
                entities = entities.Where(x => x.VAR_EMAIL.ToUpper().Contains(qc.email.ToUpper()));

            if (!string.IsNullOrWhiteSpace(qc.citta))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_CITTA.ToUpper().Contains(qc.citta.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                entities = entities.Where(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.localita))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_LOCALITA.ToUpper().Contains(qc.citta.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                entities = entities.Where(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.codiceFiscale))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_COD_FISC.ToUpper().Contains(qc.citta.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                entities = entities.Where(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.partitaIva))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_COD_PI.ToUpper().Contains(qc.citta.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                entities = entities.Where(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }
        }

        protected async Task ApplyBasicFilters(ExpressionStarter<CorrGlobaliEntity> predicate, ParametriRicercaRubrica qc)
        {
            if (!string.IsNullOrWhiteSpace(qc.codice))
            {
                if (qc.queryCodiceEsatta)
                    predicate.And(x => x.VAR_COD_RUBRICA.ToUpper() == qc.codice.ToUpper());
                else
                    predicate.And(x => x.VAR_COD_RUBRICA.ToUpper().Contains(qc.codice.ToUpper()));

            }

            if (!string.IsNullOrWhiteSpace(qc.descrizione))
                predicate.And(x => EF.Functions.Like(x.VAR_DESC_CORR.ToUpper(), $"%{qc.descrizione.ToUpper()}%"));

            if (!string.IsNullOrWhiteSpace(qc.email))
                predicate.And(x => x.VAR_EMAIL.ToUpper().Contains(qc.email.ToUpper()));

            if (!string.IsNullOrWhiteSpace(qc.citta))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_CITTA.ToUpper().Contains(qc.citta.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                predicate.And(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.localita))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_LOCALITA.ToUpper().Contains(qc.citta.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                predicate.And(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.codiceFiscale))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_COD_FISC.ToUpper().Contains(qc.citta.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                predicate.And(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.partitaIva))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_COD_PI.ToUpper().Contains(qc.citta.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                predicate.And(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }
            predicate.And(x=>!x.DTA_FINE.HasValue);
        }

        protected static void ApplyFilterListeDistribuzione(ExpressionStarter<CorrGlobaliEntity> predicate, string idTenant, string idPeople, string idGruppo)
        {
            predicate.And(x => x.ID_AMM == idTenant.AsLong() && x.CHA_TIPO_URP == "L");

            var hasIdUser = !string.IsNullOrWhiteSpace(idPeople);
            var hasIdGruppo = !string.IsNullOrWhiteSpace(idGruppo);

            if (hasIdUser && !hasIdGruppo)
                predicate.And(x => x.ID_GRUPPO_LISTE == null && x.DTA_FINE == null &&
                                            (x.ID_PEOPLE_LISTE == null || x.ID_PEOPLE_LISTE == idPeople.AsLong()));

            else if (!hasIdUser && hasIdGruppo)
                predicate.And(x => x.ID_PEOPLE_LISTE == null && x.DTA_FINE == null &&
                                            (x.ID_GRUPPO_LISTE == null || x.ID_GRUPPO_LISTE == idGruppo.AsLong()));

            else if (hasIdUser && hasIdGruppo)
                predicate.And(x => (x.ID_PEOPLE_LISTE == idPeople.AsLong() && x.ID_GRUPPO_LISTE == null) ||
                                                (x.ID_PEOPLE_LISTE == null && x.ID_GRUPPO_LISTE == idGruppo.AsLong()) ||
                                                (x.ID_PEOPLE_LISTE == null && x.ID_GRUPPO_LISTE == null) &&
                                                x.DTA_FINE == null);
        }

        protected async Task<ElementoRubrica> GetElementoRubrica(CorrWithCodReg entity, bool fromGetChildrenSp = false)
        {
            var elemento = new ElementoRubrica
            {
                codice = entity.Corr.VAR_COD_RUBRICA!,
                descrizione = entity.Corr.VAR_DESC_CORR!,
                interno = entity.Corr.CHA_TIPO_IE == "I",
                tipo = entity.Corr.CHA_TIPO_URP!,
                systemId = entity.Corr.SYSTEM_ID.ToString(),
                has_children = false,
                idRegistro = entity.Corr.ID_REGISTRO.ToString(),
                //disabledTrasm =  entity.Corr.CHA_DISABLED_TRASM == "1",
                disabledTrasm = (!fromGetChildrenSp && entity.Corr.CHA_DISABLED_TRASM != null && entity.Corr.CHA_DISABLED_TRASM.ToString() == "1"),
                disabled = (!fromGetChildrenSp && entity.Corr.DTA_FINE != null && !string.IsNullOrEmpty(entity.Corr.DTA_FINE.ToString())),
                idPeople = entity.Corr.ID_PEOPLE.ToString(),
                nome = entity.Corr.VAR_NOME,
                cognome = entity.Corr.VAR_COGNOME,
                codiceRegistro = entity.CodRegRf ?? string.Empty
            };

            var dettGlobaliEntity = await this._dbContext.DettGlobaliEntities.FirstOrDefaultAsync(x => x.ID_CORR_GLOBALI == entity.Corr.SYSTEM_ID);

            if(dettGlobaliEntity is not null)
            {
                elemento.codiceFiscale = dettGlobaliEntity.VAR_COD_FISC;
                elemento.partitaIva = dettGlobaliEntity.VAR_COD_PI;
            }

            return elemento;
        }

        #endregion
    }
}
