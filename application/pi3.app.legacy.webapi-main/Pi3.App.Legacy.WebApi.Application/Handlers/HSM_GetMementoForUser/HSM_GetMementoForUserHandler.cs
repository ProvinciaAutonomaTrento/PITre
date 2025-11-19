// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSM_GetMementoForUserRequest = Pi3.App.Legacy.WebApi.Application.Requests.HSM_GetMementoForUser;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.HSM_GetMementoForUser
{
    public class HSM_GetMementoForUserHandler : IRequestHandler<HSM_GetMementoForUserRequest, HSM_GetMementoForUserResult>
    {
        #region Public Members

        public HSM_GetMementoForUserHandler(ILogger<HSM_GetMementoForUserHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<HSM_GetMementoForUserResult> Handle(HSM_GetMementoForUserRequest request, CancellationToken cancellationToken)
        {
            string[] output = null;

            try
            {
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var idPeople = delegatedIdUser != 0 ? delegatedIdUser : idUser;

                var memento = await this._dbContext.PeopleEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == idPeople && p.ID_AMM == idTenant && p.DISABLED != "Y")
                    .Select(p => p.VAR_MEMENTO)
                    .FirstAsync();

                output = memento != null && memento.Contains("§") ? memento.Split('§') : new string[] { memento };
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new HSM_GetMementoForUserResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<HSM_GetMementoForUserHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
