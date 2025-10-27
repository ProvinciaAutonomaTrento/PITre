// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.DocumentFSRepository.AggregateModels.DocumentBlobAggregate;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFileDocumentById
{
    // Richiede libreria MediatR
    public class GetFileDocumentByIdCommandHandler : IRequestHandler<GetFileDocumentByIdCommand, GetFileDocumentByIdCommandResponse>
    {
        #region Public Members

        public GetFileDocumentByIdCommandHandler(ILogger<GetFileDocumentByIdCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IDocumentoAmministrativoRepository documentoAmministrativoRepository, IDocumentBlobRepository blobRepository, IWebMethodLoggerService loggerService,
            ICAdESService cAdESService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._docRepository = documentoAmministrativoRepository;
            this._blobRepository = blobRepository;
            this._loggerService = loggerService;
            this._cAdESService = cAdESService;
        }

        public async Task<GetFileDocumentByIdCommandResponse> Handle(GetFileDocumentByIdCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetFileDocumentById - START");

            GetFileDocumentByIdCommandResponse response = new GetFileDocumentByIdCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_ID");
                }
                #endregion

                #region implementazione
                // TODO: l metodo pu� riguardare sia un allegato che un documento, quindi il controllo della security va verificato
                // TODO: come si comporta con i file firmati?

                // Test per documento principale
                string idDocPrincipale = request.IdDocument;
                var verificaIdDocPrincipale = await DBUtils.GetIdDocPrincipale(request.IdDocument, _pi3DbContext);
                if (verificaIdDocPrincipale != null && verificaIdDocPrincipale > 0) idDocPrincipale = verificaIdDocPrincipale.ToString();
                
                try
                {
                    await _pi3DbContext.AssertSecurityRights(idDocPrincipale, infoUtente.idPeople, infoUtente.idGruppo);
                }
                catch (Exception security) { throw new RestException("DOCUMENT_NOT_FOUND"); }

                var documentAggregate = await _docRepository.Get(infoUtente.idAmministrazione, request.IdDocument);
                Pi3.Core.AggregateModels.DocumentAggregate.Entities.DocumentVersion version = null;
                if (!string.IsNullOrWhiteSpace(request.VersionId) && request.VersionId != "SIGNED")
                    version = documentAggregate.Versions.First(v => v.Id.Equals(request.VersionId, StringComparison.InvariantCultureIgnoreCase));
                else
                    version = documentAggregate.CurrentVersion;

                this._logger.LogDebug($"IdVersion: {version.Id}");

                if (version.DocumentBlobRef == null)
                    throw new RestException("DOCUMENT_NOT_FOUND");

                var idBlob = version.DocumentBlobRef.IdBlob;
                this._logger.LogDebug($"idBlob: {idBlob}");

                if (!await _blobRepository.Exists(infoUtente.idAmministrazione, idBlob))
                    throw new RestException("DOCUMENT_NOT_FOUND");

                var documentBlobAggregate = await _blobRepository.Get(infoUtente.idAmministrazione, idBlob);
                if (documentBlobAggregate != null)
                {
                    response.File = new File()
                    {
                        Id = documentAggregate.Id,
                        Description = documentBlobAggregate.Description?.ToString(),
                        MimeType = documentBlobAggregate.ContentType?.ToString(),
                        Name = documentBlobAggregate.FileName?.ToString(),
                        VersionId = version.Id
                    };
                    if (documentBlobAggregate.Stream != null)
                    {
                        if (!string.IsNullOrEmpty(request.VersionId) && request.VersionId == "SIGNED")
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                documentBlobAggregate.Stream.CopyTo(ms);
                                response.File.Content = ms.ToArray();
                            }
                            var sbustato = new MemoryStream();
                            await _cAdESService.LoadOriginalFile(Path.GetFileName(documentBlobAggregate.FileName), documentBlobAggregate.Stream, sbustato);
                            response.File.Content = sbustato.ToArray();
                        }
                        else
                        {
                            if (documentBlobAggregate.FileName != null &&
                                (documentBlobAggregate.FileName.ToUpper().EndsWith("P7M")
                                || documentBlobAggregate.FileName.ToUpper().EndsWith("M7M")
                                || documentBlobAggregate.FileName.ToUpper().EndsWith("TSD")
                                ))
                            {
                                var sbustato = new MemoryStream();
                                await _cAdESService.LoadOriginalFile(Path.GetFileName(documentBlobAggregate.FileName), documentBlobAggregate.Stream, sbustato);
                                response.File.Content = sbustato.ToArray();
                            }
                            else
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    documentBlobAggregate.Stream.CopyTo(ms);
                                    response.File.Content = ms.ToArray();
                                }
                            }
                        }

                        
                    }
                    else
                    {
                        throw new RestException("FILESTREAM_NOT_FOUND", "Stream del File non trovato");
                    }
                }
                else throw new RestException("FILE_NOT_FOUND","File non trovato");

                await _loggerService.LogOK("DOCUMENTOGETFILE",
                        documentAggregate.Id, $"PIS REST:Visualizzato il documento con id {documentAggregate.Id} tramite PIS",
                        null, infoUtente.codWorkingApplication);
                #endregion

                response.Code = GetFileDocumentByIdResponseCode.OK;

                _logger.LogInformation("end GetFileDocumentById");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetFileDocumentById: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetFileDocumentByIdCommandResponse();
                response.Code = GetFileDocumentByIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetFileDocumentById");
                response = new GetFileDocumentByIdCommandResponse();
                response.Code = GetFileDocumentByIdResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetFileDocumentByIdCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentoAmministrativoRepository _docRepository;
        protected readonly IDocumentBlobRepository _blobRepository;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly ICAdESService _cAdESService;

        #endregion
    }

}