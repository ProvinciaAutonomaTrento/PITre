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
using IsTitolareElementoInLFRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsTitolareElementoInLF;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsTitolareElementoInLF
{

    public class IsTitolareElementoInLFHandler : IRequestHandler<IsTitolareElementoInLFRequest, IsTitolareElementoInLFResult>
    {
        #region Public Members

        public IsTitolareElementoInLFHandler(ILogger<IsTitolareElementoInLFHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsTitolareElementoInLFResult> Handle(IsTitolareElementoInLFRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long idDocumento = Convert.ToInt64(request.docNumber);
                long idUtenteCoinvolto = Convert.ToInt64(this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true));
                long idRuoloCoinvolto = Convert.ToInt64(this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true));

                output = await this._dbContext.ElementoInLibroFirmaEntities
                    .AnyAsync(e => e.DOC_NUMBER == idDocumento && e.DTA_ESECUZIONE == null
                    && e.ID_RUOLO_TITOLARE == idRuoloCoinvolto
                    && ((e.ID_UTENTE_LOCKER ?? 0) == 0 || e.ID_UTENTE_LOCKER == idUtenteCoinvolto)
                    && ((e.ID_UTENTE_TITOLARE ?? 0) == 0 || e.ID_UTENTE_TITOLARE == idUtenteCoinvolto));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new IsTitolareElementoInLFResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsTitolareElementoInLFHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
