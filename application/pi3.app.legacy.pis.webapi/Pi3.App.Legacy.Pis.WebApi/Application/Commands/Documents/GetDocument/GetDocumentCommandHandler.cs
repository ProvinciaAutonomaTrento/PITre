// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocument
{
    // Richiede libreria MediatR
    public class GetDocumentCommandHandler : IRequestHandler<GetDocumentCommand, GetDocumentCommandResponse>
    {
        #region Public Members

        public GetDocumentCommandHandler(ILogger<GetDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IDocumentoAmministrativoRepository amministrativoRepository, IDocumentBlobRepository blobRepository, IWebMethodLoggerService loggerService, ICAdESService cAdESService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._darepository = amministrativoRepository;
            this._blobRepository = blobRepository;
            this._loggerService = loggerService;
            this._cAdESService = cAdESService;
        }

        public async Task<GetDocumentCommandResponse> Handle(GetDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetDocument - START");

            GetDocumentCommandResponse response = new GetDocumentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                #endregion

                #region implementazione
                string idDocument = "";
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    idDocument = (from a in _pi3DbContext.ProfileEntities where a.VAR_SEGNATURA != null && a.VAR_SEGNATURA.ToUpper() == request.Signature.ToUpper() select a.SYSTEM_ID).FirstOrDefault().ToString();
                }
                else
                {
                    idDocument = request.IdDocument;
                }

                var idProfile = idDocument.AsLong();
                var idDocumentoPrincipale = await this._pi3DbContext.ProfileEntities
                    .AsNoTracking()
                    .Where(p => p.SYSTEM_ID == idProfile)
                    .Select(p => p.ID_DOCUMENTO_PRINCIPALE)
                    .FirstOrDefaultAsync();

                if (idDocumentoPrincipale.HasValue)
                    idProfile = idDocumentoPrincipale.Value;

                if (string.IsNullOrWhiteSpace(idDocument) || idDocument == "0") throw new RestException("DOCUMENT_NOT_FOUND");
                // verificare controllo security su documento allegato
                try
                {
                    await _pi3DbContext.AssertSecurityRights(idProfile.ToString(), infoUtente.idPeople, infoUtente.idGruppo);
                }
                catch (Exception security) { throw new RestException("DOCUMENT_NOT_FOUND"); }

                var aggregate = await _darepository.Get(infoUtente.idAmministrazione, idDocument,
                    new Core.SeedWork.ILoadBehavior[1]{ new Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories.GetDocumentoAmministrativoLoadBehavior()
                {
                    LoadMittentiDestinatari = true,
                     LoadNote =true,
                      LoadProfilesMetadata =true,
                      LoadProfiles = true
                }});

                if (aggregate.InRecycleBin) throw new RestException("DOCUMENT_NOT_FOUND");

                if (aggregate != null && !string.IsNullOrWhiteSpace(aggregate.Id))
                {
                    response.Document = RestUtils.getDocFromAggregate(aggregate, _pi3DbContext,this._mediator,null,true);
                    //TODO: eventualmente aggiungere i campi mancanti
                    var note = DBUtils.getNoteOggetto(aggregate.Id, _pi3DbContext);
                    if (note != null)
                    {
                        response.Document.Note = note.ToArray();
                    }
                    response.Document.Template = DBUtils.getTemplateFromDocumentId(aggregate.Id, _pi3DbContext);
                    response.Document.ParentDocument = await DBUtils.getParentDocInfoFromDocId(aggregate.Id, _pi3DbContext);
                    var childDocs = await DBUtils.getChildDocsInfoFromDocId(aggregate.Id, _pi3DbContext);
                    if (childDocs != null && childDocs.Any())
                        response.Document.LinkedDocuments = childDocs.ToArray();
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                #region prelievo del file principale
                if (request.GetFile)
                {
                    //TODO: prelievo file sia senza che con firma
                    var version = aggregate.CurrentVersion;
                    if (version.DocumentBlobRef != null)
                    {
                        var idBlob = version.DocumentBlobRef.IdBlob;
                        if (await _blobRepository.Exists(infoUtente.idAmministrazione, idBlob))
                        {
                            var documentBlobAggregate = await _blobRepository.Get(infoUtente.idAmministrazione, idBlob);
                            response.Document.MainDocument = new File()
                            {

                                Id = aggregate.Id,
                                Description = documentBlobAggregate.Description?.ToString(),
                                MimeType = documentBlobAggregate.ContentType?.ToString(),
                                Name = documentBlobAggregate.FileName?.ToString(),
                                VersionId = version.Id
                            };
                            if (documentBlobAggregate.Stream != null)
                            {
                                if (!string.IsNullOrEmpty(request.GetFileWithSignature) && request.GetFileWithSignature == "1")
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        documentBlobAggregate.Stream.CopyTo(ms);
                                        response.Document.MainDocument.Content = ms.ToArray();
                                    }
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
                                        response.Document.MainDocument.Content = sbustato.ToArray();
                                    }
                                    else
                                    {
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            documentBlobAggregate.Stream.CopyTo(ms);
                                            response.Document.MainDocument.Content = ms.ToArray();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var version = aggregate.CurrentVersion;
                    if (version.DocumentBlobRef != null)
                    {
                        response.Document.MainDocument = new File()
                        {

                            Id = aggregate.Id,
                            MimeType = version.DocumentBlobRef.ContentType?.ToString(),
                            Name = version.DocumentBlobRef.FileName?.ToString(),
                            VersionId = version.Id
                        };
                    }
                }

                #endregion

                #region allegati
                var allegati = DBUtils.GetAllegatiDocumento(aggregate.Id, infoUtente.codWorkingApplication, _pi3DbContext);
                if (allegati != null && allegati.Any())
                {
                    if (!request.GetFile)
                    {
                        response.Document.Attachments = allegati.ToArray();
                    }
                    else
                    {
                        List<Documents.File> allegatiRisp = new List<Documents.File>();
                        foreach (var attach in allegati)
                        {
                            var attAgg = await _darepository.Get(infoUtente.idAmministrazione, attach.Id);
                            if (attAgg != null)
                            {
                                var version = attAgg.CurrentVersion;
                                if (version.DocumentBlobRef != null)
                                {
                                    var idBlob = version.DocumentBlobRef.IdBlob;
                                    if (await _blobRepository.Exists(infoUtente.idAmministrazione, idBlob))
                                    {
                                        var documentBlobAggregate = await _blobRepository.Get(infoUtente.idAmministrazione, idBlob);
                                        var aggFile = new File()
                                        {

                                            Id = aggregate.Id,
                                            Description = documentBlobAggregate.Description?.ToString(),
                                            MimeType = documentBlobAggregate.ContentType?.ToString(),
                                            Name = documentBlobAggregate.FileName?.ToString(),
                                            VersionId = version.Id
                                        };
                                        //if (documentBlobAggregate.Stream != null)
                                        //{
                                        //    using (MemoryStream ms = new MemoryStream())
                                        //    {
                                        //        documentBlobAggregate.Stream.CopyTo(ms);
                                        //        aggFile.Content = ms.ToArray();
                                        //    }
                                        //}
                                        if (documentBlobAggregate.Stream != null)
                                        {
                                            if (!string.IsNullOrEmpty(request.GetFileWithSignature) && request.GetFileWithSignature == "1")
                                            {

                                                using (MemoryStream ms = new MemoryStream())
                                                {
                                                    documentBlobAggregate.Stream.CopyTo(ms);
                                                    aggFile.Content = ms.ToArray();
                                                }

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
                                                    aggFile.Content = sbustato.ToArray();
                                                }
                                                else
                                                {
                                                    using (MemoryStream ms = new MemoryStream())
                                                    {
                                                        documentBlobAggregate.Stream.CopyTo(ms);
                                                        aggFile.Content = ms.ToArray();
                                                    }
                                                }
                                            }
                                        }
                                        allegatiRisp.Add(aggFile);
                                    }
                                }
                            }
                        }
                        response.Document.Attachments = allegatiRisp.ToArray();
                    }
                }

                #endregion

                await _loggerService.LogOK("DOCUMENTOGETDETTAGLIODOCUMENTO",
                        aggregate.Id, $"PIS REST:Visualizzato il dettaglio del documento con id {aggregate.Id} tramite PIS",
                        null, infoUtente.codWorkingApplication);
                #endregion

                response.Code = GetDocumentResponseCode.OK;

                _logger.LogInformation("end GetDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocumentCommandResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetDocument");
                response = new GetDocumentCommandResponse();
                response.Code = GetDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IDocumentoAmministrativoRepository _darepository;
        protected readonly IDocumentBlobRepository _blobRepository;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly ICAdESService _cAdESService;
        #endregion
    }

}