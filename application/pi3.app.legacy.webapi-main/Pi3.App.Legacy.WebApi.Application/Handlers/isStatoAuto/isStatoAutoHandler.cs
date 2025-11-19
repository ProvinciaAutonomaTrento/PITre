// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.isStatoAuto
{
    public class isStatoAutoHandler : IRequestHandler<Application.Requests.isStatoAuto, isStatoAutoResult>
    {
        #region Public Members

        public isStatoAutoHandler(ILogger<isStatoAutoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<isStatoAutoResult> Handle(Application.Requests.isStatoAuto request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                var result = await this._mediator.Send(new Application.Requests.getDiagrammaById(request.idDiagramma));
                DiagrammaStato diagramma = result.output;

                output = diagramma.PASSI.ToArray().Any(p => (p as Passo).ID_STATO_AUTOMATICO == request.idStato);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new isStatoAutoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isStatoAutoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }

}
