// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using static Pi3.App.Legacy.WebApi.Application.Handlers.HSM_OpenMultiSignSession.HSM_OpenMultiSignSessionHandler;
using DocsPaVO.documento;
using DocsPaVO.utente;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.WebMethodLogger;
using Microsoft.EntityFrameworkCore;
using DocsPaVO.LibroFirma;
using System.Reflection;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using System.IO;
using DocsPaVO.FlussoAutomatico;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.File.FirmaRemota2;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.HSM_SignMultiSignSession
{

    // Richiede libreria MediatR
    public class HSM_SignMultiSignSessionHandler : IRequestHandler<Requests.HSM_SignMultiSignSession, HSM_SignMultiSignSessionResult>
    {
        #region Public Members

        public HSM_SignMultiSignSessionHandler(ILogger<HSM_SignMultiSignSessionHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IFirmaRemota2Service firmaRemotaService,
            IFileConverterService converterService,
            IDocumentBlobRepository documentBlobRepository,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IWebMethodLoggerService webMethodLoggerService,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._firmaRemotaService = firmaRemotaService;
            this._converterService = converterService;
            this._documentBlobRepository = documentBlobRepository;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._webMethodLoggerService = webMethodLoggerService;
            _fileValidatorService = fileValidatorService;
        }

        public async Task<HSM_SignMultiSignSessionResult> Handle(Requests.HSM_SignMultiSignSession request, CancellationToken cancellationToken)
        {

            List<FirmaResult> retval = new List<FirmaResult>();
            FirmaResult esitoComplessivo = new FirmaResult();

            DocsPaVO.documento.FileRequest[] fileRequestList = request.fileRequestList;
            string multiSignToken = request.MultiSignToken;
            string aliasCertificato = request.AliasCertificato;
            string dominioCertificato = request.DominioCertificato;
            string otpFirma = request.OtpFirma;
            string pinCertificato = request.PinCertificato;
            bool cofirma = request.cofirma;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var groupId = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
            var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);


            _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO UserId: {userId}");

            try
            {
                List<byte[]> filesRead = new List<byte[]>();
                string[] tokenLst = multiSignToken.Split('|');
                string sessionToken = tokenLst[0];

                string descrizioneFirmatario = await GetSubjectNameSignature(aliasCertificato, dominioCertificato, pinCertificato);
                if (string.IsNullOrEmpty(descrizioneFirmatario))
                    descrizioneFirmatario = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteByIdPeople(idPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, new InfoUtente()))).output.descrizione;

                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

                var directory = Path.Combine(
                            repositoryRootPath,
                            tenantCode.ToUpper(),
                            "TemporaryUploads",
                            sessionToken)
                    .PathAsUnixPath();

                _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO recupero file Manifest.xml {directory} UserId: {userId}");

                var manifestFile = Path.Combine(directory, "Manifest.xml");

                if (System.IO.File.Exists(manifestFile))
                {
                    List<FileDaFirmare> fileDaFirmare = new List<FileDaFirmare>();
                    String manifestXML = System.IO.File.ReadAllText(manifestFile);
                    Manifest.ManifestFile mft = Manifest.ManifestFile.Deserialize(manifestXML);
                    foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                    {
                        _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO recupero content file {FileInformation.OriginalFullName} UserId: {userId}");

                        byte[] content = System.IO.File.ReadAllBytes(Path.Combine(directory, FileInformation.OriginalFullName));

                        filesRead.Add(content);
                        fileDaFirmare.Add(new FileDaFirmare() { FileBase64 = content, FileName = FileInformation.OriginalFullName });

                        _logger.LogDebug($"HSM_SignMultiSignSession: FINE recupero content file {FileInformation.OriginalFullName} UserId: {userId}");
                    }

                    sbyte firmaParallela = 0;

                    if (mft.cosign)
                        firmaParallela = 1;

                    if (mft.SignatureType == Manifest.SignType.CADES)
                    {
                        //retval = client.FirmaRemotaMultiplaP7M(filesRead.ToArray(), aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marca, firmaParallela);

                        var firmaCadesRequest = new FirmaCAdESRequest()
                        {
                            AliasCertificato = aliasCertificato,
                            DominioCertificato = dominioCertificato,
                            OtpFirma = otpFirma,
                            PinCertificato = pinCertificato,
                            MarcaTemporale = mft.timestamp,
                            FilesDaFirmare = fileDaFirmare,
                            unzipOutput = true
                        };

                        _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO chiamata a FirmaCAdESREST UserId: {userId}");

                        FirmaCAdESResponse firmaCadesResponse = await _firmaRemotaService.FirmaCAdESREST(firmaCadesRequest);

                        _logger.LogDebug($"HSM_SignMultiSignSession: FINE chiamata a FirmaCAdESREST UserId: {userId}");

                        if (firmaCadesResponse != null && firmaCadesResponse.FileFirmato != null && firmaCadesResponse.FileFirmato.Count() > 0)
                        {
                            _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO scrittura file firmati UserId: {userId}");

                            foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                            {
                                _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO scrittura file firmato {FileInformation.OriginalFullName}");

                                byte[] content = (firmaCadesResponse.FileFirmato.Where(x => x.FileName.Contains(FileInformation.OriginalFullName)).FirstOrDefault()).FileBase64;
                                string newName = "signed_" + FileInformation.OriginalFullName;
                                System.IO.File.WriteAllBytes(Path.Combine(directory, newName), content);
                                FileInformation.SignedFullName = newName;

                                _logger.LogDebug($"HSM_SignMultiSignSession: FINE scrittura file firmato {FileInformation.OriginalFullName} UserId: {userId}");
                            }

                            _logger.LogDebug($"HSM_SignMultiSignSession: FINE scrittura file firmati UserId: {userId}");

                        }
                        else
                        {
                            _logger.LogDebug($"HSM_SignMultiSignSession: file firmati non restituiti UserId: {userId}");
                        }
                    }
                    else
                    {
                        var firmaPadesRequest = new FirmaPAdESRequest()
                        {
                            AliasCertificato = aliasCertificato,
                            DominioCertificato = dominioCertificato,
                            OtpFirma = otpFirma,
                            PinCertificato = pinCertificato,
                            MarcaTemporale = mft.timestamp,
                            FilesDaFirmare = fileDaFirmare,
                            unzipOutput = true
                        };

                        _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO chiamata a FirmaPAdESREST UserId: {userId}");

                        FirmaPAdESResponse firmaPadesResponse = await _firmaRemotaService.FirmaPAdESREST(firmaPadesRequest);

                        _logger.LogDebug($"HSM_SignMultiSignSession: FINE chiamata a FirmaPAdESREST UserId: {userId}");

                        if (firmaPadesResponse != null && firmaPadesResponse.FileFirmato != null && firmaPadesResponse.FileFirmato.Count() > 0)
                        {
                            _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO scrittura file firmati UserId: {userId}");

                            foreach (Manifest.MainfestFileInformation FileInformation in mft.FileInformation)
                            {
                                _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO scrittura file firmato {FileInformation.OriginalFullName} UserId: {userId}");

                                byte[] content = (firmaPadesResponse.FileFirmato.Where(x => x.FileName.Equals(FileInformation.OriginalFullName)).FirstOrDefault()).FileBase64;
                                string newName = "signed_" + FileInformation.OriginalFullName;
                                System.IO.File.WriteAllBytes(Path.Combine(directory, newName), content);
                                FileInformation.SignedFullName = newName;

                                _logger.LogDebug($"HSM_SignMultiSignSession: FINE scrittura file firmato {FileInformation.OriginalFullName} UserId: {userId}");
                            }

                            _logger.LogDebug($"HSM_SignMultiSignSession: FINE scrittura file firmati UserId: {userId}");
                        }
                        else
                        {
                            _logger.LogDebug($"HSM_SignMultiSignSession: file firmati non restituiti UserId: {userId}");
                        }
                    }

                    foreach (string fileSession in tokenLst)
                    {

                        string[] hashLst = fileSession.Split('§');
                        if (hashLst.Length != 2) //non ci sta la coppia , o è il primo o ci sono problemi
                            continue;

                        string filehash = hashLst[0];
                        string docNumber = hashLst[1];
                        //seleziono l'oggetto FR dal docnumber del token
                        FileRequest fr = (from a in fileRequestList where a.docNumber == docNumber select a).FirstOrDefault();
                        FirmaResult firmres = new FirmaResult { fileRequest = fr };

                        // Non viene settato nel FE
                        fr.inLibroFirma = (await this._mediator.Send(new Requests.IsDocInLibroFirma(fr.docNumber))).output;

                        //non trovato
                        if (fr == null)
                        {
                            firmres.errore = Resources.NullFileRequestError;
                        }
                        else
                        {
                            string signedFullName = mft.FileInformation.FirstOrDefault(x => x.hash.ToUpper() == filehash.ToUpper()).SignedFullName;
                            byte[] signed = System.IO.File.ReadAllBytes(Path.Combine(directory, signedFullName));
                            if (signed != null)
                            {
                                bool signResult = false;

                                if (mft.SignatureType == Manifest.SignType.PADES)
                                {
                                    fr = await this.AppendDocumentoFirmatoPades(signed, mft.cosign, fr, request.infoUtente, idTenant);
                                    signResult = fr != null;
                                }
                                else
                                {
                                    fr = await this.AppendDocumentoFirmato(signed, mft.cosign, fr, request.infoUtente, false, idTenant);
                                    signResult = fr != null;
                                }
                                if (!signResult)
                                {
                                    firmres.errore = Resources.ErrorInNewVersion;
                                    if (fr.inLibroFirma)
                                    {
                                        string[] splitMsg = firmres.errore.Split(':');
                                        await this.AggiornaErroreEsitoFirma(fr.docNumber.AsLong(), splitMsg[1].ToString());
                                    }
                                }
                                else
                                {
                                    //Inserisco nella tabella firmatari documento
                                    FirmatarioDocumento firmatarioDoc = new FirmatarioDocumento() { IdProfile = fr.docNumber, IdVersion = fr.versionId, DescrizioneFirmatario = descrizioneFirmatario };
                                    FirmatarioDocEntity newFirmatarioDocEntity = new FirmatarioDocEntity()
                                    {
                                        ID_PROFILE = fr.docNumber.AsLong(),
                                        ID_VERSION = fr.versionId.AsLong(),
                                        DESCRIZIONE_FIRMATARIO = descrizioneFirmatario.Replace("\"", ""),
                                        DTA_FIRMA = DateTime.Now
                                    };

                                    await this._dbContext.FirmatarioDocEntities.AddAsync(newFirmatarioDocEntity);
                                    int rowsInserted = await ((Pi3DbContext)_dbContext).SaveChangesAsync();

                                    string method = mft.SignatureType == Manifest.SignType.PADES ? "DOC_SIGNATURE_P" : "DOC_SIGNATURE";
                                    string description = mft.SignatureType == Manifest.SignType.PADES ? Resources.SignDescriptionPADES : Resources.SignDescriptionCADES;

                                    await this._webMethodLoggerService.LogOK(method, fr.docNumber, description, null, "PITRE");

                                    firmres.errore = Resources.VersionCreated;

                                    //MEV LIBRO FIRMA EMANUELA 12-06-2015: Se il documento è in libro firma aggiorno la data di esecuzione
                                    if (fr.inLibroFirma)
                                    {
                                        await this.AggiornaDataEsecuzioneElemento(fr.docNumber.AsLong(), DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                                        await this.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fr.docNumber.AsLong(), description, idPeople.AsLong(), userId, groupId, idPeopleDelegato);


                                        //Inserisco nella coda del motore di Libro firma
                                        await this._mediator.Send(
                                            new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                                            {
                                                IdProfile = fr.docNumber,
                                                Evento = method,
                                            }));
                                    }
                                }
                            }
                            else
                            {
                                firmres.errore = Resources.SignError;
                            }
                        }
                        retval.Add(firmres);
                    }
                }
                else
                {
                    _logger.LogDebug($"HSM_SignMultiSignSession: Manifest.xml non trovato. UserId: {userId}");
                }

            }
            catch (Core.Services.File.FirmaRemota2.FirmaRemotaPi3Exception pi3Ex) 
            {
                this._logger.LogError(exception: pi3Ex, message: $"HSM_SignMultiSignSession errore servizio TIBCO {pi3Ex.Message} UserId: {userId}");
                var tibcoMessageError = pi3Ex.Message.Split("\"");
                if (tibcoMessageError.Length == 3)
                {
                    var esito = tibcoMessageError[1].Split('-');

                    if (esito.Count() == 5)
                    {
                        esitoComplessivo.esito = new EsitoFirma()
                        {
                            Codice = esito[3].Trim(),
                            Messaggio = esito[4].Trim()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new HSM_SignMultiSignSessionResult(retval.ToArray(), esitoComplessivo);
        }


        #endregion

        #region Private Members
        private async Task<FileRequest> AppendDocumentoFirmato(byte[] signed, bool cosign, FileRequest fr, InfoUtente infoUtente, bool isPades, long idTenant, bool convertPdf = false)
        {
            DocsPaVO.documento.FileRequest fileRequest_old = (DocsPaVO.documento.FileRequest)fr.Clone();

            if (fr.repositoryContext == null) // da vedere però come farlo
            {
                // Verifica stato di consolidamento del documento, solamente se non si sta firmando nel repository context
                bool canExecuteAction = await this.CanExecuteAction(fr.docNumber, ConsolidationActionsDeniedEnum.SignDocument, true);
            }

            if (fr != null && !String.IsNullOrEmpty(fr.fileName) && (fr.fileName.ToLower().EndsWith("pdf_convertito") || convertPdf))
            {
                fr.fileName = System.IO.Path.GetFileNameWithoutExtension(fr.fileName) + ".pdf";
                convertPdf = true;
            }

            if (!await this.IsFormatSupportedForSign(Convert.ToInt32(idTenant), fr))
                throw new FileNotSupportedPi3Exception();

            DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();

            fileDoc.content = signed;
            fileDoc.length = fileDoc.content.Length;
            string nomeOriginale = await this.GetOriginalFileName(fr);

            if (isPades)
            {
                // INC000001085991 PITRE - conversione in pdf su firma pades non aggiorna l'estensione
                // fileDoc.nomeOriginale = nomeOriginale ;
                if (!string.IsNullOrEmpty(nomeOriginale))
                {
                    //se il filename finisce PDF probabilmente è stato convertito.
                    //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                    if ((System.IO.Path.GetExtension(fr.fileName).ToUpper() == ".PDF") &&
                        (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF") && convertPdf)
                        nomeOriginale += ".PDF";

                    fileDoc.nomeOriginale = nomeOriginale;
                }

                fileDoc.estensioneFile = GetAppSuffix(fr.fileName);
                fileDoc.name = fr.fileName;
                app.estensione = GetAppSuffix(fr.fileName);
            }
            else
            {
                if (cosign && System.IO.Path.GetExtension(nomeOriginale).ToUpper().Equals(".P7M"))
                {
                    app.estensione = GetAppSuffix(fr.fileName);
                    fileDoc.name = fr.fileName;

                    if (!string.IsNullOrEmpty(nomeOriginale))
                    {
                        //se il filename finisce PDF probabilmente è stato convertito.
                        //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                        if ((System.IO.Path.GetExtension(fr.fileName).ToUpper() == ".PDF") &&
                            (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF"))
                            nomeOriginale += ".PDF";

                        fileDoc.nomeOriginale = nomeOriginale;
                    }
                    fileDoc.estensioneFile = GetAppSuffix(fr.fileName);
                }
                else
                {
                    app.estensione = GetAppSuffix(fr.fileName + ".P7M");
                    fileDoc.name = fr.fileName + ".P7M";

                    if (!string.IsNullOrEmpty(nomeOriginale))
                    {
                        //se il filename finisce PDF probabilmente è stato convertito.
                        //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                        if ((System.IO.Path.GetExtension(fr.fileName).ToUpper() == ".PDF") &&
                            (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF"))
                            nomeOriginale += ".PDF";

                        fileDoc.nomeOriginale = nomeOriginale + ".P7M";
                    }
                    fileDoc.estensioneFile = GetAppSuffix(fr.fileName + ".p7m");
                }
            }

            fileDoc.fullName = fileDoc.name;

            bool addNewAttatchment = true;
            bool isAllegato = (fr.GetType().Equals(typeof(DocsPaVO.documento.Allegato)));

            //La chiave c è sempre attiva per tutti gli enti del PiTre
            if (isAllegato && fr.repositoryContext == null)
            {
                // La firma digitale per l'allegato viene fatta solamente se il documento già esiste su database

                // Se è attiva la gestione di profilazione degli allegati,
                // deve essere aggiunta una nuova versione del documento.
                // Altrimenti, deve essere creato un nuovo allegato.
                addNewAttatchment = false;
            }

            if (!addNewAttatchment)
            {
                fr.docNumber = await this.GetIdDocumentoPrincipale((DocsPaVO.documento.Allegato)fr);
                fr.descrizione = Resources.SignedAttachment;
                fr.cartaceo = false;
                fr = (await this._mediator.Send(new Application.Requests.DocumentoAggiungiAllegato(new InfoUtente(), (DocsPaVO.documento.Allegato)fr))).output;

                if (fr == null)
                    throw new SignedAttachmentCreationErrorException();
            }
            else
            {
                fr.applicazione = app;
                fr.versionId = "";
                fr.descrizione = Resources.SignedVersion;
                fr.cartaceo = false;
                //fr = BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, infoUtente, false);
                fr = (await this._mediator.Send(new Application.Requests.DocumentoAggiungiVersione(fr, new InfoUtente()))).output;

                if (fr == null)
                    throw new SignedVersionCreationErrorException();

                bool setDataFirma = await this.SetDataFirmaDocumento(fr.docNumber, fr.versionId);

                if (!isAllegato)
                    ((DocsPaVO.documento.Documento)fr).daInviare = "1";
            }

            List<DocsPaVO.LibroFirma.FirmaElettronica> firmaE = await this.GetFirmaElettronicaDaFileRequest(fileRequest_old);
            bool isFirmatoElettonicamente = firmaE != null && firmaE.Count > 0;
            fr.tipoFirma = isFirmatoElettonicamente ? DocsPaVO.documento.TipoFirma.ELETTORNICA : fr.tipoFirma;

            DocumentoAmministrativo aggregato = await this._documentoAmministrativoRepository.Get(idTenant.ToString(), fr.docNumber, new ILoadBehavior[1]
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
                    TipoFirma = isPades ? TipoFirmaEnum.Pades : TipoFirmaEnum.Cades
                },
                new TargetVersionBehavior()
                {
                    CreateNewVersion = false,
                    IdVersion = fr.versionId,
                    Name = new TextValue(Resources.SignedVersion)
                }
                );


            await this._documentoAmministrativoRepository.Update(aggregato);

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
                long versionId = fr.versionId.AsLong();
                long docNumber = fr.docNumber.AsLong();
                string impronta = await this._dbContext.ComponentEntities
                    .Where(x => x.VERSION_ID == versionId && x.DOCNUMBER == docNumber)
                    .Select(x => x.VAR_IMPRONTA)
                    .FirstOrDefaultAsync() ?? string.Empty;

                foreach (DocsPaVO.LibroFirma.FirmaElettronica firma in firmaE)
                {
                    firma.UpdateXml(impronta, fr.versionId, fr.version, fr.docNumber);
                    await this.InserisciFirmaElettronica(firma);
                }
            }

            return fr;
        }
        private async Task<FileRequest> AppendDocumentoFirmatoPades(byte[] signed, bool cosign, FileRequest fr, InfoUtente infoUtente, long idTenant, bool convertPdf = false)
        {
            return await this.AppendDocumentoFirmato(signed, cosign, fr, infoUtente, true, idTenant);
        }
        private async Task<string> GetOriginalFileName(DocsPaVO.documento.FileRequest? fr)
        {
            long versionIdAsLong = fr.versionId.AsLong();
            long docNumberAsLong = fr.docNumber.AsLong();
            return RemoveIllegalChars(await this._dbContext.ComponentEntities
                .Where(x => x.VERSION_ID == versionIdAsLong && x.DOCNUMBER == docNumberAsLong)
                .Select(x => x.VAR_NOMEORIGINALE)
                .FirstOrDefaultAsync() ?? string.Empty, false);
        }
        public static string RemoveIllegalChars(string filename, bool normalizeDotsAndSpacesToo)
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
        private async Task<string> GetSubjectNameSignature(string aliasCertificato, string dominioCertificato, string userPwd)
        {
            string retValue = string.Empty;
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);

            _logger.LogDebug($"HSM_SignMultiSignSession: INIZIO chiamata VisualizzaCertificatoREST UserId: {userId}");

            try
            {
                VisualizzaCertificatoResponse vcRes = await _firmaRemotaService.VisualizzaCertificatoREST(new VisualizzaCertificatoRequest()
                {
                    AliasCertificato = aliasCertificato,
                    DominioCertificato = dominioCertificato,
                    UserPwd = userPwd
                });

                _logger.LogDebug($"HSM_SignMultiSignSession: FINE chiamata VisualizzaCertificatoREST UserId: {userId}");

                if (vcRes != null && vcRes.Esito != null)
                {
                    string subjectName = vcRes.Esito[0].Subject;
                    int startCN = subjectName.ToUpper().LastIndexOf("CN=") + 3;
                    int endCN = subjectName.IndexOf(",", startCN);
                    if (endCN == -1)
                    {
                        endCN = subjectName.Length;
                        string cn = subjectName.Substring(startCN, endCN - startCN).Trim();
                        int startRt = cn.ToUpper().LastIndexOf("\"") + 1;
                        int endRt = cn.IndexOf(@"\", startRt);
                        retValue = cn.Substring(startRt, endRt - startRt).Trim().Replace("   ", " ");
                    }
                    else
                        retValue = subjectName.Substring(startCN, endCN - startCN).Trim();

                }
                else
                {
                    _logger.LogDebug($"HSM_SignMultiSignSession: ERRORE chiamata VisualizzaCertificatoREST UserId: {userId}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogDebug($"HSM_SignMultiSignSession: ERRORE in GetSubjectNameSignature UserId: {userId} Eccezione: {ex.Message}");
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return retValue;
        }
        private async Task<bool> CanExecuteAction(string idProfile, Enum action, bool throwOnError)
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
        private async Task<DocumentConsolidationStateInfo> GetState(long idProfile)
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
        private async Task InserisciFirmaElettronica(FirmaElettronica firma)
        {
            var newFirmaElettronicaEntity = new FirmaElettronicaEntity()
            {
                ID_DOCUMENTO = firma.Docnumber.AsLong(),
                VERSION_ID = firma.Versionid.AsLong(),
                DOC_ALL = firma.DocAll,
                NUM_ALL = firma.NumAll.AsLong(),
                NUMERO_VERSIONE = firma.NumVersione.AsLong(),
                DATA_APPOSIZIONE = DateTime.Now,
                XML = firma.Xml
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
        private async Task<bool> SetDataFirmaDocumento(string docNumber, string versionId)
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
        private async Task<string> GetIdDocumentoPrincipale(DocsPaVO.documento.Allegato fr)
        {
            long docNumberAsLong = fr.docNumber.AsLong();
            return await this._dbContext.ProfileEntities
                .Where(x => x.DOCNUMBER == docNumberAsLong)
                .Select(x => x.ID_DOCUMENTO_PRINCIPALE.ToString())
                .FirstOrDefaultAsync() ?? string.Empty;
        }
        private string GetAppSuffix(string fileName)
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
        private async Task<bool> IsFormatSupportedForSign(int idTenant, DocsPaVO.documento.FileRequest? fileRequest)
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
        private async Task AggiornaErroreEsitoFirma(long docNumber, string msgError)
        {
            var elInLfEntity = await this._dbContext.ElementoInLibroFirmaEntities
                .Where(x => x.DOC_NUMBER == docNumber && x.DTA_ESECUZIONE == null)
                .FirstOrDefaultAsync();

            if (elInLfEntity != null)
                elInLfEntity.ERRORE_FIRMA = msgError;

            int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();
        }
        private async Task SalvaStoricoIstanzaProcessoFirmaByDocnumber(long docNumber, string description, long idPeople, string userId, long groupId, long idPeopleDelegato)
        {
            var istanza = await this._dbContext.IstanzaProcessoFirmaEntities
                .Where(x => x.ID_DOCUMENTO == docNumber && x.STATO.Equals("IN_EXEC"))
                .Select(x => new { ID_ISTANZA = x.ID_ISTANZA, CHA_CAMBIO_STATO_DIAG = x.CHA_CAMBIO_STATO_DIAG })
                .FirstOrDefaultAsync();

            if (istanza != null)
            {
                long corrGlobaliGroup = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == groupId).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
                var istanzaProcessoFirmaStoEntity = new IstanzaProcFirmaStoEntity()
                {
                    ID_USER = userId,
                    DOC_NUMBER = docNumber,
                    ID_ISTANZA_PROCESSO = istanza.ID_ISTANZA,
                    DTA_DATE = DateTime.Now,
                    VAR_DESC_AZIONE = description.Replace("'", "''"),
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = groupId == 0 ? 0 : corrGlobaliGroup,
                    ID_PEOPLE_DELEGATO = idPeopleDelegato,
                    CHA_CAMBIO_STATO_DIAG = istanza.CHA_CAMBIO_STATO_DIAG
                };

                await this._dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcessoFirmaStoEntity);
                int rowsInserted = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
            }
        }
        private async Task AggiornaDataEsecuzioneElemento(long docNumber, string stato)
        {
            var elInLfEntity = await this._dbContext.ElementoInLibroFirmaEntities
                .Where(x => x.DOC_NUMBER == docNumber && x.DTA_ESECUZIONE == null)
                .FirstOrDefaultAsync();

            if (elInLfEntity != null)
            {
                elInLfEntity.DOC_NUMBER = docNumber;
                elInLfEntity.STATO_FIRMA = stato;
                elInLfEntity.DTA_ESECUZIONE = DateTime.Now;
            }

            int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected readonly ILogger<HSM_SignMultiSignSessionHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly IFirmaRemota2Service _firmaRemotaService;
        protected IFileConverterService _converterService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IFileValidatorService _fileValidatorService;


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
        #endregion
    }

}
