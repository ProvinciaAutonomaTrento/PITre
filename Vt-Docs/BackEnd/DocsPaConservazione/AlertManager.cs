using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DocsPaDB;
using DocsPaDB.Query_DocsPAWS;
using DocsPaUtils;
using DocsPaVO.utente;
using log4net;

namespace DocsPaConservazione
{
    //MEV CS 1.5
    //Classe per la gestione delle operazioni relative
    //agli alert della conservazione
    public class AlertManager
    {

        #region task asincrono invio mail

        private static ILog logger = LogManager.GetLogger(typeof(AlertManager));

        public AlertManager()
        {

        }

        private bool _isInEsecuzioneAsync = false;
        private InfoUtente _infoUtenteAsync = null;
        private string _codiceAsync = string.Empty;

        private AlertManager(InfoUtente infoUtente, string codice)
        {
            this._infoUtenteAsync = infoUtente;
            this._codiceAsync = codice;
        }

        private delegate bool InvioAlertDelegate(InfoUtente infoUtente, string codice, string idConservazione, string idSupporto);

        private static Dictionary<string, AlertManager> _am = new Dictionary<string, AlertManager>();

        private static string GetKey(string idPeople, string codice)
        {
            return string.Format("{0}§{1}", idPeople, codice);
        }

        public static void InvioAlertAsync(InfoUtente infoUtente, string codice, string idConservazione, string idSupporto)
        {
            string key = GetKey(infoUtente.idPeople, codice);
            logger.Debug("Inizio - " + key);
            try
            {
                if (!_am.ContainsKey(key))
                {
                    AlertManager manager = new AlertManager(infoUtente, codice);
                    manager._isInEsecuzioneAsync = true;

                    //avvio task asincrono
                    InvioAlertDelegate del = new InvioAlertDelegate(manager.InvioAlert);
                    IAsyncResult result = del.BeginInvoke(infoUtente, codice, idConservazione, idSupporto, new AsyncCallback(manager.InvioAlertExecuted), manager);

                    _am.Add(key, manager);
                }
                else
                {
                    //il task è già in esecuzione
                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore: {0} stk {1}", ex.Message, ex.StackTrace);
                throw ex;
            }
        }

        private void InvioAlertExecuted(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                AlertManager am = (AlertManager)result.AsyncState;

                string key = GetKey(am._infoUtenteAsync.idPeople, am._codiceAsync);
                if (_am.ContainsKey(key))
                {
                    _am.Remove(key);
                }
            }
        }

