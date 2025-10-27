// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoPutFileNoException;
using Pi3.App.Legacy.WebApi.Application.Models;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentAggregate.Entities;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DocumentoPutFileRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoPutFile;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoPutFile
{
    public class DocumentoPutFileHandler : IRequestHandler<DocumentoPutFileRequest, DocumentoPutFileResult>
    {
        #region Public Members

        public DocumentoPutFileHandler(
            ILogger<DocumentoPutFileNoExceptionHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IFileValidatorService fileValidatorService,
            ISessionRepositoryService sessionRepositoryService,
            ICAdESService cAdESService,
            IPAdESService pAdESService,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._fileValidatorService = fileValidatorService;
            this._sessionRepositoryService = sessionRepositoryService;
            this._cAdESService = cAdESService;
            this._pAdESService = pAdESService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentBlobRepository = documentBlobRepository;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<DocumentoPutFileResult> Handle(DocumentoPutFileRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);
            var groupCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true);

            bool output = false;
            string errorMessage = null!;
            bool wasErrors = false;
            bool digitalSigned = false;
            DocumentVersion version = null!;
            var fileName = Path.GetFileName(request.fileDocument.name);
            var idDocMain = request.fileRequest.docNumber;

            using var stream = new MemoryStream(request.fileDocument.content);

            try
            {
                var fileValidateResult = await this._fileValidatorService.Validate(new FileToValidate()
                {
                    Name = fileName,
                    Stream = stream
                });

                if (!fileValidateResult.FormatIsAdmitted)
                    throw new DocumentoPutFileFormatoFileNonAmmessoPi3Exception(Path.GetExtension(GetExtentionIntoSignedFile(fileName)));

                TipoFirmaEnum tipoFirma = await GetTipoFirmaFile(request.fileDocument);
                digitalSigned = tipoFirma != TipoFirmaEnum.Nessuna;

                if (request.fileRequest.repositoryContext != null)
                {
                    await this._sessionRepositoryService.SetFile(
                        request.fileRequest.repositoryContext,
                        request.fileRequest,
                        request.fileDocument);
                }
                else
                {
                    //Rimosso controllo perchè non aggancia le ricevute da pec
                    //if (!await this._documentoAmministrativoRepository.Exists(idTenant, request.fileRequest.docNumber))
                    //    throw new DocumentoPutFilePi3Exception(ErrorDescriptions.DocumentoAmministrativoNonTrovato);

                    var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(
                        idTenant,
                        request.fileRequest.docNumber, new ILoadBehavior[1]
                        {
                            new GetDocumentoAmministrativoLoadBehavior()
                            {
                                BypassSecurityCheck = true
                            }
                        });

                    idDocMain = documentoAmministrativoAggregate.IdDocPrimario! != null! ?
                                                            documentoAmministrativoAggregate.IdDocPrimario.Identiticativo :
                                                            request.fileRequest.docNumber;

                    if (documentoAmministrativoAggregate.Consolidamento! != null!
                        && documentoAmministrativoAggregate.Consolidamento.Stato == StatiConsolidamentoEnum.Livello2)
                        throw new DocumentoPutFilePi3Exception(ErrorDescriptions.DocumentoAmministrativoConsolidato);

                    if (documentoAmministrativoAggregate.InLibroFirma)
                        throw new DocumentoPutFilePi3Exception(ErrorDescriptions.DocumentoAmministrativoInLibroFirma);

                    //Verifico che per la versione non è stato già acquisito un file
                    var impronta = await _pi3DbContext.ComponentEntities.AsNoTracking()
                                    .Where(c => c.DOCNUMBER == request.fileRequest.docNumber.AsLong()
                                            && c.VERSION_ID == request.fileRequest.versionId.AsLong())
                                    .Select(c => c.VAR_IMPRONTA)
                                    .FirstOrDefaultAsync();
                    if (impronta != null)
                        throw new DocumentoPutFilePi3Exception(ErrorDescriptions.FileGiaAcquisito);

                    var sysdate = await _pi3DbContext.GetSystemDateTime();

                    var documentBlobAggregate = new DocumentBlob(idTenant,
                            sysdate, new TextValue(fileName));

                    stream.Position = 0;
                    documentBlobAggregate.UploadStream(stream, fileName, request.fileDocument.contentType);
                    documentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                    await _documentBlobRepository.Add(documentBlobAggregate);

                    documentoAmministrativoAggregate.AssignDocumentBlobRef(new DocumentBlobRef()
                    {
                        FileName = documentBlobAggregate.FileName,
                        FileSize = documentBlobAggregate.FileSize,
                        CreationDate = await _pi3DbContext.GetSystemDateTime(),
                        ContentType = documentBlobAggregate.ContentType,
                        Hash = documentBlobAggregate.Hash,
                        HashName = Enum.Parse<Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum>(documentBlobAggregate.HashName.ToString(), true),
                        IdBlob = documentBlobAggregate.Id,
                        TipoFirma = tipoFirma,
                        Cartaceo = request.fileRequest.cartaceo
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = (string.IsNullOrWhiteSpace(request.fileRequest.versionId)),
                        IdVersion = (string.IsNullOrWhiteSpace(request.fileRequest.versionId) ? null : request.fileRequest.versionId),
                        //Name = (!string.IsNullOrWhiteSpace(request.fileRequest.descrizione) ? new TextValue(request.fileRequest.descrizione) : null)
                    });

                    await this._documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);

                    request.fileRequest.dataAcquisizione = documentBlobAggregate.CreationDate.AsDateTimeFormat();
                    request.fileRequest.subVersion = "A";
                    request.fileRequest.impronta = BitConverter.ToString(documentBlobAggregate.Hash).Replace("-", string.Empty);
                    request.fileRequest.path = documentBlobAggregate.Id;

                    if (request.fileRequest is DocsPaVO.documento.Documento)
                        ((DocsPaVO.documento.Documento)request.fileRequest).daInviare = 0.ToString();

                    if (string.IsNullOrWhiteSpace(request.fileRequest.versionId))
                        version = documentoAmministrativoAggregate.CurrentVersion;
                    else
                        version = documentoAmministrativoAggregate.Versions.First(v => v.Id == request.fileRequest.versionId);

                    try
                    {
                        // Aggiornamento conformità in tabella InfoFile
                        var infoFileEntity = await this._pi3DbContext.InfoFileEntities
                            .Where(i => i.ID_PROFILE == documentoAmministrativoAggregate.Id.AsLong())
                            .FirstOrDefaultAsync();

                        if (infoFileEntity == null)
                        {
                            infoFileEntity = new InfoFileEntity()
                            {
                                ID_PROFILE = documentoAmministrativoAggregate.Id.AsLong(),
                                ID_DOCUMENTO_PRINCIPALE = (documentoAmministrativoAggregate.IdDocPrimario! != null! ?
                                                            documentoAmministrativoAggregate.IdDocPrimario.Identiticativo.AsLong() :
                                                            null)
                            };

                            this._pi3DbContext.InfoFileEntities.Add(infoFileEntity);
                        }

                        infoFileEntity.DTA_ACQUISIZIONE = DateTime.Now;
                        infoFileEntity.VERSION_ID = version.Id.AsLong();
                        infoFileEntity.VAR_ESTENSIONE = Path.GetExtension(fileName).Replace(".", string.Empty);
                        infoFileEntity.VAR_NOME_FILE = fileName;
                        infoFileEntity.CHA_CONFORME = fileValidateResult.Compliance.IsCompliantToFormat
                                                       && !fileValidateResult.Compliance.HasMacro.GetValueOrDefault()
                                                       && !fileValidateResult.Compliance.HasForms.GetValueOrDefault()
                                                       && !fileValidateResult.Compliance.HasJavascript.GetValueOrDefault() ? "1" : "0";
                        infoFileEntity.CHA_ESTENSIONE_CONFORME = fileValidateResult.Compliance.IsCompliantToFormat ? "1" : "0";
                        infoFileEntity.CHA_PRESENZA_MACRO = fileValidateResult.Compliance.HasMacro.GetValueOrDefault() ? "1" : "0";
                        infoFileEntity.CHA_PRESENZA_FORMS = fileValidateResult.Compliance.HasForms.GetValueOrDefault() ? "1" : "0";
                        infoFileEntity.CHA_PRESENZA_JAVASCRIPT = fileValidateResult.Compliance.HasJavascript.GetValueOrDefault() ? "1" : "0";
                        infoFileEntity.CHA_NOTIFICA = "0";

                        if (infoFileEntity.CHA_CONFORME == "0")
                        {
                            if (!fileValidateResult.Compliance.IsCompliantToFormat)
                                infoFileEntity.VAR_DESC_INFO_FILE = "NON_CONFORME";

                            if (fileValidateResult.Compliance.HasMacro.GetValueOrDefault())
                                infoFileEntity.VAR_DESC_INFO_FILE = string.IsNullOrEmpty(infoFileEntity.VAR_DESC_INFO_FILE) ? "MACRO" : ",MACRO";

                            if (fileValidateResult.Compliance.HasForms.GetValueOrDefault())
                                infoFileEntity.VAR_DESC_INFO_FILE = string.IsNullOrEmpty(infoFileEntity.VAR_DESC_INFO_FILE) ? "FormPDF" : ",FormPDF";

                            if (fileValidateResult.Compliance.HasJavascript.GetValueOrDefault())
                                infoFileEntity.VAR_DESC_INFO_FILE = string.IsNullOrEmpty(infoFileEntity.VAR_DESC_INFO_FILE) ? "JAVASCRIPT" : ",JAVASCRIPT";
                        }


                        await ((Pi3DbContext)this._pi3DbContext).SaveChangesAsync();

                        if (fileName.ToUpper().EndsWith("TSD"))
                        {
                            await _mediator.Send(new Requests.DocumentSaveTimestamp(request.fileDocument, request.fileRequest));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Nel caso in cui non riesca ad aggiornare il record in info file, non ferma l'inserimento
                        this._logger.LogCritical(exception: ex, message: ex.Message);
                    }
                }

                // Aggiornamento oggetto FileRequest
                request.fileRequest.fileName = request.fileDocument.name;
                request.fileRequest.fileSize = request.fileDocument.content.Length.ToString();
                request.fileRequest.subVersion = "A";
                request.fileRequest.firmato = digitalSigned ? "1" : "0";
                request.fileRequest.idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
                request.fileRequest.idPeopleDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);

                output = true;
            }
            catch (Pi3Exception pi3Ex)
            {
                wasErrors = true;
                errorMessage = pi3Ex.Message;

                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                wasErrors = true;
                errorMessage = ex.Message;

                this._logger.LogCritical(exception: ex, message: ex.Message);

                throw;
            }
            finally
            {
                if (wasErrors)
                {
                    output = false;

                    await this._webMethodLoggerService.LogKO(
                        "DOCUMENTOPUTFILE",
                        request.fileRequest.docNumber,
                        string.Format(Descriptions.LogDescriptorFileFirmato, request.fileRequest.docNumber));
                }
                else
                {
                    output = true;

                    await this._webMethodLoggerService.LogOK(
                        "DOCUMENTOPUTFILE",
                        request.fileRequest.docNumber,
                        (digitalSigned ?
                            string.Format(Descriptions.LogDescriptorFileFirmato, request.fileRequest.docNumber) :
                            string.Format(Descriptions.LogDescriptor, request.fileRequest.docNumber)));

                    //traccio l'evento FOLLOW_DOC_EXT_APP
                    var description = request.fileRequest.GetType() == typeof(DocsPaVO.documento.Allegato) ?
                            string.Format(Descriptions.LogFollowAddFileAttachDoc, request.fileRequest.descrizione, idDocMain) :
                            string.Format(Descriptions.LogFollowAddFileDoc, idDocMain);
                    await this._webMethodLoggerService.LogKO("FOLLOWDOCEXTAPP", idDocMain, description);

                }
            }

            return new DocumentoPutFileResult(request.fileRequest);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoPutFileNoExceptionHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IFileValidatorService _fileValidatorService;
        protected readonly ISessionRepositoryService _sessionRepositoryService;
        protected readonly ICAdESService _cAdESService;
        protected readonly IPAdESService _pAdESService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        protected async Task<TipoFirmaEnum> GetTipoFirmaFile(FileDocumento fileDoc)
        {
            TipoFirmaEnum tipoFirmaEnum = TipoFirmaEnum.Nessuna;

            if (fileDoc.name.ToUpper().EndsWith("P7M"))
            {
                tipoFirmaEnum = TipoFirmaEnum.Cades;
            }
            if (fileDoc.name.ToUpper().EndsWith("TSD"))
            {
                tipoFirmaEnum = TipoFirmaEnum.Tsd;
            }
            if (fileDoc.name.ToUpper().EndsWith("PDF") && await _pAdESService.IsPAdESFile(new MemoryStream(fileDoc.content)))
            {
                tipoFirmaEnum = TipoFirmaEnum.Pades;
            }
            if (fileDoc.name.ToUpper().EndsWith("XML") && await IsSignedXades(fileDoc))
            {
                tipoFirmaEnum = TipoFirmaEnum.Xades;
            }

            return tipoFirmaEnum;
        }

        protected string GetExtentionIntoSignedFile(string fileName)
        {
            var filename = fileName;

            if (filename.ToUpper().EndsWith("P7M") ||
                filename.ToUpper().EndsWith("TSD") ||
                filename.ToUpper().EndsWith("M7M"))
            {
                filename = filename.Remove(filename.LastIndexOf("."));

                while (filename.LastIndexOf(".") > -1)
                {
                    if (!filename.ToUpper().EndsWith("P7M") &&
                        !filename.ToUpper().EndsWith("TSD") &&
                        !filename.ToUpper().EndsWith("M7M"))
                        break;

                    filename = filename.Remove(filename.LastIndexOf("."));
                }

                //Vado a rimuovere il (1) aggiunto dai browser
                if (filename.EndsWith(")") && filename.LastIndexOf("(") != -1)
                    filename = filename.Remove(filename.LastIndexOf("("));
            }

            return filename;
        }

        protected virtual async Task<bool> IsSignedXades(FileDocumento fileDoc)
        {
            bool result = false;
            XmlDocument Xmlfile = new XmlDocument();
            XmlTextReader tr = new XmlTextReader(new System.IO.MemoryStream(fileDoc.content));
            tr.XmlResolver = null;
            try
            {
                Xmlfile.Load(tr);
                XmlNodeList signature = Xmlfile.DocumentElement.GetElementsByTagName("ds:Signature");
                if (signature != null && signature.Count > 0)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Errore nel metodo IsSignedXades " + e.Message);
                result = false;
            }
            finally
            {
                tr.Close();
            }

            return result;
        }
        #endregion
    }
}
