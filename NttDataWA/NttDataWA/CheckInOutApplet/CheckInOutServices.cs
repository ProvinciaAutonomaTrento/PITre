using System;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.CheckInOutApplet
{
    /// <summary>
    /// Classe per la gestione dei servizi relativi al checkin/checkout dei documenti
    /// </summary>
    public sealed class CheckInOutServices
    {

        /// <summary>
        /// Constante che identifica il nome della funzione
        /// di creazione nuova versione
        /// </summary>
        private const string FUNCTION_NUOVA_VERSIONE = "DO_VER_NUOVA";

        /// <summary>
        /// 
        /// </summary>
        private static DocsPaWebService _webServices = null;

        static CheckInOutServices()
        {
            _webServices = new DocsPaWebService();
        }

        #region Public methods

        /// <summary>
        /// Verifica se l'utente corrente con il ruolo corrente 
        /// è abilitato alla funzione di checkin-checkout
        /// </summary>
        public static bool UserEnabled
        {
            get
            {
                bool retValue = true;

                // Controllo se il documento è in stato readonly o stato finale,
                // l'utente non è abilitato alla funzionalità
                SchedaDocumento currentSchedaDocument = CurrentSchedaDocumento;

                if (currentSchedaDocument != null)
                {
                    retValue = (!UIManager.UserManager.disabilitaButtHMDiritti(currentSchedaDocument.accessRights));
                }

                if (retValue)
                {
                    // Verifica se l'utente è abilitato alla funzione
                    // di inserimento di una nuova versione
                    //Utente user = UIManager.UserManager.getUtente();

                    Ruolo currentRole = UIManager.UserManager.GetSelectedRole();

                    foreach (Funzione function in currentRole.funzioni)
                    {
                        retValue = function.codice.Equals(FUNCTION_NUOVA_VERSIONE);

                        if (retValue)
                            break;
                    }
                }

                return retValue;
            }
        }

        /// <summary>
        /// Inizializzazione contesto checkinout per il documento corrente
        /// </summary>
        public static void InitializeContext()
        {
            if (CurrentSchedaDocumento != null)
            {
                // Inizializzazione del contesto di checkout del documento
                CheckOutAppletContext.Current = new CheckOutAppletContext(CurrentSchedaDocumento);
            }
        }

        /// <summary>
        /// Reperimento del file in stato checkedout
        /// </summary>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        public static byte[] GetCheckedOutFileDocument()
        {
            CheckOutStatus checkOutStatus = CheckOutAppletContext.Current.Status;

            return _webServices.GetCheckedOutFileDocument(checkOutStatus, GetInfoUtente());
        }

        /// <summary>
        /// Verifica, in base al contesto corrente, se il tab corrente della scheda documento è quella degli allegati
        /// </summary>
        /// <returns></returns>
        protected static bool IsSelectedTabAllegati()
        {

            /* ABBATANGELI - è scomparso il codice corretto ????
            string currentTab = System.Web.HttpContext.Current.Session["tab"] as string;
            return (!string.IsNullOrEmpty(currentTab) && currentTab.ToLower() == "allegati");
            */

            Boolean retVal = false;

            if (UIManager.DocumentManager.getSelectedAttachId() != null)
            {
                retVal = true;
            }

            return retVal;
        }

        /// <summary>
        /// Reperimento delle informazioni di stato checkout del documento
        /// </summary>
        /// <returns></returns>
        public static CheckOutStatus GetCheckOutDocumentStatus()
        {
            DocsPaWR.SchedaDocumento schedaDocumento = null;

            if (IsEnabledProfilazioneAllegati && IsSelectedTabAllegati())
            {
                // Tab "allegati" correntemente selezionato,
                // reperimento dello stato checkout dell'allegato selezionato
                DocsPaWR.FileRequest fileRequest = UIManager.FileManager.getSelectedFile();

                if (fileRequest != null && fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                {

                    schedaDocumento = System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] as DocsPaWR.SchedaDocumento;
                }
            }
            else
            {
                // Qualsiasi altro tab differente dal tab "allegati",
                // reperimento dello stato checkout del documento principale
                schedaDocumento = CheckInOutServices.CurrentSchedaDocumento;
            }

            if (schedaDocumento != null)
                return schedaDocumento.checkOutStatus;
            else
                return null;
        }

        /// <summary>
        /// CheckOut di un documento senza estrarre il contenuto del file
        /// </summary>
        /// <param name="documentLocation"></param>
        /// <param name="machineName"></param>
        /// <param name="checkOutStatus"></param>
        /// <returns></returns>
        public static ValidationResultInfo CheckOutDocument(string documentLocation, string machineName, out CheckOutStatus checkOutStatus)
        {
            ValidationResultInfo retValue = null;
            checkOutStatus = null;

            SchedaDocumento schedaDocumento = CheckInOutServices.CurrentSchedaDocumento;

            if (schedaDocumento != null)
            {
                string docnum = schedaDocumento.docNumber;
                string docid = schedaDocumento.systemId;
                if (IsSelectedTabAllegati()) {
                    docnum = UIManager.DocumentManager.GetSelectedAttachment().docNumber;
                    docid = UIManager.DocumentManager.GetSelectedAttachment().docNumber;
                }

                retValue = _webServices.CheckOutDocument(docid, docnum, documentLocation, machineName, GetInfoUtente(), out checkOutStatus);

                if (retValue.Value && !IsSelectedTabAllegati())
                {
                    schedaDocumento.checkOutStatus = checkOutStatus;

                    CheckOutAppletContext.Current = new CheckOutAppletContext(schedaDocumento);
                }
            }

            return retValue;
        }

        /// <summary>
        /// CheckOut di un documento e restituzione del file
        /// </summary>
        /// <param name="documentLocation"></param>
        /// <param name="machineName"></param>
        /// <param name="checkOutStatus"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static ValidationResultInfo CheckOutDocumentWithFile(string documentLocation, string machineName, out CheckOutStatus checkOutStatus, out byte[] content, string docId = null, string docNum = null)
        {
            SchedaDocumento schedaDocumento = CheckInOutServices.CurrentSchedaDocumento;

            ValidationResultInfo retValue = _webServices.CheckOutDocumentWithFile((docId==null?schedaDocumento.systemId:docId), (docNum==null?schedaDocumento.docNumber:docNum), documentLocation, machineName, GetInfoUtente(), out checkOutStatus, out content);

            if (retValue.Value)
            {
                schedaDocumento.checkOutStatus = checkOutStatus;

                CheckOutAppletContext.Current = new CheckOutAppletContext(schedaDocumento);
            }

            return retValue;
        }

        /// <summary>
        /// CheckIn di un documento
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static ValidationResultInfo CheckInDocument(byte[] content)
        {
            ValidationResultInfo retValue = null;

            CheckOutStatus ckSt = null;
            string checkInComments = string.Empty;

            if (IsSelectedTabAllegati())
            {
                if (UIManager.DocumentManager.GetSelectedAttachment() != null)
                {
                    ckSt = UIManager.DocumentManager.GetCheckOutDocumentStatus(UIManager.DocumentManager.GetSelectedAttachment().docNumber);
                }
            }
            else
            {
                if (CheckOutAppletContext.Current != null)
                {
                    ckSt = CheckOutAppletContext.Current.Status;
                    checkInComments = CheckOutAppletContext.Current.CheckInComments;
                }
            }
            
            if (ckSt != null)
            {

                retValue = _webServices.CheckInDocument(ckSt, GetInfoUtente(), content, checkInComments);

                if (retValue.Value)
                {
                    CheckOutAppletContext.Current = null;

                    if (System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] != null)
                        System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] = null;
                }
            }
            else
            {
                retValue = new ValidationResultInfo();
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ValidationResultInfo UndoCheckOutDocument()
        {
            ValidationResultInfo retValue = null;

            if (IsSelectedTabAllegati())
            {
                Allegato tempAll = UIManager.DocumentManager.GetSelectedAttachment();
                if (tempAll != null)
                {
                    CheckOutStatus checkOutStatus = UIManager.DocumentManager.GetCheckOutDocumentStatus(UIManager.DocumentManager.GetSelectedAttachment().docNumber);
                    retValue = _webServices.UndoCheckOutDocument(checkOutStatus, GetInfoUtente());
                }
            }
            else
            {
                if (CheckOutAppletContext.Current != null && CheckOutAppletContext.Current.Status != null)
                {
                    CheckOutStatus checkOutStatus = CheckOutAppletContext.Current.Status;

                    retValue = _webServices.UndoCheckOutDocument(checkOutStatus, GetInfoUtente());

                    if (retValue.Value)
                    {
                        CheckOutAppletContext.Current = null;
                    }
                }
                else
                {
                    retValue = new ValidationResultInfo();
                    retValue.Value = true;
                    retValue.BrokenRules = new BrokenRule[0];
                }
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se un documento è in checkout
        /// </summary>
        /// <param name="ownerUser">Utente che ha fatto il checkout del documento</param>
        /// <returns></returns>
        public static bool IsCheckedOutDocumentWithUser(out string ownerUser)
        {
            bool isCheckedOut = false;
            ownerUser = string.Empty;

            SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord(); //CheckInOutServices.CurrentSchedaDocumento;

            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
            {
                isCheckedOut = true;
                ownerUser = schedaDocumento.checkOutStatus.UserName;
            }

            return isCheckedOut;
        }

        /// <summary>
        /// Verifica se il documento principale o un allegato è in checkout, relativamente al parametro checkAllegati
        /// </summary>
        /// <param name="idDocument">SystemID del documento</param>
        /// <param name="checkedOutUser"></param>
        /// <param name="documentNumber"></param>
        /// <param name="utente">Utente che ha fatto il checkout del documento</param>
        /// <returns></returns>
        public static bool IsCheckedOutDocument(string idDocument, string documentNumber, InfoUtente utente, bool checkAllegati)
        {
            bool retValue = false;
            if (!string.IsNullOrEmpty(idDocument) && !string.IsNullOrEmpty(documentNumber))
                retValue = _webServices.IsCheckedOutDocument(idDocument, documentNumber, utente, checkAllegati);
            return retValue;
        }

        /// <summary>
        /// Verifica se il documento principale è in checkout
        /// </summary>
        /// <returns></returns>
        public static bool IsCheckedOutDocument()
        {
            SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord();

            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
                return (!string.IsNullOrEmpty(schedaDocumento.checkOutStatus.ID));
            else
                return false;
        }

        /// <summary>
        /// Verifica se il documento è in checkout per una conversione PDF
        /// </summary>
        /// <returns></returns>
        public static bool IsCheckedOutConversionePdf()
        {
            SchedaDocumento schedaDocumento = UIManager.DocumentManager.getSelectedRecord();

            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
                return schedaDocumento.checkOutStatus.InConversionePdf;
            else
                return false;
        }

        /// <summary>
        /// Verifica se il documento è stato messo in checkout dall'utente richiesto
        /// </summary>
        /// <param name="checkOutMessage"></param>
        /// <returns></returns>
        public static bool IsCheckedOutDocument(out string checkOutMessage)
        {
            bool retValue = false;
            checkOutMessage = string.Empty;

            string ownerUser;

            retValue = IsCheckedOutDocumentWithUser(out ownerUser);

            if (retValue)
            {
                if (ownerUser.ToUpper() == GetInfoUtente().userId.ToUpper())
                    //checkOutMessage="Il documento risulta bloccato." +  Environment.NewLine + "Per effettuare l\\'operazione richiesta è necessario prima rilasciare il documento.";
                    checkOutMessage = "Il documento risulta bloccato.";
                else
                    //checkOutMessage = "Il documento risulta già bloccato dall\\'utente " + ownerUser + "." + Environment.NewLine + "Impossibile completare l\\'operazione richiesta.";
                    checkOutMessage = "Il documento risulta già bloccato dall\\'utente " + ownerUser + ".";
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se il documento è stato messo in checkout dall'utente richiesto
        /// </summary>
        /// <returns></returns>
        public static bool IsOwnerCheckedOutDocument()
        {
            SchedaDocumento schedaDocumento = CheckInOutServices.CurrentSchedaDocumento;

            if (schedaDocumento != null && schedaDocumento.checkOutStatus != null)
            {
                InfoUtente infoUtente = GetInfoUtente();

                return (infoUtente.userId.ToUpper().Equals(schedaDocumento.checkOutStatus.UserName));
            }
            else
                return false;
        }

        /// <summary>
        /// Aggiornamento dello statto di checkout della scheda documento correntemente visualizzata

        /// </summary>
        /// <remarks>
        /// Qualora sia attivata la gestione degli allegati profilati, la scheda documento sarà relativa
        /// all'allegato correntemente selezionato da tab allegati
        /// </remarks>
        public static void RefreshCheckOutStatus()
        {
            NttDataWA.DocsPaWR.SchedaDocumento schedaDocumento = null;

            //if (IsEnabledProfilazioneAllegati && IsSelectedTabAllegati())
            if (IsSelectedTabAllegati())
            {
                // Tab "allegati" correntemente selezionato,
                // reperimento dello stato checkout dell'allegato selezionato.
                // Solo se attiva la profilazione allegati.
                DocsPaWR.FileRequest fileRequest = UIManager.FileManager.getSelectedFile();

                if (fileRequest != null && fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                {

                    schedaDocumento = System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] as DocsPaWR.SchedaDocumento;

                    if (schedaDocumento != null)
                    {
                        schedaDocumento = _webServices.DocumentoGetDettaglioDocumento(GetInfoUtente(), schedaDocumento.systemId, schedaDocumento.docNumber);

                        System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] = schedaDocumento;
                    }
                }
            }
            else
            {
                schedaDocumento = UIManager.DocumentManager.getSelectedRecord();
                
                if (schedaDocumento != null)
                {
                    // Reperimento scheda documento per l'allegato
                    schedaDocumento = _webServices.DocumentoGetDettaglioDocumento(GetInfoUtente(), schedaDocumento.systemId, schedaDocumento.docNumber);

                    UIManager.DocumentManager.setSelectedRecord(schedaDocumento);
                }
            }

            if (schedaDocumento != null)
                //Inizializzazione del contesto di checkout del documento
                NttDataWA.CheckInOutApplet.CheckOutAppletContext.Current = new NttDataWA.CheckInOutApplet.CheckOutAppletContext(schedaDocumento);
            else
                NttDataWA.CheckInOutApplet.CheckOutAppletContext.Current = null;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Indica se è attiva o meno la profilazione degli allegati
        /// </summary>
        private static bool IsEnabledProfilazioneAllegati
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["isEnabledProfileazioneAllegati"] == null)
                {
                    // Verifica se è attiva la profilazione degli allegati
                    System.Web.HttpContext.Current.Session["isEnabledProfileazioneAllegati"] =
                        _webServices.IsEnabledProfilazioneAllegati();
                }

                return Convert.ToBoolean(System.Web.HttpContext.Current.Session["isEnabledProfileazioneAllegati"]);
            }
        }

        /// <summary>
        /// Reperimento utente corrente
        /// </summary>
        /// <returns></returns>
        private static InfoUtente GetInfoUtente()
        {
            return UIManager.UserManager.GetInfoUser();
        }

        /// <summary>
        /// Reperimento scheda documento corrente
        /// </summary>
        public static SchedaDocumento CurrentSchedaDocumento
        {
            get
            {
                DocsPaWR.SchedaDocumento schedaDocumento = null;

                //if (IsEnabledProfilazioneAllegati && IsSelectedTabAllegati())
                if (IsSelectedTabAllegati())
                {
                    // Tab "allegati" correntemente selezionato,
                    // reperimento dello stato checkout dell'allegato selezionato.
                    // Solo se attiva la profilazione allegati.
                    //DocsPaWR.FileRequest fileRequest = UIManager.FileManager.getSelectedFile();
                    DocsPaWR.FileRequest fileRequest = null;
                    if (UIManager.DocumentManager.getSelectedAttachId() != null)
                    {
                        fileRequest = FileManager.GetFileRequest(UIManager.DocumentManager.getSelectedAttachId());
                    }
                    else
                    {
                        fileRequest = FileManager.GetFileRequest();
                    }

                    //if (fileRequest != null && fileRequest.GetType() == typeof(DocsPaWR.Allegato))
                    if (fileRequest != null)
                    {
                        schedaDocumento = System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] as DocsPaWR.SchedaDocumento;

                        if (schedaDocumento == null ||
                            (schedaDocumento != null && schedaDocumento.docNumber != fileRequest.docNumber))
                        {
                            // Reperimento scheda documento per l'allegato se è valorizzato il docNumber
                            if(!String.IsNullOrEmpty(fileRequest.docNumber))
                            {
                                schedaDocumento = _webServices.DocumentoGetDettaglioDocumento(GetInfoUtente(), fileRequest.docNumber, fileRequest.docNumber);

                                System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] = schedaDocumento;
                            }
                        }
                    }
                    else
                    {
                        if (System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"]!=null)
                            System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] = null;
                    }
                }
                else
                {
                    // Qualsiasi altro tab differente dal tab "allegati",
                    // reperimento dello stato checkout del documento principale
                    if (System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"]!=null)
                        System.Web.HttpContext.Current.Session["schedaAllegatoSelezionato"] = null;

                    schedaDocumento =UIManager.DocumentManager.getSelectedRecord();
                }

                return schedaDocumento;
            }
        }

        #endregion
    }
}
