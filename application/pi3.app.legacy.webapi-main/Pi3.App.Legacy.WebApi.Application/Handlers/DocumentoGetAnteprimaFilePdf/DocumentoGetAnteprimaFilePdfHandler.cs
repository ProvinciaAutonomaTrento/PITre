// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetAnteprimaFilePdfRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetAnteprimaFilePdf;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetAnteprimaFilePdf
{
    public class DocumentoGetAnteprimaFilePdfHandler : IRequestHandler<DocumentoGetAnteprimaFilePdfRequest, DocumentoGetAnteprimaFilePdfResult>
    {
        #region Public Members

        public DocumentoGetAnteprimaFilePdfHandler(ILogger<DocumentoGetAnteprimaFilePdfHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<DocumentoGetAnteprimaFilePdfResult> Handle(DocumentoGetAnteprimaFilePdfRequest request, CancellationToken cancellationToken)
        {
            var msgError = string.Empty;

            if (!GetEstensioneIntoSignedFile(request.fileRequest.fileName.ToUpper()).Equals("PDF"))
                throw new FormatoFileNonPDFPi3Exception();

            var fileDocumento = (await this._mediator.Send(new Requests.DocumentoGetFileConSegnatura(request.fileRequest, request.sch, request.infoUtente, request.position, false))).output;

            var fileDocumentoAnteprima = new FileDocumentoAnteprima();
            fileDocumentoAnteprima.Import(fileDocumento);
            fileDocumentoAnteprima.firstPg = 1;

            return new DocumentoGetAnteprimaFilePdfResult(fileDocumentoAnteprima, msgError);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetAnteprimaFilePdfHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected string GetEstensioneIntoSignedFile(string fullname)
        {
            string retValue = string.Empty;

            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');

            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M") ||
                    items[i].ToUpper().EndsWith("TSD") ||
                    items[i].ToUpper().EndsWith("M7M"))
                    )
                {
                    retValue = items[i];
                    break;
                }
            }
            return retValue;
        }

        #endregion
    }
}