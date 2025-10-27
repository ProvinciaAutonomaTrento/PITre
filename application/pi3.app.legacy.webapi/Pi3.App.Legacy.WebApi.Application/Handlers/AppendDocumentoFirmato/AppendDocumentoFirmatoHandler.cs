// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
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
using AppendDocumentoFirmatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.AppendDocumentoFirmato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AppendDocumentoFirmato
{
    public class AppendDocumentoFirmatoHandler : IRequestHandler<AppendDocumentoFirmatoRequest, AppendDocumentoFirmatoResult>
    {
        #region Public Members

        public AppendDocumentoFirmatoHandler(ILogger<AppendDocumentoFirmatoHandler> logger,
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
        }

        public async Task<AppendDocumentoFirmatoResult> Handle(AppendDocumentoFirmatoRequest request, CancellationToken cancellationToken)
        {
            bool retValue = true;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var groupId = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
            DocsPaVO.documento.FileRequest fileRequest = request.fileRequest;
            var signedContent = Convert.FromBase64String(request.base64content);
            bool cofirma = request.cofirma;
            bool isConvertedToPdf = false;
            var isPades = false;

            try
            {
                DocsPaVO.documento.FileRequest fileRequest_old = (DocsPaVO.documento.FileRequest)fileRequest.Clone();

                if (fileRequest.repositoryContext == null)
                {
                    // Verifica stato di consolidamento del documento, solamente se non si sta firmando nel repository context
                    bool canExecuteAction = await this.CanExecuteAction(fileRequest.docNumber, ConsolidationActionsDeniedEnum.SignDocument, true);
                }

                if (fileRequest != null && !String.IsNullOrEmpty(fileRequest.fileName) && (fileRequest.fileName.ToLower().EndsWith("pdf_convertito") || isConvertedToPdf))
                {
                    fileRequest.fileName = System.IO.Path.GetFileNameWithoutExtension(fileRequest.fileName) + ".pdf";
                    isConvertedToPdf = true;
                }

                if (!await this.IsFormatSupportedForSign(Convert.ToInt32(idTenant), fileRequest))
                    throw new FileNotSupportedPi3Exception();

                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();

                fileDoc.content = signedContent;
                fileDoc.length = fileDoc.content.Length;
                string nomeOriginale = await this.GetOriginalFileName(fileRequest);

                if (isPades)
                {
                    // INC000001085991 PITRE - conversione in pdf su firma pades non aggiorna l'estensione
                    // fileDoc.nomeOriginale = nomeOriginale ;
                    if (!string.IsNullOrEmpty(nomeOriginale))
                    {
                        //se il filename finisce PDF probabilmente � stato convertito.
                        //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
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
                    if (cofirma && System.IO.Path.GetExtension(nomeOriginale).ToUpper().Equals(".P7M"))
                    {
                        app.estensione = GetAppSuffix(fileRequest.fileName);
                        fileDoc.name = fileRequest.fileName;

                        if (!string.IsNullOrEmpty(nomeOriginale))
                        {
                            //se il filename finisce PDF probabilmente � stato convertito.
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
                            //se il filename finisce PDF probabilmente � stato convertito.
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

                bool addNewAttatchment = false;
                bool isAllegato = (fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)));

                //La chiave c � sempre attiva per tutti gli enti del PiTre
                if (isAllegato && fileRequest.repositoryContext == null)
                {
                    // La firma digitale per l'allegato viene fatta solamente se il documento gi� esiste su database

                    // Se � attiva la gestione di profilazione degli allegati,
                    // deve essere aggiunta una nuova versione del documento.
                    // Altrimenti, deve essere creato un nuovo allegato.
                    addNewAttatchment = false;
                }

                if (addNewAttatchment)
                {
                    fileRequest.docNumber = await this.GetIdDocumentoPrincipale((DocsPaVO.documento.Allegato)fileRequest);
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

                    bool setDataFirma = await this.SetDataFirmaDocumento(fileRequest.docNumber, fileRequest.versionId);

                    if (!isAllegato)
                        ((DocsPaVO.documento.Documento)fileRequest).daInviare = "1";
                }

                List<DocsPaVO.LibroFirma.FirmaElettronica> firmaE = await this.GetFirmaElettronicaDaFileRequest(fileRequest_old);
                bool isFirmatoElettonicamente = firmaE != null && firmaE.Count > 0;
                fileRequest.tipoFirma = isFirmatoElettonicamente ? DocsPaVO.documento.TipoFirma.ELETTORNICA : fileRequest.tipoFirma;

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
                        IdVersion = fileRequest.versionId,
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
                    long versionId = fileRequest.versionId.AsLong();
                    long docNumber = fileRequest.docNumber.AsLong();
                    string impronta = await this._dbContext.ComponentEntities
                        .Where(x => x.VERSION_ID == versionId && x.DOCNUMBER == docNumber)
                        .Select(x => x.VAR_IMPRONTA)
                        .FirstOrDefaultAsync() ?? string.Empty;

                    foreach (DocsPaVO.LibroFirma.FirmaElettronica firma in firmaE)
                    {
                        firma.UpdateXml(impronta, fileRequest.versionId, fileRequest.version, fileRequest.docNumber);
                        await this.InserisciFirmaElettronica(firma);
                    }
                }

                if (retValue)
                {
                    string description = Resources.SignedFileLogMessage;
                    // Non viene settato nel FE
                    if ((await this._mediator.Send(new Requests.IsDocInLibroFirma(fileRequest.docNumber))).output)
                    {
                        await this.AggiornaDataEsecuzioneElemento(fileRequest.docNumber.AsLong(), TipoStatoElemento.FIRMATO.ToString());
                        await this.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fileRequest.docNumber.AsLong(), description, idPeople.AsLong(), userId, groupId, idPeopleDelegato);
                        //Inserisco nella coda del motore di Libro firma
                        await this._mediator.Send(
                            new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                            {
                                IdProfile = fileRequest.docNumber,
                                Evento = "DOC_SIGNATURE",
                            }));
                    }
                    await this._webMethodLoggerService.LogOK("DOC_SIGNATURE", fileRequest.docNumber, description);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                retValue = false;
            }

            return new AppendDocumentoFirmatoResult(retValue, fileRequest);
        }

        public async Task<AppendDocumentoFirmatoResult> HandleOld(AppendDocumentoFirmatoRequest request, CancellationToken cancellationToken)
        {
            bool retValue = true;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var groupId = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
            DocsPaVO.documento.FileRequest fileRequest = request.fileRequest;
            string base64content = request.base64content;
            bool cofirma = request.cofirma;
            bool isConvertedToPdf = false;

            try
            {
                DocsPaVO.documento.FileRequest fileRequest_old = (DocsPaVO.documento.FileRequest)fileRequest.Clone();

                if (fileRequest.repositoryContext == null) // da vedere per� come farlo
                {
                    // Verifica stato di consolidamento del documento, solamente se non si sta firmando nel repository context
                    bool canExecuteAction = await this.CanExecuteAction(fileRequest.docNumber, ConsolidationActionsDeniedEnum.SignDocument, true);
                }

                if (fileRequest != null && !String.IsNullOrEmpty(fileRequest.fileName) && (fileRequest.fileName.ToLower().EndsWith("pdf_convertito") || isConvertedToPdf))
                {
                    fileRequest.fileName = System.IO.Path.GetFileNameWithoutExtension(fileRequest.fileName) + ".pdf";
                    isConvertedToPdf = true;
                }

                if (!await this.IsFormatSupportedForSign(Convert.ToInt32(idTenant), fileRequest))
                    throw new FileNotSupportedPi3Exception();

                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();

                fileDoc.content = Convert.FromBase64String(base64content);
                fileDoc.length = fileDoc.content.Length;
                string nomeOriginale = await this.GetOriginalFileName(fileRequest);

                if (cofirma && System.IO.Path.GetExtension(nomeOriginale).ToUpper().Equals(".P7M"))
                {
                    app.estensione = GetAppSuffix(fileRequest.fileName);
                    fileDoc.name = fileRequest.fileName;

                    if (!string.IsNullOrEmpty(nomeOriginale))
                    {
                        //se il filename finisce PDF probabilmente � stato convertito.
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
                        //se il filename finisce PDF probabilmente � stato convertito.
                        //controllo inoltre se il nomeoriginale non finisce con PDF, in tal caso lo popolo.
                        if ((System.IO.Path.GetExtension(fileRequest.fileName).ToUpper() == ".PDF") &&
                            (System.IO.Path.GetExtension(nomeOriginale).ToUpper() != ".PDF"))
                            nomeOriginale += ".PDF";

                        fileDoc.nomeOriginale = nomeOriginale + ".P7M";
                    }
                    fileDoc.estensioneFile = GetAppSuffix(fileRequest.fileName + ".p7m");
                }
                fileDoc.fullName = fileDoc.name;

                bool addNewAttatchment = true;
                bool isAllegato = (fileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)));

                //La chiave c � sempre attiva per tutti gli enti del PiTre
                if (isAllegato && fileRequest.repositoryContext == null)
                {
                    // La firma digitale per l'allegato viene fatta solamente se il documento gi� esiste su database

                    // Se � attiva la gestione di profilazione degli allegati,
                    // deve essere aggiunta una nuova versione del documento.
                    // Altrimenti, deve essere creato un nuovo allegato.
                    addNewAttatchment = false;
                }

                if (!addNewAttatchment)
                {
                    fileRequest.docNumber = await this.GetIdDocumentoPrincipale((DocsPaVO.documento.Allegato)fileRequest);
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

                    bool setDataFirma = await this.SetDataFirmaDocumento(fileRequest.docNumber, fileRequest.versionId);

                    if (!isAllegato)
                        ((DocsPaVO.documento.Documento)fileRequest).daInviare = "1";
                }

                List<DocsPaVO.LibroFirma.FirmaElettronica> firmaE = await this.GetFirmaElettronicaDaFileRequest(fileRequest_old);
                bool isFirmatoElettonicamente = firmaE != null && firmaE.Count > 0;
                fileRequest.tipoFirma = isFirmatoElettonicamente ? DocsPaVO.documento.TipoFirma.ELETTORNICA : fileRequest.tipoFirma;

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
                        LoadNote = true,
                        MittentiDestinatariPagination = new Pagination() { Skip = 0, Take = 10000 }
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
                        HashName = Core.AggregateModels.DocumentAggregate.ValueObjects.HashNamesEnum.SHA256
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = false,
                        IdVersion = fileRequest.versionId,
                        Name = new TextValue(Resources.SignedVersion)
                    });

                await this._documentoAmministrativoRepository.Update(aggregato);

                if (isFirmatoElettonicamente)
                {
                    long versionId = fileRequest.versionId.AsLong();
                    long docNumber = fileRequest.docNumber.AsLong();
                    string impronta = await this._dbContext.ComponentEntities
                        .Where(x => x.VERSION_ID == versionId && x.DOCNUMBER == docNumber)
                        .Select(x => x.VAR_IMPRONTA)
                        .FirstOrDefaultAsync() ?? string.Empty;

                    foreach (DocsPaVO.LibroFirma.FirmaElettronica firma in firmaE)
                    {
                        firma.UpdateXml(impronta, fileRequest.versionId, fileRequest.version, fileRequest.docNumber);
                        await this.InserisciFirmaElettronica(firma);
                    }
                }

                if (retValue)
                {
                    string description = Resources.SignedFileLogMessage;
                    // Non viene settato nel FE
                    if ((await this._mediator.Send(new Requests.IsDocInLibroFirma(fileRequest.docNumber))).output)
                    {
                        await this.AggiornaDataEsecuzioneElemento(fileRequest.docNumber.AsLong(), TipoStatoElemento.FIRMATO.ToString());
                        await this.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fileRequest.docNumber.AsLong(), description, idPeople.AsLong(), userId, groupId, idPeopleDelegato);
                        //Inserisco nella coda del motore di Libro firma
                        await this._mediator.Send(
                            new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                            {
                                IdProfile = fileRequest.docNumber,
                                Evento = "DOC_SIGNATURE",
                            }));
                    }
                    await this._webMethodLoggerService.LogOK("DOC_SIGNATURE", fileRequest.docNumber, description);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                retValue = false;
            }

            return new AppendDocumentoFirmatoResult(retValue, fileRequest);
        }

        #endregion

        #region Private Members

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
        private async Task<bool> IsFormatSupportedForSign(int idTenant, DocsPaVO.documento.FileRequest? fileRequest)
        {
            bool retValue = false;
            //In PiTre la chiave SUPPORTED_FILE_TYPES_ENABLED � sempre abilita per cui evito il controllo presente sul vecchio BE
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
        private async Task<string> GetIdDocumentoPrincipale(DocsPaVO.documento.Allegato fr)
        {
            long docNumberAsLong = fr.docNumber.AsLong();
            return await this._dbContext.ProfileEntities
                .Where(x => x.DOCNUMBER == docNumberAsLong)
                .Select(x => x.ID_DOCUMENTO_PRINCIPALE.ToString())
                .FirstOrDefaultAsync() ?? string.Empty;
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

        protected readonly ILogger<AppendDocumentoFirmatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IFileValidatorService _fileValidatorService;

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