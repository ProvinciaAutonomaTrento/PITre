// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetSignTypePreferenceRequest = Pi3.App.Legacy.WebApi.Application.Requests.SetSignTypePreference;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SetSignTypePreference
{
    public class SetSignTypePreferenceHandler : IRequestHandler<SetSignTypePreferenceRequest, SetSignTypePreferenceResult>
    {
        #region Public Members

        public SetSignTypePreferenceHandler(ILogger<SetSignTypePreferenceHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<SetSignTypePreferenceResult> Handle(SetSignTypePreferenceRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                var idPeople = request.idPeople.AsLong();
                var peopleEntity = await this._dbContext.PeopleEntities.FirstOrDefaultAsync(p => p.SYSTEM_ID == idPeople);

                if(peopleEntity != null)
                {
                    peopleEntity.CHA_TIPO_FIRMA = request.chaPreference;
                    await ((DbContext)_dbContext).SaveChangesAsync();
                    output = true;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new SetSignTypePreferenceResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SetSignTypePreferenceHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
