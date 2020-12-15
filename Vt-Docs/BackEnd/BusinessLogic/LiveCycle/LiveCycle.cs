using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveCycle;
using log4net;

namespace BusinessLogic.LiveCycle
{
    public class LiveCycle
    {
        private static ILog logger = LogManager.GetLogger(typeof(LiveCycle));

        public static DocsPaVO.LiveCycle.ProcessFormOutput processFormPdf(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LiveCycle.ProcessFormInput processFormInput)
        {
            try
            {
                return LCServices.processFormPdf(infoUtente, processFormInput);     
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle BusinessLogic - metodo: processFormPdf", e);
                return null;
            }
        
        }


        /// <summary>
        /// Funzione per l'invocazione del servizio offerto da Adobe LivCycle di conversione PDF sincrona
        /// </summary>
        /// <param name="docToConvert">Le informazioni sul documento da convertire</param>
        /// <returns>Le informazioni sul documento convertito</returns>
        public static DocsPaVO.documento.FileDocumento GeneratePDFInSyncMod(DocsPaVO.documento.FileDocumento docToConvert)
        {
            DocsPaVO.documento.FileDocumento retVal = null;
            //luluciani per ticket INC000000422959
            string ext=string.Empty;
            if (docToConvert != null && !string.IsNullOrEmpty(docToConvert.name) ) 
            {
                ext= System.IO.Path.GetExtension(docToConvert.name);

                logger.DebugFormat("ext file conversione sincrona {0}", ext);

                if (!string.IsNullOrEmpty(ext) && ext.ToLower().Equals(".htm"))
                {
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(docToConvert.name);

                    docToConvert.name = fileNameWithoutExtension + ".html";
                }

            }
            logger.DebugFormat("Inizio conversione sincrona del file {0}", docToConvert.name);
            try
            {
                // Richiamiamo il servizio per la conversione sincrona e restituiamo l'oggetto con le
                // informaizoni sul file convertito
                retVal = LCServices.generatePdfService(docToConvert);

                //ABBATANGELI LINEARIZZAZIONE
                retVal = BusinessLogic.Documenti.FileManager.LinearizzePDFContent(retVal);
                //FINE LINEARIZZAZIONE
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle BusinessLogic - metodo: GeneratePDFInSyncMod", e);
                return null;
            }
            
            logger.DebugFormat("File {0} convertito correttamente.", docToConvert.name);
            return retVal;
        }

        public static DocsPaVO.LiveCycle.ProcessFormOutput processBarcodeFormPdf(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.LiveCycle.ProcessFormInput processFormInput)
        {
            try
            {
                return LCServices.processBarcodeForm(infoUtente, processFormInput);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle BusinessLogic - metodo: processBarcodeFormPdf", e);
                return null;
            }

        }
    }
}
