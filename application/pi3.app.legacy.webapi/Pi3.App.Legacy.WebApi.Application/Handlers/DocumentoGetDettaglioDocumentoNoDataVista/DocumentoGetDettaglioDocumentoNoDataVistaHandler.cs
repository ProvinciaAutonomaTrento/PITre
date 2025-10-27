// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetDettaglioDocumentoNoDataVistaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumentoNoDataVista;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetDettaglioDocumentoNoDataVista
{
    public class DocumentoGetDettaglioDocumentoNoDataVistaHandler : IRequestHandler<DocumentoGetDettaglioDocumentoNoDataVistaRequest, DocumentoGetDettaglioDocumentoNoDataVistaResult>
    {
        #region Public Members

        public DocumentoGetDettaglioDocumentoNoDataVistaHandler(ILogger<DocumentoGetDettaglioDocumentoNoDataVistaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<DocumentoGetDettaglioDocumentoNoDataVistaResult> Handle(DocumentoGetDettaglioDocumentoNoDataVistaRequest request, CancellationToken cancellationToken)
        {
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true).AsLong();
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true).AsLong();
            var idProfile = request.idProfile.AsLong();
            var idAmministrazioneAsNumber = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            SchedaDocumento schedaDocumento = null;
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

                schedaDocumento = (await this._mediator.Send(new Requests.DocumentoGetDettaglioDocumentoNoSecurity(request.infoutente, request.idProfile, request.docNumber))).output;

                schedaDocumento.accessRights = (await this._dbContext.GetSecurity(idProfile.ToString(), idUser.ToString(), idGroup.ToString())).ACCESSRIGHTS.ToString();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);

                schedaDocumento = null;
            }

            return new DocumentoGetDettaglioDocumentoNoDataVistaResult(schedaDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetDettaglioDocumentoNoDataVistaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}