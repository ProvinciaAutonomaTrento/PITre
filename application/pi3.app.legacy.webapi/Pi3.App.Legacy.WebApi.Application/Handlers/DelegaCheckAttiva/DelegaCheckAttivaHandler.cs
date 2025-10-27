// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using DelegaCheckAttivaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaCheckAttiva;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaCheckAttiva
{
    public class DelegaCheckAttivaHandler : IRequestHandler<DelegaCheckAttivaRequest, DelegaCheckAttivaResult>
    {
        #region Public Members

        public DelegaCheckAttivaHandler(ILogger<DelegaCheckAttivaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DelegaCheckAttivaResult> Handle(DelegaCheckAttivaRequest request, CancellationToken cancellationToken)
        {
            int output = 0;

            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var systemDate = await this._dbContext.GetSystemDateTime();

                output = await this._dbContext.DelegheEntities.AsNoTracking()
                    .CountAsync(d => d.ID_PEOPLE_DELEGATO == idPeople && d.DATA_DECORRENZA <= systemDate && d.DATA_SCADENZA >= systemDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new DelegaCheckAttivaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DelegaCheckAttivaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}
