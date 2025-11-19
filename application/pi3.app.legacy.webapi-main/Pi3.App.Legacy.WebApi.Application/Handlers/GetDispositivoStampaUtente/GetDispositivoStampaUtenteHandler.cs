// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetDispositivoStampaUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetDispositivoStampaUtente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDispositivoStampaUtente
{
    public class GetDispositivoStampaUtenteHandler : IRequestHandler<GetDispositivoStampaUtenteRequest, GetDispositivoStampaUtenteResult>
    {
        #region Public Members

        public GetDispositivoStampaUtenteHandler(ILogger<GetDispositivoStampaUtenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetDispositivoStampaUtenteResult> Handle(GetDispositivoStampaUtenteRequest request, CancellationToken cancellationToken)
        {
            string output = null;

            try
            {
                var idPeople = request.idPeople.AsLong();
                var dispositiviStampaEntities = await this._dbContext.DispositivoStampaEntities.AsNoTracking().ToListAsync();

                var dispositivoStampaUser = await this._dbContext.PeopleEntities
                    .Join(this._dbContext.AmministraEntities, people => people.ID_AMM, amm => amm.SYSTEM_ID, (people, amm) => new { people, amm })
                    .Where(j => j.people.SYSTEM_ID == idPeople)
                    .Select(j => new
                    {
                        DISPOSITIVO_PEOPLE = j.people.ID_DISPOSITIVO_STAMPA,
                        DIPSOSITIVO_AMMINISTRA = j.amm.ID_DISPOSITIVO_STAMPA
                    })
                    .FirstAsync();

                var idDisposito = dispositivoStampaUser.DISPOSITIVO_PEOPLE != null ? dispositivoStampaUser.DISPOSITIVO_PEOPLE : dispositivoStampaUser.DIPSOSITIVO_AMMINISTRA;

                output = dispositiviStampaEntities.Where(i => i.ID == idDisposito).Select(i => i.CODE).FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetDispositivoStampaUtenteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDispositivoStampaUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
