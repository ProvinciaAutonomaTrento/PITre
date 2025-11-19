// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
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
using getAccessRightFascBySystemIDRequest = Pi3.App.Legacy.WebApi.Application.Requests.getAccessRightFascBySystemID;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getAccessRightFascBySystemID
{
    public class getAccessRightFascBySystemIDHandler : IRequestHandler<getAccessRightFascBySystemIDRequest, getAccessRightFascBySystemIDResult>
    {
        #region Public Members

        public getAccessRightFascBySystemIDHandler(ILogger<getAccessRightFascBySystemIDHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getAccessRightFascBySystemIDResult> Handle(getAccessRightFascBySystemIDRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var idFascicolo = request.fascSystemId;
            try
            {
                var securityEntity = await this._dbContext.GetSecurity(idFascicolo, idPeople, idGruppo);
                if(securityEntity != null)
                    output = securityEntity.ACCESSRIGHTS.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = null;
            }

            return new getAccessRightFascBySystemIDResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getAccessRightFascBySystemIDHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
