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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CancellaRicerca
{

    // Richiede libreria MediatR
    public class CancellaRicercaHandler : IRequestHandler<Application.Requests.CancellaRicerca, CancellaRicercaResult>
    {
        #region Public Members

        public CancellaRicercaHandler(ILogger<CancellaRicercaHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }



        public async Task<CancellaRicercaResult> Handle(Application.Requests.CancellaRicerca request, CancellationToken cancellationToken)
        {
            var ricerca = await _dbContext.SalvaRicercaEntities.Where(w => w.SYSTEM_ID == request.id).Select(s => new SalvaRicercaEntity()
            {
                SYSTEM_ID= request.id
            }).FirstOrDefaultAsync();

            _dbContext.SalvaRicercaEntities.Remove(ricerca);
            await ((DbContext)_dbContext).SaveChangesAsync();
            return new CancellaRicercaResult(true);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CancellaRicercaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
