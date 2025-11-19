// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Smistamento;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetUOAppartenenzaRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetUOAppartenenza;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetUOAppartenenza
{
    public class GetUOAppartenenzaHandler : IRequestHandler<GetUOAppartenenzaRequest, GetUOAppartenenzaResult>
    {
        #region Public Members

        public GetUOAppartenenzaHandler(
            ILogger<GetUOAppartenenzaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetUOAppartenenzaResult> Handle(GetUOAppartenenzaRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.Smistamento.UOSmistamento? uoSmistamento = null!;
            
            try
            {
                var query = this._pi3DbContext.CorrGlobaliEntities.AsNoTracking().AsQueryable();

                if (request.isCurrentUO)
                    query = query.Where(cg => cg.SYSTEM_ID == request.idUnitaOrganizzativa.AsLong());
                else
                    query = query.Where(cg => cg.SYSTEM_ID == this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                                    .Where(cg2 => cg2.SYSTEM_ID == request.idUnitaOrganizzativa.AsLong())
                                                    .Select(cg2 => cg2.ID_UO)
                                                    .First());

                uoSmistamento = await query.Select(cg => new DocsPaVO.Smistamento.UOSmistamento()
                    {
                        ID = cg.SYSTEM_ID.ToString(),
                        Codice = cg.VAR_COD_RUBRICA,
                        Descrizione = cg.VAR_DESC_CORR
                    })
                    .FirstOrDefaultAsync();

                if (uoSmistamento == null)
                    throw new GetUOAppartenenzaPi3Exception(ErrorDescriptions.UoNonTrovata);

                var idUO = uoSmistamento.ID.AsLong();
                var registriAppartenenza = Array.ConvertAll(request.mittente.RegistriAppartenenza, long.Parse);
                var livelloRuolo = request.mittente.LivelloRuolo.AsLong();

                var ruoliSmistamentoEntity = await (from cg in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                    join tr in this._pi3DbContext.TipoRuoloEntities.AsNoTracking() on cg.ID_TIPO_RUOLO equals tr.SYSTEM_ID
                                    join rr in this._pi3DbContext.RuoloRegistroEntities.AsNoTracking() on cg.SYSTEM_ID equals rr.ID_RUOLO_IN_UO
                                    where registriAppartenenza.Contains(rr.ID_REGISTRO.Value)
                                        && rr.DTA_FINE == null
                                        && cg.ID_UO == idUO
                                        && cg.CHA_TIPO_URP == "R"
                                        && cg.DTA_FINE == null
                                    select new RuoloSmistamentoEntity()
                                    {
                                        ID = cg.SYSTEM_ID,
                                        ID_GRUPPO = cg.ID_GRUPPO,
                                        CODICE = cg.VAR_CODICE!,
                                        DESCRIZIONE = cg.VAR_DESC_CORR!,
                                        RUOLO_RIFERIMENTO = (!string.IsNullOrWhiteSpace(cg.CHA_RIFERIMENTO) ? cg.CHA_RIFERIMENTO == "1" : false),
                                        GERARCHIA = cg.NUM_LIVELLO > livelloRuolo ? "0" :
                                                        cg.NUM_LIVELLO == livelloRuolo ? "2" :
                                                                cg.NUM_LIVELLO < livelloRuolo ? "1" : null,
                                        DISABLED_TRASM = cg.CHA_DISABLED_TRASM == "1",
                                        NUMERO_LIVELLO = tr.NUM_LIVELLO                               
                                    })                       
                        .Distinct()
                        .OrderBy(r => r.NUMERO_LIVELLO)
                        .ToArrayAsync();

                List<RuoloSmistamento> ruoli = new List<RuoloSmistamento>();
                foreach (var ruolo in ruoliSmistamentoEntity)
                {
                    var ruoloSmistamento = new RuoloSmistamento()
                    {
                        ID = ruolo.ID.ToString(),
                        Codice = ruolo.CODICE,
                        Descrizione = ruolo.DESCRIZIONE,
                        RuoloRiferimento = ruolo.RUOLO_RIFERIMENTO,
                        Gerarchia = ruolo.GERARCHIA,
                        disabledTrasm = ruolo.DISABLED_TRASM,
                        Registri = (await this._mediator.Send(new Requests.SmistaGetRegistriRuolo(ruolo.ID.ToString()))).output                       
                    };

                    ruoloSmistamento.Utenti = await (from g in this._pi3DbContext.GroupEntities.AsNoTracking()
                                   join pg in this._pi3DbContext.PeopleGroupEntities.AsNoTracking() on g.SYSTEM_ID equals pg.GROUPS_SYSTEM_ID
                                   join p in this._pi3DbContext.PeopleEntities.AsNoTracking() on pg.PEOPLE_SYSTEM_ID equals p.SYSTEM_ID
                                   join cg in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking() on p.SYSTEM_ID equals cg.ID_PEOPLE
                                   where g.SYSTEM_ID == ruolo.ID_GRUPPO
                                       && p.DISABLED != "Y"
                                       && pg.DTA_FINE == null
                                       && cg.CHA_TIPO_URP != "L"
                                   orderby p.VAR_COGNOME
                                   select new DocsPaVO.Smistamento.UtenteSmistamento()
                                   {
                                       ID = p.SYSTEM_ID.ToString(),
                                       IDCorrGlobali = cg.SYSTEM_ID.ToString(),
                                       UserID = p.USER_ID,
                                       Denominazione = p.FULL_NAME,
                                       EMail = p.EMAIL_ADDRESS,
                                       TipoNotificaSmistamento = GetTipoNotificaSmistamento(p.CHA_NOTIFICA, p.CHA_NOTIFICA_CON_ALLEGATO)
                                   })
                            .ToArrayAsync();

                    ruoli.Add(ruoloSmistamento);
                }

                uoSmistamento.Ruoli = ruoli.ToArray();
            }
            catch (Pi3Exception pi3Ex)
            {
                uoSmistamento = null!;
                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                uoSmistamento = null!;
                this._logger.LogCritical(ex, ex.Message);
            }

            return new GetUOAppartenenzaResult(uoSmistamento!);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetUOAppartenenzaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        protected class RuoloSmistamentoEntity
        {
            public long? ID { get; set; }
            public long? ID_GRUPPO { get; set; }
            public long? NUMERO_LIVELLO { get; set; }
            public string? CODICE { get; set; }
            public string? DESCRIZIONE { get; set; }
            public bool RUOLO_RIFERIMENTO { get; set; }
            public bool DISABLED_TRASM { get; set; }
            public string? GERARCHIA { get; set; }
        }

        private static DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum GetTipoNotificaSmistamento(string chaNotifica, string chaNotificaConAllegato)
        {
            DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;

            string tipoNotifica = chaNotifica + chaNotificaConAllegato;

            if (tipoNotifica.Equals(string.Empty))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;
            else if (tipoNotifica.Equals("E") || tipoNotifica.Equals("E0"))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.Mail;
            else if (tipoNotifica.Equals("E1"))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.MailConAllegati;
            else if (tipoNotifica.Equals("1"))
                retValue = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.SoloAllegati;

            return retValue;
        }

        #endregion
    }
}