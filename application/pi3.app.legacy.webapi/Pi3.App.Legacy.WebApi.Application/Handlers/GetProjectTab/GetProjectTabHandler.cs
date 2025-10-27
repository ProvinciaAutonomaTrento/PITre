// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
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
using GetProjectTabRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetProjectTab;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetProjectTab
{
    public class GetProjectTabHandler : IRequestHandler<GetProjectTabRequest, GetProjectTabResult>
    {
        #region Public Members

        public GetProjectTabHandler(ILogger<GetProjectTabHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetProjectTabResult> Handle(GetProjectTabRequest request, CancellationToken cancellationToken)
        {
            Tab output = new Tab();

            try
            {
                var idProjectAsLong = request.projectId.AsLong();

                output.TransmissionsNumber = (await this._dbContext.TrasmissioneEntities.AsNoTracking()
                    .CountAsync(t => t.ID_PROJECT == idProjectAsLong))
                    .ToString();

                output.DeletedSecurity = await this._dbContext.DeletedSecurityEntities.AsNoTracking()
                    .AnyAsync(d => d.THING == idProjectAsLong && !this._dbContext.SecurityEntities.AsNoTracking().Any(s => s.THING == d.THING && s.ACCESSRIGHTS > 20 && s.PERSONORGROUP == d.PERSONORGROUP));
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetProjectTabResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetProjectTabHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
