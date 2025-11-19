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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRuoloOrUserLista
{
    public class GetRuoloOrUserListaHandler : IRequestHandler<getRuoloOrUserLista, getRuoloOrUserListaResult>
    {
        #region Public members
        public GetRuoloOrUserListaHandler(ILogger<GetRuoloOrUserListaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getRuoloOrUserListaResult> Handle(getRuoloOrUserLista request, CancellationToken cancellationToken)
        {
            string output = null;

            try
            {
                var entity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.SYSTEM_ID == request.idLista.AsLong() && x.CHA_TIPO_URP == "L");

                if (entity is not null && entity.ID_GRUPPO_LISTE.HasValue) output = entity.ID_GRUPPO_LISTE.ToString();

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getRuoloOrUserListaResult(output);

        }
        #endregion

        #region Private members

        protected readonly ILogger<GetRuoloOrUserListaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
