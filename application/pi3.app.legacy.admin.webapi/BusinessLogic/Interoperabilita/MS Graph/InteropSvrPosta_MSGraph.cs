// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
#if false

using Azure.Identity;
using BusinessLogic.Interoperabilità;
using DocsPaVO.amministrazione;
using log4net;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.Pkcs;
using MimeKit.Cryptography;
using MimeKit;
using System.ServiceModel.MsmqIntegration;
using System.Web.Services.Description;
using MessagePart = MimeKit.MessagePart;
using Message = Microsoft.Graph.Models.Message;
using MimePart = MimeKit.MimePart;
using MailKit;
using Microsoft.Graph.Models.CallRecords;
using Aspose.Pdf.Drawing;
using ActiveUp.Net.Mail;
using ZfLib;
using Org.BouncyCastle.Asn1.Crmf;
using System.Net;
using RestSharp;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cmp;
using static Microsoft.Graph.CoreConstants;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using Aspose.Pdf.Operators;


namespace BusinessLogic.Interoperabilita
{
    public class InteropSvrPosta_MSGraph : IMailConnector
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteropSvrPosta_MSGraph));

        private string _tenantId;
        private string _clientId;
        private string _clientSecret;
        private string _mailbox;
        private string[] _scopes;
        private string[] _uidls;
        private int _messagesCount;
        private string _folderToRead;
        private GraphServiceClient _graphClient;
        private Microsoft.Graph.Models.MailFolder _inboxFolder;

        public InteropSvrPosta_MSGraph(string tenantId, string clientId, string clientSecret, string mailbox, string folderToRead)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            this._clientId = clientId;
            this._tenantId = tenantId;
            string scope = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MS_GRAPH_SCOPE") ??
                "https://graph.microsoft.com/.default"; //Imposto anche un default per sicurezza
            this._scopes = new string[] { scope };
            this._clientSecret = clientSecret;
            this._messagesCount = 0;
            this._mailbox = mailbox;
            this._folderToRead = folderToRead;
        }

        public InteropSvrPosta_MSGraph()
        {

        }

        public bool cancellaMailImap()
        {
            logger.DebugFormat("Sto provando a fare un cancellaMailImap ma sono su MS Exchange..... Ritorno true");
            return true;
        }

        public void connect()
        {
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(
                    this._tenantId, this._clientId, this._clientSecret, options);

            this._graphClient = new GraphServiceClient(clientSecretCredential, _scopes);
        }

        public void deleteSingleMessage(int i)
        {
            throw new NotImplementedException();
        }

        public void deleteSingleMessage(string uidl)
        {
            throw new NotImplementedException();
        }

        public void disconnect()
        {
            logger.DebugFormat("Sto provando a fare un disconnect ma sono su MS Exchange..... Non serve");
        }

        public string getBodyFromMail(string email)
        {
            logger.DebugFormat("Sto provando a fare un getBodyFromMail ma non credo sia utilizzato.....");
            return "";
        }

        public CMMsg getMessage(int index)
        {
            return getMessage(this._uidls[index - 1]);
        }

        public CMMsg getMessage(byte[] email)
        {
            //Viene utilizzato a frontend per la visualizzazione delle email
            CMMsg msg = null;
            bool isReceivedSigned = false;
            bool signaturesValid = false;
            MimeMessage mimeMessage = new MimeMessage();
            var parser = new MimeParser(new MemoryStream(email), MimeFormat.Default);
            mimeMessage = parser.ParseMessage();
            /* COMMENTATO - NON SERVE FARE CONTROLLO SULLA FIRMA PERCHE' DEVO SOLO MOSTRARE IL CONTENT
            if (mimeMessage.Body is MultipartSigned)
            {
                isReceivedSigned = true;
                var signed = (MultipartSigned)mimeMessage.Body;

                using (var ctx = new WindowsSecureMimeContext())
                {
                    foreach (var signature in signed.Verify(ctx))
                    {
                        try
                        {
                            signaturesValid = signature.Verify(verifySignatureOnly: true);
                        }
                        catch (DigitalSignatureVerifyException ex)
                        {
                            logger.DebugFormat("Errore nella verifica delle firme: {0}", ex.Message);
                        }
                    }
                }

                if ((isReceivedSigned) && (!signaturesValid))
                {
                    //return null;
                    try
                    {
                        logger.ErrorFormat("Errore nella firma del file .eml: Segnatura non valida. Soggetto:{0}. Indirizzo Mittente: {1} ", mimeMessage.Subject, ((MimeKit.MailboxAddress)mimeMessage.From[0]).Address);
                    }
                    catch (Exception e) { }
                }
            }*/
            return extractMessage(mimeMessage);
            logger.DebugFormat("Sto provando a fare un getMessage con byte[] ma sono su MS Exchange.....");
            return null;
        }

        public CMMsg getMessage(string uidl)
        {
            CMMsg msg = new CMMsg();
            var messageFromClient = _graphClient.Users[this._mailbox].Messages[uidl].GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = new string[] { "*" };
            });
            var message = messageFromClient.Result;

            //Mimemessage
            MimeMessage mimeMessage = new MimeMessage();
            var messageStream = _graphClient.Users[this._mailbox].Messages[message.Id].Content.GetAsync().Result;
            byte[] messageBytes = ReadStream(messageStream);

            var parser = new MimeParser(new MemoryStream(messageBytes), MimeFormat.Default);
            mimeMessage = parser.ParseMessage();

            if (message != null)
            {
                msg = extractMessage(mimeMessage);
                msg.date = message.ReceivedDateTime.Value.UtcDateTime.ToLocalTime();
            }

            return msg;
        }

        private CMMsg extractMessage(MimeMessage mimeMessage)
        {
            CMMsg msg = new CMMsg();
            DateTime ReceiveDate = DateTime.MinValue;

            IEnumerable<MimeEntity> mimeAttachments = mimeMessage.Attachments.ToList();

            //Controllo se PEC
            var attachedMessage = mimeMessage.BodyParts?.ToList().Where(p => p is MessagePart && p.ContentType.Name.ToLower().Equals("postacert.eml")).FirstOrDefault();
            bool isPec = attachedMessage != null;


            //Sbustamento SI/NO
            //string DISABLE_SBUSTA_MAIL_INOLTRATA = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_DISABLE_SBUSTA_MAIL_INOLTRATA");
            //bool disableSbusta = !string.IsNullOrEmpty(DISABLE_SBUSTA_MAIL_INOLTRATA) && DISABLE_SBUSTA_MAIL_INOLTRATA == "true";
            //if (!string.IsNullOrEmpty(DISABLE_SBUSTA_MAIL_INOLTRATA) && DISABLE_SBUSTA_MAIL_INOLTRATA == "true" && !isPec)
            //{
            msg = extractMail(mimeMessage, true);
            //}
            //else
            //{
            //    msg = extractMail(mimeMessage);
            //}

            if (msg != null)
            {
                var dateTimeHeader = msg.headers["X-MS-Exchange-CrossTenant-OriginalArrivalTime"];
                if (dateTimeHeader != null && !DateTime.TryParse(dateTimeHeader.ToString().Replace("(UTC)", ""), out ReceiveDate))
                {
                    logger.Debug("Errore nel formato della data di spedizione delle mail");
                }
                if (ReceiveDate != DateTime.MinValue)
                    msg.date = ReceiveDate.ToLocalTime();
                if (msg.isPECDelivered() ||
                    (msg.isFromNonPEC() &&
                    (attachedMessage != null)))
                {
                    /*
                    if (!string.IsNullOrEmpty(DISABLE_SBUSTA_MAIL_INOLTRATA) && DISABLE_SBUSTA_MAIL_INOLTRATA == "true")
                    {
                        // ************************************
                        // La mail inoltrata non viene sbustata
                        // ************************************
                        if (msg.isPECDelivered() || msg.isFromNonPEC())
                        {
                            if (mimeMessage != null)
                            {
                                foreach (var bodyPart in mimeMessage.BodyParts.ToList())
                                {
                                    if (bodyPart is MessagePart)
                                    {
                                        MessagePart messagePart = ((MessagePart)bodyPart);
                                        if (bodyPart.ContentType != null &&
                                            bodyPart.ContentType.Name != null &&
                                            bodyPart.ContentType.Name.Equals("postacert.eml"))
                                        {
                                            using (MemoryStream memoryStream = new MemoryStream())
                                            {
                                                //svuoto gli allegati
                                                //msg.attachments.Clear();
                                                messagePart.Message.WriteTo(memoryStream);
                                                //byte[] contentBytes = memoryStream.ToArray();
                                                memoryStream.Position = 0;
                                                var parserMessagePart = new MimeParser(memoryStream, MimeFormat.Default);
                                                mimeMessage = parserMessagePart.ParseMessage();
                                            }
                                        }
                                    }
                                }

                                msg = extractMail(mimeMessage, true);
                                bool mailInoltrataAcquisita = false;
                                bool hasAttachedMessage = mimeMessage.BodyParts?.ToList().Where(p => p is MessagePart).Count() > 0;

                                // Se la mail è inoltrata viene acquisita come allegato
                                if (hasAttachedMessage)
                                {
                                    // Creazione mail inoltrata come allegato
                                    string nomeEmail = mimeMessage.Subject + ".eml";
                                    byte[] emailByteArray = null;
                                    string contentType = "message/rfc822";
                                    using (MemoryStream memoryStream = new MemoryStream())
                                    {
                                        mimeMessage.WriteTo(memoryStream);
                                        emailByteArray = memoryStream.ToArray();
                                    }
                                    CMAttachment att = new CMAttachment(nomeEmail, contentType, emailByteArray);
                                    msg.attachments.Add(att);
                                    mailInoltrataAcquisita = true;
                                }


                                if (mimeMessage.Attachments.ToList().Count() != 0 && !mailInoltrataAcquisita)
                                {
                                    //se invece la mail allegata sta negli attachments allora la allego
                                    foreach (var a in mimeMessage.Attachments.ToList())
                                    {
                                        string nomeAllegato = a.ContentDisposition?.FileName;
                                        if (a is MessagePart)//if (nomeAllegato.Contains(".eml"))
                                        {
                                            string nomeEmail = mimeMessage.Subject + ".eml";
                                            byte[] emailByteArray = null;
                                            string contentType = "message/rfc822";
                                            using (MemoryStream memoryStream = new MemoryStream())
                                            {
                                                mimeMessage.WriteTo(memoryStream);
                                                emailByteArray = memoryStream.ToArray();
                                            }

                                            CMAttachment att = new CMAttachment(nomeEmail, contentType, emailByteArray);
                                            msg.attachments.Add(att);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var listAttachedMessages = mimeMessage.BodyParts.ToList().Where(p => p is MessagePart).ToList();
                            bool hasAttachedMessage = listAttachedMessages.Count() > 0;

                            if (hasAttachedMessage)
                            {
                                // Creazione mail inoltrata come allegato
                                //string nomeEmail = System.Guid.NewGuid().ToString().Substring(0, 25) + ".eml";
                                string nomeEmail = mimeMessage.Subject + ".eml";
                                byte[] emailByteArray = null;
                                string contentType = "message/rfc822";
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    mimeMessage.WriteTo(memoryStream);
                                    emailByteArray = memoryStream.ToArray();
                                }
                                CMAttachment att = new CMAttachment(nomeEmail, contentType, emailByteArray);
                                msg.attachments.Add(att);
                            }
                        }

                        if (ReceiveDate != DateTime.MinValue)
                            msg.date = ReceiveDate.ToLocalTime();

                        // Verifico se il messaggio contiene un message/delivery-status
                        bool dsnRootLeaf = isDSN(mimeMessage);
                        if (dsnRootLeaf)
                        {
                            CMMsg msgNew = extractDSNInfo(mimeMessage);
                            foreach (string h in msgNew.headers.Keys)
                                msg.headers.Add(h, msgNew.getHeader(h));
                            msg.subject = msgNew.subject;
                            msg.HTMLBody = msgNew.body;
                            msg.body = msgNew.body;
                            msg.recipients = msgNew.recipients;
                        }
                    }
                    else
                    {*/
                    mimeAttachments = mimeMessage.Attachments;
                    foreach (var bodyPart in mimeMessage.BodyParts.ToList())
                    {
                        if (bodyPart is MessagePart)
                        {
                            MessagePart messagePart = ((MessagePart)bodyPart);
                            if (bodyPart.ContentType != null &&
                                bodyPart.ContentType.Name != null &&
                                bodyPart.ContentType.Name.Equals("postacert.eml"))
                            {
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    //svuoto gli allegati
                                    msg.attachments.Clear();
                                    messagePart.Message.WriteTo(memoryStream);
                                    //byte[] contentBytes = memoryStream.ToArray();
                                    memoryStream.Position = 0;
                                    var parserMessagePart = new MimeParser(memoryStream, MimeFormat.Default);
                                    mimeMessage = parserMessagePart.ParseMessage();
                                }

                            }
                        }
                    }
                    msg = extractMail(mimeMessage);
                    if (ReceiveDate != DateTime.MinValue)
                        msg.date = ReceiveDate.ToLocalTime();

                    bool dsnRootLeaf = isDSN(mimeMessage);
                    if (dsnRootLeaf)
                    {
                        CMMsg msgNew = extractDSNInfo(mimeMessage);
                        foreach (string h in msgNew.headers.Keys)
                            msg.headers.Add(h, msgNew.getHeader(h));

                        msg.subject = msgNew.subject;
                        msg.HTMLBody = msgNew.body;
                        msg.body = msgNew.body;
                        msg.recipients = msgNew.recipients;
                    }
                    /*}*/
                }
            }

            return msg;
        }

        private CMMsg extractDSNInfo(MimeMessage message)
        {
            CMMsg retval = new CMMsg();
            var report = message.Body as MultipartReport;

            retval.body = string.Format("<PRE> {0} </PRE>", message.GetTextBody(MimeKit.Text.TextFormat.Plain));

            if (report != null && report.ReportType != null && report.ReportType.Equals("delivery-status", StringComparison.OrdinalIgnoreCase))
            {
                //string msgDstat = report.;
                //string dsnErrorCode = CMMsg.findSmptErrorCode(msgDstat);
                retval.headers.Add("IsDSN", "TRUE");
                retval.headers.Add("Status", "550");
                retval.headers.Add("Diagnostic", "Generic smpt error");

                string failedRecipients = string.Empty;
                // process the report
                foreach (var mds in report.OfType<MessageDeliveryStatus>())
                {
                    // process the status groups - each status group represents a different recipient

                    // The first status group contains information about the message
                    var envelopeId = mds.StatusGroups[0]["Original-Envelope-Id"];

                    // all of the other status groups contain per-recipient information
                    for (int i = 1; i < mds.StatusGroups.Count(); i++)
                    {
                        var recipient = mds.StatusGroups[i]["Original-Recipient"];
                        var action = mds.StatusGroups[i]["Action"];
                        var status = mds.StatusGroups[i]["Status"];
                        var diagnosticCode = mds.StatusGroups[i]["Diagnostic-Code"];

                        if (recipient == null)
                            recipient = mds.StatusGroups[i]["Final-Recipient"];

                        // the recipient string should be in the form: "rfc822;user@domain.com"
                        var indexRecipientString = recipient.IndexOf(';');
                        var address = recipient.Substring(indexRecipientString + 1);

                        //string[] recipientsArr = recipient?.Split(';');
                        //failedRecipients += recipientsArr[1] + "§";

                        // the Diagnostic - code string should be in the form: "smtp; 550 user unknown"
                        var indexDiagnosticCode = diagnosticCode.IndexOf(';');
                        var code = recipient.Substring(indexDiagnosticCode + 1);

                        failedRecipients += address + "§" + status + "§" + code + ";";

                        failedRecipients = failedRecipients.Replace("<", string.Empty).Replace(">", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

                    }
                    retval.headers.Add("FailedRecepients", failedRecipients);

                    //message/rfc822
                    MimeEntity envelopeMessage = message.BodyParts.FirstOrDefault(x =>
                        (x.ContentType.MimeType == "message/rfc822" || x.ContentType.MimeType == "text/rfc822-headers"));

                    if (envelopeMessage != null && envelopeMessage is MessagePart)
                    {
                        var rfc822 = envelopeMessage as MessagePart;
                        if (rfc822 != null)
                        {
                            retval.subject = rfc822.Headers["subject"];
                            retval.from = rfc822.Headers["from"];
                            string toField = rfc822.Headers["to"];
                            retval.recipients.Add(new CMRecipient { mail = toField, name = toField });
                        }
                    }
                }
            }

            return retval;
        }

        private bool isDSN(MimeMessage mimeMessage)
        {
            //These messages have a top-level MIME-type of multipart/report with a report-type value of delivery-status.
            //Content-Type: multipart/report; report-type=delivery-status;

            //Once you parse the message with MimeMessage.Load(), you can check if the Body is a MultipartReport with the expected ReportType property value.
            if (mimeMessage.Body is MultipartReport)
            {
                //From there, you can locate the child part that is of type MessageDeliveryStatus.
                var report = mimeMessage.Body as MultipartReport;
                return report != null && report.ReportType != null & report.ReportType.ToLower().Equals("delivery-status");
            }
            else
            {
                return false;
            }
        }

        private CMMsg extractMail(MimeMessage mimeMessage, bool inoltrata = false)
        {
            CMMsg msg = new CMMsg();
            string sTemp;

            sTemp = ExtractEmail(mimeMessage, msg);


            //se la mail è una ricevuta di ritorno non eseguo il forward di una mail
            //Attachments Message - Forward di una email alla casella istituzionale 
            if (!(msg.isPECDelivered() || msg.isFromNonPEC()) &&
                !BusinessLogic.interoperabilita.InteroperabilitaManager.isRicevutaPec(msg))
            {
                if (inoltrata)
                {
                    // ************************************
                    // La mail inoltrata non viene sbustata
                    // ************************************
                    bool forwardKeepExternalAttach = !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FORWARD_KEEP_EXTERNAL_ATTACH")) &&
                           !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FORWARD_KEEP_EXTERNAL_ATTACH").ToString().Equals("0");
                    //if (!forwardKeepExternalAttach)
                    //{
                    //	msg.attachments.Clear();
                    //}
                    //else //mantengo anche gli allegati esterni e rimuovo solo il daticert.xml
                    //{
                    foreach (CMAttachment attach in msg.attachments)
                    {
                        if (attach.name.ToLower() == "daticert.xml")
                        {
                            msg.attachments.Remove(attach);
                            break;
                        }
                    }
                    //}
                    //if (mimeMessage != null) //aggiungo gli allegati .eml
                    //{
                    //    foreach (var bodyPart in mimeMessage.BodyParts.ToList())
                    //    {
                    //        if (bodyPart is MessagePart)
                    //        {
                    //            MessagePart messagePart = ((MessagePart)bodyPart);
                    //            if (bodyPart.ContentType != null &&
                    //                bodyPart.ContentType.Name != null)
                    //            {
                    //                using (MemoryStream memoryStream = new MemoryStream())
                    //                {
                    //                    messagePart.Message.WriteTo(memoryStream);
                    //                    byte[] contentBytes = memoryStream.ToArray();
                    //                    msg.attachments.Add(new CMAttachment(bodyPart.ContentType.Name,
                    //                        messagePart.ContentType.MimeType, contentBytes));
                    //                }
                    //            }
                    //        }

                    //    }
                    //}
                }
                else
                {
                    if (mimeMessage != null)
                    {
                        //bool forwardKeepExternalAttach = !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FORWARD_KEEP_EXTERNAL_ATTACH")) &&
                        //			!DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_FORWARD_KEEP_EXTERNAL_ATTACH").ToString().Equals("0");
                        MimeMessage mailAtt = new MimeMessage();
                        foreach (var bodyPart in mimeMessage.BodyParts.ToList())
                        {
                            if (bodyPart is MessagePart)
                            {
                                MessagePart messagePart = ((MessagePart)bodyPart);
                                if (bodyPart.ContentType != null &&
                                    bodyPart.ContentType.Name != null &&
                                    bodyPart.ContentType.Name.Equals("postacert.eml"))
                                {
                                    using (MemoryStream memoryStream = new MemoryStream())
                                    {
                                        messagePart.Message.WriteTo(memoryStream);
                                        memoryStream.Position = 0;
                                        var parserMessagePart = new MimeParser(memoryStream, MimeFormat.Default);
                                        mailAtt = parserMessagePart.ParseMessage();
                                    }

                                }
                            }
                            //if (bodyPart is MimePart)
                            //{
                            //	MimePart mimePart = ((MimePart)bodyPart);
                            //	if (mimePart.FileName != null &&
                            //		mimePart.FileName.Equals("daticert.xml"))
                            //	{
                            //		using (MemoryStream memoryStream = new MemoryStream())
                            //		{
                            //			mimePart.Content.WriteTo(memoryStream);
                            //			byte[] contentBytes = memoryStream.ToArray();
                            //			msg.attachments.Add(new CMAttachment(mimePart.FileName,
                            //				mimePart.ContentType.MimeType, contentBytes));
                            //		}
                            //	}
                            //}
                        }

                        //svuoto gli allegati
                        //msg.attachments.Clear();

                        foreach (var att in mailAtt.Attachments.ToList())
                        {
                            using (MemoryStream memoryStreamAtt = new MemoryStream())
                            {
                                if (att is MimePart)
                                    ((MimePart)att).Content.DecodeTo(memoryStreamAtt);
                                else
                                    ((MessagePart)att).Message.WriteTo(memoryStreamAtt);

                                byte[] contentBytes = memoryStreamAtt.ToArray();
                                msg.attachments.Add(new CMAttachment(att.ContentType.Name,
                                    att.ContentType.MimeType, contentBytes));
                            }
                        }
                        msg.headers.Add("utenteDocspa", msg.from);
                        msg.from = ((MimeKit.MailboxAddress)mimeMessage.From[0]).Address;
                        msg.subject = mimeMessage.Subject;
                        msg.body = mimeMessage.TextBody;
                        msg.HTMLBody = mimeMessage.HtmlBody;
                    }
                }
            }

            foreach (var r in mimeMessage.GetRecipients())
            {
                msg.recipients.Add(new CMRecipient
                {
                    mail = r.Address,
                    name = r.Name
                });
            }

            return msg;
        }

        private string ExtractEmail(MimeMessage mimeMessage, CMMsg msg)
        {
            string retval = null;
            msg.body = mimeMessage.TextBody;
            msg.HTMLBody = mimeMessage.HtmlBody;
            msg.subject = mimeMessage.Subject;
            msg.date = mimeMessage.Date.DateTime;
            msg.from = ((MimeKit.MailboxAddress)mimeMessage.From[0]).Address;

            foreach (var h in mimeMessage.Headers)
            {
                if (msg.headers.ContainsKey(h.Field))
                {
                    msg.headers.Remove(h.Field);
                }
                msg.headers.Add(h.Field, h.Value);
            }

            // Attachments
            foreach (var a in mimeMessage.Attachments.ToList())
            {
                using (MemoryStream memoryStreamAtt = new MemoryStream())
                {
                    if (a is MimePart)
                        ((MimePart)a).Content.DecodeTo(memoryStreamAtt);
                    else
                        ((MessagePart)a).Message.WriteTo(memoryStreamAtt);

                    byte[] contentBytes = memoryStreamAtt.ToArray();
                    msg.attachments.Add(new CMAttachment(a.ContentType.Name,
                        a.ContentType.MimeType, contentBytes));
                }
            }

            bool isPec = mimeMessage.BodyParts.ToList().Where(p => p is MessagePart && !string.IsNullOrEmpty(p.ContentType.Name) && p.ContentType.Name.ToLower().Equals("postacert.eml")).FirstOrDefault() != null;

            if (isPec) //Aggiungo daticert.xml e postacert.eml agli allegati del msg 
            {
                foreach (var bodyPart in mimeMessage.BodyParts.ToList())
                {
                    if (bodyPart is MessagePart)
                    {
                        MessagePart messagePart = ((MessagePart)bodyPart);
                        if (bodyPart.ContentType != null &&
                            bodyPart.ContentType.Name != null &&
                            bodyPart.ContentType.Name.Equals("postacert.eml"))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                messagePart.Message.WriteTo(memoryStream);
                                byte[] contentBytes = memoryStream.ToArray();
                                msg.attachments.Add(new CMAttachment(bodyPart.ContentType.Name,
                                    messagePart.ContentType.MimeType, contentBytes));
                            }
                        }
                    }
                    if (bodyPart is MimePart)
                    {
                        MimePart mimePart = ((MimePart)bodyPart);
                        if (mimePart.FileName != null &&
                            mimePart.FileName.Equals("daticert.xml"))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                mimePart.Content.WriteTo(memoryStream);
                                byte[] contentBytes = memoryStream.ToArray();
                                msg.attachments.Add(new CMAttachment(mimePart.FileName,
                                    mimePart.ContentType.MimeType, contentBytes));
                            }
                        }
                    }
                }
            }

            return retval;
        }

        public bool getMessagePec(int index)
        {
            return getMessagePec(this._uidls[index - 1]);
        }

        public bool getMessagePec(string uidl)
        {
            bool retval = false;

            try
            {
                //var messageFromClient = _graphClient.Users[this._mailbox].Messages[uidl].GetAsync((requestConfiguration) =>
                //{
                //	requestConfiguration.QueryParameters.Select = new string[] { "*" };
                //});

                //var message = messageFromClient.Result;

                MimeMessage mimeMessage = new MimeMessage();
                var messageStream = _graphClient.Users[this._mailbox].Messages[uidl].Content.GetAsync().Result;

                byte[] messageBytes = ReadStream(messageStream);

                var parser = new MimeParser(new MemoryStream(messageBytes), MimeFormat.Default);
                mimeMessage = parser.ParseMessage();
                //Controllo se c'è una firma digitale (smime.p7m)
                bool isReceivedSigned = false;
                //Verifica sulle firme
                bool signaturesValid = false;
                if (mimeMessage.Body is MultipartSigned)
                {
                    isReceivedSigned = true;
                    var signed = (MultipartSigned)mimeMessage.Body;

                    using (var ctx = new WindowsSecureMimeContext())
                    {
                        foreach (var signature in signed.Verify(ctx))
                        //foreach (var signature in signed.Verify())
                        {
                            try
                            {
                                signaturesValid = signature.Verify(verifySignatureOnly: true);
                            }
                            catch (DigitalSignatureVerifyException ex)
                            {
                                logger.DebugFormat("Errore nella verifica delle firme: {0}", ex.Message);
                            }
                        }
                    }
                }

                if (isReceivedSigned && !signaturesValid)
                    return false;

                retval = extractMailPec(mimeMessage);



            }
            catch (Exception exc)
            {
                logger.Debug(String.Format("Errore nel recupero del tipo della mail da [{0}]. {1}", this._mailbox, exc.Message));
            }

            return retval;
        }

        private bool extractMailPec(MimeMessage mimeMessage)
        {
            if (mimeMessage.Headers["X-Trasporto"] != null)
                return (mimeMessage.Headers["X-Trasporto"].ToString().ToLower() == "posta-certificata");
            else
                return false;
        }

        public string[] getUidls()
        {
            //Al posto degli UIDL, ritorno la lista di ID di Exchange
            Task<MessageCollectionResponse> messages = null;
            if (!string.IsNullOrWhiteSpace(this._folderToRead))
            {
                var folder = _graphClient.Users[this._mailbox].MailFolders.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Filter = String.Format("displayName eq '{0}'", this._folderToRead);
                }).Result.Value.FirstOrDefault();
                if (folder != null)
                {
                    messages = _graphClient.Users[this._mailbox].MailFolders[folder.Id].Messages.GetAsync((requestConfiguration) =>
                    {
                        requestConfiguration.QueryParameters.Select = new string[] { "id", "isRead" };
                        //requestConfiguration.QueryParameters.Filter = "isRead eq false";
                        requestConfiguration.QueryParameters.Top = 1000;//Altrimenti li pagina a 10 alla volta
                    });
                }
                else
                    logger.ErrorFormat("Cartella {0} non trovata", this._folderToRead);
            }
            else
                messages = _graphClient.Users[this._mailbox].MailFolders["Inbox"].Messages.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Select = new string[] { "id", "isRead" };
                    //requestConfiguration.QueryParameters.Filter = "isRead eq false";
                    requestConfiguration.QueryParameters.Top = 1000;//Altrimenti li pagina a 10 alla volta
                });
            var messagesArray = new string[this._messagesCount];
            for (int i = 0; i < this._messagesCount; i++)
            {
                messagesArray[i] = messages.Result.Value.ElementAt(i).Id;
            }
            this._uidls = messagesArray;
            return messagesArray;
        }

        public int messageCount()
        {
            Task<MessageCollectionResponse> messages = null;
            if (!string.IsNullOrWhiteSpace(this._folderToRead))
            {
                var folder = _graphClient.Users[this._mailbox].MailFolders.GetAsync((requestConfiguration) =>
                {
                    requestConfiguration.QueryParameters.Filter = String.Format("displayName eq '{0}'", this._folderToRead);
                }).Result.Value.FirstOrDefault();
                if (folder != null)
                {
                    this._messagesCount = _graphClient.Users[this._mailbox].MailFolders[folder.Id].Messages.GetAsync((requestConfiguration) =>
                    {
                        //requestConfiguration.QueryParameters.Filter = "isRead eq false";
                        requestConfiguration.QueryParameters.Top = 1000;//Altrimenti li pagina a 10 alla volta
                    }).Result.Value.Count;
                }
                else
                    logger.ErrorFormat("Cartella {0} non trovata", this._folderToRead);
            }
            else
                this._messagesCount = _graphClient.Users[this._mailbox].MailFolders["Inbox"].Messages.GetAsync((requestConfiguration) =>
                {
                    //requestConfiguration.QueryParameters.Filter = "isRead eq false";
                    requestConfiguration.QueryParameters.Top = 1000;//Altrimenti li pagina a 10 alla volta
                }).Result.Value.Count;

            //if (messagesCount.Result.OdataCount != null && messagesCount.Result.OdataCount > 0)
            //{
            //	this._messagesCount = Convert.ToInt32(messagesCount.Result.OdataCount);
            //}
            return this._messagesCount;
        }

        public bool moveImap(int index, bool elaborata)
        {
            Message resultMove = null;

            #region Search folder 
            string folder = elaborata ? "mailelaborate" : "mailnonelaborate";
            string folderId = string.Empty;

            var folderToMove = _graphClient.Users[this._mailbox].MailFolders.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = String.Format("displayName eq '{0}'", folder);
            }).Result.Value.FirstOrDefault();

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

            #region Move 
            resultMove = _graphClient.Users[this._mailbox].Messages[this._uidls[index - 1].ToString()].Move
                .PostAsync(new Microsoft.Graph.Users.Item.Messages.Item.Move.MovePostRequestBody
                {
                    DestinationId = folderId,
                }).Result;
            #endregion

            return resultMove != null;
        }

        public bool moveImap(string uid, bool elaborata)
        {
            Message resultMove = null;

            #region Search folder 
            string folder = elaborata ? "mailelaborate" : "mailnonelaborate";
            string folderId = string.Empty;

            var folderToMove = _graphClient.Users[this._mailbox].MailFolders.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = String.Format("displayName eq '{0}'", folder);
            }).Result.Value.FirstOrDefault();

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

            #region Move 
            resultMove = _graphClient.Users[this._mailbox].Messages[uid].Move
                .PostAsync(new Microsoft.Graph.Users.Item.Messages.Item.Move.MovePostRequestBody
                {
                    DestinationId = folderId,
                }).Result;
            #endregion

            return resultMove != null;
        }

        public bool provaConnessione(OrgRegistro.MailRegistro mailRegistro, out string errore, string tipoConnessione)
        {
            errore = string.Empty;

            if (String.IsNullOrEmpty(mailRegistro.TenantId))
            {
                errore += "Tenant id mancante ";
                return false;
            }
            if (String.IsNullOrEmpty(mailRegistro.ClientId))
            {
                errore += "Client id mancante ";
                return false;
            }
            if (String.IsNullOrEmpty(mailRegistro.ClientSecret))
            {
                errore += "Client secret mancante ";
                return false;
            }

            #region Costruzione messaggio 
            MSSendMessageRequest requestBody = new MSSendMessageRequest
            {
                Message = new MSMessage
                {
                    Subject = "TEST CONNESSIONE",
                    Body = new MSItemBody
                    {
                        ContentType = BodyType.Text.ToString(),
                        Content = "TEST CONNESSIONE"
                    },
                    From = new MSRecipient
                    {
                        EmailAddress = new MSEmailAddress { Address = mailRegistro.Email }
                    },
                    ToRecipients = new List<MSRecipient>
                    {
                        new MSRecipient
                        {
                            EmailAddress = new MSEmailAddress { Address = mailRegistro.Email }
                        }
                    }
                },
            };
            #endregion

            try
            {

                //Uso RestSharp
                if (requestBody != null)
                {
                    #region Autenticazione
                    RestClient authClient = null;
                    RestRequest authRequest = null;
                    string token = string.Empty;

                    string authUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MS_GRAPH_LOGIN_URL") ?? "https://login.microsoftonline.com/"; //imposto un default per sicurezza
                    authClient = new RestClient(authUrl);
                    string authSendUrl = "/{tenant}/oauth2/v2.0/token";

                    authRequest = new RestRequest(authSendUrl, Method.Post);
                    authRequest.AddUrlSegment("tenant", mailRegistro.TenantId);
                    authRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    authRequest.AddParameter("scope", DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MS_GRAPH_SCOPE") ??
                "https://graph.microsoft.com/.default");
                    authRequest.AddParameter("client_secret", mailRegistro.ClientSecret);
                    authRequest.AddParameter("grant_type", "client_credentials");
                    authRequest.AddParameter("client_id", mailRegistro.ClientId);

                    var authResponse = authClient.Execute(authRequest);

                    if (authResponse.StatusCode == HttpStatusCode.OK && authResponse.IsSuccessful)
                    {
                        var responseToken = System.Text.Json.JsonSerializer.Deserialize<MSGetTokenResponse>(authResponse.Content);
                        token = System.Text.Json.JsonSerializer.Deserialize<MSGetTokenResponse>(authResponse.Content).access_token;
                        if (responseToken.access_token != null)
                        {
                            token = responseToken.access_token;
                        }
                        else
                        {
                            errore = "Token nullo";
                            return false;
                        }
                    }
                    else
                    {
                        var authContent = !string.IsNullOrEmpty(authResponse.Content) ? JObject.Parse(authResponse.Content) : null;
                        if (authContent != null)
                        {
                            string errorCode = (authContent["error"])["code"].ToString();
                            string errorMsg = (authContent["error"])["message"].ToString();
                            logger.DebugFormat("Response status {0}, errore {1} - {2}", authResponse.StatusCode, errorCode, errorMsg);
                            errore = errorMsg;
                            //throw new Exception(errorMsg);
                        }
                        return false;
                    }
                    #endregion

                    #region Invio messaggio 
                    RestClient client = null;
                    RestRequest request = null;

                    string graphServicesUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MS_GRAPH_URL") ?? "https://graph.microsoft.com/v1.0";
                    client = new RestClient(graphServicesUrl);
                    string sendUrl = "users/{mail}/sendMail";
                    request = new RestRequest(sendUrl, Method.Post);
                    request.AddUrlSegment("mail", mailRegistro.Email);
                    request.AddHeader("Authorization", $"Bearer {token}");

                    JsonSerializerSettings config = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    string jsonString = JsonConvert.SerializeObject(requestBody, Formatting.Indented, config);
                    //string jsonString = System.Text.Json.JsonSerializer.Serialize(requestBody);
                    request.AddJsonBody(jsonString);
                    //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

                    var response = client.Execute(request);
                    if (response != null)
                    {
                        var content = !string.IsNullOrEmpty(response.Content) ? JObject.Parse(response.Content) : null;

                        if (response.StatusCode == HttpStatusCode.Accepted)
                        {
                            logger.DebugFormat("Response status {0}", response.StatusCode);
                            return true;
                        }
                        else
                        {
                            if (content != null)
                            {
                                string errorCode = (content["error"])["code"].ToString();
                                string errorMsg = (content["error"])["message"].ToString();
                                logger.DebugFormat("Response status {0}, errore {1} - {2}", response.StatusCode, errorCode, errorMsg);
                                errore = errorMsg;
                            }
                            return false;
                        }
                    }
                    else
                    {
                        errore = "Response in SendMessage null";
                        return false;
                    }
                    #endregion
                }

            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Errore nel test di connessione {0}.", exc.Message));
            }

            return true;
        }

        public bool salvaMailInLocale(int indexMail, string pathFile, string NomeDellaMail)
        {
            return salvaMailInLocale(this._uidls[indexMail - 1], pathFile, NomeDellaMail);
        }

        public bool salvaMailInLocale(string uidl, string pathFile, string NomeDellaMail)
        {
            bool retval = false;
            logger.DebugFormat("Sto usando salvaMailInLocale con UIDL, path {0}, NomeDellaMail {1}, UIDL {2}", pathFile, NomeDellaMail, uidl);
            try
            {
                MimeMessage mimeMessage = new MimeMessage();
                var messageStream = _graphClient.Users[this._mailbox].Messages[uidl].Content.GetAsync().Result;
                byte[] messageBytes = ReadStream(messageStream);

                //var parser = new MimeParser(new MemoryStream(messageBytes), MimeFormat.Default);
                //mimeMessage = parser.ParseMessage();

                System.IO.File.WriteAllBytes(@pathFile + "\\" + NomeDellaMail, messageBytes);

                retval = true;

            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore durante il salvataggio della mail: {0}, {1}", ex.Message);

            }
            return retval;

        }

        //private void SaveMailBlobStorage(string mailPath, byte[] eml)
        //{
        //    string path = string.Format(@"{0}{1}",
        //                    System.Configuration.ConfigurationManager.AppSettings["DOC_ROOT"],
        //                    mailPath);
        //    var containerName = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_STORAGE_CONTAINER_NAME");
        //    string storageConnectionString = "DefaultEndpointsProtocol=https;"
        //                                    + "AccountName=" + DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_STORAGE_ACCOUNT_NAME")
        //                                    + ";AccountKey=" + DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_STORAGE_ACCOUNT_KEY")
        //                                    + ";EndpointSuffix=core.windows.net";
        //    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
        //    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

        //    CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference(containerName);
        //    CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(path);

        //    using (var stream = new MemoryStream(eml))
        //    {
        //        blockBlob.UploadFromStreamAsync(stream).Wait();
        //    }
        //}

        public void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, CMMailHeaders[] headers, out string outError)
        {
            outError = string.Empty;
            #region Headers
            List<MSInternetMessageHeader> msHeaders = null;
            if (headers != null && headers.Count() > 0)
            {
                msHeaders = new List<MSInternetMessageHeader>();
                foreach (CMMailHeaders h in headers)
                {
                    msHeaders.Add(new MSInternetMessageHeader
                    {
                        Name = h.header,
                        Value = h.value
                    });
                }
            }
            #endregion

            #region Allegati
            List<MSAttachment> msAttachments = null;
            if (attachments != null && attachments.Count() > 0)
            {
                msAttachments = new List<MSAttachment>();
                foreach (CMAttachment att in attachments)
                {
                    msAttachments.Add(new MSAttachment
                    {
                        OdataType = "#microsoft.graph.fileAttachment",
                        Name = att.name,
                        ContentType = att.contentType,
                        ContentBytes = Convert.ToBase64String(att._data)
                    });
                }
            }
            #endregion

            MSSendMessageRequest requestBody = null;

            try
            {
                #region Costruzione messaggio 
                requestBody = new MSSendMessageRequest
                {
                    Message = new MSMessage
                    {
                        Subject = sSubject,
                        Body = new MSItemBody
                        {
                            ContentType = format == CMMailFormat.HTML ? BodyType.Html.ToString() : BodyType.Text.ToString(),
                            Content = sBody
                        },
                        From = new MSRecipient
                        {
                            EmailAddress = new MSEmailAddress { Address = sFrom }
                        },
                        ToRecipients = new List<MSRecipient>
                        {
                            new MSRecipient
                            {
                                EmailAddress = new MSEmailAddress { Address = sTo }
                            }
                        },
                        CcRecipients = new List<MSRecipient>(),
                        BccRecipients = new List<MSRecipient>(),
                        InternetMessageHeaders = msHeaders,
                        Attachments = msAttachments,
                    },
                };
                #endregion
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Errore nella creazione del messaggio. {0}", exc.Message));
            }

            try
            {
                //commentata la chiamata a GraphClient perché non restituisce codici di risposta. 
                //_graphClient.Users[this._mailbox].SendMail.PostAsync(requestBody).Wait();

                //Uso RestSharp
                if (requestBody != null)
                {
                    #region Autenticazione
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    RestClient authClient = null;
                    RestRequest authRequest = null;
                    string token = string.Empty;

                    string authUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MS_GRAPH_LOGIN_URL") ?? "https://login.microsoftonline.com/"; //imposto un default per sicurezza
                    authClient = new RestClient(authUrl);
                    string authSendUrl = "/{tenant}/oauth2/v2.0/token";

                    authRequest = new RestRequest(authSendUrl, Method.Post);
                    authRequest.AddUrlSegment("tenant", this._tenantId);
                    authRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    authRequest.AddParameter("scope", this._scopes[0]);
                    authRequest.AddParameter("client_secret", this._clientSecret);
                    authRequest.AddParameter("grant_type", "client_credentials");
                    authRequest.AddParameter("client_id", this._clientId);

                    var authResponse = authClient.Execute(authRequest);

                    if (authResponse.StatusCode == HttpStatusCode.OK && authResponse.IsSuccessful)
                    {
                        var responseToken = System.Text.Json.JsonSerializer.Deserialize<MSGetTokenResponse>(authResponse.Content);
                        token = System.Text.Json.JsonSerializer.Deserialize<MSGetTokenResponse>(authResponse.Content).access_token;
                        if (responseToken.access_token != null)
                        {
                            token = responseToken.access_token;
                        }
                    }
                    #endregion

                    #region Invio messaggio 
                    RestClient client = null;
                    RestRequest request = null;

                    string graphServicesUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MS_GRAPH_URL") ?? "https://graph.microsoft.com/v1.0";
                    client = new RestClient(graphServicesUrl);
                    string sendUrl = "users/{mail}/sendMail";
                    request = new RestRequest(sendUrl, Method.Post);
                    request.AddUrlSegment("mail", this._mailbox);
                    request.AddHeader("Authorization", $"Bearer {token}");

                    JsonSerializerSettings config = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    string jsonString = JsonConvert.SerializeObject(requestBody, Formatting.Indented, config);
                    //string jsonString = System.Text.Json.JsonSerializer.Serialize(requestBody);
                    request.AddJsonBody(jsonString);
                    //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

                    var response = client.Execute(request);
                    if (response != null)
                    {
                        var content = !string.IsNullOrEmpty(response.Content) ? JObject.Parse(response.Content) : null;

                        if (response.StatusCode == HttpStatusCode.Accepted)
                        {
                            logger.DebugFormat("Response status {0}", response.StatusCode);
                        }
                        else
                        {
                            if (content != null)
                            {
                                string errorCode = (content["error"])["code"].ToString();
                                string errorMsg = (content["error"])["message"].ToString();
                                logger.DebugFormat("Response status {0}, errore {1} - {2}", response.StatusCode, errorCode, errorMsg);
                                outError = errorMsg;
                                throw new Exception(errorMsg);
                            }

                        }
                    }
                    #endregion
                }

            }
            catch (Exception exc)
            {
                if (outError != "Impossibile contattare il server SMTP")
                    outError = "Errore generico";
                throw new Exception(String.Format("Errore nell'invio del messaggio {0}.", exc.Message));
            }
        }

        public void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, out string outError)
        {
            sendMail(sFrom, sTo, sCC, sBCC, sSubject, sBody, format, attachments, null, out outError);
        }

        public void sendMail(string sFrom, string sTo, string sSubject, string sBody, CMAttachment[] attachments)
        {
            string errors;
            sendMail(sFrom, sTo, "", "", sSubject, sBody, CMMailFormat.HTML, attachments, null, out errors);
        }

        public void sendMail(string sFrom, string sTo, string sSubject, string sBody)
        {
            string errors;
            sendMail(sFrom, sTo, "", "", sSubject, sBody, CMMailFormat.HTML, (CMAttachment[])null, out errors);
        }

        public bool markRead(string messageId, out string errMsg)
        {
            errMsg = string.Empty;
            bool result = false;

            var message = _graphClient.Users[this._mailbox].Messages.GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Filter = String.Format("internetmessageid eq '{0}'", messageId);
            }).Result.Value.FirstOrDefault();

            if (message != null)
            {
                string token = string.Empty;

                string authUrl = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MS_GRAPH_LOGIN_URL") ?? "https://login.microsoftonline.com/"; //imposto un default per sicurezza
                string authSendUrl = "/{tenant}/oauth2/v2.0/token";

                RestClient authClient = new RestClient(authUrl);
                RestRequest authRequest = new RestRequest(authSendUrl, Method.Post);

                authRequest.AddUrlSegment("tenant", this._tenantId);
                authRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                authRequest.AddParameter("scope", this._scopes[0]);
                authRequest.AddParameter("client_secret", this._clientSecret);
                authRequest.AddParameter("grant_type", "client_credentials");
                authRequest.AddParameter("client_id", this._clientId);

                var authResponse = authClient.Execute(authRequest);

                if (authResponse.StatusCode == HttpStatusCode.OK && authResponse.IsSuccessful)
                {
                    var responseToken = System.Text.Json.JsonSerializer.Deserialize<MSGetTokenResponse>(authResponse.Content);
                    token = System.Text.Json.JsonSerializer.Deserialize<MSGetTokenResponse>(authResponse.Content).access_token;
                    if (responseToken.access_token != null)
                    {
                        token = responseToken.access_token;
                    }
                }

                RestClient client = new RestClient("https://graph.microsoft.com/v1.0");
                string sendUrl = "users/{mail}/messages/{messageId}";
                RestRequest request = new RestRequest(sendUrl, Method.Patch);
                request.AddUrlSegment("mail", this._mailbox);
                request.AddUrlSegment("messageId", message.Id);
                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddHeader("Content-Type", "application/json");

                var requestBody = @"{ ""isRead"" : true }";

                request.AddStringBody(requestBody, DataFormat.Json);

                var response = client.Execute(request);

                if (response != null)
                {
                    var content = !string.IsNullOrEmpty(response.Content) ? JObject.Parse(response.Content) : null;
                    if (response.StatusCode == HttpStatusCode.OK && response.IsSuccessful)
                    {
                        logger.DebugFormat("Messaggio con id {0} segnato come letto", (content != null ? content["internetMessageId"].ToString() : messageId));
                        result = true;
                    }
                    else
                    {
                        if (content != null)
                        {
                            string errorCode = (content["error"])["code"].ToString();
                            string errorMsg = (content["error"])["message"].ToString();
                            logger.DebugFormat("Response status {0}, errore {1} - {2}", response.StatusCode, errorCode, errorMsg);
                            errMsg = string.Format("{0} - {1}", errorCode, errorMsg);
                            result = false;
                        }
                    }
                }
            }
            return result;
        }

        #region Metodi privati
        /// <summary>
        /// il mittente in from non è di tipo pec
        /// </summary>
        /// <returns></returns>
        private bool isFromNonPEC(Message message)
        {
            return isPECError(message) &&
                message.Subject.Trim().ToUpper().StartsWith("ANOMALIA MESSAGGIO") &&
                ((message.Body.Content.IndexOf("dati non sono stati certificati") >= 0));
        }
        private bool isPECError(Message message)
        {
            var xTransportHeader = message.InternetMessageHeaders.Where(h => h.Name.ToLower().Equals("x-trasporto")).FirstOrDefault();
            if (xTransportHeader != null)
                return (xTransportHeader.ToString().ToLower() == "errore");
            else
                return false;
        }

        public static byte[] ReadStream(Stream input)
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

        #endregion
    }

    internal class MSGetTokenResponse
    {
        /*Indicates the token type value. The only type that Azure AD supports is Bearer.*/
        public string token_type { get; set; }

        /*How long the access token is valid (in seconds).*/
        public int expires_in { get; set; }

        /*Used to indicate an extended lifetime for the access token and to support resiliency when the token issuance service is not responding.*/
        public int ext_expires_in { get; set; }

        /*The requested access token. Your app can use this token in calls to Microsoft Graph.*/
        public string access_token { get; set; }
    }

    public class MSSendMessageRequest
    {
        public MSMessage Message { get; set; }
    }

    public class MSMessage
    {
        public string Subject { get; set; }
        public MSItemBody Body { get; set; }
        public MSRecipient From { get; set; }
        public List<MSRecipient> ToRecipients { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MSRecipient> CcRecipients { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MSRecipient> BccRecipients { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MSAttachment> Attachments { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MSInternetMessageHeader> InternetMessageHeaders { get; set; }
    }

    public class MSRecipient
    {
        public MSEmailAddress EmailAddress { get; set; }
    }

    public class MSEmailAddress
    {
        public string Address { get; set; }
    }

    public class MSItemBody
    {
        public string ContentType { get; set; }
        public string Content { get; set; }

    }

    public class MSAttachment
    {
        public string ContentType { get; set; }
        public string Name { get; set; }

        [JsonProperty(PropertyName = "@odata.type")]
        public string OdataType { get; set; }
        public string ContentBytes { get; set; }
    }

    public class MSInternetMessageHeader
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}

#endif
