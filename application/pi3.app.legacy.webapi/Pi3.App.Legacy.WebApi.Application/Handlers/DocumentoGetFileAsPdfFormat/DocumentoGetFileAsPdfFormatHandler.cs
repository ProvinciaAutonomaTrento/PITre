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
using DocumentoGetFileAsPdfFormatRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileAsPdfFormat;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetFileAsPdfFormat
{
    public class DocumentoGetFileAsPdfFormatHandler : IRequestHandler<DocumentoGetFileAsPdfFormatRequest, DocumentoGetFileAsPdfFormatResult>
    {
        #region Public Members

        public DocumentoGetFileAsPdfFormatHandler(ILogger<DocumentoGetFileAsPdfFormatHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<DocumentoGetFileAsPdfFormatResult> Handle(DocumentoGetFileAsPdfFormatRequest request, CancellationToken cancellationToken)
        {
            var fileDocumento = (await this._mediator.Send(new Requests.DocumentoGetFile(request.fileRequest, request.infoUtente))).output;

            return new DocumentoGetFileAsPdfFormatResult(fileDocumento, false);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetFileAsPdfFormatHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}