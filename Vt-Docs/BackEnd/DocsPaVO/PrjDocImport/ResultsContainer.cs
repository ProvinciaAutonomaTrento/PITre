using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.PrjDocImport
{
    /// <summary>
    /// Questa classe rappresenta un contenitore di risultati
    /// </summary>
    [Serializable()]
    public class ResultsContainer
    {
        /// <summary>
        /// Creazione di un nuovo oggetto container di risultati
        /// </summary>
        public ResultsContainer()
        {
            // Creazione delle varie collezioni
            this.InDocument = new List<ImportResult>();
            this.OutDocument = new List<ImportResult>();
            this.OwnDocument = new List<ImportResult>();
            this.GrayDocument = new List<ImportResult>();
            this.Attachment =  new List<ImportResult>();
            this.General = new List<ImportResult>();
 
        }

        /// <summary>
        /// Report generale
        /// </summary>
        public List<ImportResult> General { get; set; }

        /// <summary>
        /// Report per i documenti in ingresso
        /// </summary>
        public List<ImportResult> InDocument { get; set; }

        /// <summary>
        /// Report per i documenti in uscita
        /// </summary>
        public List<ImportResult> OutDocument { get; set; }

        /// <summary>
        /// Report per i documenti interni
        /// </summary>
        public List<ImportResult> OwnDocument { get; set; }

        /// <summary>
        /// Report per i documenti grigi
        /// </summary>
        public List<ImportResult> GrayDocument { get; set; }

        /// <summary>
        /// Report per gli allegati
        /// </summary>
        public List<ImportResult> Attachment { get; set; }
                
    }

}
