// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using MimeKit;
using Pi3Core = Pi3.Core.Services.Email.BoxScanner;
using System.Collections.Specialized;
using Pi3.Core.Services.Email.BoxScanner;
using System.ComponentModel.DataAnnotations;

namespace Pi3.Infrastructure.Graph.Services.Email.BoxScanner
{
    public class GraphEmailBoxScannerService : IEmailBoxScannerService
    {
        #region Public Members

        public GraphEmailBoxScannerService(ILogger<GraphEmailBoxScannerService> logger)
        {
            this._logger = logger;
        }

        public virtual string Provider => "MSGRAPH";


        public virtual async Task Scan(Action<EmailBoxConfigurations> loadEmailBoxConfigurations, Func<EmailBoxScannerCallback, bool> callback)
        {
            var emailBoxConfigurations = new GraphEmailBoxConfiguration();

            loadEmailBoxConfigurations(emailBoxConfigurations);

            Validator.ValidateObject(emailBoxConfigurations, new ValidationContext(emailBoxConfigurations), true);

            string missingParameters = string.Empty;
            if (!this.ValidateConfiguration(emailBoxConfigurations, out missingParameters))
                throw new GraphArgumentNotFoundPi3Exception(missingParameters);

            this._clientId = emailBoxConfigurations.ClientId;
            this._tenantId = emailBoxConfigurations.TenantId;
            this._clientSecret = emailBoxConfigurations.ClientSecret;
            this._mailbox = emailBoxConfigurations.MailBox;
            this._folderToRead = emailBoxConfigurations.FolderToRead ?? string.Empty;

            try
            {
                #region Login
                this._scopes = new string[] { Resources.ScopeUrl };

                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                };

                var clientSecretCredential = new ClientSecretCredential(
                        this._tenantId, this._clientId, this._clientSecret, options);

                this._graphClient = new GraphServiceClient(clientSecretCredential, this._scopes);

                #endregion

                #region Recupero lista messaggi
                List<Message> messages = new List<Message>();
                if (!string.IsNullOrWhiteSpace(this._folderToRead))
                {
                    var folder = (await _graphClient.Users[this._mailbox].MailFolders.GetAsync((requestConfiguration) =>
                    {
                        requestConfiguration.QueryParameters.Filter = String.Format("displayName eq '{0}'", this._folderToRead);
                    })).Value?.FirstOrDefault();

                    if (folder != null)
                    {
                        messages = (await _graphClient.Users[this._mailbox].MailFolders[folder.Id].Messages.GetAsync((requestConfiguration) =>
                        {
                            requestConfiguration.QueryParameters.Select = new string[] { "*", "internetMessageHeaders" };
                            requestConfiguration.QueryParameters.Top = 1000;//Altrimenti li pagina a 10 alla volta
                        })).Value;
                    }
                    else
                        this._logger.LogError(String.Format(ErrorDescriptions.FolderNotFound, this._folderToRead));
                }
                else
                {
                    messages = (await _graphClient.Users[this._mailbox].MailFolders[Resources.InboxFolder].Messages.GetAsync((requestConfiguration) =>
                    {
                        requestConfiguration.QueryParameters.Select = new string[] { "*", "internetMessageHeaders" };
                        requestConfiguration.QueryParameters.Top = 1000;//Altrimenti li pagina a 10 alla volta
                    })).Value;
                }

                this._messagesCount = messages.Count;
                #endregion

                #region Parse dei messaggi
                for (int i = 0; i < this._messagesCount; i++)
                {
                    var message = messages[i];
                    var messageStream = await _graphClient.Users[this._mailbox].Messages[message.Id].Content.GetAsync();

                    if (messageStream == null)
                        this._logger.LogError("Stream del messaggio null");

                    byte[] messageBytes = ReadStream(messageStream);
                    MimeMessage mimeMessage = new MimeMessage();
                    var parser = new MimeParser(new MemoryStream(messageBytes), MimeFormat.Default);
                    mimeMessage = parser.ParseMessage();

                    DateTime ReceiveDate = DateTime.MinValue;

                    IEnumerable<MimeEntity> mimeAttachments = mimeMessage.Attachments.ToList();

                    if (IsPECDelivered(mimeMessage) || IsFromNonPEC(mimeMessage))
                    {
                        //var envelopeMessage = mimeMessage.BodyParts?.FirstOrDefault(x => x.ContentType.MimeType == "message/rfc822");
                        var envelopeMessage = mimeMessage.BodyParts?.ToList().Where(p => p is MessagePart && p.ContentType.Name.ToLower().Equals(Resources.PostaCertAttachmentName)).FirstOrDefault();

                        if (envelopeMessage != null && envelopeMessage is MessagePart)
                            mimeMessage = (envelopeMessage as MessagePart).Message;
                    }

                    var elaborata = callback(new EmailBoxScannerCallback()
                    {
                        Email = this.AsPi3Email(mimeMessage),
                        Current = i,
                        Total = this._messagesCount
                    });

                    #region Spostamento in cartelle
                    if (!await Move(message.Id, elaborata))
                        this._logger.LogDebug($"Problemi nello spostamento della mail con id {message.Id}");

                    #endregion
                }
                #endregion

            }
            catch (Exception ex)
            {
                this._logger.LogError($"Eccezione nella consultazione della casella {ex.Message}");
                throw new BoxScannerPi3Exception(ex.Message);
            }
        }

