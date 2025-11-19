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
using getListePerRuoloUtRequest = Pi3.App.Legacy.WebApi.Application.Requests.getListePerRuoloUt;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getListePerRuoloUt
{
    public class getListePerRuoloUtHandler : IRequestHandler<getListePerRuoloUtRequest, getListePerRuoloUtResult>
    {
        #region Public Members

        public getListePerRuoloUtHandler(ILogger<getListePerRuoloUtHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getListePerRuoloUtResult> Handle(getListePerRuoloUtRequest request, CancellationToken cancellationToken)
        {
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities
                .AsNoTracking()
                .Where(c => c.CHA_TIPO_URP == "L" && (c.ID_GRUPPO_LISTE == idGruppo || c.ID_PEOPLE_LISTE == idPeople))
                .OrderBy(c => c.VAR_DESC_CORR)
                .ToListAsync();

            return new getListePerRuoloUtResult(corrGlobaliEntity.AsDataSet());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getListePerRuoloUtHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