        public bool InvioAlert(InfoUtente infoUtente, string codice, string idConservazione, string idSupporto)
        {
            bool result = false;
            bool inviaMail = false;

            //paramtri server SMTP
            string serverSMTP = string.Empty;
            string portaSMTP = string.Empty;
            string useSSL = string.Empty;
            string username = string.Empty;
            string password = string.Empty;

            //parametri mail
            string mailFrom = string.Empty;
            string mailTo = string.Empty;
            string subject = string.Empty;
            string body = string.Empty;

            //stringa parametri contatore
            string parContatore = string.Empty;

            try
            {
                //per gli alert con contatore (LEGGIBILITA_SINGOLO, DOWNLOAD, SFOGLIA)
                //incremento il contatore e verifico se ha raggiunto la soglia nel periodo di monitoraggio
                if (codice == "LEGGIBILITA_SINGOLO" || codice == "DOWNLOAD" || codice == "SFOGLIA")
                {
                    //prima di eseguire la SP recupero le informazioni di monitoraggio
                    //la data è necessaria per la mail di alert
                    parContatore = this.GetContatoreInfo(codice, infoUtente.idAmministrazione, infoUtente.idPeople);

                    string check = this.CheckContatoreAlert(infoUtente, codice);
                    if (check == "1")
                        inviaMail = true;
                    else if (check == "-1")
                        throw new Exception("Errore nel reperimento dei contatori per l'alert " + codice);
                }
                //per gli alert senza contatore (LEGGIBILITA_ANTICIPATA, LEGGIBILITA_PERC) le condizioni per l'invio
                //vengono verificate da frontend
                //il metodo viene invocato solo quando queste sono soddisfatte
                else
                {
                    parContatore = idConservazione;
                    inviaMail = true;
                }

                if (inviaMail)
                {

                    //reperimento dei parametri di configurazione della casella mittente
                    DocsPaVO.amministrazione.AlertConservazione mailParam = this.GetParametriMail(infoUtente.idAmministrazione);
                    if (!(mailParam != null))
                        throw new Exception("Errore nel reperimento della configurazione del server SMTP per l'amministrazione " + infoUtente.idAmministrazione);

                    serverSMTP = mailParam.serverSMTP;
                    portaSMTP = mailParam.portaSMTP;
                    useSSL = mailParam.chaSSL;
                    username = mailParam.userID;
                    password = mailParam.pwd;

                    mailFrom = mailParam.fromField;
                    mailTo = mailParam.toField;
                    
                    //costruzione del messaggio
                    subject = this.getOggettoMail(codice);
                    body = this.getBodyMail(codice, infoUtente, parContatore);

                    //invio della mail
                    BusinessLogic.Interoperabilità.SvrPosta svr = new BusinessLogic.Interoperabilità.SvrPosta(serverSMTP, username, password, portaSMTP, System.IO.Path.GetTempPath(), BusinessLogic.Interoperabilità.CMClientType.SMTP, useSSL, string.Empty);
                    svr.connect();

                    svr.sendMail(mailFrom, mailTo, subject, body);

                    //esito positivo
                    result = true;
                }
                else
                {
                    //per alert con contatore
                    //non sono state soddisfatte le condizioni per l'invio
                    result = true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = false;
            }

            return result;
        }

        #endregion

        //private string VerificaAlert(InfoUtente infoUtente, string codice)
        //{
        //    string result = string.Empty;
        //    try
        //    {
        //        using (DBProvider dbProvider = new DBProvider())
        //        {
        //            Query query = InitQuery.getInstance().getQuery("S_ALERT_CONS_CHECK");
        //            query.setParam("idAmm", infoUtente.idAmministrazione);
        //            query.setParam("userid", infoUtente.idPeople);
        //            query.setParam("codice", codice);

        //            string commandText = query.getSQL();
        //            string field = string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return result;
        //}

        #region COSTRUZIONE MESSAGGIO

        /// <summary>
        /// Restituisce l'oggetto della mail relativa all'alert specificato.
        /// </summary>
        /// <param name="codice"></param>
        /// <returns></returns>
        private string getOggettoMail(string codice)
        {
            string oggetto = string.Empty;

            switch (codice)
            {
                case "LEGGIBILITA_ANTICIPATA":
                    oggetto = "Esecuzione della verifica di leggibilità in anticipo rispetto ai termini";
                    break;

                case "LEGGIBILITA_PERC":
                    oggetto = "Esecuzione della verifica di leggibilità su un campione di dimensioni non consentite";
                    break;

                case "LEGGIBILITA_SINGOLO":
                    oggetto = "Superamento dei limiti di utilizzo della funzione 'Verifica leggibilità singolo documento'";
                    break;

                case "DOWNLOAD":
                    oggetto = "Superamento dei limiti di utilizzo della funzione 'Download istanza'";
                    break;

                case "SFOGLIA":
                    oggetto = "Superamento dei limiti di utilizzo della funzione 'Sfoglia istanza'";
                    break;
            }

            return oggetto;
        }

        /// <summary>
        /// Metodo per la costruzione del corpo della mail
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="utente"></param>
        /// <param name="param">Parametri relativi all'alert</param>
        /// <returns></returns>
        private string getBodyMail(string codice, InfoUtente utente, string param)
        {
            string testo = string.Empty;
            string dataInizio = string.Empty;

            //recupero i parametri relativi all'alert
            //string parametri = this.getParametriAlert(utente.idAmministrazione, codice);

            //dati utente CS da inserire nel messaggio
            Utente user = BusinessLogic.Utenti.UserManager.getUtenteById(utente.idPeople);
            string nome = user.nome + " " + user.cognome;

            //amministrazione
            string amministrazione = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(utente.idAmministrazione).Descrizione;

            //costruzione corpo del messaggio
            testo = "Si segnala che l'utente " + nome;
            switch (codice)
            {
                case "LEGGIBILITA_ANTICIPATA":
                    testo = testo + ", in data " + DateTime.Now.ToString("dd/MM/yyyy") +
                            ", ha eseguito un'operazione di verifica di leggibilità sull'istanza di conservazione numero " + param +
                            " dell'Amministrazione " + amministrazione + " anticipatamente rispetto ai termini di scadenza previsti.";
                    break;

                case "LEGGIBILITA_PERC":
                    testo = testo + ", in data " + DateTime.Now.ToString("dd/MM/yyyy") + " ha eseguito un'operazione di " +
                                    "verifica di leggibilità sull'istanza di conservazione numero " + param + " dell'Amministrazione " 
                                    + amministrazione + " selezionando un campione da verificare di dimensioni maggiori rispetto a quelle consentite.";
                    break;

                case "LEGGIBILITA_SINGOLO":
                    if (!string.IsNullOrEmpty(param))
                        dataInizio = param.Split('§')[2];
                    testo = testo + ", nel periodo dal " + dataInizio + " al " + DateTime.Now.ToString("dd/MM/yyyy") + 
                                    ", ha superato la frequenza consentita per l'utilizzo della funzione di verifica " +
                                    "leggibilità di un singolo documento dell'Amministrazione " + amministrazione + ".";
                    break;

                case "DOWNLOAD":
                    if (!string.IsNullOrEmpty(param))
                        dataInizio = param.Split('§')[2];
                    testo = testo + ", nel periodo dal " + dataInizio + " al " + DateTime.Now.ToString("dd/MM/yyyy") + 
                                    ", ha superato la frequenza consentita per l'utilizzo della funzione di download delle " +
                                    "istanze di conservazione dell'Amministrazione "+amministrazione+".";
                    break;

                case "SFOGLIA":
                    if (!string.IsNullOrEmpty(param))
                        dataInizio = param.Split('§')[2];
                    testo = testo + ", nel periodo dal " + dataInizio + " al " + DateTime.Now.ToString("dd/MM/yyyy") +
                                    ", ha superato la frequenza consentita per l'utilizzo della funzione di consultazione " +
                                    "delle istanze di conservazione dell'Amministrazione " + amministrazione + ".";
                    break;
            }

            return testo;
        }

        #endregion

        #region UTILS

        /// <summary>
        /// Determina se un dato alert è attivo per l'amministrazione corrente
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        public bool isLogAttivo(string idAmm, string codice)
        {
            bool retVal = false;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    Query query = InitQuery.getInstance().getQuery("S_ALERT_CONS_IS_ATTIVO");
                    query.setParam("idAmm", idAmm);
                    query.setParam("codice", codice);

                    string result = string.Empty;
                    string commandText = query.getSQL();
                    logger.Debug(commandText);

                    if (!dbProvider.ExecuteScalar(out result, commandText))
                        throw new Exception("Errore nell'esecuzione della query");

                    if (result == "1")
                        retVal = true;

                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                }

                return retVal;
            }


        }

