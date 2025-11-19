// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCodiceLista
{

    // Richiede libreria MediatR
    public class GetCodiceListaHandler : IRequestHandler<Pi3.App.Legacy.WebApi.Application.Requests.getCodiceLista, getCodiceListaResult>
    {
        #region Public Members

        public GetCodiceListaHandler(ILogger<GetCodiceListaHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,IPi3DbContext dbContext,IListaDistribuzioneRepository listaDistribuzioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._listaDistribuzioneRepository = listaDistribuzioneRepository;
        }


        public async Task<getCodiceListaResult> Handle(getCodiceLista request, CancellationToken cancellationToken)
        {

            string result = string.Empty;
            try
            {
                result = (await _listaDistribuzioneRepository.Get(_claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true), request.idLista)).Name.ToString();
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new getCodiceListaResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCodiceListaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IListaDistribuzioneRepository _listaDistribuzioneRepository;


        #endregion
    }

}