        private async Task<bool> Move(string messageId, bool elaborata)
        {
            Message resultMove = null;

            string folder = elaborata ? Resources.MailElaborate : Resources.MailNonElaborate;
            string folderId = string.Empty;

            #region Ricerca cartella
            var folderToMove = (await _graphClient.Users[this._mailbox].MailFolders.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Filter = String.Format("displayName eq '{0}'", folder);
                })).Value.FirstOrDefault();

            if (folderToMove == null)
            {
                //Creo la cartella se non esiste
                var resultCreateFolder = _graphClient.Users[this._mailbox].MailFolders
                    .PostAsync(new Microsoft.Graph.Models.MailFolder
                    {
                        DisplayName = folder,
                        IsHidden = false,
                    }).Result;
                if (resultCreateFolder != null && !string.IsNullOrEmpty(resultCreateFolder.Id))
                {
                    folderId = resultCreateFolder.Id;
                }
            }
            else
            {
                folderId = folderToMove.Id;
            }
            #endregion

            #region Spostamento
            resultMove = _graphClient.Users[this._mailbox].Messages[messageId].Move
                .PostAsync(new Microsoft.Graph.Users.Item.Messages.Item.Move.MovePostRequestBody
                {
                    DestinationId = folderId,
                }).Result;
            #endregion

            return resultMove != null;
        }
        #endregion

        #region Private Members

        protected ILogger<GraphEmailBoxScannerService> _logger;
        private string[] _scopes;
        private string _tenantId;
        private string _clientId;
        private string _clientSecret;
        private string _mailbox;
        private string _folderToRead;
        private GraphServiceClient _graphClient;
        private Microsoft.Graph.Models.MailFolder _inboxFolder;
        private int _messagesCount;

        private Pi3Core.Email AsPi3Email(MimeMessage mimeMessage)
        {
            var attachmentsAsEmails = new List<Pi3Core.Email>();
            var attachments = new List<Pi3Core.EmailContentAttachment>();
            var headers = new List<Pi3Core.EmailHeader>();
            var toRecipients = new List<Pi3Core.EmailRecipient>();
            var ccRecipients = new List<Pi3Core.EmailRecipient>();
            byte[] binaryContent = null;

            #region Allegati eml
            var emailAttachments = mimeMessage.BodyParts?.ToList().Where(p => p is MessagePart).ToList();
            foreach (var emailAttachment in emailAttachments)
                attachmentsAsEmails.Add(this.AsPi3Email((emailAttachment as MessagePart).Message));
            #endregion

            #region Allegati
            foreach (var attachment in mimeMessage.Attachments)
            {
                if (attachment is MimePart)
                {
                    byte[] attachmentByteArray = null;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        //attachment.WriteTo(memoryStream);
                        ((MimePart)attachment).Content.DecodeTo(memoryStream);
                        attachmentByteArray = memoryStream.ToArray();
                    }
                    attachments.Add(new Pi3Core.EmailContentAttachment
                    {
                        Content = attachmentByteArray,
                        ContentType = attachment.ContentType.Name,
                        FileName = attachment.ContentDisposition.FileName
                    });
                }
            }
            #endregion

            #region Headers
            foreach (var header in mimeMessage.Headers)
                headers.Add(new Pi3Core.EmailHeader
                {
                    Name = header.Field,
                    Value = header.Value
                });
            #endregion

            #region Destinatari in TO
            foreach (var recipientInTo in mimeMessage.To.Mailboxes)
                toRecipients.Add(new Pi3Core.EmailRecipient
                {
                    Address = recipientInTo.Address,
                    DisplayName = recipientInTo.Name
                });
            #endregion

            #region Destinatari in CC 
            foreach (var recipientInCC in mimeMessage.Cc.Mailboxes)
                toRecipients.Add(new Pi3Core.EmailRecipient
                {
                    Address = recipientInCC.Address,
                    DisplayName = recipientInCC.Name
                });
            #endregion

            #region Content della mail
            using (MemoryStream memoryStream = new MemoryStream())
            {
                mimeMessage.WriteTo(memoryStream);
                binaryContent = memoryStream.ToArray();
            }
            #endregion

            #region Mittente
            var sender = mimeMessage.From.Mailboxes.FirstOrDefault();
            #endregion

            return new Pi3Core.Email
            {
                Sender = new Pi3Core.EmailSender { Address = sender.Address, DisplayName = sender.Name },
                To = toRecipients,
                Cc = ccRecipients,
                Subject = new Core.SeedWork.TextValue(mimeMessage.Subject),
                Body = new Core.SeedWork.TextValue(mimeMessage.GetTextBody(MimeKit.Text.TextFormat.Plain)),
                AttachmentsAsEmails = attachmentsAsEmails,
                Attachments = attachments,
                Headers = headers,
                BinaryContent = binaryContent,
                DeliveryDate = mimeMessage.Date.DateTime.ToLocalTime()
            };

        }
        private bool ValidateConfiguration(GraphEmailBoxConfiguration emailBoxConfigurations, out string missingParameters)
        {
            missingParameters = string.Empty;

            if (string.IsNullOrEmpty(emailBoxConfigurations.TenantId))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.TenantId : $"; {Resources.TenantId}";
            if (string.IsNullOrEmpty(emailBoxConfigurations.ClientId))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.ClientId : $"; {Resources.ClientId}";
            if (string.IsNullOrEmpty(emailBoxConfigurations.ClientSecret))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.ClientSecret : $"; {Resources.ClientSecret}";
            if (string.IsNullOrEmpty(emailBoxConfigurations.MailBox))
                missingParameters += string.IsNullOrEmpty(missingParameters) ? Resources.MailBox : $"; {Resources.MailBox}";

            return string.IsNullOrEmpty(missingParameters);
        }
        public virtual async Task<bool> Find(string filter)
        {
            return filter == this.Provider;
        }
        private static byte[] ReadStream(Stream? input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private bool IsPECDelivered(MimeMessage email)
        {
            return email.Headers?.Where(x => x.Field.Equals("X-Trasporto")).Select(x => x.Value).FirstOrDefault()?.ToLower() == "posta-certificata";
        }
        private static bool IsFromNonPEC(MimeMessage email)
        {
            return IsError(email) &&
                email.Subject.Trim().ToUpper().StartsWith("ANOMALIA MESSAGGIO") &&
                email.GetTextBody(MimeKit.Text.TextFormat.Plain).IndexOf("dati non sono stati certificati") >= 0 &&
                email.Attachments?.ToList().Where(p => p.ContentType.Equals("message/rfc822")).FirstOrDefault() != null;
        }
        private static bool IsError(MimeMessage email)
        {
            return email.Headers?.Where(x => x.Field.Equals("X-Trasporto")).Select(x => x.Value).FirstOrDefault()?.ToLower() == "errore";
        }
        public Task<Core.Services.Email.BoxScanner.Email> Parse(Stream stream)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
