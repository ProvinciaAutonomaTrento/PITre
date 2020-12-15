using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.CheckInOut
{
    /// <summary>
    /// Classe per la gestione dei servizi relativi al download dei documenti 
    /// </summary>
    public sealed class SaveFileServices
    {
        /// <summary>
        /// Constante che identifica il nome della funzione
        /// di creazione nuova versione
        /// </summary>
        private const string FUNCTION_VISUALIZZA = "DO_VISUALIZZA";

        private static DocsPaWebService _webServices = null;

        /// <summary>
        /// 
        /// </summary>
        static SaveFileServices()
		{
			_webServices = new DocsPaWebService();
		}

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DocsPaWR.FileDocumento GetFileInfo()
        {
            return WsInstance.DocumentoGetInfoFile(FileManager.getSelectedFile(), UserManager.getInfoUtente());
        }
        
        /// <summary>
        /// Reperimento contenuto del file firmato
        /// </summary>
        /// <returns></returns>
        public static byte[] GetSignedFileContent()
        {
            DocsPaWR.FileDocumento fileDocumento = WsInstance.DocumentoGetFileFirmato(FileManager.getSelectedFile(), UserManager.getInfoUtente());
            
            if (fileDocumento != null)
                return fileDocumento.content;
            else
                return null;
        }

        /// <summary>
        /// Reperimento contenuto del file
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public static byte[] GetFileContent()
        {
            DocsPaWR.FileDocumento fileDocumento = WsInstance.DocumentoGetFile(FileManager.getSelectedFile(), UserManager.getInfoUtente());

            if (fileDocumento != null)
                return fileDocumento.content;
            else
                return null;
        }

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
                SchedaDocumento currentSchedaDocument = DocumentManager.getDocumentoSelezionato();

                if (currentSchedaDocument != null)
                {
                    retValue = (!UserManager.disabilitaButtHMDiritti(currentSchedaDocument.accessRights));
                }

                if (retValue)
                {
                    // Verifica se l'utente è abilitato alla funzione
                    // di inserimento di una nuova versione
                    Utente user = UserManager.getUtente();

                    Ruolo currentRole = UserManager.getRuolo();

                    foreach (Funzione function in currentRole.funzioni)
                    {
                        retValue = function.codice.Equals(FUNCTION_VISUALIZZA);

                        if (retValue)
                            break;
                    }
                }

                return retValue;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Reperimento istanza del webservice
        /// </summary>
        private static DocsPaWebService WsInstance
        {
            get
            {
                return _webServices;
            }
        }

        #endregion
    }
}