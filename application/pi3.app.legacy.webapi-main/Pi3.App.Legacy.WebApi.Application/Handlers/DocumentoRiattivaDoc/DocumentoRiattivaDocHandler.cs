// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoRiattivaDoc
{

    // Richiede libreria MediatR
    public class DocumentoRiattivaDocHandler : IRequestHandler<Application.Requests.DocumentoRiattivaDoc, DocumentoRiattivaDocResult>
    {
        #region Public Members

        public DocumentoRiattivaDocHandler(ILogger<DocumentoRiattivaDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<DocumentoRiattivaDocResult> Handle(Application.Requests.DocumentoRiattivaDoc request, CancellationToken cancellationToken)
        {
            bool result = false;
            DocsPaVO.documento.InfoDocumento infoDocumento = request.infoDocumento;
            long idProfileAsLong = request.infoDocumento.idProfile.AsLong();

            string idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            try
            {

                DocumentoAmministrativo documentAggregate = await this._documentoAmministrativoRepository.Get(idTenant.ToString(), infoDocumento.idProfile, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadProfilesMetadata = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false,
                        BypassSecurityCheck = true
                    }
                });

                if (documentAggregate == null)
                    throw new DocumentNotFoundPi3Exception(idProfileAsLong);

                documentAggregate.Restore();

                // Gestione chiave IS_ENABLED_PROFILAZIONE_ALLEGATI??
                //bool isAllegato = infoDocumento.allegato;
                //if (!isAllegato)
                //{
                //    var attachments = documentAggregate.Allegati;
                //    foreach(var a in attachments)
                //    {
                //        DocumentoAmministrativo attachmentAggregate = await this._documentoAmministrativoRepository.Get(idTenant.ToString(), a.IdDoc.Identiticativo);
                //        attachmentAggregate.Restore();
                //        await this._documentoAmministrativoRepository.Update(attachmentAggregate);
                //    }
                //}

                await this._documentoAmministrativoRepository.Update(documentAggregate);

                await this._webMethodLoggerService.LogOK("DOCUMENTORIATTIVADOC", infoDocumento.docNumber, string.Format(Resources.LogRiattivaDoc, infoDocumento.docNumber));
                result = true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("DOCUMENTORIATTIVADOC", infoDocumento.docNumber, string.Format(Resources.LogRiattivaDoc, infoDocumento.docNumber));
            }

            return new DocumentoRiattivaDocResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoRiattivaDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        #endregion
    }

}
