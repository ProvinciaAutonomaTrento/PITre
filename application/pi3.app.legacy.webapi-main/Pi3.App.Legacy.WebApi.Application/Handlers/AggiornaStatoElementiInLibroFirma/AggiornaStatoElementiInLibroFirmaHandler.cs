// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AggiornaStatoElementiInLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AggiornaStatoElementiInLibroFirma;
using AggiornaStatoElementoInLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AggiornaStatoElementoInLibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaStatoElementiInLibroFirma
{
    public class AggiornaStatoElementiInLibroFirmaHandler : IRequestHandler<AggiornaStatoElementiInLibroFirmaRequest, AggiornaStatoElementiInLibroFirmaResult>
    {
        #region Public Members

        public AggiornaStatoElementiInLibroFirmaHandler(ILogger<AggiornaStatoElementiInLibroFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AggiornaStatoElementiInLibroFirmaResult> Handle(AggiornaStatoElementiInLibroFirmaRequest request, CancellationToken cancellationToken)
        {
            bool output = true;
            var message = string.Empty;

            foreach(var elemento in request.elementi)
            {
                var result = await this._mediator.Send(new AggiornaStatoElementoInLibroFirmaRequest(elemento, request.nuovoStato, request.ruolo, request.infoUtente));
                message += result.message;
                if (!result.output)
                    output = false;
            }

            return new AggiornaStatoElementiInLibroFirmaResult(output, message);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AggiornaStatoElementiInLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
