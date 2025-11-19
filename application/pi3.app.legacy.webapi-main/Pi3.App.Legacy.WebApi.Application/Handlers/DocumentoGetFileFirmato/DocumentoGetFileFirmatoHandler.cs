// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetFileFirmatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileFirmato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetFileFirmato
{
    public class DocumentoGetFileFirmatoHandler : IRequestHandler<DocumentoGetFileFirmatoRequest, DocumentoGetFileFirmatoResult>
    {
        #region Public Members

        public DocumentoGetFileFirmatoHandler(ILogger<DocumentoGetFileFirmatoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService
        )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<DocumentoGetFileFirmatoResult> Handle(DocumentoGetFileFirmatoRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = null;
            try
            {
                output = (await this._mediator.Send(new Application.Requests.GetFileDocument(request.fileRequest, request.infoUtente))).output;
                await this._webMethodLoggerService.LogOK("DOCUMENTOGETFILE", request.fileRequest.docNumber, string.Format(Resources.DocumentoGetFileOK, request.fileRequest.docNumber, request.fileRequest.version));
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = null;
                await this._webMethodLoggerService.LogKO("DOCUMENTOGETFILE", request.fileRequest.docNumber, string.Format(Resources.DocumentoGetFileKO, request.fileRequest.docNumber, request.fileRequest.version));
            }
            return new DocumentoGetFileFirmatoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetFileFirmatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
