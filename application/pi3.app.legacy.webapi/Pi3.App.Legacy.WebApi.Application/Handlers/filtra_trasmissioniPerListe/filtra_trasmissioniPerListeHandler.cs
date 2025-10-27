// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.rubrica;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using filtra_trasmissioniPerListeRequest = Pi3.App.Legacy.WebApi.Application.Requests.filtra_trasmissioniPerListe;
using DocsPaVO.trasmissione;
using DocumentFormat.OpenXml.Spreadsheet;
using DocsPaVO.utente;
using LinqKit;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using DocsPaVO.ProfilazioneDinamicaLite;
using System.Collections;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.filtra_trasmissioniPerListe
{
    public class filtra_trasmissioniPerListeHandler : IRequestHandler<filtra_trasmissioniPerListeRequest, filtra_trasmissioniPerListeResult>
    {
        #region Public Members

        public filtra_trasmissioniPerListeHandler(IConfigurationService configurationService,ILogger<filtra_trasmissioniPerListeHandler> logger, IPi3DbContext _dbContext, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = _dbContext;
            this._configurationService = configurationService;
        }

        public async Task<filtra_trasmissioniPerListeResult> Handle(filtra_trasmissioniPerListeRequest request, CancellationToken cancellationToken)
        {
            List<ElementoRubrica> output = new();
            try
            {
                output = await this.FiltraTrasmissioni(request.qc,request.ers,request.u);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            return new(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<filtra_trasmissioniPerListeHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        private async Task<List<ElementoRubrica>> FiltraTrasmissioni(DocsPaVO.rubrica.ParametriRicercaRubrica qr, ElementoRubrica[] ers, DocsPaVO.utente.InfoUtente user)
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

        private bool RuoloIsAutorizzato(string codice, string[] ruoliAutorizzati)
        {
            return ruoliAutorizzati.ToList().Contains(codice) == true;
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


        private class UoResult
        {
            public long SystemId { get; set; }
            public long? IdParent { get; set; }
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
                                 from c in this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                 select new CorrRuoInf()
                                 {
                                     SystemId = a.SYSTEM_ID,
                                     IdGruppo = a.ID_GRUPPO,
                                     VarDescRuolo = b.VAR_DESC_RUOLO,
                                     VarCodice = a.VAR_CODICE,
                                     VarCodiceRubrica = a.VAR_COD_RUBRICA,
                                     IdParent = a.ID_PARENT,
                                     IdUo = a.ID_UO
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
                                     IdUo = a.ID_UO
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
                                     IdUo = a.ID_UO
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

            return roles;
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
        #endregion
    }
}