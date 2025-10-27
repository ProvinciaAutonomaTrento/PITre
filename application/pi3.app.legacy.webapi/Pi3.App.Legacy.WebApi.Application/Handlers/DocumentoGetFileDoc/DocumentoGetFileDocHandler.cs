// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetFileDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetFileDoc
{
    public class DocumentoGetFileDocHandler : IRequestHandler<DocumentoGetFileDocRequest, DocumentoGetFileDocResult>
    {
        #region Public Members

        public DocumentoGetFileDocHandler(ILogger<DocumentoGetFileDocHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<DocumentoGetFileDocResult> Handle(DocumentoGetFileDocRequest request, CancellationToken cancellationToken)
        {
            var msgError = string.Empty;

            var fileDocumento = (await this._mediator.Send(new Requests.DocumentoGetFile(request.fileRequest, request.infoUtente))).output;

            return new DocumentoGetFileDocResult(fileDocumento, msgError);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetFileDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}