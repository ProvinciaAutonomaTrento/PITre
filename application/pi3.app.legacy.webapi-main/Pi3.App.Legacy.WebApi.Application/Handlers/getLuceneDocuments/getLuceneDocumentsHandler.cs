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
using getLuceneDocumentsRequest = Pi3.App.Legacy.WebApi.Application.Requests.getLuceneDocuments;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getLuceneDocuments
{
    public class getLuceneDocumentsHandler : IRequestHandler<getLuceneDocumentsRequest, getLuceneDocumentsResult>
    {
        #region Public Members

        public getLuceneDocumentsHandler(ILogger<getLuceneDocumentsHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<getLuceneDocumentsResult> Handle(getLuceneDocumentsRequest request, CancellationToken cancellationToken)
        {
            return new getLuceneDocumentsResult(new DocsPaVO.Grids.SearchObject[0], 0);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getLuceneDocumentsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}