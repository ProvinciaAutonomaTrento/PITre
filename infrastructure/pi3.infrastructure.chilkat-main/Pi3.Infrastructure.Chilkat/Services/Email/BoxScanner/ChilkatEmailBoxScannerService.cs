// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChilkatLib = Chilkat;
using Pi3Core = Pi3.Core.Services.Email.BoxScanner;

namespace Pi3.Infrastructure.Chilkat.Services.Email.BoxScanner
{
    public class ChilkatEmailBoxScannerService : IEmailBoxScannerService
    {
        #region Public members
        public ChilkatEmailBoxScannerService(
            ILogger<ChilkatEmailBoxScannerService> logger,
            IOptions<ChilkatOptions> options)
        {
            this._logger = logger;
            this._options = options;
        }

        public string Provider => "CHILKAT";

        public async Task<Pi3Core.Email> Parse(Stream stream)
        {
            Validator.ValidateObject(stream, new ValidationContext(stream), true);

            string emailPath = null!;

            try
            {
                if (!Directory.Exists(this._options.Value.SpoolFolder))
                    throw new DirectoryNotFoundException(this._options.Value.SpoolFolder);

                emailPath = Path.Combine(this._options.Value.SpoolFolder, $"{Guid.NewGuid()}.eml");

                using (var fileStream = new FileStream(emailPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    stream.CopyTo(fileStream);
                    fileStream.Flush();
                }

                ChilkatLib.Email email = new ChilkatLib.Email();

                if (!email.LoadEml(emailPath))
                    throw new ChilkatBoxScannerPi3Exception(email.LastErrorText);

                return this.AsPi3Email(email);
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);

                throw new ChilkatBoxScannerPi3Exception(ex.Message);
            }
            finally
            {
                try
                {
                    if (System.IO.File.Exists(emailPath))
                        System.IO.File.Delete(emailPath);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(exception: ex, ex.Message);
                }
            }
        }

        public async Task Scan(Action<EmailBoxConfigurations> loadEmailBoxConfigurations, Func<EmailBoxScannerCallback, bool> callback)
        {
            var emailBoxConfigurations = new ChilkatEmailBoxConfigurations();

            loadEmailBoxConfigurations(emailBoxConfigurations);

            Validator.ValidateObject(emailBoxConfigurations, new ValidationContext(emailBoxConfigurations), true);

            string missingParameters = string.Empty;
            if (!this.ValidateConfiguration(emailBoxConfigurations, out missingParameters))
                throw new ChilkatArgumentNotFoundPi3Exception(missingParameters);

            ChilkatLib.Global global = new ChilkatLib.Global();
            if (!global.UnlockBundle(this._options.Value.LicenseKey))
                throw new ChilkatUnlockPi3Exception();

            switch (emailBoxConfigurations.EmailBoxTypeEnum)
            {
                case EmailBoxTypeEnum.POP:
                    await this.ScanPOP(emailBoxConfigurations, callback);
                    break;

                case EmailBoxTypeEnum.IMAP:
                    await this.ScanIMAP(emailBoxConfigurations, callback);
                    break;
            }
        }

        private bool ValidateConfiguration(ChilkatEmailBoxConfigurations emailBoxConfigurations, out string missingParameters)
        {
            missingParameters = string.Empty;

            if (string.IsNullOrEmpty(emailBoxConfigurations.Host))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.Host : $"; {Resources.Host}";
            
            if (emailBoxConfigurations.Port == 0)
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.Port : $"; {Resources.Port}";
            
            return string.IsNullOrEmpty(missingParameters);
        }


        #endregion

        #region Private members

        protected readonly ILogger<ChilkatEmailBoxScannerService> _logger;
        protected IOptions<ChilkatOptions> _options;

