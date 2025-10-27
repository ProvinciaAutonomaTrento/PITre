// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckScaricoOtherRoleRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckScaricoOtherRole;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckScaricoOtherRole
{
    public class CheckScaricoOtherRoleHandler : IRequestHandler<CheckScaricoOtherRoleRequest, CheckScaricoOtherRoleResult>
    {
        #region Public Members

        public CheckScaricoOtherRoleHandler(ILogger<CheckScaricoOtherRoleHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<CheckScaricoOtherRoleResult> Handle(CheckScaricoOtherRoleRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var idRegistroAsLong = request.idReg.AsLong();
            try
            {
                var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);

                var idCorrGlobali = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.ID_PEOPLE == idUser)
                    .Select(c => c.SYSTEM_ID)
                    .FirstAsync();

                output = await this._dbContext.CheckMailboxEntities
                    .AsNoTracking()
                    .AnyAsync(c => c.CONCLUDED == "0" && c.IDREG == idRegistroAsLong && c.MAIL == request.email && c.IDUSER != idCorrGlobali);

            }
            catch(Pi3Exception pi3ex)
            {
                this._logger.LogError(pi3ex, null, null);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, null, null);
            }

            return new CheckScaricoOtherRoleResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CheckScaricoOtherRoleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
