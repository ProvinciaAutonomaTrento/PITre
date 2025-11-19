// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DeSign;
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using DocsPaVO.Mobile;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.Decorators;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DocumentoGetFileRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFile;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetFile
{
    public class DocumentoGetFileHandler : IRequestHandler<DocumentoGetFileRequest, DocumentoGetFileResult>
    {
        #region Public Members

        public DocumentoGetFileHandler(ILogger<DocumentoGetFileHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDocumentBlobRepository documentBlobRepository,
            IWebMethodLoggerService webMethodLoggerService,
            IPi3DbContext dbContext,
            ISessionRepositoryService sessionRepositoryService,
            IFirmaDigitale2Service firmaDigitale2Service,
            IPAdESService pAdESService,
            ICAdESService cAdESService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentBlobRepository = documentBlobRepository;
            this._webMethodLoggerService = webMethodLoggerService;
            this._sessionRepositoryService = sessionRepositoryService;
            this._firmaDigitale2Service = firmaDigitale2Service;
            this._pAdESService = pAdESService;
            this._cAdESService = cAdESService;

            this.InitializeMapper();
        }

        public async Task<DocumentoGetFileResult> Handle(DocumentoGetFileRequest request, CancellationToken cancellationToken)
        {
            FileDocumento fileDocumento = new FileDocumento();
            var fileRequest = request.fileRequest;
            var isDigitallySigned = false;
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            try
            {
                if (fileRequest.repositoryContext != null)
                {
                    fileDocumento = await _sessionRepositoryService.GetFile(fileRequest.repositoryContext, fileRequest);
                }
                else
                {
                    var componentsEntity = await _dbContext.ComponentEntities.AsNoTracking()
                    .Where(c => c.VERSION_ID == fileRequest.versionId.AsLong())
                    .Select(c => new
                    {
                        c.PATH,
                        c.VAR_NOMEORIGINALE,
                        c.EXT,
                        c.CHA_FIRMATO,
                        c.CHA_TIPO_FIRMA
                    })
                    .FirstAsync();

                    fileDocumento.path = componentsEntity.PATH;
                    fileDocumento.nomeOriginale = componentsEntity.VAR_NOMEORIGINALE;
                    fileDocumento.estensioneFile = componentsEntity.EXT;
                    fileDocumento.fullName = componentsEntity.VAR_NOMEORIGINALE != null ? componentsEntity.VAR_NOMEORIGINALE : componentsEntity.PATH;
                    fileDocumento.name = componentsEntity.VAR_NOMEORIGINALE;

                    isDigitallySigned = componentsEntity.CHA_FIRMATO == "1" && componentsEntity.CHA_TIPO_FIRMA != "E";

                    DocumentBlob blob = await this._documentBlobRepository.Get(idTenant, fileDocumento.path);
                    using (var memoryStream = new MemoryStream())
                    {
                        blob.Stream.CopyTo(memoryStream);
                        fileDocumento.content = memoryStream.ToArray();
                    }
                    fileDocumento.contentType = blob.ContentType;
                    fileDocumento.length = fileDocumento.content.Length;
                }

                if ((fileDocumento.fullName.ToUpper().EndsWith("P7M")) || //cades
                   (fileDocumento.fullName.ToUpper().EndsWith("TSD")) || //timestamp
                   (fileDocumento.fullName.ToUpper().EndsWith("M7M")) || //timestamp
                   (fileDocumento.fullName.ToUpper().EndsWith("PDF") && (isDigitallySigned || await _pAdESService.IsPAdESFile(new MemoryStream(fileDocumento.content)))) ||
                   (fileDocumento.fullName.ToUpper().EndsWith("XML") && await IsSignedXades(fileDocumento))) // XADES
                {
                    try
                    {
                        var verificaFirmaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
                        {
                            VerificaCompleta = fileDocumento.fullName.ToUpper().EndsWith("P7M"),
                            FileFirmato = fileDocumento.content,
                            DataVerifica = await _dbContext.GetSystemDateTime(),
                            TipoVerifica = TipiVerifica.Appiattita,
                            ReturnFileOriginale = false,
                            ReturnXmlCompleto = true
                        });

                        fileDocumento.signatureResult = new VerifySignatureResult();
                        fileDocumento.signatureResult.FinalDocumentName = fileDocumento.nomeOriginale;
                        if (verificaFirmaResponse.Esito != null)
                        {
                            fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.Valid;
                            fileDocumento.signatureResult.PKCS7Documents = await GetSignature(verificaFirmaResponse.Esito.DatiGeneraliVerifica, verificaFirmaResponse.Esito.DatiFirmatari);
                        }
                        else if (verificaFirmaResponse.Warning != null)
                        {
                            fileDocumento.signatureResult.PKCS7Documents = await GetSignature(verificaFirmaResponse.Warning.DettaglioFirmaDigitale.DatiGeneraliVerifica, verificaFirmaResponse.Warning.DettaglioFirmaDigitale.DatiFirmatari);
                            fileDocumento.signatureResult.ErrorMessages = verificaFirmaResponse.Warning.WarningFault.Select(w => w.ErrorMsg).ToArray();
                            fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.ErroreGenerico;
                            verificaFirmaResponse.Warning.WarningFault.ForEach(w =>
                            {
                                switch (w.ErrorCode)
                                {
                                    case "1426":
                                        fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.CtlNotTimeValid;
                                        break;
                                    case "1468":
                                        fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.SHA1NonSupportato;
                                        break;
                                    case "1407":
                                        fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.NotTimeValid;
                                        break;
                                    case "1408":
                                        fileDocumento.signatureResult.StatusCode = (int)EsitoVerificaStatus.Revoked;
                                        break;
                                }
                            });
                        }


                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(exception: ex, message: ex.Message);
                    }
                }


                if ((fileDocumento.fullName.ToUpper().EndsWith("P7M")) || //cades
                     (fileDocumento.fullName.ToUpper().EndsWith("TSD")) || //timestamp
                     (fileDocumento.fullName.ToUpper().EndsWith("M7M")))//timestamp
                {
                    using (var msSignedFile = new MemoryStream(fileDocumento.content))
                    {
                        using (var msOriginalFile = new MemoryStream())
                        {
                            await this._cAdESService.LoadOriginalFile(fileDocumento.fullName, msSignedFile, msOriginalFile);
                            fileDocumento.content = msOriginalFile.ToArray();
                        }
                    }
                    fileDocumento.estensioneFile = GetEstensioneIntoSignedFile(fileDocumento.nomeOriginale ?? fileDocumento.name);
                    fileDocumento.name = Path.GetFileNameWithoutExtension(fileDocumento.name);
                    fileDocumento.length = fileDocumento.content.Length;
                    fileDocumento.contentType = await GetMimeType(fileDocumento.estensioneFile);
                    fileDocumento.fullName = fileDocumento.name;
                }

                await this._webMethodLoggerService.LogOK("DOCUMENTOGETFILE",
                request.fileRequest.docNumber, string.Format(Resources.LogDocumentoGetFile,
                request.fileRequest.docNumber, request.fileRequest.version));
            }
            catch (Pi3Exception pi3Ex)
            {
                await this._webMethodLoggerService.LogKO("DOCUMENTOGETFILE",
                    request.fileRequest.docNumber, string.Format(Resources.LogDocumentoGetFile,
                    request.fileRequest.docNumber, request.fileRequest.version));

                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                fileDocumento = null;
            }
            catch (Exception ex)
            {
                await this._webMethodLoggerService.LogKO("DOCUMENTOGETFILE",
                    request.fileRequest.docNumber, string.Format(Resources.LogDocumentoGetFile,
                    request.fileRequest.docNumber, request.fileRequest.version));

                this._logger.LogCritical(exception: ex, message: ex.Message);
                fileDocumento = null;
            }

            return new DocumentoGetFileResult(fileDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetFileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPAdESService _pAdESService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly ISessionRepositoryService _sessionRepositoryService;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;
        protected readonly ICAdESService _cAdESService;

        protected enum EsitoVerificaStatus
        {
            Valid = 0,         //OK
            NotTimeValid = 1,  //Scaduto
            Revoked = 4,       //Revocato
            CtlNotTimeValid = 131072, //Data non corretta
            ErroreGenerico = -1,
            SHA1NonSupportato = -2
        }

        protected IMapper _mapper = null;


        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DatiFirmatari, SignerInfo>()
                     .ForMember(dest => dest.SignatureAlgorithm, opt => opt.MapFrom(src => src.Firmatario.DigestAlgorithm))
                     .ForMember(dest => dest.SigningTime, opt => opt.MapFrom(src => src.Firmatario.DataOraFirma))
                     .AfterMap((src, dest) =>
                     {
                         dest.SubjectInfo = new SubjectInfo()
                         {
                             CertId = src.Firmatario.DistinguishName,
                             Cognome = src.Firmatario.Cognome,
                             Nome = src.Firmatario.Nome ?? src.Firmatario.CommonName,
                             CodiceFiscale = src.Firmatario.CodiceFiscale,
                             Country = src.Firmatario.Nazione,
                             Organizzazione = src.Firmatario.Organizzazione
                         };

                         dest.CertificateInfo = new CertificateInfo()
                         {
                             IssuerName = $"CN={src.Firmatario.CnCertAuthority}",
                             SerialNumber = src.Firmatario.SerialNumber,
                             SubjectName = src.Firmatario.CommonName,
                             ValidFromDate = src.Firmatario.DataInizioValiditaCert,
                             ValidToDate = src.Firmatario.DataFineValiditaCert,
                         };

                         if (src.MarcaFirma != null)
                         {
                             dest.SignatureTimeStampInfo = new TSInfo[1]
                             {
                                 new TSInfo()
                                 {
                                     TSANameIssuer = src.MarcaFirma.TSANameIssuer,
                                     TSANameSubject = src.MarcaFirma.TSANameSubject,
                                     TSdateTime = src.MarcaFirma.TSdateTime,
                                     TSimprint = src.MarcaFirma.TSimprint,
                                     TSserialNumber = src.MarcaFirma.TSserialNumber,
                                     dataFineValiditaCert = src.MarcaFirma.DataFineValiditaCert,
                                     dataInizioValiditaCert = src.MarcaFirma.DataInizioValiditaCert
                                 }
                             };
                         }
                     });

                cfg.CreateMap<countersigner, SignerInfo>()
                     .ForMember(dest => dest.SignatureAlgorithm, opt => opt.MapFrom(src => src.digestAlgorithm))
                     .ForMember(dest => dest.SigningTime, opt => opt.MapFrom(src => DateTime.ParseExact(src.signedAttributes.signingTime, "yyMMddHHmmssZ", CultureInfo.InvariantCulture)))
                     .ForMember(dest => dest.isCountersigner, opt => opt.MapFrom(src => true))
                     .AfterMap((src, dest) =>
                     {
                         dest.SubjectInfo = new SubjectInfo()
                         {
                             CertId = src.subject.DNQUALIF,
                             Cognome = src.subject.SUR,
                             Nome = src.subject.GIVEN,
                             CodiceFiscale = src.subject.SER,
                             Country = src.subject.C,
                             Organizzazione = src.subject.O
                         };

                         dest.CertificateInfo = new CertificateInfo()
                         {
                             IssuerName = $"CN={src.issuer.CN}",
                             SerialNumber = src.serial,
                             SubjectName = src.subject.CN,
                             ValidFromDate = DateTime.ParseExact(src.certNotBefore, "yyMMddHHmmssZ", CultureInfo.InvariantCulture),
                             ValidToDate = DateTime.ParseExact(src.certNotAfter, "yyMMddHHmmssZ", CultureInfo.InvariantCulture),
                         };

                         if (src.signatureTimeStamp != null && !string.IsNullOrEmpty(src.signatureTimeStamp.timeStampDate))
                         {
                             dest.SignatureTimeStampInfo = new TSInfo[1]
                             {
                                 new TSInfo()
                                 {
                                     TSANameIssuer = src.signatureTimeStamp.issuer.DESCR,
                                     TSANameSubject = src.signatureTimeStamp.subject.DESCR,
                                     TSdateTime = DateTime.ParseExact(src.signatureTimeStamp.timeStampDate, "yyMMddHHmmssZ", CultureInfo.InvariantCulture),
                                     TSimprint = src.signatureTimeStamp.timeStampImprint,
                                     TSserialNumber = src.signatureTimeStamp.timeStampSerial,
                                     dataFineValiditaCert = DateTime.ParseExact(src.signatureTimeStamp.certNotAfter, "yyMMddHHmmssZ", CultureInfo.InvariantCulture),
                                     dataInizioValiditaCert = DateTime.ParseExact(src.signatureTimeStamp.certNotBefore, "yyMMddHHmmssZ", CultureInfo.InvariantCulture)
                                 }
                             };
                         }
                     });
            });

            this._mapper = configuration.CreateMapper();
        }

        protected virtual async Task<PKCS7Document[]> GetSignature(string datiGeneraliVerificaFirma, List<DatiFirmatari> datiFirmatario)
        {
            List<PKCS7Document> pKCS7Documents = new List<PKCS7Document>();

            try
            {
                var datiGeneraliVerifica = DeSign.deSign.Deserialize(datiGeneraliVerificaFirma);
                if (datiGeneraliVerifica != null && datiGeneraliVerifica.signedData != null && datiGeneraliVerifica.signedData.Count > 0)
                {
                    var pKCS7Document = new PKCS7Document();
                    List<SignerInfo> signers = new List<SignerInfo>();

                    for (int i = 0; i < datiGeneraliVerifica.signedData.Count; i++)
                    {
                        var signature = datiGeneraliVerifica.signedData[i];

                        pKCS7Document = new PKCS7Document();
                        signers = new List<SignerInfo>();

                        switch (signature.filetype)
                        {
                            case "XML file":
                                pKCS7Document.SignatureType = SignType.XADES;
                                break;
                            case "PDF file":
                                pKCS7Document.SignatureType = SignType.PADES;
                                break;
                            case "PKCS7 file":
                            case "CMS file":
                                pKCS7Document.SignatureType = SignType.CADES;
                                break;
                            case "TSD file":
                                if(!string.IsNullOrEmpty(signature.pkcs7))
                                {
                                    pKCS7Document.SignatureType = SignType.CADES;
                                }
                                break;
                        }

                        pKCS7Document.SignAlgorithm = pKCS7Document.SignatureType.ToString();
                        foreach (var signer in signature.signer)
                        {
                            //var signerInfo = _mapper.Map<SignerInfo>(datiFirmatario.Where(d => (d.Firmatario.DistinguishName != null && d.Firmatario.DistinguishName.Equals(signer.subject.DNQUALIF))
                            //        || (d.Firmatario.CnCertAuthority != null && d.Firmatario.CnCertAuthority.Equals(signer.subject.CN))
                            //        || (d.Firmatario.Organizzazione != null && d.Firmatario.Organizzazione.Equals(signer.subject.O)))
                            //    .FirstOrDefault());

                            var signerInfo = _mapper.Map<SignerInfo>(datiFirmatario.Where(d => d.Firmatario.SerialNumber == signer.serial).FirstOrDefault());

                            signerInfo.SigningTime = signer.signingTime != null ? DateTime.ParseExact(signer.signingTime, "yyMMddHHmmssZ", CultureInfo.InvariantCulture) :
                                (signer.signedAttributes != null && signer.signedAttributes.signingTime != null ? DateTime.ParseExact(signer.signedAttributes.signingTime, "yyMMddHHmmssZ", CultureInfo.InvariantCulture) : DateTime.MinValue);

                            if(signer.countersigner != null && signer.countersigner.Count > 0)
                            {
                                signerInfo.counterSignatures = _mapper.Map<SignerInfo[]>(signer.countersigner);
                            }

                            if (pKCS7Document.SignatureType == SignType.CADES)
                            {
                                pKCS7Documents.Add(new PKCS7Document()
                                {
                                     SignatureType = pKCS7Document.SignatureType,
                                     SignersInfo = new SignerInfo[] { signerInfo },
                                     SignAlgorithm = pKCS7Document.SignAlgorithm
                                });
                            }
                            else
                            {
                                signers.Add(signerInfo);
                            }
                        }
                    }

                    if (signers.Count > 0)
                    {
                        pKCS7Document.SignersInfo = signers.ToArray();
                        pKCS7Documents.Add(pKCS7Document);
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Errore nel metodo GetTypeSignature " + e.Message);
                pKCS7Documents = new List<PKCS7Document>();
            }

            return pKCS7Documents.ToArray();
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

        protected string GetEstensioneIntoSignedFile(string fullname)
        {
            string retValue = string.Empty;

            // Reperimento del nome del file con estensione
            string fileName = new System.IO.FileInfo(fullname).Name;

            string[] items = fileName.Split('.');

            for (int i = (items.Length - 1); i >= 0; i--)
            {
                if (!(items[i].ToUpper().EndsWith("P7M") ||
                    items[i].ToUpper().EndsWith("TSD") ||
                    items[i].ToUpper().EndsWith("M7M"))
                    )
                {
                    retValue = items[i];
                    break;
                }
            }
            return retValue;
        }

        protected async Task<string> GetMimeType(string ext)
        {
            this._logger.LogDebug($"DocumentoGetFile > GetMimeType > ext: {ext}");
            string mimeType = string.Empty;

            List<Applicazione> apps = new List<Applicazione>();

            if (!string.IsNullOrEmpty(ext))
            {
                var appsEntity = await this._dbContext.AppEntities.AsNoTracking()
                    .Where(a => a.DEFAULT_EXTENSION.ToUpper() == ext.ToUpper())
                    .Select(a => new
                    {
                        a.DEFAULT_EXTENSION,
                        a.MIME_TYPE
                    })
                    .ToListAsync();

                appsEntity.ForEach(a =>
                 apps.Add(new Applicazione
                 {
                     estensione = a.DEFAULT_EXTENSION,
                     mimeType = a.MIME_TYPE
                 })
                );

                if (apps.Count == 0)
                {
                    this._logger.LogDebug($"DocumentoGetFile > GetMimeType > inserisco ext: {ext}");
                    var appEntity = new AppEntity
                    {
                        APPLICATION = "GEN_" + ext,
                        DESCRIPTION = "GEN_" + ext,
                        FILING_SCHEME = 2,
                        DEFAULT_EXTENSION = ext
                    };

                    await this._dbContext.AppEntities.AddAsync(appEntity);

                    await ((DbContext)_dbContext).SaveChangesAsync();

                    apps.Add(new Applicazione
                    {
                        estensione = appEntity.DEFAULT_EXTENSION,
                        mimeType = appEntity.MIME_TYPE
                    });
                }

            }

            if (apps != null && apps.Count > 0)
            {
                var application = apps[0];
                mimeType = application != null && !string.IsNullOrEmpty(application.mimeType) ? application.mimeType : "application/x-" + ext;
            }

            return mimeType;
        }
        #endregion
    }
}