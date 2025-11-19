// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CambiaFascicolazionePrimariaRequest = Pi3.App.Legacy.WebApi.Application.Requests.CambiaFascicolazionePrimaria;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CambiaFascicolazionePrimaria
{
    public class CambiaFascicolazionePrimariaHandler : IRequestHandler<CambiaFascicolazionePrimariaRequest, CambiaFascicolazionePrimariaResult>
    {
        #region Public Members

        public CambiaFascicolazionePrimariaHandler(ILogger<CambiaFascicolazionePrimariaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._repository = repository;
        }

        public async Task<CambiaFascicolazionePrimariaResult> Handle(CambiaFascicolazionePrimariaRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idProject = request.idProject;

                var aggregate = await this._repository.Get(idTenant, request.idProfile, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = true,
                        LoadAllegati = false,
                        LoadAggregazioni = true,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false,
                        LoadProfilesMetadata = false,
                        LoadRelatedElements = false,
                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                    }
                });

                if (!aggregate.Classifications.Any(c => c.Id == idProject) && !aggregate.Aggregazioni.Any(c => c.Id == idProject))
                    throw new FascicoloNotFoundPi3Exception();

                if(aggregate.Classifications.Any(c => c.Id == idProject))
                {
                    var classification = aggregate.Classifications.Where(c => c.Id == idProject).Select(c => c).First();
                    aggregate.AssignClassificationOrAggAsPrincipale(classification);
                }
                else if(aggregate.Aggregazioni.Any(c => c.Id == idProject))
                {
                    var aggregazione = aggregate.Aggregazioni.Where(c => c.Id == idProject).Select(c => c).First();
                    aggregate.AssignClassificationOrAggAsPrincipale(aggregazione);
                }

                await this._repository.Update(aggregate);

                output = true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new CambiaFascicolazionePrimariaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CambiaFascicolazionePrimariaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
