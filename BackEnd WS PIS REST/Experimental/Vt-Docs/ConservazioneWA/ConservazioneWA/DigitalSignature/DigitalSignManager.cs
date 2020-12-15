using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using ConservazioneWA.DocsPaWR;
using ConservazioneWA.Utils;

namespace ConservazioneWA.DigitalSignature
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
        protected static DocsPaWR.DocsPaWebService ws = new ProxyManager().getProxyDocsPa();

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
            //ConservazioneWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
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

        public string HSM_OpenMultiSignSession(ConservazioneWA.DocsPaWR.FileRequest[] fileRequestList, bool cofirma, bool timestamp, tipoFirma TipoFirma, InfoUtente infoUt)
        {

            
            //ConservazioneWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
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


        public DocsPaWR.FirmaResult[] HSM_SignMultiSignSession(ConservazioneWA.DocsPaWR.FileRequest[] fileRequestList, string MultiSignToken, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, InfoUtente infoUt)
        {
            //ConservazioneWA.DocsPaWR.DocsPaWebService ws = ProxyManager.);
            try
            {
                return ws.HSM_SignMultiSignSession(infoUt, fileRequestList, MultiSignToken, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato);
            }
            catch (Exception e)
            {
                // loggiamo un errore ?!?!?!?
                return null;
            }
        }




        public bool HSM_Sign(FileRequest fr, bool cofirma, bool timestamp, tipoFirma TipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf, InfoUtente infoUt)
        {
            //this.wss = new ProxyManager().getProxy();
            try
            {
                string tipoFirmaSTR = TipoFirma.ToString();
                return ws.HSM_Sign(infoUt, fr, cofirma, timestamp, tipoFirmaSTR, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato, ConvertPdf);
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public bool HSM_SignContent(String idInstanza, byte[] content, bool cofirma, bool timestamp, tipoFirma TipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf, InfoUtente infoUt)
        {
            bool retVal = false;

            try
            {
                string tipoFirmaSTR = TipoFirma.ToString();
                retVal = ws.HSM_SignContent(idInstanza, infoUt, content, cofirma, timestamp, tipoFirmaSTR, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato, ConvertPdf);
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                retVal =  false;
            }

            return retVal;
        }

        public FirmaResult[] HSM_SignMultiSign(FileRequest[] fileRequestList, bool cofirma, bool timestamp, tipoFirma TipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, InfoUtente infoUtente)
        {
            FirmaResult[] firmaResult = null;
            //DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();
            try
            {
                string tipoFirmaSTR = TipoFirma.ToString();
                string multiSignToken = ws.HSM_OpenMultiSignSession(infoUtente, fileRequestList, cofirma, timestamp, tipoFirmaSTR);
                if(!string.IsNullOrEmpty(multiSignToken))
                {
                    firmaResult = ws.HSM_SignMultiSignSession(infoUtente, fileRequestList, multiSignToken, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato);
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

            ConservazioneWA.DocsPaWR.DocsPaWebService ws = new ConservazioneWA.DocsPaWR.DocsPaWebService();
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


        public Memento HSM_GetMementoForUser(InfoUtente infoUt)
        {
           //DocsPaWR.InfoUtente infoUt=  ConservazioneWA.UIManager.UserManager.GetInfoUser();
           //ConservazioneWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
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
               //UIManager.AdministrationManager.DiagnosticError(ex);
               return null;
           }
        }

        public bool HSM_SetMementoForUser(Memento memento, InfoUtente infoUt)
        {
            if (memento == null)
                return false;

            if (String.IsNullOrEmpty ( memento.Alias) )
                return false;

            if (String.IsNullOrEmpty(memento.Dominio))
                return false;
            
            //ConservazioneWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
            try
            {
                return ws.HSM_SetMementoForUser(infoUt, memento.Dominio, memento.Alias);
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }
    }

	/// <summary>
	/// 
	/// </summary>
    //public class DigitalSignManager
    //{
    //    protected static DocsPaWR.DocsPaWebService ws = new ProxyManager().getProxyDocsPa();

    //    public DigitalSignManager()
    //    {
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="page"></param>
    //    /// <returns></returns>
    //    public ConservazioneWA.DocsPaWR.FileDocumento GetSignedDocumentInfo(System.Web.UI.Page page)
    //    {
    //        return this.GetSignedDocumentInfo(page, ConservazioneWA.UIManager.FileManager.getSelectedFile());
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="page"></param>
    //    /// <param name="idDocumento"></param>
    //    /// <returns></returns>
    //    public ConservazioneWA.DocsPaWR.FileDocumento GetSignedDocumentInfo(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest)
    //    {
    //        //ConservazioneWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();

    //        ConservazioneWA.DocsPaWR.FileDocumento retValue = ws.DocumentoGetInfoFile(fileRequest, ConservazioneWA.UIManager.UserManager.GetInfoUser());

    //        if (retValue == null)
    //            throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

    //        return retValue;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="page"></param>
    //    /// <returns></returns>
    //    public ConservazioneWA.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page)
    //    {
    //        return this.GetSignedDocument(page, ConservazioneWA.UIManager.FileManager.getSelectedFile());
    //    }

    //    /// <summary>
    //    /// Reperimento del documento firmato
    //    /// </summary>
    //    /// <param name="page"></param>
    //    /// <param name="fileRequest"></param>
    //    /// <returns></returns>
    //    public ConservazioneWA.DocsPaWR.FileDocumento GetSignedDocument(System.Web.UI.Page page, DocsPaWR.FileRequest fileRequest, InfoUtente infoUt)
    //    {
    //        //ConservazioneWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();

    //        ConservazioneWA.DocsPaWR.FileDocumento retValue = ws.DocumentoGetFileFirmato(fileRequest, infoUt);

    //        if (retValue == null)
    //            throw new ApplicationException("Attenzione! il file non è visualizzabile.<br><br>Verificare:<br>&bull;&nbsp;l'esistenza del file in:<br>" + fileRequest.docServerLoc + fileRequest.path + fileRequest.fileName + "<br><br>&bull;&nbsp;la stringa di Impronta sulla base dati.");

    //        return retValue;
    //    }

    //    /// <summary>
    //    /// Reperimento scheda del documento da firmare digitalmente
    //    /// </summary>
    //    /// <param name="idDocumento"></param>
    //    /// <returns></returns>
    //    public DocsPaWR.SchedaDocumento GetSchedaDocumento(string idDocumento, InfoUtente infoUt)
    //    {
    //        if (string.IsNullOrEmpty(idDocumento))
    //        {
    //            return ConservazioneWA.UIManager.DocumentManager.getSelectedRecord();
    //        }
    //        else
    //        {
    //            //ConservazioneWA.DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
    //            return ws.DocumentoGetDettaglioDocumentoNoDataVista(infoUt, idDocumento, idDocumento);
    //        }
    //    }
    //}
}
