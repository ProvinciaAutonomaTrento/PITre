// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using Pi3.Core.Extensions;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using DocsPaVO.utente;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.Services.WebMethodLogger;
using System.Text;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using StackExchange.Redis;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadBigFileInChunks
{
    // Richiede libreria MediatR
    public class UploadBigFileInChunksCommandHandler : IRequestHandler<UploadBigFileInChunksCommand, UploadBigFileInChunksCommandResponse>
    {
        #region Public Members

        public UploadBigFileInChunksCommandHandler(
            ILogger<UploadBigFileInChunksCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext,
            IConfigurationService configurationService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IWebMethodLoggerService loggerService,
            IDocumentBlobRepository documentBlobRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._configurationService = configurationService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._loggerService = loggerService;
            this._documentBlobRepository = documentBlobRepository;
        }

        public async Task<UploadBigFileInChunksCommandResponse> Handle(UploadBigFileInChunksCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("UploadBigFileInChunks - START");

            UploadBigFileInChunksCommandResponse response = new UploadBigFileInChunksCommandResponse();
            try
            {
                #region token e autenticazione
                bool docFound = false;
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


                #region checking document if exists
                if (!string.IsNullOrEmpty(request.IdDocument))
                {
                    try
                    {
                        docFound = await this.WasDocumentFound(request.IdDocument);
                    }
                    catch (Exception ex)
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }
                    if (!docFound)
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }
                }
                #endregion

                #region computing directory path

                var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

                var folderPath = Path.Combine(
                            repositoryRootPath,
                            "DocServer",
                             tenantCode,
                            "TemporaryUploads",
                            infoUtente.idAmministrazione, infoUtente.idPeople, request.IdDocument).PathAsUnixPath();

                #endregion

                var computedPhase = request.Phase.ToUpper().Trim();
                if (computedPhase.StartsWith("END_CREATE_ATTACHMENT"))
                {
                    computedPhase = "END_CREATE_ATTACHMENT";
                }

                if (docFound)
                {
                    switch (computedPhase)  
                    {
                        case "PREPARE_ATTACHMENT":
                            response.ResultMessage = await this.PrepareAttachmentPhase(request,infoUtente);
                            break;
                        case "END_CREATE_ATTACHMENT":
                            response.ResultMessage = await this.EndAttachmentPhase(request,infoUtente,folderPath);
                            break;
                        case "END":
                            response.ResultMessage = await this.EndPhase(request,infoUtente,folderPath);
                            break;
                        default:
                            response.ResultMessage = this.StartOrChunkPhase(request,folderPath);
                            break;
                    }
                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }


                #endregion

                response.Code = MessageResponseCode.OK;

                _logger.LogInformation("end UploadBigFileInChunks");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione UploadBigFileInChunks: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new UploadBigFileInChunksCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione UploadBigFileInChunks");
                response = new UploadBigFileInChunksCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<UploadBigFileInChunksCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;

        private async Task<bool> WasDocumentFound(string IdDocument)
        {
            bool found = false;
            var getDocumentReq = new GetDocumentCommand()
            {
                IdDocument = IdDocument
            };
                
            var documentResult = await this._mediator.Send(getDocumentReq);

            if(documentResult.Document != null && !string.IsNullOrWhiteSpace(documentResult.Document.Id))
            {
                found = true;
            }

            return found;
        }

        #region validation utils
        private void AssertRequestWithStartOrChunkPhase(UploadBigFileInChunksCommand request)
        {
            if (request.ChunkNumber == null) throw new RestException("MISSING_PARAMETER");
            if (request.ChunkContent == null) throw new RestException("REQUIRED_FILE");
        }

        private void AsssertRequestWithAttachmentPhases(UploadBigFileInChunksCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
                throw new RestException("MISSING_PARAMETER");
        }
        #endregion


        private DocumentoAmministrativo BuildAttachment(string description, string idDoc)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            return new DocumentoAmministrativo(idTenant, DateTime.Now,
                new OggettoDelDocumento()
                {
                    Descrizione = new Core.SeedWork.TextValue(description)
                },
                null,
                null,
                TipologieVisibilitaEnum.Gerarchica,
                new IdDoc()
                {
                    Identiticativo = idDoc
                });
        }

        private string StartOrChunkPhase(UploadBigFileInChunksCommand request,string folderPath)
        {
            // Cancello la cartella creata nello start
            if (!string.IsNullOrWhiteSpace(request.Phase) && 
                request.Phase.ToUpper().Trim() == "START" && 
                System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.Delete(folderPath, true);
            }
            if(!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            this.AssertRequestWithStartOrChunkPhase(request);

            var filePath = System.IO.Path.Combine(folderPath, string.Concat(request.FileName, ".part")).PathAsUnixPath();

            using (var fs = new FileStream(filePath,FileMode.Append, FileAccess.Write, FileShare.None))

            using (var bw = new BinaryWriter(fs))
                bw.Write(request.ChunkContent);

            return string.Format(Messages.ChunkMessage, request.ChunkNumber, request.IdDocument);
        }


        private async Task<string> PrepareAttachmentPhase(UploadBigFileInChunksCommand request,InfoUtente infoUtente)
        {
            string resultMsg = string.Empty;

            this.AsssertRequestWithAttachmentPhases(request);
            var allegato = this.BuildAttachment(request.FileName,request.IdDocument);

            try
            {
                await _documentoAmministrativoRepository.Add(allegato);
                resultMsg = string.Format(Messages.AddAttachmentSuccess,request.IdDocument, allegato.Id);
                await this._loggerService.LogOK("DOCNEWALLEGATO",
                    request.IdDocument,
                    string.Format(Messages.LogMessageAddAttachmentOK,request.IdDocument),
                    null, 
                    infoUtente.codWorkingApplication
                    );
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: string.Format(Messages.AddAttachmentExc,ex));
            }



            return resultMsg;
        }


        private async Task<string> EndAttachmentPhase(UploadBigFileInChunksCommand request, InfoUtente infoUtente, 
            string folderPath)
        {
            string resultMsg = string.Empty;

            this.AsssertRequestWithAttachmentPhases(request);
            string descAllegato = request.FileName;
            DocumentoAmministrativo allegato = null;

            if (request.Phase.ToUpper().Contains("END_CREATE_ATTACHMENT_NAME:"))
            {
                string tempDescAllegato = request.Phase.Replace("END_CREATE_ATTACHMENT_NAME:", "");
                if (!string.IsNullOrWhiteSpace(tempDescAllegato))
                {
                    descAllegato = tempDescAllegato;
                }
            }

            var paths = System.IO.Directory.GetFiles(folderPath);
            var filePath = System.IO.Path.Combine(folderPath, request.FileName);
            if(paths.Length > 0)
            {
                System.IO.File.Move(string.Concat(filePath, ".part").PathAsUnixPath(), filePath.PathAsUnixPath());
            }

            var documentBlobAggregate = new DocumentBlob(infoUtente.idAmministrazione, DateTime.Now, new Core.SeedWork.TextValue(request.FileName));
            documentBlobAggregate.UploadStream(new MemoryStream(System.IO.File.ReadAllBytes(filePath)), request.FileName);
            documentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

            try
            {
                await _documentBlobRepository.Add(documentBlobAggregate);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: string.Format(Messages.EndAttachmentPhaseExc, ex.Message));
                throw new RestException("FILE_CREATION_ERROR");
            }

            if (!string.IsNullOrWhiteSpace(request.Hash))
            {
                var hash = BitConverter.ToString(documentBlobAggregate.Hash).Replace("-", string.Empty).ToLowerInvariant();
                if (hash != request.Hash)
                {
                    throw new Exception(Messages.HashExc);
                }
            }

            try
            {
                allegato = this.BuildAttachment(descAllegato, request.IdDocument);
                allegato.AssignDocumentBlobRef(new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef()
                {
                    IdBlob = documentBlobAggregate.Id,
                    Hash = documentBlobAggregate.Hash,
                    HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                    CreationDate = documentBlobAggregate.CreationDate,
                    ContentType = documentBlobAggregate.ContentType,
                    FileSize = documentBlobAggregate.FileSize,
                    FileName = documentBlobAggregate.FileName
                },
                new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior()
                {
                    CreateNewVersion = true,
                    Name = new Core.SeedWork.TextValue(request.FileName)
                }
                );

                await this._documentoAmministrativoRepository.Add(allegato);

                System.IO.Directory.Delete(folderPath, true);
                resultMsg = string.Format(Messages.AddAttachmentRes, request.IdDocument, allegato.Id);
                await this._loggerService.LogOK("DOCNEWALLEGATO",
                    request.IdDocument,
                    string.Format(Messages.LogMessageAddAttachmentOK, request.IdDocument),
                    null,
                    infoUtente.codWorkingApplication
                    );

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: string.Format(Messages.EndAttachmentPhaseExc,ex.Message));
                throw new RestException("FILE_CREATION_ERROR");
            }


            return resultMsg;
        }

        private async Task<string> EndPhase(UploadBigFileInChunksCommand request, InfoUtente infoUtente,
            string folderPath)
        {
            string resultMsg = string.Empty;
            this.AsssertRequestWithAttachmentPhases(request);

            var paths = System.IO.Directory.GetFiles(folderPath);
            var filePath = System.IO.Path.Combine(folderPath, request.FileName);
            if (paths.Length > 0)
            {
                System.IO.File.Move(string.Concat(filePath, ".part").PathAsUnixPath(), filePath.PathAsUnixPath());
            }
            try
            {
                var documentBlobAggregate = new DocumentBlob(infoUtente.idAmministrazione, DateTime.Now, new Core.SeedWork.TextValue(request.FileName));
                documentBlobAggregate.UploadStream(new MemoryStream(System.IO.File.ReadAllBytes(filePath)), request.FileName);
                documentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                await _documentBlobRepository.Add(documentBlobAggregate);

                if (!string.IsNullOrWhiteSpace(request.Hash))
                {
                    var hash = BitConverter.ToString(documentBlobAggregate.Hash).Replace("-", string.Empty).ToLowerInvariant();
                    if (hash != request.Hash)
                    {
                        throw new Exception(Messages.HashExc);
                    }
                }

                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(
                    infoUtente.idAmministrazione,
                    request.IdDocument,
                    new ILoadBehavior[1]
                        {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadProfilesMetadata = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false,
                    }
                        });

                documentoAmministrativoAggregate.AssignDocumentBlobRef(new DocumentBlobRef()
                {
                    FileName = documentBlobAggregate.FileName,
                    FileSize = documentBlobAggregate.FileSize,
                    CreationDate = await _pi3DbContext.GetSystemDateTime(),
                    ContentType = documentBlobAggregate.ContentType,
                    Hash = documentBlobAggregate.Hash,
                    HashName = Enum.Parse<Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum>(documentBlobAggregate.HashName.ToString(), true),
                    IdBlob = documentBlobAggregate.Id,
                },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = true,
                        IdVersion = null,
                        Name = new TextValue("Nuova versione big file")
                    });

                await this._documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
                System.IO.Directory.Delete(folderPath, true);

                resultMsg = string.Format(Messages.AddVersionSuccess, request.IdDocument);
                await this._loggerService.LogOK("DOCUMENTOAGGIUNGIVERSIONE",
                    request.IdDocument,
                    string.Format(Messages.LogMessageAddVersionOK, request.IdDocument),
                    null,
                    infoUtente.codWorkingApplication
                    );
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: string.Format(Messages.EndExc, ex.Message));
                throw new RestException("FILE_CREATION_ERROR");
            }
            

            return resultMsg;
        }

        #endregion
    }

}