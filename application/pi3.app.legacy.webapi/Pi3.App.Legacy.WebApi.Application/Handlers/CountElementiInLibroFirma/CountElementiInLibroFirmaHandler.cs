// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using CountElementiInLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.CountElementiInLibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CountElementiInLibroFirma
{
    public class CountElementiInLibroFirmaHandler : IRequestHandler<CountElementiInLibroFirmaRequest, CountElementiInLibroFirmaResult>
    {
        #region Public Members

        public CountElementiInLibroFirmaHandler(ILogger<CountElementiInLibroFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;    
        }

        public async Task<CountElementiInLibroFirmaResult> Handle(CountElementiInLibroFirmaRequest request, CancellationToken cancellationToken)
        {
            int output = 0;
            var idPeople = request.infoUtente.idPeople.AsLong();
            var idGruppo = request.infoUtente.idGruppo.AsLong();

            try
            {
                output = await this._dbContext.ElementoInLibroFirmaEntities.AsNoTracking()
                    .CountAsync(e => e.ID_RUOLO_TITOLARE == idGruppo && (e.ID_UTENTE_TITOLARE == null || e.ID_UTENTE_TITOLARE == idPeople) &&
                    (e.ID_UTENTE_LOCKER == null || e.ID_UTENTE_LOCKER == idPeople) && e.DTA_ESECUZIONE == null && e.ID_TRASM_SINGOLA != null);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new CountElementiInLibroFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CountElementiInLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
