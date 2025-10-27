// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using DocsPaVO.InstanceAccess.Metadata;
using DocsPaVO.Smistamento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using GetUOInferioriRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetUOInferiori;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetUOInferiori
{
    public class GetUOInferioriHandler : IRequestHandler<GetUOInferioriRequest, GetUOInferioriResult>
    {
        protected readonly ILogger<GetUOInferioriHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        private class InfoUo
        {
            public long ID { get; set; }
            public string? CODICE_UO { get; set; }
            public string? DESCRIZIONE_UO { get; set; }
            public long? ID_PESO_ORG { get; set; }
        }

        private class InfoUoWithRole
        {
            public long ID { get; set; }
            public long? ID_UO { get; set; }
            public long? ID_GRUPPO { get; set; }
            public string? CODICE_RUOLO { get; set; }
            public string? DESCRIZIONE_RUOLO { get; set; }
            public string? RUOLO_RIFERIMENTO { get; set; }
            public long? GERARCHIA { get; set; }
        }


        private class InfoUtInUo
        {
            public long ID { get; set; }
            public long? ID_CORR_GLOBALI { get; set; }
            public long? ID_GRUPPO { get; set; }
            public string? CODICE_UTENTE { get; set; }
            public string? DESCRIZIONE_UTENTE { get; set; }
            public string? EMAIL_UTENTE { get; set; }
            public string? CHA_NOTIFICA { get; set; }
            public string? CHA_NOTIFICA_CON_ALLEGATO { get; set; }
            public string? VAR_COGNOME { get; set; }
        }

        private class RoleAssUtInfo
        {
            public InfoUoWithRole UoRole { get; set; }
            public IEnumerable<InfoUtInUo> UtUo { get; set; }
        }

        private class SmistaInfo
        {
            public List<InfoUo> UoEntities { get; set; }
            public List<InfoUoWithRole> RuoloEntities { get; set; }
            public List<InfoUtInUo> UtentiUoAppEntities { get; set; }

        }

        private async Task<string[]> GetRegistriRuolo(string idRuolo)
        {
            List<string> result = new();
            try
            {
                result = await this._dbContext.RuoloRegistroEntities.AsNoTracking()
                    .Where(reg => reg.ID_RUOLO_IN_UO == idRuolo.AsLong() && !reg.DTA_FINE.HasValue)
                    .Select(reg => reg.ID_REGISTRO.ToString())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result.ToArray();
        }
        private DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum GetTipoNotificaSmistamento(InfoUtInUo rowUtente)
        {
            DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;

            string tipoNotifica = rowUtente.CHA_NOTIFICA +
                                rowUtente.CHA_NOTIFICA_CON_ALLEGATO;

            if (tipoNotifica.Equals(""))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;
            else if (tipoNotifica.Equals("E") || tipoNotifica.Equals("E0"))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.Mail;
            else if (tipoNotifica.Equals("E1"))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.MailConAllegati;
            else if (tipoNotifica.Equals("1"))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.SoloAllegati;

            return retValue;
        }


        private async Task<List<UOSmistamento>> CreateUOSmistamento(SmistaInfo ds, string idPeople, string idRuolo, string livelloRuolo)
        {
            var uoEntities = ds.UoEntities;
            var ruoloEntities = ds.RuoloEntities;
            var utentiUoAppEntities = ds.UtentiUoAppEntities;

            List<UOSmistamento> retValue = new();
            var uoAssRuoEnt = uoEntities.GroupJoin(ruoloEntities,
                       (uo) => uo.ID,
                       (r) => r.ID_UO,
                       (uo, rc) => new
                       {
                           uo,
                           rc
                       }).ToDictionary(uoAssRuo => uoAssRuo.uo);

            var ruoAssUtUo = ruoloEntities.GroupJoin(utentiUoAppEntities,
                (r) => r.ID_GRUPPO,
                (ut) => ut.ID_GRUPPO,
                (r, utc) => new RoleAssUtInfo
                {
                    UoRole = r,
                    UtUo = utc
                }).ToDictionary(ruoAssUt => ruoAssUt.UoRole);

            DocsPaVO.Smistamento.UOSmistamento uo = null;
            DocsPaVO.Smistamento.RuoloSmistamento ruolo = null;
            DocsPaVO.Smistamento.UtenteSmistamento utente = null;

            foreach (var uoEnt in uoAssRuoEnt.Keys)
            {
                var uoRuoli = uoAssRuoEnt[uoEnt].rc;

                uo = new DocsPaVO.Smistamento.UOSmistamento();
                uo.ID = uoEnt.ID.ToString();
                uo.Codice = uoEnt.CODICE_UO ?? string.Empty;
                uo.Descrizione = uoEnt.DESCRIZIONE_UO ?? string.Empty;
                List<RuoloSmistamento> rolSmis = new();

                foreach (var r in uoRuoli)
                {
                    RoleAssUtInfo? ruoliUt = new();
                    bool hasRuoUtUo = ruoAssUtUo.TryGetValue(r, out ruoliUt);
                    List<UtenteSmistamento> utSmis = new();


                    if (hasRuoUtUo && ruoliUt != null && ruoliUt.UtUo.Any())
                    {
                        ruolo = new DocsPaVO.Smistamento.RuoloSmistamento();
                        ruolo.ID = r.ID.ToString();
                        ruolo.Codice = r.CODICE_RUOLO ?? string.Empty;
                        ruolo.Descrizione = r.DESCRIZIONE_RUOLO ?? string.Empty;
                        ruolo.RuoloRiferimento = (r.RUOLO_RIFERIMENTO != null ? r.RUOLO_RIFERIMENTO.Equals("1") : false);
                        ruolo.Registri = await this.GetRegistriRuolo(r.ID.ToString());

                        //indica se il ruolo è superiore a quello che sta smistando
                        //è popolato solamente per la Uo di Appartenenza
                        if (r.GERARCHIA != null)
                            ruolo.Gerarchia = r.GERARCHIA > livelloRuolo.AsLong() ? "0" : (r.GERARCHIA == livelloRuolo.AsLong() ? "2" : "1");

                        rolSmis.Add(ruolo);

                        foreach (var rowUtente in ruoliUt.UtUo)
                        {
                            if ((!rowUtente.ID.ToString().Equals(idPeople)) ||
                                 (rowUtente.ID.ToString().Equals(idPeople) && !ruolo.ID.Equals(idRuolo)))
                            {

                                utente = new DocsPaVO.Smistamento.UtenteSmistamento();
                                utente.ID = rowUtente.ID.ToString();
                                if (rowUtente.ID_CORR_GLOBALI != null)
                                {
                                    utente.IDCorrGlobali = ((long)rowUtente.ID_CORR_GLOBALI).ToString();
                                }
                                utente.UserID = rowUtente.CODICE_UTENTE ?? string.Empty;
                                utente.Denominazione = rowUtente.DESCRIZIONE_UTENTE ?? string.Empty;
                                utente.TipoNotificaSmistamento = this.GetTipoNotificaSmistamento(rowUtente);
                                utente.EMail = rowUtente.EMAIL_UTENTE ?? string.Empty;

                                utSmis.Add(utente);
                                utente = null;
                            }
                        }
                        ruolo.Utenti = utSmis.ToArray();

                    }
                    ruolo = null;
                }

                uo.Ruoli = rolSmis.ToArray();
                retValue.Add(uo);
            }

            return retValue;
        }

        private async Task<List<UOSmistamento>> GetUoInf(
            string idUOAppartenenza,
            DocsPaVO.Smistamento.MittenteSmistamento mittente
            )
        {
            List<UOSmistamento> UoList = null;

            var ds = await this.GetUOInferiori(idUOAppartenenza, mittente.RegistriAppartenenza, mittente.IDAmministrazione, mittente.LivelloRuolo);

            if (ds != null)
            {
                UoList = await CreateUOSmistamento(ds, mittente.IDPeople, mittente.IDCorrGlobaleRuolo, mittente.LivelloRuolo);
            }
            return UoList;
        }


        private async Task<SmistaInfo?> GetUOInferiori(
            string idUnitaOrganizzativa,
            string[] registriAppartenenza,
            string idAmministrazione,
            string livelloRuolo
            )
        {
            List<long> conditionRegistri = this.GetConditionRegistriAppartenenza(registriAppartenenza);
            List<InfoUo> queryRes = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(),
                    cg => cg.SYSTEM_ID,
                    cgg => cgg.ID_UO,
                    (cg, cgg) => new {cg, cgg })
                .Join(_dbContext.UoRegEnties.AsNoTracking(),
                    j => j.cg.SYSTEM_ID,
                    uor => uor.ID_UO,
                    (j, uor) => new { j.cg, j.cgg, uor })
                .Where(j => j.cg.ID_PARENT == idUnitaOrganizzativa.AsLong() &&
                        j.uor.ID_REGISTRO != null && conditionRegistri.Contains(j.uor.ID_REGISTRO.Value) &&
                        j.cgg.ID_UO > 0 &&
                        j.cg.SYSTEM_ID > 0 &&
                        !j.cg.DTA_FINE.HasValue &&
                        !j.cgg.DTA_FINE.HasValue)
                .Select(j => new InfoUo()
                {
                    ID = j.cg.SYSTEM_ID,
                    CODICE_UO = j.cg.VAR_COD_RUBRICA,
                    DESCRIZIONE_UO = j.cg.VAR_DESC_CORR,
                    ID_PESO_ORG = j.cg.ID_PESO_ORG
                })
                .ToListAsync();

            queryRes = queryRes.DistinctBy(x => x.ID).OrderBy(x => x.ID_PESO_ORG).ThenBy(x => x.DESCRIZIONE_UO).ToList();

            SmistaInfo? ds = null;
            if (queryRes != null)
            {
                ds = await this.FillRuoliInUOInferiori(queryRes, livelloRuolo, this.ConcatArrayRegRuolo(registriAppartenenza), false);
            }

            return ds;
        }


        private async Task<SmistaInfo?> FillRuoliInUOInferiori(List<InfoUo> uoEntities, string livelloRuolo, string listaRegistri, bool fromProtoSempl)
        {

            List<long> listaReg = new();
            SmistaInfo result = null;
            List<InfoUoWithRole> ruoloEntities = new();
            List<InfoUtInUo> utentiUoAppEntities = new();

            if (listaRegistri != null && listaRegistri.Length > 0)
            {
                listaReg = listaRegistri.Split(',').Select(s => s.AsLong()).ToList();
            }

            foreach (var rowUO in uoEntities)
            {
                var tempRuoloEnt = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(_dbContext.TipoRuoloEntities.AsNoTracking(),
                        cg => cg.ID_TIPO_RUOLO,
                        tr => tr.SYSTEM_ID,
                        (cg, tr) => new {cg, tr})
                    .Join(_dbContext.RuoloRegistroEntities.AsNoTracking(),
                        j => j.cg.SYSTEM_ID,
                        rr => rr.ID_RUOLO_IN_UO,
                        (j, rr) => new {j.cg, j.tr, rr})
                    .Where(j => j.rr.ID_REGISTRO != null && listaReg.Contains(j.rr.ID_REGISTRO.Value) &&
                        !j.rr.DTA_FINE.HasValue &&
                        j.cg.ID_UO == rowUO.ID &&
                        j.cg.CHA_TIPO_URP == "R" && 
                        !j.cg.DTA_FINE.HasValue)
                    .Select(j => new InfoUoWithRole()
                    {
                        ID = j.cg.SYSTEM_ID,
                        ID_UO = j.cg.ID_UO,
                        ID_GRUPPO = j.cg.ID_GRUPPO,
                        CODICE_RUOLO = j.cg.VAR_CODICE,
                        DESCRIZIONE_RUOLO = j.cg.VAR_DESC_CORR,
                        RUOLO_RIFERIMENTO = j.cg.CHA_RIFERIMENTO,
                        GERARCHIA = j.tr.NUM_LIVELLO
                    })
                    .ToListAsync();
                ruoloEntities.AddRange(tempRuoloEnt.DistinctBy(x => x.ID).ToList());

                var rowsGruppiUO = ruoloEntities.Where(uo => uo.ID_UO == rowUO.ID).ToList();
                List<long?> paramIDRuoli = new();

                if (rowsGruppiUO != null && rowsGruppiUO.Count > 0)
                {
                    foreach (var rowRuolo in rowsGruppiUO)
                    {
                        paramIDRuoli.Add(rowRuolo.ID_GRUPPO);
                    }
                    // qui trova 0
                    if (paramIDRuoli.Count > 0)
                    {
                        var utentiEntities = await _dbContext.GroupEntities.AsNoTracking()
                            .Join(_dbContext.PeopleGroupEntities.AsNoTracking(),
                                g => g.SYSTEM_ID,
                                pg => pg.GROUPS_SYSTEM_ID,
                                (g, pg) => new {g, pg})
                            .Join(_dbContext.PeopleEntities.AsNoTracking(),
                                j => j.pg.PEOPLE_SYSTEM_ID,
                                p => p.SYSTEM_ID,
                                (j, p) => new {j.g, j.pg, p})
                            .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(),
                                j => j.p.SYSTEM_ID,
                                cg => cg.ID_PEOPLE,
                                (j, cg) => new {j.g, j.pg, j.p, cg})
                            .Where(j => paramIDRuoli.Contains(j.g.SYSTEM_ID) &&
                                   j.p.DISABLED != null &&
                                   j.p.DISABLED != "Y" &&
                                   !j.pg.DTA_FINE.HasValue && 
                                   j.cg.CHA_TIPO_URP != null &&
                                   j.cg.CHA_TIPO_URP != "L")
                            .Select(j => new InfoUtInUo()
                            {
                                ID = j.p.SYSTEM_ID,
                                ID_CORR_GLOBALI = j.cg.SYSTEM_ID,
                                ID_GRUPPO = j.g.SYSTEM_ID,
                                CODICE_UTENTE = j.p.USER_ID,
                                DESCRIZIONE_UTENTE = j.p.FULL_NAME,
                                EMAIL_UTENTE = j.p.EMAIL_ADDRESS,
                                CHA_NOTIFICA = j.p.CHA_NOTIFICA,
                                CHA_NOTIFICA_CON_ALLEGATO = j.p.CHA_NOTIFICA_CON_ALLEGATO,
                                VAR_COGNOME = j.p.VAR_COGNOME
                            })
                            .OrderBy(j => j.VAR_COGNOME)
                            .ToListAsync();

                        if (utentiEntities.Any())
                            utentiUoAppEntities.AddRange(utentiEntities);
                    }
                }
            }
            result = new()
            {
                UtentiUoAppEntities = utentiUoAppEntities,
                RuoloEntities = ruoloEntities,
                UoEntities = uoEntities
            };
            return result;

        }

        private List<long> GetConditionRegistriAppartenenza(string[] registriAppartenenza)
        {

            return registriAppartenenza.Select(r => r.AsLong()).ToList();
        }

        private string ConcatArrayRegRuolo(string[] listaRegRuolo)
        {
            string retValue = string.Empty;
            for (int n = 0; n <= listaRegRuolo.Count() - 1; n++)
            {
                retValue += "," + listaRegRuolo[n].ToString();
            }
            retValue = retValue.Substring(1, retValue.Length - 1);
            return retValue;
        }

        public GetUOInferioriHandler
            (
            ILogger<GetUOInferioriHandler> logger,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
        }

        public async Task<GetUOInferioriResult> Handle(GetUOInferioriRequest request, CancellationToken cancellationToken)
        {
            UOSmistamento[] output = null;
            try
            {
                List<UOSmistamento> result = await this.GetUoInf(request.idUOAppartenenza, request.mittente);
                output = result != null ? result.ToArray() : null;
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }

    }
}
