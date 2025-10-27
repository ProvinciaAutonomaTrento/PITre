// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.addressbook;
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.RemoveNotifications;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeStateReadNotificationRequest = Pi3.App.Legacy.WebApi.Application.Requests.ChangeStateReadNotification;
using CheckNotificationRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckNotification;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.ChangeStateReadNotification
{

    public class ChangeStateReadNotificationHandler : IRequestHandler<ChangeStateReadNotificationRequest, ChangeStateReadNotificationResult>
    {
        #region Public Members

        public ChangeStateReadNotificationHandler(ILogger<ChangeStateReadNotificationHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<ChangeStateReadNotificationResult> Handle(ChangeStateReadNotificationRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            var removeNotify = false;
            try
            {
                long? idTrasmSingolaAsLong = 0;
                if (!string.IsNullOrEmpty(request.notify.ID_SPECIALIZED_OBJECT))
                    idTrasmSingolaAsLong = request.notify.ID_SPECIALIZED_OBJECT.AsLong();

                //Se l'amministrazione non ha abilitato il tasto visto e la notifica non è relativa ad una trasmissione di tipo workflow o interoperabilita pitre rimuovo la notifica.
                if (request.isNotEnabledSetDataVistaGrd)
                {
                    removeNotify = idTrasmSingolaAsLong == 0 || await this._dbContext.RagioneTrasmissioneEntities
                        .Join(this._dbContext.TrasmSingolaEntities, ragione => ragione.SYSTEM_ID, trasmSingola => trasmSingola.ID_RAGIONE, (ragione, trasmSingola) => new { ragione, trasmSingola })
                        .AnyAsync(j => j.trasmSingola.SYSTEM_ID == idTrasmSingolaAsLong && j.ragione.CHA_TIPO_RAGIONE != "W" && j.ragione.CHA_TIPO_RAGIONE != "S");
                }

                if (removeNotify)
                {
                    var checkNotification = await this._mediator.Send(new CheckNotificationRequest(request.notify, null));
                    output = checkNotification.output;
                }
                else
                {
                    var systemIdNotifyAsLong = request.notify.ID_NOTIFY.AsLong();

                    var notifyEntity = await this._dbContext.NotifyEntities
                        .Where(n => n.SYSTEM_ID == systemIdNotifyAsLong)
                        .FirstOrDefaultAsync();

                    if (notifyEntity != null)
                    {
                        notifyEntity.READ_NOTIFICATION = "1";
                        await ((DbContext)_dbContext).SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                output = false;
                this._logger.LogError(ex, null, null);
            }

            return new ChangeStateReadNotificationResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ChangeStateReadNotificationHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
