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
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoRimuoviVersioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoRimuoviVersione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoRimuoviVersione
{
    public class DocumentoRimuoviVersioneHandler : IRequestHandler<DocumentoRimuoviVersioneRequest, DocumentoRimuoviVersioneResult>
    {
        #region Public Members

        public DocumentoRimuoviVersioneHandler(ILogger<DocumentoRimuoviVersioneHandler> logger,
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

        public async Task<DocumentoRimuoviVersioneResult> Handle(DocumentoRimuoviVersioneRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                if (!(request.schedaDocumento != null && request.schedaDocumento.protocollo != null && !string.IsNullOrEmpty(request.schedaDocumento.protocollo.segnatura)))
                {

                    var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

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

                    aggregate.RemoveVersion(request.fileRequest.versionId);

                    await this._repository.Update(aggregate);

                    output = true;

                    //AGGIOIRNO LA DPA_INFO_FILE CON LA NUOVA ULTIMA VERSIONE DEL DOCUMENTO

                    await this._webMethodLoggerService.LogOK("DOCUMENTORIMUOVIVERSIONE", request.schedaDocumento.docNumber, string.Format(Resources.LogDocumentoRimuoviVersione, request.schedaDocumento.docNumber));

                    //traccio l'evento FOLLOW_DOC_EXT_APP
                    var idDocMain = request.schedaDocumento.documentoPrincipale != null ? request.schedaDocumento.documentoPrincipale.docNumber : request.fileRequest.docNumber;
                    var descriptions = request.schedaDocumento.documentoPrincipale != null ? 
                        string.Format(Resources.LogFollowRemovedVersionAttachDoc, request.fileRequest.docNumber, idDocMain) : 
                        string.Format(Resources.LogFollowRemoveVersionDoc, idDocMain);

                    await this._webMethodLoggerService.LogKO("FOLLOWDOCEXTAPP", idDocMain, descriptions);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("DOCUMENTORIMUOVIVERSIONE", request.schedaDocumento.docNumber, string.Format(Resources.LogDocumentoRimuoviVersione, request.schedaDocumento.docNumber));
                output = false;
            }

            return new DocumentoRimuoviVersioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoRimuoviVersioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
