// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRootFolderFasc
{
    public class GetRootFolderFascHandler : IRequestHandler<getRootFolderFasc, getRootFolderFascResult>
    {
        private readonly ILogger<GetRootFolderFascHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        public GetRootFolderFascHandler(
            ILogger<GetRootFolderFascHandler> logger,
            IPi3DbContext dbContext, IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        public async Task<getRootFolderFascResult> Handle(getRootFolderFasc request, CancellationToken cancellationToken)
        {
            long idFascicolo = request.IdFasc.AsLong();
            string? resultValue;
            try
            {
                long idProject = await this._dbContext
                                            .ProjectEntities
                                            .AsNoTracking()
                                            .Where(p => p.ID_FASCICOLO == idFascicolo)
                                            .Select(p => p.SYSTEM_ID)
                                            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                if(idProject == 0)
                {
                    throw new Exceptions.ProjectByIdFascicoloNotFoundPi3Exception(idFascicolo);
                }
                resultValue = idProject.ToString();
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                resultValue = null;
            }

            return new getRootFolderFascResult(resultValue);
        }
    }
}
