using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using SAAdminTool.DocsPaWR;
using System.Collections.Generic;
using log4net;

namespace SAAdminTool.utils
{
    /// <summary>
    /// Questa classe fornisce funzionalità di utilità per l'importazione di documenti
    /// </summary>
    public class ImportDocumentsUtils
    {
        private static ILog logger = LogManager.GetLogger(typeof(ImportDocumentsUtils));
        public static ResultsContainer ImportDocuments(byte[] content,
           string fileName,
           string serverPath,
           InfoUtente userInfo,
           Ruolo userRole)
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la profilazione del documento
            bool isProfilationRequired = false;

            //Boolean.TryParse(
            //    ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"],
            //    out isProfilationRequired);
            string idAmm = userInfo.idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                isProfilationRequired = true;
            else
                isProfilationRequired = false;


            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;

            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
            if (string.IsNullOrEmpty(valoreChiaveFasc))
                valoreChiaveFasc = "false";
            Boolean.TryParse(
                valoreChiaveFasc,
                out isClassificationRequired);

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportDocuments(
                    content,
                    fileName,
                    serverPath,
                    userInfo,
                    userRole,
                    isProfilationRequired,
                    isClassificationRequired,
                    Utils.getAbilitazioneSmistamento() == "1",
                    importazionePregressiAbilitata());
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static DocumentRowDataContainer ReadDocumentDataFromExcelFile(
            byte[] content,
            InfoUtente userInfo,
            Ruolo role,
            bool isStampaUnione,
            out string error)
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            try
            {
                logger.Debug("Reperimento informazioni documenti");

                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ReadDocumentDataFromExcelFile(
                    content,
                    userInfo,
                    role,
                    importazionePregressiAbilitata(),
                    isStampaUnione,
                    out error);

                logger.Debug("Informazioni sui documenti reperite.");

            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static ImportResult[] ImportDocumentsFromArray(
            DocumentRowData[] documentToImport,
            ProtoType protoType,
            string serverPath,
            InfoUtente userInfo,
            Ruolo role,
            ref ResultsContainer resultsContainer)
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la profilazione del documento
            bool isProfilationRequired = false;

            //Boolean.TryParse(
            //    ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"],
            //    out isProfilationRequired);
            string idAmm = userInfo.idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                isProfilationRequired = true;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
            if (string.IsNullOrEmpty(valoreChiaveFasc))
                valoreChiaveFasc = "false";

            Boolean.TryParse(
                valoreChiaveFasc,
                out isClassificationRequired);

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportDocumentsFromArray(
                    documentToImport,
                    protoType,
                    serverPath,
                    userInfo,
                    role,
                    isProfilationRequired,
                    isClassificationRequired,
                    Utils.getAbilitazioneSmistamento() == "1",
                    ref resultsContainer,
                    importazionePregressiAbilitata());
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static ImportResult ImportDocument(
            DocumentRowData documentToImport,
            ProtoType protoType,
            string serverPath,
            InfoUtente userInfo,
            Ruolo role,
            ref ResultsContainer resultsContainer)
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la profilazione del documento
            bool isProfilationRequired = false;

            //Boolean.TryParse(
            //    ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"],
            //    out isProfilationRequired);
            string idAmm = userInfo.idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                isProfilationRequired = true;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
            if (string.IsNullOrEmpty(valoreChiaveFasc))
                valoreChiaveFasc = "false";

            Boolean.TryParse(
                valoreChiaveFasc,
                out isClassificationRequired);

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportDocument(
                    documentToImport,
                    protoType,
                    serverPath,
                    userInfo,
                    role,
                    isProfilationRequired,
                    isClassificationRequired,
                    Utils.getAbilitazioneSmistamento() == "1",
                    ref resultsContainer,
                    importazionePregressiAbilitata());
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static ImportResult ImportAndAcquireDocument(
            DocumentRowData documentToImport,
            ProtoType protoType,
            string serverPath,
            InfoUtente userInfo,
            Ruolo role,
            ref ResultsContainer resultsContainer)
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la profilazione del documento
            bool isProfilationRequired = false;

            //Boolean.TryParse(
            //    ConfigurationManager.AppSettings["TIPO_ATTO_REQUIRED"],
            //    out isProfilationRequired);
            string idAmm = userInfo.idAmministrazione;
            if (DocumentManager.GetTipoDocObbl(idAmm).Equals("1"))
                isProfilationRequired = true;

            // Lettura del valore di configurazione utilizzato per indicare
            // se è obbligatoria la classificazione del documento
            bool isClassificationRequired = false;
            string valoreChiaveFasc = utils.InitConfigurationKeys.GetValue(UserManager.getInfoUtente().idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
            if (string.IsNullOrEmpty(valoreChiaveFasc))
                valoreChiaveFasc = "false";

            Boolean.TryParse(
                valoreChiaveFasc,
                out isClassificationRequired);

            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.ImportAndAcquireDocument(
                    documentToImport,
                    protoType,
                    serverPath,
                    userInfo,
                    role,
                    isProfilationRequired,
                    isClassificationRequired,
                    Utils.getAbilitazioneSmistamento() == "1",
                    ref resultsContainer,
                    importazionePregressiAbilitata());
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        public static byte[] CreateZipFromReport(ResultsContainer report,InfoUtente userInfo)
        {
            // Instanziazione del web service
            DocsPaWebService ws = new DocsPaWebService();

            // Impostazione del timeout ad infinito
            ws.Timeout = System.Threading.Timeout.Infinite;
            try
            {
                // Chiamata del webservice per l'importazione dei documenti da
                // foglio Excel e restituzione della lista dei risultati
                return ws.CreateZipFromReport(report, userInfo);
            }
            catch (Exception e)
            {
                throw new Exception("Ricevuto errore dal servizio.");
            }
        }

        /// <summary>
        /// Funzione per il reperimento del massimo numero di documenti importabili
        /// </summary>
        /// <param name="userInfo">Le informazioni sull'utente</param>
        /// <returns>Il numero massimo di documenti che è possibile importare</returns>
        public static int GetMaxDocumentsNumber(InfoUtente userInfo)
        {
            // Il valore da restituire
            int toReturn;

            // Valore utilizzato per indicare se il reperimento è andato a buon fine
            bool success;

            // Reperimento del valore
            success = Int32.TryParse(ConfigurationManager.AppSettings["MAX_DOC_CAN_IMPORT"], out toReturn);
            
            // Se non è stato reperito un valore, viene restituita un'eccezione
            if (!success)
                throw new Exception("Errore durante il reperimento del massimo numero di documenti che è possibile importare. Contattare l'amministratore.");

            // Restituzione del valore
            return toReturn;

        }

        /*
        /// <summary>
        /// Funzione per il reperimento del valore associato al flag ImportDocuments_CdC 
        /// </summary>
        /// <returns>True se bisogna considerare l'importazione documenti per Corte dei Conti</returns>
        public static bool IsEnabledPregressi()
        {
            // Il valore da restituire
            bool toReturn = false;

            // Lettura del valore di configurazione
            Boolean.TryParse(
                ConfigurationManager.AppSettings["IMPORTDOCUMENTS_CDC"],
                out toReturn);

          
            // Restituzione del risultato
            return toReturn;

        }*/

        /// <summary>
        /// Funzione per il reperimento della microfunzione IMP_DOC_MASSIVA_PREG nel ruolo
        /// </summary>
        /// <returns>True se bisogna considerare l'importazione documenti pregressi</returns>
        public static bool importazionePregressiAbilitata()
        {

            DocsPaWR.Funzione[] funz = UserManager.getRuolo().funzioni;
            foreach (DocsPaWR.Funzione f in funz)//verifico se attiva la microfunz IMP_DOC_MASSIVA_PREG
            {
                if (f.codice.Equals("IMP_DOC_MASSIVA_PREG"))
                    return true;
            }



            return false;
        }

    }
}
