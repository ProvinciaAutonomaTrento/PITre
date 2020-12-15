using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Modelli
{
    /// <summary>
    /// Interfaccia per l'elaborazione dei dati sui modelli
    /// </summary>
    public interface IModelProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DocsPaVO.Modelli.ModelResponse ProcessModel(DocsPaVO.Modelli.ModelRequest request);
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ModelProcessor : IModelProcessor
    {
        /// <summary>
        /// 
        /// </summary>
        public ModelProcessor() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DocsPaVO.Modelli.ModelResponse ProcessModel(DocsPaVO.Modelli.ModelRequest request)
        {
            DocsPaVO.Modelli.ModelResponse response = null;

            try
            {
                IModelProcessor processor = this.CreateInstance(request.ModelType);

                //return processor.ProcessModel(request);
                response = processor.ProcessModel(request);
            }
            catch (Exception ex)
            {
                response = new DocsPaVO.Modelli.ModelResponse
                {
                    DocumentId = request.DocumentId,
                    Exception = ex.Message,
                };
            }

            return response;
        }

        /// <summary>
        /// Creazione istanza di un ModelProcessor in base al tipo di modello richiesto
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private IModelProcessor CreateInstance(string modelType)
        {
            if (modelType.Equals(BaseDocModelProcessor.MODEL_VERSION_1) ||
                modelType.Equals(BaseDocModelProcessor.MODEL_VERSION_2) ||
                modelType.StartsWith(BaseDocModelProcessor.MODEL_ATTATCHMENT))
            {
                return new DocModelProcessor();
            }
            else if (modelType.Equals(BaseDocModelProcessor.MODEL_STAMPA_RICEVUTA))
            {
                return new StampaRicevuteDocModelProcessor();
            }
            else
                throw new ApplicationException(string.Format("Modello '{0}' non supportato", modelType));
        }
    }
}
