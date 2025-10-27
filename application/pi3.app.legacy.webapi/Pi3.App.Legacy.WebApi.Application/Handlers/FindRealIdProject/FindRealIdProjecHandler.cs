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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FindRealIdProject
{
    public class FindRealIdProjecHandler : IRequestHandler<Application.Requests.FindRealIdProject, FindRealIdProjectResult>
    {
        #region Public Members

        public FindRealIdProjecHandler(ILogger<FindRealIdProjecHandler> logger, 
                        IClaimsPrincipalService claimsPrincipalService, 
                        IMediator mediator,
                        IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<FindRealIdProjectResult> Handle(Application.Requests.FindRealIdProject request, CancellationToken cancellationToken)
        {
            var projectEntity = await this._dbContext.ProjectEntities.AsNoTracking()
                                .Where(p => p.SYSTEM_ID == request.idProject)
                                .Select(p => new
                                {
                                    p.SYSTEM_ID,
                                    p.CHA_TIPO_PROJ,
                                    p.ID_PARENT
                                })
                                .FirstOrDefaultAsync();

            if (projectEntity == null)
                throw new ProjectNotFoundPi3Exception(request.idProject);

            while (projectEntity.CHA_TIPO_PROJ != "C")
            {
                projectEntity = await this._dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.ID_PARENT == projectEntity.SYSTEM_ID)
                    .Select(p => new
                    {
                        p.SYSTEM_ID,
                        p.CHA_TIPO_PROJ,
                        p.ID_PARENT
                    })
                    .FirstAsync();
            }

            return new FindRealIdProjectResult(projectEntity.SYSTEM_ID);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FindRealIdProjecHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
