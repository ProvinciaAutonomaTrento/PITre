// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneCanRemoveFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneCanRemoveFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneCanRemoveFascicolo
{
    public class FascicolazioneCanRemoveFascicoloHandler : IRequestHandler<FascicolazioneCanRemoveFascicoloRequest, FascicolazioneCanRemoveFascicoloResult>
    {
        #region Public Members

        public FascicolazioneCanRemoveFascicoloHandler(ILogger<FascicolazioneCanRemoveFascicoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<FascicolazioneCanRemoveFascicoloResult> Handle(FascicolazioneCanRemoveFascicoloRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var nFasc = string.Empty;

            try
            {
                var projectId = request.project_Id.AsLong();

                var countSottoFascicoli = await this._dbContext.ProjectEntities.CountAsync(p => p.ID_PARENT == projectId);

                if(countSottoFascicoli != 0)
                {
                    output = false;
                    nFasc = countSottoFascicoli.ToString();
                }
                else
                {
                    var countDocInFasc = await this._dbContext.ProjectComponentEntities.CountAsync(p => p.PROJECT_ID == projectId);
                    output = countDocInFasc == 0;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new FascicolazioneCanRemoveFascicoloResult(output, nFasc);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneCanRemoveFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
