using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Web.UI;
using System.Threading;
using NttDataWA.UserControls;
using System.Collections;
using System.Configuration;
using System.Data;

namespace NttDataWA.UIManager
{
    public class CheckMailboxManager
    {        
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        /// <summary>
        /// Metodo per la consultazione della casella istituzionale.
        /// </summary>
        /// <returns></returns>
        public static bool CheckMailBox(string serverName, DocsPaWR.Registro reg, DocsPaWR.Utente user, DocsPaWR.Ruolo role)
        {
            try
            {
                return docsPaWS.CheckMailBox(serverName, reg, user, role);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public delegate bool CheckMailBoxDelegate(string serverName, DocsPaWR.Registro reg, DocsPaWR.Utente user, DocsPaWR.Ruolo role);

        /// <summary>
        /// Restituisce le informazioni sul report associato alla mailbox da consultare.
        /// </summary>
        /// <returns></returns>
        public static MailAccountCheckResponse InfoReportMailbox(string idCheckMailbox)
        {
            try
            {
                return docsPaWS.InfoReportMailbox(idCheckMailbox);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Elimina le informazioni sul report della mailbox appena consultato
        /// </summary>
        /// <returns></returns>
        public static bool RemoveReportMailbox(string idCheckMailbox)
        {
            try
            {
            return docsPaWS.RemoveReportMailbox(idCheckMailbox);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        

        /// <summary>
        /// Restituisce lo stato sulle n caselle passate come parametro di input
        /// </summary>
        /// <returns></returns>
        public static List<DocsPaWR.InfoCheckMailbox> InfoCheckMailbox(List<string> listEmails)
        {
            try
            {
                return docsPaWS.InfoCheckMailbox(listEmails.ToArray()).ToList();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }


        /// <summary>
        /// Funzione per la generazione dell'anagrafica dei report
        /// </summary>
        /// <param name="contextName">Nome del contesto</param>
        /// <returns>Anagrafica dei report</returns>
        public static ReportMetadata[] GetReportRegistry(String contextName)
        {
            PrintReportResponse response = null;

            try
            {
                response = docsPaWS.GetReportRegistry(contextName);
            }
            catch (Exception e)
            {
               SoapExceptionParser.ThrowOriginalException(e);

            }

            return response.ReportMetadata;

        }

        /// <summary>
        /// Funzione per la generazione di un report
        /// </summary>
        /// <param name="request">Informazioni sul report da generare</param>
        /// <returns>Report prodotto</returns>
        public static FileDocumento GenerateReport(PrintReportRequest request)
        {
            PrintReportResponse response = null;

            try
            {
                response = docsPaWS.GenerateReport(request);
            }
            catch (Exception e)
            {
                SoapExceptionParser.ThrowOriginalException(e);
            }

            return response.Document;

        }

        public static SchedaDocumento DocumentoAddDocGrigia(SchedaDocumento schedaDocumento, InfoUtente infoUtente, Ruolo ruolo)
        {
            try
            {
                return docsPaWS.DocumentoAddDocGrigia(schedaDocumento, infoUtente, ruolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static FileRequest DocumentoPutFile(FileRequest fileRequest, FileDocumento fileDocument, InfoUtente infoUtente)
        {
            try
            {
                return docsPaWS.DocumentoPutFile(fileRequest, fileDocument, infoUtente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}