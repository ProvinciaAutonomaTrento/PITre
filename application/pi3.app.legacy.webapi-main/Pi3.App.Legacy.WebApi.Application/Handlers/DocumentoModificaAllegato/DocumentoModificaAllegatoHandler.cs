// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoModificaAllegatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoModificaAllegato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoModificaAllegato
{
    public class DocumentoModificaAllegatoHandler : IRequestHandler<DocumentoModificaAllegatoRequest, DocumentoModificaAllegatoResult>
    {
        #region Public Members

        public DocumentoModificaAllegatoHandler(ILogger<DocumentoModificaAllegatoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IDocumentoAmministrativoRepository repository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._repository = repository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<DocumentoModificaAllegatoResult> Handle(DocumentoModificaAllegatoRequest request, CancellationToken cancellationToken)
        {
            bool output = true;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            try
            {
                if (request.allegato.repositoryContext == null)
                {
                    var aggregate = await this._repository.Get(idTenant, request.allegato.docNumber, new ILoadBehavior[1]
                    {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = false,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = false,
                        LoadKeywords = false,
                        LoadNote = false,
                        LoadProfilesMetadata = false,
                        LoadRelatedElements = false
                    }
                    });

                    aggregate.ChangeOggettoDelDocumento(new OggettoDelDocumento()
                    {
                        Descrizione = new TextValue(request.allegato.descrizione)
                    });

                    aggregate.ChangeNumeroPagineAllegato(request.allegato.numeroPagine);

                    await this._repository.Update(aggregate);

                    //l'evento FOLLOW_DOC_EXT_APP scatta in seguito all' annullamento di un protocollo
                    if (!string.IsNullOrEmpty(request.idDocumentoPrincipale))
                        await this._webMethodLoggerService.LogKO("FOLLOWDOCEXTAPP", 
                            request.idDocumentoPrincipale, 
                            string.Format(Descriptions.LogFollowUpdMetaDataDoc, request.allegato.docNumber, request.allegato.descrizione));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = false;
            }

            return new DocumentoModificaAllegatoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoModificaAllegatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IDocumentoAmministrativoRepository _repository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;


        #endregion
    }
}
