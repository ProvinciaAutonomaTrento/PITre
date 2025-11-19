// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Text;
using System.Xml;
using CreateTSDVersionRequest = Pi3.App.Legacy.WebApi.Application.Requests.CreateTSDVersion;
using DocsPaVO.utente;
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.WebMethodLogger;
using System.Globalization;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using DocsPaVO.LibroFirma;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.File.MarcaTemporale;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CreateTSDVersion
{
    public class CreateTSDVersionHandler : IRequestHandler<CreateTSDVersionRequest, CreateTSDVersionResult>
    {
        #region Public Members

        public CreateTSDVersionHandler(ILogger<CreateTSDVersionHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext, IConfigurationService configurationService,
            IFileValidatorService fileValidatorService, IWebMethodLoggerService webMethodLoggerService,
            IFirmaDigitale2Service firmaDigitale2Service, IMarcaTemporaleService marcaTemporaleService,
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
            this._fileValidatorService = fileValidatorService;
            this._firmaDigitale2Service = firmaDigitale2Service;
            this._marcaTemporaleService = marcaTemporaleService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentBlobRepository = documentBlobRepository;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;

            this.InitializeMapper();
        }

        public async Task<CreateTSDVersionResult> Handle(CreateTSDVersionRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.FileRequest tsdFileReq = null;
            DocsPaVO.documento.FileRequest fileRequest = request.fileRequest;
            DocsPaVO.utente.InfoUtente infoUtente = request.infoUtente;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var groupId = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);


            try
            {
                byte[] lastTSR = await this.GetTSRForDocument(fileRequest);
                if (lastTSR == null)
                    return null;

                //Conversione in TSD
                DocsPaVO.documento.FileDocumento docFile = (await this._mediator.Send(new Requests.DocumentoGetFileFirmato(fileRequest, infoUtente))).output; //FileManager.getFileFirmato(fileRequest, infoUtente, false);

                if ((await this.Verify(docFile.content, lastTSR)).esito != "OK")
                    return null;

                MarcaTsdRequest tsdRequest = new MarcaTsdRequest()
                {
                    FileDaMarcare = new FileMarcatura() { FileBase64 = docFile.content, FileName = docFile.name }
                };

                MarcaTsdResponse tsdResponse = await this._marcaTemporaleService.MarcaTsd(tsdRequest);

                if (tsdResponse == null)
                    this._logger.LogError("CreateTSDVersion > MarcaTsdResponse nulla");
                else
                {
                    docFile.content = tsdResponse.FileMarcato.FileBase64;
                    docFile.estensioneFile = "tsd";
                    docFile.fullName = docFile.fullName + ".tsd";
                    docFile.length = (int)docFile.content.Length;
                    docFile.name = docFile.name + ".tsd";
                    docFile.nomeOriginale = docFile.nomeOriginale + ".tsd";

                    //DocsPaVO.documento.FileRequest tsdFileReq;
                    if (fileRequest.GetType() == typeof(DocsPaVO.documento.Allegato))
                    {
                        tsdFileReq = new DocsPaVO.documento.Allegato();
                        (tsdFileReq as DocsPaVO.documento.Allegato).numeroPagine = (fileRequest as DocsPaVO.documento.Allegato).numeroPagine;
                    }
                    else
                        tsdFileReq = new DocsPaVO.documento.Documento();

                    tsdFileReq.docNumber = fileRequest.docNumber;
                    tsdFileReq.descrizione = Resources.TSDNewVersionDecription;
                    tsdFileReq.firmato = fileRequest.firmato;
                    tsdFileReq.tipoFirma = fileRequest.tipoFirma;

                    DocumentoAmministrativo aggregato = await this._documentoAmministrativoRepository.Get(idTenant.ToString(), fileRequest.docNumber, new ILoadBehavior[1]
                    {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadProfiles = true,
                            LoadClassifications = true,
                            LoadAllegati = true,
                            LoadAggregazioni = true,
                            LoadVersions = true,
                            LoadPermissions = true,
                            LoadMittentiDestinatari = true,
                            LoadKeywords = true,
                            LoadNote = true
                        }
                    });

                    var newDocumentBlobAggregate = new DocumentBlob(idTenant.ToString(), DateTime.Now, new TextValue(docFile.name));
                    newDocumentBlobAggregate.UploadStream(new MemoryStream(docFile.content), docFile.name);
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
                            TipoFirma = TipoFirmaEnum.Tsd
                        },
                        new TargetVersionBehavior()
                        {
                            CreateNewVersion = true,
                            IdVersion = tsdFileReq.versionId,
                            Name = new TextValue(Resources.TSDNewVersionDecription)
                        });

                    await this._documentoAmministrativoRepository.Update(aggregato);

                    tsdFileReq.version = aggregato.Versions[aggregato.Versions.Count - 1].VersionNumber.ToString();
                    tsdFileReq.versionId = aggregato.Versions[aggregato.Versions.Count - 1].Id.ToString();

                    try
                    {
                        // Aggiornamento conformit� in tabella InfoFile
                        var infoFileEntity = await this._dbContext.InfoFileEntities
                            .Where(i => i.ID_PROFILE == aggregato.Id.AsLong())
                            .FirstOrDefaultAsync();

                        if (infoFileEntity == null)
                        {
                            infoFileEntity = new InfoFileEntity()
                            {
                                ID_PROFILE = aggregato.Id.AsLong(),
                                ID_DOCUMENTO_PRINCIPALE = (aggregato.IdDocPrimario! != null! ?
                                                            aggregato.IdDocPrimario.Identiticativo.AsLong() :
                                                            null)
                            };

                            this._dbContext.InfoFileEntities.Add(infoFileEntity);
                        }

                        var fileName = Path.GetFileName(docFile.name);
                        using var stream = new MemoryStream(docFile.content);
                        var fileValidateResult = await this._fileValidatorService.Validate(new FileToValidate()
                        {
                            Name = fileName,
                            Stream = stream
                        });

                        infoFileEntity.DTA_ACQUISIZIONE = DateTime.Now;
                        infoFileEntity.VERSION_ID = tsdFileReq.versionId.AsLong();
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

                        await ((Pi3DbContext)this._dbContext).SaveChangesAsync();

                        await _mediator.Send(new Requests.DocumentSaveTimestamp(docFile, tsdFileReq));

                    }
                    catch (Exception ex)
                    {
                        this._logger.LogCritical(exception: ex, message: ex.Message);
                    }

                    List<DocsPaVO.LibroFirma.FirmaElettronica> firmaE = await this.GetFirmaElettronicaDaFileRequest(fileRequest);

                    bool isFirmatoElettonicamente = firmaE != null && firmaE.Count > 0;
                    if (isFirmatoElettonicamente)
                    {
                        long versionId = tsdFileReq.versionId.AsLong();
                        long docNumber = tsdFileReq.docNumber.AsLong();
                        string impronta = await this._dbContext.ComponentEntities
                            .Where(x => x.VERSION_ID == versionId && x.DOCNUMBER == docNumber)
                            .Select(x => x.VAR_IMPRONTA)
                            .FirstOrDefaultAsync() ?? string.Empty;

                        foreach (DocsPaVO.LibroFirma.FirmaElettronica firma in firmaE)
                        {
                            firma.UpdateXml(impronta, tsdFileReq.versionId, tsdFileReq.version, tsdFileReq.docNumber);
                            await this.InserisciFirmaElettronica(firma);
                        }
                    }

                    if (tsdFileReq != null)
                        await this._webMethodLoggerService.LogOK("DOCUMENTOAGGIUNGIVERSIONE", tsdFileReq.docNumber, string.Format(Resources.LogDocumentoAggiungiVersione, tsdFileReq.docNumber, tsdFileReq.version));
                    else
                        await this._webMethodLoggerService.LogKO("DOCUMENTOAGGIUNGIVERSIONE", fileRequest.docNumber, string.Format(Resources.LogDocumentoAggiungiVersione, fileRequest.docNumber, tsdFileReq?.version));


                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }
            return new CreateTSDVersionResult(tsdFileReq);
        }




        #endregion

        #region Private Members
        private async Task InserisciFirmaElettronica(FirmaElettronica firma)
        {
            var newFirmaElettronicaEntity = new FirmaElettronicaEntity()
            {
                ID_DOCUMENTO = firma.Docnumber.AsLong(),
                VERSION_ID = firma.Versionid.AsLong(),
                DOC_ALL = firma.DocAll,
                NUM_ALL = firma.NumAll.AsLong(),
                NUMERO_VERSIONE = firma.NumVersione.AsLong(),
                DATA_APPOSIZIONE = DateTime.Now
            };

            await this._dbContext.FirmaElettronicaEntities.AddAsync(newFirmaElettronicaEntity);
            int rowsInserted = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
        }
        private async Task<List<FirmaElettronica>> GetFirmaElettronicaDaFileRequest(DocsPaVO.documento.FileRequest fileRequest_old)
        {
            List<FirmaElettronica> returnList = new List<FirmaElettronica>();
            long versionId = fileRequest_old.versionId.AsLong();
            var firmaEntities = await this._dbContext.FirmaElettronicaEntities
                .Where(x => x.VERSION_ID == versionId && !string.IsNullOrEmpty(x.XML))
                .Select(x => new
                {
                    ID_FIRMA = x.ID_FIRMA,
                    ID_DOCUMENTO = x.ID_DOCUMENTO,
                    VERSION_ID = x.VERSION_ID,
                    DOC_ALL = x.DOC_ALL,
                    NUM_ALL = x.NUM_ALL,
                    NUMERO_VERSIONE = x.NUMERO_VERSIONE,
                    XML = x.XML,
                    DATA_APPOSIZIONE = x.DATA_APPOSIZIONE
                })
                .OrderBy(x => x.ID_FIRMA)
                .ToListAsync();

            foreach (var fe in firmaEntities)
            {
                returnList.Add(new FirmaElettronica()
                {
                    IdFirma = fe.ID_FIRMA.ToString(),
                    Docnumber = fe.ID_DOCUMENTO.ToString(),
                    Versionid = fe.VERSION_ID.ToString(),
                    DocAll = fe.DOC_ALL,
                    NumAll = fe.NUM_ALL.ToString() ?? string.Empty,
                    NumVersione = fe.NUMERO_VERSIONE.ToString(),
                    Xml = fe.XML ?? string.Empty,
                    DataApposizione = fe.DATA_APPOSIZIONE.ToString()
                });
            }

            return returnList;
        }

        private async Task<byte[]> GetTSRForDocument(DocsPaVO.documento.FileRequest fileRequest)
        {
            byte[] retval = null!;

            List<DocsPaVO.documento.TimestampDoc> tsrAL = await this.GetTimestampsDoc(fileRequest);

            var firstTs = tsrAL.FirstOrDefault();
            if (firstTs != null)
                retval = Convert.FromBase64String(firstTs.TSR_FILE);

            return retval;
        }

        private async Task<OutputResponseMarca> Verify(byte[] filep7m, byte[] fileTSR)
        {
            OutputResponseMarca outTSR = null;
            var verificaMarcaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
            {
                VerificaCompleta = false,
                FileFirmato = fileTSR,
                FileOriginale = filep7m,
                DataVerifica = DateTime.Now,
                TipoVerifica = TipiVerifica.Incapsulata,
                ReturnFileOriginale = true,
                ReturnXmlCompleto = true,
            });

            if (verificaMarcaResponse != null)
            {
                outTSR = new OutputResponseMarca();
                XmlDocument doc = new XmlDocument();

                if (verificaMarcaResponse.Esito != null && verificaMarcaResponse.Esito.DatiGeneraliVerifica != null)
                    doc.LoadXml(verificaMarcaResponse.Esito.DatiGeneraliVerifica);
                else if (verificaMarcaResponse.Warning != null &&
                        verificaMarcaResponse.Warning.WarningFault != null &&
                        verificaMarcaResponse.Warning.WarningFault.Length != 0 &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale.FileMarcato &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale != null &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale.DatiGeneraliVerifica != null)
                {
                    doc.LoadXml(verificaMarcaResponse.Warning.DettaglioFirmaDigitale.DatiGeneraliVerifica);
                    for (int i = 0; i < verificaMarcaResponse.Warning.WarningFault.Length; i++)
                        outTSR.descrizioneErrore += !string.IsNullOrEmpty(outTSR.descrizioneErrore) ?
                            string.Concat(outTSR.descrizioneErrore, " - ", verificaMarcaResponse.Warning.WarningFault[i].ErrorMsg)
                            : verificaMarcaResponse.Warning.WarningFault[i].ErrorMsg;
                }

                if (doc != null)
                {
                    XmlNode node = doc.DocumentElement;
                    XmlNode timestampNode = node.SelectSingleNode("/deSign/timeStamp");
                    XmlNode certNode = timestampNode.SelectSingleNode("certificate");

                    var certBytes = Encoding.UTF8.GetBytes(certNode.InnerText);
                    var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certBytes);

                    if (DateTime.Now.CompareTo(cert.NotAfter.ToLocalTime()) > 0)
                        outTSR.descrizioneErrore = ErrorDescription.ElapsedTimestamp;

                    outTSR.dsm = cert.NotAfter.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                    outTSR.sernum = int.Parse(timestampNode.SelectSingleNode("timeStampSerial")?.InnerText, System.Globalization.NumberStyles.HexNumber).ToString() ?? string.Empty;
                    string hexHash = BitConverter.ToString(filep7m.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant();
                    outTSR.fhash = hexHash;
                    outTSR.docm = DateTime.ParseExact(timestampNode.SelectSingleNode("verificationTime")?.InnerText, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).ToString("HH:mm:ss");
                    outTSR.docm_date = DateTime.ParseExact(timestampNode.SelectSingleNode("verificationTime")?.InnerText, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).AsDateTimeFormat();
                    outTSR.marca = Convert.ToBase64String(fileTSR); //BitConverter.ToString(tSR.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant(); //Convert.ToBase64String(ParseHex(timestampNode.SelectSingleNode("timeStampImprint")?.InnerText));
                    outTSR.fromDate = cert.NotBefore.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                    outTSR.snCertificato = int.Parse(cert.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
                    outTSR.TSA = new TSARFC2253()
                    {
                        TSARFC2253Name = String.Format("CN={0},OU={1},O={2},C={3}",
                            timestampNode.SelectSingleNode("issuer/CN")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/OU")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/O")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/C")?.InnerText),
                        C = timestampNode.SelectSingleNode("issuer/C")?.InnerText,
                        CN = timestampNode.SelectSingleNode("issuer/CN")?.InnerText,
                        O = timestampNode.SelectSingleNode("issuer/O")?.InnerText,
                        OU = timestampNode.SelectSingleNode("issuer/OU")?.InnerText
                    };
                    System.Security.Cryptography.Oid oidHash = new System.Security.Cryptography.Oid(timestampNode.SelectSingleNode("timeStampImprintAlgorithm")?.InnerText);
                    outTSR.algHash = oidHash.FriendlyName;
                    outTSR.esito = "OK";
                }
            }
            return outTSR;
        }

        private async Task<List<TimestampDoc>> GetTimestampsDoc(DocsPaVO.documento.FileRequest fileRequest)
        {
            var entities = await this._dbContext.TimestampDocEntities
                .Where(x => x.VERSION_ID == fileRequest.versionId.AsLong() && x.DOC_NUMBER == fileRequest.docNumber.AsLong())
                .ToListAsync();

            return this._mapper.Map<List<TimestampDoc>>(entities);
        }

        protected readonly ILogger<CreateTSDVersionHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IFileValidatorService _fileValidatorService;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;
        protected readonly IMarcaTemporaleService _marcaTemporaleService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TimestampDocEntity, TimestampDoc>()
                    .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID.ToString()))
                    .ForMember(dest => dest.DOC_NUMBER, opt => opt.MapFrom(src => src.DOC_NUMBER.ToString()))
                    .ForMember(dest => dest.VERSION_ID, opt => opt.MapFrom(src => src.VERSION_ID.ToString()))
                    .ForMember(dest => dest.ID_PEOPLE, opt => opt.MapFrom(src => src.ID_PEOPLE.ToString()))
                    .ForMember(dest => dest.DTA_CREAZIONE, opt => opt.MapFrom(src => src.DTA_CREAZIONE.ToString()))
                    .ForMember(dest => dest.DTA_SCADENZA, opt => opt.MapFrom(src => src.DTA_SCADENZA.ToString()))
                    .ForMember(dest => dest.NUM_SERIE, opt => opt.MapFrom(src => src.NUM_SERIE ?? string.Empty))
                    .ForMember(dest => dest.S_N_CERTIFICATO, opt => opt.MapFrom(src => src.S_N_CERTIFICATO ?? string.Empty))
                    .ForMember(dest => dest.ALG_HASH, opt => opt.MapFrom(src => src.ALG_HASH ?? string.Empty))
                    .ForMember(dest => dest.SOGGETTO, opt => opt.MapFrom(src => src.SOGGETTO ?? string.Empty))
                    .ForMember(dest => dest.PAESE, opt => opt.MapFrom(src => src.PAESE ?? string.Empty))
                    .ForMember(dest => dest.TSR_FILE, opt => opt.MapFrom(src => src.TSR_FILE ?? string.Empty));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}