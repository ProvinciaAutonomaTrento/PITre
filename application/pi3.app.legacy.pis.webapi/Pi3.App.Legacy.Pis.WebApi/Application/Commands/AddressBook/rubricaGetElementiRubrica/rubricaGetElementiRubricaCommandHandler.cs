// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services;
using Pi3.Infrastructure.Legacy.EF.Entities;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using CallTypes = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType;
using DocsPaVO.rubrica;
using System.Collections;
using DocsPaVO.trasmissione;
using Pi3.Core.Services.Configuration;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteCompletoBySystemId;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetRuolo;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetRagione;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils.GetOrSetAacToken;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.rubricaGetElementiRubrica
{
    public class rubricaGetElementiRubricaCommandHandler : IRequestHandler<rubricaGetElementiRubricaCommand, rubricaGetElementiRubricaCommandResponse>
    {
        public rubricaGetElementiRubricaCommandHandler(
            ILogger<rubricaGetElementiRubricaCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext, IRubricaComuneService rubricaComuneService, IHttpContextAccessor httpContextAccessor,
            IConfigurationService configurationService
            )
        {
            this._configurationService = configurationService;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._rubricaComuneService = rubricaComuneService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<rubricaGetElementiRubricaCommandResponse> Handle(rubricaGetElementiRubricaCommand request, CancellationToken cancellationToken)
        {
            List<ElementoRubrica> output = null;
            try
            {
                output = await this.RicercaInRubricaStandard(request.Qc, request.U,request.SmistamentoRubrica);

                if (string.IsNullOrWhiteSpace(request.Qc.localita) &&
                    request.Qc.tipoIE != DocsPaVO.addressbook.TipoUtente.INTERNO &&
                    request.Qc.doRubricaComune
                    )
                {
                    var rubricaComuneElements = await this.RicercaInRubricaComune(request.Qc, request.U);

                    if (rubricaComuneElements.Any()) output.AddRange(rubricaComuneElements);

                    output = output.OrderBy(a => a.descrizione.ToUpper().Trim()).ToList();

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new rubricaGetElementiRubricaCommandResponse()
            {
                Output = output?.ToArray()
            };
        }

        #region Private members
        protected ILogger<rubricaGetElementiRubricaCommandHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IRubricaComuneService _rubricaComuneService;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IConfigurationService _configurationService;
        private readonly string BearerPrefix = "Bearer ";

        protected async Task<string?> GetAuthToken()
        {
            string token = string.Empty;
            var aacResp = await this._mediator.Send(new GetOrSetAacTokenCommand());
            if(aacResp != null)
                token = aacResp.Token;
            return token;
        }
        #region Search filter smistamento
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
            if (res != null)
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

            if (res != null)
            {
                result = await this.VerificaDipendenzaUo(systeIdUoAppartenenza, res.ToString());
            }
            return result;
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

            var ruoliRes = await (
                from u in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                from r in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                from p in this._dbContext.PeopleEntities.AsNoTracking()
                from pg in this._dbContext.PeopleGroupEntities.AsNoTracking()
                where (r.ID_GRUPPO == pg.GROUPS_SYSTEM_ID) &&
                (u.ID_PEOPLE == p.SYSTEM_ID) &&
                (p.SYSTEM_ID == pg.PEOPLE_SYSTEM_ID) &&
                (u.CHA_TIPO_URP != null && u.CHA_TIPO_URP.Equals("P")) &&
                (!u.DTA_FINE.HasValue) &&
                (!r.DTA_FINE.HasValue) &&
                (!pg.DTA_FINE.HasValue) &&
                (!u.DTA_FINE.HasValue)
                orderby u.VAR_COD_RUBRICA
                select new
                {
                    COD_UTENTE = u.VAR_COD_RUBRICA,
                    COD_RUOLO = r.VAR_COD_RUBRICA,
                    DESC_RUOLO = r.VAR_DESC_CORR
                }).ToListAsync();

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
        private async Task<bool> VerificaDipendenzaUo(string systeIdUoAppartenenza, string systemId)
        {
            bool result = false;

            while (true)
            {
                var queryRes = await (from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                      where c.SYSTEM_ID == systemId.AsLong()
                                      select c.ID_PARENT).FirstOrDefaultAsync();
                if (queryRes != null)
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
        private async Task<ArrayList> GetListaUOSmistamentoRubrica(string idRegistro)
        {
            var inUoSmistamento = await (from s in this._dbContext.UoSmistamentoEntities.AsNoTracking()
                                         where s.ID_REGISTRO != null && s.ID_REGISTRO == idRegistro.AsLong()
                                         select s.ID_UO).ToListAsync();

            var res = await (from cg in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                             where !cg.DTA_FINE.HasValue && inUoSmistamento.Contains(cg.SYSTEM_ID)
                             select new
                             {
                                 ID = cg.SYSTEM_ID,
                                 CODICE_UO = cg.VAR_COD_RUBRICA,
                                 DESCRIZIONE_UO = cg.VAR_DESC_CORR
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

        private async Task<string> GetIdUoAppartenenza(string codiceUoAppartenenza)
        {
            var res = await (from c in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                             where c.VAR_COD_RUBRICA != null && codiceUoAppartenenza.ToUpper().Equals(c.VAR_COD_RUBRICA.ToUpper())
                             select c.SYSTEM_ID
                                 ).FirstOrDefaultAsync();

            return res.ToString();
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
                    DocsPaVO.utente.Corrispondente ruoloProt = (await this._mediator.Send(new AddressbookGetCorrispondenteCompletoBySystemIdCommand()
                    {
                        SystemId = idRuolo,
                        TipoIE = "INTERNO",
                        u = utente

                    } )).output;
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
                                                //caso in cui la Uo del ruolo è SUPERIORE a quella del protocollatore
                                                if (!tableUoSmistamento.ContainsKey(er.codice))
                                                {
                                                    //vuol dire che l'elemento rubrica NON è nella DPA_UO_SMISTAMENTO
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
                                            //caso in cui la Uo del ruolo è SUPERIORE a quella del ruolo che protocolla.
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
                                            //se la Uo è sottoposta a quella del protocollista allora rendo visibile l'utente
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
            catch (Exception ex)
            {
                result = false;
            }
            return (result, ers);
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

                if (c1 && c2)
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
                else if (c1 && !c2)
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
                this._logger.LogError(exception: ex, message: ex.Message);
            }


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

            var lista = this.GetChildrenUO2(ruolo.uo.systemId, childrenUo);

            return lista;
        }

        private List<string> GetChildrenUO2(string idUO, List<UoResult> queryRes)
        {
            List<UoResult> children = queryRes.Where(c => c.IdParent == idUO.AsLong()).ToList();
            List<string> result = new();

            children.ForEach((c) =>
            {
                result.Add(c.SystemId.ToString());
                var lista2 = this.GetChildrenUO2(c.SystemId.ToString(), queryRes);
                lista2.ForEach((c2) =>
                {
                    result.Add(c2);
                });

            });

            return result;
        }
        #region utils data structures 
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
            public long? IdParent { get; set; }
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
        private class UoResult
        {
            public long SystemId { get; set; }
            public long? IdParent { get; set; }
        }

        #endregion
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


            var res = await this.GetCorrGlobRuolo(baseQuery, tipoOggetto, idRegistro, idNodoTitolario, ruolo, children, DocsPaVO.trasmissione.TipoGerarchia.INFERIORE.ToString());

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
        private async Task<List<DocsPaVO.utente.Ruolo>> GetCorrGlobRuolo(IQueryable<CorrRuoInf> baseQuery, DocsPaVO.trasmissione.TipoOggetto tipoOggetto, string idRegistro, string idNodoTitolario, DocsPaVO.utente.Ruolo ruolo, List<string> childrenUO, string tipoGerarchia)
        {
            var predicate = PredicateBuilder.New<CorrRuoInf>();
            var predicateSubQueryNumLivello = PredicateBuilder.New<CorrRuoInf>();


            if (ruolo.idAmministrazione != null && !ruolo.idAmministrazione.ToString().Equals(""))
            {
                predicate = predicate.And(r => r.IdAmm == ruolo.idAmministrazione.AsLong());
            }

            (string? estKey, bool found) = await this._configurationService.TryGetValue<string>("EST_VIS_SUP_PARI_LIV");

            if (!found || string.IsNullOrEmpty(estKey) || estKey.Equals("0"))
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
                if (role.IdUo != null)
                    ruolo.uo = this.GetParents(role.IdUo.ToString(), ruolo);
                if (role.IdGruppo != null)
                    ruolo.idGruppo = role.IdGruppo.ToString();
                roles.Add(ruolo);
            });

            return roles;
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
        private async Task<List<ElementoRubrica>> FiltraTrasmissioni(DocsPaVO.rubrica.ParametriRicercaRubrica qr, List<ElementoRubrica> ers, DocsPaVO.utente.InfoUtente user)
        {
            List<ElementoRubrica> a = new List<ElementoRubrica>();
            List<Ruolo> ruoliAutorizzati = new();


            TipoOggetto tipoOggetto = (qr.ObjectType != null && qr.ObjectType.StartsWith("F:")) ? TipoOggetto.FASCICOLO : TipoOggetto.DOCUMENTO;
            string idNodoTitolario = (tipoOggetto == TipoOggetto.FASCICOLO) ? qr.ObjectType.Substring(2) : null;
            DocsPaVO.utente.Ruolo r = (await this._mediator.Send(new GetRuoloCommand()
            {
                IdCorrGlobali = qr.caller.IdRuolo
            })).Output;

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
                    RagioneTrasmissione rTo = (await this._mediator.Send(new GetRagioneCommand()
                    {
                        IdAmm = user.idAmministrazione,
                        TipoDest = "TO"
                    })).Output;
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
                        if (await this.UtenteIsAutorizzato((er.interno ? "I" : "E") + @"\" + er.codice, cRuoliAuth, user))
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

        private async Task<bool> UoIsAutorizzato(string cod, DocsPaVO.utente.Ruolo ruolo_caller, string[] ruoliAuth, DocsPaVO.utente.InfoUtente user)
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
        private async Task<Hashtable> GetRuoliUOSemplice(string idAmm)
        {
            Hashtable h = new Hashtable();
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

        private bool RuoloIsAutorizzato(string codice, string[] ruoliAutorizzati)
        {
            return ruoliAutorizzati.ToList().Contains(codice) == true;
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
                ers = await this.FiltraAoo(qr, ers, user);

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

        #endregion

        protected async Task<List<ElementoRubrica>> RicercaInRubricaStandard(ParametriRicercaRubrica qc, InfoUtente u, SmistamentoRubrica smistamentoRubrica)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant) ?? u.idAmministrazione;
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup) != null ? this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup) : u.idGruppo.AsLong();

            var elementiRubrica = new List<ElementoRubrica>();

            var predicate = PredicateBuilder.New<CorrGlobaliEntity>();

            var entities = new List<CorrWithCodReg>();

            if (!string.IsNullOrWhiteSpace(qc.parent))
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
                            .Where(x => x.CHA_TIPO_URP == "P"
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
                    predicate.And(x => this._dbContext.MailCorrEsterniEntities.Any
                        (mc => mc.VAR_NOTE.ToUpper().Contains(qc.noteEmailPISREST.ToUpper())
                            && mc.ID_CORR == x.SYSTEM_ID));

                if (!string.IsNullOrWhiteSpace(qc.emailsPISREST))
                    predicate.And(x => this._dbContext.MailCorrEsterniEntities.Any(
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
                        var listePredicate = PredicateBuilder.New<CorrGlobaliEntity>();

                        listePredicate.And(x => x.CHA_TIPO_URP == "L" && x.ID_AMM == idTenant.AsLong());

                        ApplyFilterListeDistribuzione(
                            listePredicate,
                            idTenant,
                            qc.caller is not null ? qc.caller.IdPeople : string.Empty,
                            u.idGruppo);

                        await this.ApplyBasicFilters(listePredicate, qc);

                        predicate.Or(listePredicate);
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
                        var rfPredicate = PredicateBuilder.New<CorrGlobaliEntity>();

                        rfPredicate.And(x => x.CHA_TIPO_URP == "F" && x.ID_AMM == idTenant.AsLong());

                        await this.ApplyBasicFilters(rfPredicate, qc);

                        predicate.Or(rfPredicate);
                    }
                }

                try
                {
                    entities = await this._dbContext.CorrGlobaliEntities
                                .Where(predicate)
                                .OrderByDescending(x => x.CHA_TIPO_URP)
                                .ThenBy(x => x.VAR_DESC_CORR).Select(c => new CorrWithCodReg()
                                {
                                    Corr = c,
                                    CodRegRf = IPi3DbContextMappedFunctions.GetCodReg(c.ID_REGISTRO.GetValueOrDefault())
                                })
                                .ToListAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            entities.ForEach(async x => elementiRubrica.Add(await this.GetElementoRubrica(x)));

            if (CheckCallTypesCheckboxVisibility(qc.calltype))
                elementiRubrica.ForEach(x => x.isVisibile = !x.disabledTrasm);

            elementiRubrica = await this.Dpa3SearchFilter(qc, elementiRubrica, smistamentoRubrica, u);

            return elementiRubrica;
        }

        protected class CorrWithCodReg
        {
            public CorrGlobaliEntity Corr { get; set; }
            public string? CodRegRf { get; set; }
        }

        protected async Task<List<ElementoRubrica>> RicercaInRubricaComune(ParametriRicercaRubrica qc, InfoUtente user)
        {
            var elementiRubrica = new List<ElementoRubrica>();

            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = qc.codice },
                new CriterioRicerca { Campo = CampiRicercaEnum.Denominazione, Valore = qc.descrizione },
                new CriterioRicerca { Campo = CampiRicercaEnum.Citta, Valore = qc.citta },
                //new CriterioRicerca { Campo = CampiRicercaEnum.Email, Valore = qc.email },
                new CriterioRicerca { Campo = CampiRicercaEnum.CodiceFiscale, Valore = qc.codiceFiscale },
                new CriterioRicerca { Campo = CampiRicercaEnum.PartitaIva, Valore = qc.partitaIva }
            };

            if (qc.rubricaEsterna is not null && qc.rubricaEsterna.Any())
            {
                criteriRicerca.Add(new CriterioRicerca
                {
                    Campo = CampiRicercaEnum.RubricaEsterna,
                    Valore = string.Join("@", qc.rubricaEsterna)
                });
            }
            List<Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente> rc = new();

            var response = await this._rubricaComuneService.Search(await this.GetAuthToken(), new SearchRequest
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

            rc.ForEach(async c =>
            {
                if (c != null)
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

            });
            return elementiRubrica;
        }
        private async Task<bool> InternoInAoo(List<string> regs, Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RubricaComune.Corrispondente corr, InfoUtente infoUtente)
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
                    .Where(x => x.VAR_LOCALITA.ToUpper().Contains(qc.localita.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                entities = entities.Where(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.codiceFiscale))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_COD_FISC.ToUpper().Contains(qc.codiceFiscale.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                entities = entities.Where(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.partitaIva))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_COD_PI.ToUpper().Contains(qc.partitaIva.ToUpper()))
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
                predicate.And(x => x.VAR_DESC_CORR.ToUpper().Contains(qc.descrizione.ToUpper()));

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
                    .Where(x => x.VAR_LOCALITA.ToUpper().Contains(qc.localita.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                predicate.And(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.codiceFiscale))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_COD_FISC.ToUpper().Contains(qc.codiceFiscale.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                predicate.And(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }

            if (!string.IsNullOrWhiteSpace(qc.partitaIva))
            {
                var dettGlobaliItems = await this._dbContext.DettGlobaliEntities.AsNoTracking()
                    .Where(x => x.VAR_COD_PI.ToUpper().Contains(qc.partitaIva.ToUpper()))
                    .Select(x => x.ID_CORR_GLOBALI).ToListAsync();

                predicate.And(x => dettGlobaliItems.Any(y => y == x.SYSTEM_ID));
            }
            predicate.And(x => !x.DTA_FINE.HasValue);
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

        protected async Task<ElementoRubrica> GetElementoRubrica(CorrWithCodReg entity)
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
                disabledTrasm = (entity.Corr.CHA_DISABLED_TRASM != null && entity.Corr.CHA_DISABLED_TRASM.ToString() == "1"),
                disabled = (entity.Corr.DTA_FINE != null && !string.IsNullOrEmpty(entity.Corr.DTA_FINE.ToString())),
                idPeople = entity.Corr.ID_PEOPLE.ToString(),
                nome = entity.Corr.VAR_NOME,
                cognome = entity.Corr.VAR_COGNOME,
                codiceRegistro = entity.CodRegRf ?? string.Empty
            };

            var dettGlobaliEntity = await this._dbContext.DettGlobaliEntities.FirstOrDefaultAsync(x => x.ID_CORR_GLOBALI == entity.Corr.SYSTEM_ID);

            if (dettGlobaliEntity is not null)
            {
                elemento.codiceFiscale = dettGlobaliEntity.VAR_COD_FISC;
                elemento.partitaIva = dettGlobaliEntity.VAR_COD_PI;
            }

            return elemento;
        }

        #endregion

    }
}
