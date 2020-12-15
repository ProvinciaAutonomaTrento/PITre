using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using DocsPAWA.DocsPaWR;
using log4net;

namespace DocsPAWA.utils
{
    public class DigitalSignManager
    {

    }

    public class RemoteDigitalSignManager
    {
        ILog logger = LogManager.GetLogger(typeof(RemoteDigitalSignManager));

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
            DocsPAWA.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
            try
            {
                return ws.HSM_RequestCertificateJson(AliasCertificato, DominioCertificato);
            }
            catch (Exception e)
            {
                // loggiamo un errore ?!?!?!?
                return null;
            }
        }

        public string HSM_OpenMultiSignSession(DocsPAWA.DocsPaWR.FileRequest[] fileRequestList, bool cofirma, bool timestamp, tipoFirma TipoFirma)
        {

            DocsPaWR.InfoUtente infoUt = DocsPAWA.UserManager.getInfoUtente();
            DocsPAWA.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
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


        public DocsPaWR.FirmaResult[] HSM_SignMultiSignSession(DocsPAWA.DocsPaWR.FileRequest[] fileRequestList, string MultiSignToken, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool cofirma)
        {
            DocsPaWR.InfoUtente infoUt = DocsPAWA.UserManager.getInfoUtente();
            DocsPAWA.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
            try
            {
                return ws.HSM_SignMultiSignSession(infoUt, fileRequestList, MultiSignToken, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato, cofirma);
            }
            catch (Exception e)
            {
                // loggiamo un errore ?!?!?!?
                // return false;
                return null;
            }
        }




        public bool HSM_Sign(FileRequest fr, bool cofirma, bool timestamp, tipoFirma TipoFirma, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato, bool ConvertPdf)
        {
            bool retval = false;

            InfoUtente infoUt = DocsPAWA.UserManager.getInfoUtente();
            DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();
            try
            {
                string tipoFirmaSTR = TipoFirma.ToString();
                FirmaResult firmaResult = new FirmaResult();
                retval = docsPaWS.HSM_Sign(infoUt, fr, cofirma, timestamp, tipoFirmaSTR, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato, ConvertPdf);
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                //return false;
                
                logger.Debug("Errore: " + ex.Message);
            }

            return retval;
        }

        public bool HSM_RequestOTP(String AliasCertificato, String DominioCertificato)
        {

            if (string.IsNullOrEmpty(AliasCertificato))
                return false;

            if (string.IsNullOrEmpty(DominioCertificato))
                return false;

            DocsPAWA.DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
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
            DocsPaWR.InfoUtente infoUt = DocsPAWA.UserManager.getInfoUtente();
            DocsPAWA.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
            try
            {
                Memento retval = new Memento();
                string[] resp = ws.HSM_GetMementoForUser(infoUt);

                if (resp.Length == 2)
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

        public bool HSM_SetMementoForUser(Memento memento)
        {
            if (memento == null)
                return false;

            if (String.IsNullOrEmpty(memento.Alias))
                return false;

            //if (String.IsNullOrEmpty(memento.Dominio))
            //    return false;

            DocsPaWR.InfoUtente infoUt = DocsPAWA.UserManager.getInfoUtente();
            DocsPAWA.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
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
}
