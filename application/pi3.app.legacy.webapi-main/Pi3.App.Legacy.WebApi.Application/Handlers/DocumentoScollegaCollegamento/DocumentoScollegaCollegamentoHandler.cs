// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoScollegaCollegamentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoScollegaCollegamento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoScollegaCollegamento
{
    public class DocumentoScollegaCollegamentoHandler : IRequestHandler<DocumentoScollegaCollegamentoRequest, DocumentoScollegaCollegamentoResult>
    {
        #region Public Members

        public DocumentoScollegaCollegamentoHandler(ILogger<DocumentoScollegaCollegamentoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }

        public async Task<DocumentoScollegaCollegamentoResult> Handle(DocumentoScollegaCollegamentoRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            string idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            try
            {
               var aggregate = await this._repository.Get(idTenant, request.systemId, new ILoadBehavior[1]
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

                var parent = aggregate.RelatedElements.FirstOrDefault(r => r.AsParent == true);
                if (parent != null)
                {
                    aggregate.RemoveReleatedElement(parent.Id);
                    await this._repository.Update(aggregate);
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new DocumentoScollegaCollegamentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoScollegaCollegamentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
