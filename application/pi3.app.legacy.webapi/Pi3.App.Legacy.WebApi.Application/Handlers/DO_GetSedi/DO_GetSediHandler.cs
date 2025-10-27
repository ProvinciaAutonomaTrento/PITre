// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO_GetSediRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_GetSedi;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_GetSedi
{
    public class DO_GetSediHandler : IRequestHandler<DO_GetSediRequest, DO_GetSediResult>
    {
        protected readonly ILogger<DO_GetSediHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        public DO_GetSediHandler(
            ILogger<DO_GetSediHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<DO_GetSediResult> Handle(DO_GetSediRequest request,CancellationToken cancellationToken)
        {
            ArrayList output = new();
            try
            {
                var sedi = await this._dbContext.ProfileEntities.AsNoTracking().Where( p => string.IsNullOrEmpty(p.VAR_SEDE) && !p.VAR_SEDE.Equals(" ")).Select(p => p.VAR_SEDE).Distinct().ToListAsync();
                sedi.ForEach((sede) =>
                {
                    output.Add(sede);
                });
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
