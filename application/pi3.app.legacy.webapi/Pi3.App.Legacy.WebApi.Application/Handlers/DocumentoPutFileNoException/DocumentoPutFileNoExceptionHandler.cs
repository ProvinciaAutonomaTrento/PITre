// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.App.Legacy.WebApi.Application.Models;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoPutFileNoExceptionRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoPutFileNoException;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoPutFileNoException
{
    public class DocumentoPutFileNoExceptionHandler : IRequestHandler<DocumentoPutFileNoExceptionRequest, DocumentoPutFileNoExceptionResult>
    {
        #region Public Members

        public DocumentoPutFileNoExceptionHandler(
            ILogger<DocumentoPutFileNoExceptionHandler> logger,
            IMediator mediator)
        {
            this._logger = logger;
            this._mediator = mediator;
        }

        public async Task<DocumentoPutFileNoExceptionResult> Handle(DocumentoPutFileNoExceptionRequest request, CancellationToken cancellationToken)
        {
            Requests.DocumentoPutFileResult putFileResult = null!;
            bool output = false;
            string errorMessage = null!;

            try
            {
                putFileResult = await this._mediator.Send(new Requests.DocumentoPutFile(request.fileRequest, request.fileDocument, request.infoUtente));
                
                output = true;
            }
            catch (Exception ex)
            {
                output = false;
                errorMessage = ex.Message;
            }

            return new DocumentoPutFileNoExceptionResult(output, request.fileRequest, errorMessage);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoPutFileNoExceptionHandler> _logger;
        protected readonly IMediator _mediator;

        #endregion
    }
}
