using System;
using System.Xml;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web.Mail;
using System.Collections;
using System.Text.RegularExpressions;
using DocsPaVO.Interoperabilita;
using System.Linq;
using System.Web;
using DocsPaUtils.Security;
using log4net;
using System.Collections.Generic;
using BusinessLogic.interoperabilita.Semplificata;
using Interoperability.Domain;
using DocsPaVO.utente;
using Interoperability.Controller;
using Interoperability.Service.Library.InteroperabilityService;
using Interoperability.Domain.FaultException;
using System.ServiceModel;
using BusinessLogic.interoperabilita.Semplificata.Exceptions;
using BusinessLogic.NotificationCenter;
using BusinessLogic.Amministrazione;
using BusinessLogic.Utenti;
using DocsPaVO.Spedizione;
using BusinessLogic.Spedizione;

namespace BusinessLogic.Interoperabilità
{

    /// <summary>
    /// </summary>
    public class InteroperabilitaInvioSegnatura
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaInvioSegnatura));
        private static System.Threading.Mutex semInterOper = new System.Threading.Mutex();
        private static DocsPaVO.utente.Registro getRegistroInteropByCodAOO(string codAoo, string idAmministrazione)
        {
            DocsPaVO.utente.Registro reg = null;

            reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCodAOO(codAoo, idAmministrazione);
            return reg;
        }
        /// <summary>
        /// metodo che ritorna un Infoutente per l'interoperabilità Interni senza mail.
        /// utilizza il ruolo e un utente ricavati dalla DPA_EL_REGISTRO:ID_RUOLO_AOO,ID_PEOPLE_AOO
        /// di solito l'utente che utilizza docspa non è lo stesso utente della ID_PEOPLE_AOO,
        /// quindi si fa una login con questo utente e si calcola dall'oggetto utente così ottenuto
        /// l'infoutente di ritorno.
        /// </summary>
        /// <param name="ruolo"></param>
        /// <param name="ut"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.InfoUtente getInfoUtenteInterOp(DocsPaVO.utente.Ruolo ruolo,
            DocsPaVO.utente.Utente ut, out bool loggato)
        {
            loggato = false;
            DocsPaVO.utente.InfoUtente infoInterOp = null;

            #region calcola infoUtenteInterop

            DocsPaDocumentale.Documentale.AdminPasswordConfig pwdConfig = new DocsPaDocumentale.Documentale.AdminPasswordConfig();
            if (pwdConfig.IsSupportedPasswordConfig())
            {
                // Se è attivata la gestione delle configurazioni delle password,
                // non è possibile reperire e fornire la password per l'utente (è criptata).
                // Non viene effettuata la login.

            }
            else
            {
                string library = DocsPaDB.Utils.Personalization.getInstance(ut.idAmministrazione).getLibrary();
                BusinessLogic.Utenti.UserManager u = new BusinessLogic.Utenti.UserManager();
                string password = u.getPassword(ut.idPeople);
                DocsPaVO.utente.UserLogin login = new DocsPaVO.utente.UserLogin(ut.userId, password, ut.idAmministrazione);
                DocsPaVO.utente.UserLogin.LoginResult lr = new DocsPaVO.utente.UserLogin.LoginResult();
                string ipAddress = "";
                //TODO: se utente già loggato, allora usare dst...altrimenti login
                string DST = u.getDST(ut.userId);

                if (!(DST != null && DST != ""))
                {
                    ut = BusinessLogic.Utenti.Login.loginMethod(login, out lr, true, "127.0.0.1", out  ipAddress);
                }
                else
                {
                    ut.dst = DST; loggato = true;
                }
            }

            if (ut != null)
            {
                infoInterOp = new DocsPaVO.utente.InfoUtente(ut, ruolo);
                //aggiungo DST necessario per DOCUMENTUM, ma utile anche in ETDOCS per scopi futuri.
                DocsPaDocumentale.Documentale.UserManager um = new DocsPaDocumentale.Documentale.UserManager();
                infoInterOp.dst = um.GetSuperUserAuthenticationToken();
            }

            #endregion

            return infoInterOp;

        }
        /// <summary>
        /// interop no mail: In partica spedisce un documento al corr, crea il pedisposto che ne
        /// risulterebbe da una eventuale operazione di controllo casella istituzionale e spedisce le trasmissioni
        /// ai ruolo con PRAU.
        /// </summary>
        /// <param name="corr"></param>
        ///<param name="schedaDocumento"></param>
        /// <param name="err">messaggio di errore o di successo. se errore la prima parola del messaggio è "errore"</param>
        /// <returns>true se OK; false se KO</returns>
        public static bool SendDocumentInteropNoMail(DocsPaVO.utente.Corrispondente corr, DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.utente.InfoUtente infoUtente, out string err, out DocsPaVO.Interoperabilita.DatiInteropAutomatica dia)
        {
            err = "";
            bool result = false; //presume insuccesso !
            bool utenteLoggato = false;
            DocsPaVO.utente.InfoUtente infoutenteInterOp = null;
            DocsPaVO.utente.Utente ut = null;
            dia = null;

            //using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            // {
            try
            {
                semInterOper.WaitOne();

                //TODO multi AMM, possono avere stessi codiceAOO ?
                DocsPaVO.utente.Registro reg = getRegistroInteropByCodAOO(corr.codiceAOO, corr.idAmministrazione);
                if (reg == null)
                {
                    err = "Errore, Nessun Registro Associato all'AOO " + corr.descrizione;
                }
                // crea ruolo per interop
                DocsPaVO.utente.Ruolo ruolo = null;
                if (reg.idRuoloAOO != null && reg.idRuoloAOO != "")
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuolo(reg.idRuoloAOO);

                }
                else
                {
                    err = "Errore, Il Ruolo responsabile dell'AOO (ID_RUOLO_AOO) non è valorizzato";
                }
                if (ruolo == null)
                {
                    err = "Errore, Il Ruolo responsabile dell'AOO destinataria (ID_RUOLO_AOO) non è valorizzato";
                }
                else //else ruolo
                {
                    //caso raro, sto facendo l'inertop con lo stesso utente che è dest e mitt  contemporaneamente
                    if (reg.idUtenteAOO != null && reg.idUtenteAOO != ""
                        && infoUtente.idPeople.Equals(reg.idUtenteAOO)
                        && infoUtente.dst != null
                        && infoUtente.dst != "")
                    {

                        infoutenteInterOp = infoUtente; utenteLoggato = true;
                        logger.Debug("inzio Procedura interop no mail interni per AOO " + corr.codiceAOO);
                        //*****************************************************************************************************//
                        // MODIFICA GIORDANO IACOZZILLI 10052012
                        //

                        // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento interop interna solo a UO destinataria della spedizione e non a tutta la AOO
                        // result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                        result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty, corr);

                        //result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                        
                        
                        //
                        //OLD CODE:
                        //  result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                        //*****************************************************************************************************//
                    }
                    else
                    {	//calcolo utente
                        ut = new DocsPaVO.utente.Utente();
                        //questo metodo non torna mai null, ma un oggetto con tutti gli attributi vuoti.

                        // S. Furnari - 16/01/2013 - Se idUtenteAOO non è valorizzato, magari per errori di configurazione
                        // deve essere segnalato un opportuno errore.
                        //ut = BusinessLogic.Utenti.UserManager.getUtente(reg.idUtenteAOO);
                        ////quindi controllo almeno se idPeople!=null penso basti.
                        //if (ut != null && ut.idPeople == null)
                        //{
                        //    err = "Attenzione, L'utente responsabile dell'AOO destinataria (ID_PEOPLE_AOO) non è valorizzato oppure è stato disabilitato.";
                        //}
                        if (!String.IsNullOrEmpty(reg.idUtenteAOO))
                            ut = BusinessLogic.Utenti.UserManager.getUtente(reg.idUtenteAOO);

                        if (ut != null && String.IsNullOrEmpty(ut.idPeople))
                        {
                            err = "Attenzione, L'utente responsabile dell'AOO destinataria (ID_PEOPLE_AOO) non è valorizzato oppure è stato disabilitato.";
                            // S. Furnari - 8/1/2013 - Se è arrivato qui, non deve andare avanti
                            return false;
                        }










                    }
                    if (ut != null)
                    {
                        //calcolo infoutenteInertOp
                        //ricava l'infoutente del ruolo_AOO/utente_AOO operando una login per essere suciri che tale 
                        //utente risulti il creatore prorpietario del documento predisposto
                        infoutenteInterOp = getInfoUtenteInterOp(ruolo, ut, out utenteLoggato);
                        if (infoutenteInterOp == null)
                        {
                            err = "Attenzione, non è stato possibile effettuare login con l'utente" + ut.userId + " per interoperare con il registro " + reg.codRegistro;
                        }
                        else
                        {
                            logger.Debug("inzio Procedura interop no mail interni per AOO " + corr.codiceAOO);
                            infoutenteInterOp.urlWA = infoUtente.urlWA;
                            //*****************************************************************************************************//
                            // MODIFICA GIORDANO IACOZZILLI 10052012
                            //
                            //modifica furnari
                            result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty, corr);
                            //result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoUtente, infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                            //
                            //OLD CODE:
                            //  result = BusinessLogic.Interoperabilità.InteroperabilitaSegnatura.eseguiSegnaturaNoMail(infoutenteInterOp.urlWA, reg, infoutenteInterOp, ruolo, schedaDocumento, out err, out dia, string.Empty);
                            //*****************************************************************************************************//
           
                            //effettuo la trasmissione dei documenti creati su registro automatico
                            // Trasmissioni.
                        }
                    } //FINE else infoutente
                }//FINE else ruolo
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                if (err == "")
                    err = "Attenzione, la spedizione al destinatario non riuscita.";
                result = false;
            }
            finally
            {
                if (infoutenteInterOp != null
                    && infoutenteInterOp.dst != null && !utenteLoggato) //se sono loggatto faccio logoff.
                    BusinessLogic.Utenti.Login.logoff(ut.userId, ut.idAmministrazione, ut.dst);

                semInterOper.ReleaseMutex();
            }

            //    if (result)
            //    {
            //        transactionContext.Complete();
            //    }
            //}

            return result;
        }
        /// <summary>
        /// Invio di un documento a tutti i destinatari
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoUtente"></param>
        /// <param name="confermaRicezione"></param>
        /// <returns></returns>

        public static SendDocumentResponse SendDocument(
                    DocsPaVO.documento.SchedaDocumento schedaDocumento,
                    DocsPaVO.utente.Registro registroMittente,
                    DocsPaVO.utente.InfoUtente infoUtente,
                    bool confermaRicezione,
                    out DocsPaVO.Interoperabilita.DatiInteropAutomatica dia)
        {
            dia = null;

            SendDocumentResponse retValue = new SendDocumentResponse();
            retValue.SendDateTime = DateTime.Now;
            retValue.SchedaDocumento = schedaDocumento;

            ArrayList listDestinatari = new ArrayList();
            listDestinatari.AddRange(((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).destinatari);
            listDestinatari.AddRange(((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza);

            Hashtable tableDestinatari = dividiDestinatari(listDestinatari, schedaDocumento.registro.codRegistro, schedaDocumento.registro.email, retValue, infoUtente.idAmministrazione);

            string mail = string.Empty;

            foreach (DictionaryEntry item in tableDestinatari)
            {
                // nuova gestione interop interno, mi basta il primo della evenutale lista dei dest. interop sulla stessa AOO
                DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)((ArrayList)item.Value)[0];

                string err = "";

                //per ottenere il codAOO e codAMM in caso di ruolo/utente
                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                {

                    corr = ((DocsPaVO.utente.Ruolo)corr).uo;
                }
                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                {
                    corr = (DocsPaVO.utente.Utente)corr;

                }


                if (InteroperabilitaUtils.InteropIntNoMail  //ATTIVATA/NON ATTIVATA INTEROP INTERNI NO MAIL
                    && corr != null
                    && corr.tipoIE != null
                    && corr.tipoIE == "I" //SOLO PER CORR: INTERNI ALL'AMM.
                    && !corr.codiceAOO.ToUpper().Equals(schedaDocumento.registro.codRegistro.ToUpper()) //NO SE RISIEDE NELLA MIA AOO
                    && corr.idAmministrazione.Trim().ToLower().Equals(schedaDocumento.registro.idAmministrazione.Trim().ToLower()))
                {
                    DocsPaVO.utente.Registro reg;
                    bool result = SendDocumentInteropNoMail(corr, schedaDocumento, infoUtente, out err, out dia);
                    DocsPaVO.utente.Corrispondente corrToAdd = null;
                    if (!result)
                    {
                        SendDocumentResponse.SendDocumentMailResponse r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(corr.email, false, err);
                        ArrayList corrs = (ArrayList)item.Value;
                        for (int i = 0; i < corrs.Count; i++)
                        {
                            corrToAdd = (DocsPaVO.utente.Corrispondente)corrs[i];
                            corrToAdd.codiceAOO = GetCodiceAOODestInterop(corrToAdd);
                            r.Destinatari.Add(corrToAdd);
                        }
                        //r.Destinatari.AddRange(corrs);
                        retValue.SendDocumentMailResponseList.Add(r);
                    }
                    else
                    {
                        logger.Debug("Fine Procedura interop no mail interni per AOO " + corr.codiceAOO + " con successo");
                        SendDocumentResponse.SendDocumentMailResponse r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(corr.email, true, "OK");


                        ArrayList corrs = (ArrayList)item.Value;
                        for (int i = 0; i < corrs.Count; i++)
                        {
                            corrToAdd = (DocsPaVO.utente.Corrispondente)corrs[i];
                            corrToAdd.codiceAOO = GetCodiceAOODestInterop(corrToAdd);
                            r.Destinatari.Add(corrToAdd);
                            //r.Destinatari.Add((DocsPaVO.utente.Corrispondente)corrs[i]);

                        }

                        //se i dati aggiuntivi dell'interoperabilità sono presenti..
                        if (dia != null)
                            r.datiInteropAutomatica = dia;

                        retValue.SendDocumentMailResponseList.Add(r);
                    }
                }
                else
                {

                    retValue.SendDocumentMailResponseList.Add(SendDocumentMail(schedaDocumento,
                                                        registroMittente,
                                                        (string)item.Key,
                                                        (ArrayList)item.Value,
                                                        infoUtente,
                                                        confermaRicezione));

                }
            }

            string sysForUpdate = string.Empty;

            for (int i = 0; i < retValue.SendDocumentMailResponseList.Count; i++)
            {
                SendDocumentResponse.SendDocumentMailResponse SmR = (SendDocumentResponse.SendDocumentMailResponse)retValue.SendDocumentMailResponseList[i];
                if (SmR.SendSucceded)//solo se la spedizione ha successo, faccio update sulla dpa_stato_invio
                    for (int j = 0; j < SmR.Destinatari.Count; j++)
                    {
                        DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)SmR.Destinatari[j];
                        if (sysForUpdate != string.Empty)
                            sysForUpdate += ", ";

                        sysForUpdate += corr.systemId;

                    }

            }

            //foreach (DictionaryEntry item in tableDestinatari)
            //{
            //    ArrayList listCorrispondenti=item.Value as ArrayList;

            //    if (listCorrispondenti!=null)
            //    {
            //        foreach (DocsPaVO.utente.Corrispondente corrispondente in listCorrispondenti)
            //        {
            //            if (sysForUpdate!=string.Empty)
            //                sysForUpdate+=", ";

            //            sysForUpdate += corrispondente.systemId;
            //        }
            //    }
            //}

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.UpdateDtaSpedizione(sysForUpdate, schedaDocumento.systemId, sysForUpdate);



            return retValue;
        }
        private static string GetCodiceAOODestInterop(DocsPaVO.utente.Corrispondente corrispondente)
        {
            string rtn = string.Empty;
            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
            {
                if (corrispondente.codiceAOO != null)
                    rtn = corrispondente.codiceAOO;

            }
            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
            {
                DocsPaVO.utente.Ruolo ruo = ((DocsPaVO.utente.Ruolo)corrispondente);
                if (ruo.uo.codiceAOO != null)
                    rtn = ruo.uo.codiceAOO;
            }
            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
            {
                DocsPaVO.utente.Utente ut = ((DocsPaVO.utente.Utente)corrispondente);
                if (ut.ruoli.Count > 0)
                {
                    DocsPaVO.utente.Ruolo ruo = ((DocsPaVO.utente.Ruolo)ut.ruoli[0]);
                    if (ruo.uo.codiceAOO != null)
                        rtn = ruo.uo.codiceAOO;
                }
            }
            return rtn;
        }

        private static string GetMailAOOInteropbyDest(DocsPaVO.utente.Corrispondente corrispondente)
        {
            string rtn = string.Empty;
            if (corrispondente != null)
            {
                if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
                {
                    if (corrispondente.tipoIE.Equals("I"))
                    {
                        DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
                        string sql = "SELECT VAR_EMAIL_REGISTRO FROM DPA_EL_REGISTRI WHERE UPPER(VAR_CODICE)='" + corrispondente.codiceAOO.ToUpper() + "' AND ID_AMM=" + corrispondente.idAmministrazione;
                        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                        {
                            using (System.Data.IDataReader reader = dbProvider.ExecuteReader(sql))
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                        rtn = reader.GetString(0);
                                }
                            }
                        }
                    }
                    else if (corrispondente.tipoIE.Equals("E"))
                    {
                        rtn = corrispondente.email;

                    }

                }
            }
            return rtn;
        }

        //
        //
        //	/// <summary>
        //	/// Spedizione del documento a tutti i destinatari tramite mail
        //	/// </summary>
        //	/// <param name="schedaDocumento"></param>
        //	/// <param name="sendDocumentMailResponse"></param>
        //	/// <param name="infoUtente"></param>
        //	/// <param name="confermaRicezione"></param>
        //	/// <returns></returns>
        //	public static SendDocumentResponse SendDocument(DocsPaVO.documento.SchedaDocumento schedaDocumento,
        //	SendDocumentResponse.SendDocumentMailResponse sendDocumentMailResponse,
        //	DocsPaVO.utente.InfoUtente infoUtente,
        //	bool confermaRicezione)
        //{
        //	//			// TODO: implementare logica per l'invio mail a tutti i destinatari
        //	//			// facenti parte dell'oggetto "sendDocumentMailResponse"
        //	//			SendDocumentResponse retValue=SendDocumentMail(schedaDocumento,
        //	//															sendDocumentMailResponse.MailAddress,
        //	//															sendDocumentMailResponse.Destinatari,
        //	//															infoUtente,
        //	//															confermaRicezione);
        //	//
        //	//			if (retValue)
        //	//			{
        //	//				// TODO: impostazione dati relativi alla spedizione del documento a ciascun destinatario
        //	//				
        //	//			}
        //
        //	return null;
        //}
        //
        //	/// <summary>
        //	/// Spedizione del documento ad un singolo oggetto "SendDocumentResponseItem"
        //	/// cui possono corrispondere più destinatari 
        //	/// </summary>
        //	/// <param name="schedaDocumento"></param>
        //	/// <param name="sendDocumentResponseItem"></param>
        //	/// <param name="infoUtente"></param>
        //	/// <param name="confermaRicezione"></param>
        //	/// <returns></returns>
        //	public static SendDocumentResponse SendDocument(DocsPaVO.documento.SchedaDocumento schedaDocumento,
        //	SendDocumentResponse.SendDocumentResponseItem sendDocumentResponseItem,
        //	DocsPaVO.utente.InfoUtente infoUtente,
        //	bool confermaRicezione)
        //{
        //	// TODO: impostazione dati relativi alla spedizione del documento a ciascun destinatario
        //
        //	return null;
        //}

        /// <summary>
        /// Invio del documento ad un indirizzo mail cui possono
        /// fare riferimento 1 o più destinatari
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="mailAddress"></param>
        /// <param name="listDestinatari"></param>
        /// <param name="infoUtente"></param>
        /// <param name="confermaRicezione"></param>
        /// <returns></returns>
        public static SendDocumentResponse SendDocument(DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                                        DocsPaVO.utente.Registro registroMittente,
                                                        string mailAddress,
                                                        ArrayList listDestinatari,
                                                        DocsPaVO.utente.InfoUtente infoUtente,
                                                        bool confermaRicezione)
        {

            SendDocumentResponse retValue = new SendDocumentResponse();
            retValue.SendDateTime = DateTime.Now;
            retValue.SchedaDocumento = schedaDocumento;

            Hashtable tableDestinatari = dividiDestinatari(listDestinatari, schedaDocumento.registro.codRegistro, schedaDocumento.registro.email, retValue, infoUtente.idAmministrazione);

            string mail = string.Empty;

            foreach (DictionaryEntry item in tableDestinatari)
            {
                DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)((ArrayList)item.Value)[0];
                string err = "";
                //per ottenere il codAOO e codAMM in caso di ruolo/utente
                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                {

                    corr = ((DocsPaVO.utente.Ruolo)corr).uo;
                }
                if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                {
                    corr = (DocsPaVO.utente.Utente)corr;

                }

                if (InteroperabilitaUtils.InteropIntNoMail  //ATTIVATA/NON ATTIVATA INTEROP INTERNI NO MAIL
                    && corr != null
                    && corr.tipoIE != null
                    && corr.tipoIE == "I" //SOLO PER CORR: INTERNI ALL'AMM.
                    && !corr.codiceAOO.ToUpper().Equals(schedaDocumento.registro.codRegistro.ToUpper()) //NO SE RISIEDE NELLA MIA AOO
                    && corr.idAmministrazione.Trim().ToLower().Equals(schedaDocumento.registro.idAmministrazione.Trim().ToLower()))
                {
                    DocsPaVO.Interoperabilita.DatiInteropAutomatica dia = null;
                    //bool result = SendDocumentInteropNoMail(corr, schedaDocumento, infoUtente, out err, out dia);
                    // S.Furnari - 17/01/2013 - Se è attiva la funzionalità di spedizione selettiva, l'utente non è ammesso
                    // come destinatario della spedizione.
                    //bool result = SendDocumentInteropNoMail(corr, schedaDocumento, infoUtente, out err, out dia);
                    bool result = false;
                    if (InteroperabilitaSegnatura.IsEnabledSelectiveTransmission(corr.idAmministrazione) && corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                    {
                        result = false;
                        err = "Impossibile spedire il documento ad un utente.";
                    }
                    else
                        result = SendDocumentInteropNoMail(corr, schedaDocumento, infoUtente, out err, out dia);



                    SendDocumentResponse.SendDocumentMailResponse mailResponse = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(mailAddress, result, err);
                    ArrayList corrs = (ArrayList)item.Value;
                    for (int i = 0; i < corrs.Count; i++)
                    {
                        mailResponse.Destinatari.Add((DocsPaVO.utente.Corrispondente)corrs[i]);
                    }

                    //se i dati aggiuntivi dell'interoperabilità sono presenti..
                    if (dia != null)
                        mailResponse.datiInteropAutomatica = dia;

                    retValue.SendDocumentMailResponseList.Add(mailResponse);

                }
                //else if (InteroperabilitaUtils.InteropSemp
                //&& corr != null
                //&& corr.tipoIE == "E"
                //&& !corr.codiceAOO.ToUpper().Equals(schedaDocumento.registro.codRegistro.ToUpper()) //NO SE RISIEDE NELLA MIA AOO
                //&& !corr.idAmministrazione.Trim().ToLower().Equals(schedaDocumento.registro.idAmministrazione.Trim().ToLower()))
                //{
                //    logger.Debug("******** Inizio Interoperabilità semplificata ********");
                //    DocsPaVO.utente.Corrispondente mittente = null;
                //    if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente != null)
                //    {
                //        mittente = ((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente;

                //    }
                //    else
                //        mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

                //    string pathAttatchments = ExtractDocumentFilesToPath(schedaDocumento, infoUtente);

                //    DocsPaInteropSemplificata.WRInteropSemp.Segnatura segnatura = CreaSegnaturaObject(mittente, schedaDocumento.registro, registroMittente, schedaDocumento, mailAddress, listDestinatari, pathAttatchments, confermaRicezione); ;

                //    string[] filesToSend = System.IO.Directory.GetFiles(pathAttatchments);
                //    DocsPaInteropSemplificata.WRInteropSemp.FileAllegato documentoPrincipale = new DocsPaInteropSemplificata.WRInteropSemp.FileAllegato();

                //    int numAllegati = filesToSend.Count() - 1;
                //    DocsPaInteropSemplificata.WRInteropSemp.FileAllegato[] documentiAllegati = null;

                //    if (numAllegati > 0)
                //    {
                //        documentiAllegati = new DocsPaInteropSemplificata.WRInteropSemp.FileAllegato[numAllegati];
                //    }

                //    for (int i = 0; i < filesToSend.Count(); i++)
                //    {
                //        if (filesToSend[i].Contains("Documento_principale"))
                //        {
                //            documentoPrincipale.NomeFile = filesToSend[i];
                //            FileStream fs = File.OpenRead(filesToSend[i]);
                //            byte[] res = new byte[fs.Length];
                //            fs.Read(res, 0, (int)fs.Length);
                //            fs.Close();
                //            documentoPrincipale.Contenuto = res;
                //        }
                //        else
                //        {
                //            //Da gestire il caso in cui non c'è neanche il documento principale???
                //            //(ossia quando viene creato il file empty.TXT)
                //            //Altrimenti qui viene generata una NullPointerException
                //            if (!filesToSend[i].Contains("segnatura.xml") && !filesToSend[i].Contains("empty.TXT"))
                //            {
                //                int temp = i - 1;
                //                documentiAllegati[temp] = new DocsPaInteropSemplificata.WRInteropSemp.FileAllegato();
                //                documentiAllegati[temp].NomeFile = filesToSend[i];
                //                FileStream fs = File.OpenRead(filesToSend[i]);
                //                byte[] res = new byte[fs.Length];
                //                fs.Read(res, 0, (int)fs.Length);
                //                fs.Close();
                //                documentiAllegati[temp].Contenuto = res;
                //            }
                //        }
                //    }
                //    DocsPaInteropSemplificata.WRInteropSemp.PacchettoSpedizione pacchettoSpedizione = new DocsPaInteropSemplificata.WRInteropSemp.PacchettoSpedizione();
                //    pacchettoSpedizione.Segnatura = segnatura;
                //    pacchettoSpedizione.DocumentoPrincipale = documentoPrincipale;
                //    pacchettoSpedizione.DocumentiAllegati = documentiAllegati;
                //    string urlWSCorrispondente = "http://localhost/DocsPa30/DocsPaWS/DocsPaWSInteropSemp.asmx"; //Url del webservice da chiamare in base alla AOO del corrispondente
                //    DocsPaInteropSemplificata.InteropSempProxy.SpedisciClient(urlWSCorrispondente, pacchettoSpedizione);
                //    logger.Debug("******** Fine Interoperabilità semplificata ********");
                //}
                else
                {
                   
                    SendDocumentResponse.SendDocumentMailResponse retmail =SendDocumentMail(schedaDocumento, registroMittente, mailAddress, listDestinatari, infoUtente, confermaRicezione);
                    retValue.SendDocumentMailResponseList.Add(retmail);
                    // PEC 4 - requisito 5 - storico spedizioni
                    //string esito = "";
                    //if (retmail.SendSucceded)
                    //{
                    //    esito = "Spedito"; 
                    //}
                    //else
                    //{
                    //    esito = retmail.SendErrorMessage;
                    //}
                    //DocsPaDB.Query_DocsPAWS.Interoperabilita interop = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                    //interop.InsertInStoricoSpedizioni(schedaDocumento.systemId, corr.systemId, esito, mailAddress);
            

                    //retValue.SendDocumentMailResponseList.Add(SendDocumentMail(schedaDocumento, registroMittente, mailAddress, listDestinatari, infoUtente, confermaRicezione));
                }
            }
            
            return retValue;
        }

        /// <summary>
        /// Metodo per la spedizione di un documento per interoperabilità semplificata
        /// </summary>
        /// <param name="schedaDocumento">Informazioni sul documento da spedire</param>
        /// <param name="infoUtente">Informazioni sull'utente che sta eefettuando la spedizione</param>
        /// <param name="receivers">Informazioni sui destinatari cui è indirizzata la spedizione</param>
        /// <returns>Esito della spedizione</returns>
        public static SendDocumentResponse SendDocumentIS(DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                                        DocsPaVO.utente.InfoUtente infoUtente,
                                                        IEnumerable<DocsPaVO.Spedizione.DestinatarioEsterno> receivers)
        {

            SendDocumentResponse retValue = new SendDocumentResponse();
            retValue.SendDateTime = DateTime.Now;
            retValue.SchedaDocumento = schedaDocumento;

            // Suddivisione dei destinatari per amministrazione
            List<List<DocsPaVO.Spedizione.DestinatarioEsterno>> splittedReceivers = receivers.GroupBy(r => r.DatiDestinatari[0].codiceAmm).Select(group => group.ToList()).ToList();
            SendDocumentResponse.SendDocumentMailResponse mailResponse = null;
            foreach (List<DocsPaVO.Spedizione.DestinatarioEsterno> recs in splittedReceivers)
            {
                mailResponse = SendDocumentSimpleInterop(schedaDocumento,
                                                         recs,
                                                         infoUtente,
                                                         recs[0].DatiDestinatari[0].Url[0].Url);
                //retValue.SendDocumentMailResponseList.Add(SendDocumentSimpleInterop(
                //    schedaDocumento,
                //    recs,
                //    infoUtente,
                //    recs[0].DatiDestinatari[0].Url[0].Url));
                retValue.SendDocumentMailResponseList.Add(mailResponse);

                // Inserimento nella stato invio ad ogni spedizione effettuata
                List<Corrispondente> corrs = new List<Corrispondente>();
                foreach (DocsPaVO.Spedizione.DestinatarioEsterno c in recs)
                    corrs.AddRange(c.DatiDestinatari);
               
                InsertStatoInvioDestinatari(schedaDocumento, corrs, mailResponse);                
            }
            
            return retValue;
        }

        /// <summary>       
        /// Aggiornamento stato spedizione nella dpa_stato_invio
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="listDestinatari"></param>
        /// <param name="mailResponse"></param>        
        public static void InsertStatoInvioDestinatari(DocsPaVO.documento.SchedaDocumento schedaDocumento, List<Corrispondente> listDestinatari, DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse mailResponse)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            foreach (DocsPaVO.utente.Corrispondente corr in listDestinatari)
            {
                if (schedaDocumento.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
                {
                    string idDocArrivoPar = doc.GetIdDocArrivoPar(schedaDocumento.systemId, corr, "'D','C','F'");

                    string mailCorrispondente = corr.email;
                    if (mailCorrispondente == null)
                        mailCorrispondente = string.Empty;

                    doc.InsertStatoInvio(corr, idDocArrivoPar, schedaDocumento.registro.systemId, schedaDocumento.systemId, (mailResponse == null || (mailResponse != null && !mailResponse.SendSucceded && !string.IsNullOrEmpty(mailResponse.SendErrorMessage))), mailResponse.MailAddress);
                }
            }
        }

        /// <summary>
        /// Invio di un documento ad un indirizzo mail cui possono
        /// fare riferimento 1 o più destinatari 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="registroMittente">
        /// Registro (o RF) mittente del documento
        /// </param>
        /// <param name="mailAddress"></param>
        /// <param name="listDestinatari"></param>
        /// <param name="infoUtente"></param>
        /// <param name="confermaRicezione"></param>
        /// <returns></returns>
        private static SendDocumentResponse.SendDocumentMailResponse SendDocumentMail(
                            DocsPaVO.documento.SchedaDocumento schedaDocumento,
                            DocsPaVO.utente.Registro registroMittente,
                            string mailAddress,
                            ArrayList listDestinatari,
                            DocsPaVO.utente.InfoUtente infoUtente,
                            bool confermaRicezione)
        {
            if (registroMittente == null)
                // Se non è specificato un registro (o RF) mittente,
                // viene impostato il registro di protocollo
                registroMittente = schedaDocumento.registro;

            SendDocumentResponse.SendDocumentMailResponse retValue = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(mailAddress);


            DocsPaVO.utente.Corrispondente mittente = null;

            //25/11/2009 se c'è il mittente nel protocollo, allora inserisco quest'ultimo nella segnatura.xml.
            if (((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente != null)
            {
                mittente = ((DocsPaVO.documento.ProtocolloUscita)(schedaDocumento.protocollo)).mittente;

            }
            else
                mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

            try
            {
                //Gestione XML con allegati da creare on the fly suap
                if (schedaDocumento.template != null)
                {
                    if (schedaDocumento.template.DESCRIZIONE.ToUpper() == "ENTESUAP")
                    {
                        BusinessLogic.interoperabilità.InteroperabilitaSendXmlInAttach xmlAtt = new interoperabilità.InteroperabilitaSendXmlInAttach();
                        bool suapRetval = xmlAtt.ManageAttachXML_Suap(ref schedaDocumento, infoUtente, registroMittente.email);
                        if (!suapRetval)
                        {
                            string idSuap = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_IDENTIFICATIVO_SUAP");
                            //se manca l'idsuap nelle chiavi di configurazione allora non posso continuare ed esco
                            if (String.IsNullOrEmpty(idSuap))
                            {
                                retValue.SendErrorMessage = "ERRORE: l'identificativo suap non è configurato";
                            }
                            else
                            {
                                retValue.SendErrorMessage = "ERRORE: campi mancanti per creare l'allegato entesuap.xml. Controllare: codice pratica, registro/RF mittente e identificativo SUAP (contattare l’amministratore di sistema)";
                            }
                            retValue.SendSucceded = false;
                            return retValue;
                        }

                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore creando o processando l'allegato suap {0} {1}", e.Message, e.StackTrace);
            }

            // estrazione degli allegati
            Dictionary<string, string> CoppiaNomeFileENomeOriginale;
            string pathAttatchments = ExtractDocumentFilesToPath(schedaDocumento, infoUtente, out CoppiaNomeFileENomeOriginale);


            //ci sono stati dei problemi relativi all'inserimento dell'allegato
            if (string.IsNullOrEmpty(pathAttatchments))
                return null;

            DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
            DocsPaVO.amministrazione.CasellaRegistro [] caselle =  BusinessLogic.Amministrazione.RegistroManager.GetMailRegistro(registroMittente.systemId);
            if (!string.IsNullOrEmpty(registroMittente.email))
            {
                foreach (DocsPaVO.amministrazione.CasellaRegistro c in caselle)
                {
                    if (c.EmailRegistro.Equals(registroMittente.email))
                    {
                        retValue = SendDocumentMail(schedaDocumento,
                                            mittente,
                                            registroMittente,
                                            mailAddress,
                                            listDestinatari,
                                            c,
                                            pathAttatchments,
                                            CoppiaNomeFileENomeOriginale,
                                            confermaRicezione);

                        c.RicevutaPEC = (!string.IsNullOrEmpty(c.RicevutaPEC)) ? c.RicevutaPEC : string.Empty;
                        if (!string.IsNullOrEmpty(c.RicevutaPEC) && (c.RicevutaPEC.Length > 1))
                        {
                            c.RicevutaPEC = c.RicevutaPEC.Substring(0, 1);
                            //Qui va eliminato il campo secondario dal DB nel caso esso esista.
                            obj.setRicevutaPec(registroMittente.systemId, new DocsPaVO.amministrazione.CasellaRegistro[] { c });
                        }
                        break;
                    }
                }
            }
            else
            {
                retValue.Destinatari = listDestinatari;
                retValue.MailAddress = mailAddress;
                retValue.MailNonInteroperante = false;
                retValue.SendErrorMessage = " Spedizione non effettuata: è necessario selezionare un registro/rf prima di effettuare la spedizione";
                retValue.SendSucceded = false;
            }
            //cancellazione della directory
            try
            {
                DocsPaUtils.Functions.Functions.CancellaDirectory(pathAttatchments);
            }
            catch { }

            return retValue;
        }

        /// <summary>
        /// Reperimento del canale preferenziale per il destintaraio
        /// </summary>
        /// <param name="destintario"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Canale GetCanalePreferenzialeDestinatario(DocsPaVO.utente.Corrispondente destintario)
        {
            logger.Debug("INIT - GetCanalePreferenzialeDestinatario");

            DocsPaVO.utente.Canale canalePref = null;

            try
            {
                if (!string.IsNullOrEmpty(destintario.tipoCorrispondente)
                    && destintario.tipoCorrispondente.Equals("O"))
                {
                    canalePref = new DocsPaVO.utente.Canale();
                    canalePref.tipoCanale = "MAIL";
                    canalePref.descrizione = "MAIL";
                }
                else
                    if (destintario.tipoIE == "E")
                    {
                        // Destinatario esterno
                        canalePref = destintario.canalePref;
                    }
                    else if (destintario.tipoIE == "I")
                    {
                        // Destinatario interno

                        if (destintario.GetType() == typeof(DocsPaVO.utente.Ruolo))
                        {
                            // Se Ruolo prende il canale preferenziale dell'UO
                            canalePref = ((DocsPaVO.utente.Ruolo)destintario).uo.canalePref;
                        }
                        else if (destintario.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                        {
                            canalePref = ((DocsPaVO.utente.UnitaOrganizzativa)destintario).canalePref;
                        }
                        else
                        {
                            // Se utente, cerca il ruolo preferito (se non c'è, prende il primo della lista),
                            // quindi prende il canale preferenziale dell'UO di tale utente
                            DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)destintario;

                            DocsPaVO.utente.Ruolo[] ruoliUtente = null;

                            // Cerca il ruolo preferito (se non c'è, prende il primo ruolo disponibile)
                            if (utente.ruoli == null || (utente.ruoli != null && utente.ruoli.Count == 0))
                                ruoliUtente = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));
                            else
                                ruoliUtente = (DocsPaVO.utente.Ruolo[])utente.ruoli.ToArray(typeof(DocsPaVO.utente.Ruolo));

                            if (ruoliUtente != null && ruoliUtente.Length > 0)
                            {
                                DocsPaVO.utente.Ruolo ruoloPreferito = ruoliUtente.Where(e => e.selezionato).FirstOrDefault();

                                if (ruoloPreferito == null)
                                    // Ruolo preferito non disponibile, reperimento del primo ruolo della lista
                                    ruoloPreferito = ruoliUtente[0];

                                if (ruoloPreferito != null)
                                {
                                    if (ruoloPreferito.uo == null)
                                        logger.Debug("GetCanalePreferenzialeDestinatario - UO non definita per il ruolo preferito");
                                    else
                                    {
                                        if (ruoloPreferito.uo != null && ruoloPreferito.uo.canalePref == null)
                                        {
                                            // Reperimento del canale associato all'UO del Ruolo
                                            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                                                canalePref = amm.GetDatiCanPref(ruoloPreferito.uo);
                                        }
                                        else
                                        {
                                            // Reperimento del canale associato all'UO del Ruolo
                                            canalePref = ruoloPreferito.uo.canalePref;
                                        }
                                    }
                                }
                            }
                        }
                        //modifica
                        if (canalePref == null &&
                            destintario.tipoIE.Contains('I'))
                        {
                            canalePref = new DocsPaVO.utente.Canale();
                            canalePref.tipoCanale = "INTEROPERABILITA";
                            canalePref.descrizione = "INTEROPERABILITA";
                        }
                        //fine modifica
                    }
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                throw new ApplicationException(string.Format("Si è verificato un errore nel reperimento del canale preferenziale per il destinatario con ID '{0}'", destintario.systemId), ex);
            }
            finally
            {
                logger.Debug("END - GetCanalePreferenzialeDestinatario");
            }

            if (canalePref != null)
            {
                if (!string.IsNullOrEmpty(canalePref.descrizione))
                    canalePref.descrizione = canalePref.descrizione.ToUpper();
                if (!string.IsNullOrEmpty(canalePref.tipoCanale))
                    canalePref.tipoCanale = canalePref.tipoCanale.ToUpper();
            }
            return canalePref;
        }

        /// <summary>
        /// Invio del documento ad un indirizzo mail
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="mittente"></param>
        /// <param name="destinatario"></param>
        /// <param name="listDestinatari"></param>
        /// <param name="rowInfoMailRegistro"></param>
        /// <param name="pathFiles"></param>
        /// <param name="confermaRicevuta"></param>
        /// <returns></returns>
        private static SendDocumentResponse.SendDocumentMailResponse SendDocumentMail(
                                            DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                            DocsPaVO.utente.Corrispondente mittente,
                                            DocsPaVO.utente.Registro registroMittente,
                                            string mailAddress,
                                            ArrayList listDestinatari,
                                            DocsPaVO.amministrazione.CasellaRegistro casellaMittente,
                                            string pathFiles,
                                            Dictionary<string, string> CoppiaNomeFileENomeOriginale,
                                            bool confermaRicevuta)
        {
            SendDocumentResponse.SendDocumentMailResponse singleResponse = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse();
            singleResponse.SendSucceded = true;
            singleResponse.Destinatari.AddRange(listDestinatari);
            singleResponse.MailAddress = mailAddress;

            logger.Debug("Creazione del file segnatura per l'indirizzo " + mailAddress);

            DocsPaVO.utente.Corrispondente dest = (DocsPaVO.utente.Corrispondente)listDestinatari[listDestinatari.Count - 1];

            // Verifica il canale preferenziale di tipo interoperabilità
            DocsPaVO.utente.Canale canalePref = GetCanalePreferenzialeDestinatario(dest);

            if (canalePref == null ||
                (canalePref != null &&
                 canalePref.descrizione != "INTEROPERABILITA" &&
                 canalePref.descrizione != "MAIL"))
            {
                singleResponse.SendSucceded = false;
                singleResponse.MailNonInteroperante = true;
                singleResponse.SendErrorMessage = "Canale preferenziale per il destinatario non di tipo MAIL o INTEROPERABILITA', impossibile spedire il documento";
            }
            else
            {
                if (canalePref.descrizione.Equals("INTEROPERABILITA"))
                    creaSegnatura(mittente, schedaDocumento.registro, registroMittente, schedaDocumento, mailAddress, listDestinatari, pathFiles,CoppiaNomeFileENomeOriginale ,confermaRicevuta);

                //creazione ed invio mail
                string porta = null;

                if (casellaMittente.PortaSMTP != 0)
                    porta = casellaMittente.PortaSMTP.ToString();

                string smtp_user = (!string.IsNullOrEmpty(casellaMittente.UserSMTP)) ? casellaMittente.UserSMTP : null;
                string smtp_pwd = (!string.IsNullOrEmpty(casellaMittente.PwdSMTP)) ?  casellaMittente.PwdSMTP : null;

                string ricevutaPec = string.Empty;
                ricevutaPec = (!string.IsNullOrEmpty(casellaMittente.RicevutaPEC)) ? casellaMittente.RicevutaPEC : null;
                string X_TipoRicevuta = null;
                //aggiunta la trim() per gestire la presenza di spazi bianchi nei campi VAR_USER_SMTP e VAR_PWD_SMTP
                if (smtp_user != null)
                    smtp_user = smtp_user.Trim();
                if (smtp_pwd != null)
                    smtp_pwd = smtp_pwd.Trim();

                if (ricevutaPec != null)
                {

                    if (ricevutaPec != string.Empty)
                    {
                        X_TipoRicevuta = ricevutaPec;
                        switch (ricevutaPec.Length)
                        {
                            case 1:
                                X_TipoRicevuta = ricevutaPec.Substring(0, 1);
                                break;
                            case 2:
                                //Se la len è maggiore di uno, vuol dire che ho un valore diverso da quello di default
                                //Preleverò quindi quello.
                                X_TipoRicevuta = ricevutaPec.Substring(1, 1);
                                break;
                            default:    //non si sa mai
                                X_TipoRicevuta = string.Empty;
                                break;
                        }
                        //Qui transcodifico il tipo ricevuta CHA in header 
                        //(sarebbe carino metterlo in un enum per evitare hardcoding nel codice).
                        switch (X_TipoRicevuta)
                        {
                            case "C":
                                X_TipoRicevuta = "completa";
                                break;
                            case "B":
                                X_TipoRicevuta = "breve";
                                break;
                            case "S":
                                X_TipoRicevuta = "sintetica";
                                break;
                            default:
                                X_TipoRicevuta = null;
                                break;
                        }

                    }

                }

                logger.Debug("Creazione ed invio del messaggio all'indirizzo " + mailAddress);


                try
                {
                    creaMail(schedaDocumento,
                            casellaMittente.ServerSMTP,
                            casellaMittente.EmailRegistro,
                            smtp_user,
                            smtp_pwd,
                            mailAddress,
                            pathFiles,
                            CoppiaNomeFileENomeOriginale,
                            porta,
                            casellaMittente.SmtpSSL.ToString(),
                            casellaMittente.PopSSL.ToString(),
                            casellaMittente.SmtpSta.ToString(),
                            X_TipoRicevuta
                            );
                }
                catch (Exception ex)
                {
                    singleResponse.SendSucceded = false;
                    singleResponse.SendErrorMessage = ex.Message;
                    logger.Error("Errore in SendDocumentMail: " + ex.Message);
                }

                //cancella file segnatura
                if (File.Exists(pathFiles + "\\segnatura.xml"))
                    System.IO.File.Delete(pathFiles + "\\segnatura.xml");
            }

            return singleResponse;
        }


        private static string ExtractDocumentFilesToPath(DocsPaVO.documento.SchedaDocumento schedaDocumento,
                                                         DocsPaVO.utente.InfoUtente infoUtente, 
                                                        out Dictionary<string, string> CoppiaNomeFileENomeOriginale)
        {
            string pathFiles = string.Empty;

            //creazione del logger
            string basePathLogger = ConfigurationManager.AppSettings["LOG_PATH"];
            basePathLogger = basePathLogger.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
            basePathLogger = basePathLogger + "\\Interoperabilita";
            DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(basePathLogger + "\\" + schedaDocumento.registro.codRegistro);
            string pathLogger = basePathLogger + "\\" + schedaDocumento.registro.codRegistro + "\\invio";

            logger.Debug("Destinatari:" + ((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).destinatari.Count);
            logger.Debug("Destinatari conoscenza:" + ((DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo).destinatariConoscenza.Count);

            // inserimento dei file in una cartella temporanei
            string basePathFiles = ConfigurationManager.AppSettings["LOG_PATH"];
            basePathFiles = basePathFiles.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));

            //PALUMBO-LUCIANI: modifica per Path per tck 11616
            //basePathFiles = basePathFiles + "\\Invio_files";
            basePathFiles = basePathFiles + "\\Invio_files_" + System.Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");

            pathFiles = basePathFiles + "\\" + schedaDocumento.registro.codRegistro;
            DocsPaUtils.Functions.Functions.CheckEsistenzaDirectory(pathFiles);

            logger.Debug("Estrazione dei file da inviare");

            
            if (!estrazioneFiles(infoUtente, schedaDocumento, pathFiles, out CoppiaNomeFileENomeOriginale))
            {
                DocsPaUtils.Functions.Functions.CancellaDirectory(pathFiles);
                pathFiles = string.Empty;
            }

            return pathFiles;
        }

        /// <summary>
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="path"></param>
        /// <param name="logger"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        /// <summary>
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="path"></param>
        /// <param name="logger"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static bool estrazioneFiles(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDoc, string path, out Dictionary<string,string> CoppiaNomeFileENomeOriginale)
        {
            System.IO.FileStream fs = null;
            System.IO.FileStream fsAll = null;
            CoppiaNomeFileENomeOriginale = new Dictionary<string, string>();
            byte[] content = null;
            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
            string docPrincipaleName = "";
            try
            {
                //estrazione documento principale
                DocsPaVO.documento.Documento doc = getDocumentoPrincipale(schedaDoc);
                if (doc.fileName != null && doc.fileName != "")
                {
                    //inizio modifica: il documento principale in alcuni casi non veniva allegato,
                    //Ciò accedeva quando durante l'estrazione del documento principale, per allegarlo alla mail,
                    //nel percorso del file è presente un ulteriore '.', oltre a quello relativo all'estensione.
                    //Ad esempio con un path del genere andava in errore ENTEC\2005\EC_RU\UO1.1\Partenza\467.XLS
                    char[] dot = { '.' };
                    string[] parts;
                    string suffix = "";
                    if (!doc.fileName.ToUpper().EndsWith("P7M"))
                    {
                        parts = doc.fileName.Split(dot);
                        suffix = parts[parts.Length - 1];
                        docPrincipaleName = "Documento_principale." + suffix;
                    }
                    else
                    {
                        string appodocPrincipaleName = doc.fileName.Substring(doc.fileName.LastIndexOf("\\") + 1);
                        parts = appodocPrincipaleName.Split(dot);
                        int cont = 0;
                        for (int i = 2; i < parts.Length; i++)
                        {
                            if (parts[i].ToUpper().Equals("P7M"))
                            {
                                cont = cont + 1;
                                suffix = suffix + ".P7M";
                            }
                        }
                        suffix = parts[parts.Length - cont - 1] + suffix;
                        appodocPrincipaleName = appodocPrincipaleName.Substring(appodocPrincipaleName.ToUpper().LastIndexOf(suffix.ToUpper()));
                        docPrincipaleName = "Documento_principale." + appodocPrincipaleName;
                    }
                    //fine modifica
                    fs = new System.IO.FileStream(path + "\\" + docPrincipaleName, System.IO.FileMode.Create);

                    //byte[] content=getDocument(infoUtente,doc.docNumber,doc.version,doc.versionId,doc.versionLabel,logger);

                    //modifica
                    DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
                    fd = BusinessLogic.Documenti.FileManager.getFileFirmato(doc, infoUtente, false);
                    content = fd.content;
                    //fien mofica
                    string NomeOriginale;
                    if (String.IsNullOrEmpty(fd.nomeOriginale))
                        NomeOriginale = fd.name;
                    else
                        NomeOriginale = fd.nomeOriginale;

                    CoppiaNomeFileENomeOriginale.Add(String.Format ( path + "\\" + docPrincipaleName).ToLowerInvariant(), NomeOriginale);
                    //content =documentManager.GetFile(doc.docNumber, doc.version, doc.versionId, doc.versionLabel);

                    if (content == null)
                    {
                        logger.Error("File allegato non valido");
                        throw new Exception();
                    }
                }
                else
                {
                    //crea un file di nome empty.txt
                    fs = new System.IO.FileStream(path + "\\empty.txt", System.IO.FileMode.Create);
                    CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\empty.txt").ToLowerInvariant(), "empty.txt");
                }

                if (content != null)
                {
                    fs.Write(content, 0, content.Length);
                }
                if (fs != null)
                    fs.Close();
                
                //gestione TSR
                if (content != null)
                {
                    DocsPaVO.documento.FileRequest fr = new DocsPaVO.documento.FileRequest { docNumber = doc.docNumber, versionId = doc.versionId};
                    byte[] tsr = InteroperabilitaUtils.GetTSRForDocument(infoUtente, fr);
                    if (tsr != null)
                    {
                        if (!BusinessLogic.Interoperabilità.InteroperabilitaUtils.MatchTSR(tsr, content))
                            tsr = null;

                        if (tsr != null)
                        {
                            string NomeOriginale = String.Format(path + "\\" + docPrincipaleName).ToLowerInvariant();
                            if (CoppiaNomeFileENomeOriginale.ContainsKey(NomeOriginale.ToLowerInvariant()))
                                NomeOriginale = CoppiaNomeFileENomeOriginale[NomeOriginale.ToLowerInvariant()] + ".tsr";

                            File.WriteAllBytes(path + "\\" + docPrincipaleName + ".tsr", tsr);
                            CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + docPrincipaleName + ".tsr").ToLowerInvariant(), NomeOriginale);
                        }
                    }
                }
                

                //estrazione degli allegati	
                byte[] all_content = null;

                //luluciani modifica 19/09/2012 evitiamo di spedire allegati pec o IS.
                //for (int i = 0; i < schedaDoc.allegati.Count; i++)
                //{
                int j = 0;
                DocsPaDB.Query_DocsPAWS.Documenti docWs = new DocsPaDB.Query_DocsPAWS.Documenti();
                foreach (DocsPaVO.documento.Allegato allegato in ((DocsPaVO.documento.Allegato[])schedaDoc.allegati.ToArray(typeof(DocsPaVO.documento.Allegato))).Where(
                a => BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(a.versionId) == "0" && BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(a.versionId) == "0"
                 && docWs.GetTipologiaAllegato(a.versionId)!="D")
            )
                {
                    //DocsPaVO.documento.Allegato all = (DocsPaVO.documento.Allegato)schedaDoc.allegati[i];

                    j = j + 1;

                    string allegatoName = "Allegato_" + (j).ToString();

                    string fileExt = getEstensione(allegato.fileName);
                    //string fileExt = getEstensione(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);
                    if (fileExt != "")
                    {
                        fsAll = new System.IO.FileStream(path + "\\" + allegatoName + "." + fileExt, System.IO.FileMode.Create);


                        documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                        DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();

                        //fd = BusinessLogic.Documenti.FileManager.getFileFirmato(all, infoUtente, false);
                        fd = BusinessLogic.Documenti.FileManager.getFileFirmato(allegato, infoUtente, false);
                        all_content = fd.content;

                        if (all_content == null)
                        {
                            logger.Error("Errore nella gestione dell'interoperabilità. (estrazioneFiles)");
                            throw new Exception();
                        }

                        string NomeOriginale;
                        if (String.IsNullOrEmpty(fd.nomeOriginale))
                            NomeOriginale = fd.name;
                        else
                            NomeOriginale = fd.nomeOriginale;

                        CoppiaNomeFileENomeOriginale.Add(String.Format (path + "\\" + allegatoName + "." + fileExt).ToLowerInvariant(), NomeOriginale);
                    }
                    else
                    {
                        fsAll = new System.IO.FileStream(path + "\\" + allegatoName + ".TXT", System.IO.FileMode.Create);
                        CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + allegatoName + ".TXT").ToLowerInvariant (), allegatoName + ".TXT");
                    }

                    if (all_content != null)
                        fsAll.Write(all_content, 0, all_content.Length);


                    
                    //gestione TSR
                    if (all_content != null)
                    {
                        DocsPaVO.documento.FileRequest fr = new DocsPaVO.documento.FileRequest { docNumber = allegato.docNumber, versionId = allegato.versionId };
                        byte[] tsr = InteroperabilitaUtils.GetTSRForDocument(infoUtente, fr);
                        if (tsr != null)
                        {
                            if (!BusinessLogic.Interoperabilità.InteroperabilitaUtils.MatchTSR(tsr, all_content))
                                tsr = null;


                            if (tsr != null)
                            {
                                string NomeOriginale = path + "\\" + allegatoName + "." + fileExt;
                                if (CoppiaNomeFileENomeOriginale.ContainsKey(NomeOriginale.ToLowerInvariant()))
                                    NomeOriginale = CoppiaNomeFileENomeOriginale[NomeOriginale.ToLowerInvariant()] + ".tsr";

                                File.WriteAllBytes(path + "\\" + allegatoName + "." + fileExt + ".tsr", tsr);
                                CoppiaNomeFileENomeOriginale.Add(String.Format(path + "\\" + allegatoName + "." + fileExt + ".tsr").ToLowerInvariant(), NomeOriginale);
                            }
                        }
                    }
                    
                    fsAll.Close();
                }

                return true;
            }
            catch (Exception e)
            {
                logger.Error("Estrazione del file non eseguita.Eccezione: " + e.ToString());

                if (fs != null)
                {
                    fs.Close();
                }

                if (fsAll != null)
                {
                    fsAll.Close();
                }

                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="mittSegnatura"></param>
        /// <param name="reg"></param>
        /// <param name="schedaDoc"></param>
        /// <param name="mailDest"></param>
        /// <param name="destinatari"></param>
        /// <param name="pathFiles"></param>
        /// <param name="confermaRic"></param>
        /// <param name="logger"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static bool creaSegnatura(DocsPaVO.utente.Corrispondente mittSegnatura, DocsPaVO.utente.Registro reg, DocsPaVO.utente.Registro regMittente, DocsPaVO.documento.SchedaDocumento schedaDoc, string mailDest, System.Collections.ArrayList destinatari, string pathFiles, Dictionary<string, string> CoppiaNomeFileENomeOriginale, bool confermaRic)
        {
            try
            {
                bool isInteropRGS = false;
                XmlDocument xdoc = new XmlDocument();

                //Impostazione
                xdoc.XmlResolver = null;
                XmlDeclaration dec = xdoc.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
                xdoc.AppendChild(dec);
                
                //NON VALIDIAMO PIù CON LA DTD MA CON L'XSD
                //XmlDocumentType dtd = xdoc.CreateDocumentType("Segnatura", null, "Segnatura.dtd", null);
                //xdoc.AppendChild(dtd);
                //logger.Debug("dtd impostato");

                //Creazione della root
                XmlElement root = xdoc.CreateElement("Segnatura");
                root.SetAttribute("xmlns", "http://www.digitPa.gov.it/protocollo/");  
                xdoc.AppendChild(root);

                //Creazione dell'intestazione
                XmlElement intestazione = xdoc.CreateElement("Intestazione");
                root.AppendChild(intestazione);
                XmlElement identificatore = xdoc.CreateElement("Identificatore");
                intestazione.AppendChild(identificatore);
                XmlElement origine = xdoc.CreateElement("Origine");
                intestazione.AppendChild(origine);
                XmlElement destinazione = xdoc.CreateElement("Destinazione");

                if (confermaRic)
                {
                    destinazione.SetAttribute("confermaRicezione", "si");
                }
                else
                {
                    destinazione.SetAttribute("confermaRicezione", "no");
                }

                intestazione.AppendChild(destinazione);
                XmlElement oggettoInt = xdoc.CreateElement("Oggetto");
                intestazione.AppendChild(oggettoInt);

                //Identificatore	
                XmlElement codiceAmm = xdoc.CreateElement("CodiceAmministrazione");
                identificatore.AppendChild(codiceAmm);
                codiceAmm.InnerText = reg.codAmministrazione;
                XmlElement codiceAOO = xdoc.CreateElement("CodiceAOO");
                identificatore.AppendChild(codiceAOO);
                codiceAOO.InnerText = reg.codRegistro;
                XmlElement codiceRegistro = xdoc.CreateElement("CodiceRegistro");
                identificatore.AppendChild(codiceRegistro);
                codiceRegistro.InnerText = reg.codRegistro;
                XmlElement numeroReg = xdoc.CreateElement("NumeroRegistrazione");
                identificatore.AppendChild(numeroReg);
                
                int MAX_LENGTH = 7;
                string zeroes = "";
                string numProto = schedaDoc.protocollo.numero;
                for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                {
                    zeroes = zeroes + "0";
                }
                numProto = zeroes + numProto;
                numeroReg.InnerText = numProto;

                //numeroReg.InnerText = schedaDoc.protocollo.numero;
                XmlElement dataReg = xdoc.CreateElement("DataRegistrazione");
                identificatore.AppendChild(dataReg);
                dataReg.InnerText = DocsPaUtils.Functions.Functions.CheckData_Invio(schedaDoc.protocollo.dataProtocollazione);  //DA CONVERTIRE

                //Origine
                XmlElement indirizzoTel = xdoc.CreateElement("IndirizzoTelematico");
                origine.AppendChild(indirizzoTel);
                if (regMittente != null)
                    indirizzoTel.InnerText = regMittente.email;
                else
                    indirizzoTel.InnerText = reg.email;
                XmlElement mittente = xdoc.CreateElement("Mittente");

                //Si riempie il campo mittente
                getCorrispondente(mittSegnatura, mittente, xdoc);
                XmlElement AOO = xdoc.CreateElement("AOO");
                mittente.AppendChild(AOO);
                XmlElement denominazioneAOO = xdoc.CreateElement("Denominazione");
                denominazioneAOO.InnerText = reg.codRegistro;
                AOO.AppendChild(denominazioneAOO);
                origine.AppendChild(mittente);

                //Destinazione
                XmlElement indirizzoTelematico = xdoc.CreateElement("IndirizzoTelematico");
                indirizzoTelematico.InnerText = mailDest;
                destinazione.AppendChild(indirizzoTelematico);

                for (int i = 0; i < destinatari.Count; i++)
                {
                    XmlElement destinatario = xdoc.CreateElement("Destinatario");
                    getCorrispondente((DocsPaVO.utente.Corrispondente)destinatari[i], destinatario, xdoc);
                    destinazione.AppendChild(destinatario);
                    logger.Debug("Destinatario aggiunto");
                }

                //Oggetto
                oggettoInt.InnerText = schedaDoc.oggetto.descrizione;

                #region Riferimenti Mittente
                //I contesti procedurali saranno due, il primo creato per mantenere la compatibilità con la vecchia versione dei CC
                //il secondo è quello ufficiale
                XmlElement riferimenti = xdoc.CreateElement("Riferimenti");
                root.AppendChild(riferimenti);
                
                //Primo Contesto Procedurale
                XmlElement contestoProceduraleUno = xdoc.CreateElement("ContestoProcedurale");

                XmlElement codiceAmmUno = xdoc.CreateElement("CodiceAmministrazione");
                codiceAmmUno.InnerText = reg.codAmministrazione;
                contestoProceduraleUno.AppendChild(codiceAmmUno);

                XmlElement codiceAOOUno = xdoc.CreateElement("CodiceAOO");
                codiceAOOUno.InnerText = reg.codRegistro;
                contestoProceduraleUno.AppendChild(codiceAOOUno);

                XmlElement identificativoUno = xdoc.CreateElement("Identificativo");
                if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario))
                    identificativoUno.InnerText = schedaDoc.protocolloTitolario;
                else
                    identificativoUno.InnerText = schedaDoc.riferimentoMittente;
                contestoProceduraleUno.AppendChild(identificativoUno);

                XmlElement tipoContestoProceduraleUno = xdoc.CreateElement("TipoContestoProcedurale");
                tipoContestoProceduraleUno.InnerText = "Protocollo Arma";
                contestoProceduraleUno.AppendChild(tipoContestoProceduraleUno);

                XmlElement oggettoContestoProceduraleUno = xdoc.CreateElement("Oggetto");
                oggettoContestoProceduraleUno.InnerText = "Pratica nuova";
                contestoProceduraleUno.AppendChild(oggettoContestoProceduraleUno);

                //Secondo Constesto Procedurale
                XmlElement contestoProceduraleDue = xdoc.CreateElement("ContestoProcedurale");

                XmlElement codiceAmmDue = xdoc.CreateElement("CodiceAmministrazione");
                codiceAmmDue.InnerText = reg.codAmministrazione;
                contestoProceduraleDue.AppendChild(codiceAmmDue);

                XmlElement codiceAOODue = xdoc.CreateElement("CodiceAOO");
                codiceAOODue.InnerText = reg.codRegistro;
                contestoProceduraleDue.AppendChild(codiceAOODue);

                XmlElement identificativoDue = xdoc.CreateElement("Identificativo");
                DocsPaVO.amministrazione.InfoAmministrazione infoAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(reg.idAmministrazione);
                //Se il protocollo titoalario esiste viene trasmesso con l'aggiunta del riferimento mittente che sarebbe il protocollo titolario epurato del sottonumero
                //altrimenti viene trasmesso solo il riferimento mittente
                if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario))
                    identificativoDue.InnerText = schedaDoc.protocolloTitolario + "$" + schedaDoc.riferimentoMittente;
                else
                    identificativoDue.InnerText = schedaDoc.riferimentoMittente;
                contestoProceduraleDue.AppendChild(identificativoDue);

                XmlElement tipoContestoProceduraleDue = xdoc.CreateElement("TipoContestoProcedurale");
                tipoContestoProceduraleDue.InnerText = "Codice Classifica";
                contestoProceduraleDue.AppendChild(tipoContestoProceduraleDue);

                XmlElement oggettoContestoProceduraleDue = xdoc.CreateElement("Oggetto");
                oggettoContestoProceduraleDue.InnerText = "Classificazione";
                contestoProceduraleDue.AppendChild(oggettoContestoProceduraleDue);

                riferimenti.AppendChild(contestoProceduraleUno);
                riferimenti.AppendChild(contestoProceduraleDue);
                
                #region Contesto procedurale RGS

                if (schedaDoc.template != null && !string.IsNullOrEmpty(schedaDoc.template.ID_CONTESTO_PROCEDURALE)
                    && schedaDoc.spedizioneDocumento != null && 
                    schedaDoc.spedizioneDocumento.tipoMessaggio != null && 
                    !string.IsNullOrEmpty(schedaDoc.spedizioneDocumento.tipoMessaggio.ID)
                    )
                {
                    //Verifico se è interoperante RGS

                    bool interoperanteRGS = false;
                    interoperanteRGS =  BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.CheckIsInteroperanteRGS(((DocsPaVO.utente.Corrispondente)destinatari[0]).systemId);
                    if (interoperanteRGS)
                    {
                        isInteropRGS = true;
                        DocsPaDB.Query_DocsPAWS.Model model = new DocsPaDB.Query_DocsPAWS.Model();
                        DocsPaVO.FlussoAutomatico.ContestoProcedurale contestoProceduraleRGS = model.GetContestoProceduraleById(schedaDoc.template.ID_CONTESTO_PROCEDURALE);
                        string idProcesso = FlussoAutomatico.FlussoAutomaticoManager.GetIdProcessoFlusso(schedaDoc, schedaDoc.spedizioneDocumento.tipoMessaggio);

                        XmlElement contestoProceduraleFlussoRGS = xdoc.CreateElement("ContestoProcedurale");
                        contestoProceduraleFlussoRGS.SetAttribute("id", idProcesso);

                        XmlElement codiceAmmRGS = xdoc.CreateElement("CodiceAmministrazione");
                        codiceAmmRGS.InnerText = reg.codAmministrazione;
                        contestoProceduraleFlussoRGS.AppendChild(codiceAmmRGS);

                        XmlElement codiceAOORGS = xdoc.CreateElement("CodiceAOO");
                        codiceAOORGS.InnerText = reg.codRegistro;
                        contestoProceduraleFlussoRGS.AppendChild(codiceAOORGS);

                        XmlElement identificativoRGS = xdoc.CreateElement("Identificativo");
                        identificativoRGS.InnerText = schedaDoc.spedizioneDocumento.tipoMessaggio.ID;
                        contestoProceduraleFlussoRGS.AppendChild(identificativoRGS);

                        XmlElement tipoContestoProceduraleRGS = xdoc.CreateElement("TipoContestoProcedurale");
                        tipoContestoProceduraleRGS.InnerText = contestoProceduraleRGS.TIPO_CONTESTO_PROCEDURALE;
                        contestoProceduraleFlussoRGS.AppendChild(tipoContestoProceduraleRGS);

                        #region PiuInfo

                        XmlElement piuInfoRGS = xdoc.CreateElement("PiuInfo");
                        piuInfoRGS.SetAttribute("XMLSchema", "AttributiEstesi.xsd");

                        XmlElement metadatiInterniRGS = xdoc.CreateElement("MetadatiInterni");
                        
                        //XmlCDataSection cDataRGS;
                        string dataRGS = "<![CDATA[<TIPOLOGIA><NOME>" + contestoProceduraleRGS.NOME + "</NOME><FAMIGLIA>" + contestoProceduraleRGS.FAMIGLIA + "</FAMIGLIA><VERSIONE>" + contestoProceduraleRGS.VERSIONE + "</VERSIONE>METADATOASSOCIATO</TIPOLOGIA>]]>";
                        string metadatoAssociato = string.Empty;

                        metadatoAssociato += "<MetadatoAssociato><Codice>TIPOLOGIA</Codice><Valore>" + schedaDoc.template.DESCRIZIONE + "</Valore></MetadatoAssociato>";

                        string codice = string.Empty;
                        string valore = string.Empty;
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in schedaDoc.template.ELENCO_OGGETTI)
                        {
                            if (!string.IsNullOrEmpty(ogg.VALORE_DATABASE))
                            {
                                codice = ogg.DESCRIZIONE;
                                valore = string.Empty;

                                switch (ogg.TIPO.DESCRIZIONE_TIPO)
                                {
                                    case "Link":
                                        valore = ogg.VALORE_DATABASE.Split(new string[] { "||||" }, StringSplitOptions.RemoveEmptyEntries)[0];
                                        break;
                                    case "Corrispondente":
                                       // XmlCDataSection cDataCorr;
                                        Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(ogg.VALORE_DATABASE);
                                        DocsPaVO.addressbook.CorrespondentDetails corrDett = null;
                                        if(corr.dettagli)
                                        {
                                            corrDett = new DocsPaDB.Query_DocsPAWS.Utenti().getCorrespondentDetails(ogg.VALORE_DATABASE);
                                        }
                                        if (corr != null)
                                        {
                                            string dataCorr = "<![CDATA[<AnagraficaDipendente><NomeDipendente>" + corr.nome + "</NomeDipendente><CognomeDipendente>" + corr.cognome + "</CognomeDipendente>";
                                            if(corrDett != null )
                                            {
                                                dataCorr += "<DataNascitaDipendente>" + corrDett.BirthDay + "</DataNascitaDipendente>";

                                                string sessoDipendente = string.Empty;
                                                if (!string.IsNullOrEmpty(corrDett.TaxId))
                                                { 
                                                    sessoDipendente = Convert.ToInt32(corrDett.TaxId.Substring(9, 2)) > 40 ? "F" : "M";
                                                }
                                                dataCorr += "<SessoDipendente>" + sessoDipendente + "</SessoDipendente>";
                                                dataCorr += "<ComuneNascitaDipendente>" + corrDett.BirthPlace + "</ComuneNascitaDipendente>";
                                                dataCorr += "<CodiceFiscaleDipendente>" + corrDett.TaxId + "</CodiceFiscaleDipendente>";
                                            }
                                            dataCorr += "</AnagraficaDipendente>]]]]><![CDATA[>";
                                            valore = dataCorr;
                                        }
                                        break;
                                    case "Contatore":
                                        String dataAnnullamento = String.Empty;
                                        valore = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(schedaDoc.docNumber, reg.idAmministrazione, false, out dataAnnullamento);
                                        break;
                                    case "CasellaDiSelezione":
                                        foreach (string val in ogg.VALORI_SELEZIONATI)
                                        {
                                            if (!string.IsNullOrEmpty(val))
                                                valore += "<Valore>" + val + "</valore>";
                                        }
                                        break;
                                    case "Data" :
                                        valore = Convert.ToDateTime(ogg.VALORE_DATABASE).ToString("dd/MM/yyyy");
                                        break;
                                    default:
                                        valore = ogg.VALORE_DATABASE;
                                        break;
                                }
                                if (ogg.TIPO.DESCRIZIONE_TIPO.Equals("CasellaDiSelezione"))
                                    metadatoAssociato += "<MetadatoAssociato><Codice>" + codice + "</Codice>" + valore + "</MetadatoAssociato>";
                                else
                                    metadatoAssociato += "<MetadatoAssociato><Codice>" + codice + "</Codice><Valore>" + valore + "</Valore></MetadatoAssociato>";
                            }
                        }

                        dataRGS = dataRGS.Replace("METADATOASSOCIATO", metadatoAssociato);
                        //cDataRGS = xdoc.CreateCDataSection(dataRGS);
                        //metadatiInterniRGS.AppendChild(cDataRGS);
                        metadatiInterniRGS.InnerText = dataRGS;

                        piuInfoRGS.AppendChild(metadatiInterniRGS);
                        contestoProceduraleFlussoRGS.AppendChild(piuInfoRGS);

                        #endregion

                        riferimenti.AppendChild(contestoProceduraleFlussoRGS);
                    }

                }

                #endregion

                #endregion Riferimenti Mittente

                //Descrizione
                XmlElement descrizione = xdoc.CreateElement("Descrizione");
                root.AppendChild(descrizione);
                XmlElement docPrinc = xdoc.CreateElement("Documento");

                
                string estensioneFile = getEstensione(getDocumentoPrincipale(schedaDoc).fileName);
                string nomefile="Documento_principale." + estensioneFile;
                if (CoppiaNomeFileENomeOriginale.ContainsKey(String.Format (pathFiles + @"\" + nomefile).ToLowerInvariant ()))
                    nomefile = CoppiaNomeFileENomeOriginale[String.Format ( pathFiles + @"\"+nomefile).ToLowerInvariant()];

                if (estensioneFile != "")
                {
                    docPrinc.SetAttribute("nome", nomefile);
                }
                else
                {
                    //per ovviare al problema del documentale EtDoc (dovrà essere studiata meglio la soluzione)
                    docPrinc.SetAttribute("nome", "empty.TXT");
                }
                logger.Debug(getDocumentoPrincipale(schedaDoc).fileName);
                logger.Debug("Estensione doc: " + getEstensione(getDocumentoPrincipale(schedaDoc).fileName));

                docPrinc.SetAttribute("tipoRiferimento", "MIME");

                //si aggiunge l'oggetto della schedaDocumento:
                XmlElement oggetto = xdoc.CreateElement("Oggetto");
                oggetto.InnerText = schedaDoc.oggetto.descrizione;
                docPrinc.AppendChild(oggetto);
                descrizione.AppendChild(docPrinc);
                
                foreach (string tsrVal in CoppiaNomeFileENomeOriginale.Values)
                {
                    if (Path.GetExtension(tsrVal).ToLowerInvariant()==".tsr")
                    {
                        DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato { fileName = tsrVal, descrizione = "Marca Temporale TSR", versionId ="1" };
                        schedaDoc.allegati.Add (all);
                    }
                }
                

                //si aggiungono gli allegati
                if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
                {
                    int countIS = 0;
                    int countPEC = 0;

                    for (int i = 0; i < schedaDoc.allegati.Count; i++)
                    {
                        if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) == "1")
                            countIS++;
                        if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) == "1")
                            countPEC++;
                    }

                    if (schedaDoc.allegati.Count - countIS - countPEC > 0)
                    {
                        XmlElement allegati = xdoc.CreateElement("Allegati");
                        descrizione.AppendChild(allegati);

                        for (int i = 0; i < schedaDoc.allegati.Count; i++)
                        {

                            if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) != "1" &&
                                BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).versionId) != "1")
                            {

                                XmlElement allegato = xdoc.CreateElement("Documento");
                                string fileExt = getEstensione(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);


                                string nomefileAll = "Allegato_" + (i + 1).ToString() + "." + fileExt;
                                if (fileExt == "")
                                    nomefileAll = "Allegato_" + (i + 1).ToString() + ".TXT";


                                if (CoppiaNomeFileENomeOriginale.ContainsKey(String.Format(pathFiles + @"\" + nomefileAll).ToLowerInvariant()))
                                    nomefileAll = CoppiaNomeFileENomeOriginale[String.Format(pathFiles + @"\" + nomefileAll).ToLowerInvariant()];
                                else if (fileExt.ToLowerInvariant() == "tsr")  //nel caso fosse un TSR (allegato ombra) prendo il nome popolato sopra.
                                    nomefileAll = Path.GetFileName(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);

                                allegato.SetAttribute("nome", nomefileAll);
                                allegato.SetAttribute("tipoRiferimento", "MIME");

                                if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione != null)
                                {
                                    XmlElement titoloDoc = xdoc.CreateElement("TitoloDocumento");
                                    titoloDoc.InnerText = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione;
                                    allegato.AppendChild(titoloDoc);
                                }

                                allegati.AppendChild(allegato);

                                if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).numeroPagine != 0)
                                {
                                    XmlElement numPagineAll = xdoc.CreateElement("NumeroPagine");
                                    numPagineAll.InnerText = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).numeroPagine.ToString();
                                    allegato.AppendChild(numPagineAll);
                                }
                            }
                        }
                    }
                }


                //NOTE (Augusto 30/08/2011)
                 string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_NOTE_IN_SEGNATURA");
                 if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                 {
                     DocsPaDB.Query_DocsPAWS.Documenti queryDoc = new DocsPaDB.Query_DocsPAWS.Documenti();
                     string ultimaNotaVisibileTutti = queryDoc.GetUltimaNotaVisibileTuttiDocumento(schedaDoc.systemId);
                     if (!string.IsNullOrEmpty(ultimaNotaVisibileTutti))
                     {
                         XmlElement noteDescrizione = xdoc.CreateElement("Note");
                         noteDescrizione.InnerText = ultimaNotaVisibileTutti;
                         //descrizione.SetAttribute("Note", ultimaNotaVisibileTutti);
                         descrizione.AppendChild(noteDescrizione);
                     }
                 }

                //Salvataggio
                 if (isInteropRGS)
                 {
                     string xmlString = HttpContext.Current.Server.HtmlDecode(xdoc.InnerXml);
                     File.WriteAllText(pathFiles + "\\segnatura.xml", xmlString);
                 }
                 else
                 {
                     System.IO.FileStream fs = new System.IO.FileStream(pathFiles + "\\segnatura.xml", System.IO.FileMode.Create);
                     xdoc.Save(fs);
                     fs.Close();
                 }

                return true;
            }
            catch (Exception e)
            {
                logger.Error("Errore nella creazione del file di segnatura. Eccezione: " + e.ToString());

                return false;
            }
        }

        #region segnatura complessiva
        public static bool creaSegnaturaComplessiva(DocsPaVO.utente.Corrispondente mittSegnatura,
                                                        DocsPaVO.utente.Registro reg,
                                                        DocsPaVO.documento.SchedaDocumento schedaDoc,
                                                        bool confermaRic,
                                                        string validatore,
                                                        out string xmlContent
                                                     )
        {
            bool esito = false;
            xmlContent = string.Empty;

            try
            {
                XmlDocument xdoc = new XmlDocument();
                //impostazione
                xdoc.XmlResolver = null;
                XmlDeclaration dec = xdoc.CreateXmlDeclaration("1.0", "ISO-8859-1", null);
                xdoc.AppendChild(dec);
                XmlDocumentType dtd = xdoc.CreateDocumentType("Segnatura", null, "Segnatura.dtd", null);
                xdoc.AppendChild(dtd);
                logger.Debug("dtd impostato");
                //creazione della root
                XmlElement root = xdoc.CreateElement("Segnatura");
                xdoc.AppendChild(root);

                //creazione dell'intestazione
                XmlElement intestazione = xdoc.CreateElement("Intestazione");
                root.AppendChild(intestazione);
                XmlElement identificatore = xdoc.CreateElement("Identificatore");
                intestazione.AppendChild(identificatore);
                XmlElement origine = xdoc.CreateElement("Origine");
                intestazione.AppendChild(origine);

                //destinatari
                if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Count > 0)
                {
                    for (int j = 0; j < ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Count; j++)
                    {
                        DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari[j];
                        XmlElement destinazione = xdoc.CreateElement("Destinazione");
                        if (confermaRic)
                            destinazione.SetAttribute("confermaRicezione", "si");
                        else
                            destinazione.SetAttribute("confermaRicezione", "no");

                        XmlElement indirizzoTelematico = xdoc.CreateElement("IndirizzoTelematico");
                        indirizzoTelematico.InnerText = corr.email;
                        destinazione.AppendChild(indirizzoTelematico);
                        XmlElement destinatario = xdoc.CreateElement("Destinatario");
                        getCorrispondente(corr, destinatario, xdoc);
                        destinazione.AppendChild(destinatario);
                        logger.Debug("Destinatario " + corr.descrizione + " aggiunto");
                        intestazione.AppendChild(destinazione);
                    }

                }
                //destinatari CC
                if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Count > 0)
                {
                    for (int i = 0; i < ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Count; i++)
                    {
                        DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza[i];
                        XmlElement destinazione = xdoc.CreateElement("PerConoscenza");
                        if (confermaRic)
                            destinazione.SetAttribute("confermaRicezione", "si");
                        else
                            destinazione.SetAttribute("confermaRicezione", "no");

                        XmlElement indirizzoTelematico = xdoc.CreateElement("IndirizzoTelematico");
                        indirizzoTelematico.InnerText = corr.email;
                        destinazione.AppendChild(indirizzoTelematico);
                        XmlElement destinatario = xdoc.CreateElement("Destinatario");
                        getCorrispondente(corr, destinatario, xdoc);
                        destinazione.AppendChild(destinatario);
                        logger.Debug("Destinatario per conoscenza " + corr.descrizione + " aggiunto");

                        intestazione.AppendChild(destinazione);
                    }

                }
                //Oggetto
                XmlElement oggettoInt = xdoc.CreateElement("Oggetto");
                intestazione.AppendChild(oggettoInt);

                //identificatore	
                XmlElement codiceAmm = xdoc.CreateElement("CodiceAmministrazione");
                identificatore.AppendChild(codiceAmm);
                codiceAmm.InnerText = reg.codAmministrazione;
                XmlElement codiceAOO = xdoc.CreateElement("CodiceAOO");
                identificatore.AppendChild(codiceAOO);
                codiceAOO.InnerText = reg.codRegistro;
                XmlElement numeroReg = xdoc.CreateElement("NumeroRegistrazione");
                identificatore.AppendChild(numeroReg);

                int MAX_LENGTH = 7;
                string zeroes = "";
                string numProto = schedaDoc.protocollo.numero;
                for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                {
                    zeroes = zeroes + "0";
                }
                numProto = zeroes + numProto;
                numeroReg.InnerText = numProto;

                //numeroReg.InnerText = schedaDoc.protocollo.numero;
                XmlElement dataReg = xdoc.CreateElement("DataRegistrazione");
                identificatore.AppendChild(dataReg);
                dataReg.InnerText = DocsPaUtils.Functions.Functions.CheckData_Invio(schedaDoc.protocollo.dataProtocollazione);  //DA CONVERTIRE

                //Origine
                XmlElement indirizzoTel = xdoc.CreateElement("IndirizzoTelematico");
                origine.AppendChild(indirizzoTel);
                if (reg != null)
                    indirizzoTel.InnerText = reg.email;

                XmlElement mittente = xdoc.CreateElement("Mittente");

                //Si riempie il campo mittente
                getCorrispondente(mittSegnatura, mittente, xdoc);
                XmlElement AOO = xdoc.CreateElement("AOO");
                mittente.AppendChild(AOO);
                XmlElement denominazioneAOO = xdoc.CreateElement("Denominazione");
                denominazioneAOO.InnerText = reg.codRegistro;
                AOO.AppendChild(denominazioneAOO);
                origine.AppendChild(mittente);


                //Oggetto
                oggettoInt.InnerText = schedaDoc.oggetto.descrizione;

                #region Riferimenti Mittente
                ////I contesti procedurali saranno due, il primo creato per mantenere la compatibilità con la vecchia versione dei CC
                ////il secondo è quello ufficiale?
                //XmlElement riferimenti = xdoc.CreateElement("Riferimenti");
                //root.AppendChild(riferimenti);

                ////Primo Contesto Procedurale
                //XmlElement contestoProceduraleUno = xdoc.CreateElement("ContestoProcedurale");

                //XmlElement codiceAmmUno = xdoc.CreateElement("CodiceAmministrazione");
                //codiceAmmUno.InnerText = reg.codAmministrazione;
                //contestoProceduraleUno.AppendChild(codiceAmmUno);

                //XmlElement codiceAOOUno = xdoc.CreateElement("CodiceAOO");
                //codiceAOOUno.InnerText = reg.codRegistro;
                //contestoProceduraleUno.AppendChild(codiceAOOUno);

                //XmlElement identificativoUno = xdoc.CreateElement("Identificativo");
                //if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario))
                //    identificativoUno.InnerText = schedaDoc.protocolloTitolario;
                //else
                //    identificativoUno.InnerText = schedaDoc.riferimentoMittente;
                //contestoProceduraleUno.AppendChild(identificativoUno);

                //XmlElement tipoContestoProceduraleUno = xdoc.CreateElement("TipoContestoProcedurale");
                //tipoContestoProceduraleUno.InnerText = "Protocollo Arma";
                //contestoProceduraleUno.AppendChild(tipoContestoProceduraleUno);

                //XmlElement oggettoContestoProceduraleUno = xdoc.CreateElement("Oggetto");
                //oggettoContestoProceduraleUno.InnerText = "Pratica nuova";
                //contestoProceduraleUno.AppendChild(oggettoContestoProceduraleUno);

                ////Secondo Constesto Procedurale
                //XmlElement contestoProceduraleDue = xdoc.CreateElement("ContestoProcedurale");

                //XmlElement codiceAmmDue = xdoc.CreateElement("CodiceAmministrazione");
                //codiceAmmDue.InnerText = reg.codAmministrazione;
                //contestoProceduraleDue.AppendChild(codiceAmmDue);

                //XmlElement codiceAOODue = xdoc.CreateElement("CodiceAOO");
                //codiceAOODue.InnerText = reg.codRegistro;
                //contestoProceduraleDue.AppendChild(codiceAOODue);

                //XmlElement identificativoDue = xdoc.CreateElement("Identificativo");
                //DocsPaVO.amministrazione.InfoAmministrazione infoAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(reg.idAmministrazione);
                ////Se il protocollo titoalario esiste viene trasmesso con l'aggiunta del riferimento mittente che sarebbe il protocollo titolario epurato del sottonumero
                ////altrimenti viene trasmesso solo il riferimento mittente
                //if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario))
                //    identificativoDue.InnerText = schedaDoc.protocolloTitolario + "$" + schedaDoc.riferimentoMittente;
                //else
                //    identificativoDue.InnerText = schedaDoc.riferimentoMittente;
                //contestoProceduraleDue.AppendChild(identificativoDue);

                //XmlElement tipoContestoProceduraleDue = xdoc.CreateElement("TipoContestoProcedurale");
                //tipoContestoProceduraleDue.InnerText = "Codice Classifica";
                //contestoProceduraleDue.AppendChild(tipoContestoProceduraleDue);

                //XmlElement oggettoContestoProceduraleDue = xdoc.CreateElement("Oggetto");
                //oggettoContestoProceduraleDue.InnerText = "Classificazione";
                //contestoProceduraleDue.AppendChild(oggettoContestoProceduraleDue);

                //riferimenti.AppendChild(contestoProceduraleUno);
                //riferimenti.AppendChild(contestoProceduraleDue);
                #endregion Riferimenti Mittente

                //Descrizione
                XmlElement descrizione = xdoc.CreateElement("Descrizione");
                root.AppendChild(descrizione);
                XmlElement docPrinc = xdoc.CreateElement("Documento");
                string nomeFile = getEstensione(getDocumentoPrincipale(schedaDoc).fileName);
                if (nomeFile != "")
                {
                    docPrinc.SetAttribute("nome", "Documento_principale." + nomeFile);
                }
                else
                {
                    //per ovviare al problema del documentale EtDoc (dovrà essere studiata meglio la soluzione)
                    docPrinc.SetAttribute("nome", "empty.TXT");
                }
                logger.Debug(getDocumentoPrincipale(schedaDoc).fileName);
                logger.Debug("Estensione doc: " + getEstensione(getDocumentoPrincipale(schedaDoc).fileName));

                docPrinc.SetAttribute("tipoRiferimento", "MIME");

                //si aggiunge l'oggetto della schedaDocumento:
                XmlElement oggetto = xdoc.CreateElement("Oggetto");
                oggetto.InnerText = schedaDoc.oggetto.descrizione;
                docPrinc.AppendChild(oggetto);
                descrizione.AppendChild(docPrinc);

                //si aggiungono gli allegati
                if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
                {
                    XmlElement allegati = xdoc.CreateElement("Allegati");
                    descrizione.AppendChild(allegati);
                    for (int i = 0; i < schedaDoc.allegati.Count; i++)
                    {
                        XmlElement allegato = xdoc.CreateElement("Documento");
                        string fileExt = getEstensione(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);
                        if (fileExt != "")
                        {
                            allegato.SetAttribute("nome", "Allegato_" + (i + 1).ToString() + "." + fileExt);
                        }
                        else
                        {
                            allegato.SetAttribute("nome", "Allegato_" + (i + 1).ToString() + ".TXT");
                        }
                        allegato.SetAttribute("tipoRiferimento", "MIME");

                        if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione != null)
                        {
                            XmlElement titoloDoc = xdoc.CreateElement("TitoloDocumento");
                            titoloDoc.InnerText = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione;
                            allegato.AppendChild(titoloDoc);
                        }

                        allegati.AppendChild(allegato);

                        if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).numeroPagine != 0)
                        {
                            XmlElement numPagineAll = xdoc.CreateElement("NumeroPagine");
                            numPagineAll.InnerText = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).numeroPagine.ToString();
                            allegato.AppendChild(numPagineAll);
                        }
                    }
                }

                //validazione file
                report.ProtoASL.ReportXML.XmlValidator valida = new BusinessLogic.report.ProtoASL.ReportXML.XmlValidator();
                esito = valida.xmlValidatorFromDTD(xdoc);

                if (esito)
                {
                    // Se la validazione è andata a buon fine, viene reperito il contenuto del file
                    using (MemoryStream stream = new MemoryStream())
                    {
                        xdoc.Save(stream);
                        stream.Position = 0;

                        using (StreamReader reader = new StreamReader(stream))
                            xmlContent = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore nella creazione del file di segnatura. Eccezione: " + e.ToString());
                return esito;
            }
            return esito;
        }


        #endregion

        #region Metodo Commentato
        //		/// <summary>
        //		/// </summary>
        //		/// <param name="infoUtente"></param>
        //		/// <param name="docNumber"></param>
        //		/// <param name="version"></param>
        //		/// <param name="versionId"></param>
        //		/// <param name="versionLabel"></param>
        //		/// <param name="logger"></param>
        //		/// <param name="debug"></param>
        //		/// <returns></returns>
        //		public static byte[] getDocument(DocsPaVO.utente.InfoUtente infoUtente,string docNumber, string version, string versionId, string versionLabel, DocsPa_V15_Utils.Logger logger)
        //		{
        //			string library = DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary();
        //							 
        //			/*
        //			PCDCLIENTLib.PCDDocObject DocObj=new PCDCLIENTLib.PCDDocObjectClass();
        //			*/
        //			DocsPaDocManagement.Documentale.Documento documento = new DocsPaDocManagement.Documentale.Documento(infoUtente.dst, library, DocsPaDocManagement.Documentale.Tipi.ObjectType.Def_Prof);
        //
        //			try
        //			{
        //				/*
        //				DocObj.SetDST(infoUtente.dst);
        //				DocObj.SetObjectType("DEF_PROF");
        //				DocObj.SetProperty("%TARGET_LIBRARY", library);
        //				*/
        //
        //				/*
        //				DocObj.SetProperty("%OBJECT_IDENTIFIER", docNumber);
        //				*/
        //				documento.ObjectIdentifier = docNumber;
        //				documento.Fetch();
        //
        //				/*
        //				DocObj.SetProperty("%STATUS", "%LOCK");
        //				*/
        //				documento.Status = DocsPaDocManagement.Documentale.Tipi.StatusType.Lock;
        //				documento.Update();
        //
        //				/*
        //				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(DocObj,"Errore nel locking");
        //				*/
        //				if(documento.GetErrorCode() != 0)
        //				{
        //					throw new Exception("Errore nel locking");
        //				}
        //
        //				logger.Debug("documento trovato e lockato");
        //
        //				/*
        //				PCDCLIENTLib.PCDGetDoc pGetDoc=new PCDCLIENTLib.PCDGetDocClass();
        //				*/
        //				DocsPaDocManagement.Documentale.AcquisizioneDocumento acquisizioneDocumento = new DocsPaDocManagement.Documentale.AcquisizioneDocumento(infoUtente.dst, library);
        //
        //				/*
        //				pGetDoc.SetDST(infoUtente.dst);
        //				pGetDoc.AddSearchCriteria("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
        //				*/
        //
        //				/*
        //				pGetDoc.AddSearchCriteria("%DOCUMENT_NUMBER", docNumber);
        //				pGetDoc.AddSearchCriteria("%VERSION", version);
        //				pGetDoc.AddSearchCriteria("%VERSION_ID", versionId);
        //				pGetDoc.AddSearchCriteria("%VERSION_LABEL", versionLabel);
        //				*/
        //				acquisizioneDocumento.AddSearchCriteria(DocsPaDocManagement.Documentale.Tipi.AcquisizioneSearchCriteriaType.DocumentNumber, docNumber);
        //				acquisizioneDocumento.AddSearchCriteria(DocsPaDocManagement.Documentale.Tipi.AcquisizioneSearchCriteriaType.Version, version);
        //				acquisizioneDocumento.AddSearchCriteria(DocsPaDocManagement.Documentale.Tipi.AcquisizioneSearchCriteriaType.VersionId, versionId);
        //				acquisizioneDocumento.AddSearchCriteria(DocsPaDocManagement.Documentale.Tipi.AcquisizioneSearchCriteriaType.VersionLabel, versionLabel);
        //
        //				acquisizioneDocumento.Execute();
        //
        //				/*
        //				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pGetDoc,"Errore nell'execute");
        //				*/
        //				if(acquisizioneDocumento.GetErrorCode() != 0)
        //				{
        //					throw new Exception("Errore nell'execute");
        //				}
        //
        //				acquisizioneDocumento.SetRow(1);
        //
        //				logger.Debug("Execute eseguito");
        //
        //				/*
        //				DocsPaWS.Utils.ErrorHandler.checkPCDOperation(pGetDoc,"Errore nel getDoc");
        //				*/
        //				if(acquisizioneDocumento.GetErrorCode() != 0)
        //				{
        //					throw new Exception("Errore nel getDoc");
        //				}
        //
        //				logger.addMessage("prova" + acquisizioneDocumento.Author);
        //				logger.addMessage("Stream..");
        //
        //				/*
        //				PCDCLIENTLib.PCDGetStream pGetStream;
        //				pGetStream = (PCDCLIENTLib.PCDGetStream)pGetDoc.GetPropertyValue("%CONTENT");
        //				*/
        //				DocsPaDocManagement.Documentale.AcquisizioneStream acquisizioneStream = acquisizioneDocumento.Stream;
        //
        //				if(acquisizioneStream == null)
        //				{
        //					logger.addMessage("Vuoto");
        //				}
        //
        //				int btread=1;  //numero di bytes che legge
        //				int fileSize=0;
        //				System.Collections.ArrayList fileContentArray=new System.Collections.ArrayList();
        //
        //				while(btread>0)
        //				{
        //					logger.addMessage("lettura");
        //
        //					/*
        //					byte[] cont=(byte[]) pGetStream.Read(128000,out btread);
        //					*/
        //					byte[] cont = acquisizioneStream.Read(128000,out btread);
        //
        //					logger.addMessage("fatta");
        //					fileSize=fileSize+btread;
        //					logger.addMessage(cont.ToString());
        //					fileContentArray.Add(cont);
        //				}
        //
        //				byte[] fileCont=new byte[fileSize];
        //
        //				for(int i=0;i<fileContentArray.Count;i++)
        //				{   
        //					logger.addMessage(i+" "+((byte[])fileContentArray[i]).Length);
        //					
        //					for(int k=0;k<((byte[])fileContentArray[i]).Length;k++)
        //					{
        //						fileCont[128000*i+k]=((byte[])fileContentArray[i])[k];
        //					}
        //				}
        //
        //				// unlock del documento
        //				documento = new DocsPaDocManagement.Documentale.Documento(infoUtente.dst, library, DocsPaDocManagement.Documentale.Tipi.ObjectType.Def_Prof);
        //
        //				/*
        //				DocObj.SetDST(infoUtente.dst);
        //				DocObj.SetObjectType("DEF_PROF");
        //				DocObj.SetProperty("%TARGET_LIBRARY", DocsPaWS.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getLibrary());
        //				*/
        //
        //				/*
        //				DocObj.SetProperty("%OBJECT_IDENTIFIER", docNumber);
        //				*/
        //				documento.ObjectIdentifier = docNumber;
        //
        //				documento.Fetch();
        //
        //				/*
        //				DocObj.SetProperty("%STATUS", "%UNLOCK");
        //				*/
        //				documento.Status = DocsPaDocManagement.Documentale.Tipi.StatusType.Unlock;
        //
        //				documento.Update();
        //
        //				return fileCont;
        //			}
        //			catch(Exception e)
        //			{
        //				/*
        //				DocObj.SetProperty("%STATUS", "%UNLOCK");
        //				*/
        //				documento.Status = DocsPaDocManagement.Documentale.Tipi.StatusType.Unlock;
        //
        //				documento.Update();
        //
        //				throw e;
        //			}
        //		}
        #endregion

        /// <summary>
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="mittente"></param>
        /// <param name="xdoc"></param>
        /// <param name="debug"></param>
        /// <param name="logger"></param>
        private static void getCorrispondente(DocsPaVO.utente.Corrispondente corr, XmlElement mittente, XmlDocument xdoc)
        {
            //			logger.addMessage("getCorrispondente");
            logger.Debug("getCorrispondente");

            System.Collections.ArrayList nomiUO = new System.Collections.ArrayList();
            DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
            DocsPaVO.utente.Ruolo ruolo = null;
            DocsPaVO.utente.Utente utente = null;

            if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
            {
                utente = (DocsPaVO.utente.Utente)corr;

                if (utente.ruoli != null && utente.ruoli.Count > 0)
                {
                    ruolo = (DocsPaVO.utente.Ruolo)utente.ruoli[0];
                    uo = ruolo.uo;
                }
                else
                {
                    //					logger.addMessage("Utente sciolto");
                    logger.Debug("Utente sciolto");
                    uo = null;
                }
            }

            if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
            {
                ruolo = (DocsPaVO.utente.Ruolo)corr;
                uo = ruolo.uo;
            }

            if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
            {
                uo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
            }

            string indPostString = null;

            if (uo != null)
            {
                indPostString = uo.indirizzo;
            }

            while (uo != null)
            {
                if (!string.IsNullOrEmpty(uo.descrizione))
                    nomiUO.Add(uo.descrizione);

                if (uo.parent != null)
                {
                    uo = uo.parent;
                    //perchè la uo ha solo l'id, così estraggo gli altri dati.
                    uo = (DocsPaVO.utente.UnitaOrganizzativa)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(uo.systemId);
                }
                else
                    break;
            }

            //			logger.addMessage("nomiUO count="+nomiUO.Count);
            logger.Debug("nomiUO count=" + nomiUO.Count);

            //costruzione dell'elemento XML
            XmlElement amministrazione = xdoc.CreateElement("Amministrazione");
            mittente.AppendChild(amministrazione);
            XmlElement amministrazioneNome = xdoc.CreateElement("Denominazione");

            if (nomiUO.Count > 0)
            {
                amministrazioneNome.InnerText = (string)nomiUO[nomiUO.Count - 1];
            }
            else
            {
                amministrazioneNome.InnerText = "Non specificato";
            }

            amministrazione.AppendChild(amministrazioneNome);
            XmlElement temp = amministrazione;

            for (int i = nomiUO.Count - 1; i > -1; i--)
            {
                XmlElement uoEl = xdoc.CreateElement("UnitaOrganizzativa");
                XmlElement uoNome = xdoc.CreateElement("Denominazione");
                uoNome.InnerText = (string)nomiUO[i];
                uoEl.AppendChild(uoNome);
                temp.AppendChild(uoEl);
                temp = uoEl;
            }

            XmlElement temp2 = temp;

            //ruolo e utente
            if (ruolo != null)
            {
                XmlElement ruo = xdoc.CreateElement("Ruolo");
                XmlElement denomRuolo = xdoc.CreateElement("Denominazione");
                denomRuolo.InnerText = ruolo.descrizione;
                ruo.AppendChild(denomRuolo);
                temp.AppendChild(ruo);
                temp = ruo;
            }

            if (utente != null)
            {
                XmlElement ut = xdoc.CreateElement("Persona");
                XmlElement denomUt = xdoc.CreateElement("Denominazione");
                denomUt.InnerText = utente.descrizione;
                ut.AppendChild(denomUt);
                temp.AppendChild(ut);
                temp = ut;
            }

            //indirizzo postale
            XmlElement indPost = xdoc.CreateElement("IndirizzoPostale");
            XmlElement denomInd = xdoc.CreateElement("Denominazione");

            if (indPostString != null)
            {
                denomInd.InnerText = indPostString;
            }
            else
            {
                denomInd.InnerText = "Non specificato";
            }

            indPost.AppendChild(denomInd);
            temp2.AppendChild(indPost);
            //			logger.addMessage("Indirizzo postale inserito");
            logger.Debug("Indirizzo postale inserito");
        }

        #region Metodo Commentato
        /*private static bool creaMail1(DocsPaVO.documento.SchedaDocumento schedaDoc,string username,string password,string server,string port,string mailMitt,string mailDest,string pathFiles,DocsPaWS.Utils.Logger logger)
		{
			WinToolZone.CSLMail.SMTP smtp=new WinToolZone.CSLMail.SMTP();
			try
			{
				smtp.Username=username; 
				smtp.Password=password;
				smtp.Authentication=WinToolZone.CSLMail.SMTP.SMTPAuthenticationType.LOGIN;
				smtp.From=mailMitt;
				smtp.To=mailDest;
				smtp.MailType=WinToolZone.CSLMail.SMTP.MailEncodingType.HTML;
				smtp.Subject=schedaDoc.oggetto.descrizione;
				//body della mail
				string bodyMail="Si trasmette come file allegato a questa e-mail il documento e gli eventuali allegati.<br>";
                bodyMail=bodyMail+"Registro: " + schedaDoc.registro.codRegistro + "<br>";
                bodyMail=bodyMail+"Numero di protocollo: " + schedaDoc.protocollo.numero + "<br>";
			    bodyMail=bodyMail+"Data protocollazione: " + schedaDoc.protocollo.dataProtocollazione + "<br>";
		        bodyMail=bodyMail+"Segnatura: " + schedaDoc.protocollo.segnatura + "<br>";
				smtp.Message=bodyMail;
				smtp.SMTPServer=server;
				if(port!=null)
				{
					smtp.SMTPPort=(short) Int32.Parse(port);
				}
				string[] files=System.IO.Directory.GetFiles(pathFiles);
				for(int i=0;i<files.Length;i++)
				{
					if(smtp.AddAttachment(files[i]))
					{
						logger.addMessage("Attachment "+files[i]+" inserito");
					}
					else
					{
						logger.addMessage("Attachment "+files[i]+"non inserito");
					};
				}
				if(!smtp.SendMail())
				{ 
					logger.addMessage("Invio mail non eseguito"+smtp.ErrorDescription);
					return false;
				}
				return true;
			}
			catch(Exception e)
			{
				logger.addMessage("Creazione ed invio mail non eseguito. Eccezione: "+e.ToString());
				return false;
			}
		}*/
        #endregion

        private static bool creaMail(DocsPaVO.documento.SchedaDocumento schedaDoc, string server, string mailMitt, string smtp_user, string smtp_pwd, string mailDest, string pathFiles, Dictionary<string, string> CoppiaNomeFileENomeOriginale, string port, string SmtpSsl, string PopSsl, string smtpSTA, string X_TipoRicevuta)
        {
            bool retValue = false;

            SvrPosta svr = new SvrPosta(server,
                smtp_user,
                smtp_pwd,
                port,
                Path.GetTempPath(),
                    CMClientType.SMTP, SmtpSsl, PopSsl, smtpSTA);

            try
            {
                svr.connect();

                //body della mail
                string bodyMail = "Si trasmette come file allegato a questa e-mail il documento e gli eventuali allegati.<br>";
                bodyMail = bodyMail + "Registro: " + schedaDoc.registro.codRegistro + "<br>";
                bodyMail = bodyMail + "Numero di protocollo: " + schedaDoc.protocollo.numero + "<br>";
                bodyMail = bodyMail + "Data protocollazione: " + schedaDoc.protocollo.dataProtocollazione + "<br>";
                bodyMail = bodyMail + "Segnatura: " + schedaDoc.protocollo.segnatura + "<br>";


                string subject = string.Empty;

                if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["SEGNATURA_NEL_SUBJECT"]) &&
                    bool.Parse(ConfigurationSettings.AppSettings["SEGNATURA_NEL_SUBJECT"]))
                    subject = schedaDoc.protocollo.segnatura + " - " + schedaDoc.oggetto.descrizione;
                else
                    subject = schedaDoc.oggetto.descrizione;

                //aggiunta del docnumber all'oggetto delal mail per la gestione delle ricevute pec
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["GESTIONE_RICEVUTE_PEC"]) &&
                    bool.Parse(ConfigurationManager.AppSettings["GESTIONE_RICEVUTE_PEC"]))
                    subject += "#" + schedaDoc.docNumber + "#";

                List<CMMailHeaders> headers = new List<CMMailHeaders>();
                if ((X_TipoRicevuta != null) && (X_TipoRicevuta != string.Empty))
                    headers.Add(new CMMailHeaders { header = "X-TipoRicevuta", value = X_TipoRicevuta });

                string[] files = System.IO.Directory.GetFiles(pathFiles);
                List<CMAttachment> attachLst = new List<CMAttachment>();
                foreach (string file in files)
                {
                    CMAttachment att = new CMAttachment(Path.GetFileName(file), Interoperabilità.MimeMapper.GetMimeType(Path.GetExtension(file)), file);
                    //Valorizzo il nome originale del file qualora esso fosse presente
                    if (CoppiaNomeFileENomeOriginale.ContainsKey(file.ToLowerInvariant()))
                        att.name = CoppiaNomeFileENomeOriginale[file.ToLowerInvariant()];

                    attachLst.Add(att);
                }

                string outErr;
                svr.sendMail(
                    mailMitt,
                    mailDest,
                    "",
                    "",
                    subject,
                    bodyMail,
                    CMMailFormat.HTML,
                    attachLst.ToArray(),
                    headers.ToArray(), out outErr);

                retValue = true;
            }
            catch (Exception e)
            {
                logger.Error("Creazione ed invio mail non eseguito. Eccezione: " + e.ToString());

                throw e;
            }
            finally
            {
                svr.disconnect();
            }

            return retValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string getEstensione(string fileName)
        {
            char[] dot = { '.' };

            #region OLD
            //			string[] parts=fileName.Split(dot);
            //			string suffix=parts[parts.Length-1];
            //			
            //			if(suffix.ToUpper().Equals("P7M"))
            //			{
            //			   suffix=fileName.Substring(fileName.IndexOf(".")+1);
            //			}
            #endregion

            //inizio modifica: il documento principale in alcuni casi non veniva allegato,
            //Ciò accedeva quando durante l'estrazione del documento principale, per allegarlo alla mail,
            //nel percorso del file è presente un ulteriore '.', oltre a quello relativo all'estensione.
            //Ad esempio con un path del genere andava in errore ENTEC\2005\EC_RU\UO1.1\Partenza\467.XLS

            string[] parts;
            string suffix = "";
            if (!fileName.ToUpper().EndsWith("P7M"))
            {
                parts = fileName.Split(dot);
                suffix = parts[parts.Length - 1];
            }
            else
            {
                string appodocPrincipaleName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                parts = appodocPrincipaleName.Split(dot);
                int cont = 0;
                for (int i = 2; i < parts.Length; i++)
                {
                    if (parts[i].ToUpper().Equals("P7M"))
                    {
                        cont = cont + 1;
                        suffix = suffix + ".P7M";
                    }
                }
                suffix = parts[parts.Length - cont - 1] + suffix;
            }
            return suffix;
        }

        /// <summary>
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="debug"></param>
        /// <param name="logger"></param>
        /// <param name="idAmm">Id dell'amministrazione cui appartiene l'utente che ha effettuata la spedizione</param>
        /// <returns></returns>
        public static System.Collections.Hashtable dividiDestinatari(System.Collections.ArrayList dest, string codRegistroMitt, string emailReg, SendDocumentResponse retValue, String idAmm)
        {
            System.Collections.Hashtable result = new System.Collections.Hashtable();

            for (int i = 0; i < dest.Count; i++)
            {
                try
                {
                    if (dest[i].GetType() != typeof(DocsPaVO.utente.Corrispondente))
                    {
                        bool isMailPrefCorr;
                        bool isMailPref;
                        string emailCorr = null;
                        string varCodiceAOO = string.Empty;
                        string varCodiceAMM = string.Empty;
                        //si estrae la UO del destinatario
                        DocsPaVO.utente.UnitaOrganizzativa uo = null;
                        DocsPaVO.utente.RaggruppamentoFunzionale rf = null;
                        string tipoIE = string.Empty;
                        
                        // Booleano utilizzato per indicare se bisogna spedire per interoperabilità semplificata
                        bool isSimpInterop = false;
                        String varUrl = String.Empty;



                        if (dest[i].GetType() == typeof(DocsPaVO.utente.Utente))
                        {
                            System.Collections.ArrayList ruoliUtente = new System.Collections.ArrayList();

                            tipoIE = ((DocsPaVO.utente.Utente)dest[i]).tipoIE; // RICAVO IL TIPO I/E

                            if (tipoIE == "I")
                            {
                                logger.Debug("Destinatario " + i + ": utente INTERNO");

                                ruoliUtente = BusinessLogic.Utenti.UserManager.getRuoliUtente(((DocsPaVO.utente.Utente)dest[i]).idPeople);
                                if (ruoliUtente != null)
                                {
                                    if (ruoliUtente.Count > 0)
                                    {
                                        uo = ((DocsPaVO.utente.Ruolo)ruoliUtente[0]).uo;
                                    }
                                }
                                else
                                {
                                    uo = null;
                                }
                            }
                            else
                            {
                                logger.Debug("Destinatario " + i + ": utente ESTERNO");
                                uo = null;
                            }

                            if (uo != null) // caso di utenti interni
                            {
                                emailCorr = uo.email;
                                varCodiceAOO = uo.codiceAOO;
                                varCodiceAMM = uo.codiceAmm;
                                //luluciani 26/10/2005
                                ((DocsPaVO.utente.Utente)dest[i]).codiceAmm = varCodiceAMM;
                                ((DocsPaVO.utente.Utente)dest[i]).codiceAOO = varCodiceAOO;

                            }
                            else // caso di utenti esterni
                            {
                                emailCorr = ((DocsPaVO.utente.Utente)dest[i]).email;
                                varCodiceAMM = ((DocsPaVO.utente.Utente)dest[i]).codiceAmm;
                                varCodiceAOO = ((DocsPaVO.utente.Utente)dest[i]).codiceAOO;

                            }
                        }

                        if (dest[i].GetType() == typeof(DocsPaVO.utente.Ruolo))
                        {
                            tipoIE = ((DocsPaVO.utente.Ruolo)dest[i]).tipoIE; // RICAVO IL TIPO I/E

                            if (((DocsPaVO.utente.Ruolo)dest[i]).tipoIE != null && ((DocsPaVO.utente.Ruolo)dest[i]).tipoIE == "I")
                            {
                                logger.Debug("Destinatario " + i + ": ruolo INTERNO");
                                uo = ((DocsPaVO.utente.Ruolo)dest[i]).uo;
                                emailCorr = uo.email;
                                varCodiceAOO = uo.codiceAOO;
                                varCodiceAMM = uo.codiceAmm;
                            }
                            else
                            {
                                logger.Debug("Destinatario " + i + ": ruolo ESTERNO");
                                emailCorr = ((DocsPaVO.utente.Ruolo)dest[i]).email;
                                varCodiceAOO = ((DocsPaVO.utente.Ruolo)dest[i]).codiceAOO;
                                varCodiceAMM = ((DocsPaVO.utente.Ruolo)dest[i]).codiceAmm;
                            }
                        }

                        if (dest[i].GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
                        {
                            tipoIE = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]).tipoIE; // RICAVO IL TIPO I/E

                            logger.Debug("Destinatario " + i + ": unita' organizzativa");

                            uo = (DocsPaVO.utente.UnitaOrganizzativa)dest[i];
                            emailCorr = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]).email;
                            varCodiceAOO = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]).codiceAOO;
                            varCodiceAMM = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]).codiceAmm;
                            
                            // Verifica se utilizzare l'interoperabilità semplificata
                            DocsPaVO.utente.UnitaOrganizzativa c = ((DocsPaVO.utente.UnitaOrganizzativa)dest[i]);
                            isSimpInterop = c.canalePref != null && (c.canalePref.tipoCanale == InteroperabilitaSemplificataManager.InteroperabilityCode || c.canalePref.typeId == InteroperabilitaSemplificataManager.InteroperabilityCode) && c.Url != null && c.Url.Count > 0 && !String.IsNullOrEmpty(c.Url[0].Url) && Uri.IsWellFormedUriString(c.Url[0].Url, UriKind.Absolute);
                            varUrl = c.Url != null && c.Url.Count > 0 ? c.Url[0].Url : String.Empty;
                        }

                        if (dest[i].GetType() == typeof(DocsPaVO.utente.RaggruppamentoFunzionale))
                        {
                            tipoIE = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]).tipoIE; 

                            logger.Debug("Destinatario " + i + ": unita' organizzativa");

                            rf = (DocsPaVO.utente.RaggruppamentoFunzionale)dest[i];

                            emailCorr = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]).email;
                            varCodiceAOO = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]).codiceAOO;
                            varCodiceAMM = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]).codiceAmm;

                            // Verifica se utilizzare l'interoperabilità semplificata per l'RF
                            DocsPaVO.utente.RaggruppamentoFunzionale c = ((DocsPaVO.utente.RaggruppamentoFunzionale)dest[i]);
                            isSimpInterop = c.canalePref != null && 
                                (c.canalePref.tipoCanale == InteroperabilitaSemplificataManager.InteroperabilityCode || c.canalePref.typeId == InteroperabilitaSemplificataManager.InteroperabilityCode)
                                && c.Url != null && c.Url.Count > 0 && !String.IsNullOrEmpty(c.Url[0].Url) && Uri.IsWellFormedUriString(c.Url[0].Url, UriKind.Absolute);
                            varUrl = c.Url != null && c.Url.Count > 0 ? c.Url[0].Url : String.Empty;
                        }

                        // Se il canale preferito è interoperabilità semplificata, questa si può utilizzare solo se attiva
                        if (isSimpInterop)
                            isSimpInterop &= InteroperabilitaSemplificataManager.IsEnabledSimplifiedInteroperability(idAmm);

                        //Federica
                        if (tipoIE.Equals("E"))
                        {
                            isMailPref = false;
                            if (dest[i].GetType() != typeof(DocsPaVO.utente.Utente)) //corrispondente di tipo uo/ruolo/rf
                                if (uo != null)
                                    isMailPref = isMailPreferred(uo);
                                else
                                    isMailPref = isMailPreferred(rf);
                            if (!isMailPref && dest[i].GetType() != typeof(DocsPaVO.utente.Utente))
                            {
                                //se canale preferenziale non interop, verifico il mezzo di spedizione
                                if(uo != null)
                                    isMailPref = (uo.canalePref.descrizione.ToUpper().Equals("MAIL") || uo.canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) ? true : false;
                                else if(rf != null)
                                    isMailPref = (rf.canalePref.descrizione.ToUpper().Equals("MAIL") || rf.canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) ? true : false;
                            }
                            isMailPrefCorr = isMailPreferred((DocsPaVO.utente.Corrispondente)dest[i]);
                            if (!isMailPrefCorr)
                            {
                                //se canale preferenziale non interop, verifico il mezzo di spedizione
                                isMailPrefCorr = (((DocsPaVO.utente.Corrispondente)dest[i]).canalePref.descrizione.ToUpper().Equals("MAIL") ||
                                                    ((DocsPaVO.utente.Corrispondente)dest[i]).canalePref.descrizione.ToUpper().Equals("INTEROPERABILITA")) ? true : false;
                            }
                        }
                        else
                        {
                            // per i corrisp interni nel caso di interoperabilità si suppone che il canale
                            // preferenziale sia la mail
                            isMailPref = true;
                            isMailPrefCorr = false;
                        }

                        string email = null;

                        //CONTROLLO NUOVO
                        if (uo != null && uo.interoperante)
                        {
                            logger.Debug("UO interoperante");

                            //OLD:IN QUESTO CASO SI PRENDE L'INDIRIZZO MAIL DELL'UO SE QUESTA HA COME CANALE PREF. LA MAIL, ALTRIMENTI NULLA

                            //no si prende sempre l'indirizzo mail della AOO data dal varCodiceAOO
                            if (isMailPref)
                            {
                                //old:email=uo.email;

                                email = GetMailAOOInteropbyDest(uo);

                                logger.Debug("Email destinatario " + i + ": " + email);
                            }
                            else
                            {
                                logger.Debug("La mail non e' canale preferenziale della UO. Destinatario, quindi per noi non interoperante " + i + " scartato");
                                email = null;
                            }
                        }
                        else
                        {
                            logger.Debug("UO nulla o non interoperante");
                            //IMPORTANTE: UTENTI E RUOLI INTERNI POSSONO INTEROPERARE SOLO SE APPARTENGONO AD UNA UO INTEROPERANTE
                            //IN QUESTO CASO SI PRENDE L'INDIRIZZO MAIL DELL'UTENTE SE QUESTO HA COME CANALE PREFERENZIALE LA MAIL 
                            //RUOLI; UTENTI interni all'amministrazione non devono essere inseriti nella DPA_T_CANALE_CORR
                            if (isMailPrefCorr)
                            {
                                email = emailCorr;
                                logger.Debug("Mail destinatario " + i + ": " + email);
                            }
                            else
                            {
                                logger.Debug("La mail non e' canale preferenziale del destinatario. Destinatario " + i + " scartato");
                                email = null;
                            }
                        }

                        
                        //aggiunta condizione per spedire solamente a quei destinatari che appartengono ad AOO differenti da quella del mittente
                        //MAC_INPS 3749
                        if ((!string.IsNullOrEmpty(varCodiceAOO) && varCodiceAOO != codRegistroMitt && tipoIE == "I") || (
                            !string.IsNullOrEmpty(email) &&
                            email != emailReg) ||
                            isSimpInterop)
                        {
                            //if ((email != null) && (!email.Equals("")) && IsValidEmail(email) == true)
                            //{
                            if (email == null)
                                email = string.Empty;

                            // Se si è nel caso di spedizione per interoperabilità semplificata, la chiave è l'url
                            if (isSimpInterop)
                                email = varUrl;

                            if (result.ContainsKey(email))
                            {
                                ((System.Collections.ArrayList)result[email]).Add(dest[i]);
                            }
                            else
                            {

                                System.Collections.ArrayList al = new System.Collections.ArrayList();
                                al.Add(dest[i]);
                                result.Add(email, al);
                            }
                            //}
                            //else //da Scartare
                            //{
                            //    //modifica gennaro
                            //    bool interop_no_mail = false;
                            //    DocsPaVO.utente.Corrispondente c = (DocsPaVO.utente.Corrispondente)dest[i];
                            //    string Email = "";
                            //    if (!string.IsNullOrEmpty(c.email))
                            //        Email = c.email;

                            //    if (Interoperabilità.InteroperabilitaUtils.InteropIntNoMail && c.tipoIE == "I")
                            //    {
                            //        c.codiceAOO = varCodiceAOO;
                            //        interop_no_mail = true;
                            //    }


                            //    SendDocumentResponse.SendDocumentMailResponse r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(Email, true, "KO");//ex ok
                            //    r.Destinatari = new ArrayList();
                            //    r.Destinatari.Add(c);

                            //    if (varCodiceAOO == codRegistroMitt)
                            //        r.SendErrorMessage = "Destinatario Non Interoperante, poichè appartenente alla stessa AOO";

                            //    if (!interop_no_mail || varCodiceAOO == codRegistroMitt)
                            //    {
                            //        r.SendErrorMessage = "Destinatario Non Interoperante";
                            //        r.SendSucceded = false;
                            //        r.MailNonInteroperante = true;
                            //    }
                            //    else
                            //    {
                            //        System.Collections.ArrayList al = new System.Collections.ArrayList();
                            //        al.Add(c);
                            //        //result.Add(c.codiceAmm, al);
                            //        //result.Add(c.codiceAOO, al);
                            //        result.Add(c.codiceAOO, al);
                            //    }
                            //    retValue.SendDocumentMailResponseList.Add(r);
                            //    //fine modifica gennaro
                            //}
                        }
                        else //da Scartare
                        {
                            //modifica gennaro
                            DocsPaVO.utente.Corrispondente c = (DocsPaVO.utente.Corrispondente)dest[i];
                            string Email = "";
                            bool interop_no_mail = false;
                            if (!string.IsNullOrEmpty(c.email))
                                Email = c.email;

                            if (Interoperabilità.InteroperabilitaUtils.InteropIntNoMail && c.tipoIE == "I")
                            {
                                c.codiceAOO = varCodiceAOO;
                                interop_no_mail = true;
                            }
                            SendDocumentResponse.SendDocumentMailResponse r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(Email, true, "KO");
                            r.Destinatari = new ArrayList();
                            r.Destinatari.Add(c);

                            if (varCodiceAOO == codRegistroMitt)
                                r.SendErrorMessage = "Destinatario Non Interoperante, poichè appartenente alla stessa AOO";

                            if (!interop_no_mail || varCodiceAOO == codRegistroMitt)
                            {
                                if (email != emailReg)
                                    r.SendErrorMessage = "Destinatario Non Interoperante, poichè appartenente alla stessa AOO";
                                else
                                    if (emailReg != null && emailReg == "")
                                        r.SendErrorMessage = "Attenzione, AOO mittente non ha alcuna mail associata.";
                                r.SendSucceded = false;
                                r.MailNonInteroperante = true;
                            }
                            else
                            {
                                System.Collections.ArrayList al = new System.Collections.ArrayList();
                                al.Add(c);
                                //MAC 3749
                                result.Add(c.codiceAOO + i.ToString(), al);
                                //result.Add(c.codiceCorrispondente, al);
                                //result.Add(c.codiceAmm, al);
                            }
                            retValue.SendDocumentMailResponseList.Add(r);
                            //fine modifica gennaro
                        }
                    }
                    else //OCCASIONALI sono da Scartare
                    {
                        DocsPaVO.utente.Corrispondente c = (DocsPaVO.utente.Corrispondente)dest[i];
                        string Email = "";

                        if (c.email != null)
                            Email = c.email;

                        if (string.IsNullOrEmpty(Email))
                        {
                            SendDocumentResponse.SendDocumentMailResponse r = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(Email, true, "OK");
                            r.Destinatari = new ArrayList();
                            r.Destinatari.Add(c);
                            r.SendErrorMessage = "Destinatario Occasionale non interoperante";
                            r.SendSucceded = false;
                            r.MailNonInteroperante = true;
                            retValue.SendDocumentMailResponseList.Add(r);
                        }
                        else
                        {
                            System.Collections.ArrayList al = new System.Collections.ArrayList();
                            al.Add(dest[i]);
                            result.Add(Email, al);

                        }

                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }

            return result;
        }

        /* Serve per verificare l'esatto formato dell'email */
        public static bool IsValidEmail(string strToCheck)
        {
            return Regex.IsMatch(strToCheck, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// </summary>
        /// <param name="corr"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static bool isMailPreferred(DocsPaVO.utente.Corrispondente corr)
        {
            System.Data.DataSet ds;

            try
            {
                DocsPaDB.Query_DocsPAWS.Interoperabilita obj = new DocsPaDB.Query_DocsPAWS.Interoperabilita();
                obj.getDatiCan(out ds, corr);

                if (ds.Tables["CANALE"].Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="schedaDoc"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.Documento getDocumentoPrincipale(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            #region Codice Commentato
            /* Elimino il controllo sul documento da inviare
			System.Collections.ArrayList docs=schedaDoc.documenti;
			DocsPaVO.documento.Documento docRes=null;
			for(int i=0;i<docs.Count;i++)
			{
				string invio=((DocsPaVO.documento.Documento) docs[i]).daInviare;
				if(invio!=null && invio.Equals("1"))
				{
					docRes=(DocsPaVO.documento.Documento) docs[i];
				}
			}
			if(docRes==null){
			  docRes=(DocsPaVO.documento.Documento) docs[0];
			}
			return docRes;
			*/
            #endregion

            return (DocsPaVO.documento.Documento)schedaDoc.documenti[0];
        }

        //private static DocsPaInteropSemplificata.WRInteropSemp.Segnatura CreaSegnaturaObject(DocsPaVO.utente.Corrispondente mittSegnatura,
        //                                                                                DocsPaVO.utente.Registro reg,
        //                                                                                DocsPaVO.utente.Registro regMittente,
        //                                                                                DocsPaVO.documento.SchedaDocumento schedaDoc,
        //                                                                                string mailDest,
        //                                                                                System.Collections.ArrayList destinatari,
        //                                                                                string pathFiles,
        //                                                                                bool confermaRic)
        //{
        //    DocsPaInteropSemplificata.WRInteropSemp.Segnatura retValue = new DocsPaInteropSemplificata.WRInteropSemp.Segnatura();

        //    #region Intestazione
        //    DocsPaInteropSemplificata.WRInteropSemp.Intestazione intestazione = new DocsPaInteropSemplificata.WRInteropSemp.Intestazione();
        //    retValue.Intestazione = intestazione;

        //    #region Intestazione --> Identificatore
        //    DocsPaInteropSemplificata.WRInteropSemp.Identificatore identificatore = new DocsPaInteropSemplificata.WRInteropSemp.Identificatore();
        //    retValue.Intestazione.Identificatore = identificatore;
        //    retValue.Intestazione.Identificatore.CodiceAmministrazione = reg.codAmministrazione;
        //    retValue.Intestazione.Identificatore.CodiceAOO = reg.codRegistro;
        //    retValue.Intestazione.Identificatore.NumeroRegistrazione = schedaDoc.protocollo.numero;
        //    retValue.Intestazione.Identificatore.DataRegistrazione = DocsPaUtils.Functions.Functions.CheckData_Invio(schedaDoc.protocollo.dataProtocollazione);  //DA CONVERTIRE
        //    #endregion Intestazione --> Identificatore

        //    #region Intestazione --> Origine
        //    DocsPaInteropSemplificata.WRInteropSemp.Origine origine = new DocsPaInteropSemplificata.WRInteropSemp.Origine();
        //    retValue.Intestazione.Origine = origine;
        //    DocsPaInteropSemplificata.WRInteropSemp.IndirizzoTelematico indirizzoTelematicoMitt = new DocsPaInteropSemplificata.WRInteropSemp.IndirizzoTelematico();
        //    retValue.Intestazione.Origine.IndirizzoTelematico = indirizzoTelematicoMitt;

        //    if (regMittente != null)
        //        retValue.Intestazione.Origine.IndirizzoTelematico.Value = regMittente.email;
        //    else
        //        retValue.Intestazione.Origine.IndirizzoTelematico.Value = reg.email;


        //    #region Intestazione --> Origine --> Mittente
        //    retValue.Intestazione.Origine.Mittente = (DocsPaInteropSemplificata.WRInteropSemp.Mittente)getMittentePerSegnaturaObject(mittSegnatura);
        //    #endregion Intestazione --> Origine --> Mittente

        //    #region Intestazione --> Origine --> AOO
        //    DocsPaInteropSemplificata.WRInteropSemp.AOO aoo = new DocsPaInteropSemplificata.WRInteropSemp.AOO();
        //    aoo.Denominazione = reg.codRegistro;
        //    retValue.Intestazione.Origine.Mittente.AOO = aoo;
        //    #endregion Intestazione --> Origine --> AOO

        //    #endregion Intestazione --> Origine

        //    #region Intestazione --> Destinazione
        //    DocsPaInteropSemplificata.WRInteropSemp.Destinazione destinazione = new DocsPaInteropSemplificata.WRInteropSemp.Destinazione();
        //    List<DocsPaInteropSemplificata.WRInteropSemp.Destinazione> destinazioneList = new List<DocsPaInteropSemplificata.WRInteropSemp.Destinazione>();

        //    if (confermaRic)
        //    {
        //        destinazione.confermaRicezione = DocsPaInteropSemplificata.WRInteropSemp.DestinazioneConfermaRicezione.si;
        //    }
        //    else
        //    {
        //        destinazione.confermaRicezione = DocsPaInteropSemplificata.WRInteropSemp.DestinazioneConfermaRicezione.no;
        //    }

        //    DocsPaInteropSemplificata.WRInteropSemp.IndirizzoTelematico indirizzoTelematicoDest = new DocsPaInteropSemplificata.WRInteropSemp.IndirizzoTelematico();
        //    indirizzoTelematicoDest.tipo = DocsPaInteropSemplificata.WRInteropSemp.IndirizzoTelematicoTipo.smtp;
        //    indirizzoTelematicoDest.Value = mailDest;
        //    destinazione.IndirizzoTelematico = indirizzoTelematicoDest;

        //    List<DocsPaInteropSemplificata.WRInteropSemp.Destinatario> destinatariList = new List<DocsPaInteropSemplificata.WRInteropSemp.Destinatario>();

        //    for (int i = 0; i < destinatari.Count; i++)
        //    {
        //        DocsPaInteropSemplificata.WRInteropSemp.Destinatario destinatarioTemp = (DocsPaInteropSemplificata.WRInteropSemp.Destinatario)getDestinatarioPerSegnaturaObject((DocsPaVO.utente.Corrispondente)destinatari[i]);
        //        destinatariList.Add(destinatarioTemp);
        //    }

        //    destinazione.Destinatario = destinatariList.ToArray();

        //    destinazioneList.Add(destinazione);
        //    retValue.Intestazione.Destinazione = destinazioneList.ToArray();
        //    #endregion Intestazione --> Destinazione

        //    #region Intestazione --> Oggetto
        //    retValue.Intestazione.Oggetto = schedaDoc.oggetto.descrizione;
        //    #endregion Intestazione --> Oggetto

        //    #endregion Intestazione

        //    #region Riferimenti
        //    //I contesti procedurali saranno due, il primo creato per mantenere la compatibilità con la vecchia versione dei CC
        //    //il secondo è quello ufficiale

        //    if (DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableRiferimentiMittente())
        //    {
        //        //I riferimenti possono essere 0 o 1
        //        //DocsPaInteropSemplificata.WRInteropSemp.Riferimenti riferimenti = new DocsPaInteropSemplificata.WRInteropSemp.Riferimenti();

        //        //Primo Contesto Procedurale
        //        DocsPaInteropSemplificata.WRInteropSemp.ContestoProcedurale contestoProceduraleUno = new DocsPaInteropSemplificata.WRInteropSemp.ContestoProcedurale();

        //        contestoProceduraleUno.CodiceAmministrazione = reg.codAmministrazione;
        //        contestoProceduraleUno.CodiceAOO = reg.codRegistro;

        //        if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario))
        //            contestoProceduraleUno.Identificativo = schedaDoc.protocolloTitolario;
        //        else
        //            contestoProceduraleUno.Identificativo = schedaDoc.riferimentoMittente;

        //        contestoProceduraleUno.TipoContestoProcedurale = "Protocollo Arma";
        //        contestoProceduraleUno.Oggetto = "Pratica nuova";

        //        DocsPaInteropSemplificata.WRInteropSemp.ContestoProcedurale contestoProceduraleDue = new DocsPaInteropSemplificata.WRInteropSemp.ContestoProcedurale();

        //        contestoProceduraleDue.CodiceAmministrazione = reg.codAmministrazione;
        //        contestoProceduraleDue.CodiceAOO = reg.codRegistro;

        //        DocsPaVO.amministrazione.InfoAmministrazione infoAmm = Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(reg.idAmministrazione);

        //        //Se il protocollo titoalario esiste viene trasmesso con l'aggiunta del riferimento mittente che sarebbe il protocollo titolario epurato del sottonumero
        //        //altrimenti viene trasmesso solo il riferimento mittente
        //        if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario))
        //            contestoProceduraleDue.Identificativo = schedaDoc.protocolloTitolario + "$" + schedaDoc.riferimentoMittente;
        //        else
        //            contestoProceduraleDue.Identificativo = schedaDoc.riferimentoMittente;

        //        contestoProceduraleDue.TipoContestoProcedurale = "Codice Classifica";
        //        contestoProceduraleDue.Oggetto = "Classificazione";

        //        object[] contestoProceduraleArray = new object[2];
        //        //riferimenti.Items = contestoProceduraleArray;

        //        contestoProceduraleArray[0] = (DocsPaInteropSemplificata.WRInteropSemp.ContestoProcedurale)contestoProceduraleUno;
        //        contestoProceduraleArray[1] = (DocsPaInteropSemplificata.WRInteropSemp.ContestoProcedurale)contestoProceduraleDue;

        //        retValue.Riferimenti = contestoProceduraleArray;

        //        //object[] rifArray = new object[1];
        //        //retValue.Riferimenti = rifArray;
        //        //retValue.Riferimenti[0] = (DocsPaInteropSemplificata.WRInteropSemp.Riferimenti)riferimenti;
        //    }
        //    #endregion Riferimenti

        //    #region Descrizione

        //    DocsPaInteropSemplificata.WRInteropSemp.Descrizione descrizione = new DocsPaInteropSemplificata.WRInteropSemp.Descrizione();
        //    retValue.Descrizione = descrizione;

        //    DocsPaInteropSemplificata.WRInteropSemp.Documento documento = new DocsPaInteropSemplificata.WRInteropSemp.Documento();

        //    string estensioneFile = getEstensione(getDocumentoPrincipale(schedaDoc).fileName);
        //    if (estensioneFile != "")
        //    {
        //        documento.nome = "Documento_principale." + estensioneFile;
        //    }
        //    else
        //    {
        //        //per ovviare al problema del documentale EtDoc (dovrà essere studiata meglio la soluzione)
        //        documento.nome = "empty.TXT" + estensioneFile;
        //    }

        //    documento.tipoRiferimento = DocsPaInteropSemplificata.WRInteropSemp.DocumentoTipoRiferimento.MIME;
        //    documento.Oggetto = schedaDoc.oggetto.descrizione;
        //    retValue.Descrizione.Item = documento;

        //    if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
        //    {
        //        //DocsPaInteropSemplificata.WRInteropSemp.Allegati allegati = new DocsPaInteropSemplificata.WRInteropSemp.Allegati();
        //        List<object> listAllegati = new List<object>();

        //        for (int i = 0; i < schedaDoc.allegati.Count; i++)
        //        {
        //            DocsPaInteropSemplificata.WRInteropSemp.Documento docAllegatoTemp = new DocsPaInteropSemplificata.WRInteropSemp.Documento();
        //            string fileExt = getEstensione(((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).fileName);
        //            if (fileExt != "")
        //            {
        //                docAllegatoTemp.nome = "Allegato_" + (i + 1).ToString() + "." + fileExt;
        //            }
        //            else
        //            {
        //                docAllegatoTemp.nome = "Allegato_" + (i + 1).ToString() + ".TXT";
        //            }
        //            docAllegatoTemp.tipoRiferimento = DocsPaInteropSemplificata.WRInteropSemp.DocumentoTipoRiferimento.MIME;

        //            if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione != null)
        //            {
        //                docAllegatoTemp.TitoloDocumento = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).descrizione;
        //            }

        //            if (((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).numeroPagine != 0)
        //            {
        //                docAllegatoTemp.NumeroPagine = ((DocsPaVO.documento.Allegato)schedaDoc.allegati[i]).numeroPagine.ToString();
        //            }

        //            listAllegati.Add((DocsPaInteropSemplificata.WRInteropSemp.Documento)docAllegatoTemp);
        //        }

        //        retValue.Descrizione.Allegati = listAllegati.ToArray();
        //    }

        //    logger.Debug(getDocumentoPrincipale(schedaDoc).fileName);
        //    logger.Debug("Estensione doc: " + getEstensione(getDocumentoPrincipale(schedaDoc).fileName));

        //    #endregion Descrizione

        //    return retValue;
        //}

        //private static DocsPaInteropSemplificata.WRInteropSemp.Mittente getMittentePerSegnaturaObject(DocsPaVO.utente.Corrispondente corr)
        //{
        //    DocsPaInteropSemplificata.WRInteropSemp.Mittente retValue = new DocsPaInteropSemplificata.WRInteropSemp.Mittente();

        //    DocsPaInteropSemplificata.WRInteropSemp.Amministrazione amministrazione = new DocsPaInteropSemplificata.WRInteropSemp.Amministrazione();
        //    retValue.Amministrazione = amministrazione;

        //    System.Collections.ArrayList nomiUO = new System.Collections.ArrayList();
        //    DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
        //    DocsPaVO.utente.Ruolo ruolo = null;
        //    DocsPaVO.utente.Utente utente = null;

        //    if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
        //    {
        //        utente = (DocsPaVO.utente.Utente)corr;

        //        if (utente.ruoli != null && utente.ruoli.Count > 0)
        //        {
        //            ruolo = (DocsPaVO.utente.Ruolo)utente.ruoli[0];
        //            uo = ruolo.uo;
        //        }
        //        else
        //        {
        //            //					logger.addMessage("Utente sciolto");
        //            logger.Debug("Utente sciolto");
        //            uo = null;
        //        }
        //    }

        //    if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
        //    {
        //        ruolo = (DocsPaVO.utente.Ruolo)corr;
        //        uo = ruolo.uo;
        //    }

        //    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
        //    {
        //        uo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
        //    }

        //    string indPostString = null;

        //    if (uo != null)
        //    {
        //        indPostString = uo.indirizzo;
        //    }

        //    while (uo != null)
        //    {
        //        if (!string.IsNullOrEmpty(uo.descrizione))
        //            nomiUO.Add(uo.descrizione);

        //        if (uo.parent != null)
        //        {
        //            uo = uo.parent;
        //            //perchè la uo ha solo l'id, così estraggo gli altri dati.
        //            uo = (DocsPaVO.utente.UnitaOrganizzativa)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(uo.systemId);
        //        }
        //        else
        //            break;
        //    }

        //    if (nomiUO.Count > 0)
        //    {
        //        retValue.Amministrazione.Denominazione = (string)nomiUO[nomiUO.Count - 1];
        //    }
        //    else
        //    {
        //        retValue.Amministrazione.Denominazione = "Non specificato";
        //    }

        //    DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa rootUO = null;
        //    DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa rootUOTemp = null;
        //    List<object> amministrazioneItems = new List<object>();

        //    for (int i = nomiUO.Count - 1; i > -1; i--)
        //    {
        //        if (i == nomiUO.Count - 1)
        //        {
        //            rootUO = new DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa();
        //            rootUO.Denominazione = (string)nomiUO[i];
        //            rootUOTemp = rootUO;
        //        }
        //        else
        //        {
        //            DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa childTemp = new DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa();
        //            childTemp.Denominazione = (string)nomiUO[i];

        //            List<object> uoItems = new List<object>();

        //            if (i == 0)
        //            {
        //                List<object> itemsChildTemp = new List<object>();
        //                //ruolo e utente
        //                if (ruolo != null)
        //                {
        //                    DocsPaInteropSemplificata.WRInteropSemp.Ruolo ruoloTemp = new DocsPaInteropSemplificata.WRInteropSemp.Ruolo();
        //                    ruoloTemp.Denominazione = ruolo.descrizione;
        //                    //amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.Ruolo)ruoloTemp);


        //                    if (utente != null)
        //                    {
        //                        DocsPaInteropSemplificata.WRInteropSemp.Persona persona = new DocsPaInteropSemplificata.WRInteropSemp.Persona();

        //                        List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType> personaChiaviItems = new List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType>();
        //                        List<String> personaValoriItems = new List<string>();
        //                        personaChiaviItems.Add(DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType.Denominazione);
        //                        personaValoriItems.Add(utente.descrizione);

        //                        persona.ItemsElementName = personaChiaviItems.ToArray();
        //                        persona.Items = personaValoriItems.ToArray();

        //                        ruoloTemp.Persona = persona;
        //                        //amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.Persona)persona);
        //                    }
        //                    itemsChildTemp.Add((DocsPaInteropSemplificata.WRInteropSemp.Ruolo)ruoloTemp);
        //                }

        //                if (indPostString != null)
        //                {
        //                    DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale indirizzoPostaleTemp = new DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale();

        //                    List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1> indirizzoPostaleChiaviItems = new List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1>();
        //                    List<object> indirizzoPostaleValoriItems = new List<object>();
        //                    indirizzoPostaleChiaviItems.Add(DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1.Denominazione);
        //                    indirizzoPostaleValoriItems.Add(indPostString);

        //                    indirizzoPostaleTemp.ItemsElementName = indirizzoPostaleChiaviItems.ToArray();
        //                    indirizzoPostaleTemp.Items = indirizzoPostaleValoriItems.ToArray();

        //                    //amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale)indirizzoPostaleTemp);
        //                    itemsChildTemp.Add((DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale)indirizzoPostaleTemp);
        //                }
        //                else
        //                {
        //                    DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale indirizzoPostaleTemp = new DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale();

        //                    List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1> indirizzoPostaleChiaviItems = new List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1>();
        //                    List<object> indirizzoPostaleValoriItems = new List<object>();
        //                    indirizzoPostaleChiaviItems.Add(DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1.Denominazione);
        //                    indirizzoPostaleValoriItems.Add("Non specificato");

        //                    indirizzoPostaleTemp.ItemsElementName = indirizzoPostaleChiaviItems.ToArray();
        //                    indirizzoPostaleTemp.Items = indirizzoPostaleValoriItems.ToArray();

        //                    //amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale)indirizzoPostaleTemp);
        //                    itemsChildTemp.Add((DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale)indirizzoPostaleTemp);
        //                }
        //                childTemp.Items = itemsChildTemp.ToArray();
        //            }
        //            uoItems.Add((DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa)childTemp);
        //            rootUOTemp.Items = uoItems.ToArray();
        //            rootUOTemp = childTemp;
        //        }
        //    }

        //    if (rootUO != null)
        //    {
        //        amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa)rootUO);
        //    }

        //    retValue.Amministrazione.Items = amministrazioneItems.ToArray();

        //    logger.Debug("Indirizzo postale inserito");

        //    return retValue;

        //}

        //private static DocsPaInteropSemplificata.WRInteropSemp.Destinatario getDestinatarioPerSegnaturaObject(DocsPaVO.utente.Corrispondente corr)
        //{
        //    DocsPaInteropSemplificata.WRInteropSemp.Destinatario retValue = new DocsPaInteropSemplificata.WRInteropSemp.Destinatario();

        //    DocsPaInteropSemplificata.WRInteropSemp.Amministrazione amministrazione = new DocsPaInteropSemplificata.WRInteropSemp.Amministrazione();


        //    System.Collections.ArrayList nomiUO = new System.Collections.ArrayList();
        //    DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
        //    DocsPaVO.utente.Ruolo ruolo = null;
        //    DocsPaVO.utente.Utente utente = null;

        //    if (corr.GetType() == typeof(DocsPaVO.utente.Utente))
        //    {
        //        utente = (DocsPaVO.utente.Utente)corr;

        //        if (utente.ruoli != null && utente.ruoli.Count > 0)
        //        {
        //            ruolo = (DocsPaVO.utente.Ruolo)utente.ruoli[0];
        //            uo = ruolo.uo;
        //        }
        //        else
        //        {
        //            //					logger.addMessage("Utente sciolto");
        //            logger.Debug("Utente sciolto");
        //            uo = null;
        //        }
        //    }

        //    if (corr.GetType() == typeof(DocsPaVO.utente.Ruolo))
        //    {
        //        ruolo = (DocsPaVO.utente.Ruolo)corr;
        //        uo = ruolo.uo;
        //    }

        //    if (corr.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa))
        //    {
        //        uo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
        //    }

        //    string indPostString = null;

        //    if (uo != null)
        //    {
        //        indPostString = uo.indirizzo;
        //    }

        //    while (uo != null)
        //    {
        //        if (!string.IsNullOrEmpty(uo.descrizione))
        //            nomiUO.Add(uo.descrizione);

        //        if (uo.parent != null)
        //        {
        //            uo = uo.parent;
        //            //perchè la uo ha solo l'id, così estraggo gli altri dati.
        //            uo = (DocsPaVO.utente.UnitaOrganizzativa)BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(uo.systemId);
        //        }
        //        else
        //            break;
        //    }

        //    if (nomiUO.Count > 0)
        //    {
        //        amministrazione.Denominazione = (string)nomiUO[nomiUO.Count - 1];
        //    }
        //    else
        //    {
        //        amministrazione.Denominazione = "Non specificato";
        //    }

        //    DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa rootUO = null;
        //    DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa rootUOTemp = null;
        //    List<object> amministrazioneItems = new List<object>();

        //    for (int i = nomiUO.Count - 1; i > -1; i--)
        //    {
        //        if (i == nomiUO.Count - 1)
        //        {
        //            rootUO = new DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa();
        //            rootUO.Denominazione = (string)nomiUO[i];
        //            rootUOTemp = rootUO;
        //        }
        //        else
        //        {
        //            DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa childTemp = new DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa();
        //            childTemp.Denominazione = (string)nomiUO[i];

        //            List<object> uoItems = new List<object>();
        //            uoItems.Add((DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa)childTemp);

        //            rootUOTemp.Items = uoItems.ToArray();
        //            rootUOTemp = childTemp;
        //        }
        //    }

        //    if (rootUO != null)
        //    {
        //        amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.UnitaOrganizzativa)rootUO);
        //    }

        //    //ruolo e utente
        //    if (ruolo != null)
        //    {
        //        DocsPaInteropSemplificata.WRInteropSemp.Ruolo ruoloTemp = new DocsPaInteropSemplificata.WRInteropSemp.Ruolo();
        //        ruoloTemp.Denominazione = ruolo.descrizione;
        //        amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.Ruolo)ruoloTemp);
        //    }

        //    if (utente != null)
        //    {
        //        DocsPaInteropSemplificata.WRInteropSemp.Persona persona = new DocsPaInteropSemplificata.WRInteropSemp.Persona();

        //        List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType> personaChiaviItems = new List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType>();
        //        List<String> personaValoriItems = new List<string>();
        //        personaChiaviItems.Add(DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType.Denominazione);
        //        personaValoriItems.Add(utente.descrizione);

        //        persona.ItemsElementName = personaChiaviItems.ToArray();
        //        persona.Items = personaValoriItems.ToArray();

        //        amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.Persona)persona);
        //    }

        //    if (indPostString != null)
        //    {
        //        DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale indirizzoPostaleTemp = new DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale();

        //        List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1> indirizzoPostaleChiaviItems = new List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1>();
        //        List<object> indirizzoPostaleValoriItems = new List<object>();
        //        indirizzoPostaleChiaviItems.Add(DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1.Denominazione);
        //        indirizzoPostaleValoriItems.Add(indPostString);

        //        indirizzoPostaleTemp.ItemsElementName = indirizzoPostaleChiaviItems.ToArray();
        //        indirizzoPostaleTemp.Items = indirizzoPostaleValoriItems.ToArray();

        //        amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale)indirizzoPostaleTemp);
        //    }
        //    else
        //    {
        //        DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale indirizzoPostaleTemp = new DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale();

        //        List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1> indirizzoPostaleChiaviItems = new List<DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1>();
        //        List<object> indirizzoPostaleValoriItems = new List<object>();
        //        indirizzoPostaleChiaviItems.Add(DocsPaInteropSemplificata.WRInteropSemp.ItemsChoiceType1.Denominazione);
        //        indirizzoPostaleValoriItems.Add("Non specificato");

        //        indirizzoPostaleTemp.ItemsElementName = indirizzoPostaleChiaviItems.ToArray();
        //        indirizzoPostaleTemp.Items = indirizzoPostaleValoriItems.ToArray();

        //        amministrazioneItems.Add((DocsPaInteropSemplificata.WRInteropSemp.IndirizzoPostale)indirizzoPostaleTemp);
        //    }

        //    amministrazione.Items = amministrazioneItems.ToArray();

        //    retValue.Items = new object[1];
        //    retValue.Items[0] = (DocsPaInteropSemplificata.WRInteropSemp.Amministrazione)amministrazione;

        //    logger.Debug("Indirizzo postale inserito");

        //    return retValue;
        //}

        #region Interoperabilità semplificata

        /// <summary>
        /// Metodo per la spedizione di un documento per interoperabilità semplificata
        /// </summary>
        /// <param name="schedaDocumento">Documento da spedire</param>
        /// <param name="receivers">Destinatari della spedizione</param>
        /// <param name="infoUtente">Informazioni sull'utente che sta effettuando la spedizione</param>
        /// <param name="receiverUrl">Url dei destinatari della spedizione</param>
        /// <returns>Esito della spedizione</returns>
        private static SendDocumentResponse.SendDocumentMailResponse SendDocumentSimpleInterop(DocsPaVO.documento.SchedaDocumento schedaDocumento, List<DocsPaVO.Spedizione.DestinatarioEsterno> receivers, DocsPaVO.utente.InfoUtente infoUtente, String receiverUrl)
        {
            bool sendSucceded = true;
            String errorMessage = String.Empty;

            // Costruzione della lista dei destinatari
            List<Corrispondente> corrs = new List<Corrispondente>();
            foreach (DocsPaVO.Spedizione.DestinatarioEsterno c in receivers)
                corrs.AddRange(c.DatiDestinatari);

            // Risultato della spedizione
            SendDocumentResponse.SendDocumentMailResponse retValue = new DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse(receiverUrl);

            InteroperabilityMessage interoperabilityMessage = null;
            try
            {
                // Costruzione dell'oggetto con le informazioni sulla spedizione
                interoperabilityMessage = new InteroperabilitaSemplificataHelper().CreateInteroperabilityMessage(schedaDocumento, infoUtente, corrs.ToArray());

                // Invio del messaggio al sistema di interoperabilità
                InteroperabilityController interoperabilityController = new InteroperabilityController();
                IAsyncResult result = interoperabilityController.SubmitInteroperabilityMessage(interoperabilityMessage, receiverUrl, OnAnalyzeInteroperabilityMessageCompleted);
                sendSucceded = true;
            }
            catch (SenderNotInteroperableException e)
            {
                sendSucceded = false;
                errorMessage = "Per poter procedere con la spedizione è necessario che il mittente sia configurato correttamente";
                logger.Error(errorMessage);
            }
            catch (Exception e)
            {
                sendSucceded = false;
                errorMessage = "Si è verificato un errore non identificato durante la spedizione";
                logger.Error(errorMessage);
            }


            // Impostazione dell'esito della spedizione e dell'eventuale errore
            retValue.SendSucceded = sendSucceded;
            retValue.SendErrorMessage = errorMessage;

            // Per poter fare in modo che il sistema capisca che il documento è già stato spedito, viene valorizzato
            // il campo Email dei corrispondenti con l'URL del destinatario
            foreach (Corrispondente destinatario in corrs)
            {
                destinatario.email = receiverUrl;
                retValue.Destinatari.Add(destinatario);
            }
            // Restituzione del risultato
            return retValue;
        }

        /// <summary>
        /// Metodo richiamato al termine dell'esecuzione dell'elaborazione della richiesta di interoperabilità
        /// </summary>
        private static void OnAnalyzeInteroperabilityMessageCompleted(IAsyncResult ar)
        {
            // L'AsyncState contiene un array di oggetti di cui il primo è il client (viene utilizzato per
            // invocare l'end sull'operazione) ed il secondo è un oggetto che contiene le informazioni sugli
            // argomenti passati al servizio
            object[] convertedParam = ar.AsyncState as object[];
            InteroperabilityMessage convertedRequest = (InteroperabilityMessage)convertedParam[1];

            try
            {
                // Completamento dell'invocazione e prelevamento dell'id del messaggio
                ElaborateNewInteroperabilityMessageResponse response = ((Interoperability.Service.Library.InteroperabilityService.InteroperabilityServiceClient)convertedParam[0]).EndElaborateNewInteroperabilityMessage(ar);

                // Associazione della ricevuta di errore di consegna per tutti i corrispondenti per cui si è verificato
                // un problema
                response.ElaborateNewInteroperabilityMessageResult.SingleRequestErrors.ForEach(request =>
                    request.Receivers.ForEach(receiver =>
                        {
                            SimplifiedInteroperabilityMessageUndeliveredProofManager.GenerateProof(
                                    convertedRequest,
                                    response.ElaborateNewInteroperabilityMessageResult.MessageId,
                                    request.ErrorMessage,
                                    receiver);

                            // Inserimento di un item nel centro notifiche
                            // Recupero dell'id dell'utente cui rendere visibile l'item
                            String userId = BusinessLogic.Utenti.UserManager.GetInternalCorrAttributeByCorrCode(
                                convertedRequest.Sender.UserId,
                                DocsPaDB.Query_DocsPAWS.Utenti.CorrAttribute.id_people,
                                convertedRequest.Sender.AdministrationId);
                            
                            if(NotificationCenterHelper.IsEnabled(convertedRequest.Sender.AdministrationId))
                                NotificationCenterHelper.InsertItem(
                                    receiver.Code,
                                    request.ErrorMessage,
                                    "Notifica di mancata consegna",
                                    Convert.ToInt32(userId),
                                    "IS",
                                    Int32.Parse(convertedRequest.MainDocument.DocumentNumber),
                                    Int32.Parse(convertedRequest.Record.RecordNumber),
                                    convertedRequest.Record.AdministrationCode);


                            using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata dbInterop = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
                            {
                                string authorId = string.Empty;
                                string creatorRole = string.Empty;
                                dbInterop.LoadDataForDeliveredProof(convertedRequest.MainDocument.DocumentNumber, out authorId, out creatorRole);

                                // Recupero il ruolo che ha effettuato l'ultima spedizione IS, dallo storico delle spedizioni. 
                               ArrayList listHistorySendDoc = SpedizioneManager.GetElementiStoricoSpedizione(convertedRequest.MainDocument.DocumentNumber);
                               if (listHistorySendDoc != null && listHistorySendDoc.Count > 0)
                               {
                                   Object lastSendIs = (from record in listHistorySendDoc.ToArray() 
                                                            where ((ElStoricoSpedizioni)record).Mail_mittente.Equals("N.A.") 
                                                            select record).ToList().OrderBy(z => ((ElStoricoSpedizioni)z).Id).LastOrDefault();

                                   Ruolo role = UserManager.getRuoloByIdGruppo(((ElStoricoSpedizioni)lastSendIs).IdGroupSender);
                                   Utente user = UserManager.getUtenteByCodice(convertedRequest.Sender.UserId,
                                                        AmministraManager.AmmGetInfoAmmCorrente(convertedRequest.Sender.AdministrationId).Codice);
                                   user.dst = UserManager.getSuperUserAuthenticationToken();
                                   InfoUtente userInfo = UserManager.GetInfoUtente(user, role);
                                   // LOG per documento
                                   string desc = "Ricevuta di mancata consegna IS: " + request.ErrorMessage + ".<br/>Destinatario spedizione: " +
                                       receiver.Code;
                                   BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo,
                                       user.idAmministrazione, "NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY",
                                       convertedRequest.MainDocument.DocumentNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                       (userInfo != null && userInfo.delegato != null ? userInfo.delegato : null), "1");
                               }
                            }
                        
                        }));

                // Associazione della ricevuta di avvenuta consegna per tutti quelli a cui non è già stata associata una
                // ricevuta di mancata consegna
                List<ReceiverInfo> receiverWithErrors = new List<ReceiverInfo>();
                foreach (var request in response.ElaborateNewInteroperabilityMessageResult.SingleRequestErrors)
                    receiverWithErrors.AddRange(request.Receivers);

                convertedRequest.Receivers.ForEach(receiver =>
                    {
                        if (!receiverWithErrors.Contains(receiver))
                        {
                            SimplifiedInteroperabilityMessageDeliveredProofManager.GenerateProof(
                                    convertedRequest,
                                    response.ElaborateNewInteroperabilityMessageResult.MessageId,
                                    receiver,
                                    response.ElaborateNewInteroperabilityMessageResult.DocumentDelivered);
                        }
                    });

            }
            catch (EndpointNotFoundException endpointNotFoundException)
            {
                logger.ErrorFormat("Interoperabilita semplificata. Impossibile contattare il destinatario: {0}", endpointNotFoundException.Message);
                SimplifiedInteroperabilityLogAndRegistryManager.InsertItemInLog(convertedRequest.MainDocument.DocumentNumber,
                    true,
                    String.Format("Impossibile contattare il destinatario: {0}", endpointNotFoundException.Message));
                
                // Associazione di una ricevuta di mancata consegna al documento per tutti i destinatari
                convertedRequest.Receivers.ForEach(receiver =>
                {
                    SimplifiedInteroperabilityMessageUndeliveredProofManager.GenerateProof(
                        convertedRequest,
                        Guid.NewGuid().ToString(),
                        "Indirizzo destinatario non raggiungibile",
                        receiver);


                    // Inserimento di un item nel centro notifiche
                    // Recupero dell'id dell'utente cui rendere visibile l'item
                    String userId = BusinessLogic.Utenti.UserManager.GetInternalCorrAttributeByCorrCode(
                        convertedRequest.Sender.UserId,
                        DocsPaDB.Query_DocsPAWS.Utenti.CorrAttribute.id_people,
                        convertedRequest.Sender.AdministrationId);

                    if (NotificationCenterHelper.IsEnabled(convertedRequest.Sender.AdministrationId))
                        NotificationCenterHelper.InsertItem(
                            receiver.Code,
                            "Indirizzo destinatatario non raggiungibile",
                            "Notifica di eccezione",
                            Convert.ToInt32(userId),
                            "IS",
                            Int32.Parse(convertedRequest.MainDocument.DocumentNumber),
                            Int32.Parse(convertedRequest.Record.RecordNumber),
                            convertedRequest.Record.AdministrationCode);

                    using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata dbInterop = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
                    {
                        string authorId = string.Empty;
                        string creatorRole = string.Empty;
                        dbInterop.LoadDataForDeliveredProof(convertedRequest.MainDocument.DocumentNumber, out authorId, out creatorRole);

                        // Recupero lo storico delle spedizioni del documento 
                        ArrayList listHistorySendDoc = SpedizioneManager.GetElementiStoricoSpedizione(convertedRequest.MainDocument.DocumentNumber);
                        if (listHistorySendDoc != null && listHistorySendDoc.Count > 0)
                        {
                            Object lastSendIs = (from record in listHistorySendDoc.ToArray()
                                                 where ((ElStoricoSpedizioni)record).Mail_mittente.Equals("N.A.")
                                                 select record).ToList().OrderBy(z => ((ElStoricoSpedizioni)z).Id).LastOrDefault();

                            Ruolo role = UserManager.getRuoloByIdGruppo(((ElStoricoSpedizioni)lastSendIs).IdGroupSender);
                            Utente user = UserManager.getUtenteByCodice(convertedRequest.Sender.UserId,
                                            AmministraManager.AmmGetInfoAmmCorrente(convertedRequest.Sender.AdministrationId).Codice);
                            user.dst = UserManager.getSuperUserAuthenticationToken();
                            InfoUtente userInfo = UserManager.GetInfoUtente(user, role);
                            // LOG per documento
                            string desc = "Ricevuta di mancata consegna IS: Indirizzo destinatatario non raggiungibile.<br/>Destinatario spedizione: " +
                                receiver.Code;
                            BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo,
                                user.idAmministrazione, "NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY",
                                convertedRequest.MainDocument.DocumentNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                (userInfo != null && userInfo.delegato != null ? userInfo.delegato : null), "1");
                        }
                    }
                });

            }
            catch (Exception e)
            {
                logger.Error("Errore non identificato durante l'analisi della richiesta di interoperabilità", e);

                // Associazione di una ricevuta di mancata consegna al documento per tutti i destinatari
                convertedRequest.Receivers.ForEach(receiver =>
                {
                    SimplifiedInteroperabilityMessageUndeliveredProofManager.GenerateProof(
                        convertedRequest,
                        Guid.NewGuid().ToString(),
                        "Errore non identificato",
                        receiver);

                    // Inserimento di un item nel centro notifiche
                    // Recupero dell'id dell'utente cui rendere visibile l'item
                    String userId = BusinessLogic.Utenti.UserManager.GetInternalCorrAttributeByCorrCode(
                        convertedRequest.Sender.UserId,
                        DocsPaDB.Query_DocsPAWS.Utenti.CorrAttribute.id_people,
                        convertedRequest.Sender.AdministrationId);

                    if (NotificationCenterHelper.IsEnabled(convertedRequest.Sender.AdministrationId))
                        NotificationCenterHelper.InsertItem(
                            receiver.Code,
                            "Errore non identificato",
                            "Notifica di eccezione",
                            Convert.ToInt32(userId),
                            "IS",
                            Int32.Parse(convertedRequest.MainDocument.DocumentNumber),
                            Int32.Parse(convertedRequest.Record.RecordNumber),
                            convertedRequest.Record.AdministrationCode);

                    using (DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata dbInterop = new DocsPaDB.Query_DocsPAWS.InteroperabilitaSemplificata())
                    {
                        string authorId = string.Empty;
                        string creatorRole = string.Empty;
                        dbInterop.LoadDataForDeliveredProof(convertedRequest.MainDocument.DocumentNumber, out authorId, out creatorRole);

                        // Recupero lo storico delle spedizioni del documento 
                        ArrayList listHistorySendDoc = SpedizioneManager.GetElementiStoricoSpedizione(convertedRequest.MainDocument.DocumentNumber);
                        if (listHistorySendDoc != null && listHistorySendDoc.Count > 0)
                        {
                            Object lastSendIs = (from record in listHistorySendDoc.ToArray()
                                                 where ((ElStoricoSpedizioni)record).Mail_mittente.Equals("N.A.")
                                                 select record).ToList().OrderBy(z => ((ElStoricoSpedizioni)z).Id).LastOrDefault();

                            Ruolo role = UserManager.getRuoloByIdGruppo(((ElStoricoSpedizioni)lastSendIs).IdGroupSender);
                            Utente user = UserManager.getUtenteByCodice(convertedRequest.Sender.UserId,
                                    AmministraManager.AmmGetInfoAmmCorrente(convertedRequest.Sender.AdministrationId).Codice);
                            user.dst = UserManager.getSuperUserAuthenticationToken();
                            InfoUtente userInfo = UserManager.GetInfoUtente(user, role);
                            // LOG per documento
                            string desc = "Ricevuta di mancata consegna IS: Errore non identificato.<br/>Destinatario spedizione: " +
                                receiver.Code;
                            BusinessLogic.UserLog.UserLog.WriteLog(user.userId, user.idPeople, role.idGruppo,
                                user.idAmministrazione, "NO_DELIVERY_SEND_SIMPLIFIED_INTEROPERABILITY",
                                convertedRequest.MainDocument.DocumentNumber, desc, DocsPaVO.Logger.CodAzione.Esito.OK,
                                (userInfo != null && userInfo.delegato != null ? userInfo.delegato : null), "1");
                        }
                    }

                });
            }
            
        }

        #endregion
    }
}
