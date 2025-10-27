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
using DocumentoCheckUserVisibilityRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoCheckUserVisibility;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoCheckUserVisibility
{
    public class DocumentoCheckUserVisibilityHandler : IRequestHandler<DocumentoCheckUserVisibilityRequest, DocumentoCheckUserVisibilityResult>
    {
        #region Public Members

        public DocumentoCheckUserVisibilityHandler(ILogger<DocumentoCheckUserVisibilityHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoCheckUserVisibilityResult> Handle(DocumentoCheckUserVisibilityRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var securityRights = await this._dbContext.GetSecurityRights(request.docNumber, idPeople, idGruppo);

                output = securityRights != SecurityRightTypesEnum.Deny;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new DocumentoCheckUserVisibilityResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoCheckUserVisibilityHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
