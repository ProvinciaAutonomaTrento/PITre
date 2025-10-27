// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getDatiAggiuntiviUtenteSmistamentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDatiAggiuntiviUtenteSmistamento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDatiAggiuntiviUtenteSmistamento
{

    // Richiede libreria MediatR
    public class getDatiAggiuntiviUtenteSmistamentoHandler : IRequestHandler<getDatiAggiuntiviUtenteSmistamentoRequest, getDatiAggiuntiviUtenteSmistamentoResult>
    {
        #region Public Members

        public getDatiAggiuntiviUtenteSmistamentoHandler(ILogger<getDatiAggiuntiviUtenteSmistamentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getDatiAggiuntiviUtenteSmistamentoResult> Handle(getDatiAggiuntiviUtenteSmistamentoRequest request, CancellationToken cancellationToken)
        {
            var utenteSmistamento = request.utenteSmistamento;

            try
            {
                var idPeople = request.idUtente.AsLong();
                var datiAggiuntivi = await this._dbContext.PeopleEntities.AsNoTracking()
                    .Join(this._dbContext.CorrGlobaliEntities,
                    p => p.SYSTEM_ID,
                    c => c.ID_PEOPLE,
                    (p, c) => new DatiAggiuntiviUtenteEntity()
                    {
                        ID_CORR_GLOBALI = c.SYSTEM_ID,
                        ID_PEOPLE = p.SYSTEM_ID,
                        EMAIL_UTENTE = p.EMAIL_ADDRESS,
                        CHA_NOTIFICA = p.CHA_NOTIFICA,
                        CHA_NOTIFICA_CON_ALLEGATO = p.CHA_NOTIFICA_CON_ALLEGATO
                    })
                    .Where(p => p.ID_PEOPLE == idPeople)
                    .FirstOrDefaultAsync();

                if(datiAggiuntivi != null)
                {
                    utenteSmistamento.IDCorrGlobali = datiAggiuntivi.ID_CORR_GLOBALI.ToString();
                    utenteSmistamento.ID = datiAggiuntivi.ID_PEOPLE.ToString();
                    utenteSmistamento.EMail = datiAggiuntivi.EMAIL_UTENTE;

                    utenteSmistamento.TipoNotificaSmistamento = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;
                    var tipoNotifica = datiAggiuntivi.CHA_NOTIFICA + datiAggiuntivi.CHA_NOTIFICA_CON_ALLEGATO;
                    if (tipoNotifica.Equals(""))
                        utenteSmistamento.TipoNotificaSmistamento = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.NoMail;
                    else if (tipoNotifica.Equals("E") || tipoNotifica.Equals("E0"))
                        utenteSmistamento.TipoNotificaSmistamento = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.Mail;
                    else if (tipoNotifica.Equals("E1"))
                        utenteSmistamento.TipoNotificaSmistamento = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.MailConAllegati;
                    else if (tipoNotifica.Equals("1"))
                        utenteSmistamento.TipoNotificaSmistamento = DocsPaVO.Smistamento.TipoNotificaSmistamentoEnum.SoloAllegati;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new getDatiAggiuntiviUtenteSmistamentoResult(utenteSmistamento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDatiAggiuntiviUtenteSmistamentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected class DatiAggiuntiviUtenteEntity
        {
            public long? ID_PEOPLE { get; set; }
            public long? ID_CORR_GLOBALI { get; set; }
            public string? EMAIL_UTENTE { get; set; }
            public string? CHA_NOTIFICA { get; set; }
            public string? CHA_NOTIFICA_CON_ALLEGATO { get; set; }
        }
        #endregion
    }
}
