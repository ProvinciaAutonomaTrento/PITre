using System;
using System.Web;
using System.Text;
using System.Collections.Generic;

namespace DocsPAWA.FirmaDigitale
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
        public DocsPAWA.DocsPaWR.FileDocumento GetSignedDocumentInfo(System.Web.UI.Page page)
        {
            return this.GetSignedDocumentInfo(page, FileManager.getSelectedFile());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public DocsPAWA.DocsPaWR.FileDocumento GetSignedDocumentInfo(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest)
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = DocsPAWA.ProxyManager.getWS();

            DocsPAWA.DocsPaWR.FileDocumento retValue = ws.DocumentoGetInfoFile(fileRequest, DocsPAWA.UserManager.getInfoUtente(page));

            if (retValue == null)
                throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public DocsPAWA.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page)
        {
            return this.GetSignedDocument(page, FileManager.getSelectedFile());
        }

        /// <summary>
		/// Reperimento del documento firmato
		/// </summary>
		/// <param name="page"></param>
        /// <param name="fileRequest"></param>
		/// <returns></returns>
        public DocsPAWA.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest)
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = DocsPAWA.ProxyManager.getWS();

            DocsPAWA.DocsPaWR.FileDocumento retValue = ws.DocumentoGetFileFirmato(fileRequest, DocsPAWA.UserManager.getInfoUtente());

            if (retValue == null)
                throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public string GetSignedDocumentHash(System.Web.UI.Page page, bool pades)
        {
            return this.GetSignedDocumentHash(page, FileManager.getSelectedFile(),pades);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public string GetCoSignDocumentHash(System.Web.UI.Page page)
        {
            DocsPaWR.FileRequest fr = FileManager.getSelectedFile();
            DocsPAWA.DocsPaWR.DocsPaWebService ws = DocsPAWA.ProxyManager.getWS();

            DocsPAWA.DocsPaWR.MassSignature msReq = new DocsPaWR.MassSignature { fileRequest = fr, signPades = false, cosign = true };
            DocsPAWA.DocsPaWR.MassSignature ms = ws.getSha256(msReq, DocsPAWA.UserManager.getInfoUtente());
            if (ms == null)
                throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fr.docServerLoc + fr.path + fr.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");



            return string.Format("{0}#{1}", ms.base64Sha256, ms.base64Signature);

        }

        /// <summary>
        /// Reperimento dell' HASH sha256 del documento da firmare
        /// </summary>
        /// <param name="page"></param>
        /// <param name="fileRequest"></param>
        /// <returns></returns>
        public string GetSignedDocumentHash(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest,bool pades)
        {
            DocsPAWA.DocsPaWR.DocsPaWebService ws = DocsPAWA.ProxyManager.getWS();

            DocsPAWA.DocsPaWR.MassSignature  msReq = new DocsPaWR.MassSignature { fileRequest = fileRequest, signPades =pades} ;
            DocsPAWA.DocsPaWR.MassSignature ms = ws.getSha256(msReq, DocsPAWA.UserManager.getInfoUtente());

            if (ms == null)
                throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

            return ms.base64Sha256;
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
                DocsPAWA.DocsPaWR.DocsPaWebService ws = DocsPAWA.ProxyManager.getWS();

                return ws.DocumentoGetDettaglioDocumentoNoDataVista(DocsPAWA.UserManager.getInfoUtente(), idDocumento, idDocumento);
            }
        }
	}
}