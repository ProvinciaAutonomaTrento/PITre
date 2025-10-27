// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetSignTypePreferenceRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetSignTypePreference;
using Pi3.Core.Extensions;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetSignTypePreference
{
    public class GetSignTypePreferenceHandler : IRequestHandler<GetSignTypePreferenceRequest, GetSignTypePreferenceResult>
    {
        #region Public Members

        public GetSignTypePreferenceHandler(ILogger<GetSignTypePreferenceHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<GetSignTypePreferenceResult> Handle(GetSignTypePreferenceRequest request, CancellationToken cancellationToken)
        {
            string output = null;

            try
            {
                var idPeople = request.idPeople.AsLong();
                output = await this._dbContext.PeopleEntities.AsNoTracking().Where(p => p.SYSTEM_ID == idPeople).Select(p => p.CHA_TIPO_FIRMA).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetSignTypePreferenceResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSignTypePreferenceHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
