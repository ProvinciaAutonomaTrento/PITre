// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using IsEventoAutomaticoRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsEventoAutomatico;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsEventoAutomatico
{
    public class IsEventoAutomaticoHandler : IRequestHandler<IsEventoAutomaticoRequest, IsEventoAutomaticoResult>
    {
        #region Public Members

        public IsEventoAutomaticoHandler(ILogger<IsEventoAutomaticoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsEventoAutomaticoResult> Handle(IsEventoAutomaticoRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                output = await this._dbContext.AnagraficaEventiEntities
                    .AsNoTracking()
                    .AnyAsync(a => a.VAR_COD_AZIONE == request.codiceEvento && a.CHA_AUTOMATICO == "1");

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new IsEventoAutomaticoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsEventoAutomaticoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
