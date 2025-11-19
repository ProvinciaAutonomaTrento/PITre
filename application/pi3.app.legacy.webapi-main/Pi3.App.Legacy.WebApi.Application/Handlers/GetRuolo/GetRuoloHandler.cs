// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GetRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRuolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRuolo
{
    public class GetRuoloHandler : IRequestHandler<GetRuoloRequest, GetRuoloResult>
    {

        public GetRuoloHandler(
            IConfigurationService configurationService,
            IPi3DbContext dbContext,
            ILogger<GetRuoloHandler> logger
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }


        public async Task<GetRuoloResult> Handle(GetRuoloRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Ruolo output = new DocsPaVO.utente.Ruolo();
            try
            {

                output = await GetRuolo(request.idCorrGlobali, true);


            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new GetRuoloResult(output);
        }





        protected readonly IConfigurationService _configurationService;
        private readonly IPi3DbContext _dbContext;
        private readonly ILogger<GetRuoloHandler> _logger;

        private IEnumerable<T> SelectRecursive<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var parent in source)
            {
                yield return parent;

                var children = selector(parent);
                foreach (var child in SelectRecursive(children, selector))
                    yield return child;
            }
        }



        private async Task<List<CorrGlobaliEntity>> GetRuoliUtente(long? idUo)
        {

            var uoEnt = await this._dbContext.CorrGlobaliEntities.AsNoTracking().FirstOrDefaultAsync(a => a.SYSTEM_ID == idUo);

            var baseQuery = await (this._dbContext.CorrGlobaliEntities.AsNoTracking().
                Where(a => a.CHA_TIPO_IE == "I" &&
                      a.CHA_TIPO_URP == "U" &&
                     !a.DTA_FINE.HasValue && a.SYSTEM_ID != idUo))
                    .ToListAsync();

            var bQ = baseQuery.Prepend(uoEnt);
            var lookup = bQ.ToLookup(x => x.SYSTEM_ID);
            return SelectRecursive(lookup[idUo ?? -1], x => lookup[x.ID_PARENT ?? -1]).ToList();


        }

        private async Task<DocsPaVO.utente.Ruolo> GetRuolo(string idRuolo, bool loadFunzioni)
        {
            this._logger.LogInformation("GetRuolo");
            DocsPaVO.utente.Ruolo objRuolo = null;

            var originalId = await _dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == idRuolo.AsLong()).Select(c => c.ORIGINAL_ID).FirstOrDefaultAsync();

            List<RoleInfoEntity> roles = await _dbContext.PeopleGroupEntities
                .Join(_dbContext.CorrGlobaliEntities,
                    a => a.GROUPS_SYSTEM_ID,
                    b => b.ID_GRUPPO,
                    (a, b) => new {a, b})
                .Join(_dbContext.TipoRuoloEntities,
                    j => j.b.ID_TIPO_RUOLO,
                    c => c.SYSTEM_ID,
                    (j, c) => new { j.a, j.b, c })
                .Join(_dbContext.CorrGlobaliEntities,
                    j => j.b.ID_UO,
                    d => d.SYSTEM_ID,
                    (j, d) => new {j.a, j.b, j.c, d})
                .Where(j => !j.a.DTA_FINE.HasValue &&
                    j.b.SYSTEM_ID ==originalId)
                .Select(j => new RoleInfoEntity(
                        j.a.PEOPLE_SYSTEM_ID,
                        j.b.SYSTEM_ID,
                        j.b.ID_GRUPPO,
                        j.c.NUM_LIVELLO,
                        j.b.ID_REGISTRO,
                        j.c.VAR_CODICE,
                        j.c.VAR_DESC_RUOLO,
                        j.d.NUM_LIVELLO,
                        j.b.ID_UO,
                        j.b.VAR_COD_RUBRICA,
                        j.b.ID_AMM,
                        j.b.VAR_DESC_CORR,
                        j.a.CHA_PREFERITO,
                        j.b.CHA_RIFERIMENTO,
                        j.b.CHA_RESPONSABILE,
                        j.b.CHA_SEGRETARIO
                 )).AsNoTracking()
                .ToListAsync();
            /*
           List <RoleInfoEntity> roles = await (from a in this._dbContext.PeopleGroupEntities.AsNoTracking()
                               join b in this._dbContext.CorrGlobaliEntities.AsNoTracking() on a.GROUPS_SYSTEM_ID equals b.ID_GRUPPO
                               join c in this._dbContext.TipoRuoloEntities.AsNoTracking() on b.ID_TIPO_RUOLO equals c.SYSTEM_ID
                               join d in this._dbContext.CorrGlobaliEntities.AsNoTracking() on b.ID_UO equals d.SYSTEM_ID
                               let orId = this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(corr => corr.SYSTEM_ID == idRuolo.AsLong()).Select(corr => corr.ORIGINAL_ID).ToList()
                               where !a.DTA_FINE.HasValue
                               && orId.Contains(b.SYSTEM_ID)
                               select new RoleInfoEntity(
                                   a.PEOPLE_SYSTEM_ID,
                                   b.SYSTEM_ID,
                                   b.ID_GRUPPO,
                                   c.NUM_LIVELLO,
                                   b.ID_REGISTRO,
                                   c.VAR_CODICE,
                                   c.VAR_DESC_RUOLO,
                                   d.NUM_LIVELLO,
                                   b.ID_UO,
                                   b.VAR_COD_RUBRICA,
                                   b.ID_AMM,
                                   b.VAR_DESC_CORR,
                                   a.CHA_PREFERITO,
                                   b.CHA_RIFERIMENTO,
                                   b.CHA_RESPONSABILE,
                                   b.CHA_SEGRETARIO
                               )).ToListAsync();
            */
            if (roles.Count > 0)
            {

                var tempQueryLivello = (from r in roles
                                        orderby r.NUM_LIVELLO descending
                                        select r)?.FirstOrDefault();

                if (tempQueryLivello != null)
                {

                    string? maxLivello = tempQueryLivello.NUM_LIVELLO != null ? tempQueryLivello.NUM_LIVELLO.ToString() : string.Empty;
                    this._logger.LogDebug($"GetRuolo: maxlivello={maxLivello}");

                    string value = await this._configurationService.GetValue<string>("0", "USA_CONNECTBYPRIOR_OR_WITH");
                    List<CorrGlobaliEntity>? uoEntSet = null;

                    if (value.Equals("1"))
                    {
                        uoEntSet = await GetRuoliUtente(tempQueryLivello.ID_UO);
                    }
                    else
                    {
                        uoEntSet = await GetRuoloMaxLivello(maxLivello, true);
                    }
                    objRuolo = await GetRuoloData(uoEntSet, tempQueryLivello, true);
                }



            }
            return objRuolo;
        }

        private async Task<DocsPaVO.utente.Ruolo> GetRuoloData(List<CorrGlobaliEntity> dataSet, RoleInfoEntity ruoloRow, bool loadFunzioni)
        {
            string id = ruoloRow.SYSTEM_ID.ToString();
            //.Where(corr => ruoloRow.ID_UO != null ? corr.SYSTEM_ID == ruoloRow.ID_UO : false).OrderBy(c => c.SYSTEM_ID)
            List<CorrGlobaliEntity> dataRow =  dataSet.Where(corr => ruoloRow.ID_UO != null ? corr.SYSTEM_ID == ruoloRow.ID_UO : false).OrderBy(c => c.SYSTEM_ID).ToList();
            DocsPaVO.utente.UnitaOrganizzativa uo = null;

            if (dataRow.Count > 0)
            {
                uo = await GetUnitaOrganizzativa(dataSet, dataRow[0]);
            }

            DocsPaVO.utente.TipoRuolo tipoRuo = new DocsPaVO.utente.TipoRuolo();

            if (ruoloRow.VAR_CODICE != null)
                tipoRuo.codice = ruoloRow.VAR_CODICE;

            tipoRuo.descrizione = ruoloRow.VAR_DESC_RUOLO;
            DocsPaVO.utente.Funzione[] funzioni = null;
            if (loadFunzioni)
                funzioni = await GetFunzioni(id);

            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo(
                ruoloRow.SYSTEM_ID.ToString(),
                //ruoloRow["VAR_DESC_RUOLO"].ToString()+" "+uo.descrizione,
                ruoloRow.VAR_DESC_CORR ?? string.Empty,
                ruoloRow.VAR_CODICE ?? string.Empty,
                ruoloRow.NUM_LIVELLO != null ? ruoloRow.NUM_LIVELLO.ToString() : string.Empty,
                ruoloRow.ID_GRUPPO != null ? ruoloRow.ID_GRUPPO.ToString() : string.Empty,
                tipoRuo,
                funzioni);

            ruolo.uo = uo;
            ruolo.codiceRubrica = ruoloRow.VAR_COD_RUBRICA ?? string.Empty;
            ruolo.idRegistro = ruoloRow.ID_REGISTRO!= null ? ruoloRow.ID_REGISTRO.ToString() : string.Empty;
            ruolo.registri = await GetRegistriRuolo(id);
            ruolo.idAmministrazione = ruoloRow.ID_AMM != null ? ruoloRow.ID_AMM.ToString() : string.Empty;
            ruolo.tipoCorrispondente = "R";

            if(ruoloRow.CHA_RESPONSABILE != null)
            {
                ruolo.Responsabile = ruoloRow.CHA_RESPONSABILE.ToString().Equals("1");
            }

            if (ruoloRow.CHA_SEGRETARIO != null)
            {
                ruolo.Segretario = ruoloRow.CHA_SEGRETARIO.ToString().Equals("1");
            }
            if (ruoloRow.CHA_PREFERITO != null)
            {
                ruolo.selezionato = ruoloRow.CHA_PREFERITO.ToString().Equals("1");
            }

            return ruolo;

        }



        private async Task<DocsPaVO.utente.Registro[]> GetRegistriRuolo(string idRuolo)
        {
            ArrayList registri = new ArrayList();

            Func<DateTime?,string> toChar = (date) => date.HasValue ? ((DateTime)date).ToString("dd/MM/yyyy") : string.Empty;

            var rolesInfo = await (from b in this._dbContext.RuoloRegistroEntities.AsNoTracking()
             join a in this._dbContext.RegistroEntities.AsNoTracking() on b.ID_REGISTRO equals a.SYSTEM_ID
             where a.CHA_RF.Equals("0") && b.ID_RUOLO_IN_UO == idRuolo.AsLong()
             orderby b.CHA_PREFERITO != null descending, b.CHA_PREFERITO, a.VAR_PREG == null descending, a.VAR_PREG ascending, a.VAR_CODICE, a.VAR_DESC_REGISTRO ascending
             select new
             {
                 a.SYSTEM_ID,
                 a.VAR_CODICE,
                 a.NUM_RIF,
                 a.VAR_DESC_REGISTRO,
                 a.VAR_EMAIL_REGISTRO,
                 a.CHA_STATO,
                 a.ID_AMM,
                 a.DTA_OPEN,
                 a.DTA_CLOSE,
                 a.DTA_ULTIMO_PROTO,
                 a.ID_RUOLO_AOO,
                 a.ID_RUOLO_RESP,
                 a.ID_PEOPLE_AOO,
                 a.CHA_AUTO_INTEROP,
                 a.VAR_PREG,
                 a.ANNO_PREG,
                 a.VAR_CODICE_IPA,
                 a.DIRITTO_RUOLO_AOO,
                 a.INVIO_RICEVUTA_MANUALE,
                 a.FLAG_WSPIA
             }).ToListAsync();
            
            if(rolesInfo.Count > 0){

                foreach(var role in rolesInfo)
                {
                    DocsPaVO.utente.Registro reg = new DocsPaVO.utente.Registro();
                    reg.systemId = role.SYSTEM_ID.ToString();
                    reg.codRegistro = role.VAR_CODICE;
                    reg.codice = role.NUM_RIF != null ? role.NUM_RIF.ToString() : string.Empty;
                    reg.descrizione = role.VAR_DESC_REGISTRO;
                    reg.email = role.VAR_EMAIL_REGISTRO;
                    reg.stato = role.CHA_STATO;
                    reg.idAmministrazione = role.ID_AMM != null ? role.ID_AMM.ToString() : string.Empty;
                    reg.codAmministrazione = await GetCodeAmm(role.ID_AMM);
                    reg.dataApertura = role.DTA_OPEN.HasValue? role.DTA_OPEN.AsDateFormat() : string.Empty;
                    reg.dataChiusura = role.DTA_CLOSE.HasValue ? role.DTA_CLOSE.AsDateFormat() : string.Empty;
                    reg.dataUltimoProtocollo = role.DTA_ULTIMO_PROTO.HasValue ? role.DTA_ULTIMO_PROTO.AsDateFormat() : string.Empty;
                    reg.idRuoloAOO = role.ID_RUOLO_AOO != null ? role.ID_RUOLO_AOO.ToString() : string.Empty;
                    reg.idRuoloResp = role.ID_RUOLO_RESP != null ? role.ID_RUOLO_RESP.ToString() : string.Empty;
                    reg.idUtenteAOO = role.ID_PEOPLE_AOO != null ? role.ID_PEOPLE_AOO.ToString() : string.Empty;
                    reg.autoInterop = role.CHA_AUTO_INTEROP;
                    reg.Diritto_Ruolo_AOO = role.DIRITTO_RUOLO_AOO != null ? role.DIRITTO_RUOLO_AOO.ToString() : string.Empty;

                    if(role.INVIO_RICEVUTA_MANUALE != null)
                    {
                        reg.invioRicevutaManuale = (role.INVIO_RICEVUTA_MANUALE!.ToString()!.Equals("0")) ? "0" : "1";
                    }
                    else
                    {
                        reg.invioRicevutaManuale = "1";
                    }
                    if (string.IsNullOrEmpty(role.FLAG_WSPIA))
                        reg.FlagWspia = "0";
                    else reg.FlagWspia = role.FLAG_WSPIA;
                    reg.codiceIpa = role.VAR_CODICE_IPA;
                    registri.Add(reg);
                }
            }

            foreach(DocsPaVO.utente.Registro reg in registri)
            {
                if (reg.stato.Equals("C"))
                {
                    if (reg.dataUltimoProtocollo != null && reg.dataUltimoProtocollo != "" && reg.dataUltimoProtocollo.Substring(6, 4).Equals(DateTime.Now.ToString("dd/MM/yyyy").Substring(6, 4)))
                    {
                        reg.ultimoNumeroProtocollo = await GetUltimoNumProto(reg.systemId);

                    }
                    else
                    {
                        reg.ultimoNumeroProtocollo = "1";
                    }

                }
            }


            return (DocsPaVO.utente.Registro[])registri.ToArray(typeof(DocsPaVO.utente.Registro));
        }


        private async Task<string?> GetCodeAmm(long? idAmm)
        {
            return await this._dbContext.AmministraEntities.AsNoTracking().Where(amm => amm.SYSTEM_ID == idAmm).Select(a => a.VAR_CODICE_AMM).FirstAsync();
        }
        private async Task<string> GetUltimoNumProto(string idRegistro)
        {
            return (await this._dbContext.RegProtoEntities.AsNoTracking().Where(reg => reg.ID_REGISTRO == idRegistro.AsLong()).Select(reg => reg.NUM_RIF).FirstAsync()).ToString();
        }

            private static bool FromCharToBool(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Equals("1"))
                return true;
            else
                return false;
        }


        private async Task<DocsPaVO.utente.UnitaOrganizzativa> GetUnitaOrganizzativa(List<CorrGlobaliEntity> dataSet, CorrGlobaliEntity uoRow)
        {
            //ricerca delle UO

            DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
            uo.systemId = uoRow.SYSTEM_ID.ToString();
            uo.descrizione = uoRow.VAR_DESC_CORR != null ? uoRow.VAR_DESC_CORR.ToString() : string.Empty;
            uo.codice = uoRow.VAR_CODICE != null ? uoRow.VAR_CODICE.ToString() : string.Empty;
            uo.idAmministrazione = uoRow.ID_AMM != null ? uoRow.ID_AMM.ToString() : string.Empty;
            uo.livello = uoRow.NUM_LIVELLO != null ? uoRow.NUM_LIVELLO.ToString() : string.Empty;
            uo.codiceRubrica = uoRow.VAR_COD_RUBRICA != null ? uoRow.VAR_COD_RUBRICA.ToString() : string.Empty;
            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
            sp.serverSMTP = uoRow.VAR_SMTP != null ? uoRow.VAR_SMTP.ToString() : string.Empty;
            sp.portaSMTP = uoRow.NUM_PORTA_SMTP != null ? uoRow.NUM_PORTA_SMTP.ToString() : string.Empty;
            uo.serverPosta = sp;
            uo.idRegistro = uoRow.ID_REGISTRO != null ? uoRow.ID_REGISTRO.ToString() : string.Empty;
            uo.interoperante = uoRow.CHA_PA != null ? FromCharToBool(uoRow.CHA_PA.ToString()) : false;
            uo.codiceAOO = uoRow.VAR_CODICE_AOO != null ? uoRow.VAR_CODICE_AOO.ToString() : string.Empty;
            uo.codiceAmm = uoRow.VAR_CODICE_AMM != null ? uoRow.VAR_CODICE_AMM.ToString() : string.Empty;
            uo.email = uoRow.VAR_EMAIL != null ? uoRow.VAR_EMAIL.ToString() : string.Empty;
            uo.tipoIE = uoRow.CHA_TIPO_IE != null ? uoRow.CHA_TIPO_IE.ToString() : string.Empty;
            uo.tipoCorrispondente = uoRow.CHA_TIPO_URP != null ? uoRow.CHA_TIPO_URP.ToString() : string.Empty;
            uo.classificaUO = uoRow.CLASSIFICA_UO != null ? uoRow.CLASSIFICA_UO.ToString() : string.Empty;

            //si ricava la parentela
            if (!uoRow.ID_PARENT.ToString().Equals(""))
            {
                if (!uoRow.ID_PARENT.ToString().Equals("0"))
                {
                    uo.parent = await GetParents(uoRow.ID_PARENT.ToString(), dataSet);
                }
            }
            return uo;
        }

        private async Task<DocsPaVO.utente.Funzione[]> GetFunzioni(string idRuolo)
        {
            var tempFunc = await (from a in this._dbContext.FunzioneEntities.AsNoTracking()
                                  join b in this._dbContext.TipoFunzioneEntities.AsNoTracking() on a.ID_TIPO_FUNZIONE equals b.SYSTEM_ID
                                  join c in this._dbContext.TipoFRuoloEntities.AsNoTracking() on b.SYSTEM_ID equals c.ID_TIPO_FUNZ
                                  where c.ID_RUOLO_IN_UO == idRuolo.AsLong()
                                  select new DocsPaVO.utente.Funzione(

                                      a.SYSTEM_ID.ToString(),
                                      a.VAR_DESC_FUNZIONE ?? string.Empty,
                                      a.COD_FUNZIONE ?? string.Empty,
                                      a.ID_TIPO_FUNZIONE != null ? a.ID_TIPO_FUNZIONE.ToString() : string.Empty,
                                      b.VAR_COD_TIPO ?? string.Empty,
                                      b.VAR_DESC_TIPO_FUN ?? string.Empty
                                  )).ToArrayAsync();

            return tempFunc;

        }

        private async Task<DocsPaVO.utente.UnitaOrganizzativa> GetParents(string idParent, List<CorrGlobaliEntity> dt)
        {
            List<CorrGlobaliEntity> parentRowList =  dt.Where(row => row.SYSTEM_ID == idParent.AsLong()).ToList();
            CorrGlobaliEntity? parentRow = null;
            DocsPaVO.utente.UnitaOrganizzativa parent = new DocsPaVO.utente.UnitaOrganizzativa();
            if (parentRowList.Count > 0)
            {
                parentRow = parentRowList[0];
                parent.systemId = parentRow.SYSTEM_ID.ToString();
                parent.descrizione = parentRow.VAR_DESC_CORR != null ? parentRow.VAR_DESC_CORR.ToString() : string.Empty;
                parent.codiceCorrispondente = parentRow.VAR_CODICE != null ? parentRow.VAR_CODICE.ToString() : string.Empty;
                parent.codiceRubrica = parentRow.VAR_COD_RUBRICA != null ? parentRow.VAR_COD_RUBRICA.ToString() : string.Empty;
                parent.livello = parentRow.NUM_LIVELLO != null ? parentRow.NUM_LIVELLO.ToString() : string.Empty;
                parent.codiceAOO = parentRow.VAR_CODICE_AOO != null ? parentRow.VAR_CODICE_AOO.ToString() : string.Empty;
                parent.codiceAmm = parentRow.VAR_CODICE_AMM != null ? parentRow.VAR_CODICE_AMM.ToString() : string.Empty;
                parent.codiceIstat = parentRow.VAR_CODICE_ISTAT != null ? parentRow.VAR_CODICE_ISTAT.ToString() : string.Empty;
                parent.idAmministrazione = parentRow.ID_AMM != null ? parentRow.ID_AMM.ToString() : string.Empty;
                parent.dettagli = parentRow.CHA_DETTAGLI != null? FromCharToBool(parentRow.CHA_DETTAGLI.ToString()) : false;
                DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
                sp.serverSMTP = parentRow.VAR_SMTP != null ? parentRow.VAR_SMTP.ToString() : string.Empty;
                sp.portaSMTP = parentRow.NUM_PORTA_SMTP != null ? parentRow.NUM_PORTA_SMTP.ToString() : string.Empty;
                parent.serverPosta = sp;
                if (parentRow.ID_REGISTRO != null)
                {
                    parent.idRegistro = parentRow.ID_REGISTRO != null ? parentRow.ID_REGISTRO.ToString() : string.Empty;
                }
                parent.email = parentRow.VAR_EMAIL != null ? parentRow.VAR_EMAIL.ToString() : string.Empty;
                parent.interoperante = FromCharToBool(parentRow.CHA_PA != null ? parentRow.CHA_PA.ToString() : string.Empty);

                parent.classificaUO = parentRow.CLASSIFICA_UO != null ? parentRow.CLASSIFICA_UO.ToString() : string.Empty;


                if (!String.IsNullOrEmpty(parentRow.ID_PARENT.ToString()) && !parentRow.ID_PARENT.ToString().Equals("0"))
                {
                    parent.parent = await GetParents(parentRow.ID_PARENT != null ? parentRow.ID_PARENT.ToString() : string.Empty, dt);
                }
            }
            else
            {
                parent = null;

            }

            return parent;

        }

        private Expression<Func<T, bool>> BuildAndPredicate<T>(IEnumerable<Expression<Func<T, bool>>> conditions)
        {
            Expression<Func<T, bool>> predicate = null;

            foreach (var condition in conditions)
            {
                if (predicate == null)
                {
                    predicate = condition;
                }
                else
                {
                    var invokedExpr = Expression.Invoke(condition, predicate.Parameters.Cast<Expression>());
                    predicate = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, invokedExpr), predicate.Parameters);
                }
            }

            return predicate ?? (t => false);
        }


        private async Task<List<CorrGlobaliEntity>> GetRuoloMaxLivello(string? maxLivello, bool getRuoliAbilitati)
        {
            List<Expression<Func<CorrGlobaliEntity, bool>>> conditions = new List<Expression<Func<CorrGlobaliEntity, bool>>>()
            {
                corr =>(corr.CHA_TIPO_URP != null ? corr.CHA_TIPO_URP.Equals("U") : false),
                corr => corr.CHA_TIPO_IE != null ? corr.CHA_TIPO_IE.Equals("I") : false,
                corr => corr.NUM_LIVELLO <= maxLivello.AsLong()
            };

            if (!getRuoliAbilitati)
            {
                conditions.Add((corr) =>!corr.DTA_FINE.HasValue);
            }
            Expression<Func<CorrGlobaliEntity, bool>> predicate = BuildAndPredicate(conditions);

            return await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(predicate).ToListAsync();

        }




        protected class RoleInfoEntity
        {
            public long? PEOPLE_SYSTEM_ID { get; set; }
            public long SYSTEM_ID { get; set; }
            public long? ID_GRUPPO { get; set; }
            public long? NUM_LIVELLO { get; set; }
            public long? ID_REGISTRO { get; set; }
            public string? VAR_CODICE { get; set; }
            public string VAR_DESC_RUOLO { get; set; }
            public long? NUM_LIVELLO_UO { get; set; }
            public long? ID_UO { get; set; }
            public string? VAR_COD_RUBRICA { get; set; }
            public long? ID_AMM { get; set; }
            public string? VAR_DESC_CORR { get; set; }
            public string? CHA_PREFERITO { get; set; }
            public string? CHA_RIFERIMENTO { get; set; }
            public string? CHA_RESPONSABILE { get; set; }
            public string? CHA_SEGRETARIO { get; set; }


            public RoleInfoEntity(
                 long? peopleSystemId,
                 long systemId,
                 long? idGruppo,
                 long? numLivello,
                 long? idRegistro,
                 string? varCodice,
                 string varDescRuolo,
                 long? numLivelloUo,
                 long? idUo,
                 string? varCodRubrica,
                 long? idAmm,
                 string? varDescCorr,
                 string? chaPreferito,
                 string? chaRiferimento,
                 string? chaResponsanbile,
                 string? chaSegretario
                )
            {
                this.PEOPLE_SYSTEM_ID = peopleSystemId;
                this.SYSTEM_ID = systemId;
                this.ID_GRUPPO = idGruppo;
                this.NUM_LIVELLO = numLivello;
                this.ID_REGISTRO = idRegistro;
                this.VAR_CODICE = varCodice;
                this.VAR_DESC_RUOLO = varDescRuolo;
                this.NUM_LIVELLO_UO = numLivelloUo;
                this.ID_UO = idUo;
                this.VAR_COD_RUBRICA = varCodRubrica;
                this.ID_AMM = idAmm;
                this.VAR_DESC_CORR = varDescCorr;
                this.CHA_PREFERITO = chaPreferito;
                this.CHA_RIFERIMENTO = chaRiferimento;
                this.CHA_RESPONSABILE = chaResponsanbile;
                this.CHA_SEGRETARIO = chaSegretario;

            }
        }
    }
}
