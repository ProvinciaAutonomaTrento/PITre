using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.SmartClient
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
            HttpContext.Current.Session["FirmaDigitaleResultManager.CurrentData"] = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void SetData(FirmaDigitaleResultStatus data)
        {
            CurrentData.Add(data);
            HttpContext.Current.Session["FirmaDigitaleResultManager.CurrentData"] = CurrentData;
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<FirmaDigitaleResultStatus> CurrentData
        {
            get
            {
                
                //SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

                if (HttpContext.Current.Session["FirmaDigitaleResultManager.CurrentData"] == null)
                {
                    //System.Web.HttpContext.Current
                    //context.ContextState["FirmaDigitaleResultManager.CurrentData"] = new List<FirmaDigitaleResultStatus>();
                    HttpContext.Current.Session["FirmaDigitaleResultManager.CurrentData"] = new List<FirmaDigitaleResultStatus>();
                }
                //return context.ContextState["FirmaDigitaleResultManager.CurrentData"] = as List<FirmaDigitaleResultStatus>;
                return HttpContext.Current.Session["FirmaDigitaleResultManager.CurrentData"] as List<FirmaDigitaleResultStatus>;
            }
            private set
            {
                //SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;
                //context.ContextState["FirmaDigitaleResultManager.CurrentData"] = value;
                HttpContext.Current.Session["FirmaDigitaleResultManager.CurrentData"] = value;
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
        public NttDataWA.DocsPaWR.FileDocumento GetSignedDocumentInfo(System.Web.UI.Page page)
        {
            return this.GetSignedDocumentInfo(page, NttDataWA.UIManager.FileManager.getSelectedFile());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public NttDataWA.DocsPaWR.FileDocumento GetSignedDocumentInfo(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest)
        {
            NttDataWA.DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();

            NttDataWA.DocsPaWR.FileDocumento retValue = ws.DocumentoGetInfoFile(fileRequest, NttDataWA.UIManager.UserManager.GetInfoUser());

            //if (retValue == null)
            //    throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public NttDataWA.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page)
        {
            return this.GetSignedDocument(page, NttDataWA.UIManager.FileManager.getSelectedFile());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public NttDataWA.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page, string idDocument)
        {
            return this.GetSignedDocument(page, NttDataWA.UIManager.FileManager.getSelectedMassSignature(idDocument).fileRequest);
        }

        /// <summary>
        /// Reperimento del documento firmato
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public NttDataWA.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest)
        {
            NttDataWA.DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();

            NttDataWA.DocsPaWR.FileDocumento retValue = null;
            if (fileRequest!=null)
                retValue = ws.DocumentoGetFileFirmato(fileRequest, NttDataWA.UIManager.UserManager.GetInfoUser());

/*
            if (retValue == null)
                throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");
 */

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
                return NttDataWA.UIManager.DocumentManager.getSelectedRecord();
            }
            else
            {
                NttDataWA.DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();

                return ws.DocumentoGetDettaglioDocumentoNoDataVista(NttDataWA.UIManager.UserManager.GetInfoUser(), idDocumento, idDocumento);
            }
        }
    }
}