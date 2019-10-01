using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.DatiCert;
using log4net;
using System.Data;

namespace BusinessLogic.interoperabilita
{
    public sealed class InteroperabilitaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteroperabilitaManager));
        private static InteroperabilitaManager instance = null;
        private static Dictionary<string, TipoNotifica> _intance = null;


        public static bool inserimentoTipoNotifica(string codiceNotifica)
        {
            bool retval = false;

            DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
            TipoNotifica tipoNotifica = new TipoNotifica();
            tipoNotifica.codiceNotifica = codiceNotifica;
            tipoNotifica.descrizioneNotifica = "email di " + codiceNotifica;

            try
            {
                retval = daticertDB.inserisciTipoNotifica(tipoNotifica);
            }
            catch (Exception e)
            {
                logger.Error("errore durante l'inserimento del tipo di notifica - errore: " + e.Message);
            }

            return retval;
        }

        public static TipoNotifica ricercaTipoNotificaByCodice(string codiceNotifica)
        {
            TipoNotifica retval = null;

            DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
            try
            {
                retval = daticertDB.ricercaTipoNotificheByCodice(codiceNotifica);
            }
            catch (Exception e)
            {
                logger.Error("errore durante l'inserimento del tipo di notifica - errore: " + e.Message);
            }

            return retval;
        }

        public static bool deleteNotifica(string docnumber)
        {
            bool retval = false;

            DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
            try
            {
                if(!string.IsNullOrEmpty(docnumber))
                    retval = daticertDB.deleteNotifica(docnumber);
            }
            catch (Exception e)
            {
                logger.Error("errore durante l'inserimento del tipo di notifica - errore: " + e.Message);
            }

            return retval;
        }

        /// <summary>
        /// inserimento nella tabella notifica indicando il tipo di notifica e il destinatario da inserire
        /// </summary>
        /// <param name="daticert"></param>
        /// <param name="systemIdTipoNotifica"></param>
        /// <param name="indiceDestinatari"></param>
        /// <returns></returns>
        public static bool inserimentoNotifica(Daticert daticert, string systemIdTipoNotifica, int indiceDestinatari, string IdAllegato)
        {
            bool retval = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
                if (daticert == null)
                    return retval;

                Notifica notifica = new Notifica();
                notifica = (Notifica)daticert;

                #region controllo sui dati immessi

                if (!string.IsNullOrEmpty(systemIdTipoNotifica))
                    notifica.idTipoNotifica = systemIdTipoNotifica;
                else
                    return retval;


                if (daticert.destinatarioLst.Length > 0 &&
                    daticert.tipoDestinatarioLst.Length > 0)
                {
                    notifica.destinatario = daticert.destinatarioLst[indiceDestinatari];
                    notifica.tipoDestinatario = daticert.tipoDestinatarioLst[indiceDestinatari];
                }
                else
                    return retval;

                if (!string.IsNullOrEmpty(daticert.giorno) &&
                    !string.IsNullOrEmpty(daticert.ora))
                    notifica.data_ora = daticert.giorno + " " + daticert.ora;
                else
                    return retval;

                if (daticert.risposteLst != null &&
                   daticert.risposteLst.Length > 0)
                    notifica.risposte = daticert.risposteLst[0];
                else
                    return retval;

                #endregion
                //inseriemnto dei dati
                retval = daticertDB.inserisciNotifica(notifica,IdAllegato);
                //PEC 4 Modifica Maschera Caratteri
                if (retval)
                {

                    bool aggiornaMaschera = AggiornaStatusMask(notifica);
                }

            }
            catch (Exception e)
            {
                logger.Error("errore durante l'inserimento del tipo di notifica - errore: " + e.Message);
            }

            return retval;
        }

        /// <summary>
        /// Inseriemente nella tabella notifica
        /// </summary>
        /// <param name="notifica"></param>
        /// <returns></returns>
        public static bool inserimentoNotifica(Notifica notifica, string IdAllegato)
        {
            bool retval = false;

            try
            {
                DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
                retval = daticertDB.inserisciNotifica(notifica, IdAllegato);
                // PEC 4 Modifica Maschera Caratteri
                if (retval)
                {
                    bool aggiornaSMask = AggiornaStatusMask(notifica);
                }
            }
            catch (Exception e)
            {
                logger.Error("errore durante l'inserimento del tipo di notifica - errore: " + e.Message);
            }

            return retval;
        }

        public static bool updateNotificaRisposte(Daticert daticert, int indiceRisposte)
        {
            bool retval = false;

            DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
            Notifica notifica = new Notifica();
            notifica = (Notifica)daticert;

            notifica.risposte = daticert.risposteLst[indiceRisposte];

            try
            {
                retval = daticertDB.inserisciNotifica(notifica,null);
            }
            catch (Exception e)
            {
                logger.Error("errore durante l'inserimento del tipo di notifica - errore: " + e.Message);
            }

            return retval;
        }

        public static Notifica[] ricercaNotifiche(string docnumber)
        {
            Notifica[] notifica = null;

            try
            {
                DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
                notifica = daticertDB.ricercaNotifica(docnumber);
                //BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaNotifiche(docnumber);
            }
            catch (Exception e)
            {
                logger.Error("errore nella ricerca delle notifiche" + e.Message);
                notifica = new Notifica[0];
            }

            return notifica;
        }

        public static Notifica[] ricercaNotificheFiltrate(DocsPaVO.filtri.FiltroRicerca [] filtri)
        {
            Notifica[] notifica = null;

            try
            {
                DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
                notifica = daticertDB.ricercaNotificheFiltrate(filtri);
                //BusinessLogic.interoperabilita.InteroperabilitaManager.ricercaNotifiche(docnumber);
            }
            catch (Exception e)
            {
                logger.Error("errore nella ricerca delle notifiche" + e.Message);
                notifica = new Notifica[0];
            }

            return notifica;
        }
        
        /// <summary>
        /// restituisce il tipo della notifica ricercandolo per systemid
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        public static TipoNotifica ricercaTipoNotificaBySystemId(string systemId)
        {
            TipoNotifica retval = null;

            DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
            try
            {
                retval = daticertDB.ricercaTipoNotificheBySystemId(systemId);
            }
            catch (Exception e)
            {
                logger.Error("errore durante l'inserimento del tipo di notifica - errore: " + e.Message);
            }

            return retval;
        }


        //Inizializza il singleton
        public void initializeInstance(string systemId)
        {
            if (_intance == null)
            {
                // Creazione oggetto dictionary contenente i dati del tipoNotifiche
                _intance = new Dictionary<string, TipoNotifica>();
                GetData(systemId);
            }

            if (!_intance.ContainsKey(systemId))
            {
                lock (_intance)
                {
                    // Caricamento dei dati
                    GetData(systemId);
                }
            }
        }


        private void GetData(string systemId)
        {
            TipoNotifica data = null;

            // Caricamento etichette relative all'amministrazione richiesta
            data = ricercaTipoNotificaBySystemId(systemId);
            _intance.Add(systemId, data);
        }

        public static TipoNotifica GetInstance(string systemId)
        {
            if (instance == null)
            {
                instance = new InteroperabilitaManager();
                instance.initializeInstance(systemId);
            }
            if (instance != null && !_intance.ContainsKey(systemId))
            {
                lock (_intance)
                {
                    instance.initializeInstance(systemId);
                }
            }
            TipoNotifica tipologia = _intance[systemId];
            return tipologia;
        }

        public static bool SetInstance(TipoNotifica tiponotifica, string systemId)
        {
            bool success = false;
            TipoNotifica data = null;
            _intance.Remove(systemId);
            data = ricercaTipoNotificaBySystemId(systemId);
            if (data != null)
                success = true;
            _intance.Add(systemId, tiponotifica);
            return success;

        }

        public static DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta getTipoRicevutaPec(Interoperabilità.CMMsg mc)
        {

            DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.unknown ;

            if (mc.isPECAcceptNotify())                         //avvenuta accettazione
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Accept_Notify;
            else if (mc.isDeliveryStatusNotification())         //messaggi non ricevuti
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.Delivery_Status_Notification;
            else if (mc.isFromNonPEC())                         //messaggi non Pec
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.From_Non_PEC;
            else if (mc.isPECAlertVirus())                      //rilevazione virus
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Alert_Virus;
            else if (mc.isPECContainVirus())                    //non accettazione
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Contain_Virus;
            else if (mc.isPECDelivered())                       //Consengata
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered;
            else if (mc.isPECDeliveredNotify())                 //avvenuta consegna
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered_Notify;
            else if (mc.isPECDeliveredNotifyShort())            //avvenuta consegna
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered_Notify_Short;
            else if (mc.isPECError())                           //errore generico
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error;
            else if (mc.isPECErrorDeliveredNotifyByVirus())     //errore consegna
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error_Delivered_Notify_By_Virus;
            else if (mc.isPECErrorPreavvisoDeliveredNotify())  //preavviso errore consegna
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify;
            else if (mc.isPECNonAcceptNotify())                //non accettazione
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Non_Accept_Notify;
            else if (mc.isPECPresaInCarico())                  //presa in carico
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Presa_In_Carico;
            else if(mc.isPECErrorDeliveredNotify())
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Mancata_Consegna;
            else if (mc.isPEC())
                tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_NO_XRicevuta;


            return tipoRicevuta;

        }

        public static bool isRicevutaPec(Interoperabilità.CMMsg mc)
        {
            bool retval = false;
            DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta ricevuta = getTipoRicevutaPec(mc);
            if (ricevuta != DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.unknown &&
                ricevuta != DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_NO_XRicevuta)
                retval = true;

            /*
            if (mc.isPECAcceptNotify() ||
                                           mc.isPECDeliveredNotify() ||
                                           mc.isDeliveryStatusNotification() ||
                                           mc.isFromNonPEC() ||
                                           mc.isPECAcceptNotify() ||
                                           mc.isPECAlertVirus() ||
                                           mc.isPECContainVirus() ||
                                          // mc.isPECDelivered() ||
                                           mc.isPECDeliveredNotifyShort() ||
                                           mc.isPECError() ||
                                           mc.isPECErrorDeliveredNotifyByVirus() ||
                                           mc.isPECErrorPreavvisoDeliveredNotify() ||
                                           mc.isPECNonAcceptNotify() ||
                                           mc.isPECPresaInCarico())
            {
                retval = true;
            }
            */
            return retval;
        }

        /// <summary>
        /// data di ricezione della mail da parte del server ricevente
        /// </summary>
        /// <param name="mc"></param>
        /// <returns></returns>
        public static string DataRicezioneMail(Interoperabilità.CMMsg mc)
        {
            return mc.DateReceivedMail();
        }

        /// <summary>
        /// data di spedizone della mail al server di posta
        /// </summary>
        /// <param name="mc"></param>
        /// <returns></returns>
        public static string DataInvioMail(Interoperabilità.CMMsg mc)
        {
            return mc.DateSendMail();
        }


        /// <summary>
        /// verifica se uan ricevuta è già stata associata al documento
        /// </summary>
        /// <param name="daticert"></param>
        /// <param name="systemIdTipoNotifica"></param>
        /// <param name="indiceDestinatari"></param>
        /// <returns></returns>
        public static bool verificaPresenzaNotifica(Daticert daticert, string systemIdTipoNotifica)
        {
            bool retval = false;
            logger.Debug("Inserimento notifica");
            try
            {
                DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert daticertDB = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
                if (daticert == null)
                    return retval;

                Notifica notifica = new Notifica();
                notifica = (Notifica)daticert;

                #region controllo sui dati immessi

                if (!string.IsNullOrEmpty(systemIdTipoNotifica))
                    notifica.idTipoNotifica = systemIdTipoNotifica;
                else
                    return retval;

                logger.Debug("Controllo destinatari");

                logger.Debug("Controllo giorno");
                if (!string.IsNullOrEmpty(daticert.giorno) &&
                    !string.IsNullOrEmpty(daticert.ora))
                    notifica.data_ora = daticert.giorno + " " + daticert.ora;
                else
                    return retval;

                logger.Debug("Controllo risposte");
                if (daticert.risposteLst != null &&
                   daticert.risposteLst.Length > 0)
                    notifica.risposte = daticert.risposteLst[0];
                else
                    return retval;

                #endregion
                //inseriemnto dei dati
                retval = daticertDB.verificaPresenzaNotifica(notifica);

            }
            catch (Exception e)
            {
                logger.Error("errore durante l'inserimento del tipo di notifica - errore: " + e.Message);
            }

            return retval;
        }

        public static string getCheckInteropFromSysIdCorrGlob(string systemId)
        {
            string check = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                check = dbAmministrazione.getCheckInteropFromSysIdCorrGlob(systemId);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return check;
        }

        /// <summary>
        /// Return l'associazione tra un documento (protocollato o predisposto alla protocollazione) e il mailAddress utilizzati per 
        /// l'invio della conferma di ricezione e la notifica di annullamento al mittente
        /// Solo per doc ricevuti per interoperabilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static DataSet GetAssDocAddress(string docNumber)
        {
            DataSet ds = new DataSet();
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            try
            {
                return amm.GetAssDocAddress(docNumber);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return ds;
            }
        }
        /// <summary>
        /// Inserisce in db l'associazione tra un documento (protocollato o predisposto alla protocollazione) e il mailAddress utilizzati per 
        /// l'invio della conferma di ricezione e la notifica di annullamento al mittente
        /// Solo per doc ricevuti per interoperabilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static bool InsertAssDocAddress(string docNumber, string idRegistro, string mailAddress)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            try
            {
                return amm.InsertAssDocAddress(docNumber, idRegistro, mailAddress);
            }
            catch(Exception e) {
                logger.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Aggiorna l'associazione tra un documento (protocollato o predisposto alla protocollazione) e il mailAddress utilizzati per 
        /// l'invio della conferma di ricezione e la notifica di annullamento al mittente
        /// Solo per doc ricevuti per interoperabilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static bool UpdateAssDocAddress(string docNumber, string idRegistro, string mailAddress)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            try
            {
                return amm.UpdateAssDocAddress(docNumber, idRegistro, mailAddress);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }
        }

        /// <summary>
        /// Elimina dal db l'associazione tra il documento e il mailAddress utilizzati per l'invio della conferma di ricezione e la notifica 
        /// di annullamento al mittente
        /// Invocato solo dopo l'eventuale annullamento ed invio della notifica di annullamento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static bool DeleteAssDocAddress(string docNumber, string idRegistro)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            try
            {
                return amm.DeleteAssDocAddress(docNumber, idRegistro);
            }
            catch (Exception e)
            {
                logger.Error(e);
                return false;
            }
        }

        /// <summary>
        /// PEC 4 Modifica Maschera Caratteri
        /// Metodo di aggiornamento della status mask a partire dalla notifica.
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool AggiornaStatusMask(Notifica notifica)
        {
            bool retval = false;
            logger.Debug("Inserita la notifica, aggiorno la status-mask");
            try
            {
                // eliminareEccezione: quando l'esito è OK, elimino una possibile eccezione preesistente.
                bool eliminareEccezione = false;
                DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert interop = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
                TipoNotifica tipoNot = interop.ricercaTipoNotificheBySystemId(notifica.idTipoNotifica);
                DocsPaVO.StatoInvio.StatoInvio statoInvio = interop.getStatoInvioFromAddressAndProfile(notifica.destinatario, notifica.docnumber);
                if (statoInvio != null)
                {
                    string statusmask="";
                    if (!string.IsNullOrEmpty(statoInvio.statusMask))
                    {
                        statusmask = statoInvio.statusMask;
                        char[] status_c_mask = statusmask.ToCharArray();

                        if (tipoNot.codiceNotifica == "accettazione")
                        {
                            status_c_mask[1] = 'V';
                            if (statoInvio.tipoCanale == "MAIL")
                            {
                                if (notifica.tipoDestinatario == "certificato")
                                {
                                    if (status_c_mask[2] != 'V' || status_c_mask[2] == 'A' || status_c_mask[2] == 'N')
                                    {
                                        status_c_mask[2] = 'A';
                                    }
                                }
                                else
                                {
                                    status_c_mask[0] = 'V';
                                    eliminareEccezione = true;
                                }
                            }
                        }
                        else if (tipoNot.codiceNotifica == "non-accettazione")
                        {
                            status_c_mask[0] = 'X';
                            status_c_mask[1] = 'X';
                            status_c_mask[2] = 'N';
                            status_c_mask[3] = 'N';
                            status_c_mask[4] = 'N';
                            status_c_mask[5] = 'N';
                            status_c_mask[6] = 'N';
                        }
                        else if (tipoNot.codiceNotifica == "DSN" || tipoNot.codiceNotifica == "errore")
                        {
                            status_c_mask[0] = 'X';
                            status_c_mask[2] = 'X';
                            status_c_mask[3] = 'N';
                            status_c_mask[4] = 'N';
                            status_c_mask[5] = 'N';
                            status_c_mask[6] = 'V';
                        }
                        else if (tipoNot.codiceNotifica == "avvenuta-consegna")
                        {
                            status_c_mask[2] = 'V';
                            status_c_mask[6] = 'X';
                            if (statoInvio.tipoCanale == "MAIL")
                            {
                                status_c_mask[0] = 'V';
                                eliminareEccezione = true;
                            }
                        }
                        else if (tipoNot.codiceNotifica == "errore-consegna" || tipoNot.codiceNotifica == "preavviso-errore-consegna")
                        {
                            status_c_mask[0] = 'X';
                            status_c_mask[2] = 'X';
                            status_c_mask[3] = 'N';
                            status_c_mask[4] = 'N';
                            status_c_mask[5] = 'N';
                            status_c_mask[6] = 'N';
                            InteroperabilitaEccezioni.AggiornaDpa_StatoInvioConEccezione(notifica.destinatario, notifica.docnumber, "Errore di consegna verso la casella PEC.");
                        }
                        statusmask = new string(status_c_mask);
                    }
                    else
                    {
                        if (tipoNot.codiceNotifica == "accettazione")
                        {
                            
                            if (statoInvio.tipoCanale == "MAIL")
                            {
                                if (notifica.tipoDestinatario == "certificato")
                                {
                                    statusmask = "AVANNNN";
                                }
                                else
                                {
                                    statusmask = "VVNNNNN";
                                    eliminareEccezione = true;
                                }
                            }
                        }
                        else if (tipoNot.codiceNotifica == "non-accettazione")
                        {
                            statusmask = "XXNNNNN";
                        }
                        else if (tipoNot.codiceNotifica == "DSN" || tipoNot.codiceNotifica == "errore")
                        {
                            statusmask = "XVXNNNV";
                        }
                        else if (tipoNot.codiceNotifica == "avvenuta-consegna")
                        {
                            statusmask = "AVVAAAN";
                            if (statoInvio.tipoCanale == "MAIL")
                            {
                                statusmask = "VVVNNNN";
                                eliminareEccezione = true;
                            }
                        }
                        else if (tipoNot.codiceNotifica == "errore-consegna" || tipoNot.codiceNotifica == "preavviso-errore-consegna")
                        {
                            statusmask = "XVXNNNN";
                            InteroperabilitaEccezioni.AggiornaDpa_StatoInvioConEccezione(notifica.destinatario, notifica.docnumber, "Errore di consegna verso la casella PEC.");
                        }
                        
                    }
                    interop.AggiornaStatusMaskFromAddressAndProfile(notifica.destinatario, notifica.docnumber, statusmask, eliminareEccezione);
                    
                    
                }
                else
                {
                    logger.Debug("Stato invio non trovato");
                }
                
                
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Errore nell'aggiornamento della status mask: Messaggio {0} - StackTrace {1}", ex.Message, ex.StackTrace);
                retval = false;
            }

            return retval;
        }

    }
}
