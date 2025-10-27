// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.addressbook;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.rubrica;
using DocsPaVO.Smistamento;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.rubricaGetRootItems
{
    // Richiede libreria MediatR
    public class rubricaGetRootItemsHandler : IRequestHandler<Requests.rubricaGetRootItems, rubricaGetRootItemsResult>
    {
        #region Public Members

        public rubricaGetRootItemsHandler(ILogger<rubricaGetRootItemsHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<rubricaGetRootItemsResult> Handle(Requests.rubricaGetRootItems request, CancellationToken cancellationToken)
        {
            DocsPaVO.addressbook.TipoUtente tipoIE = request.tipoIE;
            DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = request.smistamentoRubrica;
            List<ElementoRubrica> ers = null;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            try
            {
                h_uo = await this.GetRuoliUtenteSemplice(idTenant);
                h_utenti = await this.GetRuoliUtenteSemplice(idTenant);
                ers = await this.GetRootItems(tipoIE, idTenant);

                if (smistamentoRubrica != null && smistamentoRubrica.smistamento.Equals("1"))
                {
                    if ((smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
                        ers = await this.FiltraPrimoSmistamento(smistamentoRubrica.idRegistro, smistamentoRubrica.ruoloProt.systemId, smistamentoRubrica.infoUt, ers);
                }

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new rubricaGetRootItemsResult(ers?.ToArray());

        }



        #endregion

        #region Private Members

        protected readonly ILogger<rubricaGetRootItemsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected Hashtable h_uo;
        protected Hashtable h_utenti;
        protected List<String> uoInAoo; 

        protected IMapper _mapper = null;

        private async Task<List<ElementoRubrica>> GetRootItems(TipoUtente tipoIE, long idTenant)
        {
            var entities = this._dbContext.CorrGlobaliEntities.Where(x => x.ID_AMM == idTenant && x.CHA_TIPO_URP.Equals("U") && (x.ID_PARENT == null || x.ID_PARENT == 0) && x.DTA_FINE == null);

            switch (tipoIE)
            {
                case TipoUtente.INTERNO:
                    entities = entities.Where(x => x.CHA_TIPO_IE.Equals("I"));
                    break;
                case TipoUtente.ESTERNO:
                    entities = entities.Where(x => x.CHA_TIPO_IE.Equals("E"));
                    break;
                case TipoUtente.GLOBALE:
                default:
                    break;
            }

            return _mapper.Map<List<ElementoRubrica>>(entities);
        }

        private async Task<List<ElementoRubrica>> FiltraPrimoSmistamento(string idRegistro, string idRuolo, InfoUtente infoUt, List<ElementoRubrica> ers)
        {
            List<UOSmistamento> listaUoSmistamento = new List<DocsPaVO.Smistamento.UOSmistamento>();
            Hashtable tableUoInterneAOO = new Hashtable();
            bool retvalue = true;

            this.uoInAoo = await this.GetUoInterneAoo(idRegistro);

            uoInAoo.ForEach(x =>
            {
                if (!tableUoInterneAOO.ContainsKey(x))
                    tableUoInterneAOO.Add(x, x);
            });

            listaUoSmistamento = await this.GetListaUOSmistamentoRubrica(idRegistro.AsLong());

            if (listaUoSmistamento.Any())
            {
                Corrispondente ruoloProt = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteCompletoBySystemId(idRuolo, TipoUtente.INTERNO, infoUt))).output;

                Hashtable tableUoSmistamento = new Hashtable();
                foreach (UOSmistamento item in listaUoSmistamento)
                    if (!tableUoSmistamento.ContainsKey(item.Codice))
                        tableUoSmistamento.Add(item.Codice, item);

                string idUoAppartenenza = await this.GetIdUoAppartenenza(((Ruolo)ruoloProt).uo.codiceRubrica);

                foreach (var er in ers)
                {
                    if (er != null && er.interno)
                    {
                        bool verificaDipendenzaCodRubrica = await this.VerificaDipendenzaCodRubrica(idUoAppartenenza.AsLong(), er.codice);
                        bool checkRuoliUtenti = this.CheckRuoliUtenti(er);
                        switch (er.tipo)
                        {
                            case "U":
                                if (er.isVisibile && tableUoInterneAOO.Contains(er.codice) && !verificaDipendenzaCodRubrica && !tableUoSmistamento.ContainsKey(er.codice))
                                {
                                    er.isVisibile = false;
                                }
                                break;
                            case "R":
                                if (er.isVisibile && checkRuoliUtenti && !verificaDipendenzaCodRubrica)
                                {
                                    er.isVisibile = false;
                                }
                                break;
                            case "P":
                                bool selectorVisibility = false;
                                if (!checkRuoliUtenti)
                                    selectorVisibility = true;
                                else if (er.isVisibile)
                                {
                                    if (verificaDipendenzaCodRubrica)
                                        selectorVisibility = true;
                                    er.isVisibile = selectorVisibility;
                                }
                                break;
                        }
                    }
                }
            }
            return ers;
        }

        private bool CheckRuoliUtenti(ElementoRubrica er)
        {
            string codUo = null;
            if (er.tipo == "R")
            {
                codUo = (string)h_utenti[er.codice];
            }
            else
            {
                string[] ruoli = (string[])h_utenti[er.codice];
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
                    if (CheckRuoliUtenti(err))
                        return true;
                }
                return false;
            }
            return this.uoInAoo.Contains(codUo);
        }

        private async Task<bool> VerificaDipendenzaCodRubrica(long idUoAppartenenza, string codice)
        {
            bool result = false;
            var entities = this._dbContext.CorrGlobaliEntities.Where(x => x.VAR_COD_RUBRICA.ToUpper().Equals(codice.ToUpper())).Select(x => new
            {
                SYSTEM_ID = x.SYSTEM_ID,
                CHA_TIPO_URP = x.CHA_TIPO_URP
            });

            foreach (var entity in entities)
            {
                switch (entity.CHA_TIPO_URP)
                {
                    case "U":
                        result = await this.VerificaDipendenzaUo(idUoAppartenenza, entity.SYSTEM_ID);
                        break;
                    case "R":
                        var idUo = await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == entity.SYSTEM_ID).Select(x => x.ID_UO).FirstOrDefaultAsync();
                        result = await this.VerificaDipendenzaUo(idUoAppartenenza, idUo ?? 0);
                        break;
                    case "P":
                        var peopleId = await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == entity.SYSTEM_ID).Select(x => x.ID_PEOPLE).FirstOrDefaultAsync();
                        var idPeopleList = await this._dbContext.PeopleGroupEntities
                            .Join(this._dbContext.GroupEntities, p => p.GROUPS_SYSTEM_ID, g => g.SYSTEM_ID, (p, g) => new { p, g })
                            .Join(this._dbContext.CorrGlobaliEntities, j1 => j1.g.GROUP_ID.ToUpper(), c => c.VAR_COD_RUBRICA.ToUpper(), (j1, c) => new { p = j1.p, g = j1.g, c })
                            .Where(x => x.p.PEOPLE_SYSTEM_ID == peopleId && x.p.DTA_FINE == null)
                            .Select(x => x.c.ID_UO)
                            .Distinct()
                            .ToListAsync();

                        foreach (var id in idPeopleList)
                        {
                            if (await this.VerificaDipendenzaUo(idUoAppartenenza, (long)id))
                                result = true;
                        }
                        break;
                }
            }
            return result;
        }

        private async Task<bool> VerificaDipendenzaUo(long idUoAppartenenza, long systemId)
        {
            return await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == systemId).Select(x => x.ID_PARENT).FirstOrDefaultAsync() == idUoAppartenenza;
        }

        private async Task<string> GetIdUoAppartenenza(string codiceUoAppartenenza)
        {
            return await this._dbContext.CorrGlobaliEntities.Where(x => x.VAR_COD_RUBRICA.ToUpper().Equals(codiceUoAppartenenza.ToUpper())).Select(x => x.SYSTEM_ID.ToString()).FirstOrDefaultAsync();
        }

        private async Task<List<UOSmistamento>> GetListaUOSmistamentoRubrica(long idRegistro)
        {
            List<long> regUos = await this._dbContext.UoSmistamentoEntities.Where(x => x.ID_REGISTRO == idRegistro).Select(x => (long)x.ID_UO).ToListAsync();

            var entities = this._dbContext.CorrGlobaliEntities.Where(x => x.DTA_FINE == null && regUos.Contains(x.SYSTEM_ID));

            return _mapper.Map<List<UOSmistamento>>(entities);
        }

        private async Task<List<string>> GetUoInterneAoo(string idRegistro)
        {
            List<long> regIds = idRegistro.Split(",").Select(x => x.AsLong()).ToList();

            var results = await this._dbContext.CorrGlobaliEntities
                .Join(this._dbContext.UoRegEnties, c => c.SYSTEM_ID, u => u.ID_UO, (c, u) => new { c, u })
                .Where(x => regIds.Contains(x.c.ID_REGISTRO ?? 0))
                .Select(x => new
                {
                    CODICE = x.c.VAR_COD_RUBRICA,
                    IDCORRGLOBALE = x.c.SYSTEM_ID,
                    DESCRIZIONE = x.c.VAR_DESC_CORR
                })
                .OrderBy(x => x.CODICE)
                .ThenBy(x => x.DESCRIZIONE)
                .ToListAsync();

            return results.Select(x => x.CODICE).ToList();

        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, ElementoRubrica>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_CORR))
                    .ForMember(dest => dest.interno, src => src.MapFrom(opt => opt.CHA_TIPO_IE == "I"))
                    .ForMember(dest => dest.tipo, src => src.MapFrom(opt => opt.CHA_TIPO_URP))
                    .ForMember(dest => dest.has_children, src => src.MapFrom(opt => false));

                cfg.CreateMap<CorrGlobaliEntity, UOSmistamento>()
                    .ForMember(dest => dest.ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_CORR));
            });

            _mapper = configuration.CreateMapper();
        }

        private async Task<Hashtable> GetRuoliUtenteSemplice(long idTenant)
        {
            var entities = await this._dbContext.CorrGlobaliEntities
                .Join(this._dbContext.PeopleEntities, u => u.ID_PEOPLE, p => p.SYSTEM_ID, (u, p) => new { u, p })
                .Join(this._dbContext.PeopleGroupEntities, j1 => j1.p.SYSTEM_ID, pg => pg.PEOPLE_SYSTEM_ID, (j1, pg) => new { u = j1.u, p = j1.p, pg })
                .Join(this._dbContext.CorrGlobaliEntities, j2 => j2.pg.GROUPS_SYSTEM_ID, r => r.ID_GRUPPO, (j2, r) => new { u = j2.u, p = j2.p, pg = j2.pg, r })
                .Where(x => x.u.CHA_TIPO_URP.Equals("P") && x.u.DTA_FINE == null && x.r.DTA_FINE == null && x.pg.DTA_FINE == null)
                .Select(x => new { COD_UTENTE = x.u.VAR_COD_RUBRICA, COD_RUOLO = x.r.VAR_COD_RUBRICA, DESC_RUOLO = x.r.VAR_DESC_CORR })
                .OrderBy(x => x.COD_UTENTE)
                .ToListAsync();

            Hashtable h = new Hashtable();
            ArrayList tmp = new ArrayList();

            string curUser = null;
            foreach (var entity in entities)
            {
                if (!entity.COD_UTENTE.Equals(curUser))
                {
                    if (!string.IsNullOrEmpty(curUser) && tmp.Count > 0)
                    {
                        string[] ruoli = new string[tmp.Count];
                        tmp.CopyTo(ruoli);
                        h[curUser] = ruoli;
                        tmp.Clear();
                    }
                    curUser = entity.COD_UTENTE;
                }
                tmp.Add(entity.COD_RUOLO);
            }
            if (curUser != null && tmp.Count > 0)
            {
                string[] ruoli = new string[tmp.Count];
                tmp.CopyTo(ruoli);
                h[curUser] = ruoli;
            }
            return h;
        }

        #endregion
    }

}
