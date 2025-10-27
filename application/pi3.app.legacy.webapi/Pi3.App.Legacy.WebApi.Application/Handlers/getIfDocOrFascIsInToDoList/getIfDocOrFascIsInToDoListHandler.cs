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
using getIfDocOrFascIsInToDoListRequest = Pi3.App.Legacy.WebApi.Application.Requests.getIfDocOrFascIsInToDoList;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getIfDocOrFascIsInToDoList
{
    public class getIfDocOrFascIsInToDoListHandler : IRequestHandler<getIfDocOrFascIsInToDoListRequest, getIfDocOrFascIsInToDoListResult>
    {
        #region Public Members

        public getIfDocOrFascIsInToDoListHandler(ILogger<getIfDocOrFascIsInToDoListHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getIfDocOrFascIsInToDoListResult> Handle(getIfDocOrFascIsInToDoListRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            var idPeopleReceiver = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            try
            {
                long idTrasmissione = Convert.ToInt64(request.idTrasmissione);
                var idTrasmSingola = await this._dbContext.TrasmUtenteEntities.Where(t => t.SYSTEM_ID == idTrasmissione).Select(t => t.ID_TRASM_SINGOLA).FirstOrDefaultAsync();
                output = await this._dbContext.NotifyEntities.AnyAsync(n => n.ID_PEOPLE_RECEIVER == idPeopleReceiver && n.ID_SPECIALIZED_OBJECT == idTrasmSingola);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getIfDocOrFascIsInToDoListResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getIfDocOrFascIsInToDoListHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
