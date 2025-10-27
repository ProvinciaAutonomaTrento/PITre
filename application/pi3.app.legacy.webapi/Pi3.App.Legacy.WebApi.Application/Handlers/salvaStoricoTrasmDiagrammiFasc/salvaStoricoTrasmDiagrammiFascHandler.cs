// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using salvaStoricoTrasmDiagrammiFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.salvaStoricoTrasmDiagrammiFasc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.salvaStoricoTrasmDiagrammiFasc
{
    public class salvaStoricoTrasmDiagrammiFascHandler : IRequestHandler<salvaStoricoTrasmDiagrammiFascRequest, salvaStoricoTrasmDiagrammiFascResult>
    {
        #region Public Members

        public salvaStoricoTrasmDiagrammiFascHandler(ILogger<salvaStoricoTrasmDiagrammiFascHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<salvaStoricoTrasmDiagrammiFascResult> Handle(salvaStoricoTrasmDiagrammiFascRequest request, CancellationToken cancellationToken)
        {
            //var idTrasmissione = request.idTrasm.AsLong();
            
            var idTrasmissione = new long();


            if (!string.IsNullOrWhiteSpace(request.idTrasm))
            {
                idTrasmissione = request.idTrasm.AsLong();
            }
            else
            {
                string value = "0";
                idTrasmissione = value.AsLong();
            };

            var idProject = request.idProject.AsLong();
            var idStato = request.idStato.AsLong();

            if(!await this._dbContext.TrasmDiagrEntities.AsNoTracking().AnyAsync(t => t.ID_PROJECT == idProject && t.ID_STATO == idStato))
            {
                var entity = new TrasmDiagrEntity()
                {
                    ID_PROJECT = idProject,
                    ID_STATO = idStato,
                    ID_TRASM = idTrasmissione
                };

                await this._dbContext.TrasmDiagrEntities.AddAsync(entity);

                await ((DbContext)_dbContext).SaveChangesAsync();
            }

            return new salvaStoricoTrasmDiagrammiFascResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<salvaStoricoTrasmDiagrammiFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
