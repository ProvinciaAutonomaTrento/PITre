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
using getNewsRequest = Pi3.App.Legacy.WebApi.Application.Requests.getNews;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getNews
{
    public class getNewsHandler : IRequestHandler<getNewsRequest, getNewsResult>
    {
        #region Public Members

        public getNewsHandler(ILogger<getNewsHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getNewsResult> Handle(getNewsRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;

            try
            {
                var idAmmAsLong = request.idAmm.AsLong();

                output = await this._dbContext.AmministraEntities.Where(a => a.SYSTEM_ID == idAmmAsLong).Select(a => a.ENABLE_NEWS + "^" + a.NEWS).FirstAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new getNewsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getNewsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
