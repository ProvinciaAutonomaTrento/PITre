// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Modelli_Trasmissioni;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isStatoTrasmAutoFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.isStatoTrasmAutoFasc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isStatoTrasmAutoFasc
{
    public class isStatoTrasmAutoFascHandler : IRequestHandler<isStatoTrasmAutoFascRequest, isStatoTrasmAutoFascResult>
    {
        #region Public Members

        public isStatoTrasmAutoFascHandler(ILogger<isStatoTrasmAutoFascHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<isStatoTrasmAutoFascResult> Handle(isStatoTrasmAutoFascRequest request, CancellationToken cancellationToken)
        {
            ModelloTrasmissione[] output = null;

            try
            {
                if (!string.IsNullOrEmpty(request.idStato))
                {
                    long idStato = request.idStato.AsLong();
                    long idTipoFasc = request.idTipoFasc.AsLong();
                    long idAmm = request.idAmm.AsLong();

                    var assDiagrammiEntities = await this._dbContext.AssDiagrammiEntities.Where(a => a.ID_STATO == idStato && a.ID_TIPO_FASC == idTipoFasc).Select(a => new { a.ID_MOD_TRASM, a.TRASM_AUT }).ToListAsync();
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
                this._logger.LogWebMethodError(ex);
            }

            return new isStatoTrasmAutoFascResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isStatoTrasmAutoFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
