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
using getDiagrammaAssociatoFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDiagrammaAssociatoFasc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDiagrammaAssociatoFasc
{
    public class getDiagrammaAssociatoFascHandler : IRequestHandler<getDiagrammaAssociatoFascRequest, getDiagrammaAssociatoFascResult>
    {
        #region Public Members

        public getDiagrammaAssociatoFascHandler(ILogger<getDiagrammaAssociatoFascHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getDiagrammaAssociatoFascResult> Handle(getDiagrammaAssociatoFascRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idTipoFascAsLong = request.idTipoFasc.AsLong();

                var idDiagramma = await this._dbContext.AssDiagrammiEntities.AsNoTracking()
                    .Where(d => d.ID_TIPO_FASC == idTipoFascAsLong && d.ID_DIAGRAMMA != null)
                    .Select(d => d.ID_DIAGRAMMA)
                    .FirstOrDefaultAsync();

                if (idDiagramma != null)
                    output = Convert.ToInt32(idDiagramma);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = 0;
            }

            return new getDiagrammaAssociatoFascResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDiagrammaAssociatoFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
