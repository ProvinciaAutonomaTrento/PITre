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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneMoveFolder
{

    // Richiede libreria MediatR
    public class FascicolazioneMoveFolderHandler : IRequestHandler<Application.Requests.FascicolazioneMoveFolder, FascicolazioneMoveFolderResult>
    {
        #region Public Members

        public FascicolazioneMoveFolderHandler(ILogger<FascicolazioneMoveFolderHandler> logger, IPi3DbContext dbContext, IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public async Task<FascicolazioneMoveFolderResult> Handle(Application.Requests.FascicolazioneMoveFolder request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _dbContext.ProjectEntities.Where(w => w.SYSTEM_ID == request.folderId.AsLong()).FirstOrDefaultAsync();
                entity.ID_PARENT = request.parentId.AsLong();

                ((DbContext)_dbContext).Update(entity);
                ((DbContext)_dbContext).SaveChanges();

                return new FascicolazioneMoveFolderResult(true);
            }
            catch
            {

                return new FascicolazioneMoveFolderResult(false);
            }
            



        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneMoveFolderHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;


        #endregion
    }

}
