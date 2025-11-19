// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Modelli_Trasmissioni;
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
using isStatoTrasmAutoRequest = Pi3.App.Legacy.WebApi.Application.Requests.isStatoTrasmAuto;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isStatoTrasmAuto
{
    public class isStatoTrasmAutoHandler : IRequestHandler<isStatoTrasmAutoRequest, isStatoTrasmAutoResult>
    {
        #region Public Members

        public isStatoTrasmAutoHandler(ILogger<isStatoTrasmAutoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<isStatoTrasmAutoResult> Handle(isStatoTrasmAutoRequest request, CancellationToken cancellationToken)
        {
            ModelloTrasmissione[] output = null;

            try
            {
                if (!string.IsNullOrEmpty(request.idStato))
                {
                    long idStato = request.idStato.AsLong();
                    long idTemplate = request.idTemplate.AsLong();
                    long idAmm = request.idAmm.AsLong();

                    var assDiagrammiEntities = await this._dbContext.AssDiagrammiEntities.Where(a => a.ID_STATO == idStato && a.ID_TIPO_DOC == idTemplate).Select(a => new { a.ID_MOD_TRASM, a.TRASM_AUT }).ToListAsync();
                    if (assDiagrammiEntities != null && assDiagrammiEntities.Count > 0)
                    {
                        List<ModelloTrasmissione> modelli = new List<ModelloTrasmissione>();
                        foreach (var a in assDiagrammiEntities)
                        {
                            if (a.TRASM_AUT == 1)
                            {
                                var modello = await this._mediator.Send(new Application.Requests.getModelloByID(request.idAmm, a.ID_MOD_TRASM.ToString()));
                                modelli.Add(modello.output);

                            }
                        }
                        output = modelli.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new isStatoTrasmAutoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isStatoTrasmAutoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
