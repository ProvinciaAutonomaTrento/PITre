// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoAnnullaPredisposizioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoAnnullaPredisposizione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAnnullaPredisposizione
{
    public class DocumentoAnnullaPredisposizioneHandler : IRequestHandler<DocumentoAnnullaPredisposizioneRequest, DocumentoAnnullaPredisposizioneResult>
    {
        #region Public Members

        public DocumentoAnnullaPredisposizioneHandler(ILogger<DocumentoAnnullaPredisposizioneHandler> logger, 
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

        public async Task<DocumentoAnnullaPredisposizioneResult> Handle(DocumentoAnnullaPredisposizioneRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, request.schedaDocumento.systemId, new ILoadBehavior[1]
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
                        LoadNote = false
                    }
                });

                if (documentoAmministrativoAggregate.DatiRegistrazione == null
                    || string.IsNullOrEmpty(((DatiRegistrazioneProtocollo)documentoAmministrativoAggregate.DatiRegistrazione).NumeroProtocolloAsString))
                {
                    documentoAmministrativoAggregate.AnnullaPredisposizione();

                    await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);

                    output = true;

                    await this._webMethodLoggerService.LogOK("DOCUMENTOANNULLAPRED",
                        request.schedaDocumento.systemId,
                        string.Format(Resources.LogAnnullaPredisposizione, request.schedaDocumento.systemId));
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                await this._webMethodLoggerService.LogKO("DOCUMENTOANNULLAPRED",
                    request.schedaDocumento.systemId,
                    string.Format(Resources.LogAnnullaPredisposizione, request.schedaDocumento.systemId));
                output = false;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("DOCUMENTOANNULLAPRED",
                    request.schedaDocumento.systemId,
                    string.Format(Resources.LogAnnullaPredisposizione, request.schedaDocumento.systemId));
                output = false;
            }

            return new DocumentoAnnullaPredisposizioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoAnnullaPredisposizioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        #endregion
    }
}