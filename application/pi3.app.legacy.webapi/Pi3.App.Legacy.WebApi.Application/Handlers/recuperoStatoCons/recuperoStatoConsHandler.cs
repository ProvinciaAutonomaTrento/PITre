// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Conservazione;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using recuperoStatoConsRequest = Pi3.App.Legacy.WebApi.Application.Requests.recuperoStatoCons;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.recuperoStatoCons
{
    public class recuperoStatoConsHandler : IRequestHandler<recuperoStatoConsRequest, recuperoStatoConsResult>
    {
        #region Public Members

        public recuperoStatoConsHandler(ILogger<recuperoStatoConsHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, ISIPService sipService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._sipService = sipService;
        }

        public async Task<recuperoStatoConsResult> Handle(recuperoStatoConsRequest request, CancellationToken cancellationToken)
        {
            var output = await this._sipService.Get(request.idDoc);

            return new recuperoStatoConsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<recuperoStatoConsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ISIPService _sipService;

        #endregion
    }
}