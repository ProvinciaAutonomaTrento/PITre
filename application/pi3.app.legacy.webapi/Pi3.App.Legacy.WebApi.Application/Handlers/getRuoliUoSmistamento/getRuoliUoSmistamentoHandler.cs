// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Interoperabilita.Segnatura;
using DocsPaVO.Smistamento;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getRuoliUoSmistamentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.getRuoliUoSmistamento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getRuoliUoSmistamento
{
    public class getRuoliUoSmistamentoHandler : IRequestHandler<getRuoliUoSmistamentoRequest, getRuoliUoSmistamentoResult>
    {

        protected readonly ILogger<getRuoliUoSmistamentoHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        private async Task<RuoloSmistamento[]> GetRuoliUoSmista(string idUo)
        {
            var ruoli = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                where (a.CHA_TIPO_URP != null && a.CHA_TIPO_URP.Equals("R")) &&
                                (a.CHA_TIPO_IE != null && a.CHA_TIPO_IE.Equals("I")) &&
                                (!a.DTA_FINE.HasValue) &&
                                (a.ID_UO == idUo.AsLong())
                                orderby a.VAR_DESC_CORR ascending
                                select new
                                {
                                    ID = a.SYSTEM_ID,
                                    DESCRIZIONE = a.VAR_DESC_CORR,
                                    CODICE = a.VAR_CODICE,
                                    a.CHA_RIFERIMENTO
                                }).ToListAsync();

            List<RuoloSmistamento> roles = new();

            ruoli.ForEach( async(role) =>
            {
                DocsPaVO.Smistamento.RuoloSmistamento ruoloSmista = new DocsPaVO.Smistamento.RuoloSmistamento();
                ruoloSmista.ID = role.ID.ToString();
                ruoloSmista.Codice = role.CODICE;
                ruoloSmista.Descrizione = role.DESCRIZIONE;
                ruoloSmista.Registri = await this.GetRegistriRuolo(ruoloSmista.ID);
                ruoloSmista.Utenti = await GetUtentiRuoloSmistamento(ruoloSmista.ID,"R");
                roles.Add(ruoloSmista);
            });

            return roles.ToArray();
        }


        private async Task<string[]> GetRegistriRuolo(string idRuolo)
        {

            string?[] registri = new string[0];

            registri = await this._dbContext.RuoloRegistroEntities.AsNoTracking()
                .Where(ruoloReg => ruoloReg.ID_RUOLO_IN_UO == idRuolo.AsLong() && !ruoloReg.DTA_FINE.HasValue)
                .Select(ruoloReg => ruoloReg.ID_REGISTRO.ToString()).ToArrayAsync();

            return registri;

        }


        private class UtRuoloInfo
        {
            public long Id { get; set; }
            public long IdCorrGlobali { get; set; }
            public string? CodiceUtente { get; set; }
            public string? DescrizioneUtente { get; set; }
            public string? EmailUtente { get; set; }
            public string? ChaNotifica { get; set; }
            public string? ChaNotificaConAllegato { get; set; }
            public string? VarCognome { get; set; }
            public long? GroupSysId { get; set; }
            public long? CorrSysId { get; set; }
        }
        private async Task<UtenteSmistamento[]> GetUtentiRuoloSmistamento(string id, string queryParam)
        {
            List<UtenteSmistamento> utentiRuolo = new();

            var query = this._dbContext.CorrGlobaliEntities.AsNoTracking().Join(
                this._dbContext.PeopleGroupEntities.AsNoTracking(),
                (corr) => true,
                (pg) => true,
                (corr, pg) => new
                {
                    corr,
                    pg
                }).Join(
                this._dbContext.PeopleEntities.AsNoTracking(),
                (cpg) => true,
                (p) => true,
                (cpg, p) => new
                {
                    cpg,
                    p
                }).Where(agg => 
                (agg.cpg.corr.ID_PEOPLE == agg.cpg.pg.PEOPLE_SYSTEM_ID) && 
                (agg.cpg.pg.PEOPLE_SYSTEM_ID == agg.p.SYSTEM_ID) && 
                (agg.p.DISABLED != null && agg.p.DISABLED.Equals("N")) && 
                (!agg.cpg.corr.DTA_FINE.HasValue) && 
                (!agg.cpg.pg.DTA_FINE.HasValue) && 
                (agg.cpg.corr.CHA_TIPO_URP != null && agg.cpg.corr.CHA_TIPO_URP.Equals("P")) && 
                (agg.cpg.corr.CHA_TIPO_IE != null && agg.cpg.corr.CHA_TIPO_IE.Equals("I"))).OrderBy( agg => agg.cpg.corr.VAR_COGNOME)
                .Select( agg => new UtRuoloInfo
                (){
                    Id = agg.p.SYSTEM_ID ,
                    IdCorrGlobali = agg.cpg.corr.SYSTEM_ID,
                    CodiceUtente = agg.p.USER_ID ,
                    DescrizioneUtente = agg.p.FULL_NAME,
                    EmailUtente = agg.p.EMAIL_ADDRESS ,
                    ChaNotifica = agg.p.CHA_NOTIFICA,
                    ChaNotificaConAllegato = agg.p.CHA_NOTIFICA_CON_ALLEGATO,
                    VarCognome = agg.cpg.corr.VAR_COGNOME,
                    GroupSysId = agg.cpg.pg.GROUPS_SYSTEM_ID,
                    CorrSysId = agg.cpg.corr.SYSTEM_ID
                });

            var predicate = PredicateBuilder.New<UtRuoloInfo>();

            if (queryParam.Equals("R"))
            {
                long? idGroupCorr = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.SYSTEM_ID == id.AsLong()).Select( c => c.ID_GRUPPO).FirstAsync();

                predicate = predicate.And( utr => utr.GroupSysId == idGroupCorr);
            }
            else
            {
                predicate = predicate.And(utr => utr.CorrSysId == id.AsLong());
            }

            var result = await query.Where(predicate).ToListAsync();

            result.ForEach((rowUtente) =>
            {
                DocsPaVO.Smistamento.UtenteSmistamento utSmistamento = new DocsPaVO.Smistamento.UtenteSmistamento();
                utSmistamento.ID = rowUtente.Id.ToString();
                utSmistamento.IDCorrGlobali = rowUtente.IdCorrGlobali.ToString();
                utSmistamento.UserID = rowUtente.CodiceUtente;
                utSmistamento.Denominazione = rowUtente.DescrizioneUtente;
                utSmistamento.EMail = rowUtente.EmailUtente;
                utSmistamento.TipoNotificaSmistamento = this.GetTipoNotificaSmistamento(rowUtente);
                utentiRuolo.Add(utSmistamento);

            });

            return utentiRuolo.ToArray();

        }


        private DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum GetTipoNotificaSmistamento(UtRuoloInfo rowUtente)
        {
            DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;

            string tipoNotifica = rowUtente.ChaNotifica +
                                rowUtente.ChaNotificaConAllegato;

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


        public getRuoliUoSmistamentoHandler(
            ILogger<getRuoliUoSmistamentoHandler> logger,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
        }


        public async Task<getRuoliUoSmistamentoResult> Handle(getRuoliUoSmistamentoRequest request, CancellationToken cancellationToken)
        {
            RuoloSmistamento[]? output = null;
            try
            {
                output = await this.GetRuoliUoSmista(request.idUo);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new getRuoliUoSmistamentoResult(output);
        }

    }
}
