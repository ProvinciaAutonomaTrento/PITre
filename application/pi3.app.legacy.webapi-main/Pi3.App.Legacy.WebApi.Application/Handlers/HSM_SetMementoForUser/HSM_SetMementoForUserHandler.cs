// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Mobile;
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
using HSM_SetMementoForUserRequest = Pi3.App.Legacy.WebApi.Application.Requests.HSM_SetMementoForUser;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.HSM_SetMementoForUser
{
    public class HSM_SetMementoForUserHandler : IRequestHandler<HSM_SetMementoForUserRequest, HSM_SetMementoForUserResult>
    {
        #region Public Members

        public HSM_SetMementoForUserHandler(ILogger<HSM_SetMementoForUserHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<HSM_SetMementoForUserResult> Handle(HSM_SetMementoForUserRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                //tolgo i separatori
                var dominio = request.dominio.Replace("§", "");
                var alias = request.alias.Replace("§", "");
                var memento = String.Format("{0}§{1}", dominio, alias);
                var idPeople = delegatedIdUser != 0 ? delegatedIdUser : idUser;

                var peopleEntity = await this._dbContext.PeopleEntities
                    .Where(p => p.SYSTEM_ID == idPeople && p.ID_AMM == idTenant && p.DISABLED != "Y")
                    .FirstAsync();

                peopleEntity.VAR_MEMENTO = memento;

                await ((DbContext)_dbContext).SaveChangesAsync();
                output = true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new HSM_SetMementoForUserResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<HSM_SetMementoForUserHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
