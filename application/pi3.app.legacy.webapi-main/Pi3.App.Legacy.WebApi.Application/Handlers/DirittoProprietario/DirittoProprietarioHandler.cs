// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirittoProprietarioRequest = Pi3.App.Legacy.WebApi.Application.Requests.DirittoProprietario;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DirittoProprietario
{
    public class DirittoProprietarioHandler : IRequestHandler<DirittoProprietarioRequest, DirittoProprietarioResult>
    {
        #region Public Members

        public DirittoProprietarioHandler(ILogger<DirittoProprietarioHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DirittoProprietarioResult> Handle(DirittoProprietarioRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var thing = Convert.ToInt64(request.idObj);
            try
            {
                output = await this._dbContext.SecurityEntities.AnyAsync(s => s.THING == thing && s.PERSONORGROUP == idPeople && s.CHA_TIPO_DIRITTO == "P");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new DirittoProprietarioResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DirittoProprietarioHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
