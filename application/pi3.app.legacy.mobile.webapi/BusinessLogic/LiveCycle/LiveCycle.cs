// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.Documenti;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.LiveCycle
{
    public class LiveCycle
    {
        private static ILogger logger = Log.ForContext(typeof(LiveCycle));

        /// <summary>
        /// Funzione per l'invocazione del servizio offerto da Adobe LivCycle di conversione PDF sincrona
        /// </summary>
        /// <param name="docToConvert">Le informazioni sul documento da convertire</param>
        /// <returns>Le informazioni sul documento convertito</returns>
        public static DocsPaVO.documento.FileDocumento GeneratePDFInSyncMod(DocsPaVO.documento.FileDocumento docToConvert)
        {
            DocsPaVO.documento.FileDocumento retVal = null;
            //luluciani per ticket INC000000422959
            string ext = string.Empty;
            if (docToConvert != null && !string.IsNullOrEmpty(docToConvert.name))
            {
                ext = System.IO.Path.GetExtension(docToConvert.name);

                logger.Debug("ext file conversione sincrona {0}", ext);

                if (!string.IsNullOrEmpty(ext) && ext.ToLower().Equals(".htm"))
                {
                    string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(docToConvert.name);

                    docToConvert.name = fileNameWithoutExtension + ".html";
                }

            }
            logger.Debug("Inizio conversione sincrona del file {0}", docToConvert.name);
            try
            {
                // Richiamiamo il servizio per la conversione sincrona e restituiamo l'oggetto con le
                // informaizoni sul file convertito
                bool sincrono = !string.IsNullOrEmpty(DocsPaVO.Settings.AppSettings.Instance.LIVECYCLE_SERVICE_GENERATE_PDF_SINCRONO) &&
                        DocsPaVO.Settings.AppSettings.Instance.LIVECYCLE_SERVICE_GENERATE_PDF_SINCRONO.Equals("1");
#if false
                if (sincrono)
                    retVal = LCServices.generatePdfServiceSincrono(docToConvert);
                else
                    retVal = LCServices.generatePdfService(docToConvert);

#endif
                //ABBATANGELI LINEARIZZAZIONE
                retVal = BusinessLogic.Documenti.FileManager.LinearizzePDFContent(retVal);
                //FINE LINEARIZZAZIONE
            }
            catch (Exception e)
            {
                logger.Debug("Errore in LiveCycle BusinessLogic - metodo: GeneratePDFInSyncMod", e);
                return null;
            }

            logger.Debug("File {0} convertito correttamente.", docToConvert.name);
            return retVal;
        }

    }
}
