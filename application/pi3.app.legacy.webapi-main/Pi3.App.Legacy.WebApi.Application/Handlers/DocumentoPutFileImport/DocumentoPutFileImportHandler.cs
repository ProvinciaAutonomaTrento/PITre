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
using DocumentoPutFileImportRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoPutFileImport;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoPutFileImport
{
    public class DocumentoPutFileImportHandler : IRequestHandler<DocumentoPutFileImportRequest, DocumentoPutFileImportResult>
    {
        #region Public Members

        public DocumentoPutFileImportHandler(ILogger<DocumentoPutFileImportHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<DocumentoPutFileImportResult> Handle(DocumentoPutFileImportRequest request, CancellationToken cancellationToken)
        {
            var fileRequest = (await _mediator.Send(new Requests.DocumentoPutFile(request.fileRequest, request.fileDocument, request.infoUtente))).output;

            return new DocumentoPutFileImportResult(fileRequest, string.Empty);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoPutFileImportHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}