// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
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
using getDgByIdTipoDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDgByIdTipoDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDgByIdTipoDoc
{
    public class getDgByIdTipoDocHandler : IRequestHandler<getDgByIdTipoDocRequest, getDgByIdTipoDocResult>
    {
        #region Public Members

        public getDgByIdTipoDocHandler(ILogger<getDgByIdTipoDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<getDgByIdTipoDocResult> Handle(getDgByIdTipoDocRequest request, CancellationToken cancellationToken)
        {
            DiagrammaStato output = null;

            try
            {
                if(!string.IsNullOrEmpty(request.systemIdTipoDoc))
                {
                    long idTipoDoc = Convert.ToInt64(request.systemIdTipoDoc);
                    long idAmm = Convert.ToInt64(request.idAmm);

                    long? idDiagramma = await this._dbContext.AssDiagrammiEntities.Where(a => a.ID_TIPO_DOC == idTipoDoc).Select(a => a.ID_DIAGRAMMA).FirstOrDefaultAsync();

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

            return new getDgByIdTipoDocResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDgByIdTipoDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
