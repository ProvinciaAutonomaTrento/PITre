using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using log4net;
using System.IO;


namespace BusinessLogic.Interoperabilità
{
    public enum MailProvider
    { MailsystemNet, Chilkat }

    public class SvrPosta : IMailConnector
    {
        private IMailConnector mail;
        public MailProvider provider = MailProvider.Chilkat;  //Default su quella veccha dopo i test cambiamo
        private static ILog logger = LogManager.GetLogger(typeof(SvrPosta));

        private void initaliazeProvider()
        {
            //string ProviderStr = "c";
            //if (System.Configuration.ConfigurationManager.AppSettings["MAIL_PROVIDER"] != null &&
            //    System.Configuration.ConfigurationManager.AppSettings["MAIL_PROVIDER"].ToString() != "")
            //    ProviderStr = System.Configuration.ConfigurationManager.AppSettings["MAIL_PROVIDER"].ToString().ToLower();


            string ProviderStr = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MAIL_PROVIDER");
            if (String.IsNullOrEmpty(ProviderStr))
                ProviderStr = "c";

            if (ProviderStr.StartsWith("c"))
                provider = MailProvider.Chilkat;
            else if (ProviderStr.StartsWith("m"))
                provider = MailProvider.MailsystemNet;

            logger.Debug("Instanziato provider di posta :" + provider.ToString());
        }
        public SvrPosta()
        {
            initaliazeProvider();
            if (provider == MailProvider.MailsystemNet)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_MSNet();

            if (provider == MailProvider.Chilkat)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_CK();

        }

        public SvrPosta(string sServer, string sUser, string sPassword, string sPort, string sWorkingDir, CMClientType clientType, string SmtpSsl, string PopSsl, string smtpSTA, string Mailbox, string Mailelaborate, string MailNonElaborate)
        {
            initaliazeProvider();
            if (provider == MailProvider.MailsystemNet)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_MSNet(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, PopSsl, smtpSTA, Mailbox, Mailelaborate, MailNonElaborate);
            if (provider == MailProvider.Chilkat)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_CK(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, PopSsl, smtpSTA, Mailbox, Mailelaborate, MailNonElaborate);
        }

        public SvrPosta(string sServer, string sUser, string sPassword, string sPort, string sWorkingDir, CMClientType clientType, string SmtpSsl, string PopSsl, string smtpSTA)
        {
            initaliazeProvider();
            if (provider == MailProvider.MailsystemNet)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_MSNet(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, PopSsl, smtpSTA);
            if (provider == MailProvider.Chilkat)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_CK(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, PopSsl, smtpSTA);
        }

        public SvrPosta(string sServer, string sUser, string sPassword, string sPort, string sWorkingDir, CMClientType clientType, string SmtpSsl, string smtpSTA)
        {
            initaliazeProvider();
            if (provider == MailProvider.MailsystemNet)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_MSNet(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, smtpSTA);
            if (provider == MailProvider.Chilkat)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_CK(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, smtpSTA);

        }

        #region IMailConnector Members

        public CMMsg getMessage(int index)
        {
            return mail.getMessage(index);
        }

        public CMMsg getMessage(byte[] email)
        {
            return mail.getMessage(email);
        }

        public CMMsg getMessage(string uidl)
        {
            return mail.getMessage(uidl);
        }

        public string[] getUidls()
        {
            return mail.getUidls();
        }

        public void deleteSingleMessage(string uidl)
        {
            mail.deleteSingleMessage(uidl);
        }

        public void deleteSingleMessage(int i)
        {
            mail.deleteSingleMessage(i);

        }

        public void connect()
        {
            mail.connect();
        }

        public void disconnect()
        {
            mail.disconnect();
        }

        public int messageCount()
        {
            return mail.messageCount();
        }

        public void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, CMMailHeaders[] headers, out string outError)
        {
            mail.sendMail(sFrom, sTo, sCC, sBCC, sSubject, sBody, format, attachments, headers, out outError);
        }

