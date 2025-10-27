// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNumberNotifyOtherRolesRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetNumberNotifyOtherRoles;
using Microsoft.EntityFrameworkCore;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetNumberNotifyOtherRoles
{
    public class GetNumberNotifyOtherRolesHandler : IRequestHandler<GetNumberNotifyOtherRolesRequest, GetNumberNotifyOtherRolesResult>
    {
        #region Public Members

        public GetNumberNotifyOtherRolesHandler(ILogger<GetNumberNotifyOtherRolesHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetNumberNotifyOtherRolesResult> Handle(GetNumberNotifyOtherRolesRequest request, CancellationToken cancellationToken)
        {
            int output = 0;

            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                output = await this._dbContext.NotifyEntities.AsNoTracking()
                    .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(), n => n.ID_GROUP_RECEIVER, p => p.GROUPS_SYSTEM_ID, (n, p) => new { n, p })
                    .Where(j => j.p.PEOPLE_SYSTEM_ID == idPeople && j.p.DTA_FINE == null && j.p.GROUPS_SYSTEM_ID != idGruppo && j.n.ID_PEOPLE_RECEIVER == j.p.PEOPLE_SYSTEM_ID)
                    .Select(j => j.n.SYSTEM_ID)
                    .Concat(this._dbContext.NotifyEntities.AsNoTracking()
                    .Where(n => n.ID_PEOPLE_RECEIVER == idPeople && n.ID_GROUP_RECEIVER == 0)
                    .Select(n => n.SYSTEM_ID))
                    .CountAsync();
                    
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetNumberNotifyOtherRolesResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetNumberNotifyOtherRolesHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
