using System;
using System.Web;
using System.Linq;
using NttDataWA.DocsPaWR;
using System.Collections.Generic;
using System.Collections;

namespace NttDataWA.UIManager
{
    public class SenderManager
    {
        /// <summary>
        /// Reperimento delle configurazioni impostate per la spedizione del protocollo in uscita ai destinatari
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.ConfigSpedizioneDocumento GetConfigSpedizioneDocumento()
        {
            try
            {
                return WsInstance.GetConfigSpedizioneDocumento(UserManager.GetInfoUser());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static DocsPaWR.SpedizioneDocumento GetSpedizioneDocumento(SchedaDocumento schedaDocumento)
        {
            try
            {
                return WsInstance.GetSpedizioneDocumento(UserManager.GetInfoUser(), schedaDocumento);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static void SendMailSupportForDocumentDelivery(SchedaDocumento schedaDocumento, Utente utente, Ruolo ruolo)
        {
            try
            {
                WsInstance.SendMailSupportForDocumentDelivery(schedaDocumento, utente, ruolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public delegate void SendMailSupportForDocumentDeliveryDelegate(SchedaDocumento schedaDocumento, Utente utente, Ruolo ruolo);

        public static void CallBackSendMail(IAsyncResult result)
        {

            var del = result.AsyncState as SendMailSupportForDocumentDeliveryDelegate;

            if (del != null)
                del.EndInvoke(result);
        }


        public static bool DocumentAlreadySent_Opt(string idDocument)
        {
            try
            {
                if (!string.IsNullOrEmpty(idDocument))
                    return WsInstance.DocumentAlreadySent_Opt(idDocument);
                else return false;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static DocsPaWR.Canale getDatiCanPref(DocsPaWR.Corrispondente corr)
        {
            try
            {
                return WsInstance.AddressBookGetDatiCanalePref_Experimental(corr);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static List<DocsPaWR.InfoDocumentoSpedito> GetReportSpedizioniDocumenti(FiltriReportSpedizioni filters, List<string> idDocumenti)
        {
            try
            {
                return WsInstance.GetReportSpedizioniDocumenti(filters, idDocumenti.ToArray(), UserManager.GetInfoUser()).ToList();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        public static DocsPaWR.SpedizioneDocumento SpedisciDocumento(SchedaDocumento schedaDocumento)
        {
            try
            {
                return WsInstance.SpedisciDocumentoInAutomatico(UserManager.GetInfoUser(), schedaDocumento);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <param name="infoSpedizione"></param>
        /// <returns></returns>
        public static DocsPaWR.SpedizioneDocumento SpedisciDocumento(SchedaDocumento schedaDocumento, DocsPaWR.SpedizioneDocumento infoSpedizione)
        {
            try
            {
                InfoUtente infoUtente = UserManager.GetInfoUser();
                infoUtente.urlWA = Utils.utils.getHttpFullPath();
                return WsInstance.SpedisciDocumento(infoUtente, schedaDocumento, infoSpedizione);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Logica di business per determinare se è necessario avvisare
        /// l'utente prima della spedizione che non è stato acquisito alcun documento
        /// </summary>
        /// <param name="documento"></param>
        public static bool AvvisaSuSpedizioneDocumento(SchedaDocumento documento)
        {
            try
            {
                DocsPaWR.ConfigSpedizioneDocumento config = GetConfigSpedizioneDocumento();

                return (config.AvvisaSuSpedizioneDocumento &&
                            documento.documenti != null && documento.documenti[0].fileSize != "0");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool SpedizioneAutomaticaDocumentoAttiva()
        {
            try
            {
                DocsPaWR.ConfigSpedizioneDocumento config = GetConfigSpedizioneDocumento();
                return config.SpedizioneAutomaticaDocumento;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string getSetRicevutaPec(string idRegistro, string ricevutaPecDefault, string ricevutaPecOneTime, bool getData, string mailAddress)
        {
            try
            {
                return WsInstance.getSetRicevutaPec(idRegistro, ricevutaPecDefault, ricevutaPecOneTime, getData, mailAddress);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        /// <summary>
        /// PEC 4 - requisito 5 - storico spedizioni
        /// </summary>
        /// <param name="idDocument"></param>
        /// <returns></returns>
        public static DocsPaWR.ElStoricoSpedizioni[] GetElementiStoricoSpedizione(string idDocument)
        {
            try
            {
                //WsInstance.GetElementiStoricoSpedizione(idDocument);
                return WsInstance.SpedizioneGetElementiStoricoSpedizione(idDocument);
                //WsInstance.GetElementiStoricoSpedizione(idDocument);
                //return null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static DocsPaWR.DocsPaWebService _wsInstance = null;

        /// <summary>
        /// Istanza webservice
        /// </summary>
        private static DocsPaWR.DocsPaWebService WsInstance
        {
            get
            {
                try
                {
                    if (_wsInstance == null)
                    {
                        _wsInstance = new DocsPaWR.DocsPaWebService();
                        _wsInstance.Timeout = System.Threading.Timeout.Infinite;
                    }
                    return _wsInstance;
                }
                catch (System.Exception ex)
                {
                    UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
        }

        #region Flusso RGS

        public static List<DocsPaWR.Messaggio> GetMessaggiSuccessiviFlussoProcedurale(DocsPaWR.SchedaDocumento schedaDocumento)
        {
            try
            {
                return WsInstance.GetMessaggiSuccessiviFlussoProcedurale(schedaDocumento, UserManager.GetInfoUser()).ToList();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        #endregion
    }
}
