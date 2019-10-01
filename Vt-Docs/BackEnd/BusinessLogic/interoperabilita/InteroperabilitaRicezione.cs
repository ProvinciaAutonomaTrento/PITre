using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Globalization;
using DocsPaUtils.Security;
using System.Collections.Generic;
using DocsPaVO.ProfilazioneDinamica;
using log4net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security.AccessControl;

namespace BusinessLogic.Interoperabilità
{
    /// <summary>
    /// </summary>
    public class InteroperabilitaRicezione
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaRicezione));
        //modifica

        private const string AVVENUTACONSEGNA = "avvenuta-consegna";

        //fine modifica

        /// <summary>
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="reg"></param>
        /// 
        //modifica del 8/7/2009
        protected static string id_registro = string.Empty;
        //fine modifica
        //modifica del 10/7/2009
        protected static int mailPec = 0;
        //fine modifica


        /// <summary>
        /// invia e ricevi 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="reg"></param>
        /// <param name="ut"></param>
        /// <param name="ruolo"></param>
        /// <param name="checkResponse"></param>
        /// <returns></returns>
        public static bool interopRiceviMethod(string serverName, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Utente ut, DocsPaVO.utente.Ruolo ruolo, out DocsPaVO.Interoperabilita.MailAccountCheckResponse checkResponse)
        {
            return interopRiceviMethod(serverName, reg, ut, ruolo, null, out checkResponse, false);
        }


        /// <summary>
        /// invia e ricevi con protocollazione automatica
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="reg"></param>
        /// <param name="ut"></param>
        /// <param name="ruolo"></param>
        /// <param name="checkResponse"></param>
        /// <returns></returns>
        public static bool interopRiceviMethodProtocollazione(string serverName, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Utente ut, DocsPaVO.utente.Ruolo ruolo, IInteropSchedaDocHandler handler, out DocsPaVO.Interoperabilita.MailAccountCheckResponse checkResponse)
        {
            return interopRiceviMethod(serverName, reg, ut, ruolo, handler, out checkResponse, true);
        }

        private static bool interopRiceviMethod(string serverName, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Utente ut, DocsPaVO.utente.Ruolo ruolo, IInteropSchedaDocHandler handler, out DocsPaVO.Interoperabilita.MailAccountCheckResponse checkResponse, bool conProtocollazione)
        {
            bool retValue = false;
            checkResponse = null;

            if (conProtocollazione)
                logger.Debug("START : DocsPAWS > Interoperabilita > InteroperabilitaRicezione > interopRiceviMethodProtocollazione");
            else
                logger.Debug("START : DocsPAWS > Interoperabilita > InteroperabilitaRicezione > interopRiceviMethod");
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            //DataSet dataSet=new DataSet();
            DataSet dataSet;
            System.Threading.Mutex mutex = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getCampiReg(out dataSet, reg);

                if (dataSet.Tables["REGISTRO"].Rows.Count > 0)
                {
                    DataRow regRow = dataSet.Tables["REGISTRO"].Rows[0];
                    //creazione del logger
                    //string basePath=ConfigurationManager.AppSettings["LOG_PATH"]+"\\Interoperabilita";
                    string basePath = ConfigurationManager.AppSettings["LOG_PATH"];
                    basePath = basePath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                    basePath = basePath + "\\Interoperabilita" + "\\" + regRow["VAR_CODICE"].ToString() + "\\ricezione";

                    //controllo delle mail
                    //old: il ruolo deve essere quello collegato a DocsPa e non ruolo[0]
                    //DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)ut.ruoli[0];
                    DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
                    infoUtente.dst = ut.dst;
                    infoUtente.idAmministrazione = reg.idAmministrazione;
                    infoUtente.idCorrGlobali = ruolo.systemId;
                    infoUtente.idGruppo = ruolo.idGruppo;
                    infoUtente.idPeople = ut.idPeople;
                    infoUtente.userId = ut.userId;

                    //modifica del 10/07/2009
                    if (regRow["VAR_SOLO_MAIL_PEC"].ToString() != "")
                        mailPec = Int32.Parse(regRow["VAR_SOLO_MAIL_PEC"].ToString());
                    else
                        mailPec = 0;
                    //modifica del 10/07/2009

                    int porta = 0;
                    id_registro = regRow["SYSTEM_ID"].ToString();
                    //modifica
                    if (regRow["VAR_TIPO_CONNESSIONE"] != null)
                    {
                        logger.Debug("chiamo CreateMutex Per interop posta");
                        // Creazione o reperimento del mutex
                        mutex = CreateMutex(regRow["VAR_USER_MAIL"].ToString());
                        logger.DebugFormat("Mutex Interop Chiamato");
                        mutex.WaitOne();
                            
                        if (regRow["VAR_TIPO_CONNESSIONE"].Equals("POP"))
                        {
                            if (regRow["NUM_PORTA_POP"] != null && !regRow["NUM_PORTA_POP"].ToString().Equals(""))
                            {
                                logger.Debug("porta non nulla");

                                porta = Int32.Parse(regRow["NUM_PORTA_POP"].ToString());
                            }
                            else
                            {
                                logger.Debug("porta nulla");
                                porta = 110;
                            }
                            //Crypter.Decode(regRow["VAR_PWD_MAIL"].ToString(), regRow["VAR_USER_MAIL"].ToString()),
                            if (conProtocollazione)
                                checkResponse = checkMailConProtocollazione(serverName, regRow["VAR_EMAIL_REGISTRO"].ToString(), regRow["VAR_USER_MAIL"].ToString(), Crypter.Decode(regRow["VAR_PWD_MAIL"].ToString(), regRow["VAR_USER_MAIL"].ToString()), regRow["VAR_SERVER_POP"].ToString(), porta, reg, infoUtente, ruolo, handler, regRow["CHA_SMTP_SSL"].ToString(), regRow["CHA_POP_SSL"].ToString(), regRow["CHA_SMTP_STA"].ToString(), regRow["VAR_TIPO_CONNESSIONE"].ToString(), regRow["VAR_INBOX_IMAP"].ToString(), regRow["VAR_BOX_MAIL_ELABORATE"].ToString(), regRow["VAR_MAIL_NON_ELABORATE"].ToString());
                            else
                                checkResponse = checkMail(serverName, regRow["VAR_EMAIL_REGISTRO"].ToString(), regRow["VAR_USER_MAIL"].ToString(), Crypter.Decode(regRow["VAR_PWD_MAIL"].ToString(), regRow["VAR_USER_MAIL"].ToString()), regRow["VAR_SERVER_POP"].ToString(), porta, reg, infoUtente, ruolo, regRow["CHA_SMTP_SSL"].ToString(), regRow["CHA_POP_SSL"].ToString(), regRow["CHA_SMTP_STA"].ToString(), regRow["VAR_TIPO_CONNESSIONE"].ToString(), regRow["VAR_INBOX_IMAP"].ToString(), regRow["VAR_BOX_MAIL_ELABORATE"].ToString(), regRow["VAR_MAIL_NON_ELABORATE"].ToString());
                        }
                        else
                        {
                            if (regRow["NUM_PORTA_IMAP"] != null && !regRow["NUM_PORTA_IMAP"].ToString().Equals(""))
                            {
                                logger.Debug("porta non nulla");

                                porta = Int32.Parse(regRow["NUM_PORTA_IMAP"].ToString());
                            }
                            else
                            {
                                logger.Debug("porta nulla");
                                porta = 993;
                            }

                            //Crypter.Decode(regRow["VAR_PWD_MAIL"].ToString(),
                                                      
                            if (conProtocollazione)
                                checkResponse = checkMailConProtocollazione(serverName, regRow["VAR_EMAIL_REGISTRO"].ToString(), regRow["VAR_USER_MAIL"].ToString(), Crypter.Decode(regRow["VAR_PWD_MAIL"].ToString(), regRow["VAR_USER_MAIL"].ToString()), regRow["VAR_SERVER_IMAP"].ToString(), porta, reg, infoUtente, ruolo, handler, regRow["CHA_SMTP_SSL"].ToString(), regRow["CHA_IMAP_SSL"].ToString(), regRow["CHA_SMTP_STA"].ToString(), regRow["VAR_TIPO_CONNESSIONE"].ToString(), regRow["VAR_INBOX_IMAP"].ToString(), regRow["VAR_BOX_MAIL_ELABORATE"].ToString(), regRow["VAR_MAIL_NON_ELABORATE"].ToString());
                            else
                                checkResponse = checkMail(serverName, regRow["VAR_EMAIL_REGISTRO"].ToString(), regRow["VAR_USER_MAIL"].ToString(), Crypter.Decode(regRow["VAR_PWD_MAIL"].ToString(), regRow["VAR_USER_MAIL"].ToString()), regRow["VAR_SERVER_IMAP"].ToString(), porta, reg, infoUtente, ruolo, regRow["CHA_SMTP_SSL"].ToString(), regRow["CHA_IMAP_SSL"].ToString(), regRow["CHA_SMTP_STA"].ToString(), regRow["VAR_TIPO_CONNESSIONE"].ToString(), regRow["VAR_INBOX_IMAP"].ToString(), regRow["VAR_BOX_MAIL_ELABORATE"].ToString(), regRow["VAR_MAIL_NON_ELABORATE"].ToString());
                           
                        }
                        //Rilascio il mutex
                        if (mutex != null)
                        {
                            mutex.ReleaseMutex();
                            mutex.Close();
                            mutex = null;
                        }
                    }
                    else
                        checkResponse.ErrorMessage = "tipo di connessione imap o pop non configurato per questo registro";
                //modifica

                    //Interoperabilità.Log.LogCheckMail logCheckMail = new BusinessLogic.Interoperabilità.Log.LogCheckMail();// interoperabilità.Log.LogCheckMail();
                    //logCheckMail.LogElement(reg, checkResponse);
                }

            }
            catch (Exception e)
            {
                logger.Error("Errore nella gestione dell'interoperabilità. (interopRiceviMethod)", e);
                if (checkResponse == null)
                {
                    checkResponse = new DocsPaVO.Interoperabilita.MailAccountCheckResponse();
                    checkResponse.ErrorMessage = e.Message.ToString();
                }
                throw e;
            }
            finally
            {
                //In ogni caso rilascio il mutex se non fosse già stato rilasciato
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                    mutex = null;
                }
            }
            logger.Debug("END : DocsPAWS > Interoperabilita > InteroperabilitaRicezione > interopRiceviMethod");

            return retValue;
        }

        private static Mutex CreateMutex(string IDMailBox)
        {
            bool createdNew;
            string mutexName = string.Format("DPACheckMail-{0}", IDMailBox);
            logger.DebugFormat("CreateMutex Name {0}", mutexName);
            Mutex m = new Mutex(false, mutexName, out createdNew);
            
            if (createdNew)
            {
                
                // the mutex was created and this thread is the owner 
                // (we don't need to call WaitOne)
            }
            else
            {
                m.WaitOne();
                logger.DebugFormat("Mutex {0} Devo aspettare un processo che termini in waitone",mutexName);
                // the mutex already exists, we hold a reference to the mutex, but we
                // don't own it (we need to call WaitOne).
            }
            return m;
        }

        /// <summary>
        /// Reperimento o creazione del Mutext per l'istanza corrente
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static Mutex GetOrCreateMutex(string IDMailBox)
        {
            string mutexName = string.Format("DPACheckMail-{0}", IDMailBox);
            logger.DebugFormat("GetOrCreateMutex Name {0}", mutexName);
            Mutex m = null;
            bool doesNotExist = false;
            bool unauthorized = false;

            // The value of this variable is set by the mutex
            // constructor. It is true if the named system mutex was
            // created, and false if the named mutex already existed.
            //
            bool mutexWasCreated = false;

            // Attempt to open the named mutex.
            try
            {
                // Open the mutex with (MutexRights.Synchronize |
                // MutexRights.Modify), to enter and release the
                // named mutex.
                //
                m = Mutex.OpenExisting(mutexName);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                logger.ErrorFormat("Mutex does not exist.");
                doesNotExist = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.ErrorFormat("Unauthorized access: {0}", ex.Message);
                unauthorized = true;
            }

            // There are three cases: (1) The mutex does not exist.
            // (2) The mutex exists, but the current user doesn't 
            // have access. (3) The mutex exists and the user has
            // access.
            //
            if (doesNotExist)
            {
                // The mutex does not exist, so create it.

                // Create an access control list (ACL) that denies the
                // current user the right to enter or release the 
                // mutex, but allows the right to read and change
                // security information for the mutex.
                //
                string user = Environment.UserDomainName + "\\" + Environment.UserName;
                MutexSecurity mSec = new MutexSecurity();

                MutexAccessRule rule = new MutexAccessRule(user,
                    MutexRights.Synchronize | MutexRights.Modify,
                    AccessControlType.Deny);
                mSec.AddAccessRule(rule);

                rule = new MutexAccessRule(user,
                    MutexRights.ReadPermissions | MutexRights.ChangePermissions,
                    AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                // Create a Mutex object that represents the system
                // mutex named by the constant 'mutexName', with
                // initial ownership for this thread, and with the
                // specified security access. The Boolean value that 
                // indicates creation of the underlying system object
                // is placed in mutexWasCreated.
                //
                m = new Mutex(true, mutexName, out mutexWasCreated, mSec);

                // If the named system mutex was created, it can be
                // used by the current instance of this program, even 
                // though the current user is denied access. The current
                // program owns the mutex. Otherwise, exit the program.
                // 
                if (mutexWasCreated)
                {
                    logger.DebugFormat("Created the mutex.");
                }
                else
                {
                    logger.DebugFormat("Unable to create the mutex.");
                    throw new ApplicationException("Unable to create the mutex.");
                }

            }
            else if (unauthorized)
            {
                // Open the mutex to read and change the access control
                // security. The access control security defined above
                // allows the current user to do this.
                //
                try
                {
                    m = Mutex.OpenExisting(mutexName,
                        MutexRights.ReadPermissions | MutexRights.ChangePermissions);

                    // Get the current ACL. This requires 
                    // MutexRights.ReadPermissions.
                    MutexSecurity mSec = m.GetAccessControl();

                    string user = Environment.UserDomainName + "\\"
                        + Environment.UserName;

                    // First, the rule that denied the current user 
                    // the right to enter and release the mutex must
                    // be removed.
                    MutexAccessRule rule = new MutexAccessRule(user,
                         MutexRights.Synchronize | MutexRights.Modify,
                         AccessControlType.Deny);
                    mSec.RemoveAccessRule(rule);

                    // Now grant the user the correct rights.
                    // 
                    rule = new MutexAccessRule(user,
                        MutexRights.Synchronize | MutexRights.Modify,
                        AccessControlType.Allow);
                    mSec.AddAccessRule(rule);

                    // Update the ACL. This requires
                    // MutexRights.ChangePermissions.
                    m.SetAccessControl(mSec);

                    logger.DebugFormat("Updated mutex security.");

                    // Open the mutex with (MutexRights.Synchronize 
                    // | MutexRights.Modify), the rights required to
                    // enter and release the mutex.
                    //
                    m = Mutex.OpenExisting(mutexName);

                }
                catch (UnauthorizedAccessException ex)
                {
                    logger.ErrorFormat("Unable to change permissions: {0}", ex.Message);
                    throw new ApplicationException(string.Format("Unable to change permissions: {0}", ex.Message));
                }
            }

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="mailAddress"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="reg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="SmtpSsl"></param>
        /// <param name="PopSsl"></param>
        /// <param name="smtpSTA"></param>
        /// <param name="tipoPosta"></param>
        /// <param name="mailbox"></param>
        /// <param name="mailelaborate"></param>
        /// <param name="mailNonElaborate"></param>
        /// <returns></returns>
        protected static DocsPaVO.Interoperabilita.MailAccountCheckResponse checkMail(string serverName, string mailAddress, string userId, string password, string server, int port, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string SmtpSsl, string PopSsl, string smtpSTA, string tipoPosta, string mailbox, string mailelaborate, string mailNonElaborate)
        {
            return checkMail(serverName, mailAddress, userId, password, server, port, reg, infoUtente, ruolo, null, SmtpSsl, PopSsl, smtpSTA, tipoPosta, mailbox, mailelaborate, mailNonElaborate, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="mailAddress"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="reg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="ruolo"></param>
        /// <param name="handler"></param>
        /// <param name="SmtpSsl"></param>
        /// <param name="PopSsl"></param>
        /// <param name="smtpSTA"></param>
        /// <param name="tipoPosta"></param>
        /// <param name="mailbox"></param>
        /// <param name="mailelaborate"></param>
        /// <param name="mailNonElaborate"></param>
        /// <returns></returns>
        protected static DocsPaVO.Interoperabilita.MailAccountCheckResponse checkMailConProtocollazione(string serverName, string mailAddress, string userId, string password, string server, int port, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, IInteropSchedaDocHandler handler, string SmtpSsl, string PopSsl, string smtpSTA, string tipoPosta, string mailbox, string mailelaborate, string mailNonElaborate)
        {
            return checkMail(serverName, mailAddress, userId, password, server, port, reg, infoUtente, ruolo, handler, SmtpSsl, PopSsl, smtpSTA, tipoPosta, mailbox, mailelaborate, mailNonElaborate, true);
        }


        private static DocsPaVO.Interoperabilita.MailAccountCheckResponse checkMail(string serverName, string mailAddress, string userId, string password, string server, int port, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, IInteropSchedaDocHandler handler, string SmtpSsl, string PopSsl, string smtpSTA, string tipoPosta, string mailbox, string mailelaborate, string mailNonElaborate, bool conProtocollazione)
        {
            DocsPaVO.Interoperabilita.MailAccountCheckResponse retValue =
              new DocsPaVO.Interoperabilita.MailAccountCheckResponse(userId, server, mailAddress, (reg.codRegistro + " - " + reg.descrizione));

            //modifica
            Interoperabilità.SvrPosta svr = null;
            if (tipoPosta.Equals("POP"))
            {
                svr = new Interoperabilità.SvrPosta
                    (server, userId, password, port.ToString(),
                    System.IO.Path.GetTempPath(),
                BusinessLogic.Interoperabilità.CMClientType.POP, SmtpSsl, PopSsl, smtpSTA);
            }
            else
            {
                svr = new Interoperabilità.SvrPosta
                    (server, userId, password, port.ToString(),
                    System.IO.Path.GetTempPath(),
                BusinessLogic.Interoperabilità.CMClientType.IMAP, SmtpSsl, PopSsl, smtpSTA, mailbox, mailelaborate, mailNonElaborate);
            }
            //modifica

            try
            {
                svr.connect();
            }
            catch (Exception e)
            {
                logger.Error("Connessione al server " + server + " fallita. " + e.Message);

                retValue.ErrorMessage = e.Message;

                throw new Exception("Connessione al server " + server + " fallita. " + e.Message);
            }

            try
            {
                string err = string.Empty;
                string docnumber = string.Empty;
                DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed = null;

                logger.Debug("Connessione al server " + server + " effettuata");

                bool rimuovi_mail_elaborata = InteroperabilitaUtils.Cfg_EliminazioneMailElaborate(infoUtente);
                bool elabora_ogni_mail = InteroperabilitaUtils.Cfg_ElaborazionePostaOrdinaria;
                int num_mess = svr.messageCount();
                logger.Debug("Numero messaggi trovati: " + num_mess);


                bool gestioneRicevutePec = false;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GESTIONE_RICEVUTE_PEC"]) &&
                    bool.Parse(ConfigurationManager.AppSettings["GESTIONE_RICEVUTE_PEC"]))
                    gestioneRicevutePec = true;


                bool salvaAllegatiPec = false;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SALVA_RICEVUTE_PEC"]) &&
                    bool.Parse(ConfigurationManager.AppSettings["SALVA_RICEVUTE_PEC"]))
                    salvaAllegatiPec = true;
                logger.Debug("gestioneRicevutePec: " + gestioneRicevutePec + " salvaAllegatiPec: " + salvaAllegatiPec);
                for (int i = 1; i <= num_mess; i++)
                {
                    bool protocolla_con_segnatura = false;
                    bool conferma_ricezione = false;
                    bool eccezione = false;
                    bool generataEccezione = false;
                    bool addToList = true;
                    string docNumMailDSN = null;
                    bool notifica_annullamento = false;
                    bool msg_mail_elaborata = false;
                    bool erroreNelMessaggio = false;
                    bool spostato = false;
                    Interoperabilità.CMMsg mc = new Interoperabilità.CMMsg();
                    bool checkId = false;
                    string messageId = string.Empty;
                    string regId = reg.systemId.ToString();
                    bool daticert = false;
                    List<string> listaAllegati = new List<string>();
                    bool ricevutaPec = false;
                    string moreError = string.Empty;

                    //Andrea De Marco - Gestione Eccezione PEC
                    bool controlloBloccante = false;
                    //End Andrea De Marco

                    try
                    {
                        mailProcessed = new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed();
                        mc = svr.getMessage(i);
                        //byte[] mymail = System.IO.File.ReadAllBytes("c:\\message.eml");
                        //mc = svr.getMessage(mymail);
                        for (int j = 0; j < mc.attachments.Count; j++)
                        {
                            if (mc.attachments[j].name.LastIndexOf(".") != -1)
                            {
                                string nomeAttach = mc.attachments[j].name;
                                string nomeAttach1 = nomeAttach.Substring(0, nomeAttach.LastIndexOf("."));
                                string nomeAttach2 = nomeAttach.Substring(nomeAttach.LastIndexOf(".") + 1);
                                if (nomeAttach2.Contains(" "))
                                {
                                    nomeAttach2 = nomeAttach2.Replace(" ", "");
                                    nomeAttach = nomeAttach1 + "." + nomeAttach2;
                                    mc.attachments[j].name = nomeAttach;
                                }
                            }
                            else //non è presente estensione
                            {
                                logger.Error("Attenzione l'allegato NON ha estensione, cerco di recuperarla dal content type");
                                string ext = Interoperabilità.MimeMapper.GetExtensionFromMime(mc.attachments[j].contentType);
                                logger.ErrorFormat("ContentType : {0}  -> Ext {1} ",mc.attachments[j].contentType, ext);
                                mc.attachments[j].name += ext;
                                
                            }

                        }
                                                   
                        if (mc != null)
                        {
                            mailProcessed.PecXRicevuta = BusinessLogic.interoperabilita.InteroperabilitaManager.getTipoRicevutaPec(mc);
                            //Gestione DSN controllo se DSN e in caso recupero il docNumer dal subject
                            if (mailProcessed.PecXRicevuta == DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.Delivery_Status_Notification)
                            {
                                if (mc.subject.Contains("#") || gestioneRicevutePec)
                                {
                                    docNumMailDSN = extractDocNumberFromSubject(mc.subject);
                                }
                            }

                            //Gestione Accettazione o Consegna della Eccezioni
                            if (mc.subject.Contains(": Ricevuta Eccezione"))
                            {
                                addToList = false;
                                msg_mail_elaborata = true;
                                protocolla_con_segnatura = false;
                            }

                            try
                            {
                                messageId = mc.getHeader("message-id").Trim();
                                logger.Debug("messageId: " + messageId);
                                mailProcessed.MailID = messageId;
                                mailProcessed.Date = mc.date;
                                mailProcessed.From = mc.from;
                                foreach (CMRecipient recipient in mc.recipients)
                                    mailProcessed.Recipients.Add(recipient.mail);

                                //modifica
                                if (mc.subject.Contains("#")
                                    || gestioneRicevutePec)
                                {
                                    string[] split = mc.subject.Split('#');
                                    if (split.Length > 0)
                                        mc.subject = split[0];
                                }
                                //fine modifica

                                mailProcessed.Subject = mc.subject;
                                mailProcessed.Subject = mailProcessed.Subject.Replace("<", "&lt;");
                                mailProcessed.Subject = mailProcessed.Subject.Replace(">", "&gt;");
                            }
                            catch (Exception e)
                            {
                                if (!erroreNelMessaggio)
                                    erroreNelMessaggio = true;
                                logger.Error("errore nel recupero delle informazioni della mail - errore: " + e.Message);
                            }

                            string datasend = mc.DateSendMail();
                            // string datasend = mc.date.ToShortTimeString();
                            if (!string.IsNullOrEmpty(datasend))
                            {
                                try
                                {
                                    int valSub = datasend.LastIndexOf("(");
                                    if (valSub != -1)
                                    {
                                        datasend = DateTime.Parse(datasend.Substring(0, datasend.LastIndexOf("("))).ToString("dd/MM/yyyy HH:mm:ss");
                                    }
                                    else
                                    {
                                        datasend = DateTime.Parse(datasend).ToString("dd/MM/yyyy HH:mm:ss");
                                    }
                                }
                                catch
                                {
                                    logger.Debug("errore nel formato della data di spedizione delle mail");
                                }
                            }


                            string datareceived = string.Empty;
                            try
                            {
                                datareceived = mailProcessed.Date.ToString("dd/MM/yyyy HH:mm:ss");
                            }
                            catch
                            {

                                //fallback
                                datareceived = mc.DateReceivedMail();
                                if (!string.IsNullOrEmpty(datareceived))
                                {
                                    try
                                    {
                                        String[] splitted = datareceived.Split(new char[] { ',', ';' });
                                        string dataRic = splitted[splitted.Length - 1];
                                        if (dataRic.Contains("("))
                                            dataRic = dataRic.Split('(')[0];
                                        datareceived = DateTime.Parse(dataRic).ToString("dd/MM/yyyy HH:mm:ss");
                                    }
                                    catch
                                    {
                                        datareceived = String.Empty;
                                        logger.Debug("errore nel formato della data di ricezione delle mail");
                                    }
                                }

                            }


                            try
                            {

                                if (!gestioneRicevutePec)
                                {
                                    ricevutaPec = BusinessLogic.interoperabilita.InteroperabilitaManager.isRicevutaPec(mc);
                                    logger.Debug("ricevutaPec: " + ricevutaPec);
                                    if (ricevutaPec)
                                    {


                                        bool risultato = cancellaRicevutePec(i, svr, ref mailProcessed, infoUtente);
                                        if (!risultato)
                                        {
                                            if (!erroreNelMessaggio)
                                                erroreNelMessaggio = true;
                                            logger.Debug("errore nella cancellazione delle ricevute di tipo pec -  errore: " + mailProcessed.ErrorMessage);
                                        }

                                    }
                                }
                                if (!ricevutaPec)
                                {
                                    logger.Debug("Tipo ricevutaPec: " + mailProcessed.PecXRicevuta.ToString());
                                    logger.Debug("Messaggio " + i + " con indirizzo mail " + mc.from + " e id=" + messageId);
                                    try
                                    {
                                        if (!svr.getMessagePec(i) && mailPec != 0)
                                        {//    //nota mailPec è il parametro di var solo mail pec
                                            logger.Debug("la mail non è di tipo pec ma il sistema e configurato per la ricezione di mail di tipo pec");
                                            try
                                            {
                                                if (tipoPosta == "IMAP")
                                                // svr.moveImap(i, false);
                                                {
                                                    logger.Debug("si sta utilizzando un protocollo IMAP ma non rispetta la configurazione pec");
                                                    if (!svr.moveImap(i, false))
                                                    {
                                                        mailProcessed.ErrorMessage = "Impossibile spostare la mail nella cartella non elaborate";
                                                        logger.Debug("errore nello spostamento della mail nella cartella non elaborata, la mail : " + mc.subject);
                                                    }
                                                }
                                            }
                                            catch (Exception ex4)
                                            {
                                                if (!erroreNelMessaggio)
                                                    erroreNelMessaggio = true;
                                                logger.Error("errore nella spostamento delle mail da parte del protoocollo IMAP -  errore: " + ex4.Message);
                                            }

                                            // mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.
                                            mailProcessed.ErrorMessage = "Mail non tipo PEC, la casella è configurata per ricevere solo mail PEC";
                                            logger.Debug("Mail non tipo PEC, la casella è configurata per ricevere solo mail PEC");
                                            try
                                            {
                                                if (tipoPosta == "POP")
                                                    checkId = InteroperabilitaUtils.CheckId(messageId, userId, regId);
                                                else
                                                    checkId = true;
                                            }
                                            catch (Exception ex5)
                                            {
                                                if (!erroreNelMessaggio)
                                                    erroreNelMessaggio = true;
                                                logger.Error("errore nello scarico della mail di pop di tipo non pec - errore:" + ex5.Message);
                                            }
                                        }

                                        else
                                        {
                                            logger.Debug("la mail è di tipo pec verifico se rispetta tutta la configurazione");
                                            try
                                            {
                                                if (messageId == null && mc.from.Equals(mc.recipients[0].mail))
                                                {
                                                    logger.Debug("Mittente=destinatario");
                                                    CultureInfo ci = new CultureInfo("it-IT");
                                                    messageId = mc.date.ToString("dd/MM/yyyy hh:mm:ss");
                                                    logger.Debug("Assegnata messageId: " + messageId);
                                                }
                                                if (tipoPosta == "POP")
                                                    checkId = InteroperabilitaUtils.CheckId(messageId, userId, regId);
                                                else
                                                    checkId = true;
                                            }
                                            catch (Exception ex6)
                                            {
                                                if (!erroreNelMessaggio)
                                                    erroreNelMessaggio = true;
                                                logger.Error("errore nell messaggio: " + ex6.Message);
                                            }

                                            try
                                            {
                                                if (checkId)
                                                {
                                                    logger.Debug("Esame del messaggio");

                                                    string path = ConfigurationManager.AppSettings["LOG_PATH"];
                                                    path = path.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                                                    path = path + @"\Attachments\" + userId;
                                                    DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(path);

                                                    string nomeEmail = string.Empty;
                                                    string salvataggiomail = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SALVA_EMAIL_IN_LOCALE");

                                                    // MEV Gestione scarico mail completa a livello di RF
                                                    DocsPaDB.Query_DocsPAWS.Interoperabilita iop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                                                    string salvaMailRF = iop.isEnabledSaveMail(regId);
                                                    if(!string.IsNullOrEmpty(salvaMailRF) && (salvaMailRF.Equals("0") || salvaMailRF.Equals("1")))
                                                    {
                                                        salvataggiomail = salvaMailRF;
                                                    }
                                                    // ------------------------
                                                    if (BusinessLogic.interoperabilita.InteroperabilitaManager.isRicevutaPec(mc))
                                                    {
                                                        logger.Debug("gestioneRicevutePec: " + gestioneRicevutePec.ToString());
                                                        logger.Debug("salvaAllegatiPec: " + salvaAllegatiPec.ToString());

                                                        if (gestioneRicevutePec &&
                                                            salvaAllegatiPec)
                                                        {
                                                            logger.Debug("Salvo la mail in locale");
                                                            nomeEmail = System.Guid.NewGuid().ToString().Substring(0, 25) + ".eml";
                                                            if (svr.salvaMailInLocale(i, path, nomeEmail))
                                                            {
                                                                logger.Debug("la mail si chiama: " + nomeEmail);
                                                                listaAllegati.Add(nomeEmail);
                                                            }
                                                            else
                                                                logger.Debug("Errore nel salvataggio della mail in locale");

                                                        }
                                                    }
                                                    else
                                                        if (!string.IsNullOrEmpty(salvataggiomail) && salvataggiomail.Equals("1"))
                                                        {
                                                            logger.Debug("Salvo la mail in locale");
                                                            nomeEmail = System.Guid.NewGuid().ToString().Substring(0, 25) + ".eml";
                                                            if (svr.salvaMailInLocale(i, path, nomeEmail))
                                                                logger.Debug("la mail si chiama: " + nomeEmail);
                                                            else
                                                                logger.Debug("Errore nel salvataggio della mail in locale");

                                                        }

                                                    salvaBody(userId, mc, path, out erroreNelMessaggio);

                                                    #region easme degli allegati della mail e gestione dei tipi
                                                    string xmlFileName = null;
                                                    try
                                                    {
                                                        mailProcessed.CountAttatchments = mc.attachments.Count;
                                                        //esame degli attachments della mail

                                                        string num_reg_mit = "";
                                                        string not_ecc = "";

                                                        for (int j = 0; j < mc.attachments.Count; j++)
                                                        {
                                                            #region controllo allegati generici
                                                            logger.Debug("allegato numero: " + j.ToString());
                                                            try
                                                            {
                                                                string nomeAttach = mc.attachments[j].name;
                                                                Regex regExpr = new System.Text.RegularExpressions.Regex("[:*?\\<>|\"/]");
                                                                if (regExpr.IsMatch(nomeAttach))
                                                                {
                                                                    nomeAttach = regExpr.Replace(nomeAttach, "_");
                                                                    mc.attachments[j].name = nomeAttach;
                                                                }
                                                                string fileName = Path.Combine(path, nomeAttach);
                                                                logger.Debug(fileName);
                                                                if (nomeAttach != null && !nomeAttach.Equals(""))
                                                                {
                                                                    mc.attachments[j].SaveToFile(fileName);
                                                                }
                                                            }
                                                            catch (Exception ex11)
                                                            {
                                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                                {

                                                                    if (!svr.moveImap(i, false))
                                                                    {
                                                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                    }
                                                                    else
                                                                    {
                                                                        spostato = true;
                                                                    }
                                                                }
                                                                if (!erroreNelMessaggio)
                                                                    erroreNelMessaggio = true;
                                                                logger.Error("errore nel reperimento dell'allegato. l'allegato è corrotto o inesistente" + ex11.Message);
                                                            }
                                                            #endregion

                                                            #region  controllo eccezione.xml
                                                            //controllo se il file è confermaRicezione.xml
                                                            try
                                                            {
                                                                if (mc.attachments[j].name.ToLower() == "eccezione.xml")
                                                                {
                                                                    logger.Debug("Trovato file eccezione");
                                                                    xmlFileName = mc.attachments[j].name;
                                                                    //a questo punto si esce dal metodo
                                                                    eccezione = true;

                                                                    mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Eccezione;
                                                                }
                                                                else
                                                                {
                                                                    logger.Debug("l'allegato non è file eccezione");
                                                                }
                                                            }
                                                            catch (Exception ex12)
                                                            {
                                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                                {

                                                                    if (!svr.moveImap(i, false))
                                                                    {
                                                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                    }
                                                                    else
                                                                    {
                                                                        spostato = true;
                                                                    }
                                                                }
                                                                if (!erroreNelMessaggio)
                                                                    erroreNelMessaggio = true;
                                                                logger.Error("nelal verifica dell'allegato di tipo confermaricezione.xml l'erroe è il seguente: " + ex12.Message);
                                                            }
                                                            #endregion

                                                            #region  controllo confermaRicezione.xml
                                                            //controllo se il file è confermaRicezione.xml
                                                            try
                                                            {
                                                                if ((mc.attachments[j].name.ToLower() == "conferma.xml") || (mc.attachments[j].name.ToLower() == "confermaricezione.xml"))
                                                                {
                                                                    logger.Debug("Trovato file conferma");
                                                                    xmlFileName = mc.attachments[j].name;
                                                                    //a questo punto si esce dal metodo
                                                                    conferma_ricezione = true;

                                                                    mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.ConfirmReception;
                                                                }
                                                                else
                                                                {
                                                                    logger.Debug("l'allegato non è file conferma");
                                                                }
                                                            }
                                                            catch (Exception ex12)
                                                            {
                                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                                {

                                                                    if (!svr.moveImap(i, false))
                                                                    {
                                                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                    }
                                                                    else
                                                                    {
                                                                        spostato = true;
                                                                    }
                                                                }
                                                                if (!erroreNelMessaggio)
                                                                    erroreNelMessaggio = true;
                                                                logger.Error("nelal verifica dell'allegato di tipo confermaricezione.xml l'erroe è il seguente: " + ex12.Message);
                                                            }
                                                            #endregion

                                                            #region controllo segnatura.xml
                                                            //controllo se il file è segnatura.xml
                                                            //try
                                                            //{
                                                            if (mc.attachments[j].name.ToLower() == "segnatura.xml")
                                                            {
                                                                logger.Debug("Trovato file segnatura");
                                                                xmlFileName = mc.attachments[j].name;

                                                                mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Signature;

                                                                //a questo punto si imposta il flag protocolla_con_segnatura;
                                                                protocolla_con_segnatura = true;

                                                                //TODO SANDALI Scommentare quando fatto

                                                                //Qui si controlla se segnatura è ok e gli altri allegati pure, nel caso di problemi 
                                                                interoperabilita.InteroperabilitaEccezioni ie = new interoperabilita.InteroperabilitaEccezioni(mc);

                                                                if (ie.eccezione_xml != null)
                                                                {
                                                                    string numRegMitt = ie.numRegMitt;
                                                                    num_reg_mit = numRegMitt;
                                                                    //Andrea De Marco - Gestione Eccezione Segnatura.xml Controlli non Bloccanti - per ripristino commentare De Marco e decommentare il resto
                                                                    if (ie.controlloBloccante)
                                                                    {
                                                                        generataEccezione = true;
                                                                        controlloBloccante = true;
                                                                        logger.DebugFormat("Invio Eccezione.xml, per il documento protocollato nr {0} a {1} ", numRegMitt, mc.from);
                                                                        interoperabilita.InteroperabilitaEccezioni.sendNotificaEccezione(reg, ie.eccezione_xml, numRegMitt, mc.from);
                                                                        msg_mail_elaborata = true;
                                                                        protocolla_con_segnatura = false;
                                                                    }
                                                                    else
                                                                    {
                                                                        //Il bool è utilizzato come parametro opzionale per indicare il verificarsi dell'eccezione
                                                                        //è utilizzato per impostare schedaDoc.interop = E
                                                                        generataEccezione = true;

                                                                        //Verifico se il messaggio è di PEC
                                                                        //string isPec = "0";
                                                                        //if (svr.getMessagePec(i))
                                                                        //    isPec = "1";

                                                                        //Occorre aggiungere Segnatura.xml tra gli allegati pec - Utilizzo il parametro opzionale generataEccezione per Gestione Eccezioni PEC
                                                                        //msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnatura(serverName, path, reg, infoUtente, ruolo, messageId, mc, isPec, out err, nomeEmail, datareceived, mailAddress, generataEccezione);

                                                                        //Replico il comportamento del controllo bloccante per non alterare il comportamento successivo del metodo
                                                                        //generataEccezione = true;
                                                                        controlloBloccante = false;

                                                                        //logger.DebugFormat("Invio Eccezione.xml, per il documento protocollato nr {0} a {1} ", numRegMitt, mc.from);
                                                                        //if (msg_mail_elaborata)
                                                                        //{
                                                                        //    interoperabilita.InteroperabilitaEccezioni.sendNotificaEccezione(reg, ie.eccezione_xml, numRegMitt, mc.from);
                                                                        //}

                                                                        not_ecc = ie.eccezione_xml;

                                                                        protocolla_con_segnatura = false;
                                                                    }
                                                                    //End Andrea De Marco

                                                                    //generataEccezione = true;
                                                                    //logger.DebugFormat("Invio Eccezione.xml, per il documento protocollato nr {0} a {1} ", numRegMitt, mc.from);
                                                                    //interoperabilita.InteroperabilitaEccezioni.sendNotificaEccezione(reg, ie.eccezione_xml, numRegMitt,mc.from);
                                                                    //msg_mail_elaborata = true;
                                                                    //protocolla_con_segnatura = false;

                                                                }

                                                            }
                                                            else
                                                            {
                                                                logger.Debug("l'allegato non è file segnatura");
                                                            }
                                                            #endregion

                                                            #region controllo annullamento.xml
                                                            //controllo se il file è annullamento.xml
                                                            try
                                                            {
                                                                if (mc.attachments[j].name.ToLower() == "annullamento.xml")
                                                                {
                                                                    logger.Debug("Trovato file annullamento");
                                                                    xmlFileName = mc.attachments[j].name;


                                                                    mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.NotifyCancellation;

                                                                    //a questo punto si imposta il flag protocolla_con_segnatura;
                                                                    notifica_annullamento = true;
                                                                }
                                                                else
                                                                {
                                                                    logger.Debug("l'allegato non è file annullamento");
                                                                }
                                                            }
                                                            catch (Exception ex16)
                                                            {
                                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                                {

                                                                    if (!svr.moveImap(i, false))
                                                                    {
                                                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                    }
                                                                    else
                                                                    {
                                                                        spostato = true;
                                                                    }
                                                                }
                                                                if (!erroreNelMessaggio)
                                                                    erroreNelMessaggio = true;
                                                                logger.Error("errore nel file annullamento.xml: " + ex16.Message);
                                                            }
                                                            #endregion

                                                            #region controllo daticert.xml
                                                            //controllo se il file è daticert.xml
                                                            try
                                                            {
                                                                if (gestioneRicevutePec)
                                                                {
                                                                    if (mc.attachments[j].name.ToLower() == "daticert.xml")
                                                                    {
                                                                        logger.Debug("Trovato file daticert");
                                                                        xmlFileName = mc.attachments[j].name;

                                                                        
                                                                        // mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.DatiCert;

                                                                        
                                                                        // il true è dato se il documber è stato trovato
                                                                        // E' stato per ora commentato in quanto deve essere approvato
                                                                        //daticert = InteroperabilitaSegnatura.controllaRiferimentoDocNumberDatiCert(path, xmlFileName);
                                                                        
                                                                        //a questo punto si imposta il flag daticert;
                                                                        daticert = true;
                                                                        
                                                                    }
                                                                    else
                                                                    {
                                                                        logger.Debug("l'allegato non è daticert");
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception ex16)
                                                            {
                                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                                {

                                                                    if (!svr.moveImap(i, false))
                                                                    {
                                                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                    }
                                                                    else
                                                                    {
                                                                        spostato = true;
                                                                    }
                                                                }
                                                                if (!erroreNelMessaggio)
                                                                    erroreNelMessaggio = true;
                                                                logger.Error("errore nel file annullamento.xml: " + ex16.Message);
                                                            }
                                                            #endregion

                                                        }//for allegati

                                                        //21-12-2012
                                                        //Se Eccezione non bloccante - Esegui senza segnatura e send eccezione
                                                        if (generataEccezione && !controlloBloccante)
                                                        {
                                                            string isPec = "0";
                                                            if (svr.getMessagePec(i))
                                                                isPec = "1";

                                                            msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnatura(serverName, path, reg, infoUtente, ruolo, messageId, false, mc, isPec, out err, out docnumber, nomeEmail, datareceived, mailAddress, generataEccezione);

                                                            if (msg_mail_elaborata)
                                                            {
                                                                logger.DebugFormat("Invio Eccezione.xml, per il documento protocollato nr {0} a {1} ", num_reg_mit, mc.from);
                                                                interoperabilita.InteroperabilitaEccezioni.sendNotificaEccezione(reg, not_ecc, num_reg_mit, mc.from);
                                                            }
                                                        }
                                                        //21-12-2012

                                                    }
                                                    catch (Exception ex10)
                                                    {
                                                        if (!tipoPosta.Equals("POP") && !spostato)
                                                        {

                                                            if (!svr.moveImap(i, false))
                                                            {
                                                                logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                            }
                                                            else
                                                            {
                                                                spostato = true;
                                                            }
                                                        }
                                                        if (!erroreNelMessaggio)
                                                            erroreNelMessaggio = true;
                                                        logger.Error("errore nella gestione degli allegati - errore: " + ex10.Message);
                                                    }
                                                    #endregion

                                                    try
                                                    {
                                                        //modifica 
                                                        string isPec = "0";
                                                        if (svr.getMessagePec(i))
                                                            isPec = "1";
                                                        //fine modifca


                                                        if (daticert)
                                                        {
                                                            try
                                                            {
                                                                logger.Debug("Elaboro i dati cert  per il messaggio:" + i.ToString());
                                                                msg_mail_elaborata = InteroperabilitaSegnatura.leggiFileDatiCert(path, xmlFileName, out err, out docnumber, listaAllegati, infoUtente, messageId);
                                                                if (msg_mail_elaborata && !string.IsNullOrEmpty(err))
                                                                {
                                                                    mailProcessed.ErrorMessage = "Ok. " + err;
                                                                    err = "";
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                }
                                                                else
                                                                    if (!msg_mail_elaborata && !string.IsNullOrEmpty(err))
                                                                    {
                                                                        mailProcessed.ErrorMessage = err;
                                                                        err = "";
                                                                        if (!erroreNelMessaggio)
                                                                            erroreNelMessaggio = true;
                                                                    }
                                                            }
                                                            catch (Exception e)//erreo con nel daticert
                                                            {
                                                                if (!erroreNelMessaggio)
                                                                    erroreNelMessaggio = true;
                                                                logger.Error("errore nell'esecuzione del file daticert: " + err + " " + e.Message);
                                                                if (!string.IsNullOrEmpty(err))
                                                                    mailProcessed.ErrorMessage = err;
                                                                else
                                                                    mailProcessed.ErrorMessage = e.Message;
                                                                err = "";
                                                                //new
                                                                if (!msg_mail_elaborata)
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {

                                                                        if (!svr.moveImap(i, false))
                                                                        {
                                                                            logger.ErrorFormat("errore nello spostamento della mail nella cartella elaborata, la mail [1] : {0} - ex {1} ", mc.subject, e.Message);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                            }
                                                        }
                                                        else
                                                            //tipo protocollazione
                                                            if (conferma_ricezione == false)
                                                            {
                                                                if (protocolla_con_segnatura)
                                                                {

                                                                    //protocollazione con segnatura
                                                                    //msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSegnatura(serverName, mailAddress, path, xmlFileName, reg, infoUtente, ruolo, messageId, mc.subject, out err);
                                                                    logger.Debug("eseguo la segnatura");
                                                                    if (conProtocollazione)
                                                                        msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSegnaturaProtocollazione(serverName, mailAddress, path, xmlFileName, reg, infoUtente, ruolo, handler, messageId, mc, isPec, out err, out docnumber, nomeEmail, datareceived);
                                                                    else
                                                                        msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSegnatura(serverName, mailAddress, path, xmlFileName, reg, infoUtente, ruolo, messageId, false, mc.subject, isPec, out err, out docnumber, nomeEmail, datareceived);

                                                                    if ((!msg_mail_elaborata) && err.Contains("CODINTEROP2"))
                                                                    {
                                                                        if (elabora_ogni_mail)
                                                                        {
                                                                            logger.Debug("eseguo senza segnatura");
                                                                            if (conProtocollazione)
                                                                                msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnaturaProtocollazione(serverName, path, reg, infoUtente, ruolo, handler, messageId, mc, isPec, out err, out docnumber, nomeEmail, datareceived, mailAddress);
                                                                            else
                                                                                msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnatura(serverName, path, reg, infoUtente, ruolo, messageId, false, mc, isPec, out err, out docnumber, nomeEmail, datareceived, mailAddress);

                                                                            if (isPec == "1")
                                                                                mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec;

                                                                            mailProcessed.ErrorMessage = "OK, Mail elaborata senza segnatura, perchè il file di segnatura non ha formato valido.";
                                                                            err = "";
                                                                        }
                                                                        else
                                                                            msg_mail_elaborata = false;
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    //Not protocolla_con_segnatura
                                                                    if (notifica_annullamento)
                                                                    {
                                                                        //notifica annullamento
                                                                        msg_mail_elaborata = InteroperabilitaNotificaAnnullamento.processaXmlAnnullamento(path, xmlFileName, reg, messageId, mc.from);
                                                                    }
                                                                    //Mail di tipo Eccezione
                                                                    else if (eccezione)
                                                                    {
                                                                        msg_mail_elaborata = interoperabilita.InteroperabilitaEccezioni.processaXmlEccezioni(path, xmlFileName, reg, infoUtente, messageId, mc, mailProcessed);
                                                                    }
                                                                    //Mail di tipo DSN
                                                                    else if (docNumMailDSN != null)
                                                                    {
                                                                        msg_mail_elaborata = interoperabilita.InteroperabilitaEccezioni.GestisciDSN(infoUtente, mailProcessed, docNumMailDSN, mc, messageId, path, nomeEmail, reg);
                                                                    }
                                                                    //Mail di tipo DSN
                                                                    else if (generataEccezione)
                                                                    {
                                                                        //Andrea De Marco - messaggio per controlli non bloccanti
                                                                        if (!controlloBloccante)
                                                                        {
                                                                            //Se eccezione non bloccante, tipo mail=signature
                                                                            mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Signature;

                                                                            if (!msg_mail_elaborata)
                                                                            {
                                                                                //Mail non elaborata
                                                                                mailProcessed.ErrorMessage = err;
                                                                            }
                                                                            else
                                                                            {
                                                                                //mailProcessed.ErrorMessage = "OK. Eccezione non bloccante.";
                                                                                mailProcessed.ErrorMessage = "OK. Eccezione non bloccante nella segnatura informatica";
                                                                            }

                                                                            err = "";
                                                                        }
                                                                        else
                                                                        {
                                                                            msg_mail_elaborata = false;
                                                                            mailProcessed.ErrorMessage = "Generata Eccezione. Si sono verificati errori nel controllo della mail in arrivo";
                                                                        }
                                                                        //End Andrea De Marco
                                                                        //mailProcessed.ErrorMessage = "Generata Eccezione. Si sono verificati errori nel controllo della mail in arrivo";
                                                                    }
                                                                    else
                                                                    {
                                                                        //Not notifica_annullamento
                                                                        if (elabora_ogni_mail)
                                                                        {
                                                                            if (conProtocollazione)
                                                                                msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnaturaProtocollazione(serverName, path, reg, infoUtente, ruolo, handler, messageId, mc, isPec, out err, out docnumber, nomeEmail, datareceived, mailAddress);
                                                                            else
                                                                                msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnatura(serverName, path, reg, infoUtente, ruolo, messageId, false, mc, isPec, out err, out docnumber, nomeEmail, datareceived, mailAddress);

                                                                            if (isPec == "1")
                                                                                mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec;

                                                                        }
                                                                        else
                                                                            msg_mail_elaborata = false;
                                                                    }
                                                                }


                                                            }

                                                            else
                                                            {
                                                                msg_mail_elaborata = InteroperabilitaControlloRicevute.processaXmlConferma(path, xmlFileName, reg, messageId, mc.from, out moreError);
                                                            }

                                                    }
                                                    catch (Exception ex18)
                                                    {
                                                        if (!erroreNelMessaggio)
                                                            erroreNelMessaggio = true;
                                                        logger.Error("errore nell'esecuzione della segnatura: " + ex18.Message);
                                                        //new
                                                        if (!msg_mail_elaborata)
                                                            if (!tipoPosta.Equals("POP") && !spostato)
                                                            {

                                                                if (!svr.moveImap(i, false))
                                                                {
                                                                    logger.ErrorFormat("errore nello spostamento della mail nella cartella elaborata, la mail [2]: {0} - ex {1}", mc.subject, ex18.Message);
                                                                }
                                                                else
                                                                {
                                                                    spostato = true;
                                                                }
                                                            }
                                                        //end new
                                                    }


                                                    try
                                                    {
                                                        if (msg_mail_elaborata)
                                                        {
                                                            if ((protocolla_con_segnatura) ||
                                                                (conferma_ricezione) ||
                                                                (notifica_annullamento) ||
                                                                (elabora_ogni_mail) ||
                                                                (eccezione) ||
                                                                (daticert))
                                                            {

                                                                bool elaboramail = false;
                                                                try
                                                                {
                                                                    if (tipoPosta == "IMAP")
                                                                    {
                                                                        if (svr.moveImap(i, true))
                                                                            elaboramail = InteroperabilitaUtils.MailElaborata(messageId, "E", regId, docnumber, mc.from);
                                                                        else
                                                                        {
                                                                            mailProcessed.ErrorMessage = "Impossibile spostare la mail nella cartella elaborate";
                                                                            logger.ErrorFormat("errore nello spostamento della mail nella cartella elaborata, la mail [3]: {0}", mc.subject);
                                                                        }
                                                                    }

                                                                }
                                                                catch (Exception ex24)
                                                                {
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {

                                                                        if (!svr.moveImap(i, false))
                                                                        {
                                                                            logger.ErrorFormat("errore nello spostamento della mail nella cartella elaborata, la mail [4]: {0} - ex {1} ", mc.subject, ex24.Message);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                                    logger.Error("errore nell'elaborazione della mail di tipo IMAP: " + ex24.Message);
                                                                }


                                                                try
                                                                {
                                                                    if (tipoPosta == "POP")
                                                                        InteroperabilitaUtils.MailElaborata(messageId, "E", regId, docnumber, mc.from);
                                                                }
                                                                catch (Exception ex25)
                                                                {
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    logger.Error("errore nell'elabarzione del messaggio di tipo POP: " + ex25.Message);
                                                                }

                                                                try
                                                                {
                                                                    if (rimuovi_mail_elaborata &&
                                                                        !erroreNelMessaggio)
                                                                        svr.deleteSingleMessage(i);
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    logger.Error(String.Format("Impossibile eliminare il messaggio [{0}] ({1})", mc.subject, e.Message));
                                                                }

                                                                //if (err.Contains("CODINTEROP3"))
                                                                //{
                                                                //    err = err.Replace("CODINTEROP3", "Mail Elaborata.");
                                                                //    mailProcessed.ErrorMessage = err;
                                                                //}

                                                            }
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                if (err.Contains("CODINTEROP1"))
                                                                {
                                                                    err = err.Replace("CODINTEROP1", "Mail Elaborata.");
                                                                    mailProcessed.ErrorMessage = err;
                                                                }
                                                                else
                                                                    if (string.IsNullOrEmpty(mailProcessed.ErrorMessage))
                                                                    {
                                                                        if (string.IsNullOrEmpty(moreError))
                                                                        {
                                                                            mailProcessed.ErrorMessage = "Mail non elaborata. " + err;
                                                                        }
                                                                        else
                                                                        {
                                                                            mailProcessed.ErrorMessage = "Mail non elaborata. " + moreError;
                                                                        }
                                                                    }
                                                                    else
                                                                        mailProcessed.ErrorMessage = "Mail non elaborata. " + mailProcessed.ErrorMessage;

                                                                //mailProcessed.ErrorMessage = "Mail non elaborata." + err;
                                                                if (tipoPosta == "IMAP")
                                                                {
                                                                    // svr.moveImap(i, false);
                                                                    if (!svr.moveImap(i, false))
                                                                    {
                                                                        logger.DebugFormat("errore nello spostamento della mail nella cartella elaborata, la mail [5]: {0}" , mc.subject);
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception ex21)
                                                            {
                                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                                {

                                                                    if (!svr.moveImap(i, false))
                                                                    {
                                                                        logger.ErrorFormat("errore nello spostamento della mail nella cartella elaborata, la mail [6]: {0} - ex {1}", mc.subject, ex21.Message);
                                                                    }
                                                                    else
                                                                    {
                                                                        spostato = true;
                                                                    }
                                                                }
                                                                logger.Error("errore nella mail non elaborata con la gestione del protocollo IMAP: " + ex21.Message);
                                                            }

                                                        }
                                                    }
                                                    catch (Exception ex20)
                                                    {
                                                        if (!tipoPosta.Equals("POP") && !spostato)
                                                        {

                                                            if (!svr.moveImap(i, false))
                                                            {
                                                                logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail [7]: " + mc.subject);
                                                            }
                                                            else
                                                            {
                                                                spostato = true;
                                                            }
                                                        }
                                                        logger.Error("errore nell'elaborazione della mail recuperata: " + ex20.Message + " con id: " + messageId);
                                                    }


                                                    // si cancella la directory temporanea
                                                    DocsPaUtils.Functions.Functions.CancellaDirectory(path);
                                                }
                                            }
                                            catch (Exception ex7)
                                            {
                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                    if (!svr.moveImap(i, false))
                                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail [8]: " + mc.subject);
                                                    else
                                                        spostato = true;

                                                logger.Error("errore nella gestione della mail " + ex7.Message);

                                            }

                                        }
                                    }//try
                                    catch (Exception ex3)
                                    {

                                        logger.Error("errore durante lo scarico di una mail diversa da una ricevuta di ritorno -  errore: " + ex3.Message);
                                    }
                                }//else email diversa da notifica pec
                            }//try
                            catch (Exception ex)
                            {
                                if (!tipoPosta.Equals("POP") && !spostato)
                                {

                                    if (!svr.moveImap(i, false))
                                    {
                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail [9]: " + mc.subject);
                                    }
                                    else
                                    {
                                        spostato = true;
                                    }
                                }
                                logger.Error("errore nella notifiche della posta certificata -  errore: " + ex.Message);
                            }
                        }
                        else
                            logger.Error("non è posibile scaricare il messaggio: " + i + "il messaggio è stato cancellato o è corrotto");
                    }
                    catch (Exception e)
                    {
                        logger.Error(String.Format("Problemi di formati con il messaggio [{0}] ({1})", mc.subject, e.Message));

                        if (!e.Message.Contains("(connerr)"))
                        {
                            if (mailProcessed != null)
                            {
                                if (mailProcessed.ErrorMessage == string.Empty)
                                    mailProcessed.ErrorMessage = e.Message;

                                // Viene incrementato il numero delle mail processate non valide
                                retValue.InvalidMailProcessedCount++;                                
                        
                                if ((mc != null) && (!string.IsNullOrEmpty(mc.subject)))
                                    mailProcessed.Subject = mc.subject;
                                else
                                    mailProcessed.Subject = "N.D.";

                                if ((mc != null) && (!string.IsNullOrEmpty(mc.from)))
                                    mailProcessed.From = mc.from;
                                else
                                    mailProcessed.From = "N.D.";

                                if (mailProcessed.CountAttatchments != mc.attachments.Count)
                                    mailProcessed.ErrorMessage = "Mail non elaborata. " + "Errore nel recupero degli allegati della mail";

                                if (addToList)
                                    retValue.MailProcessedList.Add(mailProcessed);
                            }
                            
                        }
                        else
                        {
                            mailProcessed.CountAttatchments = 0;
                            if ((mc != null) && (!string.IsNullOrEmpty(mc.subject)))
                                mailProcessed.Subject = mc.subject;
                            else
                                mailProcessed.Subject = "N.D.";

                            //mailProcessed.Subject = "Errore di connessione al server di posta durante il download della mail";
                            mailProcessed.ErrorMessage = "Errore di connessione al server di posta durante il download della mail";
                            if (addToList)
                                retValue.MailProcessedList.Add(mailProcessed);
                        }

                    }

                    if (mailProcessed != null && checkId)
                    {
                        if (addToList)
                            retValue.MailProcessedList.Add(mailProcessed);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel controllo dei messaggi. " + e.Message);

                if (retValue.ErrorMessage == string.Empty)
                    retValue.ErrorMessage = e.Message;

            }
            finally
            {
                try
                {
                    // svr.imap.Expunge();
                    if (!tipoPosta.Equals("POP"))
                        //svr.cancellaMailImap();
                        if (svr.cancellaMailImap())
                            logger.Debug("sono state cancellate tutte le mail dalla inbox");
                        else
                            logger.Debug("non sono state cancellate tutte le mail dalla inbox");
                    svr.disconnect();
                }
                catch (Exception e)
                {
                    logger.Error("Disconnessione dal server " + server + " fallita:" + e.Message);

                    if (retValue.ErrorMessage == string.Empty)
                        retValue.ErrorMessage = e.Message;
                }

                // Se almeno un messaggio di posta non è stato processato correttamente,
                // l'esito generale del controllo viene invalidato
                if (retValue.ErrorMessage == string.Empty && retValue.InvalidMailProcessedCount > 0)
                    retValue.ErrorMessage = "Si sono verificati degli errori nel controllo dei messaggi di posta.";
            }

            // Ordinamento delle singole mail per data decrescente
            retValue.MailProcessedList.Sort(new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedComparer());

            return retValue;
        }

        public static string extractDocNumberFromSubject(string subject)
        {
            string retval = string.Empty;
            if (subject.Contains("#"))
            {
                string[] split = subject.Split('#');
                if (split.Length > 1)
                    retval = split[1];
            }
            retval = retval.Replace("#", string.Empty);
            try
            {
                BusinessLogic.Documenti.DocManager.GetTipoDocumento(retval);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Problemi nel reperimento del documento numero {0} errore {1}", retval, e.Message);
                retval = null;
            }
            return retval;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Ruolo getRuoloReg(DocsPaVO.utente.Registro reg)
        {
            logger.Debug("getRuoloReg");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //DataSet ds=new DataSet();
            DataSet ds;
            DocsPaVO.utente.Ruolo res = null;
            try
            {
                #region Codice Commentato
                //db.openConnection();
                /*
                string query="SELECT B.*, C.NUM_LIVELLO AS liv FROM DPA_L_RUOLO_REG A, DPA_CORR_GLOBALI B, DPA_TIPO_RUOLO C WHERE A.ID_REGISTRO="+reg.systemId+" AND A.CHA_RIFERIMENTO='1' AND B.SYSTEM_ID=A.ID_RUOLO_IN_UO AND C.SYSTEM_ID=B.ID_TIPO_RUOLO";
                db.fillTable(query,ds,"RUOLO");           
                */
                #endregion

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getRuoReg(out ds, reg);

                if (ds.Tables["RUOLO"].Rows.Count > 0)
                {
                    res = new DocsPaVO.utente.Ruolo();
                    res.codiceRubrica = ds.Tables["RUOLO"].Rows[0]["VAR_COD_RUBRICA"].ToString();
                    res.codiceCorrispondente = ds.Tables["RUOLO"].Rows[0]["VAR_CODICE"].ToString();
                    res.idAmministrazione = reg.idAmministrazione;
                    res.idGruppo = ds.Tables["RUOLO"].Rows[0]["ID_GRUPPO"].ToString();
                    res.systemId = ds.Tables["RUOLO"].Rows[0]["SYSTEM_ID"].ToString();
                    res.livello = ds.Tables["RUOLO"].Rows[0]["LIV"].ToString();
                }
                //db.closeConnection();
                return res;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                //db.closeConnection();
                logger.Error("Errore nella gestione dell'interoperabilità. (getRuoloReg)", e);
                throw e;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="ruolo"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.InfoUtente getUtenteReg(DocsPaVO.utente.Registro reg, DocsPaVO.utente.Ruolo ruolo)
        {
            //NOTE: per poter creare un doc predisposto alla protocollazione è necessario che l'utente di riferimento sia loggato al sistema perchè hummingbird prevede un dst associato all'utente creatore del doc
            logger.Debug("getUtenteReg");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //DataSet ds=new DataSet();
            DataSet ds;
            DocsPaVO.utente.InfoUtente res = null;
            try
            {
                #region Codice Commentato
                //db.openConnection();
                /*
                string query="SELECT A.USER_ID, A.USER_PASSWORD FROM PEOPLE A, PEOPLEGROUPS B WHERE A.SYSTEM_ID=B.PEOPLE_SYSTEM_ID AND B.GROUPS_SYSTEM_ID="+ruolo.idGruppo+" AND B.CHA_UTENTE_RIF='1'";
                //string query="SELECT A.SYSTEM_ID,A.USER_ID FROM PEOPLE A, PEOPLEGROUPS B WHERE A.SYSTEM_ID=B.PEOPLE_SYSTEM_ID AND B.GROUPS_SYSTEM_ID="+ruolo.idGruppo+" AND B.CHA_UTENTE_RIF='1'";
                db.fillTable(query,ds,"UTENTE");
                */
                #endregion

                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getUtReg(out ds, ruolo);

                if (ds.Tables["UTENTE"].Rows.Count > 0)
                {
                    DocsPaVO.utente.UserLogin log = new DocsPaVO.utente.UserLogin(ds.Tables["UTENTE"].Rows[0]["USER_ID"].ToString(),
                        ds.Tables["UTENTE"].Rows[0]["USER_PASSWORD"].ToString(),
                        reg.idAmministrazione, "", null, true);
                    /*log.Login[0].idAmministrazione=reg.idAmministrazione;
                    log.Login[0].userName=ds.Tables["UTENTE"].Rows[0]["USER_ID"].ToString();
                    log.Login[0].password=ds.Tables["UTENTE"].Rows[0]["USER_PASSWORD"].ToString();
                    */
                    string ipaddress = "";
                    DocsPaVO.utente.UserLogin.LoginResult loginResult;
                    DocsPaVO.utente.Utente ut = BusinessLogic.Utenti.Login.loginMethod(log, out loginResult, false, null, out ipaddress);

                    #region Codice Commentato
                    //DataRow utRow=ds.Tables["UTENTE"].Rows[0];
                    //string queryUser="SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE ID_PEOPLE="+utRow["SYSTEM_ID"].ToString();
                    //logger.Debug(queryUser);
                    //db.fillTable(queryUser,ds,"INFO_UT");
                    //DataRow utInfoRow=ds.Tables["INFO_UT"].Rows[0];
                    #endregion

                    res = new DocsPaVO.utente.InfoUtente();
                    res.dst = ut.dst;
                    res.idAmministrazione = reg.idAmministrazione;
                    res.idCorrGlobali = ut.systemId;
                    res.idGruppo = ruolo.idGruppo;
                    res.idPeople = ut.idPeople;
                    res.userId = ut.userId;
                    return res;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                //db.closeConnection();
                logger.Debug("Errore nella gestione dell'interoperabilità. (getUtenteReg)", e);
                throw e;
            }
        }


        private static bool Cfg_EliminaRicevutePec(DocsPaVO.utente.InfoUtente infoUtente)
        {
            string valoreChiaveDB = string.Empty;
            DocsPaVO.amministrazione.ConfigRepository chiaviAmm = DocsPaUtils.Configuration.InitConfigurationKeys.getInstance(infoUtente.idAmministrazione);
            if (chiaviAmm != null && chiaviAmm.ContainsKey("BE_ELIMINA_RICEVUTE_PEC"))
                valoreChiaveDB = chiaviAmm["BE_ELIMINA_RICEVUTE_PEC"].ToString();

            return (valoreChiaveDB != null && valoreChiaveDB == "1") ? true : false;
        }


        private static bool cancellaRicevutePec(int indiceDellaMail, SvrPosta svr, ref DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed, DocsPaVO.utente.InfoUtente infoUtente)
        {
            //bool retval = false;
            bool retval = true;
            if (svr.getMessage(indiceDellaMail).HTMLBody != null && svr.getMessage(indiceDellaMail).HTMLBody != "")
            {




                // if (System.Configuration.ConfigurationManager.AppSettings["ELIMINA_RICEVUTE_PEC"] != null &&
                //     System.Configuration.ConfigurationManager.AppSettings["ELIMINA_RICEVUTE_PEC"].ToString().Equals("1"))

                if (Cfg_EliminaRicevutePec(infoUtente))
                {
                    logger.Debug("Si cancella il messaggio della ricevuta della posta certificata: " + svr.getMessage(indiceDellaMail).HTMLBody);
                    // retval = svr.deleteSingleMessagePop(indiceDellaMail);
                    svr.deleteSingleMessage(indiceDellaMail);
                    mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec;
                }
            }

            return retval;
        }

        private static bool DeleteReceivedPec(string uidlDellaMail, SvrMailbox svr, ref DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed, DocsPaVO.utente.InfoUtente infoUtente)
        {
            //bool retval = false;
            bool retval = true;
            if (svr.getMessage(uidlDellaMail).HTMLBody != null && svr.getMessage(uidlDellaMail).HTMLBody != "")
            {
                if (Cfg_EliminaRicevutePec(infoUtente))
                {
                    logger.Debug("Si cancella il messaggio della ricevuta della posta certificata: " + svr.getMessage(uidlDellaMail).HTMLBody);
                    // retval = svr.deleteSingleMessagePop(indiceDellaMail);
                    svr.deleteSingleMessage(uidlDellaMail);
                    mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec;
                }
            }

            return retval;
        }

        private static bool DeleteReceivedPec(int indiceDellaMail, SvrMailbox svr, ref DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed, DocsPaVO.utente.InfoUtente infoUtente)
        {
            //bool retval = false;
            bool retval = true;
            if (svr.getMessage(indiceDellaMail).HTMLBody != null && svr.getMessage(indiceDellaMail).HTMLBody != "")
            {




                // if (System.Configuration.ConfigurationManager.AppSettings["ELIMINA_RICEVUTE_PEC"] != null &&
                //     System.Configuration.ConfigurationManager.AppSettings["ELIMINA_RICEVUTE_PEC"].ToString().Equals("1"))

                if (Cfg_EliminaRicevutePec(infoUtente))
                {
                    logger.Debug("Si cancella il messaggio della ricevuta della posta certificata: " + svr.getMessage(indiceDellaMail).HTMLBody);
                    // retval = svr.deleteSingleMessagePop(indiceDellaMail);
                    svr.deleteSingleMessage(indiceDellaMail);
                    mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec;
                }
            }

            return retval;
        }

        private static bool salvaBody(string userId, CMMsg mc, string path, out bool erroreNelMessaggio)
        {

            bool retval = false;
            erroreNelMessaggio = false;
            try
            {
                //salvataggio del testo della mail
                System.IO.StreamWriter sw = new System.IO.StreamWriter(Path.Combine(path, "body.html"), false, System.Text.Encoding.UTF8);
                if ((mc.HTMLBody != null))
                    sw.Write(mc.HTMLBody);
                else if ((mc.body != null))
                    sw.Write("<html><head><title>Corpo messaggio</title></head><body><pre>" + mc.body + "</pre></body></html>");
                else
                    sw.Write("Nessun corpo nella mail.");
                sw.Close();
                retval = true;
            }
            catch (Exception ex9)
            {
                if (!erroreNelMessaggio)
                    erroreNelMessaggio = true;
                logger.Error("errore nella lettura delegate file - errore: " + ex9.Message);
            }

            return retval;
        }

        private static bool salvaAllegati(SvrPosta svr, int indexDellaMail, CMMsg mc, string path, string tipoPosta, out bool spostato, ref DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed, out bool erroreNelMessaggio,
            out string xmlFileName, out bool conferma_ricezione, out bool notifica_annullamento, out bool protocolla_con_segnatura)
        {
            bool retval = false;
            spostato = false;
            erroreNelMessaggio = false;
            conferma_ricezione = false;
            notifica_annullamento = false;
            protocolla_con_segnatura = false;


            xmlFileName = null;
            try
            {
                mailProcessed.CountAttatchments = mc.attachments.Count;

                //esame degli attachments della mail
                for (int j = 0; j < mc.attachments.Count; j++)
                {
                    logger.Debug("allegato numero: " + j.ToString());
                    try
                    {
                        string fileName = Path.Combine(path, mc.attachments[j].name);
                        logger.Debug(fileName);
                        if (mc.attachments[j].name != null && !mc.attachments[j].name.Equals(""))
                        {
                            mc.attachments[j].SaveToFile(fileName);
                        }
                    }
                    catch (Exception ex11)
                    {
                        if (!tipoPosta.Equals("POP") && !spostato)
                        {

                            if (!svr.moveImap(indexDellaMail, false))
                            {
                                logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                            }
                            else
                            {
                                spostato = true;
                            }
                        }
                        if (!erroreNelMessaggio)
                            erroreNelMessaggio = true;
                        logger.Error("errore nel reperimento dell'allegato. l'allegato è corrotto o inesistente" + ex11.Message);
                    }

                    //controllo se il file è confermaRicezione.xml
                    try
                    {
                        if ((mc.attachments[j].name.ToLower() == "conferma.xml") || (mc.attachments[j].name.ToLower() == "confermaricezione.xml"))
                        {
                            logger.Debug("Trovato file conferma");
                            xmlFileName = mc.attachments[j].name;
                            //a questo punto si esce dal metodo
                            conferma_ricezione = true;

                            mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.ConfirmReception;
                        }
                    }
                    catch (Exception ex12)
                    {
                        if (!tipoPosta.Equals("POP") && !spostato)
                        {

                            if (!svr.moveImap(indexDellaMail, false))
                            {
                                logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                            }
                            else
                            {
                                spostato = true;
                            }
                        }
                        if (!erroreNelMessaggio)
                            erroreNelMessaggio = true;
                        logger.Error("nelal verifica dell'allegato di tipo confermaricezione.xml l'erroe è il seguente: " + ex12.Message);
                    }

                    //controllo se il file è segnatura.xml
                    //try
                    //{
                    if (mc.attachments[j].name.ToLower() == "segnatura.xml")
                    {
                        logger.Debug("Trovato file segnatura");
                        xmlFileName = mc.attachments[j].name;

                        mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Signature;

                        //a questo punto si imposta il flag protocolla_con_segnatura;
                        protocolla_con_segnatura = true;

                    }

                    //controllo se il file è annullamento.xml
                    try
                    {
                        if (mc.attachments[j].name.ToLower() == "annullamento.xml")
                        {
                            logger.Debug("Trovato file annullamento");
                            xmlFileName = mc.attachments[j].name;


                            mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.NotifyCancellation;

                            //a questo punto si imposta il flag protocolla_con_segnatura;
                            notifica_annullamento = true;
                        }
                    }
                    catch (Exception ex16)
                    {
                        if (!tipoPosta.Equals("POP") && !spostato)
                        {

                            if (!svr.moveImap(indexDellaMail, false))
                            {
                                logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                            }
                            else
                            {
                                spostato = true;
                            }
                        }
                        if (!erroreNelMessaggio)
                            erroreNelMessaggio = true;
                        logger.Error("errore nel file annullamento.xml: " + ex16.Message);
                    }

                }//for allegati

                retval = true;
            }
            catch (Exception e)
            {
                logger.Error("errore durante il savataggio degli allegati" + e.Message);
            }

            return retval;
        }

        #region Check mailbox new UI

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="reg"></param>
        /// <param name="ut"></param>
        /// <param name="ruolo"></param>
        /// <param name="checkResponse"></param>
        /// <returns></returns>
        public static void StartCheckMailbox(string serverName, DocsPaVO.utente.Registro register, DocsPaVO.utente.Utente user, DocsPaVO.utente.Ruolo role, out DocsPaVO.Interoperabilita.MailAccountCheckResponse checkResponse)
        {
            checkResponse = null;
            string idJob;
            string idCheckMailbox = string.Empty;
            DocsPaDB.Query_DocsPAWS.Interoperabilita docsPaDB = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            logger.Debug("START : DocsPAWS > Interoperabilita > InteroperabilitaRicezione > interopRiceviMethod");
            DataSet dataSet;
            
            //Inserita questa chiave per problema Zanotti(scarichi che si bloccano in continuazione)
            bool useMutex = (string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CHECK_MAIL_NO_MUTEX"))
                                            || DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CHECK_MAIL_NO_MUTEX").Equals("0"));
            System.Threading.Mutex mutex = null;
            try
            {
                //1. creo il job associato al check della casella(questa versione è provvisoria, andrà modellata con il centro notifiche
                docsPaDB.CreateJobs(out idJob);
                //2. creo il record in DPA_CHECK_MAILBOX che verrà sucessivamente popolato con le informazioni
                //sullo scarico della casella istituzionale
                docsPaDB.CreateCheckMailbox(idJob, user.systemId, role.systemId, register.systemId, register.email, out idCheckMailbox);

                docsPaDB.getCampiReg(out dataSet, register);

                if (dataSet.Tables["REGISTRO"].Rows.Count > 0)
                {
                    DataRow regRow = dataSet.Tables["REGISTRO"].Rows[0];
                    string basePath = ConfigurationManager.AppSettings["LOG_PATH"];
                    basePath = basePath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                    basePath = basePath + "\\Interoperabilita" + "\\" + regRow["VAR_CODICE"].ToString() + "\\ricezione";

                    //controllo delle mail
                    //old: il ruolo deve essere quello collegato a DocsPa e non ruolo[0]
                    //DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)ut.ruoli[0];
                    DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
                    infoUtente.dst = user.dst;
                    infoUtente.idAmministrazione = register.idAmministrazione;
                    infoUtente.idCorrGlobali = role.systemId;
                    infoUtente.idGruppo = role.idGruppo;
                    infoUtente.idPeople = user.idPeople;
                    infoUtente.userId = user.userId;

                    //modifica del 10/07/2009
                    if (regRow["VAR_SOLO_MAIL_PEC"].ToString() != "")
                        mailPec = Int32.Parse(regRow["VAR_SOLO_MAIL_PEC"].ToString());
                    else
                        mailPec = 0;
                    //modifica del 10/07/2009

                    int porta = 0;
                    id_registro = regRow["SYSTEM_ID"].ToString();
                    //modifica
                    if (regRow["VAR_TIPO_CONNESSIONE"] != null)
                    {
                        if (useMutex)
                        {
                            logger.Debug("chiamo CreateMutex Per interop posta");
                            // Creazione o reperimento del mutex
                            mutex = CreateMutex(regRow["VAR_USER_MAIL"].ToString());
                            logger.DebugFormat("Mutex Interop Chiamato");
                            mutex.WaitOne();
                        }
                        if (regRow["VAR_TIPO_CONNESSIONE"].Equals("POP"))
                        {
                            if (regRow["NUM_PORTA_POP"] != null && !regRow["NUM_PORTA_POP"].ToString().Equals(""))
                            {
                                logger.Debug("porta non nulla");

                                porta = Int32.Parse(regRow["NUM_PORTA_POP"].ToString());
                            }
                            else
                            {
                                logger.Debug("porta nulla");
                                porta = 110;
                            }

                            checkResponse = CheckMailbox(
                                serverName,
                                regRow["VAR_EMAIL_REGISTRO"].ToString(),
                                regRow["VAR_USER_MAIL"].ToString(),
                                Crypter.Decode(regRow["VAR_PWD_MAIL"].ToString(),
                                regRow["VAR_USER_MAIL"].ToString()),
                                regRow["VAR_SERVER_POP"].ToString(),
                                porta,
                                register,
                                infoUtente,
                                role,
                                regRow["CHA_SMTP_SSL"].ToString(),
                                regRow["CHA_POP_SSL"].ToString(),
                                regRow["CHA_SMTP_STA"].ToString(),
                                regRow["VAR_TIPO_CONNESSIONE"].ToString(),
                                regRow["VAR_INBOX_IMAP"].ToString(),
                                regRow["VAR_BOX_MAIL_ELABORATE"].ToString(),
                                regRow["VAR_MAIL_NON_ELABORATE"].ToString(),
                                idCheckMailbox);
                        }
                        else
                        {
                            if (regRow["NUM_PORTA_IMAP"] != null && !regRow["NUM_PORTA_IMAP"].ToString().Equals(""))
                            {
                                logger.Debug("porta non nulla");

                                porta = Int32.Parse(regRow["NUM_PORTA_IMAP"].ToString());
                            }
                            else
                            {
                                logger.Debug("porta nulla");
                                porta = 993;
                            }

                            checkResponse = CheckMailbox(
                                serverName,
                                regRow["VAR_EMAIL_REGISTRO"].ToString(),
                                regRow["VAR_USER_MAIL"].ToString(),
                                Crypter.Decode(regRow["VAR_PWD_MAIL"].ToString(),
                                regRow["VAR_USER_MAIL"].ToString()),
                                regRow["VAR_SERVER_IMAP"].ToString(),
                                porta,
                                register,
                                infoUtente,
                                role,
                                regRow["CHA_SMTP_SSL"].ToString(),
                                regRow["CHA_IMAP_SSL"].ToString(),
                                regRow["CHA_SMTP_STA"].ToString(),
                                regRow["VAR_TIPO_CONNESSIONE"].ToString(),
                                regRow["VAR_INBOX_IMAP"].ToString(),
                                regRow["VAR_BOX_MAIL_ELABORATE"].ToString(),
                                regRow["VAR_MAIL_NON_ELABORATE"].ToString(),
                                idCheckMailbox);
                        }
                        if (useMutex)
                        {
                            //Rilascio il mutex
                            if (mutex != null)
                            {
                                mutex.ReleaseMutex();
                                mutex.Close();
                                mutex = null;
                            }
                        }
                    }
                    else
                        checkResponse.ErrorMessage = "tipo di connessione imap o pop non configurato per questo registro";
                    //Interoperabilità.Log.LogCheckMail logCheckMail = new BusinessLogic.Interoperabilità.Log.LogCheckMail();
                    //logCheckMail.LogElement(register, checkResponse);
                }
                if (checkResponse != null && checkResponse.MailProcessedList != null)
                {
                    logger.Debug("checkResponse.MailProcessedList: " + checkResponse.MailProcessedList.Count);
                    logger.Debug("checkResponse.InvalidMailProcessedCount" + checkResponse.InvalidMailProcessedCount);
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nell'interrogazione della casella(CheckMailBox)", e);
                if (checkResponse == null)
                {
                    checkResponse = new DocsPaVO.Interoperabilita.MailAccountCheckResponse();
                    checkResponse.ErrorMessage = e.Message.ToString();
                }
            }
            finally
            {
                if (useMutex)
                {
                    //In ogni caso rilascio il mutex se non fosse già stato rilasciato
                    if (mutex != null)
                    {
                        mutex.ReleaseMutex();
                        mutex.Close();
                        mutex = null;
                    }
                }
                //salvo le informazioni sul mail server, mail user id ed errormessage in DPA_CHECK_MAILBOX
                docsPaDB.UpdateInfoCheckMailbox(idCheckMailbox, checkResponse.MailUserID == null ? string.Empty : checkResponse.MailUserID,
                    checkResponse.ErrorMessage == null ? string.Empty : checkResponse.ErrorMessage,
                    checkResponse.MailServer == null ? string.Empty : checkResponse.MailServer);

                //metto concluded a 1 in DPA_CHECK_MAILBOX
                //questa operazione ovviamente va sempre fatta anche nel caso di eventuali problemi durante l'interrogazione della casella
                docsPaDB.ConcludedCheckMailbox(idCheckMailbox);
            }
            if (checkResponse != null && checkResponse.MailProcessedList != null)
            {
                logger.Debug(" PRE_END checkResponse.MailProcessedList: " + checkResponse.MailProcessedList.Count);
                logger.Debug("PRE_END checkResponse.InvalidMailProcessedCount" + checkResponse.InvalidMailProcessedCount);
            }
            logger.Debug("END : DocsPAWS > Interoperabilita > InteroperabilitaRicezione > CheckailBox");
        }

        /// <summary>
        /// Metodo di core che si occupa di processare le mail scaricate dal mail provider
        /// </summary>
        /// <returns>MailAccountCheckResponse</returns>
        private static DocsPaVO.Interoperabilita.MailAccountCheckResponse CheckMailbox(string serverName, string mailAddress, string userId, string password, string server, int port, DocsPaVO.utente.Registro reg, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, string SmtpSsl, string PopSsl, string smtpSTA, string tipoPosta, string mailbox, string mailelaborate, string mailNonElaborate, string idCheckMailbox)
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita docsPaDB = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            DocsPaVO.Interoperabilita.MailAccountCheckResponse retValue =
              new DocsPaVO.Interoperabilita.MailAccountCheckResponse(userId, server, mailAddress, (reg.codRegistro + " - " + reg.descrizione));

            //modifica
            Interoperabilità.SvrMailbox svr = null;
            if (tipoPosta.Equals("POP"))
            {
                svr = new Interoperabilità.SvrMailbox(
                    server,
                    userId,
                    password,
                    port.ToString(),
                    System.IO.Path.GetTempPath(),
                    BusinessLogic.Interoperabilità.CMClientType.POP,
                    SmtpSsl,
                    PopSsl,
                    smtpSTA,
                    reg.systemId
                    );
            }
            else
            {
                svr = new Interoperabilità.SvrMailbox(
                    server,
                    userId,
                    password,
                    port.ToString(),
                    System.IO.Path.GetTempPath(),
                    BusinessLogic.Interoperabilità.CMClientType.IMAP,
                    SmtpSsl,
                    PopSsl,
                    smtpSTA,
                    reg.systemId,
                    mailbox,
                    mailelaborate,
                    mailNonElaborate
                    );
            }

            try
            {
                svr.connect();
            }
            catch (Exception e)
            {
                logger.Error("Connessione al server " + server + " fallita. " + e.Message);

                retValue.ErrorMessage = e.Message;

                throw new Exception("Connessione al server " + server + " fallita. " + e.Message);
            }

            try
            {
                //1. aggiorno il numero totale di mail da processare
                docsPaDB.MailTotalCheckMailbox(idCheckMailbox, svr.messageCount().ToString());

                string err = string.Empty;
                string docnumber = string.Empty;
                DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed mailProcessed = null;

                logger.Debug("Connessione al server " + server + " effettuata");

                bool rimuovi_mail_elaborata = InteroperabilitaUtils.Cfg_EliminazioneMailElaborate(infoUtente);
                bool elabora_ogni_mail = InteroperabilitaUtils.Cfg_ElaborazionePostaOrdinaria;
                int num_mess = svr.messageCount();
                logger.Debug("Numero messaggi trovati: " + num_mess);

                //se la chiave è abilitata, estraggo l'email con l'uidl e non con l'indice
                bool getMessageByUIDL = false;
                string[] uidls = svr.getUidls();
                if( !string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CHECK_MAIL_BY_UIDL")) &&
                    !DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CHECK_MAIL_BY_UIDL").ToString().Equals("0"))
                {
                    if(uidls != null && uidls.Length > 0)
                        getMessageByUIDL = true;
                }

                bool gestioneRicevutePec = false;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GESTIONE_RICEVUTE_PEC"]) &&
                    bool.Parse(ConfigurationManager.AppSettings["GESTIONE_RICEVUTE_PEC"]))
                    gestioneRicevutePec = true;


                bool salvaAllegatiPec = false;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SALVA_RICEVUTE_PEC"]) &&
                    bool.Parse(ConfigurationManager.AppSettings["SALVA_RICEVUTE_PEC"]))
                    salvaAllegatiPec = true;
                logger.Debug("gestioneRicevutePec: " + gestioneRicevutePec + " salvaAllegatiPec: " + salvaAllegatiPec);
                for (int i = 1; i <= num_mess; i++)
                {
                    bool protocolla_con_segnatura = false;
                    bool conferma_ricezione = false;
                    bool eccezione = false;
                    bool generataEccezione = false;
                    bool addToList = true;
                    string docNumMailDSN = null;
                    bool notifica_annullamento = false;
                    bool msg_mail_elaborata = false;
                    bool erroreNelMessaggio = false;
                    bool spostato = false;
                    Interoperabilità.CMMsg mc = new Interoperabilità.CMMsg();
                    bool checkId = false;
                    string messageId = string.Empty;
                    string uidl = string.Empty;
                    string regId = reg.systemId.ToString();
                    bool daticert = false;
                    List<string> listaAllegati = new List<string>();
                    bool ricevutaPec = false;
                    string moreError = string.Empty;
                    bool mailSignatureIsValid = true;

                    bool fatturaElDaPEC = false;
                    bool isFatturaAttiva = false;

                    //Andrea De Marco - Gestione Eccezione PEC
                    bool controlloBloccante = false;
                    //End Andrea De Marco

                    docnumber = string.Empty;
                    try
                    {
                        mailProcessed = new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed();

                        if (getMessageByUIDL)
                        {
                            uidl = uidls[i-1];
                        }
                        mc = !string.IsNullOrEmpty(uidl) ? svr.getMessage(uidl) : svr.getMessage(i);

                        //byte[] mymail = System.IO.File.ReadAllBytes("c:\\postacert.eml");
                        //mc = svr.getMessage(mymail);

                        if (mc == null || !string.IsNullOrEmpty(mc.errorMessage))
                        {
                            mailProcessed.ErrorMessage = "Mail non elaborata. " + mc.errorMessage;
                            mailSignatureIsValid = false;
                        }
                        for (int j = 0; j < mc.attachments.Count; j++)
                        {
                            if (mc.attachments[j].name.LastIndexOf(".") != -1)
                            {
                                string nomeAttach = mc.attachments[j].name;
                                string nomeAttach1 = nomeAttach.Substring(0, nomeAttach.LastIndexOf("."));
                                string nomeAttach2 = nomeAttach.Substring(nomeAttach.LastIndexOf(".") + 1);
                                if (nomeAttach2.Contains(" "))
                                {
                                    nomeAttach2 = nomeAttach2.Replace(" ", "");
                                    nomeAttach = nomeAttach1 + "." + nomeAttach2;
                                    mc.attachments[j].name = nomeAttach;
                                }
                            }
                            else //non è presente estensione
                            {
                                logger.Error("Attenzione l'allegato NON ha estensione, cerco di recuperarla dal content type");
                                string ext = Interoperabilità.MimeMapper.GetExtensionFromMime(mc.attachments[j].contentType);
                                logger.ErrorFormat("ContentType : {0}  -> Ext {1} ", mc.attachments[j].contentType, ext);
                                mc.attachments[j].name += ext;
                            }

                        }
                        if (mc != null)
                        {
                            mailProcessed.PecXRicevuta = BusinessLogic.interoperabilita.InteroperabilitaManager.getTipoRicevutaPec(mc);
                            //Gestione DSN controllo se DSN e in caso recupero il docNumer dal subject
                            if (mailProcessed.PecXRicevuta == DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.Delivery_Status_Notification)
                            {
                                if (mc.subject.Contains("#") || gestioneRicevutePec)
                                {
                                    docNumMailDSN = extractDocNumberFromSubject(mc.subject);
                                }
                            }

                            //Gestione Accettazione o Consegna della Eccezioni
                            if (mc.subject.Contains(": Ricevuta Eccezione"))
                            {
                                addToList = false;
                                msg_mail_elaborata = true;
                                protocolla_con_segnatura = false;
                            }

                            try
                            {
                                messageId = mc.getHeader("message-id").Trim();
                                logger.Debug("messageId: " + messageId);
                                mailProcessed.MailID = messageId;
                                mailProcessed.Date = mc.date;
                                mailProcessed.From = mc.from;
                                foreach (CMRecipient recipient in mc.recipients)
                                    mailProcessed.Recipients.Add(recipient.mail);

                                //modifica
                                if (mc.subject.Contains("#")
                                    || gestioneRicevutePec)
                                {
                                    string[] split = mc.subject.Split('#');
                                    if (split.Length > 0)
                                        mc.subject = split[0];
                                }
                                //fine modifica

                                mailProcessed.Subject = mc.subject;
                                mailProcessed.Subject = mailProcessed.Subject.Replace("<", "&lt;");
                                mailProcessed.Subject = mailProcessed.Subject.Replace(">", "&gt;");
                            }
                            catch (Exception e)
                            {
                                if (!erroreNelMessaggio)
                                    erroreNelMessaggio = true;
                                logger.Error("errore nel recupero delle informazioni della mail - errore: " + e.Message);
                            }

                            string datasend = mc.DateSendMail();
                            // string datasend = mc.date.ToShortTimeString();
                            if (!string.IsNullOrEmpty(datasend))
                            {
                                try
                                {
                                    int valSub = datasend.LastIndexOf("(");
                                    if (valSub != -1)
                                    {
                                        datasend = DateTime.Parse(datasend.Substring(0, datasend.LastIndexOf("("))).ToString("dd/MM/yyyy HH:mm:ss");
                                    }
                                    else
                                    {
                                        datasend = DateTime.Parse(datasend).ToString("dd/MM/yyyy HH:mm:ss");
                                    }
                                }
                                catch
                                {
                                    logger.Debug("errore nel formato della data di spedizione delle mail");
                                }
                            }

                            string datareceived = String.Empty;
                            try
                            {
                                datareceived = mailProcessed.Date.ToString("dd/MM/yyyy HH:mm:ss");
                            }
                            catch
                            {
                                //fallback
                                datareceived = mc.DateReceivedMail();
                                if (!string.IsNullOrEmpty(datareceived))
                                {
                                    try
                                    {
                                        String[] splitted = datareceived.Split(new char[] { ',', ';' });
                                        string dataRic = splitted[splitted.Length - 1];
                                        if (dataRic.Contains("("))
                                            dataRic = dataRic.Split('(')[0];
                                        datareceived = DateTime.Parse(dataRic).ToString("dd/MM/yyyy HH:mm:ss");
                                    }
                                    catch
                                    {
                                        datareceived = String.Empty;
                                        logger.Debug("errore nel formato della data di ricezione delle mail");
                                    }
                                }
                            }
                            try
                            {

                                if (!gestioneRicevutePec)
                                {
                                    ricevutaPec = BusinessLogic.interoperabilita.InteroperabilitaManager.isRicevutaPec(mc);
                                    logger.Debug("ricevutaPec: " + ricevutaPec);
                                    if (ricevutaPec)
                                    {
                                        bool risultato;
                                        if (!string.IsNullOrEmpty(uidl))
                                        {
                                            risultato = DeleteReceivedPec(uidl, svr, ref mailProcessed, infoUtente);
                                        }
                                        else
                                        {
                                            risultato = DeleteReceivedPec(i, svr, ref mailProcessed, infoUtente);
                                        }
                                        if (!risultato)
                                        {
                                            if (!erroreNelMessaggio)
                                                erroreNelMessaggio = true;
                                            logger.Debug("errore nella cancellazione delle ricevute di tipo pec -  errore: " + mailProcessed.ErrorMessage);
                                        }

                                    }
                                }
                                if (!ricevutaPec)
                                {
                                    logger.Debug("Tipo ricevutaPec: " + mailProcessed.PecXRicevuta.ToString());
                                    logger.Debug("Messaggio " + i + " con indirizzo mail " + mc.from + " e id=" + messageId);
                                    try
                                    {
                                        bool isMailPec;
                                        if (!string.IsNullOrEmpty(uidl))
                                            isMailPec = svr.getMessagePec(uidl);
                                        else
                                            isMailPec = svr.getMessagePec(i);
                                        if (!isMailPec && mailPec != 0)
                                        {//    //nota mailPec è il parametro di var solo mail pec
                                            logger.Debug("la mail non è di tipo pec ma il sistema e configurato per la ricezione di mail di tipo pec");
                                            try
                                            {
                                                if (tipoPosta == "IMAP")
                                                // svr.moveImap(i, false);
                                                {
                                                    logger.Debug("si sta utilizzando un protocollo IMAP ma non rispetta la configurazione pec");
                                                    bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                    if (!moveImap)
                                                    {
                                                        mailProcessed.ErrorMessage = "Impossibile spostare la mail nella cartella non elaborate";
                                                        logger.Debug("errore nello spostamento della mail nella cartella non elaborata, la mail : " + mc.subject);
                                                    }
                                                }
                                            }
                                            catch (Exception ex4)
                                            {
                                                if (!erroreNelMessaggio)
                                                    erroreNelMessaggio = true;
                                                logger.Error("errore nella spostamento delle mail da parte del protoocollo IMAP -  errore: " + ex4.Message);
                                            }

                                            // mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.
                                            mailProcessed.ErrorMessage = "Mail non tipo PEC, la casella è configurata per ricevere solo mail PEC";
                                            logger.Debug("Mail non tipo PEC, la casella è configurata per ricevere solo mail PEC");
                                            try
                                            {
                                                if (tipoPosta == "POP")
                                                    checkId = InteroperabilitaUtils.CheckId(messageId, userId, regId);
                                                else
                                                    checkId = true;
                                            }
                                            catch (Exception ex5)
                                            {
                                                if (!erroreNelMessaggio)
                                                    erroreNelMessaggio = true;
                                                logger.Error("errore nello scarico della mail di pop di tipo non pec - errore:" + ex5.Message);
                                            }
                                        }

                                        else
                                        {
                                            logger.Debug("la mail è di tipo pec verifico se rispetta tutta la configurazione");
                                            try
                                            {
                                                if (messageId == null && mc.from.Equals(mc.recipients[0].mail))
                                                {
                                                    logger.Debug("Mittente=destinatario");
                                                    CultureInfo ci = new CultureInfo("it-IT");
                                                    messageId = mc.date.ToString("dd/MM/yyyy hh:mm:ss");
                                                    logger.Debug("Assegnata messageId: " + messageId);
                                                }
                                                if (tipoPosta == "POP")
                                                    checkId = InteroperabilitaUtils.CheckId(messageId, userId, regId);
                                                else
                                                    checkId = true;
                                            }
                                            catch (Exception ex6)
                                            {
                                                if (!erroreNelMessaggio)
                                                    erroreNelMessaggio = true;
                                                logger.Error("errore nell messaggio: " + ex6.Message);
                                            }

                                            try
                                            {
                                                if (checkId)
                                                {
                                                    if (mailSignatureIsValid)
                                                    {
                                                        logger.Debug("Esame del messaggio");

                                                        string path = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_TEMP_PATH");
                                                        //path = path.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                                                        path = path + @"\" + DateTime.Now.ToString("yyyyMMdd") + @"\Attachments\" + userId;
                                                        DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(path);

                                                        string nomeEmail = string.Empty;
                                                        string salvataggiomail = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SALVA_EMAIL_IN_LOCALE");

                                                        // MEV Gestione scarico mail completa a livello di RF
                                                        DocsPaDB.Query_DocsPAWS.Interoperabilita iop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                                                        string salvaMailRF = iop.isEnabledSaveMail(reg.systemId);
                                                        if (!string.IsNullOrEmpty(salvaMailRF) && (salvaMailRF.Equals("0") || salvaMailRF.Equals("1")))
                                                        {
                                                            salvataggiomail = salvaMailRF;
                                                        }
                                                        // ------------------------

                                                        if (BusinessLogic.interoperabilita.InteroperabilitaManager.isRicevutaPec(mc))
                                                        {
                                                            logger.Debug("gestioneRicevutePec: " + gestioneRicevutePec.ToString());
                                                            logger.Debug("salvaAllegatiPec: " + salvaAllegatiPec.ToString());

                                                            if (gestioneRicevutePec &&
                                                                salvaAllegatiPec)
                                                            {
                                                                logger.Debug("Salvo la mail in locale");
                                                                nomeEmail = System.Guid.NewGuid().ToString().Substring(0, 25) + ".eml";

                                                                //Se UIDL non è vuoto salvo la mail estraendola con l'uidl altrimenti tramite l'indice
                                                                bool salvaMail = !string.IsNullOrEmpty(uidl) ? svr.salvaMailInLocale(uidl, path, nomeEmail) : svr.salvaMailInLocale(i, path, nomeEmail);
                                                                if (salvaMail)
                                                                {
                                                                    logger.Debug("la mail si chiama: " + nomeEmail);
                                                                    listaAllegati.Add(nomeEmail);
                                                                }
                                                                else
                                                                {
                                                                    erroreNelMessaggio = true;
                                                                    mailProcessed.ErrorMessage = "Errore nel salvataggio della mail.";
                                                                    logger.Debug("Errore nel salvataggio della mail in locale");
                                                                    throw new Exception("Mail non elaborata. Errore nel salvataggio della mail.");
                                                                }

                                                            }
                                                        }
                                                        else
                                                            if (!string.IsNullOrEmpty(salvataggiomail) && salvataggiomail.Equals("1"))
                                                        {
                                                            logger.Debug("Salvo la mail in locale");
                                                            nomeEmail = System.Guid.NewGuid().ToString().Substring(0, 25);
                                                            //PURTROPPO E' ACCADUTO CHE CI FOSSERO DUEGUID CON IN PRIMI 25 CARATTERI UGUALI..
                                                            nomeEmail += DateTime.Now.ToString("ss.ff") + ".eml";
                                                            bool salvaMail = !string.IsNullOrEmpty(uidl) ? svr.salvaMailInLocale(uidl, path, nomeEmail) : svr.salvaMailInLocale(i, path, nomeEmail);
                                                            if (salvaMail)
                                                                logger.Debug("la mail si chiama: " + nomeEmail);
                                                            else
                                                            {
                                                                erroreNelMessaggio = true;
                                                                mailProcessed.ErrorMessage = "Errore nel salvataggio della mail.";
                                                                logger.Debug("Errore nel salvataggio della mail in locale");
                                                                throw new Exception("Mail non elaborata. Errore nel salvataggio della mail.");
                                                            }
                                                        }

                                                        salvaBody(userId, mc, path, out erroreNelMessaggio);

                                                        #region easme degli allegati della mail e gestione dei tipi
                                                        string xmlFileName = null;
                                                        try
                                                        {
                                                            mailProcessed.CountAttatchments = mc.attachments.Count;
                                                            //esame degli attachments della mail

                                                            string num_reg_mit = "";
                                                            string not_ecc = "";

                                                            for (int j = 0; j < mc.attachments.Count; j++)
                                                            {
                                                                #region controllo allegati generici
                                                                logger.Debug("allegato numero: " + j.ToString());
                                                                try
                                                                {
                                                                    string nomeAttach = mc.attachments[j].name;
                                                                    Regex regExpr = new System.Text.RegularExpressions.Regex("[:*?\\<>|\"/]");
                                                                    if (regExpr.IsMatch(nomeAttach))
                                                                    {
                                                                        nomeAttach = regExpr.Replace(nomeAttach, "_");
                                                                        mc.attachments[j].name = nomeAttach;
                                                                    }
                                                                    string fileName = Path.Combine(path, nomeAttach);
                                                                    logger.Debug(fileName);
                                                                    if (nomeAttach != null && !nomeAttach.Equals(""))
                                                                    {
                                                                        mc.attachments[j].SaveToFile(fileName);
                                                                    }
                                                                }
                                                                catch (Exception ex11)
                                                                {
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {
                                                                        bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                        if (!moveImap)
                                                                        {
                                                                            logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    logger.Error("errore nel reperimento dell'allegato. l'allegato è corrotto o inesistente" + ex11.Message);
                                                                }
                                                                #endregion

                                                                #region  controllo eccezione.xml
                                                                //controllo se il file è confermaRicezione.xml
                                                                try
                                                                {
                                                                    if (mc.attachments[j].name.ToLower() == "eccezione.xml")
                                                                    {
                                                                        logger.Debug("Trovato file eccezione");
                                                                        xmlFileName = mc.attachments[j].name;
                                                                        //a questo punto si esce dal metodo
                                                                        eccezione = true;

                                                                        mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Eccezione;
                                                                    }
                                                                    else
                                                                    {
                                                                        logger.Debug("l'allegato non è file eccezione");
                                                                    }
                                                                }
                                                                catch (Exception ex12)
                                                                {
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {
                                                                        bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                        if (!moveImap)
                                                                        {
                                                                            logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    logger.Error("nelal verifica dell'allegato di tipo confermaricezione.xml l'erroe è il seguente: " + ex12.Message);
                                                                }
                                                                #endregion

                                                                #region  controllo confermaRicezione.xml
                                                                //controllo se il file è confermaRicezione.xml
                                                                try
                                                                {
                                                                    if ((mc.attachments[j].name.ToLower() == "conferma.xml") || (mc.attachments[j].name.ToLower() == "confermaricezione.xml"))
                                                                    {
                                                                        logger.Debug("Trovato file conferma");
                                                                        xmlFileName = mc.attachments[j].name;
                                                                        //a questo punto si esce dal metodo
                                                                        conferma_ricezione = true;

                                                                        mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.ConfirmReception;
                                                                    }
                                                                    else
                                                                    {
                                                                        logger.Debug("l'allegato non è file conferma");
                                                                    }
                                                                }
                                                                catch (Exception ex12)
                                                                {
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {
                                                                        bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                        if (moveImap)
                                                                        {
                                                                            logger.Debug("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    logger.Error("nelal verifica dell'allegato di tipo confermaricezione.xml l'erroe è il seguente: " + ex12.Message);
                                                                }
                                                                #endregion

                                                                #region controllo segnatura.xml
                                                                //controllo se il file è segnatura.xml
                                                                //try
                                                                //{
                                                                if (mc.attachments[j].name.ToLower() == "segnatura.xml")
                                                                {
                                                                    logger.Debug("Trovato file segnatura");
                                                                    xmlFileName = mc.attachments[j].name;

                                                                    mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Signature;

                                                                    //a questo punto si imposta il flag protocolla_con_segnatura;
                                                                    protocolla_con_segnatura = true;

                                                                    //TODO SANDALI Scommentare quando fatto

                                                                    //Qui si controlla se segnatura è ok e gli altri allegati pure, nel caso di problemi 
                                                                    interoperabilita.InteroperabilitaEccezioni ie = new interoperabilita.InteroperabilitaEccezioni(mc);

                                                                    if (ie.eccezione_xml != null)
                                                                    {
                                                                        string numRegMitt = ie.numRegMitt;
                                                                        num_reg_mit = numRegMitt;
                                                                        //Andrea De Marco - Gestione Eccezione Segnatura.xml Controlli non Bloccanti - per ripristino commentare De Marco e decommentare il resto
                                                                        if (ie.controlloBloccante)
                                                                        {
                                                                            generataEccezione = true;
                                                                            controlloBloccante = true;
                                                                            logger.DebugFormat("Invio Eccezione.xml, per il documento protocollato nr {0} a {1} ", numRegMitt, mc.from);
                                                                            interoperabilita.InteroperabilitaEccezioni.sendNotificaEccezione(reg, ie.eccezione_xml, numRegMitt, mc.from);
                                                                            msg_mail_elaborata = true;
                                                                            protocolla_con_segnatura = false;
                                                                        }
                                                                        else
                                                                        {
                                                                            //Il bool è utilizzato come parametro opzionale per indicare il verificarsi dell'eccezione
                                                                            //è utilizzato per impostare schedaDoc.interop = E
                                                                            generataEccezione = true;

                                                                            //Verifico se il messaggio è di PEC
                                                                            //string isPec = "0";
                                                                            //if (svr.getMessagePec(i))
                                                                            //    isPec = "1";

                                                                            //Occorre aggiungere Segnatura.xml tra gli allegati pec - Utilizzo il parametro opzionale generataEccezione per Gestione Eccezioni PEC
                                                                            //msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnatura(serverName, path, reg, infoUtente, ruolo, messageId, mc, isPec, out err, nomeEmail, datareceived, mailAddress, generataEccezione);

                                                                            //Replico il comportamento del controllo bloccante per non alterare il comportamento successivo del metodo
                                                                            //generataEccezione = true;
                                                                            controlloBloccante = false;

                                                                            //logger.DebugFormat("Invio Eccezione.xml, per il documento protocollato nr {0} a {1} ", numRegMitt, mc.from);
                                                                            //if (msg_mail_elaborata)
                                                                            //{
                                                                            //    interoperabilita.InteroperabilitaEccezioni.sendNotificaEccezione(reg, ie.eccezione_xml, numRegMitt, mc.from);
                                                                            //}

                                                                            not_ecc = ie.eccezione_xml;

                                                                            protocolla_con_segnatura = false;
                                                                        }
                                                                        //End Andrea De Marco

                                                                        //generataEccezione = true;
                                                                        //logger.DebugFormat("Invio Eccezione.xml, per il documento protocollato nr {0} a {1} ", numRegMitt, mc.from);
                                                                        //interoperabilita.InteroperabilitaEccezioni.sendNotificaEccezione(reg, ie.eccezione_xml, numRegMitt,mc.from);
                                                                        //msg_mail_elaborata = true;
                                                                        //protocolla_con_segnatura = false;

                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    logger.Debug("l'allegato non è file segnatura");
                                                                }
                                                                #endregion

                                                                #region controllo annullamento.xml
                                                                //controllo se il file è annullamento.xml
                                                                try
                                                                {
                                                                    if (mc.attachments[j].name.ToLower() == "annullamento.xml")
                                                                    {
                                                                        logger.Debug("Trovato file annullamento");
                                                                        xmlFileName = mc.attachments[j].name;


                                                                        mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.NotifyCancellation;

                                                                        //a questo punto si imposta il flag protocolla_con_segnatura;
                                                                        notifica_annullamento = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        logger.Debug("l'allegato non è file annullamento");
                                                                    }
                                                                }
                                                                catch (Exception ex16)
                                                                {
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {
                                                                        bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                        if (!moveImap)
                                                                        {
                                                                            logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    logger.Error("errore nel file annullamento.xml: " + ex16.Message);
                                                                }
                                                                #endregion

                                                                #region controllo daticert.xml
                                                                //controllo se il file è daticert.xml
                                                                try
                                                                {
                                                                    if (gestioneRicevutePec)
                                                                    {
                                                                        if (mc.attachments[j].name.ToLower() == "daticert.xml")
                                                                        {
                                                                            logger.Debug("Trovato file daticert");
                                                                            xmlFileName = mc.attachments[j].name;


                                                                            // mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.DatiCert;

                                                                            //a questo punto si imposta il flag daticert;
                                                                            daticert = true;
                                                                        }
                                                                        else
                                                                        {
                                                                            logger.Debug("l'allegato non è daticert");
                                                                        }
                                                                    }
                                                                }
                                                                catch (Exception ex16)
                                                                {
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {
                                                                        bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                        if (!moveImap)
                                                                        {
                                                                            logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    logger.Error("errore nel file annullamento.xml: " + ex16.Message);
                                                                }
                                                                #endregion

                                                                #region Controllo presenza fattura elettronica
                                                                try
                                                                {
                                                                    if (mc.attachments[j].name.ToLower().Contains(".xml") &&
                                                                        !((mc.attachments[j].name.ToLower() == "conferma.xml") ||
                                                                        (mc.attachments[j].name.ToLower() == "confermaricezione.xml") ||
                                                                        (mc.attachments[j].name.ToLower() == "daticert.xml") ||
                                                                        (mc.attachments[j].name.ToLower() == "eccezione.xml") ||
                                                                        (mc.attachments[j].name.ToLower() == "annullamento.xml") ||
                                                                        (mc.attachments[j].name.ToLower() == "segnatura.xml")
                                                                        ))
                                                                    {
                                                                        logger.Debug("Trovato file xml non di tipo conosciuto: " + mc.attachments[j].name);
                                                                        if (BusinessLogic.Amministrazione.SistemiEsterni.FattElCtrlPresenzaFattura(infoUtente.idAmministrazione))
                                                                        {
                                                                            if (mc.attachments[j].name.ToLower().Contains(".p7m"))
                                                                            {
                                                                                // non posso sbustarlo, quindi analizzerò in seguito.
                                                                                fatturaElDaPEC = true;
                                                                            }
                                                                            else
                                                                            {
                                                                                string xmlFatt = System.Text.Encoding.UTF8.GetString(mc.attachments[j]._data);
                                                                                xmlFatt = xmlFatt.Trim();
                                                                                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                                                                                if (xmlFatt.Contains("xml version=\"1.1\""))
                                                                                {
                                                                                    logger.Debug("Versione XML 1.1. Provo conversione");
                                                                                    xmlFatt = xmlFatt.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                                                                                }
                                                                                xmlDoc.LoadXml(xmlFatt);
                                                                                if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://www.fatturapa.gov.it/sdi/fatturapa/v1") ||
                                                                                    xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/"))
                                                                                {
                                                                                    fatturaElDaPEC = true;

                                                                                    //Se la fattura elettronica è una fattura attiva blocco lo scarico della PEC
                                                                                    string mappFornitore = string.Format("//*[name()='{0}']/*[name()='{1}']/*[name()='{2}']/*[name()='{3}']", "CedentePrestatore", "DatiAnagrafici", "IdFiscaleIVA", "IdCodice");
                                                                                    string codFornitore = (xmlDoc.DocumentElement.SelectSingleNode(mappFornitore)).InnerXml;

                                                                                    if (!string.IsNullOrEmpty(codFornitore))
                                                                                    {
                                                                                        DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                                                                                        System.Collections.ArrayList fornitori = dbAmm.FattElAttive_getCodFornitore(infoUtente.idAmministrazione);
                                      
                                                                                        foreach (DocsPaVO.ExternalServices.FornitoreFattAttiva forn in fornitori)
                                                                                        {
                                                                                            if (!string.IsNullOrEmpty(forn.CodFornitore) && forn.CodFornitore.ToUpper() == codFornitore.ToUpper())
                                                                                            {
                                                                                                //SE FATTURA ATTIVA BLOCCO LO SCARICO DELLA PEC
                                                                                                isFatturaAttiva = true;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    logger.Debug("Il file xml non è una fattura.");
                                                                                }

                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            logger.Debug("Non è previsto il popolamento di una tipologia fattura elettronica per questa amministrazione");
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        logger.Debug("Non è presente una possibile fattura elettronica");
                                                                    }
                                                                }
                                                                catch (Exception ex12)
                                                                {
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {
                                                                        bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                        if (!moveImap)
                                                                        {
                                                                            logger.Debug("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    logger.Error("nelal verifica dell'allegato di tipo confermaricezione.xml l'erroe è il seguente: " + ex12.Message);
                                                                }

                                                                #endregion

                                                            }//for allegati

                                                            //21-12-2012
                                                            //Se Eccezione non bloccante - Esegui senza segnatura e send eccezione
                                                            //27/06/2016 - aggiunto daticert per catturare un caso di errore dove tra gli allegati inserisco anche un file daticert
                                                            if (generataEccezione && !controlloBloccante && !daticert)
                                                            {
                                                                string isPec = "0";
                                                                //if (svr.getMessagePec(i))
                                                                //    isPec = "1";
                                                                if (!string.IsNullOrEmpty(uidl))
                                                                {
                                                                    if (svr.getMessagePec(uidl))
                                                                        isPec = "1";
                                                                }
                                                                else
                                                                {
                                                                    if (svr.getMessagePec(i))
                                                                        isPec = "1";
                                                                }

                                                                msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnatura(serverName, path, reg, infoUtente, ruolo, messageId, fatturaElDaPEC, mc, isPec, out err, out docnumber, nomeEmail, datareceived, mailAddress, generataEccezione);

                                                                if (msg_mail_elaborata)
                                                                {
                                                                    logger.DebugFormat("Invio Eccezione.xml, per il documento protocollato nr {0} a {1} ", num_reg_mit, mc.from);
                                                                    interoperabilita.InteroperabilitaEccezioni.sendNotificaEccezione(reg, not_ecc, num_reg_mit, mc.from);
                                                                }
                                                            }
                                                            //21-12-2012

                                                        }
                                                        catch (Exception ex10)
                                                        {
                                                            if (!tipoPosta.Equals("POP") && !spostato)
                                                            {
                                                                bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                if (!moveImap)
                                                                {
                                                                    logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                }
                                                                else
                                                                {
                                                                    spostato = true;
                                                                }
                                                            }
                                                            if (!erroreNelMessaggio)
                                                                erroreNelMessaggio = true;
                                                            logger.Error("errore nella gestione degli allegati - errore: " + ex10.Message);
                                                        }
                                                        #endregion

                                                        if (isFatturaAttiva)
                                                        {
                                                            erroreNelMessaggio = true;
                                                            mailProcessed.ErrorMessage = "Mail non elaborata. Il documento è una Fattura attiva.";
                                                            throw new Exception("Mail non elaborata. Il documento è una Fattura attiva.");
                                                        }

                                                        try
                                                        {
                                                            //modifica 
                                                            string isPec = "0";
                                                            //if (svr.getMessagePec(i))
                                                            //    isPec = "1";
                                                            if (!string.IsNullOrEmpty(uidl))
                                                            {
                                                                if (svr.getMessagePec(uidl))
                                                                    isPec = "1";
                                                            }
                                                            else
                                                            {
                                                                if (svr.getMessagePec(i))
                                                                    isPec = "1";
                                                            }
                                                            //fine modifca


                                                            if (daticert)
                                                            {
                                                                try
                                                                {
                                                                    logger.Debug("Elaboro i dati cert  per il messaggio:" + i.ToString());
                                                                    msg_mail_elaborata = InteroperabilitaSegnatura.leggiFileDatiCert(path, xmlFileName, out err, out docnumber, listaAllegati, infoUtente, messageId);
                                                                    if (msg_mail_elaborata && !string.IsNullOrEmpty(err))
                                                                    {
                                                                        mailProcessed.ErrorMessage = "Ok. " + err;
                                                                        err = "";
                                                                        if (!erroreNelMessaggio)
                                                                            erroreNelMessaggio = true;
                                                                    }
                                                                    else
                                                                        if (!msg_mail_elaborata && !string.IsNullOrEmpty(err))
                                                                    {
                                                                        mailProcessed.ErrorMessage = err;
                                                                        err = "";
                                                                        if (!erroreNelMessaggio)
                                                                            erroreNelMessaggio = true;
                                                                    }
                                                                }
                                                                catch (Exception e)//erreo con nel daticert
                                                                {
                                                                    if (!erroreNelMessaggio)
                                                                        erroreNelMessaggio = true;
                                                                    logger.Error("errore nell'esecuzione del file daticert: " + err + " " + e.Message);
                                                                    if (!string.IsNullOrEmpty(err))
                                                                        mailProcessed.ErrorMessage = err;
                                                                    else
                                                                        mailProcessed.ErrorMessage = e.Message;
                                                                    err = "";
                                                                    //new
                                                                    if (!msg_mail_elaborata)
                                                                        if (!tipoPosta.Equals("POP") && !spostato)
                                                                        {
                                                                            bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                            if (!moveImap)
                                                                            {
                                                                                logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                            }
                                                                            else
                                                                            {
                                                                                spostato = true;
                                                                            }
                                                                        }
                                                                }
                                                            }
                                                            else
                                                                //tipo protocollazione
                                                                if (conferma_ricezione == false)
                                                            {
                                                                if (protocolla_con_segnatura)
                                                                {

                                                                    msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSegnatura(serverName, mailAddress, path, xmlFileName, reg, infoUtente, ruolo, messageId, fatturaElDaPEC, mc.subject, isPec, out err, out docnumber, nomeEmail, datareceived);
                                                                    if ((!msg_mail_elaborata) && err.Contains("CODINTEROP2"))
                                                                    {
                                                                        if (elabora_ogni_mail)
                                                                        {
                                                                            logger.Debug("eseguo senza segnatura");
                                                                            msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnatura(serverName, path, reg, infoUtente, ruolo, messageId, fatturaElDaPEC, mc, isPec, out err, out docnumber, nomeEmail, datareceived, mailAddress);

                                                                            if (isPec == "1")
                                                                                mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec;

                                                                            mailProcessed.ErrorMessage = "OK, Mail elaborata senza segnatura, perchè il file di segnatura non ha formato valido.";
                                                                            err = "";
                                                                        }
                                                                        else
                                                                            msg_mail_elaborata = false;
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    //Not protocolla_con_segnatura
                                                                    if (notifica_annullamento)
                                                                    {
                                                                        //notifica annullamento
                                                                        msg_mail_elaborata = InteroperabilitaNotificaAnnullamento.processaXmlAnnullamento(path, xmlFileName, reg, messageId, mc.from);
                                                                    }
                                                                    //Mail di tipo Eccezione
                                                                    else if (eccezione)
                                                                    {
                                                                        msg_mail_elaborata = interoperabilita.InteroperabilitaEccezioni.processaXmlEccezioni(path, xmlFileName, reg, infoUtente, messageId, mc, mailProcessed);
                                                                        // Modifica PEC 4 requisito 2
                                                                        // Qui il punto dove inserire la notifica di eccezione.
                                                                    }
                                                                    //Mail di tipo DSN
                                                                    else if (docNumMailDSN != null)
                                                                    {
                                                                        msg_mail_elaborata = interoperabilita.InteroperabilitaEccezioni.GestisciDSN(infoUtente, mailProcessed, docNumMailDSN, mc, messageId, path, nomeEmail, reg);
                                                                    }
                                                                    //Mail di tipo DSN
                                                                    else if (generataEccezione)
                                                                    {
                                                                        //Andrea De Marco - messaggio per controlli non bloccanti
                                                                        if (!controlloBloccante)
                                                                        {
                                                                            //Se eccezione non bloccante, tipo mail=signature
                                                                            mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Signature;

                                                                            if (!msg_mail_elaborata)
                                                                            {
                                                                                //Mail non elaborata
                                                                                mailProcessed.ErrorMessage = err;
                                                                            }
                                                                            else
                                                                            {
                                                                                //mailProcessed.ErrorMessage = "OK. Eccezione non bloccante.";
                                                                                mailProcessed.ErrorMessage = "OK. Eccezione non bloccante nella segnatura informatica";
                                                                            }

                                                                            err = "";
                                                                        }
                                                                        else
                                                                        {
                                                                            msg_mail_elaborata = false;
                                                                            mailProcessed.ErrorMessage = "Generata Eccezione. Si sono verificati errori nel controllo della mail in arrivo";
                                                                        }
                                                                        //End Andrea De Marco
                                                                        //mailProcessed.ErrorMessage = "Generata Eccezione. Si sono verificati errori nel controllo della mail in arrivo";
                                                                    }
                                                                    else
                                                                    {
                                                                        //Not notifica_annullamento
                                                                        if (elabora_ogni_mail)
                                                                        {
                                                                            msg_mail_elaborata = InteroperabilitaSegnatura.eseguiSenzaSegnatura(serverName, path, reg, infoUtente, ruolo, messageId, fatturaElDaPEC, mc, isPec, out err, out docnumber, nomeEmail, datareceived, mailAddress);

                                                                            if (isPec == "1")
                                                                                mailProcessed.ProcessedType = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType.Pec;
                                                                        }
                                                                        else
                                                                            msg_mail_elaborata = false;
                                                                    }
                                                                }
                                                            }

                                                            else
                                                            {
                                                                msg_mail_elaborata = InteroperabilitaControlloRicevute.processaXmlConferma(path, xmlFileName, reg, messageId, mc.from, out moreError);
                                                                // PEC 4 Modifica Maschera Caratteri

                                                            }

                                                        }
                                                        catch (Exception ex18)
                                                        {
                                                            if (!erroreNelMessaggio)
                                                                erroreNelMessaggio = true;
                                                            logger.Error("errore nell'esecuzione della segnatura: " + ex18.Message);
                                                            //new
                                                            if (!msg_mail_elaborata)
                                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                                {
                                                                    bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                    if (!moveImap)
                                                                    {
                                                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                    }
                                                                    else
                                                                    {
                                                                        spostato = true;
                                                                    }
                                                                }
                                                            //end new
                                                        }


                                                        try
                                                        {
                                                            if (msg_mail_elaborata)
                                                            {
                                                                if ((protocolla_con_segnatura) ||
                                                                    (conferma_ricezione) ||
                                                                    (notifica_annullamento) ||
                                                                    (elabora_ogni_mail) ||
                                                                    (eccezione) ||
                                                                    (daticert))
                                                                {

                                                                    bool elaboramail = false;
                                                                    try
                                                                    {
                                                                        if (tipoPosta == "IMAP")
                                                                        {
                                                                            bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, true) : svr.moveImap(i, true);
                                                                            if (moveImap)
                                                                                elaboramail = InteroperabilitaUtils.MailElaborata(messageId, "E", regId, docnumber, mc.from);
                                                                            else
                                                                            {
                                                                                mailProcessed.ErrorMessage = "Impossibile spostare la mail nella cartella elaborate";
                                                                                logger.Debug("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                            }
                                                                        }

                                                                    }
                                                                    catch (Exception ex24)
                                                                    {
                                                                        if (!erroreNelMessaggio)
                                                                            erroreNelMessaggio = true;
                                                                        if (!tipoPosta.Equals("POP") && !spostato)
                                                                        {
                                                                            bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                            if (!moveImap)
                                                                            {
                                                                                logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                            }
                                                                            else
                                                                            {
                                                                                spostato = true;
                                                                            }
                                                                        }
                                                                        logger.Error("errore nell'elaborazione della mail di tipo IMAP: " + ex24.Message);
                                                                    }


                                                                    try
                                                                    {
                                                                        if (tipoPosta == "POP")
                                                                            InteroperabilitaUtils.MailElaborata(messageId, "E", regId, docnumber, mc.from);
                                                                    }
                                                                    catch (Exception ex25)
                                                                    {
                                                                        if (!erroreNelMessaggio)
                                                                            erroreNelMessaggio = true;
                                                                        logger.Error("errore nell'elabarzione del messaggio di tipo POP: " + ex25.Message);
                                                                    }

                                                                    try
                                                                    {
                                                                        if (rimuovi_mail_elaborata &&
                                                                            !erroreNelMessaggio)
                                                                        {
                                                                            //svr.deleteSingleMessage(i);
                                                                            if (!string.IsNullOrEmpty(uidl))
                                                                                svr.deleteSingleMessage(uidl);
                                                                            else
                                                                                svr.deleteSingleMessage(i);
                                                                        }
                                                                    }
                                                                    catch (Exception e)
                                                                    {
                                                                        logger.Error(String.Format("Impossibile eliminare il messaggio [{0}] ({1})", mc.subject, e.Message));
                                                                    }
                                                                }
                                                                // causa errori nello scarico delle fatture, il metodo viene inserito in eseguiSegnatura ed eseguiSenzaSegnatura
                                                                //if(fatturaElDaPEC)
                                                                //{
                                                                //    BusinessLogic.Amministrazione.SistemiEsterni.FattElDaPEC(docnumber, infoUtente);

                                                                //}
                                                            }
                                                            else
                                                            {
                                                                try
                                                                {
                                                                    if (err.Contains("CODINTEROP1"))
                                                                    {
                                                                        err = err.Replace("CODINTEROP1", "Mail Elaborata.");
                                                                        mailProcessed.ErrorMessage = err;
                                                                    }
                                                                    else
                                                                        if (string.IsNullOrEmpty(mailProcessed.ErrorMessage))
                                                                    {
                                                                        if (string.IsNullOrEmpty(moreError))
                                                                        {
                                                                            mailProcessed.ErrorMessage = "Mail non elaborata. " + err;
                                                                        }
                                                                        else
                                                                        {
                                                                            mailProcessed.ErrorMessage = "Mail non elaborata. " + moreError;
                                                                        }
                                                                    }
                                                                    else
                                                                        mailProcessed.ErrorMessage = "Mail non elaborata. " + mailProcessed.ErrorMessage;

                                                                    //mailProcessed.ErrorMessage = "Mail non elaborata." + err;
                                                                    if (tipoPosta == "IMAP" && !spostato)
                                                                    {
                                                                        // svr.moveImap(i, false);
                                                                        bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                        if (!moveImap)
                                                                        {
                                                                            logger.Debug("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                        }
                                                                    }
                                                                }
                                                                catch (Exception ex21)
                                                                {
                                                                    if (!tipoPosta.Equals("POP") && !spostato)
                                                                    {
                                                                        bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                        if (!moveImap)
                                                                        {
                                                                            logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                        }
                                                                        else
                                                                        {
                                                                            spostato = true;
                                                                        }
                                                                    }
                                                                    logger.Error("errore nella mail non elaborata con la gestione del protocollo IMAP: " + ex21.Message);
                                                                }

                                                            }
                                                        }
                                                        catch (Exception ex20)
                                                        {
                                                            if (!tipoPosta.Equals("POP") && !spostato)
                                                            {
                                                                bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                                if (!moveImap)
                                                                {
                                                                    logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                                }
                                                                else
                                                                {
                                                                    spostato = true;
                                                                }
                                                            }
                                                            logger.Error("errore nell'elaborazione della mail recuperata: " + ex20.Message + " con id: " + messageId);
                                                        }


                                                        // si cancella la directory temporanea
                                                        DocsPaUtils.Functions.Functions.CancellaDirectory(path);
                                                    }
                                                    else //Se la firma della mail non è valida sposto tra le mail non elaborate
                                                    {
                                                        if (tipoPosta == "IMAP" && !spostato)
                                                        {
                                                            // svr.moveImap(i, false);
                                                            bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                            if (!moveImap)
                                                            {
                                                                logger.Debug("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                            }
                                                            else
                                                            {
                                                                spostato = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex7)
                                            {
                                                if (!tipoPosta.Equals("POP") && !spostato)
                                                {
                                                    bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                                    if (!moveImap)
                                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                                    else
                                                        spostato = true;
                                                }

                                                logger.Error("errore nella gestione della mail " + ex7.Message);

                                            }

                                        }
                                    }//try
                                    catch (Exception ex3)
                                    {

                                        logger.Error("errore durante lo scarico di una mail diversa da una ricevuta di ritorno -  errore: " + ex3.Message);
                                    }
                                }//else email diversa da notifica pec
                            }//try
                            catch (Exception ex)
                            {
                                if (!tipoPosta.Equals("POP") && !spostato)
                                {
                                    bool moveImap = !string.IsNullOrEmpty(uidl) ? svr.moveImap(uidl, false) : svr.moveImap(i, false);
                                    if (!moveImap)
                                    {
                                        logger.Error("errore nello spostamento della mail nella cartella elaborata, la mail: " + mc.subject);
                                    }
                                    else
                                    {
                                        spostato = true;
                                    }
                                }
                                logger.Error("errore nella notifiche della posta certificata -  errore: " + ex.Message);
                            }
                        }
                        else
                            logger.Error("non è posibile scaricare il messaggio: " + i + "il messaggio è stato cancellato o è corrotto");
                    }
                    catch (Exception e)
                    {
                        logger.Error(String.Format("Problemi di formato con il messaggio [{0}] ({1})", mc.subject, e.Message));

                        if (!e.Message.Contains("(connerr)"))
                        {
                            if (mailProcessed != null)
                            {
                                if (mailProcessed.ErrorMessage == string.Empty)
                                    mailProcessed.ErrorMessage = e.Message;

                                // Viene incrementato il numero delle mail processate non valide
                                retValue.InvalidMailProcessedCount++;
                                if(addToList)
                                    retValue.MailProcessedList.Add(mailProcessed);
                            }
                        }
                        else
                        {
                            mailProcessed.CountAttatchments = 0;
                            if ((mc != null) && (!string.IsNullOrEmpty(mc.subject)))
                            {
                                mailProcessed.Subject = mc.subject;
                            }
                            else
                                mailProcessed.Subject = "N.D.";

                            if (mc != null && mc.date != null)
                                mailProcessed.Date = mc.date;

                            //mailProcessed.Subject = "Errore di connessione al server di posta durante il download della mail";
                            mailProcessed.ErrorMessage = "Errore di connessione al server di posta durante il download della mail";
                            if (addToList)
                            {
                                // 1. aggiorno il contatore numero di mail processate in DPA_CHECK_MAILBOX
                                docsPaDB.MailProcessedCheckMailbox(idCheckMailbox, (i).ToString());
                                mailProcessed.ErrorMessage.Replace("<BR>", "");
                                //2. aggiungo il record sulla mail processata in DPA_REPORT_MAILBOX
                                docsPaDB.CreateCheckMailboxReport(idCheckMailbox, mailProcessed);
                                //3. prova conteggio mail in log
                                retValue.MailProcessedList.Add(mailProcessed);
                            }
                        }

                    }

                    if (mailProcessed != null && checkId)
                    {
                        if (addToList)
                        {
                            // 1. aggiorno il contatore numero di mail processate in DPA_CHECK_MAILBOX
                            docsPaDB.MailProcessedCheckMailbox(idCheckMailbox, i.ToString());
                            mailProcessed.ErrorMessage.Replace("<BR>", "");
                            //2. aggiungo il record sulla mail processata in DPA_REPORT_MAILBOX
                            docsPaDB.CreateCheckMailboxReport(idCheckMailbox, mailProcessed);
                            //3. prova conteggio mail in log
                            retValue.MailProcessedList.Add(mailProcessed);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nel controllo dei messaggi. " + e.Message);

                if (retValue.ErrorMessage == string.Empty)
                    retValue.ErrorMessage = e.Message;

            }
            finally
            {
                try
                {
                    // svr.imap.Expunge();
                    if (!tipoPosta.Equals("POP"))
                        //svr.cancellaMailImap();
                        if (svr.cancellaMailImap())
                            logger.Debug("sono state cancellate tutte le mail dalla inbox");
                        else
                            logger.Debug("non sono state cancellate tutte le mail dalla inbox");
                    svr.disconnect();
                }
                catch (Exception e)
                {
                    logger.Error("Disconnessione dal server " + server + " fallita:" + e.Message);

                    if (retValue.ErrorMessage == string.Empty)
                        retValue.ErrorMessage = e.Message;
                }

                // Se almeno un messaggio di posta non è stato processato correttamente,
                // l'esito generale del controllo viene invalidato
                if (retValue.ErrorMessage == string.Empty && retValue.InvalidMailProcessedCount > 0)
                    retValue.ErrorMessage = "Si sono verificati degli errori nel controllo dei messaggi di posta.";
            }
            if (retValue != null && retValue.MailProcessedList != null)
            {
                logger.Debug("retValue.MailProcessedList: "+retValue.MailProcessedList.Count);
            }
            // Ordinamento delle singole mail per data decrescente
            retValue.MailProcessedList.Sort(new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedComparer());

            return retValue;
        }

        /// <summary>
        /// restituisce l'elenco dei record nella DPA_CHECK_MAILBOX filtrando per emails passate come valore di input
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        public static List<DocsPaVO.Interoperabilita.InfoCheckMailbox> InfoCheckMailbox(List<string> emails)
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita docsPaDB = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            return docsPaDB.InfoCheckMailbox(emails);
        }

        /// <summary>
        /// restituisce true se l'eiminazione delle informazioni sul report è andata a buon fine.
        /// </summary>
        /// <param name="idCheckmailbox"></param>
        /// <returns></returns>
        public static bool RemoveReportMailbox(string idCheckmailbox)
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita docsPaDB = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            return docsPaDB.RemoveReportMailbox(idCheckmailbox);
        }

        /// <summary>
        /// Restituisce l'oggetto MailAccountCheckResponse contenente le informazioni sul report associato alla mailbox da consultare
        /// </summary>
        /// <param name="idCheckMailbox"></param>
        /// <returns></returns>
        public static DocsPaVO.Interoperabilita.MailAccountCheckResponse InfoReportMailbox(string idCheckMailbox)
        {
            DocsPaDB.Query_DocsPAWS.Interoperabilita docsPaDB = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            return docsPaDB.InfoReportMailbox(idCheckMailbox);
        }
        #endregion
    }
}
