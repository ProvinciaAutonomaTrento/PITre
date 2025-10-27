// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ModificaListaGruppo
{
    public class ModificaListaGruppoHandler : IRequestHandler<modificaListaGruppo, modificaListaGruppoResult>
    {
        #region Public members
        public ModificaListaGruppoHandler(ILogger<ModificaListaGruppoHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IListaDistribuzioneRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }
        public async Task<modificaListaGruppoResult> Handle(modificaListaGruppo request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var aggregate = await this._repository.Get(idTenant, request.idLista);

            if (!request.nomeLista.ToUpper().Equals(aggregate.Description.Value.ToUpper())) aggregate.ChangeDescription(new TextValue(request.nomeLista));
            if (!request.codiceLista.ToUpper().Equals(aggregate.Name.Value.ToUpper())) aggregate.ChangeName(new TextValue(request.codiceLista));

            aggregate.AssignAutoreRuolo(request.idGruppo);

            await this._repository.Update(aggregate);

            await this._mediator.Send(new Requests.modificaListaCorr(
                request.dsCorrLista,
                request.idLista));

            return new modificaListaGruppoResult();
        }

        #endregion

        #region Private members

        protected ILogger<ModificaListaGruppoHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected IListaDistribuzioneRepository _repository;

        #endregion
    }
}
