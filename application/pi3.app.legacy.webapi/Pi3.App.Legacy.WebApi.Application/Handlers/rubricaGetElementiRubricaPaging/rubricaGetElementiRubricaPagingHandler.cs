// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.addressbook;
using DocsPaVO.rubrica;
using DocsPaVO.RubricaComune;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using rubricaGetElementiRubricaPagingRequest = Pi3.App.Legacy.WebApi.Application.Requests.rubricaGetElementiRubricaPaging;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.rubricaGetElementiRubricaPaging
{
    public class rubricaGetElementiRubricaPagingHandler : IRequestHandler<rubricaGetElementiRubricaPagingRequest, rubricaGetElementiRubricaPagingResult>
    {
        public rubricaGetElementiRubricaPagingHandler(
            ILogger<rubricaGetElementiRubricaPagingHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IHttpContextAccessor httpContextAccessor,
            IRubricaComuneService rubricaComuneService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._logger = logger;
            this._configurationService = configurationService;
            this._rubricaComuneService = rubricaComuneService;
            this._httpContextAccessor = httpContextAccessor;
        }



        public async Task<rubricaGetElementiRubricaPagingResult> Handle(rubricaGetElementiRubricaPagingRequest request, CancellationToken cancellationToken)
        {
            List<ElementoRubrica> els = new();
            int elCount = 0;
            try
            {

                var qc = request.qc;
                var u = request.u;

                if (qc.caller != null && qc.caller.IdUtente == null)
                    qc.caller.IdUtente = u.idPeople;

                (els, elCount) = await this.SearchPaging(qc,request.smistamentoRubrica,request.firstRowNum,request.maxRowForPage,u);


            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return new(els.ToArray(), elCount);
        }




        #region Private Member
        protected readonly ILogger<rubricaGetElementiRubricaPagingHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IRubricaComuneService _rubricaComuneService;
        private readonly string BearerPrefix = "Bearer ";
        protected IHttpContextAccessor _httpContextAccessor;


        private async Task<(List<ElementoRubrica>,int)> SearchPaging(DocsPaVO.rubrica.ParametriRicercaRubrica qr, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, int firstRowNum, int maxRowForPage, DocsPaVO.utente.InfoUtente user)
        {
            List<ElementoRubrica> ers = await this.GetElementiRubricaPaging(qr, firstRowNum, maxRowForPage, user);

            ers = await this.Dpa3SearchFilter(qr,ers,smistamentoRubrica,user); 

            this.SetVisibleCheckBoxElement(qr, ref ers);


            if (!(qr.localita == null || qr.localita == ""))
            {
                qr.doRubricaComune = false;
            }

            if (this.RicercaRubricaComune(qr) || this.RicercaRubricaEsterna(qr))
            {

                var criteriRicerca = new List<CriterioRicerca>
                {
                    new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = string.IsNullOrEmpty(qr.codice) ? qr.codice : qr.codice.TrimEnd() },
                    new CriterioRicerca { Campo = CampiRicercaEnum.Denominazione, Valore = string.IsNullOrEmpty(qr.descrizione) ? qr.descrizione : qr.descrizione.TrimEnd()  },
                    new CriterioRicerca { Campo = CampiRicercaEnum.Citta, Valore = string.IsNullOrEmpty(qr.citta) ? qr.citta : qr.citta.TrimEnd()  },
                    new CriterioRicerca { Campo = CampiRicercaEnum.Email, Valore = string.IsNullOrEmpty(qr.email) ? qr.email : qr.email.TrimEnd() },
                    new CriterioRicerca { Campo = CampiRicercaEnum.CodiceFiscale, Valore = string.IsNullOrEmpty(qr.codiceFiscale) ? qr.codiceFiscale : qr.codiceFiscale.TrimEnd()  },
                    new CriterioRicerca { Campo = CampiRicercaEnum.PartitaIva, Valore = string.IsNullOrEmpty(qr.partitaIva) ? qr.partitaIva : qr.partitaIva.TrimEnd()  }
                };
                if (qr.rubricaEsterna is not null && qr.rubricaEsterna.Any())
                {
                    criteriRicerca.Add(new CriterioRicerca
                    {
                        Campo = CampiRicercaEnum.RubricaEsterna,
                        Valore = string.Join("@", qr.rubricaEsterna)
                    });
                }
                if (!this.RicercaRubricaComune(qr))
                {
                    //Non effettuo la ricerca nella rubrica comune ma solo in quella esterna
                    criteriRicerca.Add(new CriterioRicerca { Campo = CampiRicercaEnum.SoloRubricaEsterna, Valore = "1" });
                }
                try
                {
                    List<Services.RubricaComune.Corrispondente> rc = new();

                    /*
                    int currPage = 1;
                    int nPagesRc = 2;

                    while (currPage <= nPagesRc)
                    {
                        var response = await this._rubricaComuneService.Search(this.GetAuthToken(), new SearchRequest
                        {
                            CriteriRicerca = criteriRicerca,
                            ElementiPerPagina = 50,
                            Pagina = currPage++
                        });
                        nPagesRc = response.TotalePagine;
                        if (response.Corrispondenti.Any())
                        {
                            rc.AddRange(response.Corrispondenti);
                        }
                    }
                    */

                    var response = await this._rubricaComuneService.Search(this.GetAuthToken(), new SearchRequest
                    {
                        CriteriRicerca = criteriRicerca,
                        ElementiPerPagina = 50,
                        Pagina = 0
                    });
                    
                    if (response.Corrispondenti!.Any())
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
                                ers.Add(new ElementoRubrica
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

                    ers = ers.OrderBy(a => a.descrizione.ToUpper().Trim()).ToList();

                }
                catch (Exception ex)
                {
                    this._logger.LogError(exception: ex, message: ex.Message);
                }


            }
            return (ers,ers.Count);
        }
        protected string? GetAuthToken()
        {
            //if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            //var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];
            var authorizationHeader = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            return authorizationHeader;
        }

        private async Task<bool> InternoInAoo(List<string> regs,Services.RubricaComune.Corrispondente corr, InfoUtente infoUtente)
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



        private bool RicercaRubricaComune(DocsPaVO.rubrica.ParametriRicercaRubrica qr)
        {
            return ((qr.tipoIE == TipoUtente.GLOBALE || qr.tipoIE == TipoUtente.ESTERNO) && qr.doRubricaComune);
        }
        private bool RicercaRubricaEsterna(DocsPaVO.rubrica.ParametriRicercaRubrica qr)
        {
            bool rubricaEsterna = false;

            if ((qr.tipoIE == TipoUtente.GLOBALE || qr.tipoIE == TipoUtente.ESTERNO) && qr.rubricaEsterna != null && qr.rubricaEsterna.Count > 0)
                rubricaEsterna = true;

            return rubricaEsterna;
        }
        private async Task<List<ElementoRubrica>> FiltraAoo(DocsPaVO.rubrica.ParametriRicercaRubrica qr, List<ElementoRubrica> ers, DocsPaVO.utente.InfoUtente user)
        {
            string[] uoInAooConRegAss = null;
            string[] uoInAoo = null;
            List<ElementoRubrica> a = new List<ElementoRubrica>();
            Hashtable hUo = await this.GetUORuoloSemplice(user.idAmministrazione);
            Hashtable hUt = await this.GetRuoliUtenteSemplice(user.idAmministrazione);

            
            if (qr.caller.IdRegistro != null && qr.caller.IdRegistro != string.Empty)
                uoInAoo = await this.GetUoInterneAoo(qr.caller.IdRegistro);
            else
                uoInAoo = await this.GetUoInterneAooNoReg();


            uoInAooConRegAss = await this.GetUoIntRegAss(user.idGruppo);

            if (uoInAoo != null)
            {
                Array.Sort(uoInAoo, CaseInsensitiveComparer.Default);
            }
            if (uoInAooConRegAss != null)
                Array.Sort(uoInAooConRegAss, CaseInsensitiveComparer.Default);



            switch (qr.tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    for (int i = 0; i < ers.Count; i++)
                    {
                        DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica)ers[i];
                        try
                        {
                            switch (er.tipo)
                            {
                                case "U":
                                    if ((!er.interno) || (er.interno && !this.CheckUo(uoInAoo, er.codice) && this.CheckUoWithReg(uoInAooConRegAss, er.codice)))
                                        a.Add(er);
                                    break;
                                case "R":
                                case "P":


                                    if (!this.CheckRuoliUtenti(hUo, hUt, uoInAoo, er))
                                    {
                                        a.Add(er);
                                    }
                                    break;
                                case "L":
                                    a.Add(er);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(exception: ex, message: ex.Message);
                        }

                    }
                    break;


                default:
                    for (int i = 0; i < ers.Count; i++)
                    {
                        DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica)ers[i];

                        switch (er.tipo)
                        {
                            case "U":
                                {
                                    if ((!er.interno && qr.tipoIE.Equals(DocsPaVO.addressbook.TipoUtente.GLOBALE)) || this.CheckUo(uoInAoo, er.codice))
                                    {
                                        er.isVisibile = true;
                                        a.Add(er);

                                    }
                                    else
                                    {
                                        if ((qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
                                            || (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO))
                                        {
                                            er.isVisibile = false;
                                            a.Add(er);
                                        }
                                        if (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO)
                                        {
                                            er.isVisibile = true;
                                            a.Add(er);
                                        }
                                    }
                                    break;
                                }
                            case "R":
                            case "P":
                                if ((!er.interno && qr.tipoIE.Equals(DocsPaVO.addressbook.TipoUtente.GLOBALE)) || this.CheckRuoliUtenti(hUo, hUt, uoInAoo, er))
                                {
                                    er.isVisibile = true;
                                    a.Add(er);
                                }
                                else
                                {
                                    if ((qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
                                    || (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO))
                                    {
                                        er.isVisibile = false;
                                        a.Add(er);
                                    }
                                    if (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO)
                                    {
                                        er.isVisibile = true;
                                        a.Add(er);
                                    }
                                }
                                break;
                            case "L":
                                a.Add(er);
                                break;
                        }
                    }
                    break;
            }
            
            return a;
        }


        private async Task<Hashtable> GetUORuoloSemplice(string idAmm)
        {
            var res = await (from r in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                             from uo in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                             where r.ID_UO == uo.SYSTEM_ID && r.CHA_TIPO_URP != null &&
                             r.CHA_TIPO_URP.Equals("R")
                             select new
                             {
                                 COD_RUOLO = r.VAR_COD_RUBRICA,
                                 COD_UO = uo.VAR_COD_RUBRICA
                             }).ToListAsync();
            Hashtable h = new Hashtable();

            res.ForEach((r) =>
            {
                h[r.COD_RUOLO] = r.COD_UO;
            });
            return h;

        }
        private async Task<Hashtable> GetRuoliUtenteSemplice(string idAmm)
        {
            // hastable with key equal to COD_UTENTE that maps to an array of COD_RUOLO
            Hashtable hUt = new();
            ArrayList tmp = new();

            var ruoliRes = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(this._dbContext.PeopleEntities.AsNoTracking(),
                        u => u.ID_PEOPLE,
                        p => p.SYSTEM_ID,
                        (u, p) => new { u, p })
                    .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(),
                        j => j.p.SYSTEM_ID,
                        pg => pg.PEOPLE_SYSTEM_ID,
                        (j, pg) => new { j.u, j.p, pg })
                    .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(),
                        j => j.pg.GROUPS_SYSTEM_ID,
                        r => r.ID_GRUPPO,
                        (j, r) => new { j.u, j.p, j.pg, r })
                    .Where(j => j.u.CHA_TIPO_URP == "P"
                        && !j.u.DTA_FINE.HasValue
                        && !j.r.DTA_FINE.HasValue
                        && !j.pg.DTA_FINE.HasValue)
                    .OrderBy(j => j.u.VAR_COD_RUBRICA)
                    .Select(j => new
                    {
                        COD_UTENTE = j.u.VAR_COD_RUBRICA,
                        COD_RUOLO = j.r.VAR_COD_RUBRICA,
                        DESC_RUOLO = j.r.VAR_DESC_CORR
                    })
                    .ToListAsync();


            //var ruoliRes = await (
            //    from u in this._dbContext.CorrGlobaliEntities.AsNoTracking()
            //    from r in this._dbContext.CorrGlobaliEntities.AsNoTracking()
            //    from p in this._dbContext.PeopleEntities.AsNoTracking()
            //    from pg in this._dbContext.PeopleGroupEntities.AsNoTracking()
            //    where (r.ID_GRUPPO == pg.GROUPS_SYSTEM_ID) &&
            //    (u.ID_PEOPLE == p.SYSTEM_ID) &&
            //    (p.SYSTEM_ID == pg.PEOPLE_SYSTEM_ID) &&
            //    (u.CHA_TIPO_URP != null && u.CHA_TIPO_URP.Equals("P")) &&
            //    (!u.DTA_FINE.HasValue) &&
            //    (!r.DTA_FINE.HasValue) &&
            //    (!pg.DTA_FINE.HasValue) &&
            //    (!u.DTA_FINE.HasValue)
            //    orderby u.VAR_COD_RUBRICA
            //    select new
            //    {
            //        COD_UTENTE = u.VAR_COD_RUBRICA,
            //        COD_RUOLO = r.VAR_COD_RUBRICA,
            //        DESC_RUOLO = r.VAR_DESC_CORR
            //    }).ToListAsync();

            string currentUser = null;


            ruoliRes.ForEach(u =>
            {
                if (!u.COD_UTENTE.Equals(currentUser))
                {

                    if (u.COD_UTENTE != currentUser)
                    {
                        if (currentUser != null && tmp.Count > 0)
                        {
                            string[] ruoli = new string[tmp.Count];
                            tmp.CopyTo(ruoli);
                            hUt[currentUser] = ruoli;
                            tmp.Clear();
                        }
                        currentUser = u.COD_UTENTE;
                    }
                    tmp.Add(u.COD_RUOLO);
                }
            });

            if (currentUser != null && tmp.Count > 0)
            {
                string[] ruoli = new string[tmp.Count];
                tmp.CopyTo(ruoli);
                hUt[currentUser] = ruoli;
            }
            return hUt;
        }

        private bool CheckRuoliUtenti(Hashtable hUO, Hashtable hUt, string[] uoInAoo, ElementoRubrica er)
        {
            string codUO = null;
            if (er.tipo == "R")
            {
                codUO = (string)hUO[er.codice];
            }
            else
            {
                string[] ruoli = (string[])hUt[er.codice];
                if (ruoli == null || ruoli.Length == 0)
                    return false;

                foreach (string rcod in ruoli)
                {
                    ElementoRubrica err = new ElementoRubrica();
                    err.codice = rcod;
                    err.interno = true;
                    err.tipo = "R";
                    err.descrizione = "";
                    err.has_children = false;

                    if (this.CheckRuoliUtenti(hUO, hUt, uoInAoo, err))
                        return true;
                }
                return false;
            }
            return this.CheckUo(uoInAoo, codUO);
        }
        private bool CheckUo(string[] uoInAoo, string cod_uo)
        {
            int i;
            i = Array.BinarySearch(uoInAoo, cod_uo);
            return (i >= 0);
        }

        private bool CheckUoWithReg(string[] uoInAooReg, string currentCodUo)
        {
            int i;

            i = Array.BinarySearch(uoInAooReg, currentCodUo);
            return (i >= 0);
        }
        private async Task<string[]> GetUoIntRegAss(string idAmm)
        {
            var result = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                from b in this._dbContext.UoRegEnties.AsNoTracking()
                                where a.ID_AMM == idAmm.AsLong() &&
                                b.ID_UO == a.SYSTEM_ID &&
                                a.CHA_SYSTEM_ROLE != null &
                                !a.CHA_SYSTEM_ROLE.Equals("1")
                                orderby a.VAR_COD_RUBRICA
                                select a.VAR_COD_RUBRICA).Distinct().ToListAsync();

            return result.ToArray();

        }

        private async Task<string[]> GetUoInterneAoo(string idReg)
        {

            var result = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                from b in this._dbContext.UoRegEnties.AsNoTracking()
                                where b.ID_REGISTRO == idReg.AsLong() &&
                                b.ID_UO == a.SYSTEM_ID
                                orderby a.VAR_COD_RUBRICA, a.VAR_DESC_CORR ascending
                                select a.VAR_COD_RUBRICA).ToListAsync();

            string[] res = result.ToArray();

            return res;
        }
        private async Task<string[]> GetUoInterneAooNoReg()
        {
            var result = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                from b in this._dbContext.UoRegEnties.AsNoTracking()
                                where a.CHA_SYSTEM_ROLE != null &&
                                b.ID_UO == a.SYSTEM_ID &&
                                !a.CHA_SYSTEM_ROLE.Equals("1")
                                orderby a.VAR_COD_RUBRICA, a.VAR_DESC_CORR ascending
                                select a.VAR_COD_RUBRICA).ToListAsync();

            string[] res = result.ToArray();

            return res;
        }
        private async Task<List<ElementoRubrica>> Dpa3SearchFilter(DocsPaVO.rubrica.ParametriRicercaRubrica qr, List<ElementoRubrica> ers, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, DocsPaVO.utente.InfoUtente user)
        {
            if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_DEST_MODELLO_TRASM)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_UFFREF_PROTO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_UTENTE_REG_NOMAIL)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI))
            {
                ers = await this.FiltraAoo(qr, ers, user);

                if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
                {
                    if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST))
                    {
                        (bool res, List<ElementoRubrica> ers2) = await this.FiltraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, user, ers);
                        ers = ers2;
                        if (!res)
                        {
                            ers = null;
                        }
                    }
                }
            }
            if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO))
            {
                ers = await this.FiltraTrasmissioni(qr, ers, user);
            }

            if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP) ||
                (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO))
                if (qr.ObjectType != null)
                    ers = await this.FiltraTrasmissioni(qr, ers, user);

            if ((((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                || qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INGRESSO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN_INT)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_UFFREF)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_GESTFASC_UFFREF)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_GESTFASC_LOCFISICA)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_FILTRIRICFASC_LOCFIS)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO)
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA && qr.caller.IdRegistro != null))
                && ((qr.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO) || (qr.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)))
                || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE && qr.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE))
            {
                ers = await this.FiltraAoo(qr,ers,user);

                if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
                {
                    if (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                        || qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI
                        || (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO))
                    {
                        (bool res, List<ElementoRubrica> ers2) = await this.FiltraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, user, ers);
                        if (!res)
                        {
                            ers = null;
                        }
                        else
                        {
                            ers = ers2;
                        }
                    }

                    if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
                    {
                        if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA && qr.caller.IdRegistro != null))
                        {
                            (bool res, List<ElementoRubrica> ers2) = await this.FiltraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, user, ers);
                            if (res)
                            {
                                ers = null;
                            }
                            else
                            {
                                ers = ers2;
                            }
                        }
                    }
                }

                if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
                {
                    if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT || 
                        qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI || 
                        (qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)) && 
                        qr.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE)
                    {
                        (bool res, List<ElementoRubrica> ers2) = await this.FiltraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, user, ers);
                        if (res)
                        {
                            ers = null;
                        }
                        else
                        {
                            ers = ers2;
                        }
                    }


                    if ((qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
                    {
                        (bool res, List<ElementoRubrica> ers2) = await this.FiltraPrimoSmistamento(qr.caller.IdRegistro, qr.caller.IdRuolo, user, ers);
                        if (res)
                        {
                            ers = null;
                        }
                        else
                        {
                            ers = ers2;
                        }
                    }
                }
            }


            return ers;


        }




        private async Task<List<ElementoRubrica>> FiltraTrasmissioni(DocsPaVO.rubrica.ParametriRicercaRubrica qr, List<ElementoRubrica> ers, DocsPaVO.utente.InfoUtente user)
        {
            List<ElementoRubrica> a = new List<ElementoRubrica>();
            List<Ruolo> ruoliAutorizzati = new();


            TipoOggetto tipoOggetto = (qr.ObjectType != null && qr.ObjectType.StartsWith("F:")) ? TipoOggetto.FASCICOLO : TipoOggetto.DOCUMENTO;
            string idNodoTitolario = (tipoOggetto == TipoOggetto.FASCICOLO) ? qr.ObjectType.Substring(2) : null;
            DocsPaVO.utente.Ruolo r = (await this._mediator.Send(new Application.Requests.GetRuolo(qr.caller.IdRuolo))).output;

            switch (qr.calltype)
            {
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL:
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL:
                    ruoliAutorizzati = await this.GetRuoAut(r, qr.caller.IdRegistro, idNodoTitolario, tipoOggetto);
                    break;
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP:
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP:
                    ruoliAutorizzati = await this.GetCorrGlobRuoSup(tipoOggetto, qr.caller.IdRegistro, idNodoTitolario, r);
                    break;
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF:
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF:
                    ruoliAutorizzati = await this.GetCorrGlobRuoInf(r, qr.caller.IdRegistro, idNodoTitolario, tipoOggetto);
                    break;
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO:
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO:
                    ruoliAutorizzati = await this.GetCorrGlobRuoPariLiv(r, qr.caller.IdRegistro, idNodoTitolario, tipoOggetto);
                    break;
                case DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST:
                    RagioneTrasmissione rTo = (await this._mediator.Send(new Application.Requests.GetRagione("TO", user.idAmministrazione))).output;
                    TipoGerarchia gTo = rTo.tipoDestinatario;

                    switch (gTo)
                    {
                        case TipoGerarchia.INFERIORE:
                            ruoliAutorizzati = await this.GetCorrGlobRuoInf(r, qr.caller.IdRegistro, idNodoTitolario, tipoOggetto);
                            break;

                        case TipoGerarchia.PARILIVELLO:
                            ruoliAutorizzati = await this.GetCorrGlobRuoPariLiv(r, qr.caller.IdRegistro, idNodoTitolario, tipoOggetto);
                            break;

                        case TipoGerarchia.SUPERIORE:
                            ruoliAutorizzati = await this.GetCorrGlobRuoSup(tipoOggetto, qr.caller.IdRegistro, idNodoTitolario, r);
                            break;

                        case TipoGerarchia.TUTTI:
                            ruoliAutorizzati = await this.GetRuoAut(r, qr.caller.IdRegistro, idNodoTitolario, tipoOggetto);
                            break;
                    }
                    break;

                default:
                    return a;
            }
            string[] cRuoliAuth = new string[ruoliAutorizzati.Count];
            for (int i = 0; i < ruoliAutorizzati.Count; i++)
                cRuoliAuth[i] = ((DocsPaVO.utente.Ruolo)ruoliAutorizzati[i]).codiceRubrica;


            foreach (DocsPaVO.rubrica.ElementoRubrica er in ers)
            {

                switch (er.tipo)
                {
                    case "U":
                        if (await this.UoIsAutorizzato((er.interno ? "I" : "E") + @"\" + er.codice, r, cRuoliAuth, user))
                            a.Add(er);
                        break;

                    case "R":
                        if (this.RuoloIsAutorizzato(er.codice, cRuoliAuth))
                            a.Add(er);
                        break;

                    case "P":
                        if (await this.UtenteIsAutorizzato((er.interno ? "I" : "E") + @"\" + er.codice, cRuoliAuth,user))
                            a.Add(er);
                        break;
                    case "L":
                    case "F":
                        a.Add(er);
                        break;
                }


            }

            Array.Sort(cRuoliAuth);

            return a;
        }

        private async Task<bool> UtenteIsAutorizzato(string cod, string[] ruoliAuth, DocsPaVO.utente.InfoUtente user)
        {
            string codice = cod;

            if (codice.StartsWith(@"E\") || codice.StartsWith(@"I\"))
                codice = codice.Substring(codice.IndexOf(@"\") + 1);

            var hUtenti = await this.GetRuoliUtenteSemplice(user.idAmministrazione);

            string[] ruoli = (string[])hUtenti[codice];
            if (ruoli != null && ruoli.Length > 0)
                foreach (string codRuolo in ruoli)
                    if (this.RuoloIsAutorizzato(codRuolo, ruoliAuth))
                        return true;

            return false;
        }

        private async Task<bool> UoIsAutorizzato(string cod, DocsPaVO.utente.Ruolo ruolo_caller, string[] ruoliAuth,DocsPaVO.utente.InfoUtente user)
        {
            string codice = cod;
            Hashtable hRuoli = await this.GetRuoliUOSemplice(user.idAmministrazione);

            if (codice.StartsWith(@"E\") || codice.StartsWith(@"I\"))
                codice = codice.Substring(codice.IndexOf(@"\") + 1);
            string[] ruoli = (string[])hRuoli[codice];
            if (ruoli != null && ruoli.Length > 0)
                foreach (string codRuolo in ruoli)
                    if (this.RuoloIsAutorizzato(codRuolo, ruoliAuth))
                        return true;

            return false;
        }

        private bool RuoloIsAutorizzato(string codice, string[] ruoliAutorizzati)
        {
            return ruoliAutorizzati.ToList().Contains(codice) == true;
        }



        private async Task<Hashtable> GetRuoliUOSemplice(string idAmm)
        {
            Hashtable h  = new Hashtable();
            ArrayList tmp = new ArrayList();
            string curUo = null;


            var uoRuoli = await (from u in this._dbContext.CorrGlobaliEntities.AsNoTracking()
             from r in this._dbContext.CorrGlobaliEntities.AsNoTracking()
             where r.ID_UO == u.SYSTEM_ID &&
             u.CHA_TIPO_URP != null &&
             u.CHA_TIPO_URP.Equals("U") &&
             u.ID_AMM == idAmm.AsLong() &&
             r.ID_AMM == idAmm.AsLong() &&
             !u.DTA_FINE.HasValue &&
             !r.DTA_FINE.HasValue &&
             r.CHA_RIFERIMENTO != null &&
             r.CHA_RIFERIMENTO.Equals("1")
             orderby u.VAR_COD_RUBRICA
             select new
             {
                 COD_UO = u.VAR_COD_RUBRICA,
                 COD_RUOLO = r.VAR_COD_RUBRICA
             }).ToListAsync();



            uoRuoli.ForEach(row =>
            {
                if ((string)row.COD_UO != curUo)
                {
                    if (curUo != null && tmp.Count > 0)
                    {
                        string[] ruoli = new string[tmp.Count];
                        tmp.CopyTo(ruoli);
                        h[curUo] = ruoli;
                        tmp.Clear();
                    }
                    curUo = (string)row.COD_UO;
                }
                tmp.Add((string)row.COD_RUOLO);
            });

            if (curUo != null && tmp.Count > 0)
            {
                string[] ruoli = new string[tmp.Count];
                tmp.CopyTo(ruoli);
                h[curUo] = ruoli;
            }
            return h;
        }


        private async Task<List<Ruolo>> GetRuoAut(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
        {
            var predicate = PredicateBuilder.New<CorrRuoInf>();
            bool isPredSet = false;
            IQueryable<CorrRuoInf> baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                      from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                      select new CorrRuoInf()
                                      {
                                          SystemId = a.SYSTEM_ID,
                                          IdGruppo = a.ID_GRUPPO,
                                          VarDescRuolo = b.VAR_DESC_RUOLO,
                                          VarCodice = a.VAR_CODICE,
                                          VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                          IdParent = a.ID_PARENT,
                                          IdUo = a.ID_UO,
                                          IdAmm = a.ID_AMM,
                                          BSysId = b.SYSTEM_ID,
                                          AIdTipRuolo = a.ID_TIPO_RUOLO
                                      });


            if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                if (idRegistro != null && idRegistro != "" && !await this.IsFiltroAooEnabled())
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                 from d in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                 select new CorrRuoInf()
                                 {
                                     SystemId = a.SYSTEM_ID,
                                     IdGruppo = a.ID_GRUPPO,
                                     VarDescRuolo = b.VAR_DESC_RUOLO,
                                     VarCodice = a.VAR_CODICE,
                                     VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                     IdParent = a.ID_PARENT,
                                     IdUo = a.ID_UO,
                                     IdAmm = a.ID_AMM,
                                     BSysId = b.SYSTEM_ID,
                                     AIdTipRuolo = a.ID_TIPO_RUOLO,
                                     RegIdReg = d.ID_REGISTRO,
                                     CIdRuoInUo = d.ID_RUOLO_IN_UO

                                 });
                }
            }
            else
            {
                bool c1 = !string.IsNullOrEmpty(idNodoTitolario);
                bool c2 = idRegistro != null && idRegistro != "" && !await this.IsFiltroAooEnabled();
                
                if(c1 && c2)
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                 from c in this._dbContext.SecurityEntities.AsNoTracking()
                                 from d in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                 select new CorrRuoInf()
                                 {
                                     SystemId = a.SYSTEM_ID,
                                     IdGruppo = a.ID_GRUPPO,
                                     VarDescRuolo = b.VAR_DESC_RUOLO,
                                     VarCodice = a.VAR_CODICE,
                                     VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                     IdParent = a.ID_PARENT,
                                     IdUo = a.ID_UO,
                                     IdAmm = a.ID_AMM,
                                     BSysId = b.SYSTEM_ID,
                                     AIdTipRuolo = a.ID_TIPO_RUOLO,
                                     RegIdRuolo = d.ID_REGISTRO,
                                     CIdRuoInUo = d.ID_RUOLO_IN_UO,
                                     SecThing = c.THING,
                                     RegIdReg = d.ID_REGISTRO,
                                     SecPersonOrGroup = c.PERSONORGROUP

                                 });
                }
                else if(c1 && !c2)
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                 from s in this._dbContext.SecurityEntities.AsNoTracking()
                                 select new CorrRuoInf()
                                 {
                                     SystemId = a.SYSTEM_ID,
                                     IdGruppo = a.ID_GRUPPO,
                                     VarDescRuolo = b.VAR_DESC_RUOLO,
                                     VarCodice = a.VAR_CODICE,
                                     VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                     IdParent = a.ID_PARENT,
                                     IdUo = a.ID_UO,
                                     IdAmm = a.ID_AMM,
                                     BSysId = b.SYSTEM_ID,
                                     AIdTipRuolo = a.ID_TIPO_RUOLO,
                                     SecThing = s.THING,
                                     SecPersonOrGroup = s.PERSONORGROUP

                                 });
                }
                else if (!c1 && c2)
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                 from d in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                 select new CorrRuoInf()
                                 {
                                     SystemId = a.SYSTEM_ID,
                                     IdGruppo = a.ID_GRUPPO,
                                     VarDescRuolo = b.VAR_DESC_RUOLO,
                                     VarCodice = a.VAR_CODICE,
                                     VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                     IdParent = a.ID_PARENT,
                                     IdUo = a.ID_UO,
                                     IdAmm = a.ID_AMM,
                                     BSysId = b.SYSTEM_ID,
                                     AIdTipRuolo = a.ID_TIPO_RUOLO,
                                     RegIdReg = d.ID_REGISTRO,
                                     CIdRuoInUo = d.ID_RUOLO_IN_UO

                                 });
                }



                
            }

            if (ruolo != null && ruolo.idAmministrazione != null && !ruolo.idAmministrazione.ToString().Equals(""))
            {
                predicate = predicate.And(row => row.IdAmm == ruolo.idAmministrazione.AsLong());
            }
            predicate = predicate.And(row => row.BSysId == row.AIdTipRuolo);
            isPredSet = true;
            if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                if (idRegistro != null && idRegistro != "" && !await this.IsFiltroAooEnabled())
                {
                    predicate = predicate.And(row => row.CIdRuoInUo == row.SystemId && row.RegIdReg == idRegistro.AsLong());
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(idNodoTitolario))
                {
                    predicate = predicate.And(row => row.SecThing == idNodoTitolario.AsLong() && row.SecPersonOrGroup == row.IdGruppo);
                }
                if (idRegistro != null && idRegistro != "" && !await this.IsFiltroAooEnabled())
                {
                    predicate = predicate.And(row => row.CIdRuoInUo == row.SystemId && row.RegIdReg == idRegistro.AsLong());
                }

            }

            Ruolo tRole = new();
            List<Ruolo> roles = new();

            try
            {
                baseQuery.Where(predicate).ForEach(r =>
                {
                    tRole = new();
                    tRole.systemId = r.SystemId.ToString();
                    tRole.descrizione = r.VarDescRuolo;
                    tRole.codiceCorrispondente = r.VarCodice;
                    tRole.codiceRubrica = r.VarCodiceRubrica;
                    tRole.uo = r.IdUo != null ? this.GetParents(r.IdUo.ToString(), tRole) : null;
                    tRole.idGruppo = r.IdGruppo != null ? r.IdGruppo.ToString() : null;
                    roles.Add(tRole);
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            

            return roles;
        }
        private async Task<long?> GetUoParent(string systemId)
        {
            var parent = await (from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
             where c.SYSTEM_ID == systemId.AsLong()
             select c.ID_PARENT).FirstOrDefaultAsync();

            if (parent != null && (parent.Equals("0") || parent.Equals("")))
            {
                parent = null;
            }

            return parent;
        }

        private async Task<List<string>> GetParentUO(DocsPaVO.utente.Ruolo ruolo)
        {
            List<string> list = new();
            string idCurrentUO = null;
            if (ruolo.uo != null)
            {
                idCurrentUO = ruolo.uo.systemId;
            }
            while (idCurrentUO != null)
            {
                list.Add(idCurrentUO);
                var tempIdCurrent = await this.GetUoParent(idCurrentUO);

                idCurrentUO = tempIdCurrent != null ? tempIdCurrent.ToString() : string.Empty;
            }

            return list;
        }

        private async Task<List<Ruolo>> GetCorrGlobRuoSup(DocsPaVO.trasmissione.TipoOggetto tipoOggetto, string idRegistro, string idNodoTitolario, DocsPaVO.utente.Ruolo ruolo)
        {
            List<string> idParentUO = await this.GetParentUO(ruolo);

            var baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
             from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
             where !a.DTA_FINE.HasValue
             select new CorrRuoInf()
             {
                 SystemId = a.SYSTEM_ID,
                 IdGruppo = a.ID_GRUPPO,
                 VarDescRuolo = b.VAR_DESC_RUOLO,
                 VarCodice = a.VAR_CODICE,
                 VarCodiceRubrica = a.VAR_COD_RUBRICA,
                 IdUo = a.ID_UO
             });

            List<Ruolo> roles = await this.GetCorrGlobRuolo(baseQuery, tipoOggetto, idRegistro, idNodoTitolario, ruolo, idParentUO, DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString());

            return roles;
        }

        private async Task<(bool, List<ElementoRubrica>)> FiltraPrimoSmistamento(string idRegistro, string idRuolo, DocsPaVO.utente.InfoUtente utente, List<ElementoRubrica> ers)
        {
            Hashtable tableUoInterneAOO = new Hashtable();
            ArrayList listaUoSmistamento = new ArrayList();
            bool result = true;
            List<ElementoRubrica> els = new();
            try
            {
                var uoInAoo = await this.GetUoInterneAoo(idRegistro);

                if (uoInAoo != null)
                {
                    Array.Sort(uoInAoo, CaseInsensitiveComparer.Default);
                    foreach (string item in uoInAoo)
                    {
                        if (!tableUoInterneAOO.ContainsKey(item))
                            tableUoInterneAOO.Add(item, item);
                    }
                }

                listaUoSmistamento = await this.GetListaUOSmistamentoRubrica(idRegistro);

                DocsPaVO.utente.Corrispondente corr = null;
                bool smistamento_empty = (listaUoSmistamento == null || listaUoSmistamento.Count == 0);
                Hashtable hUo = await this.GetUORuoloSemplice(utente.idAmministrazione);
                Hashtable hUt = await this.GetRuoliUtenteSemplice(utente.idAmministrazione);

                if (!smistamento_empty)
                {
                    DocsPaVO.utente.Corrispondente ruoloProt = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId(idRuolo, DocsPaVO.addressbook.TipoUtente.INTERNO, utente))).output;
                    Hashtable tableUoSmistamento = new Hashtable();
                    foreach (DocsPaVO.Smistamento.UOSmistamento item in listaUoSmistamento)
                    {
                        if (!tableUoSmistamento.ContainsKey(item.Codice))
                            tableUoSmistamento.Add(item.Codice, item);
                    }
                    string idUoAppartenenza = await this.GetIdUoAppartenenza(((DocsPaVO.utente.Ruolo)ruoloProt).uo.codiceRubrica);

                    for (int i = 0; i < ers.Count; i++)
                    {
                        DocsPaVO.rubrica.ElementoRubrica er = (DocsPaVO.rubrica.ElementoRubrica)ers[i];

                        if (er != null)
                        {
                            if (!er.interno)
                            {
                                continue;
                            }

                            switch (er.tipo)
                            {
                                case "U":
                                    {
                                        if (er.isVisibile && tableUoInterneAOO.Contains(er.codice))
                                        {
                                            if (!await VerificaDipendenzaCodRubrica(idUoAppartenenza, er.codice))
                                            {
                                                //caso in cui la Uo del ruolo � SUPERIORE a quella del protocollatore
                                                if (!tableUoSmistamento.ContainsKey(er.codice))
                                                {
                                                    //vuol dire che l'elemento rubrica NON � nella DPA_UO_SMISTAMENTO
                                                    //quindi NON deve essere selezionabile
                                                    er.isVisibile = false;
                                                }

                                            }
                                        }
                                    }
                                    break;

                                case "R":
                                    if (er.isVisibile && this.CheckRuoliUtenti(hUo, hUt, uoInAoo, er))
                                    {
                                        if (!await this.VerificaDipendenzaCodRubrica(idUoAppartenenza, er.codice))
                                        {
                                            //caso in cui la Uo del ruolo � SUPERIORE a quella del ruolo che protocolla.
                                            er.isVisibile = false;
                                        }
                                    }
                                    break;

                                case "P":
                                    bool SelectorVisibility = false;
                                    if (!this.CheckRuoliUtenti(hUo, hUt, uoInAoo, er))
                                    {
                                        SelectorVisibility = true;
                                    }
                                    else
                                    {
                                        if (er.isVisibile)
                                        {
                                            //se la Uo � sottoposta a quella del protocollista allora rendo visibile l'utente
                                            if (await this.VerificaDipendenzaCodRubrica(idUoAppartenenza, er.codice))
                                            {

                                                SelectorVisibility = true;

                                            }
                                            er.isVisibile = SelectorVisibility;

                                        }
                                    }
                                    break;
                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                result = false;
            }
            return (result,ers);
        }


        private async Task<bool> VerificaDipendenzaCodRubrica(string systeIdUoAppartenenza, string codiceRubrica)
        {
            bool dipRub = false;

            var res = await (from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
             where c.VAR_COD_RUBRICA != null && c.VAR_COD_RUBRICA.ToUpper().Equals(codiceRubrica.ToUpper())
             select new
             {
                 c.SYSTEM_ID,
                 c.CHA_TIPO_URP
             }).FirstOrDefaultAsync();
            if(res != null)
            {
                if (res.CHA_TIPO_URP == "U")
                {
                    dipRub = await this.VerificaDipendenzaUo(systeIdUoAppartenenza, res.SYSTEM_ID.ToString());
                }
                if (res.CHA_TIPO_URP == "R")
                {
                    dipRub = await this.VerificaDipendenzaRuolo(systeIdUoAppartenenza, res.SYSTEM_ID.ToString());
                }
                if (res.CHA_TIPO_URP == "P")
                {
                    dipRub = await this.VerificaDipendenzaUtente(systeIdUoAppartenenza, res.SYSTEM_ID.ToString());
                }
            }
            return dipRub;
        }
            
        private async Task<bool> VerificaDipendenzaUtente(string systeIdUoAppartenenza, string systemId)
        {
            bool result = false;


            var query = await (from pg in this._dbContext.PeopleGroupEntities.AsNoTracking()
                         from g in this._dbContext.GroupEntities.AsNoTracking()
                         from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                         let sq = (from cl in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                   where cl.SYSTEM_ID == systemId.AsLong()
                                   select c.ID_PEOPLE).ToList()
                         where sq.Contains(pg.PEOPLE_SYSTEM_ID) &&
                             pg.GROUPS_SYSTEM_ID == g.SYSTEM_ID &&
                             c.VAR_COD_RUBRICA != null &&
                             g.GROUP_ID != null && g.GROUP_ID.ToUpper().Equals(c.VAR_COD_RUBRICA.ToUpper()) &&
                             !pg.DTA_FINE.HasValue
                         select c.ID_UO).ToListAsync();

            foreach (var u in query)
            {
                result = await this.VerificaDipendenzaUo(systeIdUoAppartenenza, u != null ? u.ToString() : null);
                if (result)
                {
                    break;
                }

            }

            return result;
        }


        private async Task<bool> VerificaDipendenzaRuolo(string systeIdUoAppartenenza, string systemId)
        {
            bool result = false;
            var res = await (from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                             where c.SYSTEM_ID == systemId.AsLong()
                             select c.ID_UO).FirstOrDefaultAsync();

            if(res != null)
            {
                result = await this.VerificaDipendenzaUo(systeIdUoAppartenenza,res.ToString());
            }
            return result;
        }


        private async Task<bool> VerificaDipendenzaUo(string systeIdUoAppartenenza, string systemId)
        {
            bool result = false;

            while (true)
            {
                var queryRes = await (from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                      where c.SYSTEM_ID == systemId.AsLong()
                                      select c.ID_PARENT).FirstOrDefaultAsync();
                if(queryRes != null)
                {
                    if (queryRes == systeIdUoAppartenenza.AsLong())
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        systemId = queryRes.ToString();
                    }
                }
                else
                {
                    break;
                }
                
            }

            return result;
        }

        private async Task<string> GetIdUoAppartenenza(string codiceUoAppartenenza)
        {
            var res = await ( from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 where c.VAR_COD_RUBRICA != null && codiceUoAppartenenza.ToUpper().Equals(c.VAR_COD_RUBRICA.ToUpper())
                                 select c.SYSTEM_ID
                                 ).FirstOrDefaultAsync();

            return res.ToString();
        }

        private async Task<ArrayList> GetListaUOSmistamentoRubrica(string idRegistro)
        {
            var inUoSmistamento = await (from s in this._dbContext.UoSmistamentoEntities.AsNoTracking()
                                         where s.ID_REGISTRO != null && s.ID_REGISTRO == idRegistro.AsLong()
                                         select s.ID_UO).ToListAsync();
            
            var res = await ( from cg in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                              where !cg.DTA_FINE.HasValue && inUoSmistamento.Contains(cg.SYSTEM_ID)
                                select new
                                {
                                    ID = cg.SYSTEM_ID ,
                                    CODICE_UO = cg.VAR_COD_RUBRICA,
                                    DESCRIZIONE_UO =cg.VAR_DESC_CORR 
                                }).ToListAsync();
            var uo = new DocsPaVO.Smistamento.UOSmistamento();
            ArrayList listSmis = new();
            res.ForEach(u =>
            {
                uo = new DocsPaVO.Smistamento.UOSmistamento();
                uo.ID = u.ID.ToString();
                uo.Codice = u.CODICE_UO;
                uo.Descrizione = u.DESCRIZIONE_UO;

                listSmis.Add(uo);
                
            });

            return listSmis;

        }

        private void SetVisibleCheckBoxElement(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ref List<ElementoRubrica> ers)
        {
            if (qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_MODELLO_TRASM ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO ||
                //qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF ||
                qr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO ||
                qr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE
                )
            {
                foreach (ElementoRubrica er in ers)
                {
                    er.isVisibile = !er.disabledTrasm;
                }
            }
        }
        private async Task<List<ElementoRubrica>> GetElementiRubricaPaging(DocsPaVO.rubrica.ParametriRicercaRubrica qc, int firstRowNum, int maxRowForPage,DocsPaVO.utente.InfoUtente user)
        {
            bool noFiltroAoo = await this.IsFiltroAooEnabled();
            string idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant,true);
            long tot = 0;
            long total = 0;
            long totale = 0;
            List<ElementoRubrica> elements = new();
            bool predicateApplied = false;

            if (qc.parent != null && qc.parent != "")
            {
                string chaTipoIe = string.Empty;
                switch (qc.tipoIE)
                {
                    case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    default:
                        chaTipoIe = "I";
                        break;

                    case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                        chaTipoIe = "E";
                        break;
                }
                int corr_types = 0;
                corr_types += (qc.doUo ? 1 : 0);
                corr_types += (qc.doRuoli ? 2 : 0);
                corr_types += (qc.doUtenti ? 4 : 0);

                await this.SPGetChildren(idAmm, chaTipoIe,qc.parent, corr_types);
            }
            else
            {
                var predicate = PredicateBuilder.New<RetQuery>();
                var subPredicate = PredicateBuilder.New<RetQuery>();
                bool subPredicateSet = false;
                IQueryable<RetQuery>? baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                                   join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                                   from d in dettj.DefaultIfEmpty()
                                                   join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                                   from c in caj.DefaultIfEmpty()
                                                   join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                                   from dt in dtypej.DefaultIfEmpty()
                                                   select new RetQuery()
                                                   {
                                                       VarCodRubrica = a.VAR_COD_RUBRICA,
                                                       Canale = dt.DESCRIPTION,
                                                       VarDescCorr = a.VAR_DESC_CORR,
                                                       Interno = a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                                                       ChaTipoCorr = a.CHA_TIPO_CORR,
                                                       ChaTipoUrp = a.CHA_TIPO_URP,
                                                       SystemId = a.SYSTEM_ID,
                                                       CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()), 
                                                       IdRegistro = a.ID_REGISTRO,
                                                       ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                                       DtaFine = a.DTA_FINE,
                                                       VarNome = a.VAR_NOME,
                                                       VarCognome = a.VAR_COGNOME,
                                                       Email = a.VAR_EMAIL,
                                                       VarCodFisc = d.VAR_COD_FISC,
                                                       VarCodPi = d.VAR_COD_PI,
                                                       IdPeople = a.ID_PEOPLE,
                                                       IdPeopleListe = a.ID_PEOPLE_LISTE,
                                                       IdGruppoListe = a.ID_GRUPPO_LISTE,
                                                       IdAmm = a.ID_AMM,
                                                       ChaTipoIe = a.CHA_TIPO_IE,
                                                       ChaSysRole = a.CHA_SYSTEM_ROLE
                                                   });
                IQueryable<RetQuery>? tempBaseQuery = baseQuery;
                // ###############     MODELLI DI TRASMISSIONE    #################
                if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM ||
                    qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE ||
                    qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE)&&
                    (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO))
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        join rl in this._dbContext.RuoloRegistroEntities.AsNoTracking() on a.SYSTEM_ID equals rl.ID_RUOLO_IN_UO into rlj
                        from r in rlj.DefaultIfEmpty()
                        join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                        from d in dettj.DefaultIfEmpty()
                        join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                        from c in caj.DefaultIfEmpty()
                        join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                        from dt in dtypej.DefaultIfEmpty()
                        where r != null && r.ID_REGISTRO == qc.caller.IdRegistro.AsLong()
                        select new RetQuery()
                        {
                            VarCodRubrica = a.VAR_COD_RUBRICA,
                            Canale = dt.DESCRIPTION,
                            VarDescCorr = a.VAR_DESC_CORR,
                            Interno = a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                            ChaTipoCorr = a.CHA_TIPO_CORR,
                            ChaTipoUrp = a.CHA_TIPO_URP,
                            SystemId = a.SYSTEM_ID,
                            CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                            IdRegistro = a.ID_REGISTRO,
                            ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                            DtaFine = a.DTA_FINE,
                            VarNome = a.VAR_NOME,
                            VarCognome = a.VAR_COGNOME,
                            Email = a.VAR_EMAIL,
                            VarCodFisc = d.VAR_COD_FISC,
                            VarCodPi = d.VAR_COD_PI,
                            IdPeople = a.ID_PEOPLE,
                            IdPeopleListe = a.ID_PEOPLE_LISTE,
                            IdGruppoListe = a.ID_GRUPPO_LISTE,
                            IdAmm = a.ID_AMM,
                            ChaTipoIe = a.CHA_TIPO_IE,
                            ChaSysRole = a.CHA_SYSTEM_ROLE
                        });
                }

                //############### Ruolo Interoperabilit� nomail ####################
                if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL || 
                    qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG) &&
                    (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) &&
                    (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty))
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 join rl in this._dbContext.RuoloRegistroEntities.AsNoTracking() on a.SYSTEM_ID equals rl.ID_RUOLO_IN_UO into rlj
                                 from r in rlj.DefaultIfEmpty()
                                 join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                 from d in dettj.DefaultIfEmpty()
                                 join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                 from c in caj.DefaultIfEmpty()
                                 join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                 from dt in dtypej.DefaultIfEmpty()
                                 where r != null && r.ID_REGISTRO == qc.caller.IdRegistro.AsLong()
                                 select new RetQuery()
                                 {
                                     VarCodRubrica = a.VAR_COD_RUBRICA,
                                     Canale = dt.DESCRIPTION,
                                     VarDescCorr = a.VAR_DESC_CORR,
                                     Interno = a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                                     ChaTipoCorr = a.CHA_TIPO_CORR,
                                     ChaTipoUrp = a.CHA_TIPO_URP,
                                     SystemId = a.SYSTEM_ID,
                                     CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                     IdRegistro = a.ID_REGISTRO,
                                     ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                     DtaFine = a.DTA_FINE,
                                     VarNome = a.VAR_NOME,
                                     VarCognome = a.VAR_COGNOME,
                                     VarCodFisc = d.VAR_COD_FISC,
                                     VarCodPi = d.VAR_COD_PI,
                                     Email = a.VAR_EMAIL,
                                     IdPeople = a.ID_PEOPLE,
                                     IdPeopleListe = a.ID_PEOPLE_LISTE,
                                     IdGruppoListe = a.ID_GRUPPO_LISTE,
                                     IdAmm = a.ID_AMM,
                                     ChaTipoIe = a.CHA_TIPO_IE,
                                     ChaSysRole = a.CHA_SYSTEM_ROLE
                                 });
                }


                if (this.CheckCallType(qc.calltype))
                {
                    if (qc.doRubricaComune)
                    {
                        predicate = predicate.And( row => 
                        (( string.IsNullOrEmpty(row.ChaTipoCorr) || !row.ChaTipoCorr.Equals("C") ) ||
                        ( row.ChaTipoCorr != null && row.ChaTipoCorr.Equals("C") && row.DtaFine.HasValue)) &&
                        ( row.IdAmm == null || row.IdAmm == idAmm.AsLong() ));
                    }
                    else
                    {
                        predicate = predicate.And(row =>
                        ((string.IsNullOrEmpty(row.ChaTipoCorr) || !row.ChaTipoCorr.Equals("C"))) &&
                        (row.IdAmm == null || row.IdAmm == idAmm.AsLong() ));
                    }
                }
                else
                {
                    predicate = predicate.And(row =>
                        ((string.IsNullOrEmpty(row.ChaTipoCorr) || !row.ChaTipoCorr.Equals("C"))) &&
                        (row.IdAmm == null || row.IdAmm == idAmm.AsLong()));
                }

                switch (qc.tipoIE)
                {
                    case DocsPaVO.addressbook.TipoUtente.INTERNO:
                        if (qc.doListe || qc.doRF)
                            predicate = predicate.And(row => (row.ChaTipoIe == null || row.ChaTipoIe.Equals("I")));
                        else
                            predicate = predicate.And(row => (row.ChaTipoIe != null && row.ChaTipoIe.Equals("I")));
                        break;

                    case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                        if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE)
                            || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM)
                            || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO
                            )
                        {
                            predicate = predicate.And(row => (row.ChaTipoIe != null && row.ChaTipoIe.Equals("E")));
                        }
                        break;

                    case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                        break;
                }

                if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                    predicate = this.GetCodEqualsPredicate(predicate, qc.codice);
                else if (qc.codice != null && qc.codice != "")
                    predicate = this.GetCodContainsPredicate(predicate, qc.codice);
                    //predicate = predicate.And(row => row.VarCodRubrica != null && row.VarCodRubrica.ToUpper().Contains(qc.codice.ToUpper()));

                if (qc.descrizione != null && qc.descrizione != "")
                    predicate = predicate.And(row => !string.IsNullOrEmpty(row.VarDescCorr) && EF.Functions.Like(row.VarDescCorr.ToUpper(), $"%{qc.descrizione.ToUpper()}%"));

                if (qc.citta != null && qc.citta != "")
                {
                    predicate = predicate.And(row => 
                        this._dbContext.DettGlobaliEntities.AsNoTracking()
                        .Where(dett => dett.ID_CORR_GLOBALI == row.SystemId && dett.VAR_CITTA != null && EF.Functions.Like(dett.VAR_CITTA.ToUpper(), $"%{qc.citta.ToUpper()}%"))
                        .Select(dett => dett.SYSTEM_ID).Any()
                    );
                }

                if (qc.localita != null && qc.localita != "")
                {
                    predicate = this.GetLocalitaPredicate(predicate, qc.localita.ToUpper());
                }

                if (!string.IsNullOrEmpty(qc.email) && string.IsNullOrEmpty(qc.noteEmail))
                {
                    predicate = predicate.And(row => row.Email != null && EF.Functions.Like(row.Email.ToUpper(), $"%{qc.email.ToUpper()}%"));
                }
                else if (!string.IsNullOrEmpty(qc.noteEmail))
                {
                    predicate = this.GetMailDescPredicate(predicate, qc.noteEmail.ToUpper(), qc.email.ToUpper());
                }

                if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                {
                    predicate = this.GetCodiceFPredicate(predicate, qc.codiceFiscale.ToUpper());
                }

                if (qc.partitaIva != null && qc.partitaIva != "")
                {
                    predicate = this.GetPIvaPredicate(predicate,qc.partitaIva.ToUpper());
                }

                if (qc.systemId != null && qc.systemId != "")
                {
                    predicate = predicate.And(row => row.SystemId == qc.systemId.AsLong());
                }

                if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doRF)
                {
                    List<string> tipiUrp = new List<string>();

                    if (qc.doUo)
                        tipiUrp.Add("U");
                    if (qc.doRuoli)
                        tipiUrp.Add("R");
                    if (qc.doUtenti)
                        tipiUrp.Add("P");
                    if (qc.doRF)
                        tipiUrp.Add("F");
                    //predicate = predicate.And(row => (row.ChaTipoUrp != null && tipiUrp.Contains(row.ChaTipoUrp))  || (row.ChaTipoCorr != null && row.ChaTipoCorr.Equals("C")));
                    predicate = predicate.And(row => (row.ChaTipoUrp != null && tipiUrp.Contains(row.ChaTipoUrp)));
                }
                else if (qc.doRubricaComune)
                {
                    predicate = predicate.And(row => row.ChaTipoCorr != null && row.ChaTipoCorr.Equals("C"));
                }
                else 
                {
                    if (!qc.doListe)
                        return elements;
                }

                if(!this.CheckCallTypeEx(qc.calltype))
                {
                    predicate = predicate.And(row => !row.DtaFine.HasValue);
                }

                if (!qc.extSystems)
                    predicate = predicate.And( row => row.ChaSysRole != null && !row.ChaSysRole.Equals("1"));

                if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO)
                        && qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca))
                {
                    var idRegs = new List<string>();

                    foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                    {
                        idRegs.Add(ricerca.Trim());
                    }

                    predicate = predicate.And(row => row.IdRegistro == null || idRegs.Contains(row.IdRegistro.ToString()));
                }

                if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INGRESSO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE) &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO) &&
                        (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty) &&
                        (qc.doUo || qc.doRuoli || qc.doUtenti))
                {
                    bool chaRf = false;
                    if (qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                    {
                        DocsPaVO.utente.Registro reg = await this.GetRegistro(qc.caller.filtroRegistroPerRicerca);
                        if (reg != null && reg.chaRF == "1")
                        {
                            chaRf = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !chaRf)
                    {
                        var idRegs = new List<string>();

                        foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                        {
                            idRegs.Add(ricerca.Trim());
                        }

                        predicate = predicate.And(row => row.IdRegistro == null || idRegs.Contains(row.IdRegistro.ToString()));
                    }

                    if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && chaRf)
                    {
                        var idRegs = new List<string>();

                        foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                        {
                            idRegs.Add(ricerca.Trim());
                        }
                        predicate = predicate.And(row => row.IdRegistro != null && idRegs.Contains(row.IdRegistro.ToString()));
                    }

                    if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !chaRf)
                    {
                        predicate = predicate.And(row => row.IdRegistro == null);
                    }
                }

                // Se si usa la rubrica da GESTIONE RUBRICA vengono restituiti solamente
                //corrispondenti esterni all'amministrazione creati sul registri visibili al ruolo corrente

                if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO)
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_NO_UO)
                {
                    bool chaRf = false;

                    if (qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                    {
                        DocsPaVO.utente.Registro reg = await this.GetRegistro(qc.caller.filtroRegistroPerRicerca);
                        if (reg != null && reg.chaRF == "1")
                        {
                            chaRf = true;
                        }
                    }

                    if (!noFiltroAoo)
                    {
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !chaRf)
                        {
                            var idRegs = new List<string>();

                            foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                            {
                                idRegs.Add(ricerca.Trim());
                            }

                            predicate = predicate.And(row => row.IdRegistro == null || idRegs.Contains(row.IdRegistro.ToString()));
                        }

                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && chaRf)
                        {
                            var idRegs = new List<string>();

                            foreach (var ricerca in qc.caller.filtroRegistroPerRicerca.Split(','))
                            {
                                idRegs.Add(ricerca.Trim());
                            }

                            predicate = predicate.And(row => row.IdRegistro != null && idRegs.Contains(row.IdRegistro.ToString()));
                        }

                        if(string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !chaRf)
                        {
                            predicate = predicate.And( row => row.IdRegistro == null);
                        }

                        //bool nessunSottoposto = false;
                        if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                        {
                            DocsPaVO.utente.Ruolo mioRuolo = (await this._mediator.Send(new Application.Requests.GetRuoloById(qc.caller.IdRuolo))).Output;
                            DocsPaVO.trasmissione.TipoOggetto tipo = new DocsPaVO.trasmissione.TipoOggetto();
                            List<DocsPaVO.utente.Ruolo> roles = await this.GetCorrGlobRuoInf(mioRuolo,null,null,tipo);
                            roles.Add(mioRuolo);
                            List<string> rolesWithOnlySysId = new();

                            if (roles.Count != 0)
                            {
                                roles.ForEach((r)=>rolesWithOnlySysId.Add(r.systemId.ToString()));
                                predicate = predicate.And(row => rolesWithOnlySysId.Contains(row.SystemId.ToString()));
                            }
                            //else
                            //{
                            //    nessunSottoposto = true;
                            //}
                        }

                        if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                        {
                            baseQuery = baseQuery.Where(predicate).OrderByDescending(row => row.ChaTipoUrp).ThenBy(row => row.VarDescCorr).ThenBy(row => row.SystemId);
                            predicateApplied = true;
                        }
                        else
                        {
                            baseQuery = baseQuery.Where(predicate).OrderByDescending(row => row.ChaTipoUrp).ThenBy(row => row.VarDescCorr);
                            predicateApplied = true;
                        }
                    }
                }

                var listPredicate = PredicateBuilder.New<RetQuery>();
                var rolesPredicate = PredicateBuilder.New<RetQuery>();
                var uoPredicate = PredicateBuilder.New<RetQuery>();
                var rfPredicate = PredicateBuilder.New<RetQuery>();

                //####### Liste di distribuzione ######
                if (qc.doListe)
                {
                    var email = qc.email != null ? qc.email.ToUpper(): string.Empty ; 

                    IQueryable<RetQuery>? listBaseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                                           join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                                           from d in dettj.DefaultIfEmpty()
                                                           join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                                           from c in caj.DefaultIfEmpty()
                                                           join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                                           from dt in dtypej.DefaultIfEmpty()
                                                           where a.ID_AMM == idAmm.AsLong() && a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("L") && a.VAR_EMAIL == null
                                                           select new RetQuery()
                                                           {
                                                               VarCodRubrica = a.VAR_COD_RUBRICA,
                                                               Canale = dt.DESCRIPTION,
                                                               VarDescCorr = a.VAR_DESC_CORR,
                                                               Interno = a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                                                               ChaTipoCorr = a.CHA_TIPO_CORR,
                                                               ChaTipoUrp = a.CHA_TIPO_URP,
                                                               SystemId = a.SYSTEM_ID,
                                                               CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                                               IdRegistro = a.ID_REGISTRO,
                                                               ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                                               DtaFine = a.DTA_FINE,
                                                               VarNome = a.VAR_NOME,
                                                               VarCognome = a.VAR_COGNOME,
                                                               Email = a.VAR_EMAIL,
                                                               VarCodFisc = d.VAR_COD_FISC,
                                                               VarCodPi = d.VAR_COD_PI,
                                                               IdPeople = a.ID_PEOPLE,
                                                               IdPeopleListe = a.ID_PEOPLE_LISTE,
                                                               IdGruppoListe = a.ID_GRUPPO_LISTE,
                                                               IdAmm = a.ID_AMM,
                                                               ChaTipoIe = a.CHA_TIPO_IE,
                                                               ChaSysRole = a.CHA_SYSTEM_ROLE
                                                           });
                    bool filterListByCode = false;
                    bool applyListPred = false;
                    bool onlyLists = false;
                    // ricerca di solo liste
                    if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doRF)
                    {
                        onlyLists = true;
                        if (qc.doRubricaComune)
                        {
                            // in questo caso devo riutilizzare la UNION con gli elementi della RC
                            listPredicate = listPredicate.And(row => row.IdAmm == idAmm.AsLong() && row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("L"));

                            var listSubPredicateUn = PredicateBuilder.New<RetQuery>();
                            bool listSubPredicateSetUn = false;
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (user != null && user.idGruppo != null && !user.idGruppo.Equals("")))
                            {
                                bool res = false;

                                if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                {
                                    res = true;
                                    listSubPredicateUn = listSubPredicateUn.And(row => row.IdPeopleListe == qc.caller.IdUtente.AsLong() && row.IdGruppoListe == null);
                                    listSubPredicateSetUn = true;
                                    applyListPred = true;
                                }

                                if (user != null && user.idGruppo != null && !user.idGruppo.Equals(""))
                                {
                                    if (res)
                                    {
                                        listSubPredicateUn = listSubPredicateUn.Or(row => row.IdGruppoListe == user.idGruppo.AsLong() && row.IdPeopleListe == null);
                                    }
                                    else
                                    {
                                        listSubPredicateUn = listSubPredicateUn.And(row => row.IdGruppoListe == user.idGruppo.AsLong() && row.IdPeopleListe == null);
                                    }
                                    res = true;
                                    listSubPredicateSetUn = true;
                                    applyListPred = true;
                                }

                                if (res)
                                {
                                    listSubPredicateSetUn = true;
                                    listSubPredicateUn = listSubPredicateUn.Or(row => row.IdPeopleListe == null && row.IdGruppoListe == null);
                                    applyListPred = true;
                                }
                                else
                                {
                                    listSubPredicateSetUn = true;
                                    listSubPredicateUn = listSubPredicateUn.And(row => row.IdPeopleListe == null && row.IdGruppoListe == null);
                                    applyListPred = true;
                                }
                                if (listSubPredicateSetUn)
                                {
                                    listPredicate = listPredicate.And(listSubPredicateUn);
                                    applyListPred = true;
                                }
                            }

                            if (qc.localita != null && qc.localita != "")
                            {
                                listPredicate = this.GetLocalitaPredicate(listPredicate, qc.localita.ToUpper());
                            }
                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                listPredicate = this.GetCodiceFPredicate(listPredicate, qc.codiceFiscale.ToUpper());
                            }
                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                listPredicate = this.GetPIvaPredicate(listPredicate, qc.partitaIva.ToUpper());
                            }
                            if (!string.IsNullOrEmpty(qc.email) && string.IsNullOrEmpty(qc.noteEmail))
                            {
                                listPredicate = listPredicate.And(row => row.Email != null && row.Email.ToUpper().Contains(qc.email.ToUpper()));
                            }
                            else if (!string.IsNullOrEmpty(qc.noteEmail))
                            {
                                listPredicate = this.GetMailDescPredicate(listPredicate, qc.noteEmail.ToUpper(), qc.email.ToUpper());
                            }
                        }
                        else
                        {
                            // in questo caso NON utilizzo la UNION
                            predicate = PredicateBuilder.New<RetQuery>();
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (user != null && user.idGruppo != null && !user.idGruppo.Equals("")))
                            {
                                bool res = false;
                                if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                {
                                    subPredicateSet = true;
                                    subPredicate = subPredicate.And(row =>
                                    row.IdGruppoListe == null &&
                                    row.IdPeopleListe != null &&
                                    row.IdPeopleListe.ToString().Equals(qc.caller.IdUtente));
                                    res = true;
                                }
                                if (res)
                                {
                                    if (user != null && user.idGruppo != null && !user.idGruppo.Equals(""))
                                    {
                                        if (!res)
                                        {
                                            subPredicate = subPredicate.And(row => row.IdGruppoListe == user.idGruppo.AsLong() && row.IdPeopleListe == null);
                                        }
                                        else
                                        {
                                            subPredicate = subPredicate.Or(row => row.IdGruppoListe == user.idGruppo.AsLong() && row.IdPeopleListe == null);
                                        }
                                        subPredicateSet = true;
                                        res = true;
                                    }
                                }

                                if (res)
                                {
                                    subPredicate = subPredicate.Or(row => row.IdPeopleListe == null && row.IdGruppoListe == null && !row.DtaFine.HasValue);
                                    subPredicateSet = true;
                                }
                                else
                                {
                                    subPredicate = subPredicate.And(row => row.IdPeopleListe == null && row.IdGruppoListe == null && !row.DtaFine.HasValue);
                                    subPredicateSet = true;
                                }

                                if (subPredicateSet)
                                {
                                    listPredicate = listPredicate.And(subPredicate);
                                }
                            }
                            else
                            {
                                listPredicate = listPredicate.And(row => row.IdAmm == idAmm.AsLong() && row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("L"));

                                var listSubPredicate = PredicateBuilder.New<RetQuery>();
                                bool listSubPredicateSet = false;
                                if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (user != null && user.idGruppo != null && !user.idGruppo.Equals("")))
                                {
                                    bool res = false;

                                    if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                    {
                                        res = true;
                                        listSubPredicate = listSubPredicate.And(row => row.IdPeopleListe == qc.caller.IdUtente.AsLong() && row.IdGruppoListe == null);
                                        listSubPredicateSet = true;
                                    }

                                    if (user != null && user.idGruppo != null && !user.idGruppo.Equals(""))
                                    {
                                        if (res)
                                        {
                                            listSubPredicate = listSubPredicate.Or(row => row.IdGruppoListe == user.idGruppo.AsLong() && row.IdPeopleListe == null);
                                        }
                                        else
                                        {
                                            listSubPredicate = listSubPredicate.And(row => row.IdGruppoListe == user.idGruppo.AsLong() && row.IdPeopleListe == null);
                                        }
                                        res = true;
                                        listSubPredicateSet = true;
                                    }

                                    if (res)
                                    {
                                        listSubPredicateSet = true;
                                        listSubPredicate = listSubPredicate.Or(row => row.IdPeopleListe == null);
                                    }
                                    else
                                    {
                                        listSubPredicateSet = true;
                                        listSubPredicate = listSubPredicate.And(row => row.IdPeopleListe == null);
                                    }
                                    if (listSubPredicateSet)
                                    {
                                        listPredicate = listPredicate.And(listSubPredicate);
                                    }
                                }
                                filterListByCode = true;
                            }
                        }
                    }
                    else
                    {
                        //E' una ricerca combinata liste con utenti, ruoli o uffici
                        listPredicate = listPredicate.And(row => row.IdAmm == idAmm.AsLong() && row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("L"));

                        var listSubPredicateUn = PredicateBuilder.New<RetQuery>();
                        bool listSubPredicateSetUn = false;
                        if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (user != null && user.idGruppo != null && !user.idGruppo.Equals("")))
                        {
                            bool res = false;

                            if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                            {
                                res = true;
                                listSubPredicateUn = listSubPredicateUn.And(row => row.IdPeopleListe == qc.caller.IdUtente.AsLong() && row.IdGruppoListe == null);
                                listSubPredicateSetUn = true;
                                applyListPred = true;
                            }

                            if (user != null && user.idGruppo != null && !user.idGruppo.Equals(""))
                            {
                                if (res)
                                {
                                    listSubPredicateUn = listSubPredicateUn.Or(row => row.IdGruppoListe == user.idGruppo.AsLong() && row.IdPeopleListe == null);
                                }
                                else
                                {
                                    listSubPredicateUn = listSubPredicateUn.And(row => row.IdGruppoListe == user.idGruppo.AsLong() && row.IdPeopleListe == null);
                                }
                                res = true;
                                listSubPredicateSetUn = true;
                                applyListPred = true;
                            }

                            if (res)
                            {
                                listSubPredicateSetUn = true;
                                listSubPredicateUn = listSubPredicateUn.Or(row => row.IdPeopleListe == null && row.IdGruppoListe == null);
                                applyListPred = true;
                            }
                            else
                            {
                                listSubPredicateSetUn = true;
                                listSubPredicateUn = listSubPredicateUn.And(row => row.IdPeopleListe == null && row.IdGruppoListe == null);
                                applyListPred = true;
                            }
                            if (listSubPredicateSetUn)
                            {
                                listPredicate = listPredicate.And(listSubPredicateUn);
                                applyListPred = true;
                            }
                        }

                        if (qc.localita != null && qc.localita != "")
                        {
                            listPredicate = this.GetLocalitaPredicate(listPredicate, qc.localita.ToUpper());
                        }
                        if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                        {
                            listPredicate = this.GetCodiceFPredicate(listPredicate, qc.codiceFiscale.ToUpper());
                        }
                        if (qc.partitaIva != null && qc.partitaIva != "")
                        {
                            listPredicate = this.GetPIvaPredicate(listPredicate, qc.partitaIva.ToUpper());
                        }
                        if (!string.IsNullOrEmpty(qc.email) && string.IsNullOrEmpty(qc.noteEmail))
                        {
                            listPredicate = listPredicate.And(row => row.Email != null && row.Email.ToUpper().Contains(qc.email.ToUpper()));
                        }
                        else if (!string.IsNullOrEmpty(qc.noteEmail))
                        {
                            listPredicate = this.GetMailDescPredicate(listPredicate, qc.noteEmail.ToUpper(), qc.email.ToUpper());
                        }
                    }
 
                    if (filterListByCode)
                    {
                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        {
                            applyListPred = true;
                            listPredicate = listPredicate.And(row => row.VarCodRubrica != null && row.VarCodRubrica.ToUpper().Equals(qc.codice.ToUpper()));
                        }
                        else if (qc.codice != null && qc.codice != "")
                        {
                            applyListPred = true;
                            listPredicate = listPredicate.And(row => row.VarCodRubrica != null && row.VarCodRubrica.ToUpper().Contains(qc.codice.ToUpper()));
                        }

                        //Descrizione
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            applyListPred = true;
                            listPredicate = listPredicate.And(row => row.VarDescCorr.ToUpper().Contains(qc.descrizione.ToUpper()));
                        }
                    }
                    else
                    {
                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        {
                            applyListPred = true;
                            listPredicate = this.GetCodEqualsPredicate(listPredicate, qc.codice);
                            //predicate = predicate.And(row => row.VarCodRubrica != null && qc.codice.ToUpper().Equals(row.VarCodRubrica.ToUpper()));
                        }
                        else if (qc.codice != null && qc.codice != "")
                        {
                            applyListPred = true;
                            listPredicate = this.GetCodContainsPredicate(listPredicate, qc.codice);
                            //predicate = predicate.And(row => row.VarCodRubrica != null && row.VarCodRubrica.ToUpper().Equals(qc.codice.ToUpper()));
                        }

                        //Descrizione
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            applyListPred = true;
                            listPredicate = listPredicate.And(row => !string.IsNullOrEmpty(row.VarDescCorr) && EF.Functions.Like(row.VarDescCorr.ToUpper(), $"%{qc.descrizione.ToUpper()}%"));
                        }
                    }

                    if (listPredicate.IsStarted)
                        listBaseQuery = listBaseQuery.Where(listPredicate);


                    if(predicateApplied)
                    {
                        if (!onlyLists)
                            baseQuery = baseQuery.Union(listBaseQuery);
                        else
                            baseQuery = listBaseQuery;

                    }
                    else
                    {
                        if (!onlyLists)
                            baseQuery = baseQuery.Where(predicate).Union(listBaseQuery);
                        else
                        {
                            if(!qc.doRubricaComune)
                                baseQuery = listBaseQuery;
                            else
                                baseQuery = baseQuery.Where(predicate).Union(listBaseQuery);
                        }
                        predicateApplied = true;
                    }
                }

                if (qc.doRF)
                {
                    if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && qc.doRF)
                    {
                        predicate = predicate.And(row => (row.IdAmm == null || row.IdAmm == user.idAmministrazione.AsLong()) &&
                        row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("F"));
                        baseQuery = baseQuery.Where(predicate);
                        predicateApplied = true;
                    }
                    else
                    {
                        IQueryable<RetQuery>? rfBaseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                                             join dett in this._dbContext.DettGlobaliEntities.AsNoTracking() on a.SYSTEM_ID equals dett.ID_CORR_GLOBALI into dettj
                                                             from d in dettj.DefaultIfEmpty()
                                                             join ca in this._dbContext.CanaleCorrEntities.AsNoTracking() on a.SYSTEM_ID equals ca.ID_CORR_GLOBALE into caj
                                                             from c in caj.DefaultIfEmpty()
                                                             join dtype in this._dbContext.DocumentTypesEntities.AsNoTracking() on c.ID_DOCUMENTTYPE equals dtype.SYSTEM_ID into dtypej
                                                             from dt in dtypej.DefaultIfEmpty()
                                                             select new RetQuery()
                                                             {
                                                                 VarCodRubrica = a.VAR_COD_RUBRICA,
                                                                 Canale = dt.DESCRIPTION,
                                                                 VarDescCorr = a.VAR_DESC_CORR,
                                                                 Interno = a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                                                                 ChaTipoCorr = a.CHA_TIPO_CORR,
                                                                 ChaTipoUrp = a.CHA_TIPO_URP,
                                                                 SystemId = a.SYSTEM_ID,
                                                                 CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(a.ID_REGISTRO.GetValueOrDefault()),
                                                                 IdRegistro = a.ID_REGISTRO,
                                                                 ChaDisabledTrasm = a.CHA_DISABLED_TRASM,
                                                                 DtaFine = a.DTA_FINE,
                                                                 VarNome = a.VAR_NOME,
                                                                 VarCognome = a.VAR_COGNOME,
                                                                 Email = a.VAR_EMAIL,
                                                                 VarCodFisc = d.VAR_COD_FISC,
                                                                 VarCodPi = d.VAR_COD_PI,
                                                                 IdPeople = a.ID_PEOPLE,
                                                                 IdPeopleListe = a.ID_PEOPLE_LISTE,
                                                                 IdGruppoListe = a.ID_GRUPPO_LISTE,
                                                                 IdAmm = a.ID_AMM,
                                                                 ChaTipoIe = a.CHA_TIPO_IE,
                                                                 ChaSysRole = a.CHA_SYSTEM_ROLE
                                                             });

                        rfPredicate = rfPredicate.And(row => row.IdAmm == idAmm.AsLong() && row.ChaTipoUrp != null && row.ChaTipoUrp.Equals("F"));

                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        {
                            //rfPredicate = rfPredicate.And(row => row.VarCodRubrica != null &&qc.codice.ToUpper().Equals(row.VarCodRubrica.ToUpper()));
                            rfPredicate = this.GetCodEqualsPredicate(rfPredicate, qc.codice);
                        }
                        else if (qc.codice != null && qc.codice != "")
                        {
                            //rfPredicate = rfPredicate.And(row => row.VarCodRubrica != null && row.VarCodRubrica.ToUpper().Contains(qc.codice.ToUpper()));
                            rfPredicate = this.GetCodContainsPredicate(rfPredicate, qc.codice);
                        }

                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            rfPredicate = rfPredicate.And(row => row.VarDescCorr != null && row.VarDescCorr.ToUpper().Contains(qc.descrizione.ToUpper()));
                        }

                        if (qc.localita != null && qc.localita != "")
                        {
                            rfPredicate = this.GetLocalitaPredicate(rfPredicate, qc.localita.ToUpper());
                        }

                        if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                        {
                            rfPredicate = this.GetCodiceFPredicate(rfPredicate, qc.codiceFiscale.ToUpper());
                        }

                        if (qc.partitaIva != null && qc.partitaIva != "")
                        {
                            rfPredicate = this.GetPIvaPredicate(rfPredicate, qc.partitaIva);
                        }

                        if (!string.IsNullOrEmpty(qc.email) && string.IsNullOrEmpty(qc.noteEmail))
                        {
                            rfPredicate = rfPredicate.And(row => row.Email != null && qc.email.ToUpper().Contains(row.Email.ToUpper()));
                        }
                        else if (!string.IsNullOrEmpty(qc.noteEmail))
                        {
                            rfPredicate = this.GetMailDescPredicate(rfPredicate, qc.noteEmail.ToUpper(), qc.email.ToUpper());
                        }
                        baseQuery = baseQuery.Where(predicate).Union(rfBaseQuery.Where(rfPredicate));
                        predicateApplied = true;
                    }
                }

                if (!predicateApplied)
                    baseQuery = baseQuery.Where(predicate);

                bool first = true;
                long rowNumber = 0;
                if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                {
                    baseQuery = baseQuery.Where(row => !string.IsNullOrEmpty(row.VarCodRubrica) && EF.Functions.Like(row.VarCodRubrica.ToUpper(), $"{qc.codice.ToUpper()}"));
                }
                else if (qc.codice != null && qc.codice != "")
                {
                    baseQuery = baseQuery.Where(row => !string.IsNullOrEmpty(row.VarCodRubrica) && EF.Functions.Like(row.VarCodRubrica.ToUpper(), $"%{qc.codice.ToUpper()}%"));
                }
                /*
                var page = (firstRowNum % maxRowForPage) + 1;
                var skip = (page * maxRowForPage) - maxRowForPage;
                baseQuery = baseQuery.Skip(skip).Take(maxRowForPage);
                */
                if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                {
                    baseQuery = baseQuery.OrderByDescending(x => x.ChaTipoUrp)
                                .ThenBy(x => x.VarDescCorr).ThenBy(x => x.SystemId);
                }
                else
                {
                    baseQuery = baseQuery.OrderByDescending(x => x.ChaTipoUrp)
                                .ThenBy(x => x.VarDescCorr);
                }

                foreach (var e in baseQuery)
                {
                    rowNumber++;
                    DocsPaVO.rubrica.ElementoRubrica er = new DocsPaVO.rubrica.ElementoRubrica();
                    er.codice = e.VarCodRubrica ?? string.Empty;
                    er.descrizione = e.VarDescCorr ?? string.Empty;
                    er.interno = e.Interno == 1;
                    er.tipo = e.ChaTipoUrp ?? string.Empty;
                    er.systemId = e.SystemId.ToString();
                    er.canale = e.Canale ?? string.Empty;
                    er.has_children = false;
                    er.codiceRegistro = e.CodRegRf ?? string.Empty;
                    er.idRegistro = e.IdRegistro != null ? e.IdRegistro.ToString() : null;
                    er.disabledTrasm = e.ChaDisabledTrasm != null ? e.ChaDisabledTrasm.Equals("1") : false;

                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO)
                    {
                        if(e.DtaFine != null && !string.IsNullOrEmpty(e.DtaFine.ToString()))
                        {
                            er.disabled = false;
                        }
                    }
                    else
                    {
                        if (e.DtaFine != null && !string.IsNullOrEmpty(e.DtaFine.ToString()))
                        {
                            er.disabled = true;
                        }
                    }
                    bool inRubComune = e.ChaTipoCorr != null && e.ChaTipoCorr.Equals("C");
                    er.isDisbledAndRC = inRubComune && er.disabled; 
                    er.nome = e.VarNome;
                    er.cognome = e.VarCognome;
                    er.codiceFiscale = e.VarCodFisc;
                    er.partitaIva = e.VarCodPi;
                    er.idPeople = e.IdPeople != null ? e.IdPeople.ToString() : string.Empty;
                    er.idPeopleLista = e.IdPeopleListe != null ? e.IdPeopleListe.ToString() : string.Empty;
                    er.idGruppoLista = e.IdGruppoListe != null ? e.IdGruppoListe.ToString() : string.Empty;

                    if (rowNumber >= firstRowNum + 1 && rowNumber <= firstRowNum + maxRowForPage)
                        elements.Add(er);
                    if(rowNumber > firstRowNum + maxRowForPage)
                    {
                        break;
                    }
                };
                if (tot != 0)
                    totale = tot;
                else
                    totale = total;
                

            }

            

            return elements;
        }

        private async Task<List<DocsPaVO.utente.Ruolo>> GetCorrGlobRuoInf(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
        {
            List<string> children = await this.GetChildrenUO(ruolo);
            children.Add(ruolo.uo.systemId);

            var baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                         from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                         select new CorrRuoInf()
                                         {
                                             SystemId = a.SYSTEM_ID,
                                             IdGruppo = a.ID_GRUPPO,
                                             NumLivello = b.NUM_LIVELLO,
                                             VarDescRuolo = a.VAR_DESC_CORR,
                                             VarCodice = a.VAR_CODICE,
                                             VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                             IdParent = a.ID_PARENT,
                                             IdUo = a.ID_UO,
                                             IdAmm = a.ID_AMM,
                                             BSysId = b.SYSTEM_ID,
                                             AIdTipRuolo = a.ID_TIPO_RUOLO,
                                             ChaTipoIe = a.CHA_TIPO_IE
                                         });


            var res = await this.GetCorrGlobRuolo(baseQuery,tipoOggetto, idRegistro, idNodoTitolario, ruolo, children, DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString());

            return res;
        }

        private async Task<List<DocsPaVO.utente.Ruolo>> GetCorrGlobRuoPariLiv(DocsPaVO.utente.Ruolo ruolo, string idRegistro, string idNodoTitolario, DocsPaVO.trasmissione.TipoOggetto tipoOggetto)
        {
            List<string> children = await this.GetChildrenUO(ruolo);
            children.Add(ruolo.uo.systemId);

            var baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                             from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                             select new CorrRuoInf()
                             {
                                 SystemId = a.SYSTEM_ID,
                                 IdGruppo = a.ID_GRUPPO,
                                 NumLivello = b.NUM_LIVELLO,
                                 VarDescRuolo = a.VAR_DESC_CORR,
                                 VarCodice = a.VAR_CODICE,
                                 VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                 IdParent = a.ID_PARENT,
                                 IdUo = a.ID_UO,
                                 IdAmm = a.ID_AMM,
                                 BSysId = b.SYSTEM_ID,
                                 AIdTipRuolo = a.ID_TIPO_RUOLO
                             });


            var res = await this.GetCorrGlobRuolo(baseQuery, tipoOggetto, idRegistro, idNodoTitolario, ruolo, children, DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString());

            return res;
        }
        private async Task<List<DocsPaVO.utente.Ruolo>> GetCorrGlobRuolo(IQueryable<CorrRuoInf> baseQuery,DocsPaVO.trasmissione.TipoOggetto tipoOggetto, string idRegistro, string idNodoTitolario, DocsPaVO.utente.Ruolo ruolo,List<string> childrenUO, string tipoGerarchia)
        {
            var predicate = PredicateBuilder.New<CorrRuoInf>();
            var predicateSubQueryNumLivello = PredicateBuilder.New<CorrRuoInf>();


            if (ruolo.idAmministrazione != null && !ruolo.idAmministrazione.ToString().Equals(""))
            {
                predicate = predicate.And( r => r.IdAmm == ruolo.idAmministrazione.AsLong() );
            }

            (string? estKey,bool found) = await this._configurationService.TryGetValue<string>("EST_VIS_SUP_PARI_LIV");

            if(!found || string.IsNullOrEmpty(estKey) || estKey.Equals("0"))
            {
                if (childrenUO.Count != null && childrenUO.Count > 0)
                {
                    predicate = predicate.And(row => row.IdUo != null && childrenUO.Contains(row.IdUo.ToString()));
                }

                if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
                    predicate = predicate.And(row => row.NumLivello > ruolo.livello.AsLong());
                else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
                    predicate = predicate.And(row => row.NumLivello < ruolo.livello.AsLong());
                else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
                    predicate = predicate.And(row => row.NumLivello == ruolo.livello.AsLong());
                predicate = predicate.And(row => row.BSysId == row.AIdTipRuolo);

            }
            else
            {
                predicateSubQueryNumLivello = predicateSubQueryNumLivello.And(row => row.IdUo == ruolo.uo.systemId.AsLong());
                if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
                {
                    if (childrenUO.Count != null && childrenUO.Count > 1)
                    {
                        predicate = predicate.And(row => row.IdUo != null && childrenUO.Contains(row.IdUo.ToString())
                        && row.NumLivello <= ruolo.livello.AsLong());
                    }
                    else
                    {
                        predicate = predicate.And(row => row.IdUo == ruolo.uo.systemId.AsLong());
                    }
                }
                else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
                {
                    if (childrenUO != null && childrenUO.Count > 1)
                    {
                        if (childrenUO.Count != null && childrenUO.Count > 1)
                        {
                            predicate = predicate.And(row => row.IdUo != null && childrenUO.Contains(row.IdUo.ToString())
                            && row.NumLivello >= ruolo.livello.AsLong());
                        }
                        else
                        {
                            predicate = predicate.And(row => row.IdUo == ruolo.uo.systemId.AsLong());
                        }
                    }

                }
                if (childrenUO.Count != null && childrenUO.Count > 1)
                {
                    if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
                        predicateSubQueryNumLivello = predicateSubQueryNumLivello.And(row => row.NumLivello < ruolo.livello.AsLong());
                    else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
                        predicateSubQueryNumLivello = predicateSubQueryNumLivello.And(row => row.NumLivello > ruolo.livello.AsLong());
                    else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
                        predicateSubQueryNumLivello = predicateSubQueryNumLivello.And(row => row.NumLivello == ruolo.livello.AsLong());
                }
                else
                {
                    if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.SUPERIORE.ToString()))
                        predicate = predicate.And(row => row.NumLivello < ruolo.livello.AsLong());
                    else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString()))
                        predicate = predicate.And(row => row.NumLivello > ruolo.livello.AsLong());
                    else if (tipoGerarchia.Equals(DocsPaVO.trasmissione.TipoGerarchia.PARILIVELLO.ToString()))
                        predicate = predicate.And(row => row.NumLivello == ruolo.livello.AsLong());
                }

                if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
                {
                    if (idRegistro != null && idRegistro != "" && !await this.IsFiltroAooEnabled())
                    {
                        predicate = predicate.And(row => row.SystemId == row.RegIdRuolo && row.RegIdReg == idRegistro.AsLong());
                    }
                }
                else
                {
                    predicate = predicate.And(row => row.SecPersonOrGroup == row.IdGruppo && row.SecThing == idNodoTitolario.AsLong() && row.SecAccessRights > 0);
                    if (idRegistro != null && idRegistro != "")
                    {
                        predicate = predicate.And(row => row.RegIdRuolo == row.SystemId && row.RegIdReg == idRegistro.AsLong());
                    }


                }
                predicate = predicate.Or(predicateSubQueryNumLivello);

            }

            if (tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO)
            {
                if (idRegistro != null && idRegistro != "" && !await this.IsFiltroAooEnabled())
                {
                    baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                 from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                 from c in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                 select new CorrRuoInf()
                                 {
                                     SystemId = a.SYSTEM_ID,
                                     IdGruppo = a.ID_GRUPPO,
                                     NumLivello = b.NUM_LIVELLO,
                                     VarDescRuolo = a.VAR_DESC_CORR,
                                     VarCodice = a.VAR_CODICE,
                                     VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                     IdParent = a.ID_PARENT,
                                     IdUo = a.ID_UO,
                                     IdAmm = a.ID_AMM,
                                     BSysId = b.SYSTEM_ID,
                                     AIdTipRuolo = a.ID_TIPO_RUOLO,
                                     RegIdRuolo = c.ID_RUOLO_IN_UO,
                                     RegIdReg = c.ID_REGISTRO,
                                     ChaTipoIe = a.CHA_TIPO_IE
                                 });
                }
                else
                {
                    if (idRegistro != null)
                    {
                        baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                     from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                     from c in this._dbContext.SecurityEntities.AsNoTracking()
                                     from d in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                     select new CorrRuoInf()
                                     {
                                         SystemId = a.SYSTEM_ID,
                                         IdGruppo = a.ID_GRUPPO,
                                         NumLivello = b.NUM_LIVELLO,
                                         VarDescRuolo = a.VAR_DESC_CORR,
                                         VarCodice = a.VAR_CODICE,
                                         VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                         IdParent = a.ID_PARENT,
                                         IdUo = a.ID_UO,
                                         IdAmm = a.ID_AMM,
                                         BSysId = b.SYSTEM_ID,
                                         AIdTipRuolo = a.ID_TIPO_RUOLO,
                                         RegIdRuolo = d.ID_RUOLO_IN_UO,
                                         RegIdReg = d.ID_REGISTRO,
                                         SecAccessRights = c.ACCESSRIGHTS,
                                         SecPersonOrGroup = c.PERSONORGROUP,
                                         SecThing = c.THING,
                                         ChaTipoIe = a.CHA_TIPO_IE

                                     });
                    }
                    else
                    {
                        baseQuery = (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                     from b in this._dbContext.TipoRuoloEntities.AsNoTracking()
                                     from d in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                     select new CorrRuoInf()
                                     {
                                         SystemId = a.SYSTEM_ID,
                                         IdGruppo = a.ID_GRUPPO,
                                         NumLivello = b.NUM_LIVELLO,
                                         VarDescRuolo = a.VAR_DESC_CORR,
                                         VarCodice = a.VAR_CODICE,
                                         VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                         IdParent = a.ID_PARENT,
                                         IdUo = a.ID_UO,
                                         IdAmm = a.ID_AMM,
                                         BSysId = b.SYSTEM_ID,
                                         AIdTipRuolo = a.ID_TIPO_RUOLO,
                                         RegIdRuolo = d.ID_RUOLO_IN_UO,
                                         RegIdReg = d.ID_REGISTRO,
                                         ChaTipoIe = a.CHA_TIPO_IE
                                     });
                    }
                }

            }
            List<DocsPaVO.utente.Ruolo> roles = new();
            var result = await baseQuery.Where(predicate).ToListAsync();
            result.ForEach((role) =>
            {
                DocsPaVO.utente.Ruolo ruolo = new();
                ruolo.systemId = role.SystemId.ToString();
                ruolo.descrizione = role.VarDescRuolo;
                ruolo.codiceCorrispondente = role.VarCodiceRubrica;
                if(role.IdUo != null)
                    ruolo.uo = this.GetParents(role.IdUo.ToString(),ruolo);
                if(role.IdGruppo != null)
                    ruolo.idGruppo = role.IdGruppo.ToString();
                roles.Add(ruolo);
            });

            return roles;
        }


        private DocsPaVO.utente.UnitaOrganizzativa GetParents(string id_parent, DocsPaVO.utente.Ruolo ruoloInit)
        {
            DocsPaVO.utente.UnitaOrganizzativa result = ruoloInit.uo;
            if (result == null) return null;

            while (result.systemId != null && result.systemId.Equals(id_parent))
            {
                result = result.parent;
                if (result == null) return null;
            }

            return result;
        }

        private class CorrRuoInf
        {
            public long? CIdRuoInUo { get; set; }
            public string? CodRegRf { get; set; }
            public long SystemId { get; set; }
            public long? IdGruppo { get; set; }
            public long? NumLivello { get; set; }
            public string? VarDescRuolo { get; set; }
            public string? VarCodice { get; set; }
            public string? VarCodiceRubrica { get; set; }
            public long? IdParent{ get; set; }
            public long? IdUo { get; set; }
            public long? IdAmm { get; set; }
            public long BSysId { get; set; }
            public long? AIdTipRuolo { get; set; }
            public long? RegIdRuolo { get; set; }
            public long? RegIdReg { get; set; }
            public long? SecPersonOrGroup { get; set; }
            public long? SecThing { get; set; }
            public long? SecAccessRights { get; set; }
            public string? ChaTipoIe { get; set; }
        }

        private async Task<List<string>> GetChildrenUO(Ruolo ruolo)
        {
            var childrenUo = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
             where a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("U") &&
             a.NUM_LIVELLO > ruolo.uo.livello.AsLong()
             select new UoResult()
             {
                 SystemId = a.SYSTEM_ID,
                 IdParent = a.ID_PARENT
             }).ToListAsync();

            var lista = this.GetChildrenUO2(ruolo.uo.systemId,childrenUo);

            return lista;
        }

        private List<string> GetChildrenUO2(string idUO,List<UoResult> queryRes)
        {
            List<UoResult> children = queryRes.Where(c => c.IdParent == idUO.AsLong()).ToList();
            List<string> result = new ();

            children.ForEach((c)=>
            {
                result.Add(c.SystemId.ToString());
                var lista2 = this.GetChildrenUO2(c.SystemId.ToString(),queryRes);
                lista2.ForEach((c2) =>
                {
                    result.Add(c2);
                });
            
            });

            return result;
        }


        private class UoResult
        {
            public long SystemId { get; set; }
            public long? IdParent { get; set; }
        }


        private async Task<DocsPaVO.utente.Registro> GetRegistro(string idRegistro)
        {
            DocsPaVO.utente.Registro registro = null;

            if (!(idRegistro != null && !idRegistro.Equals("")))
            {
                return null;
            }

            var reg = await this._dbContext.RegistroEntities.AsNoTracking().Where(r => r.SYSTEM_ID == idRegistro.AsLong()).Select(r => new
            {
                r.SYSTEM_ID,
                r.VAR_CODICE,
                r.CHA_STATO,
                r.ID_AMM,
                r.VAR_DESC_REGISTRO,
                r.VAR_EMAIL_REGISTRO,
                DTA_OPEN = r.DTA_OPEN.AsDateFormat(),
                DTA_CLOSE = r.DTA_CLOSE.AsDateFormat(),
                DTA_ULTIMO_PROTOCOLLO = r.DTA_ULTIMO_PROTO.AsDateFormat(),
                r.CHA_AUTO_INTEROP,
                r.CHA_RF
            }).FirstOrDefaultAsync();

            if(reg != null)
            {
                registro = new()
                {
                    systemId = reg.SYSTEM_ID.ToString(),
                    codRegistro = reg.VAR_CODICE,
                    stato = reg.CHA_STATO,
                    idAmministrazione = reg.ID_AMM != null ? reg.ID_AMM.ToString() : null,
                    descrizione = reg.VAR_DESC_REGISTRO,
                    email = reg.VAR_EMAIL_REGISTRO,
                    dataApertura = reg.DTA_OPEN,
                    dataChiusura = reg.DTA_CLOSE,
                    dataUltimoProtocollo = reg.DTA_ULTIMO_PROTOCOLLO,
                    autoInterop = reg.CHA_AUTO_INTEROP,
                    chaRF = reg.CHA_RF
                };
            }

            if(registro != null)
            {
                var regAmm = await (from a in this._dbContext.AmministraEntities.AsNoTracking()
                 from r in this._dbContext.RegistroEntities.AsNoTracking()
                 where (r.ID_AMM == a.SYSTEM_ID) &&
                 (r.SYSTEM_ID == idRegistro.AsLong())
                 select new
                 {
                     a.SYSTEM_ID,
                     a.VAR_CODICE_AMM,
                 }).FirstOrDefaultAsync();
                
                if(regAmm != null)
                {
                    registro.codice = regAmm.SYSTEM_ID.ToString();
                    registro.codAmministrazione = regAmm.VAR_CODICE_AMM;
                }
            
            }

            return registro;
        }


        /*
        private ExpressionStarter<RetQuery>? GetCodContainsPredicate(ExpressionStarter<RetQuery>? predicate, string filter)
        {
            return predicate.And(row => row.VarCodRubrica != null &&
                            row.VarCodRubrica.ToUpper().Contains(filter.ToUpper()));
        }

        private ExpressionStarter<RetQuery>? GetCodEqualsPredicate(ExpressionStarter<RetQuery>? predicate, string filter)
        {
            return predicate.And(row => row.VarCodRubrica != null &&
                            row.VarCodRubrica.ToUpper().Equals(filter.ToUpper()));
        }
        */

        private ExpressionStarter<RetQuery>? GetCodContainsPredicate(ExpressionStarter<RetQuery>? predicate, string filter)
        {
            return predicate;
        }

        private ExpressionStarter<RetQuery>? GetCodEqualsPredicate(ExpressionStarter<RetQuery>? predicate, string filter)
        {
            return predicate;
        }

        private ExpressionStarter<RetQuery>? GetLocalitaPredicate(ExpressionStarter<RetQuery>? predicate,string filter)
        {

            return predicate.And(row =>
                        this._dbContext.DettGlobaliEntities.AsNoTracking()
                        .Where(dett => dett.ID_CORR_GLOBALI == row.SystemId && 
                        dett.VAR_LOCALITA != null &&
                        dett.VAR_LOCALITA.ToUpper().Contains(filter))
                        .Select(dett => dett.SYSTEM_ID).Any()
                    );
        }

        private ExpressionStarter<RetQuery>? GetCodiceFPredicate(ExpressionStarter<RetQuery>? predicate, string filter)
        {
            return predicate.And( row => this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(dett => dett.ID_CORR_GLOBALI == row.SystemId && dett.VAR_COD_FISC != null && dett.VAR_COD_FISC.ToUpper().Contains(filter)).Select(dett => dett.SYSTEM_ID).Any());
        }

        private ExpressionStarter<RetQuery>? GetPIvaPredicate(ExpressionStarter<RetQuery>? predicate, string filter)
        {
            return predicate = predicate.And(row => this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(dett => dett.ID_CORR_GLOBALI == row.SystemId && dett.VAR_COD_PI != null && dett.VAR_COD_PI.ToUpper().Contains(filter)).Select(dett => dett.SYSTEM_ID).Any());
        }

        private ExpressionStarter<RetQuery>? GetMailDescPredicate(ExpressionStarter<RetQuery>? predicate, string noteEmail,string email)
        {
            return predicate.And(row => this._dbContext.MailCorrEsterniEntities.AsNoTracking().Where(m =>
                                            m.ID_CORR ==  row.SystemId &&
                                            m.VAR_NOTE != null &&
                                             EF.Functions.Like(m.VAR_NOTE.ToUpper(), $"%{noteEmail.ToUpper()}%") &&
                                            m.VAR_EMAIL != null &&
                                            EF.Functions.Like(m.VAR_EMAIL.ToUpper(), $"%{email.ToUpper()}%"))
                                        .Select(m => m.SYSTEM_ID)
                                        .Any());
        }

        private async Task<bool> IsFiltroAooEnabled()
        {
            bool result = false;
            (string? keyVal, bool found) = await this._configurationService.TryGetValue<string>("NO_FILTRO_AOO");

            if (found)
            {
                result = !string.IsNullOrEmpty(keyVal) ? keyVal.Equals("1") : false;
            }
            return result;
        }
        private bool CheckCallTypeEx(DocsPaVO.rubrica.ParametriRicercaRubrica.CallType callType)
        {
            return (callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CREATOR ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_OWNER_AUTHOR ||
                         callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI) ;
        }



        private string Nvl(string? s1, string s2)
        {
            return s1 ?? s2;
        }

        private class RetQuery
        {
            public string? CodRegRf { get; set; }
            public string? VarCodRubrica { get; set; }
            public string? Canale { get; set; }
            public string? VarDescCorr { get; set; }
            public int Interno { get; set; }
            public string? ChaTipoCorr { get; set; }
            public string? ChaTipoUrp { get; set; }
            public long SystemId { get; set; }
            public long? IdRegistro { get; set; }
            public string? ChaDisabledTrasm { get; set; }
            public DateTime? DtaFine { get; set; }
            public string? VarNome { get; set; }
            public string? VarCognome { get; set; }
            public string? Email { get; set; }
            public string? VarCodFisc { get; set; }
            public string? VarCodPi { get; set; }
            public long? IdPeople { get; set; }
            public long? IdPeopleListe { get; set; }
            public long? IdGruppoListe { get; set; }
            public long? IdAmm { get; set; }
            public string? ChaTipoIe {  get; set; }
            public string? ChaSysRole { get; set; }

        }

        private bool CheckCallType(DocsPaVO.rubrica.ParametriRicercaRubrica.CallType callType)
        {
            return (callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CREATOR ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_OWNER_AUTHOR ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI ||
                        callType == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI);
        }
        private async Task<List<ElRubInfo>> SPGetChildren(string pIdAmm,string pChaTipoIE,string pVarCodRubrica,long pCorrTypes)
        {

            List<ElRubInfo>? res = new();

            var corr = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_AMM == pIdAmm.AsLong()
            && c.CHA_TIPO_IE != null
            && c.CHA_TIPO_IE.Equals(pChaTipoIE)
            && c.VAR_COD_RUBRICA != null
            && c.VAR_COD_RUBRICA.Equals(pVarCodRubrica)
            && !c.DTA_FINE.HasValue).Select(c => new
            {
                vTipo = c.CHA_TIPO_URP,
                vSystemId = c.SYSTEM_ID,
                vIdGruppo = c.ID_GRUPPO
            }).FirstOrDefaultAsync();

            if(corr != null)
            {
                if (corr.vTipo!= null && corr.vTipo.Equals("U"))
                {
                        res = await this._dbContext.CorrGlobaliEntities.AsNoTracking().
                        Where(c => c.ID_PARENT == corr.vSystemId && !c.DTA_FINE.HasValue && (pCorrTypes & 1)>0)
                        .Select(c => new ElRubInfo()
                        {
                            VarCodRubrica = c.VAR_COD_RUBRICA,
                            VarDescCorr = c.VAR_DESC_CORR,
                            Interno = c.CHA_TIPO_IE != null && c.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                            ChaTipoUrp = c.CHA_TIPO_URP,
                            SystemId = c.SYSTEM_ID
                        }).Union(this._dbContext.CorrGlobaliEntities.AsNoTracking().
                        Where(c => c.CHA_TIPO_URP != null && c.CHA_TIPO_URP.Equals("R") && c.ID_UO == corr.vSystemId && !c.DTA_FINE.HasValue && (pCorrTypes & 2) > 0)
                        .Select(c => new ElRubInfo()
                        {
                            VarCodRubrica = c.VAR_COD_RUBRICA,
                            VarDescCorr = c.VAR_DESC_CORR,
                            Interno = c.CHA_TIPO_IE != null && c.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                            ChaTipoUrp = c.CHA_TIPO_URP,
                            SystemId = c.SYSTEM_ID
                        })).ToListAsync();
                }
                else
                {
                    var queryListDist = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                        .Where(p => p.GROUPS_SYSTEM_ID == corr.vIdGruppo && !p.DTA_FINE.HasValue).Select(p => p.PEOPLE_SYSTEM_ID).ToListAsync();

                     res = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => queryListDist.Contains(c.ID_PEOPLE) && 
                        !c.DTA_FINE.HasValue && 
                        c.CHA_TIPO_URP != null && 
                        c.CHA_TIPO_URP.Equals("L") &&
                        (pCorrTypes & 4) > 0) 
                    .Select(c => new ElRubInfo()
                        {
                            VarCodRubrica = c.VAR_COD_RUBRICA,
                            VarDescCorr = c.VAR_DESC_CORR,
                            Interno = c.CHA_TIPO_IE != null && c.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                            ChaTipoUrp = c.CHA_TIPO_URP,
                            SystemId = c.SYSTEM_ID
                        }).Distinct().ToListAsync();
                }

            }
            return res;
        }

        private class ElRubInfo
        {
            public string? VarCodRubrica { get; set; }
            public string? VarDescCorr { get; set; }
            public int Interno { get; set; }
            public string? ChaTipoUrp { get; set; }
            public long SystemId { get; set; }
        }

        #endregion

    }
}
