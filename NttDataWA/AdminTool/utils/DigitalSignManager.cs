using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using SAAdminTool.DocsPaWR;
using log4net;

namespace SAAdminTool.utils
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
            SAAdminTool.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
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

        public string HSM_OpenMultiSignSession(SAAdminTool.DocsPaWR.FileRequest[] fileRequestList, bool cofirma, bool timestamp, tipoFirma TipoFirma)
        {

            DocsPaWR.InfoUtente infoUt = SAAdminTool.UserManager.getInfoUtente();
            SAAdminTool.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
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


        public DocsPaWR.FirmaResult[] HSM_SignMultiSignSession(SAAdminTool.DocsPaWR.FileRequest[] fileRequestList, string MultiSignToken, String AliasCertificato, String DominioCertificato, String OtpFirma, String PinCertificato)
        {
            DocsPaWR.InfoUtente infoUt = SAAdminTool.UserManager.getInfoUtente();
            SAAdminTool.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
            try
            {
                return ws.HSM_SignMultiSignSession(infoUt, fileRequestList, MultiSignToken, AliasCertificato, DominioCertificato, OtpFirma, PinCertificato);
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

            InfoUtente infoUt = SAAdminTool.UserManager.getInfoUtente();
            DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();
            try
            {
                string tipoFirmaSTR = TipoFirma.ToString();
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

            SAAdminTool.DocsPaWR.DocsPaWebService ws = new SAAdminTool.DocsPaWR.DocsPaWebService();
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
            DocsPaWR.InfoUtente infoUt = SAAdminTool.UserManager.getInfoUtente();
            SAAdminTool.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
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

            DocsPaWR.InfoUtente infoUt = SAAdminTool.UserManager.getInfoUtente();
            SAAdminTool.DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
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