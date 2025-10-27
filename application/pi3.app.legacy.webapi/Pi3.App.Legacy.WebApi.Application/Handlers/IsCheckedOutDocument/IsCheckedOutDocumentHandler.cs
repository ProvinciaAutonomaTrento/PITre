// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Validations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsCheckedOutDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsCheckedOutDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsCheckedOutDocument
{
    public class IsCheckedOutDocumentHandler : IRequestHandler<IsCheckedOutDocumentRequest, IsCheckedOutDocumentResult>
    {
        #region Public Members

        public IsCheckedOutDocumentHandler(
            ILogger<IsCheckedOutDocumentHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<IsCheckedOutDocumentResult> Handle(IsCheckedOutDocumentRequest request, CancellationToken cancellationToken)
        {
            bool reserved = false;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, request.idDocument);

                reserved = (documentoAmministrativoAggregate.Reserved);

                if (!reserved && request.checkAllegati)
                {
                    reserved = (await (from p in this._pi3DbContext.ProfileEntities
                                       join c in this._pi3DbContext.CheckinCheckoutEntities
                                           on p.SYSTEM_ID equals c.ID_DOCUMENT
                                       where p.ID_DOCUMENTO_PRINCIPALE == documentoAmministrativoAggregate.Id.AsLong()
                                       select p.SYSTEM_ID)
                                .AnyAsync());
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                reserved = false;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                reserved = false;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new IsCheckedOutDocumentResult(reserved);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsCheckedOutDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        #endregion
    }
}