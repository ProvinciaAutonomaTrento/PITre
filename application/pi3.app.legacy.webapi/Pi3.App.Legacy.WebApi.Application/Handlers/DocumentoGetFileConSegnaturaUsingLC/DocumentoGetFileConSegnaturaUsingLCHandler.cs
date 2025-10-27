// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetFileConSegnaturaUsingLCRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileConSegnaturaUsingLC;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetFileConSegnaturaUsingLC
{
    public class DocumentoGetFileConSegnaturaUsingLCHandler : IRequestHandler<DocumentoGetFileConSegnaturaUsingLCRequest, DocumentoGetFileConSegnaturaUsingLCResult>
    {
        #region Public Members

        public DocumentoGetFileConSegnaturaUsingLCHandler(ILogger<DocumentoGetFileConSegnaturaUsingLCHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<DocumentoGetFileConSegnaturaUsingLCResult> Handle(DocumentoGetFileConSegnaturaUsingLCRequest request, CancellationToken cancellationToken)
        {
            var fileDocumento = (await this._mediator.Send(new Requests.DocumentoGetFileConSegnatura(request.fileRequest, 
                                    request.sch, request.infoUtente, 
                                    request.position, false, true)))
                                .output;

            return new DocumentoGetFileConSegnaturaUsingLCResult(fileDocumento);    
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetFileConSegnaturaUsingLCHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}