        /// <summary>
        /// Determina se un dato alert è attivo per l'amministrazione corrente
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        public bool isAlertAttivo(string idAmm, string codice)
        {
            bool retVal = false;
            string param = string.Empty;
            Query query = InitQuery.getInstance().getQuery("S_ALERT_CONS_IS_ATTIVO");

            switch (codice)
            {
                case "LEGGIBILITA_ANTICIPATA":
                    param = "cha_alert_leggibilita_scadenza";
                    break;

                case "LEGGIBILITA_PERC":
                    param = "cha_alert_leggibilita_max_doc";
                    break;

                case "LEGGIBILITA_SINGOLO":
                    param = "cha_alert_leggibilita_sing";
                    break;

                case "DOWNLOAD":
                    param = "cha_alert_download";
                    break;

                case "SFOGLIA":
                    param = "cha_alert_sfoglia";
                    break;

            }

            query.setParam("idAmm", idAmm);
            query.setParam("codice", param);

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string field = string.Empty;
                    string commandText = query.getSQL();
                    if (!dbProvider.ExecuteScalar(out field, commandText))
                        throw new Exception("Errore nell'esecuzione della query.");

                    if (field == "1")
                        retVal = true;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                logger.Debug(ex.Message);
            }

            return retVal;
        }

        /// <summary>
        /// Ricava i parametri di configurazione per l'alert e l'amministrazione richiesti
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        public string getParametriAlert(string idAmm, string codice)
        {
            string retVal = string.Empty;
            Query query = InitQuery.getInstance().getQuery("S_ALERT_CONS_GET_PARAMETRI");
            string paramToExtract = string.Empty;

            switch (codice)
            {
                case "LEGGIBILITA_PERC":
                    paramToExtract = " num_leggibilita_max_doc_perc ";
                    break;

                case "LEGGIBILITA_SINGOLO":
                    paramToExtract = " num_legg_sing_max_oper ||'_'|| num_legg_sing_periodo_mon ";
                    break;

                case "DOWNLOAD":
                    paramToExtract = " num_download_max_oper ||'_'|| num_download_periodo_mon ";
                    break;

                case "SFOGLIA":
                    paramToExtract = "num_sfoglia_max_oper ||'_'|| num_sfoglia_periodo_mon ";
                    break;
            }

            query.setParam("idAmm", idAmm);
            query.setParam("params", paramToExtract);

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string commandText = query.getSQL();
                    logger.Debug(commandText);

                    if (!dbProvider.ExecuteScalar(out retVal, commandText))
                        throw new Exception("Errore nell'esecuzione della query.");
                }
            }
            catch (Exception ex)
            {
                retVal = string.Empty;
                logger.Debug(ex.Message);
            }

            return retVal;
        }

        /// <summary>
        /// Controlla per gli alert con contatore se sono verificate le condizioni per l'invio della mail
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="codice"></param>
        /// <returns>
        /// 1: soglia raggiunta nel periodo di monitoraggio > invia l'alert
        /// 0: soglia non raggiunta nel periodo di monitoraggio > non inviare l'alert
        /// -1: errore nella funzione
        /// </returns>
        public string CheckContatoreAlert(InfoUtente infoUtente, string codice)
        {
            Conservazione cons = new Conservazione();
            return cons.IncrementaContatoriAlertConservazione(infoUtente, codice);
        }

        /// <summary>
        /// Recupera i parametri di configurazione della casella mittente
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        private DocsPaVO.amministrazione.AlertConservazione GetParametriMail(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.GetGestioneAlert(idAmm);
        }

        /// <summary>
        /// Estrae per un dato utente i dati relativi al monitoraggio delle operazioni con contatore
        /// nel formato 
        /// codice alert § contatore § data inizio monitoraggio
        /// </summary>
        /// <param name="codice">alert di cui si richiede il contatore</param>
        /// <param name="idAmm"></param>
        /// <param name="idUtente"></param>
        /// <returns></returns>
        public string GetContatoreInfo(string codice, string idAmm, string idUtente)
        {
            string retVal = string.Empty;

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query query = InitQuery.getInstance().getQuery("S_CONSERVAZIONE_ALERT_GET_CONTATORI");
                    query.setParam("codice", codice);
                    query.setParam("idAmm", idAmm);
                    query.setParam("userId", idUtente);

                    string commandText = query.getSQL();
                    logger.Debug(commandText);

                    if(!dbProvider.ExecuteScalar(out retVal, commandText))
                        throw new Exception("Errore nel recupero dei contatori per l'utente " + idUtente + " dell'amministrazione " + idAmm);

                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                retVal = string.Empty;
            }

            return retVal;
        }

        /// <summary>
        /// Determina se una verifica di leggibilità viene eseguita prima dei termini previsti
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <param name="idSupporto"></param>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public bool isVerificaLeggibilitaAnticipata(string idConservazione, string idSupporto, string idAmm)
        {
            bool result = false;

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    Query query = InitQuery.getInstance().getQuery("S_CONSERVAZIONE_GET_DATA_PROX_VERIFICA_LEGG");
                    query.setParam("idCons", idConservazione);
                    query.setParam("idSupporto", idSupporto);

                    string dataVer = string.Empty;

                    if (!dbProvider.ExecuteScalar(out dataVer, query.getSQL()))
                        throw new Exception("Errore nel reperimento della data di prossima verifica.");

                    //data di prossima verifica leggibilità
                    DateTime dataVerifica = DateTime.Parse(dataVer);

                    //intervallo in giorni per l'invio della notifica
                    int giorniNotifica = Convert.ToInt32(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_CONS_VER_LEG_GG_NOTIFICHE"));

                    //confronto tra le due date
                    int value = DateTime.Compare(DateTime.Now.AddDays(giorniNotifica), dataVerifica);
                    if (value < 0)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                return false;
            }

            return result;
        }

        #endregion
    }
}
