// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateIstanzaProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateIstanzaProcessoDiFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateIstanzaProcessoDiFirma
{

    public class UpdateIstanzaProcessoDiFirmaHandler : IRequestHandler<UpdateIstanzaProcessoDiFirmaRequest, UpdateIstanzaProcessoDiFirmaResult>
    {
        #region Public Members

        public UpdateIstanzaProcessoDiFirmaHandler(ILogger<UpdateIstanzaProcessoDiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<UpdateIstanzaProcessoDiFirmaResult> Handle(UpdateIstanzaProcessoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idIstanza = request.istanzaProcesso.idIstanzaProcesso.AsLong();

                var istanzaProcessoFirmaEntity = await this._dbContext.IstanzaProcessoFirmaEntities
                    .Where(i => i.ID_ISTANZA == idIstanza)
                    .FirstAsync();

                istanzaProcessoFirmaEntity.NOTIFICA_CONCLUSO = request.istanzaProcesso.Notifiche.Notifica_concluso ? "1" : "0";
                istanzaProcessoFirmaEntity.NOTIFICA_INTERROTTO = request.istanzaProcesso.Notifiche.Notifica_interrotto ? "1" : "0";
                istanzaProcessoFirmaEntity.NOTIFICA_ERRORE = request.istanzaProcesso.Notifiche.NotificaErrore ? "1" : "0";
                istanzaProcessoFirmaEntity.NOTIFICA_DEST_NON_INTEROP = request.istanzaProcesso.Notifiche.NotificaPresenzaDestNonInterop ? "1" : "0";

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new UpdateIstanzaProcessoDiFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateIstanzaProcessoDiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
