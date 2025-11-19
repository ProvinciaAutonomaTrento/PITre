// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Pi3.App.Legacy.WebApi.Application.Handlers.ScanGmailCallback.Exceptions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Email.BoxScanner;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ScanGmailCallback
{
    internal static class GmailMessageParser
    {
        public static async Task<Email> Parse(Message email, GmailService service, IEmailBoxScannerService emailBoxScanner)
        {

            var sender = DecodeSender(GetHeader(email.Payload.Headers, "From"));
            var to = DecodeEmails(GetHeader(email.Payload.Headers, "To"));
            var cc = DecodeEmails(GetHeader(email.Payload.Headers, "Cc"));
            var bcc = DecodeEmails(GetHeader(email.Payload.Headers, "Bcc"));
            var subject = GetHeader(email.Payload.Headers, "Subject");
            var date = (DecodeDateTime(GetHeader(email.Payload.Headers, "Date"))).ToLocalTime();
            var body = GetMessageContent(email.Payload);
            var attachments = await GetAttachments(service, email);
            var headers = email.Payload.Headers.Select(header => new EmailHeader()
            {
                Name = header.Name,
                Value = header.Value
            }).ToList();

            var emailAttanchments = new List<Email>();

            foreach (var attachment in attachments ?? Enumerable.Empty<EmailContentAttachment>())
            {
                if (attachment.FileName.ToLower().EndsWith(".eml"))
                {
                    using var stream = new MemoryStream(attachment.Content);

                    var emailAttach = await emailBoxScanner.Parse(stream);

                    emailAttanchments.Add(emailAttach);
                }
            }

            var emailEntry = new Core.Services.Email.BoxScanner.Email()
            {
                Attachments = attachments,
                Body = new TextValue(body),
                Cc = cc,
                Bcc = bcc,
                DeliveryDate = date,
                Headers = headers,
                Sender = sender,
                Subject = new TextValue(subject),
                To = to,
                AttachmentsAsEmails = emailAttanchments
            };

            return emailEntry;
        }

        private static DateTime DecodeDateTime(string dateString)
        {
            //  Rimuove la  parte  tra  parentesi (es.  " (UTC)")
            int parenIndex = dateString.IndexOf(" (");
            if (parenIndex != -1)
            {
                dateString = dateString.Substring(0, parenIndex);
            }

            string[] formats = new string[] {
                "ddd, dd MMM yyyy HH:mm:ss zzz",
                "ddd, d MMM yyyy HH:mm:ss zzz",
                "dd MMM yyyy HH:mm:ss zzz",
                "d MMM yyyy HH:mm:ss zzz",
                "dd MMM yyyy HH:mm:ss zzzz",
                "d MMM yyyy HH:mm:ss zzzz",
                "ddd, dd MMM yyyy HH:mm:ss zzzz",
                "ddd, d MMM yyyy HH:mm:ss zzzz"
            };

            CultureInfo provider = CultureInfo.InvariantCulture;

            DateTimeOffset dateTimeOffset = DateTimeOffset.ParseExact(dateString, formats, provider);
            DateTime dateTime = dateTimeOffset.DateTime;
            return dateTime;
        }

        private static EmailSender DecodeSender(string senderString)
        {
            var campi = senderString.Split('<');
            var displayName = string.Empty;
            var email = string.Empty;
            switch (campi.Length)
            {
                case 1:
                    email = displayName = campi[0].Replace("\"", string.Empty).Trim();
                    break;
                case 2:
                default:
                    displayName = campi[0].Replace("\"", string.Empty).Trim();
                    email = campi[1].Replace(">", string.Empty).Replace("\"", string.Empty).Trim();
                    break;
            }

            var sender = new EmailSender()
            {
                Address = email,
                DisplayName = displayName
            };
            return sender;
        }

        private static List<EmailRecipient> DecodeEmails(string valore)
        {
            var result = new List<EmailRecipient>();

            if (valore == null)
                return result;

            var indirizzi = valore.Split(',');

            foreach (var indirizzo in indirizzi)
            {
                var campi = indirizzo.Split('<');
                var displayName = campi[0].Replace("\"", string.Empty).Trim();
                var email =
                    campi.Length == 1
                        ? displayName
                        : campi[1].Replace(">", string.Empty).Replace("\"", string.Empty).Trim();

                var recipient = new EmailRecipient()
                {
                    Address = email,
                    DisplayName = displayName
                };
                result.Add(recipient);
            }

            return result;
        }

        private static string GetHeader(IList<MessagePartHeader> headers, string name)
        {
            var header = headers.FirstOrDefault(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return header?.Value!;
        }

        private static string GetMessageContent(MessagePart payload)
        {
            if (payload.Parts == null && payload.Body != null)
            {
                return DecodeBase64String(payload.Body.Data);
            }

            var messageContent = string.Empty;
            foreach (var part in payload.Parts!)
            {
                if (part.MimeType == "text/plain")
                {
                    messageContent += DecodeBase64String(part.Body.Data);
                }
                else if (part.MimeType == "text/html")
                {
                    if (string.IsNullOrEmpty(messageContent))
                        messageContent += DecodeBase64String(part.Body.Data);
                }
                else if (part.Parts != null)
                {
                    messageContent += GetMessageContent(part);
                }
            }

            return messageContent;
        }

        private static string DecodeBase64String(string base64String)
        {
            var data = System.Convert.FromBase64String(base64String.Replace("-", "+").Replace("_", "/"));
            return System.Text.Encoding.UTF8.GetString(data);
        }

        private static byte[] DecodeBase64(string base64String)
        {
            var data = System.Convert.FromBase64String(base64String.Replace("-", "+").Replace("_", "/"));
            return data;
        }

        private static async Task<List<EmailContentAttachment>> GetAttachments(GmailService service, MessagePart part)
        {
            var attachments = new List<EmailContentAttachment>();

            foreach (var subPart in part.Parts)
            {
                if (!string.IsNullOrEmpty(subPart.Filename))
                {
                    var attId = subPart.Body.AttachmentId;
                    var attachPart = await service.Users.Messages.Attachments.Get("me", part.PartId, attId).ExecuteAsync();
                    var data = DecodeBase64(attachPart.Data);
                    attachments.Add(new EmailContentAttachment
                    {
                        FileName = subPart.Filename,
                        ContentType = subPart.MimeType,
                        Content = data
                    });
                }
                else if (subPart.Parts != null)
                {
                    attachments.AddRange(await GetAttachments(service, subPart));
                }
            }

            return attachments;
        }

        private static async Task<List<EmailContentAttachment>> GetAttachments(GmailService service, Message email)
        {
            var attachments = new List<EmailContentAttachment>();

            foreach (var part in email.Payload.Parts ?? Enumerable.Empty<MessagePart>())
            {
                if (!string.IsNullOrEmpty(part.Filename))
                {
                    var attId = part.Body.AttachmentId;
                    var attachPart = await service.Users.Messages.Attachments.Get("me", email.Id, attId).ExecuteAsync();
                    var data = GmailMessageParser.DecodeBase64(attachPart.Data);
                    attachments.Add(new EmailContentAttachment
                    {
                        FileName = part.Filename,
                        ContentType = part.MimeType,
                        Content = data
                    });
                }
                else if (part.Parts != null)
                {
                    attachments.AddRange(await GetAttachments(service, part));
                }
            }

            return attachments;
        }
    }
}
