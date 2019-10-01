using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using NttDataWA.DocsPaWR;
using NttDataWA.Utils;

namespace NttDataWA.DigitalSignature
{
    /// <summary>
    /// Classe che gestisce nel contesto di sessione le informazioni 
    /// relative all'esito della firma digitale applicata ai documenti
    /// </summary>
    public class DigitalSignatureResultManager
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
        public static void SetData(DigitalSignatureResultStatus data)
        {
            CurrentData.Add(data);
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<DigitalSignatureResultStatus> CurrentData
        {
            get
            {
                //SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

                if (HttpContext.Current.Session["DigitalSignatureResultManager.CurrentData"]==null)
                    HttpContext.Current.Session["DigitalSignatureResultManager.CurrentData"] = new List<DigitalSignatureResultStatus>();

                return HttpContext.Current.Session["DigitalSignatureResultManager.CurrentData"] as List<DigitalSignatureResultStatus>;
            }
            private set
            {
                //SiteNavigation.CallContext context = SiteNavigation.CallContextStack.CurrentContext;

                HttpContext.Current.Session["DigitalSignatureResultManager.CurrentData"] = value;
            }
        }
    }

    /// <summary>
    /// Informazioni di stato relative all'esito della firma digitale di un documento
    /// </summary>
    [Serializable()]
    public class DigitalSignatureResultStatus
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


    public class RemoteDigitalSignManager
    {

        public class Memento
        {
            public String Alias;
            public String Dominio;
        }


        public enum tipoFirma
        {
            CADES,
            PADES
        }
        

        public RemoteDigitalSignManager()
        {
        }



        public string HSM_RequestCertificateJson(String AliasCertificato, String DominioCertificato)
        {
            NttDataWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
            try
            {
                return ws.HSM_RequestCertificateJson(AliasCertificato,DominioCertificato);
            }
            catch (Exception e)
            {
                // loggiamo un errore ?!?!?!?
                return null;
            }
        }

        public string HSM_OpenMultiSignSession(NttDataWA.DocsPaWR.FileRequest[] fileRequestList, bool cofirma, bool timestamp, tipoFirma  TipoFirma)
        {

            DocsPaWR.InfoUtente infoUt = NttDataWA.UIManager.UserManager.GetInfoUser();
            NttDataWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
            try
            {
                string tipoFirmaSTR = TipoFirma.ToString();
                return ws.HSM_OpenMultiSignSession(infoUt, fileRequestList, cofirma, timestamp, tipoFirmaSTR);
            }
            catch (Exception e)
            {
                // loggiamo un errore ?!?!?!?
                return null;
            }
        }


        public DocsPaWR.FirmaResult[] HSM_SignMultiSignSession(NttDataWA.DocsPaWR.FileRequest[] fileRequestList, string MultiSignToken, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool cofirma)
        {
            DocsPaWR.InfoUtente infoUt = NttDataWA.UIManager.UserManager.GetInfoUser();
            NttDataWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
            try
            {
                return ws.HSM_SignMultiSignSession(infoUt, fileRequestList, MultiSignToken, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato, cofirma);
            }
            catch (Exception e)
            {
                // loggiamo un errore ?!?!?!?
                return null;
            }
        }




        public bool HSM_Sign(FileRequest fr, bool cofirma, bool timestamp, tipoFirma TipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf, out DocsPaWR.FirmaResult esito)
        {
            InfoUtente infoUt = NttDataWA.UIManager.UserManager.GetInfoUser();
            esito = new FirmaResult();
            DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();
            try
            {
                string tipoFirmaSTR = TipoFirma.ToString();
                return docsPaWS.HSM_SignConEsito(infoUt, fr, cofirma, timestamp, tipoFirmaSTR, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato, ConvertPdf, out esito);
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public FirmaResult[] HSM_SignMultiSign(FileRequest[] fileRequestList, bool cofirma, bool timestamp, tipoFirma TipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato)
        {
            InfoUtente infoUtente = NttDataWA.UIManager.UserManager.GetInfoUser();
            FirmaResult[] firmaResult = null;
            DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();
            try
            {
                string tipoFirmaSTR = TipoFirma.ToString();
                string multiSignToken = docsPaWS.HSM_OpenMultiSignSession(infoUtente, fileRequestList, cofirma, timestamp, tipoFirmaSTR);
                if(!string.IsNullOrEmpty(multiSignToken))
                {
                    firmaResult = docsPaWS.HSM_SignMultiSignSession(infoUtente, fileRequestList, multiSignToken, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato, cofirma);
                }
            }
            catch (System.Exception ex)
            {
                return null;
            }
            return  firmaResult;
        }

        public bool HSM_RequestOTP(String AliasCertificato, String DominioCertificato)
        {

            if (string.IsNullOrEmpty (AliasCertificato))
                return false;

            if (string.IsNullOrEmpty(DominioCertificato))
                return false;

            NttDataWA.DocsPaWR.DocsPaWebService ws = new NttDataWA.DocsPaWR.DocsPaWebService();
            try
            {
                return ws.HSM_RequestOTP(AliasCertificato, DominioCertificato);
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }


        public Memento HSM_GetMementoForUser()
        {
           DocsPaWR.InfoUtente infoUt=  NttDataWA.UIManager.UserManager.GetInfoUser();
           NttDataWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
           try
           {
               Memento retval= new Memento ();
               string[] resp = ws.HSM_GetMementoForUser(infoUt);

               if (resp.Length ==2)
               {
                   retval.Dominio = resp[0];
                   retval.Alias = resp[1];
               }
               return retval;
           }
           catch (System.Exception ex)
           {
               UIManager.AdministrationManager.DiagnosticError(ex);
               return null;
           }
        }

        public bool HSM_SetMementoForUser(Memento memento)
        {
            if (memento == null)
                return false;

            if (String.IsNullOrEmpty ( memento.Alias) )
                return false;

            if (String.IsNullOrEmpty(memento.Dominio))
                return false;
            
            DocsPaWR.InfoUtente infoUt = NttDataWA.UIManager.UserManager.GetInfoUser();
            NttDataWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
            try
            {
                return ws.HSM_SetMementoForUser(infoUt, memento.Dominio, memento.Alias);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }
    }

	/// <summary>
	/// 
	/// </summary>
	public class DigitalSignManager
	{
		public DigitalSignManager()
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
            NttDataWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();

            NttDataWA.DocsPaWR.FileDocumento retValue = ws.DocumentoGetInfoFile(fileRequest, NttDataWA.UIManager.UserManager.GetInfoUser());

            if (retValue == null)
                throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

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
		/// Reperimento del documento firmato
		/// </summary>
		/// <param name="page"></param>
        /// <param name="fileRequest"></param>
		/// <returns></returns>
        public NttDataWA.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest)
        {
            NttDataWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();

            NttDataWA.DocsPaWR.FileDocumento retValue = ws.DocumentoGetFileFirmato(fileRequest, NttDataWA.UIManager.UserManager.GetInfoUser());

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
                return NttDataWA.UIManager.DocumentManager.getSelectedRecord();
            }
            else
            {
                NttDataWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
                return ws.DocumentoGetDettaglioDocumentoNoDataVista(NttDataWA.UIManager.UserManager.GetInfoUser(), idDocumento, idDocumento);
            }
        }
	}
}
