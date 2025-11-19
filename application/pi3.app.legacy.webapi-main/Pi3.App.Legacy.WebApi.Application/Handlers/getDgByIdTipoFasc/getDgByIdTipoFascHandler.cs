// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using getDgByIdTipoFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDgByIdTipoFasc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDgByIdTipoFasc
{
    public class getDgByIdTipoFascHandler : IRequestHandler<getDgByIdTipoFascRequest, getDgByIdTipoFascResult>
    {
        #region Public Members

        public getDgByIdTipoFascHandler(ILogger<getDgByIdTipoFascHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<getDgByIdTipoFascResult> Handle(getDgByIdTipoFascRequest request, CancellationToken cancellationToken)
        {
            DiagrammaStato output = null;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            try
            {
                if(!string.IsNullOrEmpty(request.systemIdTipoFasc))
                {
                    var idTipoFasc = request.systemIdTipoFasc.AsLong();

                    var idDiagramma = await this._dbContext.TipoFascEntities.AsNoTracking()
                        .Join(this._dbContext.AssDiagrammiEntities.AsNoTracking(), f => f.SYSTEM_ID, d => d.ID_TIPO_FASC, (f, d) => new { f, d })
                        .Where(j => j.f.SYSTEM_ID == idTipoFasc && j.f.ID_AMM == idTenant && j.d.ID_DIAGRAMMA != null)
                        .Select(j => j.d.ID_DIAGRAMMA)
                        .FirstOrDefaultAsync();

                    if (idDiagramma != null)
                    {
                        var result = await this._mediator.Send(new getDiagrammaById(idDiagramma.ToString()));
                        output = result.output;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getDgByIdTipoFascResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDgByIdTipoFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
