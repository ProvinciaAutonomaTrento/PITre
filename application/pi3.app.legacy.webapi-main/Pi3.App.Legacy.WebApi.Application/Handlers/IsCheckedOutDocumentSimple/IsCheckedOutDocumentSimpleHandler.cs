// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.Import.Pregressi;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsCheckedOutDocumentSimpleRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsCheckedOutDocumentSimple;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsCheckedOutDocumentSimple
{
    public class IsCheckedOutDocumentSimpleHandler : IRequestHandler<IsCheckedOutDocumentSimpleRequest, IsCheckedOutDocumentSimpleResult>
    {
        #region Public Members

        public IsCheckedOutDocumentSimpleHandler(ILogger<IsCheckedOutDocumentSimpleHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;    
        }

        public async Task<IsCheckedOutDocumentSimpleResult> Handle(IsCheckedOutDocumentSimpleRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            SchedaDocumento schedaDocumento = null;
            var idDocumentAsLong = request.idDocument.AsLong();

            if (request.doc == null || string.IsNullOrEmpty(request.doc.systemId) || !request.doc.systemId.Equals(request.idDocument))
            {
                var documentNumberAsLong = request.documentNumber.AsLong();

                output = await this._dbContext.CheckinCheckoutEntities.AsNoTracking()
                    .AnyAsync(p => p.ID_DOCUMENT == idDocumentAsLong && p.DOCUMENT_NUMBER == documentNumberAsLong);
            }
            else
            {
                schedaDocumento = request.doc;
            }

            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
                output = (!string.IsNullOrEmpty(schedaDocumento.checkOutStatus.ID));

            if(!output && request.checkAllegati)
            {
                output = await this._dbContext.CheckinCheckoutEntities.AsNoTracking()
                           .Join(_dbContext.ProfileEntities.AsNoTracking(),
                                 c => c.ID_DOCUMENT,
                                 p => p.DOCNUMBER,
                                 (c, p) => new { c, p })
                           .AnyAsync(j => j.p.ID_DOCUMENTO_PRINCIPALE == idDocumentAsLong &&
                                       j.c.DOCUMENT_NUMBER == j.p.DOCNUMBER);
            }
               
            return new IsCheckedOutDocumentSimpleResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsCheckedOutDocumentSimpleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