        protected virtual Pi3Core.Email AsPi3Email(ChilkatLib.Email email)
        {
            var attachmentsAsEmails = new List<Pi3Core.Email>();
            var attachments = new List<Pi3Core.EmailContentAttachment>();
            var headers = new List<Pi3Core.EmailHeader>();
            var toRecipients = new List<Pi3Core.EmailRecipient>();
            var ccRecipients = new List<Pi3Core.EmailRecipient>();

            for (int i = 0; i < email.NumAttachedMessages; i++)
            {
                attachmentsAsEmails.Add(this.AsPi3Email(email.GetAttachedMessage(i)));
            }

            for (int i = 0; i < email.NumAttachments; i++)
            {
                attachments.Add(new Pi3Core.EmailContentAttachment
                {
                    Content = email.GetAttachmentData(i),
                    ContentType = email.GetAttachmentContentType(i),
                    FileName = email.GetAttachmentFilename(i)
                });
            }

            for (int i = 0; i < email.NumHeaderFields; i++)
            {
                headers.Add(new Pi3Core.EmailHeader
                {
                    Name = email.GetHeaderFieldName(i),
                    Value = email.GetHeaderFieldValue(i)
                });
            }

            for (int i = 0; i < email.NumTo; i++)
            {
                toRecipients.Add(new Pi3Core.EmailRecipient
                {
                    Address = email.GetToAddr(i),
                    DisplayName = email.GetToName(i)
                });
            }

            for (int i = 0; i < email.NumCC; i++)
            {
                ccRecipients.Add(new Pi3Core.EmailRecipient
                {
                    Address = email.GetCcAddr(i),
                    DisplayName = email.GetCcName(i)
                });
            }

            return new Pi3Core.Email
            {
                Sender = new Pi3Core.EmailSender { Address = email.FromAddress, DisplayName = email.FromName },
                To = toRecipients,
                Cc = ccRecipients,
                Subject = new Core.SeedWork.TextValue(email.Subject),
                Body = new Core.SeedWork.TextValue(email.Body),
                AttachmentsAsEmails = attachmentsAsEmails,
                Attachments = attachments,
                Headers = headers,
                BinaryContent = email.GetMimeBinary(),
                //DeliveryDate = DateTime.Parse(email.EmailDateStr, new System.Globalization.CultureInfo("it-IT"))
                //DeliveryDate = DateTime.Parse(email.LocalDateStr, new System.Globalization.CultureInfo("it-IT")).ToLocalTime()
                DeliveryDate = ConvertToLocalDateTime(DateTime.Parse(email.LocalDateStr, new System.Globalization.CultureInfo("it-IT")))
            };
        }

        private DateTime ConvertToLocalDateTime(DateTime dateTime)
        {
            // Definisci il fuso orario CEST (Central European Summer Time)
            TimeZoneInfo cestTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

            // Converti la data e ora da UTC a CEST
            DateTime cestDateTime = TimeZoneInfo.ConvertTime(dateTime, cestTimeZone);

            return cestDateTime;
        }

