// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFile;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;
using System.IO;
using System.Security.Cryptography.Xml;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFileWithSignatureOrStamp
{
    // Richiede libreria MediatR
    public class GetFileWithSignatureOrStampCommandHandler : IRequestHandler<GetFileWithSignatureOrStampCommand, GetFileWithSignatureOrStampCommandResponse>
    {
        #region Public Members

        public GetFileWithSignatureOrStampCommandHandler(ILogger<GetFileWithSignatureOrStampCommandHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor,
            IPi3DbContext pi3DbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<GetFileWithSignatureOrStampCommandResponse> Handle(GetFileWithSignatureOrStampCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetFileWithSignatureOrStamp - START");

            GetFileWithSignatureOrStampCommandResponse response = new GetFileWithSignatureOrStampCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.idDocument) && string.IsNullOrEmpty(request.signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.idDocument) && !string.IsNullOrEmpty(request.signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }
                #endregion

                #region implementazione

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    long? idProfile = !string.IsNullOrEmpty(request.idDocument) ? request.idDocument.AsLong() : null;
                    if(idProfile == null && !string.IsNullOrEmpty(request.signature))
                    {
                        idProfile = await _pi3DbContext.ProfileEntities.AsNoTracking()
                                .Where(p => p.VAR_SEGNATURA != null && p.VAR_SEGNATURA.ToUpper() == request.signature.ToUpper())
                                .Select(p => p.SYSTEM_ID)
                                .FirstOrDefaultAsync();
                    }

                    await _pi3DbContext.AssertSecurityRights(idProfile.ToString(), infoUtente.idPeople, infoUtente.idGruppo);

                    documento = (await this._mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                    {
                        Infoutente = infoUtente,
                        DocNumber = idProfile.ToString(),
                        IdProfile = idProfile.ToString()
                    })).Output;

                    var withSignature = true;
                    var withStamp = false;

                    if (!string.IsNullOrWhiteSpace(request.signOrStamp) && request.signOrStamp.ToUpper() == "STAMP")
                    {
                        withSignature = false;
                        withStamp = true;
                    }
                    if (documento != null && documento.documenti != null && documento.documenti.Length > 0)
                    {
                        int numVersione = 0;
                        var path = Resources.labelPDF;
                        DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[numVersione];
                        response.File = (await _mediator.Send(new GetFileCommand()
                        {
                             fileRequest = versione,
                             getFile = true,
                             infoUtente = infoUtente,
                             segnatura = withSignature,
                             timbro = withStamp,
                             path = path,
                             schedaDoc = documento,
                             fileConFirma = false
                        })).File;

                        response.Code = GetFileDocumentByIdResponseCode.OK;
                    }
                    else
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                await this._webMethodLoggerService.LogOK("DOCUMENTOGETFILE", documento.docNumber, string.Format(Resources.LogVisualizzatoDocumento, documento.docNumber));

                #endregion

                response.Code = GetFileDocumentByIdResponseCode.OK;

                _logger.LogInformation("end GetFileWithSignatureOrStamp");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetFileWithSignatureOrStamp: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetFileWithSignatureOrStampCommandResponse();
                response.Code = GetFileDocumentByIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetFileWithSignatureOrStamp");
                response = new GetFileWithSignatureOrStampCommandResponse();
                response.Code = GetFileDocumentByIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetFileWithSignatureOrStampCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }

}