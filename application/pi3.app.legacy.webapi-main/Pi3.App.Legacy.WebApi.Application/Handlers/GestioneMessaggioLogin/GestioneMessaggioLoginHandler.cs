// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioneMessaggioLoginRequest = Pi3.App.Legacy.WebApi.Application.Requests.GestioneMessaggioLogin;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GestioneMessaggioLogin
{
    public class GestioneMessaggioLoginHandler : IRequestHandler<GestioneMessaggioLoginRequest, GestioneMessaggioLoginResult>
    {
        #region Public Members

        public GestioneMessaggioLoginHandler(ILogger<GestioneMessaggioLoginHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GestioneMessaggioLoginResult> Handle(GestioneMessaggioLoginRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;

            if (!string.IsNullOrEmpty(request.password) && request.password == "UpMsgLogin20XX")
            {
                var docspaEntity = await this._dbContext.DocsPaEntities.OrderByDescending(m => m.SYSTEM_ID).FirstAsync();
                docspaEntity.VAR_MESSAGGIO_LOGIN = request.message;

                await ((DbContext)this._dbContext).SaveChangesAsync();

                output = request.message;
            }
            else
            {
                output = await this._dbContext.DocsPaEntities.AsNoTracking().OrderByDescending(m => m.SYSTEM_ID).Select(m => m.VAR_MESSAGGIO_LOGIN).FirstAsync();
            }

            return new GestioneMessaggioLoginResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GestioneMessaggioLoginHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
