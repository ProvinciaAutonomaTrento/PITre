// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.File.MarcaTemporale;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static DocsPaVO.documento.FileInformation;
using executeAndSaveTSR_AMRequest = Pi3.App.Legacy.WebApi.Application.Requests.executeAndSaveTSR_AM;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.executeAndSaveTSR_AM
{
    public class executeAndSaveTSR_AMHandler : IRequestHandler<executeAndSaveTSR_AMRequest, executeAndSaveTSR_AMResult>
    {
        #region Public Members

        public executeAndSaveTSR_AMHandler(ILogger<executeAndSaveTSR_AMHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext, IConfigurationService configurationService,
            IFileValidatorService fileValidatorService, IWebMethodLoggerService webMethodLoggerService,
            IFirmaDigitale2Service firmaDigitale2Service, IMarcaTemporaleService marcaTemporaleService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator; ;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
            this._fileValidatorService = fileValidatorService;
            this._firmaDigitale2Service = firmaDigitale2Service;
            this._marcaTemporaleService = marcaTemporaleService;

            this.InitializeMapper();
        }

        public async Task<executeAndSaveTSR_AMResult> Handle(executeAndSaveTSR_AMRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.areaConservazione.OutputResponseMarca ret = null;

            DocsPaVO.documento.FileRequest fileRequest = request.fileRequest;
            InfoUtente infoUtente = request.infoUtente;
            InputMarca richiesta = request.richiesta;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            try
            {
                long idVersion = fileRequest.versionId.AsLong();
                long docNumber = fileRequest.docNumber.AsLong();
                var componentsEntity = await this._dbContext.ComponentEntities
                    .Join(this._dbContext.VersionEntities, c => c.VERSION_ID, v => v.VERSION_ID, (c, v) => new {c, v})
                    .Where(x => x.c.VERSION_ID == idVersion && x.c.DOCNUMBER == docNumber)
                    .Select(x => new DocsPaVO.documento.FileRequest()
                    {
                        fileSize = x.c.FILE_SIZE.ToString(),
                        docNumber = x.c.DOCNUMBER.ToString(),
                        versionId = x.c.VERSION_ID.ToString(),
                        fileName = x.c.PATH,
                        version = x.v.VERSION.ToString(),
                        versionLabel = x.v.VERSION_LABEL

                    }).FirstOrDefaultAsync();
                //ret = await this.ExecuteAndSaveTSR(richiesta, fileRequest, idPeople, infoUtente, idTenant);
                ret = await this.ExecuteAndSaveTSR(richiesta, componentsEntity, idPeople, infoUtente, idTenant);
                if (ret == null)
                    await this._webMethodLoggerService.LogKO("DOCUMENTOTIMESTAMP", fileRequest.docNumber, string.Format(Resources.LogMarcaTemporale, fileRequest.docNumber));
                else
                    await this._webMethodLoggerService.LogOK("DOCUMENTOTIMESTAMP", fileRequest.docNumber, string.Format(Resources.LogMarcaTemporale, fileRequest.docNumber));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null);
                await this._webMethodLoggerService.LogKO("DOCUMENTOTIMESTAMP", fileRequest.docNumber, string.Format(Resources.LogMarcaTemporale, fileRequest.docNumber));
            }
            return new executeAndSaveTSR_AMResult(ret);
        }

        #endregion

        #region Private Members
        private async Task<OutputResponseMarca> ExecuteAndSaveTSR(InputMarca richiesta, DocsPaVO.documento.FileRequest fileRequest, long idPeople, DocsPaVO.utente.InfoUtente infoUtente, long idTenant)
        {
            DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = null;

            if (fileRequest == null)
                //throw new FileRequestNullPi3Exception();
                this._logger.LogError("ExecuteAndSaveTSR > FileRequest nullo");

            //Scelta del tipo di implementazione per la richiesta della marca temporale
            #region Forse non serve
            //(string? typeName, bool found) = await this._configurationService.TryGetValue<string>("TYPE_TSA");

            //Type instanceType = Type.GetType(typeName, false);

            //if (instanceType == null)
            //    throw new InstanceTypeNotFoundPi3Exception(typeName);
            #endregion



            //Ottengo una marca temporale
            resultMarca = await this.GetTimeStamp(richiesta, fileRequest.fileName);

            //Genero l'array di byte per il file p7m e TSR
            byte[] p7m = StringToBytes(richiesta.file_p7m);
            byte[] TSR = Convert.FromBase64String(resultMarca.marca);

            //Verifico la marca e completo l'oggetto OutputResponseMarca
            resultMarca = await this.VerificaMarca(p7m, TSR);

            if (resultMarca == null)
                //throw new TimestampNullPi3Exception();
                this._logger.LogError("ExecuteAndSaveTSR > Marca nulla");

            //Salvo la marca generata sul database
            var tsEntityToInsert = new TimestampDocEntity()
            {
                DOC_NUMBER = fileRequest.docNumber.AsLong(),
                VERSION_ID = fileRequest.versionId.AsLong(),
                ID_PEOPLE = idPeople,
                DTA_CREAZIONE = resultMarca.docm_date.AsDateTime(),
                DTA_SCADENZA = resultMarca.dsm.AsDateTime(),
                NUM_SERIE = resultMarca.sernum,
                S_N_CERTIFICATO = resultMarca.snCertificato,
                ALG_HASH = resultMarca.algHash,
                SOGGETTO = resultMarca.TSA.O,
                PAESE = resultMarca.TSA.C,
                TSR_FILE = resultMarca.marca
            };

            await this._dbContext.TimestampDocEntities.AddAsync(tsEntityToInsert);
            int rowsInserted = await ((DbContext)_dbContext).SaveChangesAsync();

            if (rowsInserted == 0)
                this._logger.LogError("ExecuteAndSaveTSR > Errore nell'inserimento della marca temporale nella DPA_TIMESTAMP_DOC");

            await this.ProcessFileInformation(fileRequest, infoUtente, idTenant);

            return resultMarca;
        }
        private async Task ProcessFileInformation(DocsPaVO.documento.FileRequest fileRequest, InfoUtente infoUtente, long idTenant)
        {
            long versionIdAsLong = fileRequest.versionId.AsLong();
            long docnumberAsLong = fileRequest.docNumber.AsLong();
            bool disabled = true;
            (string? config, bool found) = await this._configurationService.TryGetValue<string>("BE_PROCESS_FILEINFO");

            if (!string.IsNullOrEmpty(config))
                if ((config == "1") || (config.ToLower() == "true"))
                    disabled = false;
            if (disabled)
                return;

            if (fileRequest.repositoryContext != null)
                return;

            if (String.IsNullOrEmpty(fileRequest.docNumber))
                return;

            string fileNameBack = fileRequest.fileName;
            string fileInfoMask = await this._dbContext.ComponentEntities
                .Where(x => x.DOCNUMBER == docnumberAsLong && x.VERSION_ID == versionIdAsLong)
                .Select(x => x.FILE_INFO)
                .FirstOrDefaultAsync() ?? string.Empty;

            FileInformation fileInfo = DocsPaVO.documento.FileInformation.decodeMask(fileInfoMask);

            fileRequest.version = await this._dbContext.VersionEntities
                .Where(x => x.VERSION_ID == versionIdAsLong)
                .Select(x => x.VERSION.ToString())
                .FirstOrDefaultAsync() ?? string.Empty;
            fileRequest.versionLabel = fileRequest.version;

            //1) prelevo il file
            DocsPaVO.documento.FileDocumento filedoc = (await this._mediator.Send(new Requests.DocumentoGetFile(fileRequest, infoUtente))).output;
            if (!String.IsNullOrEmpty(filedoc.name))
            {
                if (Path.GetExtension(filedoc.name).ToLowerInvariant().Contains("pdf"))
                {
                    fileInfo.PdfVer = Encoding.UTF8.GetString(filedoc.content, 5, 3);
                }
            }

            DocsPaVO.documento.FileDocumento filedocfirmato = (await this._mediator.Send(new Requests.DocumentoGetFileFirmato(fileRequest, infoUtente))).output;
            FileValidationResult fileValidateResult = null;
            var fileName = filedocfirmato.name ?? filedocfirmato.fullName;
            using (var stream = new MemoryStream(filedocfirmato.content))
            {
                fileValidateResult = await this._fileValidatorService.Validate(new FileToValidate()
                {
                    Name = fileName,
                    Stream = stream
                });
            }

            string fileExtension = Path.GetExtension(fileName);

            if (fileValidateResult != null && fileValidateResult.Compliance != null)
            {
                bool macroOrExe = fileValidateResult.Compliance.IsExecutable != null && (bool)fileValidateResult.Compliance.IsExecutable;
                fileInfo.NoMacroOrExe = macroOrExe ? VerifyStatus.Invalid : VerifyStatus.Valid;

                bool fileformatOK = fileValidateResult.Compliance.IsCompliantToFormat;
                fileInfo.FileFormatOK = fileformatOK ? VerifyStatus.Valid : VerifyStatus.Invalid;
            }

            string extOFN = System.IO.Path.GetExtension(filedocfirmato.nomeOriginale);

            //Test sull' Impronta
            string impronta = await this._dbContext.ComponentEntities
                .Where(x => x.VERSION_ID == versionIdAsLong && x.DOCNUMBER == docnumberAsLong).Select(x => x.VAR_IMPRONTA).FirstOrDefaultAsync() ?? string.Empty;

            if (string.IsNullOrEmpty(impronta))
                fileInfo.FileHashOK = VerifyStatus.Untested;
            else
            {
                //manca sha1, sul vecchio servizio lo faceva
                var computedHashSHA256 = BitConverter.ToString(filedocfirmato.content.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant();
                fileInfo.FileHashOK = impronta.Equals(computedHashSHA256) ? VerifyStatus.Valid : VerifyStatus.Invalid;
            }

            //controllo TS
            List<DocsPaVO.documento.TimestampDoc> tsAl = await this.GetTimestampsDoc(fileRequest);
            if (tsAl.Count == 0)     //TS non presenti
                fileInfo.TimeStampStatus = VerifyStatus.NotApplicable;

            var tsInfo = tsAl.FirstOrDefault();

            if (tsInfo != null)
            {
                //solo il primo ovvero l'ultima marca

                OutputResponseMarca resultMarca = await this.VerificaMarca(filedocfirmato.content, Convert.FromBase64String(tsInfo.TSR_FILE));
                if (resultMarca.esito == "OK")
                    fileInfo.TimeStampStatus = VerifyStatus.Valid;
                else
                {
                    //spostato all'inizio, nel vecchio be � alla fine ma cos� viene messo sempre a Invalid
                    fileInfo.TimeStampStatus = VerifyStatus.Invalid;
                    //gestire se � TSD o M7M, il controllo non va fatto dato che sar� errato 
                    if (extOFN.ToUpperInvariant().Contains("TSD") || extOFN.ToUpperInvariant().Contains("M7M"))
                    {
                        resultMarca = await this.VerificaMarca(await this.SbustaFileTimestamped(filedocfirmato.content), Convert.FromBase64String(tsInfo.TSR_FILE));
                        fileInfo.TimeStampStatus = VerifyStatus.NotApplicable;
                        if (resultMarca.esito == "OK")
                            fileInfo.TimeStampStatus = Convert.ToDateTime(tsInfo.DTA_SCADENZA) < System.DateTime.Now ? VerifyStatus.Expired : VerifyStatus.Valid;
                    }
                }

                if (Convert.ToDateTime(tsInfo.DTA_SCADENZA) < System.DateTime.Now)
                    fileInfo.TimeStampStatus = VerifyStatus.Expired;
            }

            if (fileInfo.CheckRefDate == DateTime.MinValue)
                fileInfo.CheckRefDate = DateTime.Now;

            if ((fileInfo.Signature != VerifyStatus.Valid) &&
               (fileInfo.CrlStatus != VerifyStatus.Valid))
            {
                //Firma non valida o non verificata
                if (fileRequest.firmato == "1")
                {
                    //Firmato, verifica schedulata
                    fileInfo.CrlStatus = VerifyStatus.InProgress;
                    fileInfo.Signature = VerifyStatus.InProgress;
                    fileInfo.CheckRefDate = DateTime.MinValue; //rimetto il minvalue, dato che la data la metto alla fine della verifica crl
                }
                else
                {
                    //Non Firmato
                    fileInfo.CrlStatus = VerifyStatus.NotApplicable;
                    fileInfo.Signature = VerifyStatus.NotApplicable;
                }
            }

            string estensione = Path.GetExtension(filedoc.name).ToUpperInvariant();
            //tolgo il punto davanti all'aestensione nel caso esista.
            if (estensione.StartsWith(".")) estensione = estensione.Substring(1);

            DocsPaVO.FormatiDocumento.SupportedFileType[] fileTypes = (await this._mediator.Send(new Application.Requests.GetSupportedFileTypes(Convert.ToInt32(idTenant)))).output;
            DocsPaVO.FormatiDocumento.SupportedFileType FileType = (from fileType in fileTypes where fileType.FileExtension.ToUpper().Equals(estensione) select fileType).FirstOrDefault();

            fileInfo.Preservable = FileType.FileTypePreservation ? VerifyStatus.Valid : VerifyStatus.Invalid;
            fileInfo.Signable = FileType.FileTypeSignature ? VerifyStatus.Valid : VerifyStatus.Invalid;

            if (fileInfo.AdminRefDate == DateTime.MinValue)
                fileInfo.AdminRefDate = DateTime.Now;

            fileInfo.setGlobalStatus();
            string fileInfoStr = DocsPaVO.documento.FileInformation.encodeMask(fileInfo);

            var componentEntityToUpdate = await this._dbContext.ComponentEntities
                .Where(x => x.VERSION_ID == versionIdAsLong && x.DOCNUMBER == docnumberAsLong)
                .FirstOrDefaultAsync();

            if (componentEntityToUpdate != null)
                componentEntityToUpdate.FILE_INFO = fileInfoMask;

            int rowsUpdated = await ((DbContext)this._dbContext).SaveChangesAsync();

            //if (rowsUpdated == 0)
            //    this._logger.LogError("ExecuteAndSaveTSR > ProcessFileInformation > Update fileInfoMask non andato a buon fine");

            fileRequest.fileName = fileNameBack;

            var versione = await this._dbContext.VersionEntities
                .Where(x => x.DOCNUMBER == docnumberAsLong)
                .Select(x => new { VERSION_ID = x.VERSION_ID, VERSION = x.VERSION })
                .OrderByDescending(x => x.VERSION)
                .FirstOrDefaultAsync();


            bool isUltimaVersione = fileRequest.versionId.Equals(versione.VERSION_ID.ToString());
            if (isUltimaVersione)
            {
                fileRequest.dataAcquisizione = (await this._dbContext.ComponentEntities
                    .Where(c => c.VERSION_ID == versionIdAsLong)
                    .Select(x => x.DTA_FILE_ACQUIRED)
                    .FirstOrDefaultAsync()).ToString() ?? string.Empty;

                await this.UpdateInfoFileAcquisito(fileInfo, fileRequest, filedoc.nomeOriginale, fileExtension);
            }
        }

        private async Task UpdateInfoFileAcquisito(FileInformation fileInfo, DocsPaVO.documento.FileRequest fileRequest, string varNomeOriginale, string fileExtFileTypeFinder, bool noNotifica = false)
        {
            bool notifica = false;
            InfoFile infoFile = new InfoFile();
            infoFile.IdProfile = fileRequest.docNumber;
            infoFile.VersionId = fileRequest.versionId;
            infoFile.NomeFile = varNomeOriginale;
            infoFile.Estensione = GetEstensioneIntoSignedFile(fileRequest.fileName);
            if (fileInfo.FileFormatOK.Equals(FileInformation.VerifyStatus.Invalid))
                infoFile.EstensioneConforme = false;

            if (fileInfo.NoMacroOrExe.Equals(FileInformation.VerifyStatus.Invalid))
            {
                infoFile.Conforme = false;
                if (fileExtFileTypeFinder.ToLower().Contains("+macro") || fileExtFileTypeFinder.ToLower().Contains("docm") ||
                        fileExtFileTypeFinder.ToLower().Contains("xlsm") || fileExtFileTypeFinder.ToLower().Contains("pptm"))
                    infoFile.ContieneMacro = true;

                infoFile.ContieneForms = fileExtFileTypeFinder.ToLower().Contains("+forms");
                infoFile.ContieneJavascript = fileExtFileTypeFinder.ToLower().Contains("+javascript");

                if (infoFile.ContieneMacro)
                    infoFile.DescrizioneInfoFile += "MACRO";
                if (infoFile.ContieneForms)
                    infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "FormPDF" : ",FormPDF";
                if (infoFile.ContieneJavascript)
                    infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "JAVASCRIPT" : ",JAVASCRIPT";
                if (!infoFile.EstensioneConforme)
                    infoFile.DescrizioneInfoFile += string.IsNullOrEmpty(infoFile.DescrizioneInfoFile) ? "NON_CONFORME" : ",NON_CONFORME";
                infoFile.DataAcquisizione = fileRequest.dataAcquisizione;

                if (!infoFile.Conforme)
                {
                    infoFile.IdDocumentoPrincipale = await this.GetIdDocumentoPrincipale(infoFile.IdProfile);
                    notifica = true && !noNotifica;
                }

                var infoFileToUpdate = await this._dbContext.InfoFileEntities
                    .Where(x => x.ID_PROFILE == infoFile.IdProfile.AsLong())
                    .FirstOrDefaultAsync();
                if (infoFileToUpdate != null)
                {
                    infoFileToUpdate.VERSION_ID = infoFile.VersionId.AsLong();
                    infoFileToUpdate.DTA_ACQUISIZIONE = infoFile.DataAcquisizione.AsDateTime();
                    infoFileToUpdate.VAR_ESTENSIONE = infoFile.Estensione;
                    infoFileToUpdate.VAR_NOME_FILE = infoFile.NomeFile;
                    infoFileToUpdate.VAR_DESC_INFO_FILE = infoFile.DescrizioneInfoFile;
                    infoFileToUpdate.CHA_CONFORME = infoFile.Conforme ? "1" : "0";
                    infoFileToUpdate.CHA_ESTENSIONE_CONFORME = infoFile.EstensioneConforme ? "1" : "0";
                    infoFileToUpdate.CHA_PRESENZA_MACRO = infoFile.ContieneMacro ? "1" : "0";
                    infoFileToUpdate.CHA_PRESENZA_FORMS = infoFile.ContieneForms ? "1" : "0";
                    infoFileToUpdate.CHA_PRESENZA_JAVASCRIPT = infoFile.ContieneJavascript ? "1" : "0";
                    infoFileToUpdate.CHA_NOTIFICA = "0";
                }

                int rowsUpdated = await ((DbContext)this._dbContext).SaveChangesAsync();
                if (rowsUpdated != 0 && notifica)
                {
                    long idProfile = (string.IsNullOrEmpty(infoFile.IdDocumentoPrincipale) ? infoFile.IdProfile : infoFile.IdDocumentoPrincipale).AsLong();
                    var infoFileToUpdateNotifica = await this._dbContext.InfoFileEntities
                        .Where(x => x.ID_PROFILE == idProfile)
                        .FirstOrDefaultAsync();

                    if (infoFileToUpdateNotifica != null)
                        infoFileToUpdateNotifica.CHA_NOTIFICA = "1";

                    int rowsUpdatedNotifica = await ((DbContext)this._dbContext).SaveChangesAsync();
                }
            }
        }

        private async Task<string> GetIdDocumentoPrincipale(string idProfile)
        {
            long docNumberAsLong = idProfile.AsLong();
            return await this._dbContext.ProfileEntities
                .Where(x => x.DOCNUMBER == docNumberAsLong)
                .Select(x => x.ID_DOCUMENTO_PRINCIPALE.ToString())
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        #region Marca temporale
        private async Task<OutputResponseMarca> GetTimeStamp(InputMarca richiesta, string fileName)
        {
            OutputResponseMarca retval = new OutputResponseMarca();
            var fileByteArray = StringToBytes(richiesta.file_p7m);
            string hexHash = BitConverter.ToString(fileByteArray.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant();
            var marcaTsrResponse = await this._marcaTemporaleService.MarcaTsrHash(new MarcaTsrHashRequest()
            {
                Hash = fileByteArray.ComputeHashAsSha256(),
            });

            if (marcaTsrResponse != null)
            {
                retval.marca = Convert.ToBase64String(marcaTsrResponse.Marca);
                retval.sernum = marcaTsrResponse.SerialNumberMarca;
                retval.TSA = new TSARFC2253() { TSARFC2253Name = marcaTsrResponse.TSAName };//giusto?
                retval.fhash = hexHash;
                retval.docm_date = marcaTsrResponse.DataOraMarca.ToString();
                retval.esito = "OK";
            }

            //var marcaTsdResponse = await this._marcaTemporaleService.MarcaTsd(new MarcaTsdRequest()
            //{
            //    FileDaMarcare = new FileMarcatura()
            //    {
            //        FileName = fileName,
            //        FileBase64 = fileByteArray
            //    }
            //});

            //if (marcaTsdResponse != null)
            //{
            //    retval.marca = Convert.ToBase64String(marcaTsdResponse.FileMarcato.FileBase64);
            //    retval.sernum = marcaTsdResponse.SerialNumberMarca;
            //    retval.TSA = new TSARFC2253() { TSARFC2253Name = marcaTsdResponse.TSAName };//giusto?
            //    retval.fhash = hexHash;
            //    retval.docm_date = marcaTsdResponse.DataOraMarca.ToString();
            //    retval.esito = "OK";
            //}

            return retval;
        }
        private async Task<OutputResponseMarca> VerificaMarca(byte[] p7m, byte[] tSR)
        {
            OutputResponseMarca outTSR = null;
            var verificaMarcaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
            {
                VerificaCompleta = false,
                FileFirmato = tSR,
                FileOriginale = p7m,
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
                    string hexHash = BitConverter.ToString(p7m.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant();
                    outTSR.fhash = hexHash;
                    outTSR.docm = DateTime.ParseExact(timestampNode.SelectSingleNode("verificationTime")?.InnerText, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).ToString("HH:mm:ss");
                    outTSR.docm_date = DateTime.ParseExact(timestampNode.SelectSingleNode("verificationTime")?.InnerText, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).AsDateTimeFormat();
                    outTSR.marca = Convert.ToBase64String(tSR); //BitConverter.ToString(tSR.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant(); //Convert.ToBase64String(ParseHex(timestampNode.SelectSingleNode("timeStampImprint")?.InnerText));
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
        private async Task<byte[]> SbustaFileTimestamped(byte[] content)
        {
            byte[] result = null;
            var verificaMarcaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
            {
                VerificaCompleta = false,
                FileFirmato = content,
                DataVerifica = DateTime.Now,
                TipoVerifica = TipiVerifica.Esterna,
                ReturnFileOriginale = true,
                ReturnXmlCompleto = true,
            });

            if (verificaMarcaResponse != null && verificaMarcaResponse.Documento != null && verificaMarcaResponse.Documento.FileOriginale != null)
                result = verificaMarcaResponse.Documento.FileOriginale;

            return result;
        }
        private async Task<List<TimestampDoc>> GetTimestampsDoc(DocsPaVO.documento.FileRequest fileRequest)
        {
            var entities = await this._dbContext.TimestampDocEntities
                .Where(x => x.VERSION_ID == fileRequest.versionId.AsLong() && x.DOC_NUMBER == fileRequest.docNumber.AsLong())
                .ToListAsync();

            return this._mapper.Map<List<TimestampDoc>>(entities);
        }
        #endregion

        #region Utility
        public static string GetEstensioneIntoSignedFile(string fullname)
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
        private static byte[] StringToBytes(string strInput)
        {
            // i variable used to hold position in string
            int i = 0;
            // x variable used to hold byte array element position
            int x = 0;
            // allocate byte array based on half of string length
            byte[] bytes = new byte[(strInput.Length) / 2];
            // loop through the string - 2 bytes at a time converting
            //  it to decimal equivalent and store in byte array
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2;
                ++x;
            }
            // return the finished byte array of decimal values
            return bytes;
        }
        #endregion

        protected readonly ILogger<executeAndSaveTSR_AMHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IFileValidatorService _fileValidatorService;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;
        protected readonly IMarcaTemporaleService _marcaTemporaleService;
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