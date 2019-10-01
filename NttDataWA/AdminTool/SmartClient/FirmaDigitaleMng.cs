using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAAdminTool.SmartClient
{
    /// <summary>
    /// Classe che gestisce nel contesto di sessione le informazioni 
    /// relative all'esito della firma digitale applicata ai documenti
    /// </summary>
    public class FirmaDigitaleResultManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static void ClearData()
        {
            CurrentData.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void SetData(FirmaDigitaleResultStatus data)
        {
            CurrentData.Add(data);
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<FirmaDigitaleResultStatus> CurrentData
        {
            get
            {
                SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

                if (!context.ContextState.ContainsKey("FirmaDigitaleResultManager.CurrentData"))
                    context.ContextState["FirmaDigitaleResultManager.CurrentData"] = new List<FirmaDigitaleResultStatus>();

                return context.ContextState["FirmaDigitaleResultManager.CurrentData"] as List<FirmaDigitaleResultStatus>;
            }
            private set
            {
                SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

                context.ContextState["FirmaDigitaleResultManager.CurrentData"] = value;
            }
        }
    }

    /// <summary>
    /// Informazioni di stato relative all'esito della firma digitale di un documento
    /// </summary>
    [Serializable()]
    public class FirmaDigitaleResultStatus
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Status
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string StatusDescription
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FirmaDigitaleMng
    {
        public FirmaDigitaleMng()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public SAAdminTool.DocsPaWR.FileDocumento GetSignedDocumentInfo(System.Web.UI.Page page)
        {
            return this.GetSignedDocumentInfo(page, FileManager.getSelectedFile());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public SAAdminTool.DocsPaWR.FileDocumento GetSignedDocumentInfo(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest)
        {
            SAAdminTool.DocsPaWR.DocsPaWebService ws = SAAdminTool.ProxyManager.getWS();

            SAAdminTool.DocsPaWR.FileDocumento retValue = ws.DocumentoGetInfoFile(fileRequest, SAAdminTool.UserManager.getInfoUtente(page));

            if (retValue == null)
                throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public SAAdminTool.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page)
        {
            return this.GetSignedDocument(page, FileManager.getSelectedFile());
        }

        /// <summary>
        /// Reperimento del documento firmato
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public SAAdminTool.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest)
        {
            SAAdminTool.DocsPaWR.DocsPaWebService ws = SAAdminTool.ProxyManager.getWS();

            SAAdminTool.DocsPaWR.FileDocumento retValue = ws.DocumentoGetFileFirmato(fileRequest, SAAdminTool.UserManager.getInfoUtente());

            if (retValue == null)
                throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

            return retValue;
        }

        /// <summary>
        /// Reperimento scheda del documento da firmare digitalmente
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public DocsPaWR.SchedaDocumento GetSchedaDocumento(string idDocumento)
        {
            if (string.IsNullOrEmpty(idDocumento))
            {
                return DocumentManager.getDocumentoSelezionato();
            }
            else
            {
                SAAdminTool.DocsPaWR.DocsPaWebService ws = SAAdminTool.ProxyManager.getWS();

                return ws.DocumentoGetDettaglioDocumentoNoDataVista(SAAdminTool.UserManager.getInfoUtente(), idDocumento, idDocumento);
            }
        }
    }
}