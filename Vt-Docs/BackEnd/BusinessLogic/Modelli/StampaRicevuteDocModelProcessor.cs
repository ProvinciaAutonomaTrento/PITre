using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using log4net;

namespace BusinessLogic.Modelli
{
    /// <summary>
    /// Classe per l'elaborazione del modello per la stampa ricevuta protocollo
    /// </summary>
    public class StampaRicevuteDocModelProcessor : BaseDocModelProcessor 
    {
        private static ILog logger = LogManager.GetLogger(typeof(StampaRicevuteDocModelProcessor));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override DocsPaVO.Modelli.ModelResponse ProcessModel(DocsPaVO.Modelli.ModelRequest request)
        {
            DocsPaVO.Modelli.ModelResponse modelResponse = new DocsPaVO.Modelli.ModelResponse();

            try
            {
                modelResponse.DocumentId = request.DocumentId;
                
                // Reperimento del motore word processor installato sul client per l'elaborazione del modello
                modelResponse.ProcessorInfo = ModelliManager.GetCurrentModelProcessor(request.UserInfo);
                
                if (modelResponse.ProcessorInfo == null)
                    throw new ApplicationException("In amministrazione non risulta impostato alcun software per generare il documento");

                // Reperimento scheda documento
                DocsPaVO.documento.SchedaDocumento document = this.GetDocument(request.UserInfo, request.DocumentId);

                // Reperimento del path del modello per la stampa della ricevuta di protocollo
                string modelPath = this.GetModelPath(document);

                // Reperimento del file template modello
                if (File.Exists(modelPath))
                {
                    modelResponse.DocumentModel.ModelType = BaseDocModelProcessor.MODEL_STAMPA_RICEVUTA;
                    modelResponse.DocumentModel.File.FileName = Path.GetFileName(modelPath);
                    modelResponse.DocumentModel.File.Content = File.ReadAllBytes(modelPath);
                    modelResponse.DocumentModel.KeyValuePairs = this.GetModelKeyValuePairs(request.UserInfo, document);
                }
                else
                    throw new ApplicationException("File modello inesistente");
            }
            catch (Exception ex)
            {
                modelResponse = new DocsPaVO.Modelli.ModelResponse();
                modelResponse.Exception = ex.Message;

                logger.Debug(string.Format("Errore nel reperimento dei dati del modello: {0}", ex.Message));
            }

            return modelResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="pathModello"></param>
        protected override DocsPaVO.Modelli.ModelKeyValuePair[] GetModelKeyValuePairs(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            ArrayList listaOggetti = new ArrayList();

            this.FetchCommonFields(listaOggetti, infoUtente, schedaDocumento);

            List<DocsPaVO.Modelli.ModelKeyValuePair> list = new List<DocsPaVO.Modelli.ModelKeyValuePair>();

            foreach (string[] items in listaOggetti)
            {
                DocsPaVO.Modelli.ModelKeyValuePair pair = new DocsPaVO.Modelli.ModelKeyValuePair();
                pair.Key = items[0];
                pair.Value = items[1];
                list.Add(pair);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento del path del modello
        /// </summary>
        /// <param name="schedaDocumento"></param>
        /// <returns></returns>
        protected string GetModelPath(DocsPaVO.documento.SchedaDocumento document)
        {
            if (document.registro == null)
                throw new ApplicationException("Nessun registro impostato per il documento");

            DocsPaVO.amministrazione.OrgRegistro registro = new DocsPaVO.amministrazione.OrgRegistro();
            registro.Codice = document.registro.codRegistro;
            registro.CodiceAmministrazione = document.registro.codAmministrazione;

            // Reperimento del path del modello per la stampa ricevuta del registro
            string modelPath = Amministrazione.RegistroManager.GetPathModelloStampaRicevuta(registro);
            
            if (!File.Exists(modelPath))
                // Se non è stato impostato alcun modello per la stampa per il registro di protocollo,
                // viene reperito il modello predefinito
                modelPath = string.Format(@"{0}\report\StampaRic\Ricevuta.rtf", AppDomain.CurrentDomain.BaseDirectory);
            
            return modelPath;
        }
    }
}
