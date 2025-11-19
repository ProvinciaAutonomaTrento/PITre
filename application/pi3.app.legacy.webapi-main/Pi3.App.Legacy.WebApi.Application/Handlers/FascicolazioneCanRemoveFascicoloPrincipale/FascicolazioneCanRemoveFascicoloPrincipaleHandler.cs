// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.getTimestampsDoc;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneCanRemoveFascicoloPrincipaleRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneCanRemoveFascicoloPrincipale;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneCanRemoveFascicoloPrincipale
{
    public class FascicolazioneCanRemoveFascicoloPrincipaleHandler : IRequestHandler<FascicolazioneCanRemoveFascicoloPrincipaleRequest, FascicolazioneCanRemoveFascicoloPrincipaleResult>
    {
        #region Public Members

        public FascicolazioneCanRemoveFascicoloPrincipaleHandler(ILogger<FascicolazioneCanRemoveFascicoloPrincipaleHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<FascicolazioneCanRemoveFascicoloPrincipaleResult> Handle(FascicolazioneCanRemoveFascicoloPrincipaleRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            var nDoc = string.Empty;

            try
            {
                var projectIdAsLong = request.project_Id.AsLong();

                var countDoc = await this._dbContext.ProjectComponentEntities.AsNoTracking()
                    .Join(this._dbContext.ProjectEntities.AsNoTracking(), pg => pg.PROJECT_ID, proj => proj.SYSTEM_ID, (pg, proj) => new { pg, proj })
                    .CountAsync(j => j.proj.ID_FASCICOLO == projectIdAsLong);

                output = countDoc == 0;
                if(countDoc != 0)
                    nDoc = countDoc.ToString();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new FascicolazioneCanRemoveFascicoloPrincipaleResult(output, nDoc);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneCanRemoveFascicoloPrincipaleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
