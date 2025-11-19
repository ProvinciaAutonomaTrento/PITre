// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deleteMezzoSpedizioneDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.deleteMezzoSpedizioneDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.deleteMezzoSpedizioneDocumento
{
    public class deleteMezzoSpedizioneDocumentoHandler : IRequestHandler<deleteMezzoSpedizioneDocumentoRequest, deleteMezzoSpedizioneDocumentoResult>
    {
        #region Public Members

        public deleteMezzoSpedizioneDocumentoHandler(ILogger<deleteMezzoSpedizioneDocumentoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._repository = repository;
        }

        public async Task<deleteMezzoSpedizioneDocumentoResult> Handle(deleteMezzoSpedizioneDocumentoRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            var aggregate = await this._repository.Get(request.info.idAmministrazione, request.idProfile, new ILoadBehavior[1]
            {
                new GetDocumentoAmministrativoLoadBehavior()
                {
                    LoadProfiles = false,
                    LoadClassifications = false,
                    LoadAllegati = false,
                    LoadAggregazioni = false,
                    LoadVersions = false,
                    LoadPermissions = false,
                    LoadMittentiDestinatari = true
                }
            });
            aggregate.RemoveMezzoSpedizione();
            await this._repository.Update(aggregate);

            output = true;

            return new deleteMezzoSpedizioneDocumentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<deleteMezzoSpedizioneDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
