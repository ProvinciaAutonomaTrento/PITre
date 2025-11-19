// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetDettaglioDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumento;
using DocumentoGetDettaglioDocumentoNoSecurityRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity;
using UpdateLastDocumentsViewRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateLastDocumentsView;
using Pi3.Core.Extensions;
using Pi3.Core.Services.WebMethodLogger;
using DocsPaVO.ProfilazioneDinamicaLite;
using Pi3.Core.Services.Configuration;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetDettaglioDocumento
{

    // Richiede libreria MediatR
    public class DocumentoGetDettaglioDocumentoHandler : IRequestHandler<DocumentoGetDettaglioDocumentoRequest, DocumentoGetDettaglioDocumentoResult>
    {
        #region Public Members

        public DocumentoGetDettaglioDocumentoHandler(
            ILogger<DocumentoGetDettaglioDocumentoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
        }

        public async Task<DocumentoGetDettaglioDocumentoResult> Handle(DocumentoGetDettaglioDocumentoRequest request, CancellationToken cancellationToken)
        {
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true).AsLong();
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true).AsLong();
            var idProfile = request.idProfile.AsLong();
            var idAmministrazioneAsNumber = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            try
            {
                var idDocumentoPrincipale = await this._dbContext.ProfileEntities
                    .AsNoTracking()
                    .Where(p => p.SYSTEM_ID == idProfile)
                    .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
                    .FirstOrDefaultAsync();

                if (idDocumentoPrincipale.HasValue)
                    idProfile = idDocumentoPrincipale.Value;

                await this._dbContext.AssertSecurityRights(idProfile.ToString(), idUser.ToString(), idGroup.ToString());
                
                var getDettaglioDocumento = await this._mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityRequest(request.infoutente, request.idProfile, request.docNumber));

                getDettaglioDocumento.output.accessRights = (await this._dbContext.GetSecurity(idProfile.ToString(), idUser.ToString(), idGroup.ToString())).ACCESSRIGHTS.ToString();

                var setDataVistaOutput = await this._mediator.Send(new Application.Requests.SetDataVistaSP(request.infoutente, request.idProfile, "D"));

                if(!idDocumentoPrincipale.HasValue)
                    await this._mediator.Send(new UpdateLastDocumentsViewRequest(request.idProfile));

                return new DocumentoGetDettaglioDocumentoResult(getDettaglioDocumento.output);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                
                await this._webMethodLoggerService.LogKO(
                    "DOCUMENTOGETDETTAGLIODOCUMENTO", 
                    request.idProfile);

                return new DocumentoGetDettaglioDocumentoResult(null);
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetDettaglioDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;


        #endregion
    }
}
