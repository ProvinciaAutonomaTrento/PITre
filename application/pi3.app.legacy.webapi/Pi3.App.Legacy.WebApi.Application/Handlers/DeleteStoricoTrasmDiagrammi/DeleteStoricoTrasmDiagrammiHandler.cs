// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
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
using deleteStoricoTrasmDiagrammiRequest = Pi3.App.Legacy.WebApi.Application.Requests.deleteStoricoTrasmDiagrammi;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.deleteStoricoTrasmDiagrammi
{

    // Richiede libreria MediatR
    public class deleteStoricoTrasmDiagrammiHandler : IRequestHandler<deleteStoricoTrasmDiagrammiRequest, deleteStoricoTrasmDiagrammiResult>
    {
        #region Public Members

        public deleteStoricoTrasmDiagrammiHandler(ILogger<deleteStoricoTrasmDiagrammiHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }



        public async Task<deleteStoricoTrasmDiagrammiResult> Handle(deleteStoricoTrasmDiagrammiRequest request, CancellationToken cancellationToken)
        { 
            try
            {
                var idStato = request.idStato.AsLong();
                var docnumber = request.docNumber.AsLong();

                var trasmDiagrammiEntity = _dbContext.TrasmDiagrEntities.AsNoTracking()
                    .Where(w => w.ID_STATO == idStato && w.DOC_NUMBER == docnumber)
                    .Select(s => s)
                    .FirstOrDefault();

                if (trasmDiagrammiEntity != null)
                {
                    this._dbContext.TrasmDiagrEntities.Remove(trasmDiagrammiEntity);

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }


            return new deleteStoricoTrasmDiagrammiResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<deleteStoricoTrasmDiagrammiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
