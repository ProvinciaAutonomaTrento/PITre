// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.PrjDocImport;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using ImportAndAcquireDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.ImportAndAcquireDocument;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportAndAcquireDocument
{
    public class ImportAndAcquireDocumentHandler : IRequestHandler<ImportAndAcquireDocumentRequest, ImportAndAcquireDocumentResult>
    {
        #region Public Members

        public ImportAndAcquireDocumentHandler(
            ILogger<ImportAndAcquireDocumentHandler> logger,
            IMediator mediator
            )
        {
            this._logger = logger;
            this._mediator = mediator;
        }

    public async Task<ImportAndAcquireDocumentResult> Handle(ImportAndAcquireDocumentRequest request, CancellationToken cancellationToken)
    {
        DocsPaVO.PrjDocImport.ImportResult output = null;
        DocsPaVO.PrjDocImport.ResultsContainer resultsContainer = request.resultsContainer;
        try
        {
                var importReq = new Pi3.App.Legacy.WebApi.Application.Requests.ImportDoc(request.documentToImport,request.protoType,request.serverPath,
                    request.userInfo,request.role,request.isProfilationRequired,request.isRapidClassificationRequired,request.isSmistamentoEnabled,
                    request.resultsContainer,request.isEnabledPregressi,true
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

    #endregion


    protected readonly ILogger<ImportAndAcquireDocumentHandler> _logger;
    protected readonly IMediator _mediator;
    }
}