        private async Task ScanPOP(ChilkatEmailBoxConfigurations emailBoxConfigurations, Func<EmailBoxScannerCallback, bool> callback)
        {
            emailBoxConfigurations = emailBoxConfigurations ?? throw new ArgumentNullException(nameof(emailBoxConfigurations));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            Validator.ValidateObject(emailBoxConfigurations, new ValidationContext(emailBoxConfigurations));

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ChilkatLib.EmailBundle emailBundle = null!;

            using (var mailManager = new ChilkatLib.MailMan())
            {
                this._logger.LogInformation($"Connessione casella POP - MailHost: {emailBoxConfigurations.Host}, MailPort: {emailBoxConfigurations.Port}, PopUsername: {emailBoxConfigurations.UserName}");

                mailManager.MailHost = emailBoxConfigurations.Host;
                mailManager.MailPort = Convert.ToInt32(emailBoxConfigurations.Port);
                mailManager.PopSsl = Convert.ToBoolean(emailBoxConfigurations.RequireSsl);
                mailManager.StartTLS = Convert.ToBoolean(emailBoxConfigurations.StartTls);

                mailManager.PopUsername = emailBoxConfigurations.UserName ?? string.Empty;
                mailManager.PopPassword = emailBoxConfigurations.Password ?? string.Empty;

                try
                {
                   
                    var uidls = mailManager.GetUidls();
                    this._logger.LogInformation($"PopUsername: {emailBoxConfigurations.UserName} - GetUidls: {uidls.Count}");

                    emailBundle = mailManager.FetchMultipleHeaders(uidls, 2);
                    var alreadyProcessedIndexes = new List<int>();
                    var totalEmails = emailBundle.MessageCount;

                    this._logger.LogInformation($"PopUsername: {emailBoxConfigurations.UserName} - emailBundle.MessageCount: {emailBundle.MessageCount}");

                    if (emailBoxConfigurations.IsEmailProcessedByMessageId != null!)
                    {   
                        for (int i = 0; i < emailBundle.MessageCount; i++)
                        {
                            var email = emailBundle.GetEmail(i);

                            var messageId = email.GetHeaderField("Message-ID");
                            if (emailBoxConfigurations.IsEmailProcessedByMessageId(messageId))
                            {
                                alreadyProcessedIndexes.Add(i);
                                this._logger.LogInformation($"PopUsername: {emailBoxConfigurations.UserName} - messageId '{messageId}' già processato");
                            }
                        }

                        totalEmails -= alreadyProcessedIndexes.Count;

                    }

                    this._logger.LogInformation($"PopUsername: {emailBoxConfigurations.UserName} - Totale email da processare: {totalEmails}");

                    var current = 1;

                    for (int i = 0; i < emailBundle.MessageCount; i++)
                    {
                        if (!alreadyProcessedIndexes.Contains(i))
                        {                            
                            var email = emailBundle.GetEmail(i);
                            var currentUidl = email.Uidl;
                            var currentMessageId = email.GetHeaderField("Message-ID");

                            if (!mailManager.IsPop3Connected)
                            {
                                mailManager.Pop3EndSession();
                                mailManager.Pop3BeginSession();
                            }

                            var attachedMessage = new ChilkatLib.Email();
                            attachedMessage.AttachMessage(mailManager.FetchMime(email.Uidl));
                            email = attachedMessage.GetAttachedMessage(0);

                            //if (IsPECDelivered(email) || IsFromNonPEC(email))
                            //{
                            //    this._logger.LogInformation($"PopUsername: {emailBoxConfigurations.UserName} - messageId '{currentMessageId}' IsPECDelivered");

                            //    email = email.GetAttachedMessage(0);
                            //}

                            bool processed = false;

                            processed = callback(new EmailBoxScannerCallback()
                            {
                                Email = this.AsPi3Email(email),
                                Current = current,
                                Total = totalEmails
                            });

                            if (processed)
                            {
                                this._logger.LogInformation($"PopUsername: {emailBoxConfigurations.UserName} - messageId '{currentMessageId}' processato");

                                // La callback ha processato correttamente il messaggio
                                if ((emailBoxConfigurations!.POPBehavior ?? new EmailBoxPOPBehavior()).DeleteEmailIfProcessed ?? false)
                                {
                                    // Il consumer ha richiesto la rimozione dell'email
                                    var removed = mailManager.DeleteByUidl(currentUidl);
                                    if (!removed)
                                    {
                                        this._logger.LogInformation($"PopUsername: {emailBoxConfigurations.UserName} - Errore nella rimozione del messaggio con messageId '{currentMessageId}'");
                                    }
                                }
                            }

                            current++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, message: ex.Message);

                    if (!string.IsNullOrWhiteSpace(mailManager.LastErrorText))
                    {
                        _logger.LogCritical(mailManager.LastErrorText);
                        throw new ChilkatBoxScannerPi3Exception(mailManager.LastErrorText);
                    }
                    else 
                        throw;
                }
                finally
                {
                    if(mailManager.IsPop3Connected)
                        mailManager.Pop3EndSession();

                    mailManager.Dispose();

                    sw.Stop();

                    this._logger.LogInformation($"Connessione casella POP - MailHost: {emailBoxConfigurations.Host}, MailPort: {emailBoxConfigurations.Port}, PopUsername: {emailBoxConfigurations.UserName} - Elapsed sec.: {sw.Elapsed.TotalSeconds}");
                }
            }
        }

        private async Task ScanIMAP(ChilkatEmailBoxConfigurations emailBoxConfigurations, Func<EmailBoxScannerCallback, bool> callback)
        {
            emailBoxConfigurations = emailBoxConfigurations ?? throw new ArgumentNullException(nameof(emailBoxConfigurations));
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            Validator.ValidateObject(emailBoxConfigurations, new ValidationContext(emailBoxConfigurations));
            
            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (var imap = new ChilkatLib.Imap())
            {
                this._logger.LogInformation($"Connessione casella IMAP - MailHost: {emailBoxConfigurations.Host}, MailPort: {emailBoxConfigurations.Port}, PopUsername: {emailBoxConfigurations.UserName}");

                try
                {
                    imap.Port = Convert.ToInt32(emailBoxConfigurations.Port);
                    imap.Ssl = Convert.ToBoolean(emailBoxConfigurations.RequireSsl);
                    imap.StartTls = Convert.ToBoolean(emailBoxConfigurations.StartTls);

                    imap.Connect(emailBoxConfigurations.Host);

                    if (!imap.Login(emailBoxConfigurations.UserName ?? string.Empty, emailBoxConfigurations.Password ?? string.Empty))
                        throw new ChilkatBoxScannerPi3Exception(imap.LastErrorText);

                    var inbox = emailBoxConfigurations.IMAPBehavior!.Folders!.CurrentFolder ?? Resources.Inbox;

                    var elabFolder = emailBoxConfigurations.IMAPBehavior!.Folders!.ProcessedFolder ?? inbox;
                    var nonElabFolder = emailBoxConfigurations.IMAPBehavior!.Folders!.FailedFolder ?? inbox;

                    imap.CreateMailbox(inbox);
                    imap.SelectMailbox(inbox);

                    imap.CreateMailbox(elabFolder);
                    imap.CreateMailbox(nonElabFolder);
                    
                    var messageSet = imap.Search("ALL", true);

                    var emailBundle = imap.FetchHeaders(messageSet);

                    var alreadyProcessedIndexes = new List<int>();
                    var totalEmails = emailBundle.MessageCount;

                    if (emailBoxConfigurations.IsEmailProcessedByMessageId != null!)
                    {
                        for (int i = 0; i < emailBundle.MessageCount; i++)
                        {
                            var email = emailBundle.GetEmail(i);

                            var messageId = email.GetHeaderField("Message-ID");

                            if (emailBoxConfigurations.IsEmailProcessedByMessageId(messageId))
                            {
                                alreadyProcessedIndexes.Add(i);
                                this._logger.LogInformation($"ImapUsername: {emailBoxConfigurations.UserName} - messageId '{messageId}' già processato");
                            }
                        }

                        totalEmails -= alreadyProcessedIndexes.Count;
                    }

                    this._logger.LogInformation($"ImapUsername: {emailBoxConfigurations.UserName} - Totale email da processare: {totalEmails}");

                    var current = 1;

                    for (int i = 0; i < emailBundle.MessageCount; i++)
                    {
                        if (!alreadyProcessedIndexes.Contains(i))
                        {
                            try
                            {
                                var email = emailBundle.GetEmail(i);

                                var attachedMessage = new ChilkatLib.Email();
                                var mime = new ChilkatLib.Mime();
                                mime.LoadMime(imap.FetchSingleAsMime(email.GetImapUid(), true));
                                attachedMessage.AttachMessage(mime.GetMimeBytes());

                                var id = messageSet.GetId(i);

                                email = attachedMessage.GetAttachedMessage(0);

                                //if (IsPECDelivered(email) || IsFromNonPEC(email))
                                //{
                                //    email = email.GetAttachedMessage(0);
                                //}

                                var callbackResult = callback(new EmailBoxScannerCallback()
                                {
                                    Email = this.AsPi3Email(email),
                                    Current = current,
                                    Total = totalEmails
                                });

                                if (!imap.Copy(id, true, callbackResult ? elabFolder : nonElabFolder))
                                    throw new Exception();

                                if (!imap.SetFlag(id, true, "Deleted", 1))
                                    throw new Exception();
                            }
                            catch (Exception)
                            {
                                this._logger.LogCritical(imap.LastErrorText);
                            }
                        }

                        current++;
                    }

                    if (!imap.ExpungeAndClose())
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    this._logger.LogCritical(ex, ex.Message);
                    if (!string.IsNullOrWhiteSpace(imap.LastErrorText))
                    {
                        this._logger.LogCritical(imap.LastErrorText);
                        throw new ChilkatBoxScannerPi3Exception(imap.LastErrorText);
                    }
                    else throw;
                }
                finally
                {
                    if(imap != null && imap.IsConnected())
                        imap.Disconnect();

                    if(imap != null)
                        imap.Dispose();

                    sw.Stop();

                    this._logger.LogInformation($"Connessione casella IMAP - MailHost: {emailBoxConfigurations.Host}, MailPort: {emailBoxConfigurations.Port}, PopUsername: {emailBoxConfigurations.UserName} - Elapsed sec.: {sw.Elapsed.TotalSeconds}");
                }
            }
        }
        private bool ValidateConfiguration(System.Collections.Specialized.StringDictionary? arguments, out string missingParameters)
        {
            missingParameters = string.Empty;

            if (!arguments!.ContainsKey("EmailBoxTypeEnum") || string.IsNullOrEmpty(arguments["EmailBoxTypeEnum"]))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.EmailBoxType : $"; {Resources.EmailBoxType}";
            if (!arguments.ContainsKey("Host") || string.IsNullOrEmpty(arguments["Host"]))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.Host : $"; {Resources.Host}";
            if (!arguments.ContainsKey("Port") || string.IsNullOrEmpty(arguments["Port"]))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.Port : $"; {Resources.Port}";
            if (!arguments.ContainsKey("RequireSsl") || string.IsNullOrEmpty(arguments["RequireSsl"]))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.RequireSsl : $"; {Resources.RequireSsl}";

            return string.IsNullOrEmpty(missingParameters);
        }

        #region Controllo ricevute
        private bool IsRicevutaPEC(ChilkatLib.Email email)
        {
            return IsPECAcceptNotify(email) || //avvenuta accettazione
                IsDeliveryStatusNotification(email) || //messaggi non ricevuti
                IsFromNonPEC(email) || //messaggi non Pec
                IsPECAlertVirus(email) || //rilevazione virus
                IsPECContainVirus(email) || //non accettazione
                IsPECDelivered(email) || //Consegnata
                IsPECDeliveredNotify(email) || //avvenuta consegna
                IsPECDeliveredNotifyShort(email) ||
                IsError(email) || //errore generico
                IsPECErrorDeliveredNotifyByVirus(email) ||  //errore consegna
                IsPECErrorPreavvisoDeliveredNotify(email) ||  //preavviso errore consegna
                IsPECNonAcceptNotify(email) || //non accettazione
                IsPECPresaInCarico(email) || //presa in carico
                IsPECErrorDeliveredNotify(email);

        }
        private bool IsPEC(ChilkatLib.Email email)
        {
            return !string.IsNullOrEmpty(email.GetHeaderField("X-Ricevuta"))
                || !string.IsNullOrEmpty(email.GetHeaderField("X-TipoRicevuta"));
        }
        private bool IsPECErrorDeliveredNotify(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "errore-consegna";
        }
        private bool IsPECPresaInCarico(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "presa-in-carico";
        }
        private bool IsPECNonAcceptNotify(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "non-accettazione";
        }
        private bool IsPECErrorPreavvisoDeliveredNotify(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "preavviso-errore-consegna";
        }
        private bool IsPECErrorDeliveredNotifyByVirus(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "errore-consegna" && email.GetHeaderField("X-VerificaSicurezza")?.ToLower() == "errore";
        }
        private bool IsPECDeliveredNotifyShort(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "avvenuta-consegna" &&
                (email.GetHeaderField("X-TipoRicevuta")?.ToLower() == "breve" || email.GetHeaderField("X-TipoRicevuta")?.ToLower() == "sintetica");
        }
        private bool IsPECDeliveredNotify(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "avvenuta-consegna";
        }
        private bool IsPECDelivered(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Trasporto")?.ToLower() == "posta-certificata";
        }
        private bool IsPECContainVirus(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "non-accettazione" && email.GetHeaderField("X-VerificaSicurezza")?.ToLower() == "errore";
        }
        private bool IsPECAlertVirus(ChilkatLib.Email email)
        {
            return !string.IsNullOrEmpty(email.GetHeaderField("X-Mittente")) && email.GetHeaderField("X-Ricevuta")?.ToLower() == "rilevazione-virus";
        }
        private bool IsDeliveryStatusNotification(ChilkatLib.Email email)
        {
            bool retvalue = false;

            var contentTypeHeader = email.GetHeaderField("Content-Type")?.ToLower();
            if (!string.IsNullOrEmpty(contentTypeHeader) && contentTypeHeader.IndexOf("delivery-status") >= 0)
            {
                this._logger.LogDebug("Delivery Status Notification");
                this._logger.LogDebug($"Content-Type {contentTypeHeader}");
                retvalue = true;
            }

            //Campo x-dsn supportato da certi provider
            var dsnHeader = email.GetHeaderField("X-dsn")?.ToLower();
            if (!string.IsNullOrEmpty(dsnHeader))
            {
                this._logger.LogDebug("Delivery Status Notification");
                retvalue = true;
            }

            return retvalue;
        }
        private bool IsPECAcceptNotify(ChilkatLib.Email email)
        {
            //avvenuta accettazione
            return email.GetHeaderField("X-Ricevuta")?.ToLower() == "accettazione";
        }
        private static bool IsError(ChilkatLib.Email email)
        {
            return email.GetHeaderField("X-Trasporto")?.ToLower() == "errore";
        }
        private static bool IsFromNonPEC(ChilkatLib.Email email)
        {
            return IsError(email) &&
                email.Subject.Trim().ToUpper().StartsWith("ANOMALIA MESSAGGIO") &&
                email.Body.IndexOf("dati non sono stati certificati") >= 0 &&
                email.NumAttachedMessages == 1;
        }


        #endregion

        #endregion
    }
}
