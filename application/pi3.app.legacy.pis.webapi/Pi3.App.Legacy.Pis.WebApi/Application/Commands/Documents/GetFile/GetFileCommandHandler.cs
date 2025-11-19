// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetFileConSegnatura;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.RemotePdfSignStamp;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Globalization;
using System.Xml;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFile
{
    public class GetFileCommandHandler : IRequestHandler<GetFileCommand, GetFileCommandResponse>
    {
        #region Public Members

        public GetFileCommandHandler(ILogger<GetFileCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IFirmaDigitale2Service firmaDigitale2Service,
            IPAdESService pAdESService,
            ICAdESService cAdESService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentBlobRepository = documentBlobRepository;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._firmaDigitale2Service = firmaDigitale2Service;
            this._pAdESService = pAdESService;
            this._cAdESService = cAdESService;
        }

        public async Task<GetFileCommandResponse> Handle(GetFileCommand request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            File result = new File();
            try
            {
                if (request.fileRequest != null)
                {
                    result.Description = request.fileRequest.descrizione;
                    result.Id = request.fileRequest.docNumber;
                    result.VersionId = request.fileRequest.version;

                    if (request.getFile)
                    {
                        FileDocumento fileDocumento = null;

                        if (request.fileConFirma)
                        {
                            fileDocumento = await GetFile(request.fileRequest, false, idTenant);
                        }
                        else if (!request.segnatura && !request.timbro)
                        {
                            fileDocumento = await GetFile(request.fileRequest, true, idTenant);
                        }
                        else
                        {
                            var position = request.position != null ? request.position : new labelPdf();
                            position.orientamento = request.segnatura ? string.Empty : "orizzontale";
                            position.position = "15-30";
                            position.sel_color = "1";
                            position.sel_font = "1";
                            position.tipoLabel = true;

                            fileDocumento = (await _mediator.Send(new DocumentoGetFileConSegnaturaCommand()
                            {
                                fileRequest = request.fileRequest,
                                sch = request.schedaDoc,
                                infoUtente = request.infoUtente,
                                position = position,
                                Forced = false
                            })).fileDocumento;
                        }

                        result.Name = fileDocumento.nomeOriginale;
                        result.Content = fileDocumento.content;
                        result.MimeType = fileDocumento.contentType;
                    }
                    else
                    {
                        FileDocumento fileInfo = await GetInfoFile(request.fileRequest, request.infoUtente);
                        if (fileInfo != null)
                        {
                            result.Name = fileInfo.nomeOriginale;
                            result.MimeType = fileInfo.contentType;
                        }
                    }
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                result = null;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                result = null;
            }
            return new GetFileCommandResponse()
            {
                File = result
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetFileCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;
        protected readonly ICAdESService _cAdESService;
        protected readonly IPAdESService _pAdESService;

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
                                pKCS7Document.SignatureType = SignType.CADES;
                                break;
                        }

                        pKCS7Document.SignAlgorithm = pKCS7Document.SignatureType.ToString();
                        foreach (var signer in signature.signer)
                        {
                            var signerInfo = _mapper.Map<SignerInfo>(datiFirmatario.Where(d => d.Firmatario.DistinguishName.Equals(signer.subject.DNQUALIF)).FirstOrDefault());
                            signerInfo.SigningTime = signer.signingTime != null ? DateTime.ParseExact(signer.signingTime, "yyMMddHHmmssZ", CultureInfo.InvariantCulture) : DateTime.MinValue;

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

        protected async Task<FileDocumento> GetFile(FileRequest fileRequest, bool verificaFileFirmato, string idTenant)
        {
            FileDocumento fileDocumento = new FileDocumento();

            try
            {
                var componentsEntity = await _dbContext.ComponentEntities.AsNoTracking()
                .Where(c => c.VERSION_ID == fileRequest.versionId.AsLong())
                .Select(c => new
                {
                    c.PATH,
                    c.VAR_NOMEORIGINALE,
                    c.EXT
                })
                .FirstAsync();

                fileDocumento.path = componentsEntity.PATH;
                fileDocumento.nomeOriginale = componentsEntity.VAR_NOMEORIGINALE;
                fileDocumento.estensioneFile = componentsEntity.EXT;
                fileDocumento.fullName = componentsEntity.VAR_NOMEORIGINALE;
                fileDocumento.name = componentsEntity.VAR_NOMEORIGINALE;

                DocumentBlob blob = await this._documentBlobRepository.Get(idTenant, fileDocumento.path);
                using (var memoryStream = new MemoryStream())
                {
                    blob.Stream.CopyTo(memoryStream);
                    fileDocumento.content = memoryStream.ToArray();
                }
                fileDocumento.contentType = blob.ContentType;
                fileDocumento.length = fileDocumento.content.Length;

                if (verificaFileFirmato)
                {
                    if ((fileDocumento.fullName.ToUpper().EndsWith("P7M")) || //cades
                       (fileDocumento.fullName.ToUpper().EndsWith("TSD")) || //timestamp
                       (fileDocumento.fullName.ToUpper().EndsWith("M7M")) || //timestamp
                       (fileDocumento.fullName.ToUpper().EndsWith("PDF") && await _pAdESService.IsPAdESFile(new MemoryStream(fileDocumento.content))) ||
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
                                await this._cAdESService.LoadOriginalFile(Path.GetFileName(fileDocumento.fullName), msSignedFile, msOriginalFile);
                                fileDocumento.content = msOriginalFile.ToArray();
                            }
                        }
                        fileDocumento.estensioneFile = GetEstensioneIntoSignedFile(fileDocumento.nomeOriginale ?? fileDocumento.name);
                        fileDocumento.name = Path.GetFileNameWithoutExtension(fileDocumento.name);
                        fileDocumento.length = fileDocumento.content.Length;
                        fileDocumento.contentType = await GetMimeType(fileDocumento.estensioneFile);
                        fileDocumento.fullName = fileDocumento.name;
                    }
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                fileDocumento = null;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                fileDocumento = null;
            }

            return fileDocumento;
        }

        protected async Task<FileDocumento> GetInfoFile(FileRequest fileRequest, InfoUtente infoUtente)
        {
            FileDocumento output = new FileDocumento();

            try
            {
                var versionId = !string.IsNullOrEmpty(fileRequest.versionId) ? fileRequest.versionId.AsLong() : 0;
                var docnumber = !string.IsNullOrEmpty(fileRequest.docNumber) ? fileRequest.docNumber.AsLong() : 0;

                output.path = fileRequest.docServerLoc + fileRequest.path;
                output.name = fileRequest.fileName != null ? fileRequest.fileName : string.Empty;

                int indice = output.name.LastIndexOf(@"\");
                if (indice < (output.name.Length - 1))
                    output.name = output.name.Substring(indice + 1);

                if (!string.IsNullOrEmpty(fileRequest.versionId) && !string.IsNullOrEmpty(fileRequest.docNumber))
                {
                    var nomeOriginale = await this._dbContext.ComponentEntities.AsNoTracking().Where(c => c.VERSION_ID == versionId && c.DOCNUMBER == docnumber).Select(c => c.VAR_NOMEORIGINALE).FirstOrDefaultAsync();
                    output.nomeOriginale = removeIllegalChars(nomeOriginale);
                }

                output.fullName = string.IsNullOrEmpty(fileRequest.path) ? '\u005C'.ToString() + output.name : fileRequest.fileName;

                string[] extArr = output.name.Split('.');
                var ext = extArr[extArr.Length - 1].ToLower();

                List<Applicazione> apps = await GetApplications(ext);
                if (apps != null && apps.Count > 0)
                {
                    var application = apps[0];
                    output.contentType = application != null && !string.IsNullOrEmpty(application.mimeType) ? application.mimeType : "application/x-" + ext;
                }

                var extIntoSignedFile = string.Empty;
                var fileName = new FileInfo(output.fullName).Name;
                string[] items = fileName.Split('.');
                for (int i = (items.Length - 1); i >= 0; i--)
                {
                    if (!(items[i].ToUpper().EndsWith("P7M") || items[i].ToUpper().EndsWith("TSD") || items[i].ToUpper().EndsWith("M7M")))
                    {
                        extIntoSignedFile = items[i];
                        break;
                    }
                }
                output.estensioneFile = extIntoSignedFile;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return output;
        }

        protected string removeIllegalChars(string filename)
        {

            if (string.IsNullOrEmpty(filename))
                return filename;

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
                filename = filename.Replace(c.ToString(), "_");

            return filename;
        }

        protected virtual async Task<List<Applicazione>> GetApplications(string ext)
        {
            List<Applicazione> output = new List<Applicazione>();

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
                 output.Add(new Applicazione
                 {
                     estensione = a.DEFAULT_EXTENSION,
                     mimeType = a.MIME_TYPE
                 })
                );

                if (output.Count == 0)
                {
                    var appEntity = new AppEntity
                    {
                        APPLICATION = "GEN_" + ext,
                        DESCRIPTION = "GEN_" + ext,
                        FILING_SCHEME = 2,
                        DEFAULT_EXTENSION = ext
                    };

                    await this._dbContext.AppEntities.AddAsync(appEntity);

                    await ((DbContext)_dbContext).SaveChangesAsync();

                    output.Add(new Applicazione
                    {
                        estensione = appEntity.DEFAULT_EXTENSION,
                        mimeType = appEntity.MIME_TYPE
                    });
                }

            }
            return output;
        }

        #endregion
    }
}
