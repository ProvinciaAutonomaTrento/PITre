using System;
using System.Collections;
using System.IO;
using log4net;
using ActiveUp.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;

namespace BusinessLogic.Interoperabilità
{
    public class Msg : Message
    {
        private Message msg;

        public Msg(Message message)
        {
            msg = message;
        }
        public bool ReceivedSigned
        {
            get
            {
                if (msg.HasSmimeSignature || msg.HasSmimeDetachedSignature)
                    return true;

                return false;
            }
        }


        public bool SignaturesValid
        {
            get
            {
                //Sempre vero come la vecchia lib
                return true;

                if (msg.Signatures.Smime != null)
                {
                    try
                    {
                        msg.Signatures.Smime.CheckSignature(true);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;

            }
        }
    }
    public class Pop3Cli : Pop3Client
    {

        public string MailHost;
        public int MailPort;
        public string PopUsername;
        public string PopPassword;
        public int ConnectTimeout;
        public bool PopSsl;
        public string LastErrorText;
        private static ILog logger = LogManager.GetLogger(typeof(Pop3Cli));

        public MessageCollection CopyMail()
        {
            return GetMail(false);
        }

        public MessageCollection TransferMail()
        {
            return GetMail(true);
        }

        public bool Login()
        {
            bool retval = false;
            try
            {
                if (PopSsl)
                    this.ConnectSsl(MailHost, MailPort, PopUsername, PopPassword);
                else
                    this.Connect(MailHost, MailPort, PopUsername, PopPassword);
                return true;
            }
            catch (Pop3Exception p3ex)
            {
                this.LastErrorText = p3ex.Message;
            }
            return retval;

        }

        private MessageCollection GetMail(bool delete)
        {
            MessageCollection mc = null;
            try
            {

                if (!this.IsConnected)
                    Login();
                if (this.IsConnected)
                {
                    mc = new MessageCollection();
                    int i;
                    this.UpdateStats();
                    logger.DebugFormat("Trovati {0} messaggi dopo la login", this.MessageCount);
                    for (i = 1; i <= this.MessageCount; i++)
                    {
                        try
                        {
                            logger.DebugFormat("RetrieveMessage inizio({0})", i);
                            byte[] msgData = this.RetrieveMessage(i);
                            logger.DebugFormat("RetrieveMessage  fine({0} -> {1} byte)", i, msgData.Length);
                            if (msgData.Length > 0)
                                mc.Add(ActiveUp.Net.Mail.Parser.ParseMessage(msgData));
                            else
                                logger.ErrorFormat("Il messaggio è grande zero byte, errore nella RetrieveMessage");

                            msgData=null;
                        }
                        catch (Exception e)
                        {
                            this.LastErrorText = e.Message;
                            logger.ErrorFormat("Errore durante la GetMail| RetrieveMessageObject (pop) {0}, stack {1}", e.Message, e.StackTrace);
                        }
                    }
                }
            }
            catch (Pop3Exception p3ex)
            {
                this.LastErrorText = p3ex.Message;
                logger.ErrorFormat("Pop3Exception durante la GetMail(pop) {0}, stack {1}", p3ex.Message, p3ex.StackTrace);

            }
            catch (Exception e)
            {
                this.LastErrorText = e.Message;
                logger.ErrorFormat("Errore durante la GetMail(pop) {0}, stack {1}", e.Message, e.StackTrace);
            }

            return mc;
        }

        public bool VerifyPopConnection()
        {
            return this.IsConnected;


            /*
            bool retval = false;
            Pop3Cli p3 = new Pop3Cli ();
            p3.MailHost = MailHost;
            p3.MailPort = MailPort;
            p3.PopSsl = PopSsl;

            try
            {
                if (PopSsl)
                    p3.ConnectSsl(MailHost, MailPort);
                else
                    p3.Connect(MailHost, MailPort);
                retval = true;

            }
            catch (Exception e)
            {
                this.LastErrorText = e.Message;
                retval = false;
            }
            finally
            {
                p3.Disconnect();
            }
            p3.Dispose(true);
            return retval;
            */
        }

    }
    public class SmtpCli : SmtpClient
    {

        public string SmtpHost;
        public int SmtpPort;
        public bool StartTLS;
        public bool SmtpSsl;
        public bool SmtpAuthenticate;
        public string serverMessage;

        public string SmtpUsername;
        public string SmtpPassword;
        private static ILog logger = LogManager.GetLogger(typeof(SmtpCli));
        //Metodo non statico 
        public bool Send(Message message, ServerCollection servers)
        {
            // Ensure that the mime part tree is built
            message.CheckBuiltMimePartTree();

            serverMessage = string.Empty;
            ActiveUp.Net.Mail.SmtpClient smtp = this;
            bool messageSent = false;
            for (int i = 0; i < servers.Count; i++)
            {
                try
                {
                    if (servers[i].ServerEncryptionType != EncryptionType.None)
                    {

                        smtp.ConnectSsl(servers[i].Host, servers[i].Port);
                    }
                    else
                    {
                        smtp.Connect(servers[i].Host, servers[i].Port);
                    }
                    try
                    {
                        smtp.Ehlo(System.Net.Dns.GetHostName());
                    }
                    catch
                    {
                        smtp.Helo(System.Net.Dns.GetHostName());
                    }
                    if (servers[i].Username != null && servers[i].Username.Length > 0 && servers[i].Password != null && servers[i].Password.Length > 0) smtp.Authenticate(servers[i].Username, servers[i].Password, SaslMechanism.Login);
                    if (message.From.Email != string.Empty) smtp.MailFrom(message.From);
                    else smtp.MailFrom(message.Sender);
                    smtp.RcptTo(message.To);
                    smtp.RcptTo(message.Cc);
                    smtp.RcptTo(message.Bcc);
                    string messageString = message.ToMimeString();
                    //UltraDebug
                    //logger.DebugFormat ("EMAIL \r\n {0} \r\n",Convert.ToBase64String (System.Text.ASCIIEncoding.ASCII.GetBytes(messageString)));

                    serverMessage = smtp.Data(messageString);//,(message.Charset!=null ? message.Charset : "iso-8859-1"));
                    logger.DebugFormat("Send Mail Server Message {0}", serverMessage);
                    smtp.Disconnect();
                    messageSent = true;
                    break;
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Errore inviando la mail {0} con ms.net  stack {1}", e.Message, e.StackTrace);
                    continue;
                }
            }

            return messageSent;
        }


    }
    public class ImapCli : Imap4Client
    {
        public string imapUsername;
        public string imapPassword;
        public bool imapSsl;
        public string imapHost;
        public int imapPort;
        public int ConnectTimeout;
        public string LastErrorText;
        public string mailbox;
        private static ILog logger = LogManager.GetLogger(typeof(ImapCli));

        public bool CreateMailboxSimple(string mailboxName)
        {
            try
            {
                CreateMailbox(mailboxName);
                return true;
            }
            catch (Imap4Exception ix)
            {
                this.LastErrorText = ix.Message;
            }
            return false;
        }


        public MessageCollection CopyMail()
        {
            return GetMail(false);
        }

        public MessageCollection TransferMail()
        {
            return GetMail(true);
        }
        public bool Login()
        {
            bool retval = false;
            if (!this.IsConnected)
            {
                try
                {
                    if (imapSsl)
                    {
                        this.ConnectSsl(imapHost, imapPort);
                        this.LoginFast(imapUsername, imapPassword);
                    }
                    else
                    {
                        this.Connect(imapHost, imapPort, imapUsername, imapPassword);
                    }
                    retval = true;
                }
                catch (Imap4Exception i4e)
                {
                    this.LastErrorText = i4e.Message;
                    retval = false;
                }
            }
            return retval;
        }

        private MessageCollection GetMail(bool delete)
        {
            MessageCollection mc = null;
            try
            {
                this.Login();
                if (this.IsConnected)
                {
                    mc = new MessageCollection();
                    Mailbox mb = this.SelectMailbox(mailbox);
                    logger.DebugFormat("Trovati {0} messaggi dopo la login", mb.MessageCount);

                    int i;
                    for (i = 1; i <= mb.MessageCount; i++)
                    {
                        try
                        {
                            logger.DebugFormat("mb.Fetch.MessageObject ({0})", i);
                            mc.Add(mb.Fetch.MessageObject(i));
                            if (delete)
                                mb.DeleteMessage(i, true);
                        }
                        catch (Exception e)
                        {
                            this.LastErrorText = e.Message;
                            logger.ErrorFormat("Errore durante la GetMail| mb.Fetch.MessageObject (imap) {0}, stack {1}", e.Message, e.StackTrace);
                        }

                    }
                }
            }

            catch (Imap4Exception  i4ex)
            {
                this.LastErrorText = i4ex.Message;
                logger.ErrorFormat("Imap4Exception durante la GetMail(impa) {0}, stack {1}", i4ex.Message, i4ex.StackTrace);
            }
            catch (Exception e)
            {
                this.LastErrorText = e.Message;
                logger.ErrorFormat("Errore durante la GetMail(imap) {0}, stack {1}", e.Message, e.StackTrace);
            }

            return mc;
        }



    }
    public class SvrPosta_MSNet : IMailConnector
    {
        private static ILog logger = LogManager.GetLogger(typeof(SvrPosta_MSNet));

        public Pop3Cli pop3 = null;
        public SmtpCli smtp = null;
        public ImapCli imap4 = null;

        #region Dichiarazioni

        private string server;
        private string password;
        private string user;
        private string port;
        private string workingDir;
        private CMClientType clientType;
        private string SmtpSsl;
        private string PopSsl; //valido anche per imapSsl
        public string codifica;
        private string smtpSTA;
        //private string connTimeout;
        //private string readTimeout;


        private MessageCollection mc;
        //modifica
        private string mailbox;
        private string mailelaborate;
        private string mailNonElaborate;
        //fine modifica



        #endregion

        #region Costruttori

        public SvrPosta_MSNet(string sServer, string sUser, string sPassword, string sPort, string sWorkingDir, CMClientType clientType, string SmtpSsl, string PopSsl, string smtpSTA, string Mailbox, string Mailelaborate, string MailNonElaborate)
        {
            this.server = sServer;
            this.user = sUser;
            this.password = sPassword;
            this.port = sPort;
            this.workingDir = sWorkingDir;
            this.clientType = clientType;
            this.SmtpSsl = SmtpSsl;
            this.PopSsl = PopSsl;
            this.smtpSTA = smtpSTA;

            mailbox = Mailbox;
            mailelaborate = Mailelaborate;
            mailNonElaborate = MailNonElaborate;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sServer"></param>
        /// <param name="sUser"></param>
        /// <param name="sPassword"></param>
        /// <param name="sPort"></param>
        /// <param name="sWorkingDir"></param>
        /// <param name="clientType"></param>
        /// <param name="Ssl"></param>
        public SvrPosta_MSNet(string sServer, string sUser, string sPassword, string sPort, string sWorkingDir, CMClientType clientType, string SmtpSsl, string PopSsl, string smtpSTA)
        {

            this.server = sServer;
            this.user = sUser;
            this.password = sPassword;
            this.port = sPort;
            this.workingDir = sWorkingDir;
            this.clientType = clientType;
            this.SmtpSsl = SmtpSsl;
            this.PopSsl = PopSsl;
            this.smtpSTA = smtpSTA;
            //mbx = null;
            //imap = null;
        }

        public SvrPosta_MSNet(string sServer, string sUser, string sPassword, string sPort, string sWorkingDir, CMClientType clientType, string SmtpSsl, string smtpSTA)
        {

            this.server = sServer;
            this.user = sUser;
            this.password = sPassword;
            this.port = sPort;
            this.workingDir = sWorkingDir;
            this.clientType = clientType;
            this.SmtpSsl = SmtpSsl;
            this.PopSsl = "";
            this.smtpSTA = smtpSTA;

        }

        public SvrPosta_MSNet()
        {
        }


        #endregion


        //Ricva il connectTimeout da imap o pop3
        private int getSmtpTimeout(bool connectionTimeout)
        {
            string popTimeoutType =  "POP3_READ_TIMEOUT";
            string imapTimeOutType = "IMAP_READ_TIMEOUT";


            if (connectionTimeout)
            {
                popTimeoutType =  "POP3_CONN_TIMEOUT";
                imapTimeOutType = "IMAP_CONN_TIMEOUT";
            }

            int retval;
            string timeout = null; ;
            if (System.Configuration.ConfigurationManager.AppSettings[popTimeoutType] != null &&
             System.Configuration.ConfigurationManager.AppSettings[popTimeoutType].ToString() != "")
                timeout = System.Configuration.ConfigurationManager.AppSettings[popTimeoutType].ToString();

            if (System.Configuration.ConfigurationManager.AppSettings[imapTimeOutType] != null &&
            System.Configuration.ConfigurationManager.AppSettings[imapTimeOutType].ToString() != "")
                timeout = System.Configuration.ConfigurationManager.AppSettings[imapTimeOutType].ToString();


            if (timeout != null)
            {
                Int32.TryParse(timeout, out retval);
                retval = retval * 1000;
            }
            else
            {

                logger.Debug("Timeout SMTP non trovato in config, lo imposto a 10000 ms");
                retval = 10000;
             //   retval = -1;
            }

            logger.DebugFormat("Timeout SMTP impostato a {0} ms", retval);
            return retval;

        }



        private void smtpInitialize()
        {
            if (smtp == null)
                smtp = new SmtpCli();

            smtp.SmtpHost = this.server;
            smtp.SmtpSsl = (this.SmtpSsl == "1") ? true : false;


            if (!this.smtpSTA.Equals(""))
                smtp.StartTLS = (this.smtpSTA == "1") ? true : false;


            smtp.ReceiveTimeout = getSmtpTimeout(false);
            logger.DebugFormat("Smtp Initalized for user {0} - SessionID {1}", this.user, Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(this.password)))));

        }
        private void pop3Initialize()
        {

            if (pop3 == null)
                pop3 = new Pop3Cli();


            pop3.MailHost = this.server;
            pop3.PopUsername = this.user;
            pop3.PopPassword = this.password;
            ;

            if (System.Configuration.ConfigurationManager.AppSettings["POP3_CONN_TIMEOUT"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["POP3_CONN_TIMEOUT"].ToString() != "")
                pop3.ConnectTimeout = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["POP3_CONN_TIMEOUT"].ToString()) * 1000;

            if (System.Configuration.ConfigurationManager.AppSettings["POP3_READ_TIMEOUT"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["POP3_READ_TIMEOUT"].ToString() != "")
                pop3.ReceiveTimeout = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["POP3_READ_TIMEOUT"].ToString()) * 1000;


            if (!this.PopSsl.Equals(""))
                pop3.PopSsl = (this.PopSsl == "1") ? true : false;

        }

