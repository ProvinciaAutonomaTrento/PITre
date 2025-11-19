// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsolidateDocumentById_AMRequest = Pi3.App.Legacy.WebApi.Application.Requests.ConsolidateDocumentById_AM;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ConsolidateDocumentById_AM
{
    public class ConsolidateDocumentById_AMHandler : IRequestHandler<ConsolidateDocumentById_AMRequest, ConsolidateDocumentById_AMResult>
    {
        #region Public Members

        public ConsolidateDocumentById_AMHandler(ILogger<ConsolidateDocumentById_AMHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<ConsolidateDocumentById_AMResult> Handle(ConsolidateDocumentById_AMRequest request, CancellationToken cancellationToken)
        {
            var consolidateDocument = await this._mediator.Send(new Requests.ConsolidateDocumentById(request.userInfo, request.idDocument, request.toState));

            return new ConsolidateDocumentById_AMResult(consolidateDocument.output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ConsolidateDocumentById_AMHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}