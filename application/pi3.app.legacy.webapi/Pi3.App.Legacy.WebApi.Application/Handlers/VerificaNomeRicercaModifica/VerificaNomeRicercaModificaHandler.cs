// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Mobile.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.verificaNomeRicerca;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using verificaNomeRicercaModificaRequest = Pi3.App.Legacy.WebApi.Application.Requests.verificaNomeRicercaModifica;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.verificaNomeRicercaModifica
{
    public class VerificaNomeRicercaModificaHandler : IRequestHandler<verificaNomeRicercaModificaRequest, verificaNomeRicercaModificaResult>
    {

        #region Public Members
        public VerificaNomeRicercaModificaHandler(ILogger<verificaNomeRicercaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }


        public async Task<verificaNomeRicercaModificaResult> Handle(verificaNomeRicercaModificaRequest request, CancellationToken cancellationToken)
        {
            var result = false;

            try
            {
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var descrizione = request.nome.ToUpper();
                var idRicerca = request.idRicerca.AsLong();

                result = await this._dbContext.SalvaRicercaEntities
                    .AsNoTracking()
                    .AnyAsync(s => s.VAR_DESCRIZIONE.ToUpper() == descrizione && (s.ID_PEOPLE == idUser || s.ID_GRUPPO == idGroup) && s.VAR_PAGINA_RIC == request.pagina && s.SYSTEM_ID != idRicerca);
            }      
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new verificaNomeRicercaModificaResult(result);
        }
        #endregion

        #region Private Members

        protected readonly ILogger<verificaNomeRicercaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
