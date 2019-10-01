using System;
using System.Linq;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;

namespace NttDataWA.CheckInOut
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

            DocsPaWR.FileRequest fileInfo = (DocumentManager.getSelectedAttachId() != null) ?
                    FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) :
                        FileManager.GetFileRequest();

            return WsInstance.DocumentoGetInfoFile(fileInfo, UserManager.GetInfoUser());
        }

        /// <summary>
        /// Reperimento contenuto del file firmato
        /// </summary>
        /// <returns></returns>
        public static byte[] GetSignedFileContent()
        {
            String selectedVersionId = null;

            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                selectedVersionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();

            DocsPaWR.FileRequest fileInfo = (DocumentManager.getSelectedAttachId() != null) ?
                FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) : FileManager.GetFileRequest(selectedVersionId);

            DocsPaWR.FileDocumento fileDocumento = WsInstance.DocumentoGetFileFirmato(fileInfo, UserManager.GetInfoUser());

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
            String selectedVersionId = null;

            if (DocumentManager.getSelectedNumberVersion() != null && DocumentManager.ListDocVersions != null)
                selectedVersionId = (from v in DocumentManager.ListDocVersions where v.version.Equals(DocumentManager.getSelectedNumberVersion()) select v.versionId).FirstOrDefault();

            DocsPaWR.FileRequest fileInfo = (DocumentManager.getSelectedAttachId() != null) ?
                FileManager.GetFileRequest(DocumentManager.getSelectedAttachId()) : FileManager.GetFileRequest(selectedVersionId);

            DocsPaWR.FileDocumento fileDocumento = WsInstance.DocumentoGetFile(fileInfo, UserManager.GetInfoUser());

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
                SchedaDocumento currentSchedaDocument = DocumentManager.getSelectedRecord();

                if (currentSchedaDocument != null)
                {
                    retValue = (!UserManager.disabilitaButtHMDiritti(currentSchedaDocument.accessRights));
                }

                if (retValue)
                {
                    // Verifica se l'utente è abilitato alla funzione
                    // di inserimento di una nuova versione
                    //Utente user = UserManager.getUtente();

                    Ruolo currentRole = UserManager.GetSelectedRole();

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

        public static byte[] GetPackage()
        {
            byte[] _zipContent = null;
            NttDataWA.DocsPaWR.SchedaDocumento _schedaDocumento;
            DocsPaWR.FileRequest _fileRequestTemp;
            DocsPaWR.FileDocumento _filePrincipale;
            DocsPaWR.FileDocumento _fileTemp;
            string _versionIdTemp;
            Ionic.Zip.ZipFile _zipFile;
            System.IO.MemoryStream _tempStream;
            try
            {
                _tempStream = new System.IO.MemoryStream();
                _zipFile = new Ionic.Zip.ZipFile();
                _schedaDocumento = UIManager.DocumentManager.getSelectedRecord();
                _versionIdTemp = DocumentManager.ListDocVersions[0].versionId;
                _fileRequestTemp = UIManager.FileManager.GetFileRequest(_versionIdTemp);
                var _docPrincipale = _schedaDocumento.documenti[0];
                var filePath = _docPrincipale.fileName;
                if (
                filePath.ToUpper().EndsWith(".P7M") ||
                filePath.ToUpper().EndsWith(".TSD") ||
                filePath.ToUpper().EndsWith(".M7M") ||
                filePath.ToUpper().EndsWith(".TSR")
                )
                {
                    _filePrincipale = WsInstance.DocumentoGetFileFirmato(_fileRequestTemp, UIManager.UserManager.GetInfoUser());
                } else
                {
                    _filePrincipale = WsInstance.DocumentoGetFile(_fileRequestTemp, UIManager.UserManager.GetInfoUser());
                }


                if (_filePrincipale != null && _filePrincipale.content != null && _filePrincipale.content.Length > 0)
                {
                    _zipFile.AddEntry(_filePrincipale.nomeOriginale, _filePrincipale.content);
                }

                int indexAllegato = 0;
                foreach (var attach in _schedaDocumento.allegati)
                {
                    filePath = attach.fileName;
                    if (
                        filePath.ToUpper().EndsWith(".P7M") ||
                        filePath.ToUpper().EndsWith(".TSD") ||
                        filePath.ToUpper().EndsWith(".M7M") ||
                        filePath.ToUpper().EndsWith(".TSR")
                    )
                    {
                        _fileTemp = WsInstance.DocumentoGetFileFirmato(attach, UIManager.UserManager.GetInfoUser());
                    } else
                    {
                        _fileTemp = WsInstance.DocumentoGetFile(attach, UIManager.UserManager.GetInfoUser());
                    }
                    // può essere null ?
                    if (_fileTemp == null || _fileTemp.content == null || _fileTemp.content.Length == 0) { continue; }
                    _zipFile.AddEntry("Allegato_" + ++indexAllegato + "_" + _fileTemp.nomeOriginale, _fileTemp.content);

                }
                _zipFile.Save(_tempStream);



                _zipContent = _tempStream.ToArray();

            }
            catch (Exception ex) { throw ex; }

            return _zipContent;
        }
    }
}