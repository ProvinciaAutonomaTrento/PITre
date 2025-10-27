// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isUserDisabledRequest = Pi3.App.Legacy.WebApi.Application.Requests.isUserDisabled;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isUserDisabled
{
    public class isUserDisabledHandler : IRequestHandler<isUserDisabledRequest, isUserDisabledResult>
    {
        #region Public Members

        public isUserDisabledHandler(ILogger<isUserDisabledHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<isUserDisabledResult> Handle(isUserDisabledRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                if (!string.IsNullOrEmpty(request.idAmm))
                {
                    long idAmm = Convert.ToInt64(request.idAmm);
                    output = await _dbContext.PeopleEntities
                        .Where(p => p.USER_ID.ToUpper().Equals(request.username.ToUpper()) && p.DISABLED.Equals("Y") && p.ID_AMM == idAmm).AnyAsync();
                }
                else
                {
                    output = await _dbContext.PeopleEntities
                        .Where(p => p.USER_ID.ToUpper().Equals(request.username.ToUpper()) && p.DISABLED.Equals("Y")).AnyAsync();
                }
            }
            catch (Exception ex)
            {
                //output = true;
                _logger.LogError(ex, null, null);
            }

            return new isUserDisabledResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isUserDisabledHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
