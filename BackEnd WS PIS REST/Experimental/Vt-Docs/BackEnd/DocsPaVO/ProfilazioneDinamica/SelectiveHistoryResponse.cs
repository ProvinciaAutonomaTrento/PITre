using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.ProfilazioneDinamica
{
    /// <summary>
    /// Response relativa ai servizi di gestione dello storico dei campi profilati
    /// </summary>
    [Serializable()]
    public class SelectiveHistoryResponse
    {
        public SelectiveHistoryResponse()
        {
            this.Result = false;
        }

        /// <summary>
        /// Esito dell'applicazione di una azione
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Lista con le informazioni sullo stato di attivazione della storicizzazione 
        /// per i campi di una determinata tipologia
        /// </summary>
        public List<CustomObjHistoryState> Fields { get; set; }
    }
}
