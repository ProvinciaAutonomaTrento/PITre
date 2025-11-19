// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnullaContatoreDiRepertorioRequest = Pi3.App.Legacy.WebApi.Application.Requests.AnnullaContatoreDiRepertorio;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AnnullaContatoreDiRepertorio
{
    public class AnnullaContatoreDiRepertorioHandler : IRequestHandler<AnnullaContatoreDiRepertorioRequest, AnnullaContatoreDiRepertorioResult>
    {
        #region Public Members

        public AnnullaContatoreDiRepertorioHandler(ILogger<AnnullaContatoreDiRepertorioHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<AnnullaContatoreDiRepertorioResult> Handle(AnnullaContatoreDiRepertorioRequest request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            var aggregate = await this._documentoAmministrativoRepository.Get(idTenant, request.docNumber, new ILoadBehavior[1]
            {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadProfilesMetadata = true,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
                    }
            });

            var idProfile = aggregate.Profiles[0].Id;

            aggregate.AnnullaContatoreRepertorio(idProfile, request.idOggetto);
            await _documentoAmministrativoRepository.Update(aggregate);

            await this._webMethodLoggerService.LogOK("DOCUMENTO_EXEC_ANNULLA_REPERTORIO", request.docNumber, Resources.LogAnnullamentoContatoreRepertorio);

            return new AnnullaContatoreDiRepertorioResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AnnullaContatoreDiRepertorioHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        #endregion
    }
}