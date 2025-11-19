// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.addressbook;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.rubrica;
using DocsPaVO.Smistamento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.rubricaGetGerarchiaElemento
{

    // Richiede libreria MediatR
    public class rubricaGetGerarchiaElementoHandler : IRequestHandler<Application.Requests.rubricaGetGerarchiaElemento, rubricaGetGerarchiaElementoResult>
    {
        #region Public Members

        public rubricaGetGerarchiaElementoHandler(ILogger<rubricaGetGerarchiaElementoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<rubricaGetGerarchiaElementoResult> Handle(Application.Requests.rubricaGetGerarchiaElemento request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.rubrica.ElementoRubrica> gerarchia = null;
            string codice = request.codice;
            DocsPaVO.addressbook.TipoUtente tipoIE = request.tipoIE;
            DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = request.smistamentoRubrica;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            try
            {
                gerarchia = await GetHierarchy(codice, tipoIE, smistamentoRubrica, idTenant);
                if (gerarchia != null && gerarchia.Count > 0)
                {
                    List<ElementoRubrica> listEl = gerarchia.Cast<ElementoRubrica>().ToList();
                    ElementoRubrica el = (from n in listEl where n.tipo.Equals("F") select n).FirstOrDefault();
                    if (el != null)
                    {
                        listEl.Remove(el);
                    }
                    gerarchia = listEl;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new rubricaGetGerarchiaElementoResult(gerarchia.ToArray());
        }

        private async Task<List<DocsPaVO.rubrica.ElementoRubrica>> GetHierarchy(string codice, TipoUtente tipoIE, SmistamentoRubrica smistamentoRubrica, long idTenant)
        {
            List<DocsPaVO.rubrica.ElementoRubrica> ers = await this.GetGerarchiaElemento(codice, tipoIE, idTenant, smistamentoRubrica);
            if (smistamentoRubrica != null && smistamentoRubrica.smistamento == "1")
            {
                //caso in cui è abilitato lo smistamento, devo quindi filtrare i corrispondenti
                //ma solo per determinati callType

                if ((smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT)
                       || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA)
                    || (smistamentoRubrica.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO))
                {
                    ers = await this.FiltraPrimoSmistamento(smistamentoRubrica.idRegistro, smistamentoRubrica.ruoloProt.systemId, smistamentoRubrica.infoUt, ers);
                }
            }

            this.SetVisibleCheckBoxElement(smistamentoRubrica, ref ers);

            return ers;
        }

        private void SetVisibleCheckBoxElement(SmistamentoRubrica sr, ref List<ElementoRubrica> ers)
        {
            if (sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_ALL ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_INF ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_SUP ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MODELLI_TRASM_PARILIVELLO ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_MODELLO_TRASM ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_PARILIVELLO ||
                //sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_ALL ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_SUP ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_TRASM_INF ||
                sr.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ORGANIGRAMMA_INTERNO ||
                sr.calltype == ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE
                )
            {
                foreach (ElementoRubrica er in ers)
                {
                    er.isVisibile = !er.disabledTrasm;
                }
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<rubricaGetGerarchiaElementoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected Hashtable h_uo;
        protected Hashtable h_utenti;
        protected List<String> uoInAoo;

        protected IMapper _mapper = null;

        private async Task<List<ElementoRubrica>> GetGerarchiaElemento(string codice, TipoUtente tipoIE, long idTenant, SmistamentoRubrica smistamentoRubrica)
        {
            List<ElementoRubrica> results = new List<ElementoRubrica>();

            //Controllo se il codice è di un utente
            var tipoURP = await this._dbContext.CorrGlobaliEntities
                .Where(x => !string.IsNullOrEmpty(x.VAR_COD_RUBRICA) && x.VAR_COD_RUBRICA.ToUpper().Equals(codice.ToUpper()) &&
                    !string.IsNullOrEmpty(x.CHA_TIPO_IE) && x.CHA_TIPO_IE.Equals((tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E") &&
                    x.ID_AMM == idTenant &&
                    x.DTA_FINE == null)
                .Select(x => x.CHA_TIPO_URP)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(tipoURP) && tipoURP.ToUpper().Equals("P"))
            {
                List<string> gerarchia = new List<string>();
                var j1 = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.PeopleEntities, a => a.ID_PEOPLE, b => b.SYSTEM_ID, (a, b) => new { a, b });
                var j2 = j1.Join(this._dbContext.PeopleGroupEntities, j1 => j1.b.SYSTEM_ID, c => c.PEOPLE_SYSTEM_ID, (j1, c) => new { a = j1.a, b = j1.b, c });
                var j3 = j2.Join(this._dbContext.GroupEntities, j2 => j2.c.GROUPS_SYSTEM_ID, d => d.SYSTEM_ID, (j2, d) => new { a = j2.a, b = j2.b, c = j2.c, d });

                var corrGlobaliEntity = await j3
                    .Where(x => !string.IsNullOrEmpty(x.a.CHA_TIPO_IE)&& x.a.CHA_TIPO_IE.Equals((tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E") &&
                        x.a.ID_AMM == idTenant &&
                        x.a.DTA_FINE == null &&
                        x.c.DTA_FINE == null &&
                        x.b.USER_ID.ToUpper().Equals(codice.ToUpper()))
                    .Select(x => new
                    {
                        ID_AMM = x.a.ID_AMM,
                        VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                        CHA_TIPO_IE = x.a.CHA_TIPO_IE,
                        GROUPS_SYSTEM_ID = x.d.SYSTEM_ID
                    }).ToListAsync();

                if (corrGlobaliEntity != null)
                {
                    foreach (var item in corrGlobaliEntity)

                    {
                        List<string> gerarchiaRuolo = await this.GetGerarchia(item.ID_AMM, item.VAR_COD_RUBRICA, item.CHA_TIPO_IE, item.GROUPS_SYSTEM_ID, new List<string>() { item.VAR_COD_RUBRICA });
                        gerarchia.AddRange(gerarchiaRuolo);

                    }

                }
                //    .FirstOrDefaultAsync();

                //if (corrGlobaliEntity != null)
                //    gerarchia = await this.GetGerarchia(corrGlobaliEntity.ID_AMM, corrGlobaliEntity.VAR_COD_RUBRICA, corrGlobaliEntity.CHA_TIPO_IE, corrGlobaliEntity.GROUPS_SYSTEM_ID, new List<string>() { corrGlobaliEntity.VAR_COD_RUBRICA });

                Dictionary<int, string> ordinamento = new Dictionary<int, string>();
                for (int i = 0; i < gerarchia.Count; i++)
                {
                    ordinamento.Add(i, gerarchia[i].ToUpper());
                }

                gerarchia = gerarchia.Select(x => x.ToUpper()).ToList();

                var erEntities = await this._dbContext.CorrGlobaliEntities
                .Where(x => !string.IsNullOrEmpty(x.VAR_COD_RUBRICA) && gerarchia.Contains(x.VAR_COD_RUBRICA.ToUpper()) &&
                    x.ID_AMM == idTenant &&
                    !string.IsNullOrEmpty(x.CHA_TIPO_IE) && x.CHA_TIPO_IE.Equals((tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E") &&
                    x.DTA_FINE == null &&
                    !string.IsNullOrEmpty(x.CHA_TIPO_URP) && !x.CHA_TIPO_URP.Equals("F"))
                .Select(x => new
                {
                    SYSTEM_ID = x.SYSTEM_ID,
                    VAR_COD_RUBRICA = x.VAR_COD_RUBRICA,
                    VAR_DESC_CORR = x.VAR_DESC_CORR,
                    INTERNO = !string.IsNullOrEmpty(x.CHA_TIPO_IE) && x.CHA_TIPO_IE.Equals("I") ? 1 : 0,
                    CHA_TIPO_URP = x.CHA_TIPO_URP,
                    DTA_FINE = x.DTA_FINE,
                    HAS_CHILDREN = string.Empty,
                    CHA_DISABLED_TRASM = x.CHA_DISABLED_TRASM,
                    NUM_LIVELLO = x.NUM_LIVELLO
                })
                .OrderBy(x => x.NUM_LIVELLO != null)
                .ToListAsync();

                List<ElementoRubrica> ers = new List<ElementoRubrica>();

                foreach (var erEntity in erEntities)
                {
                    ers.Add(new ElementoRubrica()
                    {
                        systemId = erEntity.SYSTEM_ID.ToString(),
                        codice = erEntity.VAR_COD_RUBRICA ?? string.Empty,
                        descrizione = erEntity.VAR_DESC_CORR ?? string.Empty,
                        interno = erEntity.INTERNO.Equals("1"),
                        tipo = erEntity.CHA_TIPO_URP ?? string.Empty,
                        disabled = erEntity.DTA_FINE != null,
                        has_children = await this.HasChildren(erEntity.SYSTEM_ID.ToString(), erEntity.CHA_TIPO_URP),
                        disabledTrasm = !string.IsNullOrEmpty(erEntity.CHA_DISABLED_TRASM) && erEntity.CHA_DISABLED_TRASM.Equals("1")
                    });
                }

                ers = ers
                    .OrderBy(x => x.has_children)
                    .ThenBy(x => x.tipo)
                    .ThenBy(x => x.descrizione)
                    .ToList();

                Dictionary<string, DocsPaVO.rubrica.ElementoRubrica> elementi = new Dictionary<string, DocsPaVO.rubrica.ElementoRubrica>();

                ers.ForEach(er =>
                {
                    if (!elementi.ContainsKey(er.codice.ToUpper()))
                    {
                        elementi.Add(er.codice.ToUpper(), er);
                    }
                });

                if (elementi != null && elementi.Count > 0)
                {
                    for (int y = 0; y < ordinamento.Count; y++)
                    {
                        string cod = ordinamento[y];
                        results.Add(elementi[cod]);
                    }
                }
            }
            else
            {
                List<string> gerarchia = await this.GetHierarchy(idTenant, codice.Replace("'", "''"), (tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E", string.Empty, new List<string>() { codice });
                foreach (string cod in gerarchia)
                    if (!string.IsNullOrEmpty(cod)) 
                        results.Add((await this._mediator.Send(new Application.Requests.rubricaGetElementoRubrica(cod, new InfoUtente(), smistamentoRubrica, string.Empty))).output);
            }

            return results;

        }

        private async Task<bool> HasChildren(string corrId, string tipoUrp)
        {
            bool rtnUO1;
            bool rtnUO2;
            bool rtnUO3;

            long corrIdAsLong = corrId.AsLong();
            var groupsSystemIdList = await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == corrIdAsLong).Select(x => x.ID_GRUPPO).ToListAsync();

            rtnUO1 = await this._dbContext.CorrGlobaliEntities.Where(x => tipoUrp.Equals("U") && !string.IsNullOrEmpty(x.CHA_TIPO_IE) && x.CHA_TIPO_IE.Equals("I") && x.ID_PARENT == corrIdAsLong).AnyAsync();
            rtnUO2 = await this._dbContext.CorrGlobaliEntities.Where(x => tipoUrp.Equals("U") && !string.IsNullOrEmpty(x.CHA_TIPO_IE) && x.CHA_TIPO_IE.Equals("I") && x.ID_UO == corrIdAsLong).AnyAsync();
            rtnUO3 = await this._dbContext.CorrGlobaliEntities.Where(x => tipoUrp.Equals("R") &&
                x.CHA_TIPO_IE.Equals("I") &&
                this._dbContext.PeopleGroupEntities.Where(p => groupsSystemIdList.Contains(p.GROUPS_SYSTEM_ID)).Any() &&
                x.DTA_FINE == null).AnyAsync();

            return rtnUO1 || rtnUO2 || rtnUO3;
        }

        private async Task<List<string>> GetGerarchia(long? idAmm, string cod, string tipoIE, long idRuolo, List<string> inputCodes)
        {
            string idParent = string.Empty, systemId = string.Empty, idUo = string.Empty, idUtente = string.Empty, cType = string.Empty, codice = cod;
            var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities
                .Where(x => !string.IsNullOrEmpty(x.VAR_COD_RUBRICA) && x.VAR_COD_RUBRICA.Equals(codice) && !string.IsNullOrEmpty(x.CHA_TIPO_IE) && x.CHA_TIPO_IE.Equals(tipoIE) && x.ID_AMM == idAmm && x.DTA_FINE == null)
                .Select(x => new
                {
                    ID_PARENT = x.ID_PARENT,
                    SYSTEM_ID = x.SYSTEM_ID,
                    ID_UO = x.ID_UO,
                    ID_PEOPLE = x.ID_PEOPLE,
                    CHA_TIPO_URP = x.CHA_TIPO_URP
                })
                .FirstOrDefaultAsync();

            if (corrGlobaliEntity != null)
            {
                idParent = corrGlobaliEntity.ID_PARENT.ToString() ?? string.Empty;
                systemId = corrGlobaliEntity.SYSTEM_ID.ToString() ?? string.Empty;
                idUo = corrGlobaliEntity.ID_UO.ToString() ?? string.Empty;
                idUtente = corrGlobaliEntity.ID_PEOPLE.ToString() ?? string.Empty;
                cType = corrGlobaliEntity.CHA_TIPO_URP?.ToString() ?? string.Empty;
            }

            return await GetGerarchiaRecursive(corrGlobaliEntity, inputCodes, idParent, systemId, codice, cType, idUo, idUtente, idAmm, idRuolo);
            
        }

        private async Task<List<string>> GetGerarchiaRecursive(object corrGlobaliEntity, List<string> codesList, string idParent, string systemId, string codice, string cType, string idUo, string idUtente, long? idAmm, long idRuolo)
        {
            switch (cType)
            {
                case "U":
                    if (string.IsNullOrEmpty(idParent) || idParent == "0")
                        return codesList;
                    long idParentAsLong = idParent.AsLong();
                    var uoCaseU = await this._dbContext.CorrGlobaliEntities
                        .Where(x => x.SYSTEM_ID == idParentAsLong && x.ID_AMM == idAmm && x.DTA_FINE == null && !string.IsNullOrEmpty(x.CHA_TIPO_URP) && x.CHA_TIPO_URP.Equals("U"))
                        .Select(x => new
                        {
                            VAR_COD_RUBRICA = x.VAR_COD_RUBRICA,
                            SYSTEM_ID = x.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();
                    if (uoCaseU != null)
                    {
                        codice = uoCaseU.VAR_COD_RUBRICA ?? string.Empty;
                        systemId = uoCaseU.SYSTEM_ID.ToString();
                    }
                    break;
                case "R":
                    if (string.IsNullOrEmpty(idUo) || idUo == "0")
                        return codesList;
                    long idUoAsLong = idUo.AsLong();
                    var uoCaseR = await this._dbContext.CorrGlobaliEntities
                        .Where(x => x.SYSTEM_ID == idUoAsLong && x.ID_AMM == idAmm && x.DTA_FINE == null && !string.IsNullOrEmpty(x.CHA_TIPO_URP) && x.CHA_TIPO_URP.Equals("U"))
                        .Select(x => new
                        {
                            VAR_COD_RUBRICA = x.VAR_COD_RUBRICA,
                            SYSTEM_ID = x.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();
                    if (uoCaseR != null)
                    {
                        codice = uoCaseR.VAR_COD_RUBRICA ?? string.Empty;
                        systemId = uoCaseR.SYSTEM_ID.ToString();
                    }
                    break;
                case "P":
                    codice = await this._dbContext.CorrGlobaliEntities
                        .Where(x => x.ID_GRUPPO == idRuolo && x.ID_AMM == idAmm && x.DTA_FINE == null)
                        .Select(x => x.VAR_COD_RUBRICA)
                        .FirstOrDefaultAsync() ?? string.Empty;
                    break;
                default:
                    codice = string.Empty;
                    break;
            }

            if (string.IsNullOrEmpty(codice))
                return codesList;

            var typesList = new List<string>() { "F", "P" };

            var cgEntity = await this._dbContext.CorrGlobaliEntities
                .Where(x => !string.IsNullOrEmpty(x.VAR_COD_RUBRICA) && x.VAR_COD_RUBRICA.Equals(codice) && x.ID_AMM == idAmm && x.DTA_FINE == null && !string.IsNullOrEmpty(x.CHA_TIPO_URP) && !typesList.Contains(x.CHA_TIPO_URP))
                .Select(x => new
                {
                    ID_PARENT = x.ID_PARENT,
                    SYSTEM_ID = x.SYSTEM_ID,
                    ID_UO = x.ID_UO,
                    CHA_TIPO_URP = x.CHA_TIPO_URP
                })
                .FirstOrDefaultAsync();

            if (cgEntity != null)
            {
                idParent = cgEntity.ID_PARENT.ToString() ?? string.Empty;
                systemId = cgEntity.SYSTEM_ID.ToString() ?? string.Empty;
                idUo = cgEntity.ID_UO.ToString() ?? string.Empty;
                cType = cgEntity.CHA_TIPO_URP?.ToString() ?? string.Empty;
            }

            codesList.Insert(0, codice.ToUpper());

            return await GetGerarchiaRecursive(cgEntity, codesList, idParent, systemId, codice, cType, idUo, idUtente, idAmm, idRuolo);
        }

        private async Task<List<string>> GetHierarchy(long? idAmm, string cod, string tipoIE, string idRuolo, List<string> inputCodes)
        {
            string idParent = string.Empty, systemId = string.Empty, idUo = string.Empty, idUtente = string.Empty, cType = string.Empty, codice = cod;
            var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities
                .Where(x => !string.IsNullOrEmpty(x.VAR_COD_RUBRICA) && x.VAR_COD_RUBRICA.Equals(codice) && !string.IsNullOrEmpty(x.CHA_TIPO_IE) && x.CHA_TIPO_IE.Equals("I") && x.CHA_TIPO_IE.Equals(tipoIE) && x.ID_AMM == idAmm && x.DTA_FINE == null)
                .Select(x => new
                {
                    ID_PARENT = x.ID_PARENT,
                    SYSTEM_ID = x.SYSTEM_ID,
                    ID_UO = x.ID_UO,
                    ID_PEOPLE = x.ID_PEOPLE,
                    CHA_TIPO_URP = x.CHA_TIPO_URP
                })
                .FirstOrDefaultAsync();

            if (corrGlobaliEntity != null)
            {
                idParent = corrGlobaliEntity.ID_PARENT.ToString() ?? string.Empty;
                systemId = corrGlobaliEntity.SYSTEM_ID.ToString() ?? string.Empty;
                idUo = corrGlobaliEntity.ID_UO.ToString() ?? string.Empty;
                idUtente = corrGlobaliEntity.ID_PEOPLE.ToString() ?? string.Empty;
                cType = corrGlobaliEntity.CHA_TIPO_URP?.ToString() ?? string.Empty;
            }

            return await GetHierarchyRecursive(corrGlobaliEntity, inputCodes, idParent, systemId, codice, cType, idUo, idUtente, idAmm, idRuolo);
        }

        private async Task<List<string>> GetHierarchyRecursive(object corrGlobaliEntity, List<string> codesList, string idParent, string systemId, string codice, string cType, string idUo, string idUtente, long? idAmm, string idRuolo)
        {

            switch (cType)
            {
                case "U":
                    if (string.IsNullOrEmpty(idParent) || idParent == "0")
                        return codesList;
                    long idParentAsLong = idParent.AsLong();
                    var uoCaseU = await this._dbContext.CorrGlobaliEntities
                        .Where(x => x.SYSTEM_ID == idParentAsLong && x.ID_AMM == idAmm && x.DTA_FINE == null)
                        .Select(x => new
                        {
                            VAR_COD_RUBRICA = x.VAR_COD_RUBRICA,
                            SYSTEM_ID = x.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();
                    if (uoCaseU != null)
                    {
                        codice = uoCaseU.VAR_COD_RUBRICA ?? string.Empty;
                        systemId = uoCaseU.SYSTEM_ID.ToString();
                    }
                    break;
                case "R":
                    if (string.IsNullOrEmpty(idUo) || idUo == "0")
                        return codesList;
                    long idUoAsLong = idUo.AsLong();
                    var uoCaseR = await this._dbContext.CorrGlobaliEntities
                        .Where(x => x.SYSTEM_ID == idUoAsLong && x.ID_AMM == idAmm && x.DTA_FINE == null)
                        .Select(x => new
                        {
                            VAR_COD_RUBRICA = x.VAR_COD_RUBRICA,
                            SYSTEM_ID = x.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();
                    if (uoCaseR != null)
                    {
                        codice = uoCaseR.VAR_COD_RUBRICA ?? string.Empty;
                        systemId = uoCaseR.SYSTEM_ID.ToString();
                    }
                    break;
                case "P":
                    codice = await this._dbContext.CorrGlobaliEntities
                        .Where(x => x.ID_GRUPPO == idRuolo.AsLong() && x.ID_AMM == idAmm && x.DTA_FINE == null)
                        .Select(x => x.VAR_COD_RUBRICA)
                        .FirstOrDefaultAsync() ?? string.Empty;
                    break;
                default:
                    codice = string.Empty;
                    break;
            }

            if (string.IsNullOrEmpty(codice))
                return codesList;

            var typesList = new List<string>() { "F", "P" };

            var cgEntity = await this._dbContext.CorrGlobaliEntities
                .Where(x => !string.IsNullOrEmpty(x.VAR_COD_RUBRICA) && x.VAR_COD_RUBRICA.Equals(codice) && x.ID_AMM == idAmm && x.DTA_FINE == null && !string.IsNullOrEmpty(x.CHA_TIPO_IE) && x.CHA_TIPO_IE.Equals("I"))
                .Select(x => new
                {
                    ID_PARENT = x.ID_PARENT,
                    SYSTEM_ID = x.SYSTEM_ID,
                    ID_UO = x.ID_UO,
                    CHA_TIPO_URP = x.CHA_TIPO_URP
                })
                .FirstOrDefaultAsync();

            if (cgEntity != null)
            {
                idParent = cgEntity.ID_PARENT.ToString() ?? string.Empty;
                systemId = cgEntity.SYSTEM_ID.ToString() ?? string.Empty;
                idUo = cgEntity.ID_UO.ToString() ?? string.Empty;
                cType = cgEntity.CHA_TIPO_URP?.ToString() ?? string.Empty;
            }

            codesList.Insert(0, codice.ToUpper());

            return await GetHierarchyRecursive(cgEntity, codesList, idParent, systemId, codice, cType, idUo, idUtente, idAmm, idRuolo);
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

        #endregion
    }

}