        private void ImapInitialize()
        {
            if (imap4 == null)
                imap4 = new ImapCli();


            imap4.imapHost = this.server;
            imap4.imapUsername = this.user;
            imap4.imapPassword = this.password;

            ////imap.Connect(this.server);
            ////imap.Login(this.user, this.password);

            if (System.Configuration.ConfigurationManager.AppSettings["IMAP_CONN_TIMEOUT"] != null &&
              System.Configuration.ConfigurationManager.AppSettings["IMAP_CONN_TIMEOUT"].ToString() != "")
                imap4.ConnectTimeout = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["IMAP_CONN_TIMEOUT"].ToString()) * 1000;

            if (System.Configuration.ConfigurationManager.AppSettings["IMAP_READ_TIMEOUT"] != null &&
                System.Configuration.ConfigurationManager.AppSettings["IMAP_READ_TIMEOUT"].ToString() != "")
                imap4.ReceiveTimeout = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["IMAP_READ_TIMEOUT"].ToString()) * 1000;

            imap4.imapSsl = (this.PopSsl == "1") ? true : false;//this.mm.PopSsl;
            imap4.mailbox = this.mailbox;

        }

        #region Metodi pubblici

        public bool cancellaMailImap()
        {
            try
            {
                imap4.Expunge();
                return true;
            }
            catch (Exception e)
            {
                logger.Debug("cancellaMailImap - errore: " + e.Message);
            }
            return false;
        }

        public bool moveImap(int index, bool elaborata)
        {
            bool retval = false;
            try
            {

                // int id = messageSet.GetId(index - 1);//index - 1); // id della mail da spostare e cancellare dalla inbox
                string cartella = mailbox + ".";      //cartella su cui spostare le mail

                if (!imap4.IsConnected)
                    if (!imap4.Login())
                        return false;


                cartella = cartella + (elaborata ? mailelaborate : mailNonElaborate);
                Mailbox mb = imap4.SelectMailbox(imap4.mailbox);
                try
                {
                    mb.CopyMessage(index , cartella);
                }
                catch (Imap4Exception ex)
                {
                    imap4.LastErrorText = ex.Message;
                    retval = false;
                }


                FlagCollection fc = new FlagCollection();
                fc.Add("Deleted");
                mb.SetFlags(index , fc);
                retval = true;
            }

            catch (Exception e)
            {
                logger.Debug("errore durante il move imap");
                logger.Debug(imap4.LastErrorText + ";errore:" + e.Message);
                return false;
            }

            return retval;
        }

        public bool moveImap(string uid, bool elaborata)
        {
            bool retval = false;

            return retval;
        }

        public bool salvaMailInLocale(int indexMail, string pathFile, string NomeDellaMail)
        {
            bool retval = false;
            try
            {

                logger.DebugFormat("Nome Della Mail Idx[{0}]: {1} {2} ", indexMail, pathFile, NomeDellaMail);

                indexMail--;
                string fileName = Path.Combine(pathFile, NomeDellaMail);
                System.IO.File.WriteAllBytes(fileName, mc[indexMail].OriginalData);

                //Pare che non funziona
                //mc[indexMail].StoreToFile(@pathFile + "\\" + NomeDellaMail);
                retval = true;
            }
            catch (Exception e)
            {
                logger.DebugFormat("Errore durante il salvataggio della mail :{0}  Stack {1}", e.Message, e.StackTrace);
                throw new Exception("Errore durante il salvataggio della mail" + e.Message);
            }
            return retval;

        }

        public bool salvaMailInLocale(string uidl, string pathFile, string NomeDellaMail)
        {
            return false;
        }

        public bool getMessagePec(string uidl)
        {
            return false;
        }

        public bool getMessagePec(int index)
        {
            bool retval = false;
            if (this.clientType == CMClientType.POP)
            {
                if (pop3 == null)
                    pop3Initialize();
            }
            else

                if (this.clientType == CMClientType.IMAP)
                {
                    if (imap4 == null)
                    {
                        ImapInitialize();
                        connect();
                    }

                }


            index--;
            try
            {
                if ((this.clientType == CMClientType.POP) && !pop3.VerifyPopConnection())
                    throw new Exception("(connerr)errore di connessione alla casella pop: " + pop3.LastErrorText);


                Message msgComponent = this.mc[index];
                Msg m = new Msg(msgComponent);

                if ((m.ReceivedSigned) && (!m.SignaturesValid))
                    return false;

                retval = extractMailPec(msgComponent);

            }
            catch (Exception exc)
            {
                logger.DebugFormat("Errore nel recupero del tipo della mail dal server [{0}]. {1}", server, exc.Message);
                throw new Exception(String.Format("Errore nel recupero della mail dal server [{0}]. {1}", server, exc.Message));
            }

            return retval;
        }


        private CMMsg extractMessage(Message msgComponent)
        {
            logger.Debug("start");
            CMMsg msg = null;
            msg = extractMail(msgComponent);
            if (msg != null)
            {
                if (msg.isPECDelivered() ||
                    (msg.isFromNonPEC() &&
                    (msgComponent.SubMessages.Count == 1)))
                {
                    msgComponent = msgComponent.SubMessages[0];
                    msg = extractMail(msgComponent);
                    int dsnRootLeaf = isDSN(msgComponent);
                    if (dsnRootLeaf != 0)
                    {
                        CMMsg msgNew = extractDSNInfo(msgComponent,dsnRootLeaf);
                        foreach (string h in msgNew.headers.Keys)
                            msg.headers.Add(h, msgNew.getHeader(h));

                        msg.subject = removeCRLF (msgNew.subject).Replace ("\t"," ");
                        msg.HTMLBody = msgNew.body;
                        msg.body = msgNew.body;
                        msg.recipients = msgNew.recipients;

                    }
                }
            }
            return msg;
        }

        private int isDSN(Message e)
        {
            int retval = 0;
            if (e.LeafMimeParts.Count > 1)
            {
                foreach (MimePart mp in e.LeafMimeParts)
                {
                    if (mp.ContentType.MimeType == "message/delivery-status")
                        return retval;
                    retval++;
                }
            }
            return 0;
        }

        private CMMsg extractDSNInfo(Message msgComponent,int dsnRootLeaf)
        {
            CMMsg retval = new CMMsg();

            if (msgComponent.LeafMimeParts.Count > 1)
            {
                MimePart dstat = msgComponent.LeafMimeParts[dsnRootLeaf];
                retval.body = string.Format("<PRE> {0} </PRE>", System.Text.ASCIIEncoding.ASCII.GetString(msgComponent.OriginalData));


                if (dstat.ContentType.MimeType == "message/delivery-status")
                {


                    string msgDstat = dstat.OriginalContent;
                    string dsnErrorCode = CMMsg.findSmptErrorCode(msgDstat);
                    retval.headers.Add("IsDSN", "TRUE");
                    if (dsnErrorCode == null)
                        dsnErrorCode = "550";

                    retval.headers.Add("Status", dsnErrorCode);
                    string dsnErrorText = CMMsg.decodeSmtpErrorCode(dsnErrorCode);
                    if (dsnErrorText != null)
                        retval.headers.Add("Diagnostic", dsnErrorText);
                    else
                        retval.headers.Add("Diagnostic", "Generic smpt error");
                
                    retval.headers.Add("FailedRecepients", getfailedRecepients(dstat.TextContent));

                    if (msgComponent.LeafMimeParts.Count > dsnRootLeaf + 1)
                    {
                        MimePart part3 = msgComponent.LeafMimeParts[dsnRootLeaf+1];
                        string mimeType = part3.ContentType.MimeType;
                        if ((mimeType == "text/rfc822-headers") || (mimeType == "message/rfc822"))
                        {
                            string msg = part3.OriginalContent;
                            msg = msg.Substring(msg.IndexOf("Received:"));
                            Message m = Parser.ParseMessage(msg + "\r\n\r\n");
                            retval.subject = removeCRLF(m.Subject).Replace ("\t"," ");
                            retval.from = m.From.ToString();

                            foreach (Address ac in m.To)
                            {
                                retval.recipients.Add(new CMRecipient { mail = ac.Email, name = ac.Name });
                            }
                        }
                    }
                }
            }
            return retval;
        }

        private string getfailedRecepients(string mailText)
        {
            string retval = string.Empty; 

            string[] lines = mailText.Split('\r');
            foreach (string line in lines)
            {
                if (line.Contains("Final-Recipient"))
                {
                    string [] lineArr = line.Split(';');
                    if (lineArr.Length > 1)
                        retval += lineArr[1] + "§";
                    else
                        retval += lineArr[0] + "§";
                }
                if (line.Contains("Status: "))
                {
                    retval += line.Replace("Status: ",string.Empty) + "§";
                }

                if (line.Contains("Diagnostic-Code:"))
                {
                    string [] lineArr = line.Split(';');
                    if (lineArr.Length > 1)
                        retval += lineArr[1] + ";";
                    else
                        retval += lineArr[0] + ";";
                }
                


            }

            retval =retval.Replace("<", string.Empty).Replace(">", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
            return retval;


        }

        public CMMsg getMessage(byte[] email)
        {
            Message msgComponent = Parser.ParseMessage(email);
            if (msgComponent != null)
            {
                Msg m = new Msg(msgComponent);
                if ((m.ReceivedSigned) && (!m.SignaturesValid))
                    return null;
            }
            return extractMessage(msgComponent); ;
        }

        #region GESTION SCARICO CASELLA CON UIDL

        public CMMsg getMessage(string uidl)
        {
            return null;
        }

        public string[] getUidls()
        {
            return new string[0];
        }

        public void deleteSingleMessage(string uidl)
        {         
        }

        #endregion 

        public CMMsg getMessage(int index)
        {

            logger.DebugFormat("Inizio : GetMessage({0})", index);
            if (this.clientType == CMClientType.POP)
            {
                if (pop3 == null)
                {
                    pop3Initialize();
                    connect();
                }
            }
            else
                if (this.clientType == CMClientType.IMAP)
                {
                    if (imap4 == null)
                    {
                        ImapInitialize();
                        connect();
                    }

                }

            index--;
            try
            {
                //Perchè devo verificare la connessione pop? non ero già collegato?
                if ((this.clientType == CMClientType.POP) && !pop3.VerifyPopConnection())
                    throw new Exception("(connerr)errore di connessione alla casella pop: " + pop3.LastErrorText);

                Message msgComponent = this.mc[index];
                Msg m = new Msg(msgComponent);


                if ((m.ReceivedSigned) && (!m.SignaturesValid))
                    return null;

                return extractMessage(msgComponent);
            }
            catch (Exception exc)
            {
                logger.DebugFormat("Errore nel recupero del tipo della mail dal server [{0}]. {1}", server, exc.Message);
                throw new Exception(String.Format("Errore nel recupero della mail dal server [{0}]. {1}", server, exc.Message));
            }
        }


        //public bool deleteSingleMessagePop(int i)
        //{
        //    bool retval = false;
        //    if (this.clientType != CMClientType.POP)
        //        logger.Debug("Il client di posta non è inizializzato correttamente");

        //    i--;

        //    try
        //    {
        //        string uidl = mbx.GetEmail(i).Uidl;

        //        retval=  mm.DeleteByUidl(uidl);
        //    }
        //    catch (Exception exc)
        //    {
        //        logger.Debug(String.Format("La cancellazione del messaggio numero {0} è fallita sul server POP [{1}]. {2}", i, server, exc.Message));
        //    }

        //    return retval;
        //}


        public void deleteSingleMessage(int i)
        {
            if (this.clientType != CMClientType.POP)
                throw new Exception("Il client di posta non è inizializzato correttamente");

            i--;
            try
            {

                string id = mc[i].MessageId;
                pop3.UpdateStats();  //Aggiorno la lista..
                for (int a = 1; a <= pop3.MessageCount; a++)
                {
                    Message m = pop3.RetrieveMessageObject(a);
                    if (m.MessageId == id)
                    {
                        pop3.DeleteMessage(a);
                        //Refresh della casella
                        mc = pop3.CopyMail();
                    }

                }

            }
            catch (Exception exc)
            {
                string errmsg = String.Format("La cancellazione del messaggio numero {0} è fallita sul server POP [{1}]. {2}", i, server, exc.Message);
                logger.Debug(errmsg);

                throw new Exception(errmsg);
            }
        }

        private bool connectImap()
        {
            bool retval = false;
            string rs;
            try
            {
                try
                {
                    if (imap4.imapSsl)
                        rs = imap4.ConnectSsl(imap4.imapHost, imap4.imapPort);
                    else
                        rs = imap4.Connect(imap4.imapHost, imap4.imapPort);

                }
                catch (Imap4Exception ie)
                {
                    imap4.LastErrorText = ie.Message;
                }

                if (imap4.IsConnected)
                {
                    string logval = imap4.Login(imap4.imapUsername, imap4.imapPassword);
                    if (logval.ToLower().Contains("ok"))
                    {
                        logger.DebugFormat("la connessione al server IMAP con l'utente: {0} è stata effettuata con successo - session id : {1}  ", this.user, Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(this.password)))));
                        retval = true;
                    }
                }

                if (!retval)
                    throw new ApplicationException("errore nella connessione IMAP: " + imap4.LastErrorText);

                imap4.CreateMailboxSimple(mailbox);
                imap4.SelectMailbox(mailbox);
                imap4.CreateMailboxSimple(mailbox + "." + mailelaborate);
                imap4.CreateMailboxSimple(mailbox + "." + mailNonElaborate);
            }
            catch (Exception e)
            {
                logger.Error("connessione al server imap" + e.Message);
                retval = false;
            }

            return retval;
        }
        /*
        private bool connectPop()
        {
            bool retval = false;
            try
            {
                try
                {
                    if (pop3.PopSsl)
                        pop3.ConnectSsl(pop3.MailHost, pop3.MailPort, pop3.PopUsername, pop3.PopPassword);
                    else
                        pop3.Connect(pop3.MailHost, pop3.MailPort, pop3.PopUsername, pop3.PopPassword);

                }
                catch (Pop3Exception p3e)
                {
                    pop3.LastErrorText = p3e.Message;
                }

                if (imap4.IsConnected)
                    retval = true;

                if (!retval)
                {
                    throw new ApplicationException("errore nella connessione POP3: " + pop3.LastErrorText);
                }

            }
            catch (Exception e)
            {
                logger.Error("connessione al server pop3" + e.Message);
                logger.Debug("connessione al server pop3" + e.Message);
                retval = false;
            }

            return retval;
        }
        */
        public void connect()
        {
            switch (clientType)
            {
                case CMClientType.POP:

                    try
                    {
                        if (pop3 == null)
                            pop3Initialize();

                        //pop3.PopSsl = this.mm.PopSsl;//(PopPort != 110 && InteroperabilitaUtils.Cfg_Pop3OverSsl);
                        pop3.MailPort = Int32.Parse(this.port);
                        int n = InteroperabilitaUtils.Cfg_NumeroTentativiLogServerPosta;

                        for (int i = 1; i <= n; i++)
                        {
                            try
                            {
                                logger.DebugFormat("Tentativo di connessione al server {0} con Userid: {1}  per controllo casella, numero {2} - session ID {3}" , pop3.MailHost, pop3.PopUsername, i, Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Convert.ToBase64String (System.Text.ASCIIEncoding.ASCII.GetBytes (pop3.PopPassword)))));
                                mc = pop3.CopyMail();
                                if (mc != null)
                                {
                                    logger.DebugFormat("Tentativo n# {0} di connessione al server {1} per controllo casella POP con userID {2}, effettuato con successo rilevate {3} email", i, pop3.MailHost, pop3.PopUsername, mc.Count);

                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                if (!string.IsNullOrEmpty(pop3.LastErrorText))
                                {
                                    logger.Error("Errore connessione POP3" + pop3.LastErrorText);
                                    throw new Exception(pop3.LastErrorText);
                                }
                                else
                                {
                                    logger.Error("Errore connessione POP3" + ex.Message);
                                    throw ex;
                                }
                            }
                        }

                        if (mc == null)
                        {
                            if (!string.IsNullOrEmpty(pop3.LastErrorText))
                            {
                                logger.Error("Errore connessione POP3" + pop3.LastErrorText);
                                throw new Exception(pop3.LastErrorText);
                            }
                            else
                            {
                                logger.Error("Errore connessione POP3");
                                throw new Exception("");
                            }
                        }

                    }
                    catch (Exception exc)
                    {
                        string errmsg = String.Format("Connessione al server POP [{0}] fallita. {1}", server, exc.Message);
                        logger.Error(errmsg);
                        throw new Exception(errmsg);
                    }

                    break;

                case CMClientType.SMTP:
                    try
                    {
                        if (smtp == null)
                            smtpInitialize();

                    }
                    catch (Exception exc)
                    {
                        string errmsg = String.Format("Connessione al server SMTP [{0}] fallita. {1}", server, exc.Message);
                        logger.Error(errmsg);
                        throw new Exception(errmsg);
                    }

                    break;

                case CMClientType.IMAP:

                    bool retval = true;
                    try
                    {
                        if (imap4 == null)
                            ImapInitialize();

                        if (retval)
                        {
                            imap4.imapPort = Int32.Parse(this.port);
                            imap4.imapHost = this.server;
                            imap4.imapUsername = this.user;
                            imap4.imapPassword = this.password;
                            int n = InteroperabilitaUtils.Cfg_NumeroTentativiLogServerPosta;

                            for (int i = 1; i <= n; i++)
                            {
                                try
                                {
                                    if (!connectImap())
                                        continue;  //Connessione fallita.. ripetere

                                    if (imap4.IsConnected)
                                    {
                                        mc = imap4.CopyMail();
                                        if (mc != null)
                                        {
                                            logger.DebugFormat("Tentativo n# {1} di connessione al server {0} per controllo casella,  effettuato con successo rilevate {2} email", imap4.imapHost, i, mc.Count);
                                            break;
                                        }
                                    }
                                }

                                catch (Exception ex)
                                {
                                    logger.ErrorFormat("errore connessione imap: {0} ; {1}", ex.Message, imap4.LastErrorText);
                                }
                            }
                        }
                    }

                    catch (Exception exc)
                    {
                        string errmsg = String.Format("Connessione al server IMAP [{0}] fallita. {1};{2}", server, exc.Message, imap4.LastErrorText);
                        logger.Error(errmsg);
                        throw new Exception(errmsg);
                    }

                    break;
            }
        }


        #region prova Connessione
        public bool provaConnessione(DocsPaVO.amministrazione.OrgRegistro.MailRegistro mailRegistro, out string errore, string tipoConnessione)
        {
            bool retval = false;
            errore = string.Empty;
            switch (tipoConnessione)
            {
                case "POP":

                    try
                    {
                        pop3 = new Pop3Cli();

                        pop3.MailHost = mailRegistro.ServerPOP;
                        pop3.PopUsername = mailRegistro.UserID;
                        pop3.PopPassword = mailRegistro.Password;

                        pop3.PopSsl = (mailRegistro.POPssl == "1") ? true : false;
                        pop3.MailPort = mailRegistro.PortaPOP;

                        try
                        {
                            mc = pop3.CopyMail();
                            if (mc != null)
                                retval = true;
                            else
                                errore = pop3.LastErrorText;


                        }
                        catch
                        {
                            logger.DebugFormat("errore in prova connessione di tipo pop: {0}", pop3.LastErrorText);
                        }

                    }
                    catch (Exception exc)
                    {
                        errore = pop3.LastErrorText;
                        logger.DebugFormat("errore in provaConnessione: ", exc.Message);
                    }

                    break;

                case "SMTP":

                    try
                    {
                        Message msg = new Message();
                        smtp = new SmtpCli();
                        if (!string.IsNullOrEmpty(mailRegistro.UserSMTP) && mailRegistro.Password != null)
                        {
                            smtp.SmtpUsername = mailRegistro.UserSMTP;
                            smtp.SmtpPassword = mailRegistro.PasswordSMTP;
                            smtp.SmtpAuthenticate = true;
                        }
                        else
                        {
                            errore = "Nome utente o password errati";
                            break;
                        }
                        smtp.SmtpSsl = (mailRegistro.SMTPssl == "1") ? true : false;
                        smtp.SmtpPort = mailRegistro.PortaSMTP;
                        smtp.StartTLS = (mailRegistro.SMTPsslSTA == "1") ? true : false;
                        smtp.SmtpHost = mailRegistro.ServerSMTP;

                        msg.BodyText.Text = "TEST CONNESSIONE";
                        if (!string.IsNullOrEmpty(mailRegistro.Email))
                            msg.From.Email = mailRegistro.Email;

                        AddressCollection ac = new AddressCollection();
                        ac.Add(new Address() { Name = "Test Connessione", Email = mailRegistro.Email });
                        msg.To = ac;


                        Server s = new Server(smtp.SmtpHost, smtp.SmtpPort);
                        s.ServerEncryptionType = EncryptionType.None;

                        if (smtp.SmtpAuthenticate)
                        {
                            s.Username = smtp.SmtpUsername;
                            s.Password = smtp.SmtpPassword;
                            s.RequiresAuthentication = true;
                        }

                        if (smtp.SmtpSsl)
                            s.ServerEncryptionType = EncryptionType.SSL;

                        if (smtp.StartTLS)
                            s.ServerEncryptionType = EncryptionType.TLS;

                        ServerCollection sc = new ServerCollection();
                        sc.Add(s);
                        string outErr = string.Empty;
                        bool res = smtp.Send(msg, sc);


                        if (res)
                            retval = true;
                        else
                            errore = smtp.serverMessage;


                    }
                    catch
                    {
                        errore = smtp.serverMessage;
                        logger.Debug("errore in prova connessione di tipo SMTP: " + smtp.serverMessage);
                    }

                    if (smtp.IsConnected)
                        smtp.Disconnect();
                    break;

                case "IMAP":
                    try
                    {
                        imap4 = new ImapCli();
                        imap4.imapSsl = (mailRegistro.POPssl == "1") ? true : false;
                        imap4.imapHost = mailRegistro.serverImap;
                        imap4.imapPort = mailRegistro.portaIMAP;

                        imap4.imapPassword = mailRegistro.Password;
                        imap4.imapUsername  = mailRegistro.UserID;

                        try
                        {


                            if (!connectImap())

                                errore = "Problemi di connnessione al server imap";


                        }
                        catch (Exception ex)
                        {
                            errore = imap4.LastErrorText;
                            logger.Debug("errore connessione imap: " + ex.Message + " ; ");
                        }

                    }


                    catch (Exception exc)
                    {
                        errore = imap4.LastErrorText;
                        logger.Debug("errore connessione imap: " + exc.Message + " ; ");
                    }
                    if (imap4.IsConnected)
                        imap4.Disconnect();
                    break;
            }


            return retval;
        }
        #endregion




        public void disconnect()
        {
            switch (clientType)
            {
                case CMClientType.POP:
                    try
                    {
                        if ((pop3 != null) && (pop3.IsConnected))
                            pop3.Disconnect();

                    }
                    catch (Exception exc)
                    {
                        string errmsg = String.Format("Disonnessione al server POP [{0}] fallita. {1}", server, exc.Message);
                        logger.Debug(errmsg);
                        throw new Exception(errmsg);
                    }
                    break;
                case CMClientType.SMTP:
                    try
                    {
                        if ((smtp != null) && (smtp.IsConnected))
                            smtp.Disconnect();
                    }
                    catch (Exception exc)
                    {
                        string errmsg = String.Format("Disconnessione al server SMTP [{0}] fallita. {1}", server, exc.Message);
                        logger.Error(errmsg);
                        throw new Exception(errmsg);
                    }
                    break;
                case CMClientType.IMAP:
                    try
                    {
                        if ((imap4 != null) && (imap4.IsConnected))
                            imap4.Disconnect();
                    }
                    catch (Exception exc)
                    {

                        string errmsg = String.Format("Disconnessione al server IMAP [{0}] fallita. {1}", server, exc.Message);
                        logger.Error(errmsg);
                        throw new Exception(errmsg);
                    }
                    break;
            }
        }

        public int messageCount()
        {
            int cnt = -1;

            try
            {
                if (this.clientType != CMClientType.POP)
                {  // throw new Exception("Il client di posta non è inizializzato correttamente");

                    if (imap4 != null)
                    {
                        logger.Debug("reperimento delle mail nel server IMAP");

                        if (!imap4.IsConnected)
                        {
                            logger.Debug("Connessione al server IMAP per recupere i messaggi");
                            if (connectImap())
                                logger.Debug("il login è avvuto per il count dei messaggi");
                        }

                        logger.Debug("già loggato eseguo il count dei messaggi");
                        Mailbox mb = null;
                        try
                        {
                            mb = imap4.SelectMailbox(mailbox);
                            cnt = mb.MessageCount;
                            logger.Debug("messageset= " + mb.MessageCount); //+messageset: " + messageSet.HasUids);
                        }
                        catch
                        {
                            logger.Debug("il messageset è null perchè il nome della casella inbox è errata");
                        }


                        logger.Debug("ho recuperato tutte le informazioni delle mail presenti sul server di posta eseguo un fetch dei messaggi la posta:");
                    }

                }
                else
                {
                    logger.Debug("recupero i messaggi dal server di tipo pop");
                    if (pop3.IsConnected)
                    {
                        pop3.UpdateStats();
                        cnt = pop3.MessageCount;
                    }
                }

                if (cnt < 0)
                {
                    string errmsg = "errore nel reperimento delle mail";
                    logger.Error(errmsg);
                    throw new Exception(errmsg);
                }
            }
            catch (Exception exc)
            {
                logger.DebugFormat("Connessione al server [{0}] non riuscita :msg {1}", server, exc.Message);
                throw new Exception(String.Format("Connessione al server [{0}] non riuscita", server));
            }

            return cnt;
        }

        #region SMTP

        private AddressCollection GenerateAddressCollection(string emailAdresses, char separator)
        {
            AddressCollection retval = new AddressCollection();

            if (emailAdresses.Contains((separator.ToString())))
            {
                string[] addresses = emailAdresses.Split(separator);
                foreach (string s in addresses)
                    retval.Add(s);
            }
            else
            {
                retval.Add(emailAdresses);
            }
            return retval;
        }

        private string removeCRLF(string inputString)
        {
            return inputString.Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, CMMailHeaders[] headers, out string outError)
        {
            outError = string.Empty;
            sSubject = removeCRLF(sSubject).Replace("\t", " ");
            //Creazione messaggio
            if (this.clientType != CMClientType.SMTP)
                throw new Exception("Il client di posta non è inizializzato correttamente");

            Message msg = new Message();

            try
            {
                switch (format)
                {
                    case CMMailFormat.HTML:
                        msg.BodyHtml.Text = sBody;
                        break;
                    case CMMailFormat.Text:
                        msg.BodyText.Text = sBody;
                        break;
                }


                msg.From.Email = sFrom;
                //commentato 11 nov 2005. Viene preso da web.config
                //se l'indirizzo mittente è nullo
                //				if(!(msg.From!=null && !msg.From.Equals("")))
                //					msg.From="protoetno@etnoteam.it";


                msg.To = GenerateAddressCollection(sTo, ',');
                msg.Subject = sSubject;
                //				msg.Priority = ?;

                if (sCC != "")
                    msg.Cc = GenerateAddressCollection(sCC, ',');

                if (sBCC != "")
                    msg.Bcc = GenerateAddressCollection(sBCC, ',');

                //Aggiunta gestione Header
                if (headers != null)
                    foreach (CMMailHeaders header in headers)
                    {
                        /*
                        msg.HeaderFieldNames.Add(header.header, header.value);
                        msg.HeaderFields.Add(header.header, header.header);
                         */
                        msg.AddHeaderField(header.header, header.value);
                    }


                if (attachments != null)
                    foreach (CMAttachment attachment in attachments)
                    {
                        MimePart mp = new MimePart(attachment._data, attachment.name);
                        mp.ContentType.MimeType = attachment.contentType;
                        if (attachment._data.Length == 0)
                           mp.ContentTransferEncoding = ContentTransferEncoding.SevenBits;

                        
                        msg.Attachments.Add(mp);
                    }
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Errore nella creazione del messaggio. {0}", exc.Message));
            }

            //Conessione al server SMTP
            try
            {
                if (user != null && user != "" && password != null)
                {
                    smtp.SmtpUsername = user;
                    smtp.SmtpPassword = password;
                    smtp.SmtpAuthenticate = true;

                }
                //				mm.SmtpSsl = InteroperabilitaUtils.Cfg_SmtpOverSsl;
                //				mm.SmtpPort = InteroperabilitaUtils.Cfg_SmtpOverSsl ? 465 : Int32.Parse(this.port);
                smtp.SmtpPort = Int32.Parse(this.port);

                //alcune connessioni smtp richiedono questo parametro, di solito quando 
                //questo parametro else true IList this.mm.SmtpSsl=false;
                smtpInitialize();

                int n = InteroperabilitaUtils.Cfg_NumeroTentativiLogServerPosta;
                bool res = false;

                Server s = new Server(smtp.SmtpHost, smtp.SmtpPort);
                s.ServerEncryptionType = EncryptionType.None;

                if (smtp.SmtpAuthenticate)
                {
                    s.Username = smtp.SmtpUsername;
                    s.Password = smtp.SmtpPassword;
                    s.RequiresAuthentication = true;
                }

                if (smtp.SmtpSsl)
                    s.ServerEncryptionType = EncryptionType.SSL;

                if (smtp.StartTLS)
                    s.ServerEncryptionType = EncryptionType.TLS;


                ServerCollection sc = new ServerCollection();
                sc.Add(s);

                for (int i = 1; i <= n; i++)
                {
                    logger.Debug("Tentativo di connessione al server " + smtp.SmtpHost + "con Userid: " + smtp.SmtpUsername + " numero " + i);
                    string outRes = string.Empty;
                    res = smtp.Send(msg, sc);

                    if (res)
                    {
                        logger.Debug("Tentativo di connessione al server " + smtp.SmtpHost + "con Userid: " + smtp.SmtpUsername + " numero " + i + " effettuato con successo");
                        break;
                    }
                }

                if (!res)
                {
                    outError = "Impossibile contattare il server SMTP";
                    throw new Exception(smtp.serverMessage);
                }
            }
            catch (Exception exc)
            {
                if (outError != "Impossibile contattare il server SMTP")
                    outError = "Errore generico";
                throw new Exception(String.Format("Errore nell'invio del messaggio tramite il server SMTP [{0}]. {1}", server, exc.Message));
            }
        }

        public void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, out string outError)
        {
            sendMail(sFrom, sTo, sCC, sBCC, sSubject, sBody, format, attachments, null, out outError);
        }

        public void sendMail(string sFrom, string sTo, string sSubject, string sBody, CMAttachment[] attachments)
        {
            string errors;
            sendMail(sFrom, sTo, "", "", sSubject, sBody, CMMailFormat.HTML, attachments, out errors);
        }
        
        public void sendMail(string sFrom, string sTo, string sSubject, string sBody)
        {
            string errors;
            sendMail(sFrom, sTo, "", "", sSubject, sBody, CMMailFormat.HTML, (CMAttachment[])null, out errors);
        }

        #endregion
        #endregion

        #region Metodi privati

        # region codice commentato
        //		private void getBody(Mime component, ref string body, ref string bodyHTML)
        //		{
        //			string contentType = component.ContentType;
        //
        //			for (int i = 0; i < component.NumParts; i++)
        //			{
        //				Mime part = new Mime();
        //				part.UnlockComponent(ChilkatKeys.SMime);
        //				part = component.GetPart (i);
        //
        //				if (part.ContentType.StartsWith("multipart")) 
        //					getBody(part, ref body,ref bodyHTML);
        //				else
        //				{
        //					if (part.ContentType.ToLower()  == "text/plain")
        //						body = part.GetEntireBody();
        //
        //					if (part.ContentType.ToLower() == "text/html")
        //					{
        //						bodyHTML = part.GetBodyDecoded();
        //						//						if (part.Header[1].Value.ToLower() == "quoted-printable")
        //						//						{
        //						//							QuotedPrintableDecoder PD = new QuotedPrintableDecoder(); 
        //						//							System.IO.MemoryStream So = new MemoryStream();  
        //						//							part.BodyStream.Position = 0;
        //						//							PD.Decode(part.BodyStream, So);
        //						//							bodyHTML = System.Text.Encoding.UTF8.GetString(So.ToArray(),0,So.ToArray().Length); 
        //						//						}
        //
        //						//						if (part.Header[1].Value.ToLower()  == "base64")
        //						//						{
        //						//							bodyHTML = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(part.Body),0,Convert.FromBase64String(part.Body).Length); 
        //						//						}
        //					}
        //					Console.WriteLine();
        //				}
        //			}
        //		}
        //
        #endregion
        //ridoni 25/10/2005 dopo interpvento in simest
        //ridoni 2/11/2005 dopo intervento per fix ANAS

        #region ExtractMail Della MailSys
        private bool extractMailPec(Message msgComponent)
        {
            string sTemp;
            //string formato = "ddd\\, dd MMM yyyy HH:mm:ss zzz";
            System.Globalization.CultureInfo MyCultureInfo = new System.Globalization.CultureInfo("en-US");
            CMMsg msg = new CMMsg();

            try
            {
                if (msgComponent != null)
                {
                    sTemp = ExtractEmail(msgComponent, msg);
                    return msg.isPECDelivered();
                }
                else
                    return false;
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Errore durante l'estrazione della mail. {1} ", exc.Message));
            }
        }

        private CMMsg extractMail(Message msgComponent)
        {
            logger.Debug("Start");
            // declarations
            string sTemp;
            //string formato = "ddd\\, dd MMM yyyy HH:mm:ss zzz";
            System.Globalization.CultureInfo MyCultureInfo = new System.Globalization.CultureInfo("en-US");
            CMMsg msg = new CMMsg();

            try
            {
                if (msgComponent != null)
                {
                    sTemp = ExtractEmail(msgComponent, msg);

                    //se la mail è una ricevuta di ritorno non eseguo il forward di una mail
                    //Attachments Message - Forward di una email alla casella istituzionale 
                    if (!(msg.isPECDelivered() || msg.isFromNonPEC()) &&
                        !BusinessLogic.interoperabilita.InteroperabilitaManager.isRicevutaPec(msg))
                    {
                        if (msgComponent.SubMessages.Count != 0)
                        {
                            //SANDALI  questo assume che cè solo un messaggio in attach.. non mi piace..no no.
                            Message mailAtt = msgComponent.SubMessages[0];
                            msg.attachments.Clear();

                            logger.Debug("Sono presenti buste nel messaggio");
                            for (int j = 0; j < mailAtt.Attachments.Count; j++)    //NumAttachments
                            {
                                string a_filename = RFC2047Decode(mailAtt.Attachments[j].ContentName); //mailAtt.Attachments[j].Filename;
                                string a_conttype = mailAtt.Attachments[j].ContentType.MimeType;
                                byte[] a_content = mailAtt.Attachments[j].BinaryContent;
                                CMAttachment att = new CMAttachment(a_filename, a_conttype, a_content);
                                logger.DebugFormat("Attach: {0} {1}", a_filename, a_conttype);

                                msg.attachments.Add(att);
                            }

                            for (int j = 0; j < mailAtt.UnknownDispositionMimeParts.Count; j++)    //NumAttachments
                            {
                                if (String.IsNullOrEmpty(mailAtt.UnknownDispositionMimeParts[j].ContentName))
                                    continue;

                                string a_filename = RFC2047Decode(mailAtt.UnknownDispositionMimeParts[j].ContentName); //mailAtt.Attachments[j].Filename;
                                string a_conttype = mailAtt.UnknownDispositionMimeParts[j].ContentType.MimeType;
                                byte[] a_content = mailAtt.UnknownDispositionMimeParts[j].BinaryContent;
                                CMAttachment att = new CMAttachment(a_filename, a_conttype, a_content);
                                logger.DebugFormat("Attach: {0} {1}", a_filename, a_conttype);

                                msg.attachments.Add(att);
                            }

                            for (int j = 0; j < mailAtt.EmbeddedObjects.Count; j++)    //NumAttachments
                            {
                                if (String.IsNullOrEmpty(mailAtt.EmbeddedObjects[j].ContentName))
                                    continue;

                                if (mailAtt.EmbeddedObjects[j].ContentType.Type.ToLower() != "application")
                                    continue;

                                string a_filename = RFC2047Decode(mailAtt.EmbeddedObjects[j].ContentName); //mailAtt.EmbeddedObjects[j].Filename;
                                string a_conttype = mailAtt.EmbeddedObjects[j].ContentType.MimeType;
                                byte[] a_content = mailAtt.EmbeddedObjects[j].BinaryContent;
                                CMAttachment att = new CMAttachment(a_filename, a_conttype, a_content);
                                logger.DebugFormat("Attach: {0} {1}", a_filename, a_conttype);

                                msg.attachments.Add(att);
                            }

                            msg.headers.Add("utenteDocspa", msg.from);
                            msg.from = mailAtt.From.Email;
                            msg.subject = removeCRLF(mailAtt.Subject).Replace("\t", " ");
                            MimePart mime_msg_1 = mailAtt.ToMimePart();

                            logger.DebugFormat("Email Busta:{3} :: {0}->{1} : {2}  Attach {4}", msg.from, msg.recipients, msg.subject, msg.date, msg.attachments);

                            if (mailAtt.PartTreeRoot.ContentType.MimeType.ToLower().StartsWith("multipart"))
                            {
                                msg.body = mailAtt.BodyText.Text;
                                if (mailAtt.BodyHtml.Text != String.Empty)
                                    msg.HTMLBody = mailAtt.BodyHtml.Text;
                            }
                            else
                            {
                                if ((mailAtt.BodyHtml.Text != "") && (mailAtt.BodyHtml.Text != null))
                                    msg.body = mailAtt.BodyHtml.Text;
                            }
                        }
                    }
                    // 	Recipients (TO)
                    for (int i = 0; i < msgComponent.To.Count; i++)
                    {
                        string name = msgComponent.To[i].Name;
                        string email = msgComponent.To[i].Email;
                        CMRecipient rcp = new CMRecipient();
                        rcp.mail = email;

                        if (name == null || name == string.Empty)
                            rcp.name = email;
                        else
                            rcp.name = msgComponent.To[i].Name;

                        msg.recipients.Add(rcp);
                    }

                    return msg;
                }
                else
                    return null;
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Errore durante l'estrazione della mail. {1} ", exc.Message));
            }
        }



        private static string ExtractEmail(Message msgComponent, CMMsg msg)
        {
            string sTemp = null;
            // mail info
            logger.Debug("ExtractEmail");
            string codifica = msgComponent.Charset;
            if (msgComponent.PartTreeRoot.ContentType.MimeType.ToLower().StartsWith("multipart"))
            {
                msg.body = msgComponent.BodyText.Text;
                if (msgComponent.BodyHtml.Text != String.Empty)
                    msg.HTMLBody = msgComponent.BodyHtml.Text;
            }
            else
            {
                if ((msgComponent.BodyHtml.Text != "") && (msgComponent.BodyHtml.Text != null))
                    msg.body = msgComponent.BodyHtml.Text;
            }

            if (String.IsNullOrEmpty(msg.body))
                msg.body = msgComponent.BodyText.Text;

            if (String.IsNullOrEmpty(msg.HTMLBody))
                msg.HTMLBody = msgComponent.BodyHtml.Text;
 

            msg.subject = msgComponent.Subject.Replace("\r",string.Empty).Replace("\n",string.Empty).Replace("\t", " ") ;

            try
            {
                //msg.date = msgComponent.ReceivedDate;
                msg.date = ParseAsDateTime(msgComponent.DateString);
            }
            catch
            { }

            msg.from = msgComponent.From.Email;

            // BCC
            for (int i = 0; i < msgComponent.Bcc.Count; i++)
                msg.hideRecipients += (msgComponent.Bcc[i] + ";");

            // Headers
            //string messageID = msgComponent.MessageId;
            //msg.headers.Add("message-id", messageID.Trim('<', '>'));

            for (int i = 0; i < msgComponent.HeaderFields.Count; i++)
            {
                string hname = msgComponent.HeaderFieldNames[i];
                string hval = msgComponent.HeaderFields[hname];
                if (msg.headers[hname] != null)
                {
                    sTemp = msg.headers[hname] + " ";
                    msg.headers.Remove(hname);
                }
                else
                    sTemp = "";

                msg.headers.Add(hname, sTemp + hval);
            }
            logger.DebugFormat("Email:{3} :: {0}->{1} : {2}  ", msg.from, msg.recipients, msg.subject, msg.date);

            // Attachments
            for (int i = 0; i < msgComponent.Attachments.Count; i++)
            {
                string a_filename = RFC2047Decode(msgComponent.Attachments[i].ContentName); // msgComponent.Attachments[i].Filename;
                string a_conttype = msgComponent.Attachments[i].ContentType.MimeType;
                byte[] a_content = msgComponent.Attachments[i].BinaryContent;
                logger.DebugFormat("Attach: {0} {1}", a_filename, a_conttype);
                if ((a_conttype != "application/ms-tnef") && ((a_filename != "smime.p7s") && (a_conttype != "application/x-pkcs7-signature")))
                {
                    CMAttachment att = new CMAttachment(a_filename,
                        a_conttype, a_content);

                    msg.attachments.Add(att);
                    logger.Debug("Attachment Added");
                }

            }
            // Attachment non riconosciuto come tali
            for (int i = 0; i < msgComponent.UnknownDispositionMimeParts.Count; i++)
            {
                if (String.IsNullOrEmpty(msgComponent.UnknownDispositionMimeParts[i].ContentName))
                {
                    continue;
                }
                string a_filename = RFC2047Decode(msgComponent.UnknownDispositionMimeParts[i].ContentName); // msgComponent.Attachments[i].Filename;
                string a_conttype = msgComponent.UnknownDispositionMimeParts[i].ContentType.MimeType;
                byte[] a_content = msgComponent.UnknownDispositionMimeParts[i].BinaryContent;
                logger.DebugFormat("Attach*: {0} {1}", a_filename, a_conttype);
                if ((a_conttype != "application/ms-tnef") && ((a_filename != "smime.p7s") && (a_conttype != "application/x-pkcs7-signature")))
                {
                    CMAttachment att = new CMAttachment(a_filename,
                        a_conttype, a_content);

                    msg.attachments.Add(att);
                    logger.Debug("Attachment* Added");
                }

            }

            // Inline che però ci servono attachment
            for (int i = 0; i < msgComponent.EmbeddedObjects.Count; i++)
            {
                if (String.IsNullOrEmpty(msgComponent.EmbeddedObjects[i].ContentName))
                    continue;

                if (msgComponent.EmbeddedObjects[i].ContentType.Type.ToLower() != "application")
                    continue;

                string a_filename = RFC2047Decode(msgComponent.EmbeddedObjects[i].ContentName); // msgComponent.Attachments[i].Filename;
                string a_conttype = msgComponent.EmbeddedObjects[i].ContentType.MimeType;
                byte[] a_content = msgComponent.EmbeddedObjects[i].BinaryContent;
                logger.DebugFormat("Attach!: {0} {1}", a_filename, a_conttype);
                if ((a_conttype != "application/ms-tnef") && ((a_filename != "smime.p7s") && (a_conttype != "application/x-pkcs7-signature")))
                {
                    CMAttachment att = new CMAttachment(a_filename,
                        a_conttype, a_content);

                    msg.attachments.Add(att);
                    logger.Debug("Attachment! Added");
                }

            }


            return sTemp;
        }


        #endregion

        #region ManipolazioneData e nomefile

        internal static string Clean(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, @"(\(((\\\))|[^)])*\))", "").Trim(' ');
        }

        private static string ReplaceTimeZone(string input)
        {
            input = input.Replace("EDT", "-0400");
            input = input.Replace("EST", "-0500");
            input = input.Replace("CDT", "-0500");
            input = input.Replace("CST", "-0600");
            input = input.Replace("MDT", "-0600");
            input = input.Replace("MST", "-0700");
            input = input.Replace("PDT", "-0700");
            input = input.Replace("PST", "-0800");
            input = input.Replace("UT", "+0000");
            input = input.Replace("GMT", "+0000");
            return input;
        }


        internal static int GetMonth(string month)
        {
            switch (month)
            {
                case "Jan": return 1;
                case "Feb": return 2;
                case "Mar": return 3;
                case "Apr": return 4;
                case "May": return 5;
                case "Jun": return 6;
                case "Jul": return 7;
                case "Aug": return 8;
                case "Sep": return 9;
                case "Oct": return 10;
                case "Nov": return 11;
                case "Dec": return 12;
                default: return -1;
            }
        }
        /// <summary>
        /// Parses as universal date time.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static System.DateTime ParseAsDateTime(string input)
        {
            try
            {
                input = ReplaceTimeZone(input);
                input = Clean(input);
                input = System.Text.RegularExpressions.Regex.Replace(input, @" +", " ");
                input = System.Text.RegularExpressions.Regex.Replace(input, @"( +: +)|(: +)|( +:)", ":");
                if (input.IndexOf(",") != -1)
                {
                    input = input.Replace(input.Split(',')[0] + ", ", "");
                }
                string[] parts = input.Replace("\t", string.Empty).Split(' ');
                int year = System.Convert.ToInt32(parts[2]);
                if (year < 100)
                {
                    if (year > 49) year += 1900;
                    else year += 2000;
                }
                int month = GetMonth(parts[1]);
                int day = System.Convert.ToInt32(parts[0]);
                string[] dateParts = parts[3].Split(':');
                int hour = System.Convert.ToInt32(dateParts[0]);
                int minute = System.Convert.ToInt32(dateParts[1]);
                int second = 0;
                if (dateParts.Length > 2) second = System.Convert.ToInt32(dateParts[2]);
                System.DateTime date = new System.DateTime(year, month, day, hour, minute, second);
                return date;
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        private static string RFC2047Decode(string p)
        {
            if (String.IsNullOrEmpty(p))
                return p;

            return RFC2047Decoder.Parse(p);
        }

        #endregion
        #endregion

        public string getBodyFromMail(string email)
        {
            string retval = null;
            Message msg = Parser.ParseMessage(email);

            retval = msg.BodyHtml.Text;

            if ((retval == null) || (retval == String.Empty))
                retval = msg.BodyText.Text;
            return retval;
        }

    }


}

