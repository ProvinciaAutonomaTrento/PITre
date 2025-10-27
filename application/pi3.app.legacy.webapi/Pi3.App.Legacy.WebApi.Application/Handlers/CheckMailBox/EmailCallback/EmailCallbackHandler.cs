// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Annullamento;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Conferma;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Daticert;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Eccezione;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed;
using static Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.CheckMailboxHandler;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.EmailCallback
{

    public class EmailCallbackHandler : IRequestHandler<EmailCallbackRequest, EmailCallbackResponse>
    {
        #region Public Members

        public EmailCallbackHandler(
            ILogger<EmailCallbackHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IConfigurationService configurationService,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _configurationService = configurationService;
            _dbContext = dbContext;
        }

        public async Task<EmailCallbackResponse> Handle(EmailCallbackRequest request, CancellationToken cancellationToken)
        {
            // Memo
            // gestioneRicevutePec è sempre TRUE
            // SALVA_RICEVUTE_PEC è sempre TRUE

            // Dal momento che l'inserimento in non_elaborate lo fa il package Pi3.Infrastructure.Chilkat
            // avrebbe senso inserire tutto in uno o più blocchi try/catch e gestire da lì tutte le eccezioni

            _logger.LogInformation($"request.IdCheckMailbox > {request.IdCheckMailbox}");

            Email email = request.Email;
            int item = request.Item;
            int total = request.Total;

            // 1 - Aggiornamento numero totale di messaggi da processare
            var checkMailBoxEntity = await _dbContext.CheckMailboxEntities
                 .FirstOrDefaultAsync(x => x.ID == request.IdCheckMailbox);

            if(checkMailBoxEntity == null)
                throw new EmailCallbackPi3Exception("CheckMailboxEntity null");

            _logger.LogInformation($"checkMailBoxEntity.ID > {checkMailBoxEntity.ID}");

            if (checkMailBoxEntity.TOTAL == 0)
                checkMailBoxEntity.TOTAL = (short)total;

            var useSegnaturaAllegato6 = await _configurationService.GetValue<string>("BE_SEGNATURA_PROTO_ALLEGATO6");

            var message = new AnalyzedMessage();

            var isPec = email.Headers.Where(x => x.Name == "X-Trasporto").FirstOrDefault()?.Value == "posta-certificata";

            var messageProcessedType = isPec ? MailProcessedType.Pec : MailProcessedType.NonPEC;

            if (IsPECDelivered(email) || IsFromNonPEC(email))
            {
                email = email.AttachmentsAsEmails[0];
            }

            var tipoRicevuta = GetTipoRicevutaPec(email);

            bool fattElDaPec = false;

            try
            {
                // Gestione DSN
                long? docNumberFromDSN = default;

                if (email.Subject.Value.Contains('#'))
                {
                    var subjectItems = email.Subject.Value.Split('#');

                    // Gestione DSN
                    if (tipoRicevuta == MailPecXRicevuta.Delivery_Status_Notification && subjectItems.Length > 1)
                        docNumberFromDSN = subjectItems[1].Replace("#", string.Empty).AsLong();
                    message.Subject = subjectItems[0];
                    // Gestione oggetto
                    // TO DO - la classe email ha le proprietà non modificabili
                }
                else
                {
                    message.Subject = email.Subject.Value;
                }
                if (tipoRicevuta == MailPecXRicevuta.Delivery_Status_Notification && email.Subject.Value.Split('#').Length > 1)
                {
                    message.HasDeliveryStatusNotification = true;
                    docNumberFromDSN = email.Subject.Value.Split('#')[1].Replace("#", string.Empty).AsLong();
                }

                // Gestione accettazione/consegna eccezioni

                message.Id = email.Headers.Where(x => x.Name.ToLower() == "message-id").FirstOrDefault()?.Value ?? string.Empty;
                _logger.LogInformation($"checkMailBoxEntity.ID > {checkMailBoxEntity.ID} messageId -> { message.Id }");

                if (!isPec && request.ProcessOnlyPec)
                {
                    // Messaggio non PEC
                    // Se la casella è configurata con IMAP devo spostare in mail non elaborate
                    _logger.LogDebug($"Elaborazione mail non PEC con message-id={message.Id}");

                    if (request.EmailBoxType == EmailBoxTypeEnum.IMAP)
                        return new EmailCallbackResponse(false);

                    // CheckID
                }

                if (IsMessageAlreadyProcessed(message.Id, request.IdRegister))
                {
                    message.AlreadyProcessed = true;
                    // riga commentata aggiornamento total non funzionante
                    //checkMailBoxEntity.TOTAL -= 1;
                    return new EmailCallbackResponse(false);
                }

                // TO DO
                // Salvataggio in locale
                // nella query nuova ho aggiunto un and con la var_mail_registro perché un registro può avere associate più mail, ma il vecchio backend si aspetta una riga sola
                /*
                 *  SELECT cha_salva_mail_loc
                    FROM dpa_mail_registri
                    WHERE id_registro=@idReg@
                 */

                string salvataggioMail = await _configurationService.GetValue<string>("BE_SALVA_EMAIL_IN_LOCALE");
                var salvaMailRF = await _dbContext.MailRegistriEntities
                    .Where(x => x.ID_REGISTRO == request.Registro.systemId.AsLong() && x.VAR_EMAIL_REGISTRO.Equals(request.EmailAddress))
                    .Select(x => x.CHA_SALVA_MAIL_LOC)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(salvaMailRF) && (salvaMailRF.Equals("0") || salvaMailRF.Equals("1")))
                    salvataggioMail = salvaMailRF;

                email.Attachments.ForEach(x => AnalyzeMailAttachment(message, x));

                // Se non trovo notifiche gestite EmailAttachmentType è NULL
                // Devo quindi gestire i casi in cui sia una mail standard (PEC o no)

                ProcessorOutput? output = default;

                if (message.HasErrors && !message.HasFatalError && !(message.EmailAttachmentType == MailProcessedType.DatiCert))
                {
                    // Esegui senza segnatura
                    // Invio notifica eccezione
                }

                if(message.EmailAttachmentType != null)
                    messageProcessedType = message.EmailAttachmentType.Value; 

                switch (message.EmailAttachmentType)
                {
                    case MailProcessedType.DatiCert:
                        if (IsRicevutaPec(tipoRicevuta))
                        {
                            output = (await _mediator.Send(new DatiCertRequest(
                                email,
                                request.IdRegister.ToString(),
                                message
                                ))).output;
                        }
                        else
                        {
                            if (isPec)
                                messageProcessedType = MailProcessedType.Pec;

                            output = (await _mediator.Send(new StandardCaseRequest(
                            message.Id,
                            email,
                            request.Registro,
                            request.EmailAddress,
                            false,
                            isPec,
                            salvataggioMail
                            ))).output;
                        }
                        break;
                    case MailProcessedType.Signature:
                        output = (await _mediator.Send(new SegnaturaRequest(message, email, request.Registro, request.EmailAddress, isPec, fattElDaPec, salvataggioMail
                            ))).output;
                        break;
                        break;
                    case MailProcessedType.NotifyCancellation:
                        output = (await _mediator.Send(new AnnullamentoRequest(message.AttachmentContent, message.Id
                            ))).output;
                        break;
                    case MailProcessedType.Eccezione:
                        output = (await _mediator.Send(new EccezioneRequest(
                            email,
                            message,
                            request.Registro
                            ))).output;
                        break;
                    case MailProcessedType.ConfirmReception:
                        output = (await _mediator.Send(new ConfermaRequest(email, message, request.Registro, message.Id, request.EmailAddress
                            ))).output;
                        break;
                    case MailProcessedType.Pec:
                        break;
                    case MailProcessedType.NonPEC:
                        break;
                    default:
                        output = (await _mediator.Send(new StandardCaseRequest(
                            message.Id,
                            email,
                            request.Registro,
                            request.EmailAddress,
                            false,
                            isPec,
                            salvataggioMail
                            ))).output;
                        break;
                }

                if (!string.IsNullOrEmpty(output.ErrorMessage))
                    message.ErrorMessages = new List<string>() { output.ErrorMessage };

                message.Processed = output?.Success ?? false;
                message.ProcessedAttachments = output?.ProcessedAttachments ?? 0;
                message.DocNumber = output?.DocNumber ?? 0;
            }
            catch (Pi3Exception pi3Ex)
            {
                _logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(exception: ex, message: ex.Message);
            }
            finally
            {
                if (message.Processed)
                {
                    checkMailBoxEntity.ELABORATE = (short)item;

                    //Aggiornamento DPA_MAIL_ELABORATE
                    var mailElaborataEntity = new MailElaborataEntity()
                    {
                        VAR_MESSAGE = message.Id,
                        CHA_RAGIONE_ELAB = "E",
                        DTA_ELAB = await _dbContext.GetSystemDateTime(), //DateTime.Now,
                        ID_REGISTRO = request.IdRegister,
                        ID_PROFILE = message.DocNumber ?? null,
                        VAR_EMAIL = email.Sender.Address
                    };

                    await _dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                //Aggiornamento contatore mail processate
                if (!message.AlreadyProcessed)
                {
                    checkMailBoxEntity.ELABORATE = (short)item;

                    _logger.LogDebug($"email.DeliveryDate = {email.DeliveryDate}");
                    _logger.LogDebug($"message.ErrorMessages.Count = {message.ErrorMessages.Count()}");

                    // Aggiunta record in DPA_REPORT_MAILBOX
                    var reportMailboxEntity = new ReportMailboxEntity
                    {
                        ID_CHECK_MAILBOX = request.IdCheckMailbox,
                        MAILID = message.Id,
                        TYPE = GetProcessedType(messageProcessedType),
                        RECEIPT = GetReceiptType(tipoRicevuta),
                        FROM_MAIL = email.Sender.Address,
                        DATE_MAIL = email.DeliveryDate,
                        ERROR = message.ErrorMessages.Any() ? string.Join(";", message.ErrorMessages) : null,
                        SUBJECT = message.Subject,
                        COUNT_ATTACHMENTS = message.ProcessedAttachments
                    };

                    await _dbContext.ReportMailboxEntities.AddAsync(reportMailboxEntity);
                }

                await ((DbContext)_dbContext).SaveChangesAsync();
            }

            return new(message.Processed);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<EmailCallbackHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        protected bool IsRicevutaPec(MailPecXRicevuta tipoRicevuta)
        {
            return tipoRicevuta == MailPecXRicevuta.PEC_Accept_Notify || tipoRicevuta == MailPecXRicevuta.PEC_Delivered_Notify ||
                    tipoRicevuta == MailPecXRicevuta.PEC_Non_Accept_Notify || tipoRicevuta == MailPecXRicevuta.PEC_Alert_Virus ||
                    tipoRicevuta == MailPecXRicevuta.PEC_Mancata_Consegna || tipoRicevuta == MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify ||
                    tipoRicevuta == MailPecXRicevuta.PEC_Presa_In_Carico;
        }

        protected string GetMesssageProcessedType(AnalyzedMessage message)
        {
            switch (message.EmailAttachmentType)
            {
                case MailProcessedType.Signature:
                    return Resources.ReportSignature;
                case MailProcessedType.ConfirmReception:
                    return Resources.ReportConfirmation;
                case MailProcessedType.NotifyCancellation:
                    return Resources.ReportCancellation;
                case MailProcessedType.Eccezione:
                    return Resources.ReportException;
                case MailProcessedType.DatiCert:
                    return Resources.ReportDaticert;
                case MailProcessedType.Pec:
                    return Resources.ReportPECMail;
                case MailProcessedType.NonPEC:
                    return Resources.ReportStandardMail;
                default:
                    return string.Empty;
            }
        }

        protected string GetReceiptType(MailPecXRicevuta ricevuta)
        {
            switch (ricevuta)
            {
                case MailPecXRicevuta.Delivery_Status_Notification:
                    return Resources.ReportDSNReceipt;
                case MailPecXRicevuta.From_Non_PEC:
                    return Resources.ReportMailReceipt;
                case MailPecXRicevuta.PEC_Accept_Notify:
                    return Resources.ReportAcceptanceNotification;
                case MailPecXRicevuta.PEC_Alert_Virus:
                    return Resources.ReportVirusAlertNotification;
                case MailPecXRicevuta.PEC_Contain_Virus:
                    return Resources.ReportVirusNotification;
                case MailPecXRicevuta.PEC_Delivered:
                    return Resources.ReportDeliveryNotification;
                case MailPecXRicevuta.PEC_Delivered_Notify:
                    return Resources.ReportDeliveredNotification;
                case MailPecXRicevuta.PEC_Delivered_Notify_Short:
                    return Resources.ReportDeliveryShortNotification;
                case MailPecXRicevuta.PEC_Error:
                    return Resources.ReportErrorNotification;
                case MailPecXRicevuta.PEC_Error_Delivered_Notify_By_Virus:
                    return Resources.ReportVirusErrorNotification;
                case MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify:
                    return Resources.ReportDeliveryErrorNotification;
                case MailPecXRicevuta.PEC_Mancata_Consegna:
                    return Resources.ReportFailedDeliveryNotification;
                case MailPecXRicevuta.PEC_NO_XRicevuta:
                    return Resources.ReportMissingXReceipt;
                case MailPecXRicevuta.PEC_Non_Accept_Notify:
                    return Resources.ReportRejectionNotification;
                case MailPecXRicevuta.PEC_Presa_In_Carico:
                    return Resources.ReportProcessedMessageNotification;
                case MailPecXRicevuta.unknown:
                    return Resources.ReportUnknownReceipt;
                default:
                    return string.Empty;
            }
        }

        protected string GetProcessedType(MailProcessedType? mailProcessedType)
        {
            var processedType = string.Empty;
            switch (mailProcessedType)
            {
                case MailProcessedType.ConfirmReception:
                    processedType = "ConfirmReception";
                    break;
                case MailProcessedType.DatiCert:
                    processedType = "DatiCert";
                    break;
                case MailProcessedType.Eccezione:
                    processedType = "Eccezione";
                    break;
                case MailProcessedType.NonPEC:
                    processedType = "NonPEC";
                    break;
                case MailProcessedType.NotifyCancellation:
                    processedType = "NotifyCancellation";
                    break;
                case MailProcessedType.Pec:
                    processedType = "Pec";
                    break;
                case MailProcessedType.Signature:
                    processedType = "Signature";
                    break;
            }

            return  processedType;
        }

        private bool IsPECDelivered(Email email)
        {
            return email.Headers.Where(x => x.Name == "X-Trasporto").FirstOrDefault()?.Value?.ToLower() == "posta-certificata";
        }

        private static bool IsFromNonPEC(Email email)
        {
            return email.Headers.Where(x => x.Name == "X-Trasporto").FirstOrDefault()?.Value?.ToLower() == "errore" &&
                email.Subject.Value.Trim().ToUpper().StartsWith("ANOMALIA MESSAGGIO") &&
                email.Body.Value.IndexOf("dati non sono stati certificati") >= 0 &&
                email.AttachmentsAsEmails?.Count() == 1;
        }

        protected MailPecXRicevuta GetTipoRicevutaPec(Email email)
        {
            var headerRicevuta = email.Headers.Where(x => x.Name == "X-Ricevuta").FirstOrDefault();
            var headerTrasporto = email.Headers.Where(x => x.Name == "X-Trasporto").FirstOrDefault();
            var headerVerificaSicurezza = email.Headers.Where(x => x.Name == "X-VerificaSicurezza").FirstOrDefault();
            var headerTipoRicevuta = email.Headers.Where(x => x.Name == "X-TipoRicevuta").FirstOrDefault();

            switch (headerRicevuta?.Value?.ToLower())
            {
                case "accettazione":
                    return MailPecXRicevuta.PEC_Accept_Notify;
                case "rilevazione-virus":
                    return MailPecXRicevuta.PEC_Alert_Virus;
                case "avvenuta-consegna":
                    return MailPecXRicevuta.PEC_Delivered_Notify;
                case "non-accettazione":
                    if (headerVerificaSicurezza?.Value?.ToLower() == "errore") return MailPecXRicevuta.PEC_Contain_Virus;
                    else return MailPecXRicevuta.PEC_Non_Accept_Notify;
                case "errore-consegna":
                    if (headerVerificaSicurezza?.Value?.ToLower() == "errore") return MailPecXRicevuta.PEC_Error_Delivered_Notify_By_Virus;
                    else return MailPecXRicevuta.PEC_Mancata_Consegna;
                case "preavviso-errore-consegna":
                    return MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify;
                case "presa-in-carico":
                    return MailPecXRicevuta.PEC_Presa_In_Carico;
            }

            switch (headerTrasporto?.Value?.ToLower())
            {
                case "posta-certificata":
                    return MailPecXRicevuta.PEC_Delivered;
                case "errore":
                    if (email.Subject.Value.ToUpper().StartsWith("ANOMALIA MESSAGGIO") && email.Body.Value.Contains("dati non sono stati certificati"))
                        return MailPecXRicevuta.From_Non_PEC;
                    else return MailPecXRicevuta.PEC_Error;
            }

            if (email.Headers.Where(x => x.Name == "Content-Type").FirstOrDefault()?.Value?.Contains("delivery-status") ?? false ||
               !string.IsNullOrWhiteSpace(email.Headers.Where(x => x.Name == "X-dsn").FirstOrDefault()?.Value))
                return MailPecXRicevuta.Delivery_Status_Notification;

            if (headerRicevuta is not null || headerTrasporto is not null || headerTipoRicevuta is not null)
                return MailPecXRicevuta.PEC_NO_XRicevuta;
            else
                return MailPecXRicevuta.unknown;
        }

        /// <summary>
        /// Verifica se la mail è già stata elaborata a partire dal suo messageId
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        protected bool IsMessageAlreadyProcessed(string messageId, long idRegister)
        {
            //if (this._emailBoxType == EmailBoxTypeEnum.IMAP) return true; //??

            if (string.IsNullOrWhiteSpace(messageId)) return true;

            return _dbContext.MailElaborataEntities.AsNoTracking()
                .Where(x => x.VAR_MESSAGE == messageId
                && x.CHA_RAGIONE_ELAB == "E"
                && (x.ID_REGISTRO == null || x.ID_REGISTRO == idRegister))
                .Any();
        }

        protected MailProcessedType? AnalyzeMailAttachment(AnalyzedMessage message, EmailContentAttachment attachment)
        {
            switch (attachment.FileName.ToLower())
            {
                case "eccezione.xml":
                    //message.HasException = true;
                    message.AttachmentContent = attachment.Content;
                    message.EmailAttachmentType = MailProcessedType.Eccezione;
                    return MailProcessedType.Eccezione;
                case "conferma.xml":
                case "confermaricezione.xml":
                    //message.HasConfirmation = true;
                    message.AttachmentContent = attachment.Content;
                    message.EmailAttachmentType = MailProcessedType.ConfirmReception;
                    return MailProcessedType.ConfirmReception;
                case "segnatura.xml":
                    //message.HasSignature = true;
                    message.AttachmentContent = attachment.Content;
                    message.EmailAttachmentType = MailProcessedType.Signature;
                    return MailProcessedType.Signature;
                case "annullamento.xml":
                    //message.HasCancellationInfo = true;
                    message.AttachmentContent = attachment.Content;
                    message.EmailAttachmentType = MailProcessedType.NotifyCancellation;
                    return MailProcessedType.NotifyCancellation;
                case "daticert.xml":
                    //message.HasDatiCert = true;
                    message.AttachmentContent = attachment.Content;
                    message.EmailAttachmentType = MailProcessedType.DatiCert;
                    return MailProcessedType.DatiCert;
                default:
                    if (attachment.FileName.ToLower().Contains(".xml"))
                    {
                        // TO DO
                        // Gestione fatture elettroniche
                    }
                    break;
            }

            return null;
        }

        #endregion
    }
}
