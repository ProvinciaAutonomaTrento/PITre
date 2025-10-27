// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DelegaAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegaDismettiRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaDismetti;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaDismetti
{
    public class DelegaDismettiHandler : IRequestHandler<DelegaDismettiRequest, DelegaDismettiResult>
    {
        #region Public Members

        public DelegaDismettiHandler(ILogger<DelegaDismettiHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDelegaRepository delegaRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._delegaRepository = delegaRepository;
        }

        public async Task<DelegaDismettiResult> Handle(DelegaDismettiRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser, true);

                var idDelega = await this._dbContext.DelegheEntities
                    .Where(d => d.ID_PEOPLE_DELEGANTE == idPeople && d.ID_PEOPLE_DELEGATO == delegatedIdUser && d.CHA_IN_ESERCIZIO == "1")
                    .Select(d => d.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                if (idDelega != null)
                {
                    var aggregate = await this._delegaRepository.Get(idTenant, idDelega.ToString());
                    aggregate.Dismetti();

                    await this._delegaRepository.Update(aggregate);
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new DelegaDismettiResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DelegaDismettiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDelegaRepository _delegaRepository;

        #endregion
    }
}
