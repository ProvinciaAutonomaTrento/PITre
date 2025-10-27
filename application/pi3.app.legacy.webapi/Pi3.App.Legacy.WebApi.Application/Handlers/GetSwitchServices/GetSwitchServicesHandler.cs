// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Models;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetSwitchServices
{
    public class GetSwitchServicesHandler : IRequestHandler<Application.Requests.GetSwitchServices, GetSwitchServicesResult>
    {
        #region Public Members

        public GetSwitchServicesHandler(ILogger<GetSwitchServicesHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetSwitchServicesResult> Handle(Application.Requests.GetSwitchServices request, CancellationToken cancellationToken)
        {
            var model = await this._dbContext.SwitchServiceEntities.AsNoTracking()
                .Select(s => new SwitchServiceModel()
                {
                    ServiceName = s.SERVICE_NAME,
                    UseDataPortalApi = s.CHA_USE_DATA_PORTAL_API == "1",
                    Category = s.CATEGORY ?? string.Empty,
                    ResponseAsRaw = s.RESPONSE_AS_RAW == "1"
                })
                .ToListAsync();

            return new GetSwitchServicesResult(model.AsReadOnly());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSwitchServicesHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }


}
