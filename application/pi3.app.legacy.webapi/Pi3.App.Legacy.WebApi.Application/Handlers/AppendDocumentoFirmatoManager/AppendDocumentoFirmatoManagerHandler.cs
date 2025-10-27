// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AppendDocumentoFirmatoManagerRequest = Pi3.App.Legacy.WebApi.Application.Requests.AppendDocumentoFirmatoManager;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AppendDocumentoFirmatoManager
{

    public class AppendDocumentoFirmatoManagerHandler : IRequestHandler<AppendDocumentoFirmatoManagerRequest, AppendDocumentoFirmatoManagerResult>
    {
        #region Public Members

        public AppendDocumentoFirmatoManagerHandler(ILogger<AppendDocumentoFirmatoManagerHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentBlobRepository documentBlobRepository,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentBlobRepository = documentBlobRepository;
            this._fileValidatorService = fileValidatorService;

            this.InitializeMapper();
        }

        public async Task<AppendDocumentoFirmatoManagerResult> Handle(AppendDocumentoFirmatoManagerRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var groupId = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);

            DocsPaVO.documento.FileRequest fileRequest = request.fileRequest;

            bool isConvertedToPdf = false;

            try
            {
                var versionIdOld = fileRequest.versionId.AsLong();

                if (fileRequest.repositoryContext == null)
                {
                    // Verifica stato di consolidamento del documento, solamente se non si sta firmando nel repository context
                    bool canExecuteAction = await this.CanExecuteAction(fileRequest.docNumber, ConsolidationActionsDeniedEnum.SignDocument, true);
                }

                if (fileRequest != null && !String.IsNullOrEmpty(fileRequest.fileName) && (fileRequest.fileName.ToLower().EndsWith("pdf_convertito") || request.isConvertedToPdf))
                {
                    fileRequest.fileName = System.IO.Path.GetFileNameWithoutExtension(fileRequest.fileName) + ".pdf";
                    isConvertedToPdf = true;
                }

                if (!await this.IsFormatSupportedForSign(Convert.ToInt32(idTenant), fileRequest))
                    throw new FileNotSupportedPi3Exception();

                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();

                fileDoc.content = request.signedContent;
                fileDoc.length = fileDoc.content.Length;
                string nomeOriginale = RemoveIllegalChars(await this._dbContext.ComponentEntities
                                            .Where(x => x.VERSION_ID == fileRequest.versionId.AsLong() && x.DOCNUMBER == fileRequest.docNumber.AsLong())
                                            .Select(x => x.VAR_NOMEORIGINALE)
                                            .FirstOrDefaultAsync() ?? string.Empty, false); 

                if (request.isPades)
                {
                    if (!string.IsNullOrEmpty(nomeOriginale))
                    {
                        if ((System.IO.Path.GetExtension(fileRequest.fileName).ToUpper() == ".PDF") &&
                            (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF") && isConvertedToPdf)
                            nomeOriginale += ".PDF";

                        fileDoc.nomeOriginale = nomeOriginale;
                    }

                    fileDoc.estensioneFile = GetAppSuffix(fileRequest.fileName);
                    fileDoc.name = fileRequest.fileName;
                    app.estensione = GetAppSuffix(fileRequest.fileName);
                }
                else
                {
                    if (request.cofirma && System.IO.Path.GetExtension(nomeOriginale).ToUpper().Equals(".P7M"))
                    {
                        app.estensione = GetAppSuffix(fileRequest.fileName);
                        fileDoc.name = fileRequest.fileName;

                        if (!string.IsNullOrEmpty(nomeOriginale))
                        {
                            //se il filename finisce PDF probabilmente è stato convertito.
                            //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                            if ((System.IO.Path.GetExtension(fileRequest.fileName).ToUpper() == ".PDF") &&
                                (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF"))
                                nomeOriginale += ".PDF";

                            fileDoc.nomeOriginale = nomeOriginale;
                        }
                        fileDoc.estensioneFile = GetAppSuffix(fileRequest.fileName);
                    }
                    else
                    {
                        app.estensione = GetAppSuffix(fileRequest.fileName + ".P7M");
                        fileDoc.name = fileRequest.fileName + ".P7M";

                        if (!string.IsNullOrEmpty(nomeOriginale))
                        {
                            //se il filename finisce PDF probabilmente è stato convertito.
                            //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                            if ((System.IO.Path.GetExtension(fileRequest.fileName).ToUpper() == ".PDF") &&
                                (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF"))
                                nomeOriginale += ".PDF";

                            fileDoc.nomeOriginale = nomeOriginale + ".P7M";
                        }
                        fileDoc.estensioneFile = GetAppSuffix(fileRequest.fileName + ".p7m");
                    }
                }

                fileDoc.fullName = fileDoc.name;

                var isAllegato = (fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)));
                var addNewAttatchment = (isAllegato && fileRequest.repositoryContext != null);

                if (addNewAttatchment)
                {
                    fileRequest.docNumber = await this._dbContext.ProfileEntities.AsNoTracking()
                                            .Where(x => x.DOCNUMBER == fileRequest.docNumber.AsLong())
                                            .Select(x => x.ID_DOCUMENTO_PRINCIPALE.ToString())
                                            .FirstOrDefaultAsync() ?? string.Empty;

                    fileRequest.descrizione = Resources.SignedAttachment;
                    fileRequest.cartaceo = false;
                    fileRequest = (await this._mediator.Send(new Application.Requests.DocumentoAggiungiAllegato(new InfoUtente(), (DocsPaVO.documento.Allegato)fileRequest))).output;

                    if (fileRequest == null)
                        throw new SignedAttachmentCreationErrorException();
                }
                else
                {
                    fileRequest.applicazione = app;
                    fileRequest.versionId = "";
                    fileRequest.descrizione = Resources.SignedVersion;
                    fileRequest.cartaceo = false;
                    fileRequest = (await this._mediator.Send(new Application.Requests.DocumentoAggiungiVersione(fileRequest, new InfoUtente()))).output;

                    if (fileRequest == null)
                        throw new SignedVersionCreationErrorException();

                    await this.SetDataFirmaDocumento(fileRequest.docNumber, fileRequest.versionId);

                    if (!isAllegato)
                    {
                        ((DocsPaVO.documento.Documento)fileRequest).daInviare = "1";
                    }
                   
                }

                var firmaEntities = await this._dbContext.FirmaElettronicaEntities.AsNoTracking()
                        .Where(x => x.VERSION_ID == versionIdOld && !string.IsNullOrEmpty(x.XML))                        
                        .OrderBy(x => x.ID_FIRMA)
                        .ToListAsync();

                var firmaE = _mapper.Map<List<FirmaElettronica>>(firmaEntities);

                bool isFirmatoElettonicamente = firmaE != null && firmaE.Count > 0;
                fileRequest.tipoFirma = isFirmatoElettonicamente ? DocsPaVO.documento.TipoFirma.ELETTORNICA : fileRequest.tipoFirma;

                DocumentoAmministrativo aggregato = await this._documentoAmministrativoRepository.Get(idTenant.ToString(), fileRequest.docNumber, new ILoadBehavior[1]
                {
                new GetDocumentoAmministrativoLoadBehavior()
                {
                    LoadProfiles = false,
                    LoadClassifications = false,
                    LoadAllegati = false,
                    LoadAggregazioni = false,
                    LoadVersions = true,
                    LoadPermissions = false,
                    LoadMittentiDestinatari = true,
                    LoadKeywords = false,
                    LoadNote = false
                }
                });

                var newDocumentBlobAggregate = new DocumentBlob(idTenant.ToString(), DateTime.Now, new TextValue(fileDoc.name));
                newDocumentBlobAggregate.UploadStream(new MemoryStream(fileDoc.content), fileDoc.name);
                newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                await _documentBlobRepository.Add(newDocumentBlobAggregate);

                aggregato.AssignDocumentBlobRef(
                    new DocumentBlobRef()
                    {
                        IdBlob = newDocumentBlobAggregate.Id,
                        FileName = newDocumentBlobAggregate.FileName,
                        ContentType = newDocumentBlobAggregate.ContentType,
                        FileSize = newDocumentBlobAggregate.FileSize,
                        CreationDate = await _dbContext.GetSystemDateTime(),
                        Hash = newDocumentBlobAggregate.Hash,
                        HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256,
                        Cartaceo = false,
                        SegnaturaPermanente = false,
                        TipoFirma = request.isPades ? TipoFirmaEnum.Pades : TipoFirmaEnum.Cades
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = false,
                        IdVersion = fileRequest.versionId,
                        Name = new TextValue(Resources.SignedVersion)
                    }
                    );
             
                await this._documentoAmministrativoRepository.Update(aggregato);

                fileRequest.firmato = "1";

                var fileValidateAllegatoResult = await this._fileValidatorService.Validate(new FileToValidate()
                {
                    Name = newDocumentBlobAggregate.FileName,
                    Stream = newDocumentBlobAggregate.Stream
                });

                await _mediator.Send(new Requests.DocumentoAddInfoFileRequest(new DocsPaVO.documento.FileRequest()
                {
                    docNumber = aggregato.Id,
                    versionId = aggregato.CurrentVersion.Id,
                    fileName = newDocumentBlobAggregate.FileName,
                    dataAcquisizione = newDocumentBlobAggregate.CreationDate.AsDateTimeFormat()
                },
                aggregato.IdDocPrimario?.Identiticativo.AsLong(),
                fileValidateAllegatoResult));

                if (isFirmatoElettonicamente)
                {
                    long versionId = fileRequest.versionId.AsLong();
                    long docNumber = fileRequest.docNumber.AsLong();
                    string impronta = await this._dbContext.ComponentEntities
                        .Where(x => x.VERSION_ID == versionId && x.DOCNUMBER == docNumber)
                        .Select(x => x.VAR_IMPRONTA)
                        .FirstOrDefaultAsync() ?? string.Empty;

                    var newFirmaElettronicaEntity = new List<FirmaElettronicaEntity>();
                    foreach (var firma in firmaE)
                    {
                        firma.UpdateXml(impronta, fileRequest.versionId, fileRequest.version, fileRequest.docNumber);

                        newFirmaElettronicaEntity.Add(new FirmaElettronicaEntity()
                        {
                            ID_DOCUMENTO = firma.Docnumber.AsLong(),
                            VERSION_ID = firma.Versionid.AsLong(),
                            DOC_ALL = firma.DocAll,
                            NUM_ALL = firma.NumAll.AsLong(),
                            NUMERO_VERSIONE = firma.NumVersione.AsLong(),
                            DATA_APPOSIZIONE = DateTime.Now
                        });
                    }

                    await this._dbContext.FirmaElettronicaEntities.AddRangeAsync(newFirmaElettronicaEntity);
                    await ((Pi3DbContext)_dbContext).SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = false;
            }

            return new AppendDocumentoFirmatoManagerResult(output, fileRequest);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AppendDocumentoFirmatoManagerHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IFileValidatorService _fileValidatorService;

        protected IMapper _mapper = null;
        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FirmaElettronicaEntity, FirmaElettronica>()
                    .ForMember(dest => dest.IdFirma, opt => opt.MapFrom(src => src.ID_FIRMA))
                    .ForMember(dest => dest.Docnumber, opt => opt.MapFrom(src => src.ID_DOCUMENTO))
                    .ForMember(dest => dest.Versionid, opt => opt.MapFrom(src => src.VERSION_ID))
                    .ForMember(dest => dest.DocAll, opt => opt.MapFrom(src => src.DOC_ALL))
                    .ForMember(dest => dest.NumAll, opt => opt.MapFrom(src => src.NUM_ALL.ToString() ?? string.Empty))
                    .ForMember(dest => dest.NumVersione, opt => opt.MapFrom(src => src.XML ?? string.Empty))
                    .ForMember(dest => dest.DataApposizione, opt => opt.MapFrom(src => src.DATA_APPOSIZIONE.AsDateTimeFormat()));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected string RemoveIllegalChars(string filename, bool normalizeDotsAndSpacesToo)
        {

            if (string.IsNullOrEmpty(filename))
                return filename;

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
                filename = filename.Replace(c.ToString(), "_");

            if (normalizeDotsAndSpacesToo)
            {
                filename = filename.Replace(".", "_");
                filename = filename.Replace(" ", "_");
            }
            return filename;
        }

        protected async Task<bool> CanExecuteAction(string idProfile, Enum action, bool throwOnError)
        {
            long idProfileAsLong = idProfile.AsLong();
            DocsPaVO.documento.DocumentConsolidationStateInfo actualState = await GetState(idProfileAsLong);
            if (actualState != null && actualState.State == DocsPaVO.documento.DocumentConsolidationStateEnum.None)
                return true;

            DocsPaVO.documento.DocumentConsolidationStateEnum actionApplyState = DocumentConsolidationAttribute.GetState(action);

            bool canExecute = (actualState?.State < actionApplyState);

            if (!canExecute && throwOnError)
                throw new ConsolidatedStatePi3Exception();

            return canExecute;
        }

        protected async Task<bool> SetDataFirmaDocumento(string docNumber, string versionId)
        {
            var newInfoFirmaDigitaleEntity = new InfoFirmaDigitaleEntity()
            {
                ID_PROFILE = docNumber.AsLong(),
                VERSION_ID = versionId.AsLong(),
                DATA_APPOSIZIONE = DateTime.Now
            };

            await this._dbContext.InfoFirmaDigitaleEntities.AddAsync(newInfoFirmaDigitaleEntity);
            int rowsInserted = await ((Pi3DbContext)_dbContext).SaveChangesAsync();

            return rowsInserted > 0;
        }

        protected async Task<bool> IsFormatSupportedForSign(int idTenant, DocsPaVO.documento.FileRequest? fileRequest)
        {
            bool retValue = false;
            //In PiTre la chiave SUPPORTED_FILE_TYPES_ENABLED è sempre abilita per cui evito il controllo presente sul vecchio BE
            string extension = System.IO.Path.GetExtension(fileRequest?.fileName) ?? string.Empty;

            if (!string.IsNullOrEmpty(extension))
            {
                // Rimozione del primo carattere dell'estensione (punto)
                extension = extension.Substring(1);

                DocsPaVO.FormatiDocumento.SupportedFileType fileType = (await this._mediator.Send(new Application.Requests.GetSupportedFileType(idTenant, extension))).output;

                retValue = (fileType != null && fileType.FileTypeUsed && fileType.FileTypeSignature);
            }

            return retValue;
        }

        protected async Task<DocumentConsolidationStateInfo> GetState(long idProfile)
        {

            var state = await this._dbContext.ProfileEntities.Where(x => x.SYSTEM_ID == idProfile).Select(x => new
            {
                CONSOLIDATION_STATE = x.CONSOLIDATION_STATE,
                CONSOLIDATION_AUTHOR = x.CONSOLIDATION_AUTHOR,
                CONSOLIDATION_ROLE = x.CONSOLIDATION_ROLE,
                CONSOLIDATION_DATE = x.CONSOLIDATION_DATE
            }).FirstOrDefaultAsync();

            var stateInfo = new DocumentConsolidationStateInfo()
            {
                State = !string.IsNullOrEmpty(state?.CONSOLIDATION_STATE) ? (DocsPaVO.documento.DocumentConsolidationStateEnum)Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum), state.CONSOLIDATION_STATE, true)
                    : DocumentConsolidationStateEnum.None,
                Author = state.CONSOLIDATION_AUTHOR?.ToString() ?? string.Empty,
                Role = state.CONSOLIDATION_ROLE?.ToString() ?? string.Empty,
                Date = state.CONSOLIDATION_DATE?.ToString() ?? string.Empty
            };


            return stateInfo;
        }

        protected string GetAppSuffix(string fileName)
        {
            char[] dot = { '.' };
            string[] parts = fileName.Split(dot);
            string suffix = parts[parts.Length - 1];
            if (suffix.ToUpper().Equals("P7M"))
            {
                string res = "";
                int index = 1;
                while (suffix.ToUpper().Equals("P7M"))
                {
                    index = index + 1;
                    res = ".P7M" + res;
                    suffix = parts[parts.Length - index];
                }
                res = suffix + res;
                return res;
            }
            else
                return suffix;
        }

        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        private class DocumentConsolidationAttribute : Attribute
        {
            public DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum state)
            {
                this.State = state;
            }

            public DocsPaVO.documento.DocumentConsolidationStateEnum State
            {
                get;
                set;
            }

            public static DocsPaVO.documento.DocumentConsolidationStateEnum GetState(Enum enumValue)
            {
                FieldInfo fi = enumValue?.GetType().GetField(enumValue.ToString());

                DocumentConsolidationAttribute[] attributes = (DocumentConsolidationAttribute[])
                        fi.GetCustomAttributes(typeof(DocumentConsolidationAttribute), false);

                if (attributes.Length > 0)
                    return attributes[0].State;
                else
                    return DocsPaVO.documento.DocumentConsolidationStateEnum.None;
            }
        }

        private enum ConsolidationActionsDeniedEnum
        {
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            AddVersions,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            RemoveVersions,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            ModifyVersions,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            AddAttatchments,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            RemoveAttatchments,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            ModifyAttatchments,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            DeleteDocument,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            SignDocument,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            PrepareProtocol,            // Predisponi alla protocollazione
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)]
            CancelProtocol,             // Annullamento protocollo
        }
        #endregion
    }

}
