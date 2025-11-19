// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using collegaMezzoSpedizioneDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.collegaMezzoSpedizioneDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.collegaMezzoSpedizioneDocumento
{
    public class collegaMezzoSpedizioneDocumentoHandler : IRequestHandler<collegaMezzoSpedizioneDocumentoRequest, collegaMezzoSpedizioneDocumentoResult>
    {
        #region Public Members

        public collegaMezzoSpedizioneDocumentoHandler(ILogger<collegaMezzoSpedizioneDocumentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._repository = repository;
        }

        public async Task<collegaMezzoSpedizioneDocumentoResult> Handle(collegaMezzoSpedizioneDocumentoRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long idAmministrazione = !string.IsNullOrEmpty(request.info.idAmministrazione) ? request.info.idAmministrazione.AsLong() : this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
                long idDocumentTypes = request.idDocumentTypes.AsLong();
                long idProfile = request.idProfile.AsLong();

                var aggregate = await this._repository.Get(idAmministrazione.ToString(), request.idProfile, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }

                    }
                });
                aggregate.AssignMezzoSpedizione(request.idDocumentTypes);
                await this._repository.Update(aggregate);

                await this._webMethodLoggerService.LogOK("DOCUMENTOSPEDIZIONE", request.idProfile, string.Format(Resources.LogDescriptionInsertMezzoSpedizione, request.idProfile));

                output = true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
                await this._webMethodLoggerService.LogKO("DOCUMENTOSPEDIZIONE", request.idProfile, string.Format(Resources.LogDescriptionInsertMezzoSpedizione, request.idProfile));
            }

            return new collegaMezzoSpedizioneDocumentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<collegaMezzoSpedizioneDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }

}
