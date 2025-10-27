// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;

using Pi3.Core.AggregateModels.DocumentBlobAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;
using System.Security.Cryptography;
using System.ComponentModel;
using DocsPaVO.ProfilazioneDinamicaLite;
using Pi3.Core.Extensions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using System.Text;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadBigFileInChunks;
using Pi3.Core.Services.File.Converters;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using System.Xml;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.File.FileValidator;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocument
{
    // Richiede libreria MediatR
    public class UploadFileToDocumentCommandHandler : IRequestHandler<UploadFileToDocumentCommand, UploadFileToDocumentCommandResponse>
    {
        #region Public Members

        public UploadFileToDocumentCommandHandler(ILogger<UploadFileToDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext,
            IWebMethodLoggerService loggerService, IDocumentoAmministrativoRepository docRepository, IDocumentBlobRepository blobRepository,
            ICAdESService cAdESService,
            IPAdESService pAdESService,
            IFileConverterService fileConverterService,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
            this._docRepository = docRepository;
            this._blobRepository = blobRepository;
            this._fileConverterService = fileConverterService;
            this._cAdESService = cAdESService;
            this._pAdESService = pAdESService;
            this._fileValidatorService = fileValidatorService;
        }

        public async Task<UploadFileToDocumentCommandResponse> Handle(UploadFileToDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("UploadFileToDocument - START");

            UploadFileToDocumentCommandResponse response = new UploadFileToDocumentCommandResponse();
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

                if (request.File == null)
                {
                    throw new RestException("REQUIRED_FILE");
                }

                var maxfilesize = DBUtils.maxFileSizePermitted(_pi3DbContext);
                if (maxfilesize > 0 && request.File.Content.Length > maxfilesize)
                {
                    throw new RestException("FILE_SIZE_EXCEEDED");
                }

                using var stream = new MemoryStream(request.File.Content);
                var validateResult = await this._fileValidatorService.Validate(new FileToValidate()
                {
                    Name = request.File.Name,
                    Stream = stream
                });

                if (!validateResult.FormatIsAdmitted)
                    throw new RestException("FILE_FORMAT_NOT_ADMITTED");

                if (string.IsNullOrEmpty(request.Description))
                {
                    throw new RestException("REQUIRED_DESCRIPTION");
                }
                #endregion

                #region implementazione
                string idDocPrincipale = request.IdDocument;
                var verificaIdDocPrincipale = await DBUtils.GetIdDocPrincipale(request.IdDocument, _pi3DbContext);
                if (verificaIdDocPrincipale != null && verificaIdDocPrincipale > 0) idDocPrincipale = verificaIdDocPrincipale.ToString();

                try
                {
                    await _pi3DbContext.AssertSecurityRights(idDocPrincipale, infoUtente.idPeople, infoUtente.idGruppo);
                }
                catch (Exception security) { throw new RestException("DOCUMENT_NOT_FOUND"); }

                DocumentoAmministrativo docAggregate = null;
                try
                {
                    docAggregate = await _docRepository.Get(infoUtente.idAmministrazione, request.IdDocument, new ILoadBehavior[1]
                    {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadProfilesMetadata = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = false,
                        LoadVersions = true,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false,
                    }
                    });
                }
                catch(Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }
                

                var fileAggregate = new DocumentBlob(infoUtente.idAmministrazione, DateTime.Now, new Core.SeedWork.TextValue(request.File.Name));
                byte[] content = request.File.Content;
                string fileName = request.File.Name;
                if (request.CovertToPDFA)
                {
                    fileName = System.IO.Path.GetFileNameWithoutExtension(request.File.Name);
                    try
                    {
                        var convertedFileContent = await this._fileConverterService.Convert(request.File.Name, request.File.Content, FileConverterOutputFormatsEnum.ToPdf);
                        content = convertedFileContent.Content;
                        fileName = $"{fileName}.pdf";
                    }
                    catch(Exception e) 
                    {
                        throw new RestException("FILE_CREATION_ERROR");
                    }
                }
                //fileAggregate.LoadFileName(request.File.Name);

                TipoFirmaEnum tipoFirma = await GetTipoFirmaFile(request.File);

                fileAggregate.UploadStream(new MemoryStream(content), fileName);
                fileAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                if (!request.CreateAttachment)
                {
                    bool creaVersione = false;
                    var versioneAttuale = (from a in _pi3DbContext.ComponentEntities where a.DOCNUMBER == request.IdDocument.AsLong() orderby a.VERSION_ID descending select a).FirstOrDefault();
                    if (versioneAttuale != null) creaVersione = !string.IsNullOrWhiteSpace(versioneAttuale.PATH);
                    try
                    {
                        await _blobRepository.Add(fileAggregate);

                        docAggregate.AssignDocumentBlobRef(new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef()
                        {
                            IdBlob = fileAggregate.Id,
                            //Hash = Encoding.UTF8.GetBytes(RestUtils.CalcolaImpronta256(request.File.Content)),
                            Hash = fileAggregate.Hash,
                            HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                            CreationDate = fileAggregate.CreationDate,
                            ContentType = fileAggregate.ContentType,
                            FileSize = fileAggregate.FileSize,
                            FileName = fileAggregate.FileName,
                            TipoFirma = tipoFirma
                        }, new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior() 
                        { 
                            CreateNewVersion = creaVersione, 
                            Name = new Core.SeedWork.TextValue(request.File.Description), 
                            IdVersion = versioneAttuale?.VERSION_ID.ToString() 
                        });
                    }
                    catch (Exception e)
                    {
                        throw new RestException("FILE_CREATION_ERROR");
                    }

                    await _docRepository.Update(docAggregate);
                    if (creaVersione)
                        await _loggerService.LogOK("DOCUMENTOAGGIUNGIVERSIONE",
                        docAggregate.Id, $"PIS REST: Aggiunta nuova versione per il documento: {docAggregate.Id}",
                        null, infoUtente.codWorkingApplication);
                    else
                        await _loggerService.LogOK("DOCUMENTOPUTFILE",
                        docAggregate.Id, $"PIS REST:Acquisito documento per l'id {docAggregate.Id} tramite PIS",
                        null, infoUtente.codWorkingApplication);
                    response.ResultMessage = $"Acquisito file per il documento: {docAggregate.Id}";
                }
                else
                {
                    if (verificaIdDocPrincipale != null && verificaIdDocPrincipale > 0)
                    {
                        throw new RestException("NO_ATTACH_TO_ATTACH");
                    }
                        
                    var allegato = new DocumentoAmministrativo(infoUtente.idAmministrazione, DateTime.Now, new OggettoDelDocumento() { Descrizione = new Core.SeedWork.TextValue(request.Description) },
                    null,null, TipologieVisibilitaEnum.Gerarchica, new IdDoc() { Identiticativo = request.IdDocument });
                    await _blobRepository.Add(fileAggregate);
                    if (!string.IsNullOrEmpty(request.AttachmentType) && request.AttachmentType == "E")
                    {
                        allegato.ChangeTipologiaAllegato(TipologieAllegatiEnum.SistemiEsterni);
                    }
                    try
                    {

                        allegato.AssignDocumentBlobRef(new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef()
                        {
                            IdBlob = fileAggregate.Id,
                            //Hash = Encoding.UTF8.GetBytes(RestUtils.CalcolaImpronta256(request.File.Content)),
                            Hash = fileAggregate.Hash,
                            HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                            CreationDate = fileAggregate.CreationDate,
                            ContentType = fileAggregate.ContentType,
                            FileSize = fileAggregate.FileSize,
                            FileName = fileAggregate.FileName,
                            TipoFirma = tipoFirma
                        }, new Core.AggregateModels.DocumentAggregate.ValueObjects.TargetVersionBehavior()
                        {
                            CreateNewVersion = true,
                            Name = new Core.SeedWork.TextValue(request.File.Description)
                        });
                        await _docRepository.Add(allegato);
                    }
                    catch (Exception ex)
                    {
                        throw new RestException("FILE_CREATION_ERROR");
                    }
                    
                        
                    await _loggerService.LogOK("DOCNEWALLEGATO",
                        docAggregate.Id, $"PIS REST: Aggiunto nuovo allegato al documento: {docAggregate.Id}",
                        null, infoUtente.codWorkingApplication);
                    response.ResultMessage = $"Aggiunto nuovo allegato al documento: {docAggregate.Id}";
                }
                



                #endregion

                response.Code = UploadFileToDocumentResponseCode.OK;

                _logger.LogInformation("end UploadFileToDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione UploadFileToDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new UploadFileToDocumentCommandResponse();
                response.Code = UploadFileToDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione UploadFileToDocument");
                response = new UploadFileToDocumentCommandResponse();
                response.Code = UploadFileToDocumentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

       

        #endregion

        #region Private Members

        protected readonly ILogger<UploadFileToDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly IDocumentoAmministrativoRepository _docRepository;
        protected readonly IDocumentBlobRepository _blobRepository;
        protected readonly IFileConverterService _fileConverterService;
        protected readonly ICAdESService _cAdESService;
        protected readonly IPAdESService _pAdESService;
        protected readonly IFileValidatorService _fileValidatorService;

        private async Task<TipoFirmaEnum> GetTipoFirmaFile(File fileDoc)
        {
            TipoFirmaEnum tipoFirmaEnum = TipoFirmaEnum.Nessuna;

            if (fileDoc.Name.ToUpper().EndsWith("P7M"))
            {
                tipoFirmaEnum = TipoFirmaEnum.Cades;
            }
            if (fileDoc.Name.ToUpper().EndsWith("TSD"))
            {
                tipoFirmaEnum = TipoFirmaEnum.Tsd;
            }
            if (fileDoc.Name.ToUpper().EndsWith("PDF") && await _pAdESService.IsPAdESFile(new MemoryStream(fileDoc.Content)))
            {
                tipoFirmaEnum = TipoFirmaEnum.Pades;
            }
            if (fileDoc.Name.ToUpper().EndsWith("XML") && await IsSignedXades(fileDoc))
            {
                tipoFirmaEnum = TipoFirmaEnum.Xades;
            }

            return tipoFirmaEnum;
        }
        protected virtual async Task<bool> IsSignedXades(File fileDoc)
        {
            bool result = false;
            XmlDocument Xmlfile = new XmlDocument();
            XmlTextReader tr = new XmlTextReader(new System.IO.MemoryStream(fileDoc.Content));
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