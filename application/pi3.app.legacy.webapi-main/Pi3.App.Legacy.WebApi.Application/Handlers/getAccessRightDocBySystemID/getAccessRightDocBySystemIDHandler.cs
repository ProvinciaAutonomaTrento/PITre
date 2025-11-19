// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.getAccessRightDocBySystemID
{

    // Richiede libreria MediatR
    public class getAccessRightDocBySystemIDHandler : IRequestHandler<Application.Requests.getAccessRightDocBySystemID, getAccessRightDocBySystemIDResult>
    {
        #region Public Members

        public getAccessRightDocBySystemIDHandler(ILogger<getAccessRightDocBySystemIDHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            _distributedCache = distributedCache;
        }

        public async Task<getAccessRightDocBySystemIDResult> Handle(Application.Requests.getAccessRightDocBySystemID request, CancellationToken cancellationToken)
        {
            string result = string.Empty;

            try
            {
                string idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
                string idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();
                long idAmm = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var rights = Convert.ToInt32(await _dbContext.GetSecurityRights(request.idprofile, idPeople, idGroup));

                switch (rights)
                {
                    case 1:
                        result = "45"; //Acquisito sola lettura
                        break;
                    case 2:
                        result = "63"; //Acquisito lettura - scrittura
                        break;
                    case 3: 
                        result = "255"; //Proprietario
                        break;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getAccessRightDocBySystemIDResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getAccessRightDocBySystemIDHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        #endregion
    }

}
