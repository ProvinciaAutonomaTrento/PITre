// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isFiltroAooEnabledRequest = Pi3.App.Legacy.WebApi.Application.Requests.isFiltroAooEnabled;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isFiltroAooEnabled
{
    public class isFiltroAooEnabledHandler : IRequestHandler<isFiltroAooEnabledRequest, isFiltroAooEnabledResult>
    {
        #region Public Members

        public isFiltroAooEnabledHandler(ILogger<isFiltroAooEnabledHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
        }

        public async Task<isFiltroAooEnabledResult> Handle(isFiltroAooEnabledRequest request, CancellationToken cancellationToken)
        {
            //Nel BE OLD di PiTre la chiave NO_FILTRO_AOO non è presente
            return new isFiltroAooEnabledResult(false);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isFiltroAooEnabledHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }

}
