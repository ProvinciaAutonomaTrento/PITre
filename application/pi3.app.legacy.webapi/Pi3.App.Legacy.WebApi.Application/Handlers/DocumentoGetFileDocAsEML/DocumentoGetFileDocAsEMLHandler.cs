// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetFileDocAsEMLRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileDocAsEML;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetFileDocAsEML
{
    public class DocumentoGetFileDocAsEMLHandler : IRequestHandler<DocumentoGetFileDocAsEMLRequest, DocumentoGetFileDocAsEMLResult>
    {
        #region Public Members

        public DocumentoGetFileDocAsEMLHandler(ILogger<DocumentoGetFileDocAsEMLHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IWebMethodLoggerService webMethodLoggerService,
            IEmailBoxScannerService emailBoxScannerService,
            IPi3DbContext dbContext,
            ISessionRepositoryService sessionRepositoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._webMethodLoggerService = webMethodLoggerService;
            this._emailBoxScannerService = emailBoxScannerService;
            this._dbContext = dbContext;
            this._sessionRepositoryService = sessionRepositoryService;
        }

        public async Task<DocumentoGetFileDocAsEMLResult> Handle(DocumentoGetFileDocAsEMLRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = null;
            var errorMsg  = string.Empty;
            try
            {
                if (request.fileRequest.repositoryContext != null)
                {
                    output = await _sessionRepositoryService.GetFile(request.fileRequest.repositoryContext, request.fileRequest);
                }
                else
                {
                    var componentsEntity = await _dbContext.ComponentEntities.AsNoTracking()
                        .Where(c => c.VERSION_ID == request.fileRequest.versionId.AsLong())
                        .Select(c => new
                        {
                            c.PATH,
                            c.VAR_NOMEORIGINALE,
                            c.EXT
                        })
                        .FirstAsync();

                    request.fileRequest.path = componentsEntity.PATH;
                    output = (await this._mediator.Send(new Application.Requests.GetFileDocument(request.fileRequest, request.infoUtente))).output;
                }

                var emailBoxScannerServiceResponse = await _emailBoxScannerService.Parse(new MemoryStream(output.content));
                var body = $"{emailBoxScannerServiceResponse.Body.ToString()} <br />Mail Mittente: {emailBoxScannerServiceResponse.Sender.Address}";

                if ((IsPECDelivered(emailBoxScannerServiceResponse.Headers) || IsFromNonPEC(emailBoxScannerServiceResponse)) 
                    && emailBoxScannerServiceResponse.AttachmentsAsEmails != null && emailBoxScannerServiceResponse.AttachmentsAsEmails.Any())
                {
                    body = $"{emailBoxScannerServiceResponse.AttachmentsAsEmails[0].Body.ToString()} <br />Mail Mittente: {emailBoxScannerServiceResponse.AttachmentsAsEmails[0].Sender.Address}";
                }

                output.content = Encoding.UTF8.GetBytes(body);
                output.contentType = "text/html";

                await this._webMethodLoggerService.LogOK("DOCUMENTOGETFILE", request.fileRequest.docNumber, string.Format(Resources.DocumentoGetFileOK, request.fileRequest.docNumber, request.fileRequest.version));
            }
            catch (Exception ex)
            {
                await this._webMethodLoggerService.LogKO("DOCUMENTOGETFILE", request.fileRequest.docNumber, string.Format(Resources.DocumentoGetFileKO, request.fileRequest.docNumber, request.fileRequest.version));
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = null;
            }
            return new DocumentoGetFileDocAsEMLResult(output, errorMsg);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetFileDocAsEMLHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IEmailBoxScannerService _emailBoxScannerService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ISessionRepositoryService _sessionRepositoryService;

        /// <summary>
        /// Verifica che mail certificata  � stata consegnata al/ai destinatari/io correttamente X-Trasporto
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public bool IsPECDelivered(List<EmailHeader> headers)
        {
            return headers.Any(h => h.Name == "X-Trasporto" && h.Value.ToLower() == "posta-certificata");
        }

        /// <summary>
        /// il mittente in from non � di tipo pec
        /// </summary>
        /// <returns></returns>
        public bool IsFromNonPEC(Email emailBoxScanner)
        {
            return emailBoxScanner.Headers.Any(h => h.Name == "X-Trasporto" && h.Value.ToLower() == "errore") &&
                emailBoxScanner.Subject.ToString().Trim().ToUpper().StartsWith("ANOMALIA MESSAGGIO") &&
                emailBoxScanner.Body.ToString().IndexOf("dati non sono stati certificati") >= 0;
        }

        #endregion
    }
}