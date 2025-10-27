// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalvaCessioneDirittiSuModelliTrasmRequest = Pi3.App.Legacy.WebApi.Application.Requests.SalvaCessioneDirittiSuModelliTrasm;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SalvaCessioneDirittiSuModelliTrasm
{

    public class SalvaCessioneDirittiSuModelliTrasmHandler : IRequestHandler<SalvaCessioneDirittiSuModelliTrasmRequest, SalvaCessioneDirittiSuModelliTrasmResult>
    {
        #region Public Members

        public SalvaCessioneDirittiSuModelliTrasmHandler(ILogger<SalvaCessioneDirittiSuModelliTrasmHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IModelloTrasmissioneRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._repository = repository;
        }

        public async Task<SalvaCessioneDirittiSuModelliTrasmResult> Handle(SalvaCessioneDirittiSuModelliTrasmRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var aggregate = await this._repository.Get(idTenant, request.objTrasm.SYSTEM_ID.ToString());

                aggregate.CediDiritti(new CessioneDiritti
                {
                        IdDestinatario = request.objTrasm.ID_GROUP_NEW_OWNER,
                        IdUtente = request.objTrasm.ID_PEOPLE_NEW_OWNER
                });

                await this._repository.Update(aggregate);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new SalvaCessioneDirittiSuModelliTrasmResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SalvaCessioneDirittiSuModelliTrasmHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IModelloTrasmissioneRepository _repository;

        #endregion
    }

}
