// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetTipologieIstanzeConservazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetTipologieIstanzeConservazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTipologieIstanzeConservazione
{
    public class GetTipologieIstanzeConservazioneHandler : IRequestHandler<GetTipologieIstanzeConservazioneRequest, GetTipologieIstanzeConservazioneResult>
    {
        #region Public Members

        public GetTipologieIstanzeConservazioneHandler(ILogger<GetTipologieIstanzeConservazioneHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetTipologieIstanzeConservazioneResult> Handle(GetTipologieIstanzeConservazioneRequest request, CancellationToken cancellationToken)
        {
            TipoIstanzaConservazione[] output = new TipoIstanzaConservazione[2]
            {
                new TipoIstanzaConservazione() {Codice = "CONSERVAZIONE_CONSOLIDATA", Descrizione = "Conservazione consolidata"},
                new TipoIstanzaConservazione { Codice = "CONSERVAZIONE_NON_CONSOLIDATA", Descrizione = "Conservazione non consolidata" }
            };

            return new GetTipologieIstanzeConservazioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTipologieIstanzeConservazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
