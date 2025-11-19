// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.InviaEmailTrasmissione;
using Pi3.App.InteropPitre.WebApi.Application.Exceptions;
using Pi3.App.InteropPitre.WebApi.Application.Services.RabbitMQ;
using Pi3.App.InteropPitre.WebApi.Extensions;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.AuthToken;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.CommonAddressBook;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.SendProof;
using Pi3.App.InteropPitre.WebApi.Models;
using Pi3.App.InteropPitre.WebApi.Resources;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using Pi3.Infrastructure.Legacy.EF.Services.FileValidator;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;
using Serilog.Events;
using StackExchange.Redis;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text.Json;
using System.Xml;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.ElaborateNewInteroperabilityMessage
{
    public class ElaborateNewInteroperabilityMessageHandler : IRequestHandler<ElaborateNewInteroperabilityMessageRequest, ElaborateNewInteroperabilityMessageResponse>
    {
        #region Public Members

        public ElaborateNewInteroperabilityMessageHandler(ILogger<ElaborateNewInteroperabilityMessageHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, 
            IPi3DbContext dbContext, IDocumentoAmministrativoRepository documentoAmministrativoRepository, ITrasmissioneRepository trasmissioneRepository, INotaRepository notaRepository,
            IUOCorrispondenteRepository uoCorrispondenteRepository, IDocumentBlobRepository documentBlobRepository, IDistributedCache distributedCache, 
            IAuthToken tokenService, ICorrispondenti corrispondentiService, IInteroperabilityService interoperabilityService, IHttpContextAccessor httpContextAccessor, IConfigurationService configurationService,
            IWebMethodLoggerService webMethodLoggerService,
            IPAdESService pAdESService,
            IFileValidatorService fileValidatorService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._trasmissioneRepository = trasmissioneRepository;
            this._notaRepository = notaRepository;
            this._uoCorrispondenteRepository = uoCorrispondenteRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._distributedCache = distributedCache;
            this._tokenService = tokenService;
            this._corrispondentiService = corrispondentiService;
            this._interoperabilityService = interoperabilityService;
            this._httpContextAccessor = httpContextAccessor;
            this._configurationService = configurationService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._pAdESService = pAdESService;
            this._fileValidatorService = fileValidatorService;
        }

        public async Task<ElaborateNewInteroperabilityMessageResponse> Handle(ElaborateNewInteroperabilityMessageRequest request, CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"New request: {JsonSerializer.Serialize<ElaborateNewInteroperabilityMessageRequest>(request)}");

            var result = new ElaborateInteroperabilityMessageResult();

            var messageId = Guid.NewGuid().ToString();

            var instance = this._httpContextAccessor.HttpContext?.GetRouteValue("Instance")!.ToString();

            try
            {
                result.MessageId = messageId;

                var receiverCode = String.Join(", ", request.InteroperabilityMessage.Receivers.Select(x => string.Format("'{0}'", x.Code)));

                var entity = new SimpInteropReceivedMessageEntity
                {
                    MESSAGEID = result.MessageId,
                    RECEIVEDPRIVATE = request.InteroperabilityMessage.IsPrivate ? 1 : 0,
                    RECEIVEDDATE = DateTime.Now,
                    SUBJECT = request.InteroperabilityMessage.Record.Subject,
                    SENDERDESCRIPTION = request.InteroperabilityMessage.Sender.Code,
                    SENDERURL = request.InteroperabilityMessage.Sender.Url,
                    AOOCODE = request.InteroperabilityMessage.Record.AOOCode,
                    RECORDNUMBER = request.InteroperabilityMessage.Record.RecordNumber.AsLong(),
                    RECORDDATE = request.InteroperabilityMessage.Record.RecordDate,
                    RECEIVERCODE = receiverCode,
                    SENDERADMINISTRATIONCODE = request.InteroperabilityMessage.Record.AdministrationCode
                };

                await this._dbContext.SimpInteropReceivedMessageEntities.AddAsync(entity);

                await ((DbContext)this._dbContext).SaveChangesAsync();
            }
            catch(Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);
                result.MessageId = null;
                await this.InsertLog(string.Empty, ErrorDescriptions.InteroperabilityRequestSaveError, (long)ErrorMessageEnum.Error);

                result.SingleRequestErrors.Add(new ElaborateInteroperabilitySingleMessage
                {
                    ErrorMessage = ErrorDescriptions.InteroperabilityRequestSaveError,
                    Receivers = request.InteroperabilityMessage.Receivers
                });
                return new ElaborateNewInteroperabilityMessageResponse
                {
                    Result = result
                };
            }

            this._logger.LogInformation($"Processing messageID {messageId}...");

            var groupedReceivers = request.InteroperabilityMessage.Receivers.GroupBy(r => r.AOOCode).Select(group => group.ToList()).ToList();

            foreach(var receiverList in groupedReceivers)
            {
                try
                {
                    #region Verifica e recupero configurazioni
                    var administrationCode = receiverList.First().AdministrationCode;
                    var AOOCode = receiverList.First().AOOCode;

                    await this.Impersonate(administrationCode, AOOCode, instance);

                    var userInfo = new
                    {
                        IdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser),
                        IdGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup),
                        IdAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant)
                    };

                    if(userInfo == null || string.IsNullOrWhiteSpace(userInfo.IdUser) || string.IsNullOrWhiteSpace(userInfo.IdGroup) || string.IsNullOrWhiteSpace(userInfo.IdAmm))
                    {
                        this._logger.LogError("Interoperability user not found.");
                        throw new SenderRecipientException(ErrorDescriptions.InteroperabilityUserAndRoleNotFound);
                    }

                    // Ricerca mittente in rubrica comune
                    var senderIdCorrespondent = await this.SearchSender(request.InteroperabilityMessage.Sender.Code, userInfo.IdAmm);

                    if(string.IsNullOrWhiteSpace(senderIdCorrespondent))
                    {
                        // Inserimento log
                        throw new SenderRecipientException(string.Format(ErrorDescriptions.CorrespondentNotFound, messageId, request.InteroperabilityMessage.Sender.Code));
                    }

                    // Verifica se il registro è interoperante
                    var interopSettings = await this._dbContext.AmministraEntities.AsNoTracking()
                        .Join(this._dbContext.RegistroEntities.AsNoTracking(), a => a.SYSTEM_ID, r => r.ID_AMM, (a, r) => new { r, a.VAR_CODICE_AMM })
                        .Join(this._dbContext.InteroperabilitySettingEntities.AsNoTracking(), ar => ar.r.SYSTEM_ID, i => i.REGISTRYID, (ar, i) => new
                        {
                            AdministrationCode = ar.VAR_CODICE_AMM,
                            RegisterCode = ar.r.VAR_CODICE,
                            RegisterId = i.REGISTRYID,
                            IsEnabledInteroperability = i.ISENABLEDINTEROPERABILITY == 1,
                            ManagementMode = i.MANAGEMENTMODE,
                            KeepPrivate = i.KEEPPRIVATE == 1
                        }).Where(x => x.RegisterCode == AOOCode && x.AdministrationCode == administrationCode)
                        .FirstOrDefaultAsync();

                    if (interopSettings == null || !interopSettings.IsEnabledInteroperability)
                    {
                        throw new RegistrationException(AOOCode, ErrorDescriptions.RegistrationNotAllowed);
                    }
                    #endregion

                    #region Creazione documento
                    var aggregateDocument = new DocumentoAmministrativo(
                        userInfo.IdAmm,
                        DateTime.Now,
                        new OggettoDelDocumento
                        {
                            Descrizione = new TextValue(request.InteroperabilityMessage.Record.Subject)
                        },
                        new DatiRegistro()
                        {
                            IdRegistro = interopSettings.RegisterId.ToString()
                        },
                        TipologiaFlussoEnum.E,
                        request.InteroperabilityMessage.IsPrivate ? TipologieVisibilitaEnum.Privata : TipologieVisibilitaEnum.Gerarchica
                        );

                    var deliveryMethod = await this._dbContext.DocumentTypesEntities.Where(d => d.TYPE_ID == "SIMPLIFIEDINTEROPERABILITY")
                        .Select(x => new
                        {
                            ID = x.SYSTEM_ID,
                            x.TYPE_ID,
                            x.DESCRIPTION
                        }).FirstAsync();

                    aggregateDocument.AssignMezzoSpedizione(deliveryMethod.ID.ToString(), new TextValue(deliveryMethod.DESCRIPTION!));

                    aggregateDocument.AssignMittente(new Mittente
                    {
                        Id = senderIdCorrespondent
                    });

                    aggregateDocument.AssignProtocolloMittente(new ProtocolloMittente
                    {
                        Data = request.InteroperabilityMessage.Record.RecordDate,
                        Segnatura = string.Format("{0}{1}{2}",
                            request.InteroperabilityMessage.Record.AOOCode,
                            await this.GetSeparator(administrationCode),
                            request.InteroperabilityMessage.Record.RecordNumber),
                        DataArrivo = await _dbContext.GetSystemDateTime()
                    }); ;

                    aggregateDocument.RichiediRegistrazione(new DatiRichiestaRegistrazione
                    {
                        Predisponi = true
                    });

                    try
                    {
                        await this._documentoAmministrativoRepository.Add(aggregateDocument);

                        await this.InsertLog(aggregateDocument.Id, string.Format("Documento relativo alla richiesta con id {0} creato correttamente.", messageId), (long)ErrorMessageEnum.None);

                        // Aggiornamento tabella messaggi ricevuti
                        var entity = await this._dbContext.SimpInteropReceivedMessageEntities.Where(x => x.MESSAGEID == messageId).FirstAsync();
                        entity.PROFILEID = aggregateDocument.Id.AsLong();

                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }
                    catch(Exception ex)
                    {
                        this._logger.LogCritical(ex, ex.Message);
                        await this.InsertLog(string.Empty, string.Format("{0} Id messaggio: {1}", ErrorDescriptions.DocumentCreationError, messageId), (long)ErrorMessageEnum.Error);
                        throw new DocumentCreationException(ErrorDescriptions.DocumentCreationError);
                    }

                    var docnumber = aggregateDocument.Id.AsLong();

                    this._logger.LogInformation($"Document created - ID={docnumber}");

                    // Associazione nota se richiesto
                    var key = await this._configurationService.GetValue<string>("BE_NOTE_IN_SEGNATURA");

                    if (key == "1" && !string.IsNullOrWhiteSpace(request.InteroperabilityMessage.Note))
                    {
                        var aggregateNote = new Nota(
                            userInfo.IdAmm,
                            DateTime.Now,
                            new TextValue { Value = request.InteroperabilityMessage.Note },
                            null,
                            new AutoreNota { IdUtente = userInfo.IdUser, IdRuolo = userInfo.IdGroup },
                            aggregateDocument.Id,
                            TipiOggettoEnum.Documento,
                            TipoAccessoNotaEnum.Pubblica,
                            string.Empty
                            );

                        await this._notaRepository.Add(aggregateNote);
                    }

                    #endregion

                    #region Upload file
                    try
                    {
                        FileValidationResult? fileValidationResult = null;
                        string filename = string.Empty;
                        DateTime? creationDate = null;

                        if (!string.IsNullOrEmpty(request.InteroperabilityMessage.MainDocument.FileName))
                        {
                            // Recupero dell'immagine e suo caricamento
                            // var aggregateBlobSourceFile = await this._documentBlobRepository.Get(userInfo.IdAmm, request.InteroperabilityMessage.MainDocument.FilePath);

                            var aggregateBlobDestinationFile = new DocumentBlob(
                                userInfo.IdAmm,
                                DateTime.Now,
                                new TextValue(request.InteroperabilityMessage.MainDocument.FileName)
                                );
                            _logger.LogInformation("Inizio UploadFromPath ElaborateNewInteroperabilityMessage: " + request.InteroperabilityMessage.MainDocument.FilePath.PathAsUnixPath());
                            aggregateBlobDestinationFile.UploadFromPath(request.InteroperabilityMessage.MainDocument.FilePath.PathAsUnixPath());
                            _logger.LogInformation("Fine UploadFromPath ElaborateNewInteroperabilityMessage: " + request.InteroperabilityMessage.MainDocument.FilePath);
                            aggregateBlobDestinationFile.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                            var hash = aggregateBlobDestinationFile.Hash;

                            await this._documentBlobRepository.Add(aggregateBlobDestinationFile);

                            fileValidationResult = await this._fileValidatorService.Validate(new FileToValidate()
                            {
                                Name = aggregateBlobDestinationFile.FileName,
                                Stream = aggregateBlobDestinationFile.Stream
                            });
                            if (!fileValidationResult.FormatIsAdmitted)
                                throw new FormatoFileNonAmmessoPi3Exception(Path.GetExtension(GetExtentionIntoSignedFile(request.InteroperabilityMessage.MainDocument.FileName)));

                            byte[] content = null;
                            using (var memoryStream = new MemoryStream())
                            {
                                aggregateBlobDestinationFile.Stream.CopyTo(memoryStream);
                                content = memoryStream.ToArray();
                            }

                            aggregateDocument.AssignDocumentBlobRef(new Core.AggregateModels.DocumentAggregate.ValueObjects.DocumentBlobRef()
                            {
                                FileName = aggregateBlobDestinationFile.FileName,
                                FileSize = aggregateBlobDestinationFile.FileSize,
                                CreationDate = aggregateBlobDestinationFile.CreationDate,
                                ContentType = aggregateBlobDestinationFile.ContentType,
                                Hash = hash,
                                HashName = (HashNamesEnum?)Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256,
                                IdBlob = aggregateBlobDestinationFile.Id,
                                TipoFirma = request.InteroperabilityMessage.MainDocument.Signature
                            }, new TargetVersionBehavior()
                            {
                                CreateNewVersion = false,
                                IdVersion = aggregateDocument.Versions.First().Id
                            });

                            filename = aggregateBlobDestinationFile.FileName;
                            creationDate = aggregateBlobDestinationFile.CreationDate;
                        }

                        await this._documentoAmministrativoRepository.Update(aggregateDocument);

                        await AddInfoFile(aggregateDocument.Id.AsLong(), aggregateDocument.Versions.First().Id.AsLong(), null, filename, creationDate, fileValidationResult);

                        this._logger.LogInformation("Uploaded document content");
                    }
                    catch (Pi3Exception ex)
                    {
                        _logger.LogError("Errore in Caricamento documento file principale:" + ex.Message);
                        await this.InsertLog(docnumber.ToString(), string.Format(ex.Message, docnumber.ToString()), (long)ErrorMessageEnum.Error);
                        throw new FileUploadException(ex.Message, docnumber.ToString());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical("Errore in Caricamento documento file principale:" + ex.Message);
                        await this.InsertLog(docnumber.ToString(), string.Format(ErrorDescriptions.DocumentUploadError, docnumber.ToString()), (long)ErrorMessageEnum.Error);
                        throw new FileUploadException(ErrorDescriptions.DocumentUploadError, docnumber.ToString());
                    }
                    #endregion

                    #region Gestione allegati
                    // Recupero degli allegati se presenti e caricamento
                    int c = 0;

                    foreach(var attachment in request.InteroperabilityMessage.Attachments)
                    {
                        var attachmentName = !string.IsNullOrWhiteSpace(attachment.Name) ? attachment.Name : string.Format("Allegato {0}", attachment.Name);
                        var idAttachment = string.Empty;

                        try
                        {
                            idAttachment = string.Empty;

                            var aggregateAttachment = new DocumentoAmministrativo(
                                userInfo.IdAmm,
                                DateTime.Now,
                                new OggettoDelDocumento()
                                {
                                    Descrizione = new TextValue(attachmentName)
                                },
                                null,
                                null,
                                null,
                                new IdDoc()
                                {
                                    Identiticativo = aggregateDocument.Id
                                }
                                );

                            idAttachment = aggregateAttachment.Id;

                            //Se presente acquisisco il file dell'allegato
                            FileValidationResult? fileValidationResult = null;
                            string filename = string.Empty;
                            DateTime? creationDate = null;

                            if (!string.IsNullOrEmpty(attachment.FileName))
                            {

                                var aggregateBlobAttachment = new DocumentBlob(
                                    userInfo.IdAmm,
                                    DateTime.Now,
                                    new TextValue(attachment.FileName)
                                    );

                                aggregateBlobAttachment.UploadFromPath(attachment.FilePath.PathAsUnixPath());
                                aggregateBlobAttachment.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                                var attachmentHash = aggregateBlobAttachment.Hash;

                                await this._documentBlobRepository.Add(aggregateBlobAttachment);

                                fileValidationResult = await this._fileValidatorService.Validate(new FileToValidate()
                                {
                                    Name = aggregateBlobAttachment.FileName,
                                    Stream = aggregateBlobAttachment.Stream
                                });

                                if (!fileValidationResult.FormatIsAdmitted)
                                    throw new FormatoFileNonAmmessoPi3Exception(Path.GetExtension(GetExtentionIntoSignedFile(attachment.FileName)));

                                byte[] content = null;
                                using (var memoryStream = new MemoryStream())
                                {
                                    aggregateBlobAttachment.Stream.CopyTo(memoryStream);
                                    content = memoryStream.ToArray();
                                }

                                aggregateAttachment.AssignDocumentBlobRef(new DocumentBlobRef()
                                {
                                    FileName = attachment.FileName,
                                    FileSize = aggregateBlobAttachment.FileSize,
                                    CreationDate = aggregateBlobAttachment.CreationDate,
                                    ContentType = aggregateBlobAttachment.ContentType,
                                    Hash = attachmentHash,
                                    HashName = (HashNamesEnum?)Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256,
                                    IdBlob = aggregateBlobAttachment.Id,
                                    TipoFirma = attachment.Signature
                                }, new TargetVersionBehavior()
                                {
                                    CreateNewVersion = true
                                });


                                filename = aggregateBlobAttachment.FileName;
                                creationDate = aggregateBlobAttachment.CreationDate;
                            }

                            await this._documentoAmministrativoRepository.Add(aggregateAttachment);

                            await AddInfoFile(aggregateAttachment.Id.AsLong(), aggregateAttachment.Versions.First().Id.AsLong(), aggregateDocument.Id.AsLong(), filename, creationDate, fileValidationResult);
                        }
                        catch (Pi3Exception ex)
                        {
                            _logger.LogError("Errore in Caricamento documento allegato:" + ex.Message);
                            await this.InsertLog(docnumber.ToString(), string.Format(ex.Message, docnumber.ToString()), (long)ErrorMessageEnum.Error);
                            throw new FileUploadException(ex.Message, docnumber.ToString());
                        }
                        catch (Exception ex)
                        {
                            _logger.LogCritical(ex, ex.Message);
                            if (!string.IsNullOrWhiteSpace(idAttachment))
                            {
                                await this.InsertLog(docnumber.ToString(), string.Format(ErrorDescriptions.AttachmentUploadError, attachmentName), (long)ErrorMessageEnum.Error);
                                throw new FileUploadException(ErrorDescriptions.AttachmentUploadError, attachmentName);
                            }
                            else 
                            {
                                await this.InsertLog(docnumber.ToString(), ErrorDescriptions.AttachmentCreationError, (long)ErrorMessageEnum.Error);
                                throw new DocumentCreationException(ErrorDescriptions.AttachmentCreationError);
                            }
                        }
                        c++;
                    }
                    #endregion

                    #region Trasmissione documento
                    var unreachableReceivers = new List<ReceiverInfo>();
                    var trasmReceivers = new List<InteropCorrespondent>();

                    // Estrazione ragione
                    var reason = await this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                        .Where(r => r.ID_AMM == userInfo.IdAmm.AsLong() && r.CHA_TIPO_RAGIONE == "S")
                        .FirstOrDefaultAsync();

                    if (reason == null)
                    {
                        throw new TransmissionException(ErrorDescriptions.ReasonNotFound);
                    }

                    // Creazione oggetto trasmissione
                    var aggregateTrasm = new Trasmissione(
                        userInfo.IdAmm,
                        DateTime.Now,
                        aggregateDocument.Id,
                        TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
                        new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Autore
                        {
                            IdUtente = userInfo.IdUser,
                            IdGruppo = userInfo.IdGroup
                        },
                        new TextValue(receiverList.Select(r => r.Code).Aggregate((r1, r2) => String.Format("{0} {1}", r1, r2)))
                        );

                    // Estrazione corrispondenti che riceveranno la trasmissione
                    foreach (var receiver in receiverList)
                    {

                        var tempReceivers = new List<InteropCorrespondent>();
                        // Costruzione corrispondente
                        var corrCode = receiver.Code;
                        if (receiver.Code.Length - (receiver.AdministrationCode.Length + 1) > 0)
                            corrCode = receiver.Code.Substring(receiver.AdministrationCode.Length + 1);

                        var rf = await this._dbContext.RegistroEntities.AsNoTracking()
                            .Where(r => r.VAR_CODICE.ToUpper() == corrCode.ToUpper())
                            .FirstOrDefaultAsync();

                        if (rf != null)
                        {
                            tempReceivers = await this.GetCorrList(rf.SYSTEM_ID, request.InteroperabilityMessage.IsPrivate);
                        }
                        else
                        {
                            var idAmm = await _dbContext.AmministraEntities.AsNoTracking()
                                .Where(a => a.VAR_CODICE_AMM.ToUpper() == administrationCode.ToUpper())
                                .Select(a => a.SYSTEM_ID)
                                .FirstOrDefaultAsync();

                            var corrType = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                .Where(c => c.DTA_FINE == null && c.ID_AMM == idAmm && c.VAR_COD_RUBRICA.ToUpper() == corrCode.ToUpper())
                                .FirstOrDefaultAsync().Result?.CHA_TIPO_URP;

                            if(corrType == "U") { tempReceivers = await this.GetCorrList(interopSettings.RegisterId.Value, request.InteroperabilityMessage.IsPrivate); }
                        }

                        trasmReceivers.AddRange(tempReceivers);

                        if (tempReceivers == null || tempReceivers.Count == 0)
                        {
                            unreachableReceivers.Add(receiver);
                            continue;
                        }
                    }

                    if(!trasmReceivers.Any())
                    {
                        // Non è stato trovato NESSUN destinatario
                        await this.InsertLog(docnumber.ToString(), ErrorDescriptions.TransmissionRecipientsNotFound, (long)ErrorMessageEnum.Error);

                        aggregateDocument.Recycle(new TextValue(ErrorDescriptions.TransmissionRecipientsNotFound));
                        await this._documentoAmministrativoRepository.Update(aggregateDocument);

                        throw new TransmissionException(ErrorDescriptions.TransmissionRecipientsNotFound, receiverList);
                    }

                    foreach (var trasmReceiver in trasmReceivers)
                    {
                        var utentiNotificati = await this.GetUsersToNotify(trasmReceiver.Code, userInfo.IdAmm);
                        if (utentiNotificati.Any())
                        {
                            // Costruzione trasmissioni singole
                            var trasmData = new DatiTrasmissioneSingolaGruppo()
                            {
                                CodiceGruppoDestinatario = trasmReceiver.Code,
                                DescrizioneGruppoDestinatario = new TextValue(trasmReceiver.Description),
                                IdGruppoDestinatario = trasmReceiver.IdGroup,
                                IdRagioneTrasmissione = reason.SYSTEM_ID.ToString(),
                                NomeRagioneTrasmissione = reason.VAR_DESC_RAGIONE,
                                RagioneConWorkflow = reason.CHA_TIPO_RAGIONE!.ToUpper() == "W",
                                Tipo = TipiTrasmissioneSingolaEnum.Uno,
                                UtentiNotificati = utentiNotificati
                            };
                            aggregateTrasm.PrepareTrasmissioneSingolaGruppo(trasmData); 
                        }
                    }

                    try
                    {
                        await _trasmissioneRepository.Add(aggregateTrasm);

                        aggregateTrasm.Invia(DateTime.Now);
                        await _trasmissioneRepository.Update(aggregateTrasm);

                        foreach (var ts in aggregateTrasm.TrasmissioniSingole)
                        {
                            await this._webMethodLoggerService.LogOK("TRASM_DOC_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_"),
                                aggregateTrasm.OggettoTrasmesso.Id,
                                string.Format("Trasmesso Documento: {0}", aggregateTrasm.OggettoTrasmesso.Id), ts.Id);
                        }

                        await InviaNotificaEmail(aggregateTrasm.Id.AsLong());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, ex.Message);
                        await this.InsertLog(docnumber.ToString(), ErrorDescriptions.TransmissionGenericError, (long)ErrorMessageEnum.Error);

                        // Se la trasmissione fallisce devo cestinare il documento
                        aggregateDocument.Recycle(new TextValue("Errore trasmissione"));
                        await this._documentoAmministrativoRepository.Update(aggregateDocument);
                        
                        throw new TransmissionException(ErrorDescriptions.TransmissionGenericError, receiverList);
                    }
                    #endregion

                    #region Verifiche finali: gestione privati e protocollazione automatica
                    // Se il documento è stato marcato privato a causa delle impostazioni sulla gestione
                    // viene eliminato il flag privato
                    var createdDocAggregate = await this._documentoAmministrativoRepository.Get(userInfo.IdAmm, aggregateDocument.Id);
                    if(!request.InteroperabilityMessage.IsPrivate && createdDocAggregate.Reserved)
                    {
                        var x = await this._dbContext.ProfileEntities.Where(p => p.DOCNUMBER == docnumber).FirstAsync();
                        x.CHA_PRIVATO = "0";
                        await ((DbContext)this._dbContext).SaveChangesAsync();
                    }

                    // Protocollazione se attiva modalità automatica
                    if (interopSettings.ManagementMode == ManagementModeEnum.A.ToString())
                    {
                        try
                        {
                            createdDocAggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione
                            {
                                DatiRegistro = new DatiRegistro()
                                {
                                    IdRegistro = interopSettings.RegisterId.ToString()
                                }
                            });

                            await _documentoAmministrativoRepository.Update(createdDocAggregate);

                            var datiRegistrazioneProtocollo = (DatiRegistrazioneProtocollo)createdDocAggregate.DatiRegistrazione;

                            // Invio ricevuta di conferma di ricezione al mittente per tutti i destinatari
                            await this.SendProofToSender(request.InteroperabilityMessage, new RecordInfo
                            {
                                AdministrationCode = administrationCode,
                                AOOCode = datiRegistrazioneProtocollo.CodiceRegistro,
                                RecordDate = datiRegistrazioneProtocollo.DataProtocollazione.Value,
                                RecordNumber = datiRegistrazioneProtocollo.NumeroProtocollo.ToString(),
                                Subject = createdDocAggregate.OggettoDelDocumento.Descrizione.Value
                            },
                            docnumber.ToString(),
                            instance!,
                            administrationCode);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogCritical(ex, ex.Message);

                            await this.InsertLog(docnumber.ToString(), ErrorDescriptions.RegistrationError, (long)ErrorMessageEnum.Error);
                            throw new RegistrationException(AOOCode, ErrorDescriptions.RegistrationError);
                        }
                    }
                    #endregion

                    // Log esito positivo
                    await this.InsertLog(docnumber.ToString(), string.Format("Documento con id {0} trasmesso correttamente ai destinatari", docnumber), (long)ErrorMessageEnum.None);

                    #region Costruzione response
                    var newDocument = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Join(this._dbContext.ComponentEntities.AsNoTracking(), p => p.DOCNUMBER, c => c.DOCNUMBER, (p, c) => new { p, c })
                        .Where(x => x.p.DOCNUMBER == docnumber)
                        .Select(x => new
                        {
                            x.p.VAR_PROF_OGGETTO,
                            x.c.VAR_NOMEORIGINALE,
                            x.c.VAR_IMPRONTA
                        })
                        .FirstAsync();

                    result.DocumentDelivered = new InfoDocumentDelivered
                    {
                        MainDocument = new DocumentInfo
                        {
                            DocumentNumber = aggregateDocument.Id,
                            Name = newDocument.VAR_PROF_OGGETTO,
                            FileName = newDocument.VAR_NOMEORIGINALE,
                            Fingerprint = newDocument.VAR_IMPRONTA
                        }
                    };

                    // Allegati
                    var newAttachments = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Join(this._dbContext.ComponentEntities.AsNoTracking(), p=> p.DOCNUMBER, c => c.DOCNUMBER, (p,c) => new {p,c })
                        .Where(a => a.p.ID_DOCUMENTO_PRINCIPALE == docnumber && !this._dbContext.NotificaEntities.Any(
                            n => n.DOCNUMBER == docnumber && (a.p.VAR_PROF_OGGETTO!.StartsWith("Ricevuta di ritorno delle Mail") || a.p.VAR_PROF_OGGETTO.StartsWith("Ricevuta di mancata consegna") || a.p.VAR_PROF_OGGETTO.StartsWith("Ricevuta di avvenuta"))
                            && !this._dbContext.VersionEntities.Any(v => v.DOCNUMBER == docnumber && (v.CHA_ALLEGATI_ESTERNO == "1" || v.CHA_ALLEGATI_ESTERNO == "0"))))
                        .Select(x => new 
                        {
                            x.p.DOCNUMBER,
                            x.p.VAR_PROF_OGGETTO,
                            x.c.VAR_NOMEORIGINALE,
                            x.c.VAR_IMPRONTA
                        }).ToListAsync();

                    result.DocumentDelivered.Attachments = newAttachments.Select(x => new DocumentInfo
                    {
                        DocumentNumber = x.DOCNUMBER.ToString()!,
                        Name = x.VAR_PROF_OGGETTO!,
                        FileName = x.VAR_NOMEORIGINALE!,
                        Fingerprint = x.VAR_IMPRONTA
                    }).ToList();
                    #endregion

                    #region Gestione eventuali destinatari non raggiunti
                    if (unreachableReceivers?.Count > 0)
                    {
                        // Alcuni destinatari non sono stati trovati
                        // Il codice PITRE rilancia un'eccezione in questo caso
                        throw new TransmissionException(ErrorDescriptions.TransmissionRecipientsNotFound, unreachableReceivers);
                    }
                    #endregion
                }
                catch (Pi3Exception ex)
                {
                    result.SingleRequestErrors.Add(new ElaborateInteroperabilitySingleMessage
                    {
                        ErrorMessage = ex.Message,
                        Receivers = ex.GetType() == typeof(TransmissionException) ? ((TransmissionException)ex).ReceiverInfos : receiverList
                    });
                }
                catch (Exception ex)
                {
                    this._logger.LogError($"Unhandled Exception: {ex.Message}");

                    result.SingleRequestErrors.Add(new ElaborateInteroperabilitySingleMessage
                    {
                        ErrorMessage = ErrorDescriptions.UnhandledError,
                        Receivers = receiverList
                    });
                }
            }

            return new ElaborateNewInteroperabilityMessageResponse
            {
                Result = result
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ElaborateNewInteroperabilityMessageHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly INotaRepository _notaRepository;
        protected readonly IUOCorrispondenteRepository _uoCorrispondenteRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IAuthToken _tokenService;
        protected readonly ICorrispondenti _corrispondentiService;
        protected readonly IInteroperabilityService _interoperabilityService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IConfigurationService _configurationService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IPAdESService _pAdESService;
        protected readonly IFileValidatorService _fileValidatorService;


        protected async Task Impersonate(string administrationCode, string aooCode, string instance)
        {
            var userContext = await
                (
                from a in _dbContext.AmministraEntities.AsNoTracking()
                join r in _dbContext.RegistroEntities.AsNoTracking() on a.SYSTEM_ID equals r.ID_AMM
                join i in _dbContext.InteroperabilitySettingEntities.AsNoTracking() on r.SYSTEM_ID equals i.REGISTRYID
                join p in _dbContext.PeopleEntities.AsNoTracking() on i.USERID equals p.SYSTEM_ID
                join g in _dbContext.GroupEntities.AsNoTracking() on i.ROLEID equals g.SYSTEM_ID
                join cgp in _dbContext.CorrGlobaliEntities.AsNoTracking() on p.SYSTEM_ID equals cgp.ID_PEOPLE
                join cgg in _dbContext.CorrGlobaliEntities.AsNoTracking() on g.SYSTEM_ID equals cgg.ID_GRUPPO
                where a.VAR_CODICE_AMM == administrationCode
                && r.VAR_CODICE == aooCode
                select new
                {
                    PEOPLE_SYSTEM_ID = i.USERID,
                    PEOPLE_USER_ID = p.USER_ID,
                    PEOPLE_VAR_COGNOME = p.VAR_COGNOME,
                    PEOPLE_VAR_NOME = p.VAR_NOME,
                    PEOPLE_CHA_AMMINISTRATORE = p.CHA_AMMINISTRATORE,
                    AMM_SYSTEM_ID = a.SYSTEM_ID,
                    AMM_VAR_CODICE_AMM = a.VAR_CODICE_AMM,
                    AMM_VAR_DESC_AMM = a.VAR_DESC_AMM,
                    CORRGLOBALI_SYSTEM_ID = cgp.SYSTEM_ID,
                    GROUPS_SYSTEM_ID = g.SYSTEM_ID,
                    GROUPS_GROUP_ID = g.GROUP_ID,
                    GROUPS_GROUP_NAME = g.GROUP_NAME,
                    CORRGLOBALI_GROUPS_SYSTEM_ID = cgg.SYSTEM_ID,
                }
                ).FirstOrDefaultAsync();

            this._logger.LogInformation($"Impersonating user {userContext.PEOPLE_USER_ID} [AOOCode: {aooCode}, AdminCode: {administrationCode}]");

            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdUser, userContext.PEOPLE_SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserId, userContext.PEOPLE_USER_ID);
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserName, userContext.PEOPLE_VAR_NOME);
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.UserSurname, userContext.PEOPLE_VAR_COGNOME);
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Admin, (userContext.PEOPLE_CHA_AMMINISTRATORE == "1").ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdGroup, userContext.GROUPS_SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupCode, userContext.GROUPS_GROUP_ID.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.GroupDescription, userContext.GROUPS_GROUP_NAME.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Instance, instance);
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.IdTenant, userContext.AMM_SYSTEM_ID.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.TenantCode, userContext.AMM_VAR_CODICE_AMM.ToString());
            this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.TenantDescription, userContext.AMM_VAR_DESC_AMM.ToString());
        }

        protected async Task<string> GetSeparator(string administrationCode)
        {
            var query = await this._dbContext.AmministraEntities.Where(a => a.VAR_CODICE_AMM == administrationCode)
                .Select(x => new { x.CHA_STR_SEGNATURA })
                .FirstOrDefaultAsync();

            return query?.CHA_STR_SEGNATURA ?? "/";
        }

        protected async Task<List<InteropCorrespondent>> GetCorrList(long registerId, bool isPrivate)
        {
            var rolesQueryable = this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), r => r.ID_RUOLO_IN_UO, c => c.SYSTEM_ID, (r, c) => new { r, c })
                                .Join(this._dbContext.TipoFRuoloEntities.AsNoTracking(), rc => rc.r.ID_RUOLO_IN_UO, t => t.ID_RUOLO_IN_UO, (rc, t) => new { rc, t })
                                .Join(this._dbContext.FunzioneEntities.AsNoTracking(), j => j.t.ID_TIPO_FUNZ, f => f.ID_TIPO_FUNZIONE, (j, f) => new { j, f.COD_FUNZIONE })
                                .Where(x => x.j.rc.r.ID_REGISTRO == registerId && x.j.rc.c.DTA_FINE == null);

            var interopRoles = isPrivate ?
                rolesQueryable.Where(x => x.COD_FUNZIONE == InteropFunctions.CanReceivePrivateDocuments) :
                rolesQueryable.Where(x => x.COD_FUNZIONE == InteropFunctions.CanReceiveNonPrivateDocuments);

            return await interopRoles.Select(x => new InteropCorrespondent
            {
                Code = x.j.rc.c.VAR_COD_RUBRICA,
                Description = x.j.rc.c.VAR_DESC_CORR,
                IdGroup = x.j.rc.c.ID_GRUPPO.ToString()
            }).Distinct()
            .ToListAsync() ?? new List<InteropCorrespondent>();

        }

        protected async Task<List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>> GetUsersToNotify(string codeRole, string idAmm)
        {
            var roleItem = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(x => x.VAR_COD_RUBRICA!.ToUpper() == codeRole.ToUpper() && x.CHA_TIPO_IE == "I" && x.CHA_TIPO_CORR == "S" && !x.DTA_FINE.HasValue && (x.ID_AMM == null || x.ID_AMM == idAmm.AsLong()))
                .FirstOrDefaultAsync();

            if(roleItem == null) { return new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>(); }

            var users = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(), cu => cu.ID_PEOPLE, pg => pg.PEOPLE_SYSTEM_ID, (cu, pg) => new { cu, pg })
                .Join(this._dbContext.PeopleEntities.AsNoTracking(), i => i.cu.ID_PEOPLE, p => p.SYSTEM_ID, (i, p) => new { i, p })
                .Where(x => x.i.pg.GROUPS_SYSTEM_ID == roleItem.ID_GRUPPO && x.i.cu.CHA_TIPO_IE == "I" && x.i.cu.CHA_TIPO_CORR == "S" && !x.i.pg.DTA_FINE.HasValue && !x.i.cu.DTA_FINE.HasValue)
                .Select(x => new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                {
                    IdUtente = x.i.cu.ID_PEOPLE.ToString(),
                    UserId = x.i.cu.VAR_COD_RUBRICA,
                    Cognome = x.p.VAR_COGNOME,
                    Nome = x.p.VAR_NOME
                }).ToListAsync();

            return users;

        }

        protected async Task InsertLog(string docnumber, string message, long error)
        {
            var insert = await ((DbContext)this._dbContext).Database.ExecuteSqlInterpolatedAsync($"INSERT INTO SimpInteropDbLog (PROFILEID, ERRORMESSAGE, TEXT) VALUES ({docnumber}, {error}, {message})");
        }

        //protected async Task<string> GetToken()
        //{
        //    var authParams = this._authenticationOptions.Value;

        //    // Richiesta token
        //    var tokenResponse = await this._tokenService.GetAuthTokenAsync(new TokenRequest
        //    {
        //        grant_type = this._authenticationOptions.Value.GrantType,
        //        client_id = this._authenticationOptions.Value.ClientID,
        //        client_secret = this._authenticationOptions.Value.ClientSecret,
        //        scope = this._authenticationOptions.Value.Scope
        //    });

        //    return tokenResponse.access_token;
        //}

        protected async Task<string?> SearchSender(string code, string idTenant)
        {
            var token = this._tokenService.GetAuthenticationToken();

            var response = await this._corrispondentiService.Search(token, new SearchRequest
            {
                CriteriRicerca = new List<CriterioRicerca>() { new CriterioRicerca
                {
                    Campo = CampiRicercaEnum.Codice,
                    Valore = code,
                    TipoRicercaParola = TipiRicercaParolaEnum.ParolaIntera
                } },
                CriteriOrdinamento = new List<CriterioOrdinamento> { new CriterioOrdinamento
                {
                    Campo = CampiRicercaEnum.Codice,
                    Tipo = TipiOrdinamentoEnum.Asc
                } },
                ElementiPerPagina = 1,
                Pagina = 1
            });

            if(!response.Corrispondenti.Any())
            {
                return null;
            }

            var corrRubricaComune = response.Corrispondenti.First();

            // Ricerco il corrispondente in rubrica.
            // Se non è presente lo censisco, altrimenti lo aggiorno
            var corrQueryable = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(x => x.VAR_COD_RUBRICA.ToUpper() == code.ToUpper() && x.CHA_TIPO_CORR == "C")
                .AsQueryable();

            string idCorrespondent = string.Empty;

            if(!corrQueryable.Any())
            {
                var aggregate = new UOCorrispondente(idTenant, DateTime.Now, new TextValue(corrRubricaComune.Codice), new TextValue(corrRubricaComune.Denominazione));

                await this._uoCorrispondenteRepository.Add(aggregate);

                aggregate.SetRubricaComune(true);
                aggregate.SetCodiceAmministrazione(corrRubricaComune.Amministrazione);
                aggregate.SetCodiceAOO(corrRubricaComune.AOO);
                aggregate.ChangeIndirizzo(corrRubricaComune.ToIndirizzoCorrispondente());
                aggregate.SetInteropUrl(corrRubricaComune.UrlApiInteroperabilita);
                /* Eliminati inserimenti di codice fiscale e partita IVA perché a volta arriva la p. IVA nel codice fiscale e i controlli non vanno a buon fine. Verrà fatto l'update sulla dpa_dett_globali in seguito
                aggregate.SetCodiceFiscale(corrRubricaComune.CodiceFiscale ?? string.Empty);
                aggregate.SetPartitaIva(corrRubricaComune.PartitaIva ?? string.Empty);
                */

                await this._uoCorrispondenteRepository.Update(aggregate);

                idCorrespondent = aggregate.Id;    
                
                if(!string.IsNullOrEmpty(corrRubricaComune.CodiceFiscale) || !string.IsNullOrEmpty(corrRubricaComune.PartitaIva))
                {
                    var dettGlobaliEntity = await _dbContext.DettGlobaliEntities
                        .Where(d => d.ID_CORR_GLOBALI == idCorrespondent.AsLong())
                        .FirstAsync();

                    if(dettGlobaliEntity != null)
                    {
                        dettGlobaliEntity.VAR_COD_FISC = corrRubricaComune.CodiceFiscale ?? string.Empty;
                        dettGlobaliEntity.VAR_COD_PI = corrRubricaComune.PartitaIva ?? string.Empty;
                    }

                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }

                if(corrRubricaComune.Tipo == TipiEnum.RaggruppamentoFunzionale)
                {
                    var newCorrispondenteEntity = await _dbContext.CorrGlobaliEntities
                        .Where(c => c.SYSTEM_ID == idCorrespondent.AsLong())
                        .FirstAsync();

                    newCorrispondenteEntity.CHA_TIPO_URP = "F";

                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
            }
            else
            {
                var corrElement = await corrQueryable.FirstAsync();

                idCorrespondent = corrElement.SYSTEM_ID.ToString();

                /***** Non è detto che è un corrispondente di tipo UO, nella maggior parte dei casi è di tipo F *****
                if (corrElement.CHA_TIPO_URP == "U")
                {
                    
                    var aggregate = await this._uoCorrispondenteRepository.Get(idTenant, corrElement.SYSTEM_ID.ToString());

                    var changed = false;

                    if (!string.Equals(aggregate.CodiceAmministrazione, corrRubricaComune.Amministrazione))
                    {
                        changed = true;
                        aggregate.SetCodiceAmministrazione(corrRubricaComune.Amministrazione);
                    }
                    if (!string.Equals(aggregate.CodiceAOO, corrRubricaComune.AOO))
                    {
                        changed = true;
                        aggregate.SetCodiceAOO(corrRubricaComune.AOO);
                    }
                    if (!string.Equals(aggregate.InteropUrl, corrRubricaComune.UrlApiInteroperabilita))
                    {
                        changed = true;
                        aggregate.SetInteropUrl(corrRubricaComune.UrlApiInteroperabilita);
                    }
                    if (!string.Equals(aggregate.CodiceFiscale, corrRubricaComune.CodiceFiscale))
                    {
                        changed = true;
                        aggregate.SetCodiceFiscale(corrRubricaComune.CodiceFiscale);
                    }
                    if (!string.Equals(aggregate.PartitaIva, corrRubricaComune.PartitaIva))
                    {
                        changed = true;
                        aggregate.SetPartitaIva(corrRubricaComune.PartitaIva);
                    }

                    if (!aggregate.Indirizzo.Equals(corrRubricaComune.ToIndirizzoCorrispondente()))
                    {
                        changed = true;
                        aggregate.ChangeIndirizzo(corrRubricaComune.ToIndirizzoCorrispondente());
                    }

                    if (changed)
                        await this._uoCorrispondenteRepository.Update(aggregate);

                    idCorrespondent = aggregate.Id;
                }
                else
                {
                    idCorrespondent = corrElement.SYSTEM_ID.ToString();
                }
                */
            }

            return idCorrespondent;
        }

        protected async Task SendProofToSender(InteroperabilityMessage interoperabilityMessage, RecordInfo receiverRecordInfo, string idProfile, string instance, string tenant)
        {
            var token = this._tokenService.GetAuthenticationToken();

            var senderInfo = new RecordInfo
            {
                AdministrationCode = interoperabilityMessage.Record.AdministrationCode,
                AOOCode = interoperabilityMessage.Record.AOOCode,
                RecordDate = interoperabilityMessage.Record.RecordDate,
                RecordNumber = interoperabilityMessage.Record.RecordNumber,
                Subject = interoperabilityMessage.Record.Subject
            };

            var key = await this._configurationService.GetValue<string>("INTEROP_SERVICE_URL");

            interoperabilityMessage.Receivers.ForEach(async r =>
            await this._interoperabilityService.AnalyzeDocumentReceivedProof(
                instance,
                token,
                tenant,
                new AnalyzeDocumentReceivedProof.AnalyzeDocumentReceivedProofRequest
                {
                    SenderRecordInfo = senderInfo,
                    ReceiverRecordInfo = receiverRecordInfo,
                    ReceiverUrl = key ?? string.Empty,
                    ReceiverCode = string.Format("'{0}'", r.Code)

                }));
        }

        protected async Task InviaNotificaEmail(long idTrasmissione)
        {
            try
            {
                var trasmSingoleEntities = await this._dbContext.TrasmSingolaEntities.AsNoTracking()
                       .Where(x => x.ID_TRASMISSIONE == idTrasmissione)
                       .Select(x => new
                       {
                           x.SYSTEM_ID,
                           x.ID_RAGIONE
                       })
                       .ToListAsync();

                foreach (var s in trasmSingoleEntities)
                {
                    var ragioneEntity = await this._dbContext.RagioneTrasmissioneEntities.FirstAsync(r => r.SYSTEM_ID == s.ID_RAGIONE);

                    if (!(ragioneEntity?.VAR_NOTIFICA_TRASM == "NN"))
                    {
                        var trasmUtenteEntities = await this._dbContext.TrasmUtenteEntities.AsNoTracking()
                        .Where(x => x.ID_TRASM_SINGOLA == s.SYSTEM_ID)
                        .ToListAsync();

                        foreach (var u in trasmUtenteEntities)
                        {
                            var tipoNotifica = ragioneEntity!.VAR_NOTIFICA_TRASM;

                            await this._mediator.Send(
                                new MessageQueueCommandWrapper(
                                    new InviaEmailTrasmissioneRequest(this._claimsPrincipalService.Current)
                                    {
                                        IdTrasmissioneUtente = u.SYSTEM_ID,
                                        TipoNotifica = tipoNotifica
                                    }));
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }
        }

        public async Task AddInfoFile(long docnumber, long versionId, long? idDocumentoPrincipale, string fileName, DateTime? dataAcquisizione, FileValidationResult? fileValidateResult)
        {
            try
            {
                var infoFileEntity = await this._dbContext.InfoFileEntities
                               .Where(i => i.ID_PROFILE == docnumber)
                               .FirstOrDefaultAsync();

                if (infoFileEntity == null)
                {
                    infoFileEntity = new InfoFileEntity()
                    {
                        ID_PROFILE = docnumber,
                        ID_DOCUMENTO_PRINCIPALE = idDocumentoPrincipale,
                        VERSION_ID = versionId
                    };

                    this._dbContext.InfoFileEntities.Add(infoFileEntity);
                }

                infoFileEntity.VERSION_ID = versionId;
                infoFileEntity.DTA_ACQUISIZIONE = null;
                infoFileEntity.VAR_ESTENSIONE = string.Empty;
                infoFileEntity.VAR_NOME_FILE = string.Empty;
                infoFileEntity.VAR_DESC_INFO_FILE = string.Empty;
                infoFileEntity.CHA_CONFORME = "1";
                infoFileEntity.CHA_ESTENSIONE_CONFORME = "1";
                infoFileEntity.CHA_PRESENZA_MACRO = "0";
                infoFileEntity.CHA_PRESENZA_FORMS = "0";
                infoFileEntity.CHA_PRESENZA_JAVASCRIPT = "0";

                if (!string.IsNullOrEmpty(fileName))
                {
                    infoFileEntity.DTA_ACQUISIZIONE = dataAcquisizione.HasValue ? dataAcquisizione : null;
                    infoFileEntity.VAR_ESTENSIONE = Path.GetExtension(fileName).Replace(".", string.Empty);
                    infoFileEntity.VAR_NOME_FILE = fileName;
                }

                if (fileValidateResult != null)
                {
                    infoFileEntity.CHA_CONFORME = fileValidateResult.Compliance!.IsCompliantToFormat
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
                }

                await ((Pi3DbContext)this._dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

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
        #endregion
    }
}