        public void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, out string outError)
        {
            mail.sendMail(sFrom, sTo, sCC, sBCC, sSubject, sBody, format, attachments, out outError);
        }

        public void sendMail(string sFrom, string sTo, string sSubject, string sBody, CMAttachment[] attachments)
        {
            mail.sendMail(sFrom, sTo, sSubject, sBody, attachments);
        }
        
        public void sendMail(string sFrom, string sTo, string sSubject, string sBody)
        {
            mail.sendMail(sFrom, sTo, sSubject, sBody);
        }

       
        public bool moveImap(int index, bool elaborata)
        {
            return mail.moveImap(index, elaborata);
        }

        public bool moveImap(string uid, bool elaborata)
        {
            return mail.moveImap(uid, elaborata);
        }

        public bool cancellaMailImap()
        {
            return mail.cancellaMailImap();
        }


        public bool provaConnessione(DocsPaVO.amministrazione.OrgRegistro.MailRegistro mailRegistro, out string errore, string tipoConnessione)
        {
            return mail.provaConnessione(mailRegistro, out errore, tipoConnessione);
        }

        public bool getMessagePec(int index)
        {
            return mail.getMessagePec(index);

        }

        public bool getMessagePec(string uidl)
        {
            return mail.getMessagePec(uidl);
        }

        public bool salvaMailInLocale(int indexMail, string pathFile, string NomeDellaMail)
        {
            return mail.salvaMailInLocale(indexMail, pathFile, NomeDellaMail);
        }

        public bool salvaMailInLocale(string uidl, string pathFile, string NomeDellaMail)
        {
            return mail.salvaMailInLocale(uidl, pathFile, NomeDellaMail);
        }

        public string getBodyFromMail(string email)
        {
            return mail.getBodyFromMail(email);
        }

        #endregion
    }

    public class SvrMailbox : IMailConnector
    {
        private IMailConnector mail;
        public MailProvider provider = MailProvider.Chilkat;
        private static ILog logger = LogManager.GetLogger(typeof(SvrMailbox));

        private void initaliazeProvider()
        {
            string ProviderStr = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_MAIL_PROVIDER");
            if (String.IsNullOrEmpty(ProviderStr))
                ProviderStr = "c";

            if (ProviderStr.StartsWith("c"))
                provider = MailProvider.Chilkat;
            else if (ProviderStr.StartsWith("m"))
                provider = MailProvider.MailsystemNet;

            logger.Debug("Instanziato provider di posta :" + provider.ToString());
        }


        public SvrMailbox(string sServer, string sUser, string sPassword, string sPort, string sWorkingDir, 
                            CMClientType clientType, string SmtpSsl, string PopSsl, string smtpSTA, string regId, 
                            string Mailbox = "", string Mailelaborate = "", string MailNonElaborate = "")
        {
            initaliazeProvider();
            if (provider == MailProvider.MailsystemNet)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_MSNet(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, PopSsl, smtpSTA);
            if (provider == MailProvider.Chilkat)
            {
                if (clientType == CMClientType.POP)
                    mail = new BusinessLogic.Interoperabilità.ChilkatEmailComponent(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, PopSsl, smtpSTA, regId);
                else if (clientType == CMClientType.IMAP)
                {
                    mail = new BusinessLogic.Interoperabilità.ChilkatEmailComponent(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, PopSsl, smtpSTA, Mailbox, Mailelaborate, MailNonElaborate, regId);
                }
            }
        }

        public SvrMailbox(string sServer, string sUser, string sPassword, string sPort, string sWorkingDir, CMClientType clientType, string SmtpSsl, string smtpSTA)
        {
            initaliazeProvider();
            if (provider == MailProvider.MailsystemNet)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_MSNet(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, smtpSTA);
            if (provider == MailProvider.Chilkat)
                mail = new BusinessLogic.Interoperabilità.SvrPosta_CK(sServer, sUser, sPassword, sPort, sWorkingDir, clientType, SmtpSsl, smtpSTA);

        }

        #region IMailConnector Members

        public CMMsg getMessage(int index)
        {
            return mail.getMessage(index);
        }

        public CMMsg getMessage(byte[] email)
        {
            return mail.getMessage(email);
        }

        public CMMsg getMessage(string uidl)
        {
            return mail.getMessage(uidl);
        }

        public string[] getUidls()
        {
            return mail.getUidls();
        }

        public void deleteSingleMessage(string uidl)
        {
            mail.deleteSingleMessage(uidl);
        }

        public void deleteSingleMessage(int i)
        {
            mail.deleteSingleMessage(i);

        }

        public void connect()
        {
            mail.connect();
        }

        public void disconnect()
        {
            mail.disconnect();
        }

        public int messageCount()
        {
            return mail.messageCount();
        }

        public void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, CMMailHeaders[] headers, out string outError)
        {
            mail.sendMail(sFrom, sTo, sCC, sBCC, sSubject, sBody, format, attachments,headers, out outError);
        }
      
        public void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, out string outError)
        {
            mail.sendMail(sFrom, sTo, sCC, sBCC, sSubject, sBody, format, attachments, out outError);
        }

        public void sendMail(string sFrom, string sTo, string sSubject, string sBody, CMAttachment[] attachments)
        {
            mail.sendMail(sFrom, sTo, sSubject, sBody, attachments);
        }

        public void sendMail(string sFrom, string sTo, string sSubject, string sBody)
        {
            mail.sendMail(sFrom, sTo, sSubject, sBody);
        }
        
        public bool moveImap(int index, bool elaborata)
        {
            return mail.moveImap(index, elaborata);
        }

        public bool moveImap(string uid, bool elaborata)
        {
            return mail.moveImap(uid, elaborata);
        }

        public bool cancellaMailImap()
        {
            return mail.cancellaMailImap();
        }

        public bool provaConnessione(DocsPaVO.amministrazione.OrgRegistro.MailRegistro mailRegistro, out string errore, string tipoConnessione)
        {
            return mail.provaConnessione(mailRegistro, out errore, tipoConnessione);
        }

        public bool getMessagePec(int index)
        {
            return mail.getMessagePec(index);

        }

        public bool getMessagePec(string uidl)
        {
            return mail.getMessagePec(uidl);
        }

        public bool salvaMailInLocale(int indexMail, string pathFile, string NomeDellaMail)
        {
            return mail.salvaMailInLocale(indexMail, pathFile, NomeDellaMail);
        }

        public bool salvaMailInLocale(string uidl, string pathFile, string NomeDellaMail)
        {
            return mail.salvaMailInLocale(uidl, pathFile, NomeDellaMail);
        }

        public string getBodyFromMail(string email)
        {
            return mail.getBodyFromMail(email);
        }

        #endregion
    }

    // Client Type
    public enum CMClientType
    { POP, SMTP, IMAP }

    // Mail format
    public enum CMMailFormat
    { Text, HTML }

    public class CMMailHeaders
    {
        public string header;
        public string value;
    }

    #region Attachments

    // CMAttachment
    public class CMAttachment
    {
        #region Dichiarazioni

        public string name;
        public string alterntiveName;
        public string contentType;
        public string sourceFile;
        public byte[] _data;
        public string _method;

        #endregion

        #region Costruttori

        public CMAttachment()
        {
            _data = null;
        }

        public CMAttachment(string FileName, string ContentType, byte[] data, string method)
        {
            name = FileName;
            contentType = ContentType;
            sourceFile = null;
            _data = data;
            _method = method;
        }

        public CMAttachment(string FileName, string ContentType, byte[] data)
        {
            this.name = FileName;
            this.contentType = ContentType;
            this.sourceFile = null;
            this._data = data;
        }

        public CMAttachment(string FileName, string ContentType, string SourceFile)
        {
            this.name = FileName;
            this.contentType = ContentType;
            this.sourceFile = FileName;
            this._data = get_file_data(SourceFile);
        }

        private byte[] get_file_data(string path)
        {
            FileStream fs = File.OpenRead(path);
            byte[] res = new byte[fs.Length];
            fs.Read(res, 0, (int)fs.Length);
            fs.Close();
            return res;
        }


        #endregion

        #region Metodi pubbliici

        public void SaveToFile(string fileName)
        {
            try
            {

                if (File.Exists(fileName))
                {
                    fileName = GetNextFileName(fileName);
                    alterntiveName = Path.GetFileName(fileName);
                }

                StreamWriter fo = new StreamWriter(fileName, false, System.Text.ASCIIEncoding.Default);
                try
                {
                    fo.BaseStream.Write(_data, 0, (int)_data.Length);
                    fo.Flush();
                }
                finally
                {
                    fo.Close();
                }
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Non è stato possibile salvare il contenuto dell'attachment [{0}]. {1}", this.name, exc.Message));
            }
        }


        #endregion



        public static string GetNextFileName(string baseFileName)
        {
            var result = baseFileName;
            var fileNamePart = System.IO.Path.GetFileNameWithoutExtension(baseFileName);
            var extensionPart = System.IO.Path.GetExtension(baseFileName);
            var currentCount = 0;
            string path = Path.GetDirectoryName(baseFileName);

            while (System.IO.File.Exists(result))
            {
                result = string.Format("{0}_{1}{2}",
                                       fileNamePart, ++currentCount, extensionPart);
                result = Path.Combine(path, result);

            }

            return result;
        }

    }


    // CMAttachments
    public class CMAttachments : ArrayList
    {
        public new CMAttachment this[int index]
        {
            get { return (CMAttachment)base[index]; }
        }
    }


    #endregion

    #region Recipients

    // CMRecipient
    public class CMRecipient
    {
        public string mail;
        public string name;

        public CMRecipient()
        { }

        public CMRecipient(string mail, string name)
        {
            this.mail = mail;
            this.name = name;
        }


    }

    // CMRecipients
    public class CMRecipients : ArrayList
    {
        public new CMRecipient this[int index]
        {
            get { return (CMRecipient)base[index]; }
        }
    }


    #endregion

    public class CMMsg
    {
        private static ILog logger = LogManager.GetLogger(typeof(CMMsg));
        #region Dichiarazioni

        public string from = "";
        public string hideRecipients = "";
        public string subject = "";
        public DateTime date;
        public string body = null;
        public string HTMLBody = null;

        public Hashtable headers = new Hashtable(
            new CaseInsensitiveHashCodeProvider(),
            new CaseInsensitiveComparer());

        public CMAttachments attachments = new CMAttachments();
        public CMRecipients recipients = new CMRecipients();

        public string errorMessage = "";
        #endregion

        #region Costruttori

        public CMMsg()
        { }

        #endregion

        #region PEC

        /// <summary>
        /// verifica se la mail è di tipo pec X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPEC()
        {
            return (headers["X-Ricevuta"] != null || headers["X-Trasporto"] != null || headers["X-TipoRicevuta"] != null);
        }

        
        public bool Pec()
        {
            return (headers["X-Ricevuta"] == null) || ((headers["X-Trasporto"] == null));
        }

        /// <summary>
        /// ricevuta di accettazione del messaggio X-Ricevuta
        /// </summary>
        /// <returns>bool</returns>
        public bool isPECAcceptNotify()
        {
            if (headers["X-Ricevuta"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "accettazione");
            else
                return false;
        }

        /// <summary>
        /// verifica l'avvenuta consegna della mail X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPECDeliveredNotify()
        {
            if (headers["X-Ricevuta"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "avvenuta-consegna");
            else
                return false;
        }

        /// <summary>
        /// verifica l'avvenuta consegna della mail in formato breve X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPECDeliveredNotifyShort()
        {
            if (headers["X-Ricevuta"] != null && headers["X-TipoRicevuta"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "avvenuta-consegna"
                    && (headers["X-TipoRicevuta"].ToString().ToLower() == "breve"
                       || headers["X-TipoRicevuta"].ToString().ToLower() == "sintetica"));
            else
                return false;
        }

        /// <summary>
        /// ricevuta di non accettazione della mail per mancato superamento dei controlli formali X-Ricevuta
        /// </summary>
        /// <returns>bool</returns>
        public bool isPECNonAcceptNotify()
        {
            if (headers["X-Ricevuta"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "non-accettazione");
            else
                return false;
        }


        /// <summary>
        /// ricevuta di errore nella consegna della mail X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPECErrorDeliveredNotify()
        {
            if (headers["X-Ricevuta"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "errore-consegna");
            else
                return false;
        }
        /// <summary>
        /// preavviso di mancata consegna della mail che viene inviato nelle 12 ore successive all'invio del messaggio X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPECErrorPreavvisoDeliveredNotify()
        {
            if (headers["X-Ricevuta"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "preavviso-errore-consegna");
            else
                return false;
        }
        /// <summary>
        /// verifica se la è stata presa in carico tra i gestore di posta certificata X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPECPresaInCarico()
        {
            if (headers["X-Ricevuta"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "presa-in-carico");
            else
                return false;
        }

        /// <summary>
        /// verifica se la mail inviata conteneva un virus al suo interno X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPECContainVirus()
        {
            if (headers["X-Ricevuta"] != null && headers["X-VerificaSicurezza"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "non-accettazione"
                    && headers["X-VerificaSicurezza"].ToString().ToLower() == "errore");
            else
                return false;
        }

        /// <summary>
        /// verifica se all'interno della mail è stato trovato un virus X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPECAlertVirus()
        {
            if (headers["X-Ricevuta"] != null && headers["X-Mittente"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "rilevazione-virus");
            else
                return false;
        }

        /// <summary>
        /// Verifica se la mail non è stata consegnata perhce conteneva un virus X-Ricevuta
        /// </summary>
        /// <returns></returns>
        public bool isPECErrorDeliveredNotifyByVirus()
        {
            if (headers["X-Ricevuta"] != null && headers["X-VerificaSicurezza"] != null)
                return (headers["X-Ricevuta"].ToString().ToLower() == "errore-consegna" &&
                    headers["X-VerificaSicurezza"].ToString().ToLower() == "errore");
            else
                return false;
        }

        /// <summary>
        /// verifica che mail certificata  è stata consegnata al/ai destinatari/io correttamente X-Trasporto
        /// </summary>
        /// <returns></returns>
        public bool isPECDelivered()
        {
            if (headers["X-Trasporto"] != null)
                return (headers["X-Trasporto"].ToString().ToLower() == "posta-certificata");
            else
                return false;
        }

        /// <summary>
        /// il mittente in from non è di tipo pec
        /// </summary>
        /// <returns></returns>
        public bool isFromNonPEC()
        {
            return isPECError() &&
                this.subject.Trim().ToUpper().StartsWith("ANOMALIA MESSAGGIO") &&
                ((this.body.IndexOf("dati non sono stati certificati") >= 0) ||
                (this.HTMLBody.IndexOf("dati non sono stati certificati") >= 0));
        }

        /// <summary>
        /// Verifica se la firma è autentica
        /// </summary>
        /// <returns></returns>
        public bool isSignatureMissingOrNotAuthentic()
        {
            return isFromNonPEC() &&
                ((this.body.IndexOf("Firma non presente o non autentica") >= 0) ||
                (this.HTMLBody.IndexOf("Firma non presente o non autentica") >= 0));
        }

        /// <summary>
        /// verifica se la mail è stata riconosciuta come una mail di tipo pec X-Trasporto
        /// </summary>
        /// <returns></returns>
        public bool isPECError()
        {
            if (headers["X-Trasporto"] != null)
                return (headers["X-Trasporto"].ToString().ToLower() == "errore");
            else
                return false;
        }


        /// <summary>
        /// messaggio non ricevuto
        /// </summary>
        /// <returns></returns>
        public bool isDeliveryStatusNotification()
        {
            bool retvalue = false;

            if (headers["Content-Type"] != null)
            {
                if (headers["Content-Type"].ToString().IndexOf("delivery-status") >= 0)
                {
                    logger.Debug("Delivery Status Notification");
                    logger.Debug("Content-Type  " + headers["Content-Type"].ToString());
                    retvalue = true;
                }

            }
            //Campo x-dsn supportato da certi provider
            if (headers["X-dsn"] != null)
            {
                logger.Debug("Delivery Status Notification");
                retvalue = true;
            }

            return retvalue;
        }
        //Content-Type: Multipart/Report; report-type=delivery-status; 

        public static string findSmptErrorCode(string mimeEmail)
        {
            if (smtpCodes.Keys.Count == 0)
                popolasmtpcodes();

            string foundVal = null ;
            foreach (string value in smtpCodes.Keys)
            {

                if (mimeEmail.Contains(value))
                {
                    foundVal = value;
                    break;
                }
            }
            
            return foundVal;
        }

        public static Dictionary<string, string> smtpCodes = new Dictionary<string, string>();
        static void popolasmtpcodes()
        {
            smtpCodes.Clear();
            smtpCodes.Add("1.0.1", "Cannot open connection");
            smtpCodes.Add("1.1.1", "Connection refused");
            smtpCodes.Add("2.1.1", "System Status message o System Help Reply");
            smtpCodes.Add("2.1.4", "Help reply message");
            smtpCodes.Add("2.2.0", "Service is running");
            smtpCodes.Add("2.2.1", "The domain service is closing the transmission channel");
            smtpCodes.Add("2.5.0", "Requested mail action completed");
            smtpCodes.Add("2.5.1", "User not local will forward");
            smtpCodes.Add("2.5.2", "Cannot verify the user - the server will accept the message and attempt to deliver it");
            smtpCodes.Add("3.5.4", "Start mail input end with <CRLF> <CRLF>");
            smtpCodes.Add("4.2.0", "Timeout communication problem encountered during trasmission");
            smtpCodes.Add("4.2.1", "Service not available - the sending email program should try again later");
            smtpCodes.Add("4.2.2", "The recipient's mailbox is over its storage limit");
            smtpCodes.Add("4.3.1", "The recipient's mail server is experiencing a Disk Full condition");
            smtpCodes.Add("4.3.2", "The recipient's Exchange Server Incoming mail queue has been stopped");
            smtpCodes.Add("4.4.1", "The recipient's server is not responding");
            smtpCodes.Add("4.4.2", "The connection was dropped during trasmission");
            smtpCodes.Add("4.4.6", "The maximum hop count was exceeded for the message");
            smtpCodes.Add("4.4.7", "Your outgoing message timed out");
            smtpCodes.Add("4.4.9", "Routing error");
            smtpCodes.Add("4.5.0", "Requested action not taken - the mailbox was unavailable at the remote end");
            smtpCodes.Add("4.5.1", "Requested action aborted - local error in processing");
            smtpCodes.Add("4.5.2", "Request action not taken - insufficient storage");
            smtpCodes.Add("4.6.5", "Code page unavailable on the recipient server");
            smtpCodes.Add("4.7.1", "Local error - Please try again later");
            smtpCodes.Add("5.0.0", "Syntax error command not recognized");
            smtpCodes.Add("5.0.1", "Syntax error in parameters or arguments");
            smtpCodes.Add("5.0.2", "Command not implemented");
            smtpCodes.Add("5.0.3", "Bad sequence of commands");
            smtpCodes.Add("5.0.4", "Command parameter non implemented");
            smtpCodes.Add("5.1.0", "Bad email Address");
            smtpCodes.Add("5.1.1", "Bad email Address");
            smtpCodes.Add("5.1.2", "The host server for the recipient's domain name cannot be found");
            smtpCodes.Add("5.1.3", "Address type is incorrect");
            smtpCodes.Add("5.2.3", "The recipient's mailbox cannot receive messages so big");
            smtpCodes.Add("5.3.0", "Authentication is required");
            smtpCodes.Add("5.4.1", "Recipient Address rejected - Access denied");
            smtpCodes.Add("5.5.0", "Requested actions not taken as the mailbox is unavailable");
            smtpCodes.Add("5.5.1", "User not local or invalid Address - relay denied");
            smtpCodes.Add("5.5.2", "Requested mail actions aborted - exceeded storage allocation");
            smtpCodes.Add("5.5.3", "Requested action not taken - mailbox name invalid");
            smtpCodes.Add("5.5.4", "Transaction failed");
            smtpCodes.Add("5.7.1", "Delivery not authorized, message refused");

        }

        /// <summary>
        /// Decodes an SMTP error code
        /// </summary>
        /// <param name="code">smtp error code string</param>
        /// <returns>smtp error code meaning</returns>
        public static string decodeSmtpErrorCode(string code)
        {
            string retval=null;
            if (smtpCodes.Keys.Count == 0)
                popolasmtpcodes();

            if (smtpCodes.ContainsKey (code))
                retval= smtpCodes [code];

            return retval;
        }
        #endregion

        #region Metodi pubblici

        public string getHeader(string headerName)
        {
            if (headers[headerName] != null)
                return headers[headerName].ToString();
            else
                return null;
        }

        public bool hasAttachments()
        {
            return (attachments.Count > 0);
        }

        /// <summary>
        /// estrae la data, ora e fuso orario di ricezione del server della mail
        /// </summary>
        /// <returns>string contente la data, ora e fuso orario di ricezione della mail</returns>
        public string DateReceivedMail()
        {
            string retvalue = string.Empty;

            if (headers["received"] != null)
            {
                if (!string.IsNullOrEmpty(headers["received"].ToString()))
                {
                    logger.Debug("Data di ricezione della mail: " + headers["received"].ToString());
                    retvalue = headers["received"].ToString();

                }

            }

            return retvalue;
        }

        /// <summary>
        /// estrae la data, ora e fuso orario di spedizione al server della mail
        /// </summary>
        /// <returns>string contente la data, ora e fuso orario di spedizione della mail</returns>
        public string DateSendMail()
        {
            string retvalue = string.Empty;

            if (headers["date"] != null)
            {
                if (!string.IsNullOrEmpty(headers["date"].ToString()))
                {
                    logger.Debug("data di spedizione della mail: " + headers["date"].ToString());
                    retvalue = headers["date"].ToString();
                }

            }

            return retvalue;
        }
        #endregion
    }

}
