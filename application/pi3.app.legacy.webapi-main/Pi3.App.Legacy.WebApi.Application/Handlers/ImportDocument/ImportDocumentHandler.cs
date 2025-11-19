// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImportDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.ImportDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportDocument
{
    public class ImportDocumentHandler : IRequestHandler<ImportDocumentRequest, ImportDocumentResult>
    {
        public ImportDocumentHandler(
            ILogger<ImportDocumentHandler> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }


        public async Task<ImportDocumentResult> Handle(ImportDocumentRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.PrjDocImport.ImportResult output = null;
            DocsPaVO.PrjDocImport.ResultsContainer resultsContainer = request.resultsContainer;
            try
            {
                var importReq = new Pi3.App.Legacy.WebApi.Application.Requests.ImportDoc(request.documentToImport, request.protoType, request.serverPath,
                    request.userInfo, request.role, request.isProfilationRequired, request.isRapidClassificationRequired, request.isSmistamentoEnabled,
                    request.resultsContainer, request.isEnabledPregressi, false
                );

                (output, resultsContainer) = await this._mediator.Send(importReq);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = new DocsPaVO.PrjDocImport.ImportResult()
                {
                    Message = ex.Message,
                    Outcome = DocsPaVO.PrjDocImport.ImportResult.OutcomeEnumeration.KO
                };
            }
            return new(output, resultsContainer);
        }




        protected readonly ILogger<ImportDocumentHandler> _logger;
        protected readonly IMediator _mediator;

    }
}
