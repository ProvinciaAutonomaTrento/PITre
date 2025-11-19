// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoAggiungiVersioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoAggiungiVersione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAggiungiVersione
{
    public class DocumentoAggiungiVersioneHandler : IRequestHandler<DocumentoAggiungiVersioneRequest, DocumentoAggiungiVersioneResult>
    {
        #region Public Members

        public DocumentoAggiungiVersioneHandler(ILogger<DocumentoAggiungiVersioneHandler> logger,
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
            this._repository = repository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<DocumentoAggiungiVersioneResult> Handle(DocumentoAggiungiVersioneRequest request, CancellationToken cancellationToken)
        {
            FileRequest output = request.fileRequest;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                if (request.fileRequest.repositoryContext != null)
                {
                    // Inserimento della versione nel repositorycontext (il documento ancora non è stato salvato)
                    int newVersion, newVersionLabel;
                    Int32.TryParse(request.fileRequest.version, out newVersion);
                    Int32.TryParse(request.fileRequest.versionLabel, out newVersionLabel);

                    output.subVersion = "!";
                    output.version = (newVersion + 1).ToString();
                    output.versionLabel = (newVersionLabel + 1).ToString();
                }
                else
                {
                    var aggregate = await this._repository.Get(idTenant, request.fileRequest.docNumber.ToString(), new ILoadBehavior[1]
                    {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadProfiles = false,
                            LoadProfilesMetadata = false,
                            LoadClassifications = false,
                            LoadAllegati = false,
                            LoadAggregazioni = false,
                            LoadVersions = true,
                            LoadPermissions = false,
                            LoadMittentiDestinatari = true,
                            LoadKeywords = false,
                            LoadNote = false,
                        }
                    });

                    aggregate.CreateEmptyVersion(new TextValue(request.fileRequest.descrizione));

                    await this._repository.Update(aggregate);

                    output.version = aggregate.Versions[aggregate.Versions.Count - 1].VersionNumber.ToString();
                    output.versionId = aggregate.Versions[aggregate.Versions.Count - 1].Id.ToString();
                }

                await this._webMethodLoggerService.LogOK("DOCUMENTOAGGIUNGIVERSIONE", output.docNumber, string.Format(Resources.LogDocumentoAggiungiVersione, output.docNumber, output.version));
            }
            catch (Exception ex) 
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("DOCUMENTOAGGIUNGIVERSIONE", output.docNumber, string.Format(Resources.LogDocumentoAggiungiVersione, output.docNumber, output.version));
                output = null;
            }

            return new DocumentoAggiungiVersioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoAggiungiVersioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
