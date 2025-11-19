// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using Albo_GetStatiSelezioneFilesRequest = Pi3.App.Legacy.WebApi.Application.Requests.Albo_GetStatiSelezioneFiles;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Albo_GetStatiSelezioneFiles
{
    public class Albo_GetStatiSelezioneFilesHandler : IRequestHandler<Albo_GetStatiSelezioneFilesRequest, Albo_GetStatiSelezioneFilesResult>
    {
        #region Public Members

        public Albo_GetStatiSelezioneFilesHandler(ILogger<Albo_GetStatiSelezioneFilesHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<Albo_GetStatiSelezioneFilesResult> Handle(Albo_GetStatiSelezioneFilesRequest request, CancellationToken cancellationToken)
        {
            string[] output = null;

            try
            {
                long idDiagramma = Convert.ToInt64(request.idDiagramma);
                var entity = await this._dbContext.StatoEntities
                    .Where(s => s.ID_DIAGRAMMA == idDiagramma && s.CHA_PUBB_SELECT_FILES == "1")
                    .Select(s => s.SYSTEM_ID )
                    .ToListAsync();

                if(entity != null && entity.Count > 0)
                {
                    var id = new List<string>();
                    entity.ForEach(x => id.Add(x.ToString()));
                    output = id.ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new Albo_GetStatiSelezioneFilesResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<Albo_GetStatiSelezioneFilesHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
