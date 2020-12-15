using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Classe per mantenere le informazioni relative ai dispositivi di stampa disponibili nel sistema
    /// </summary>
    [Serializable()]
    public class DispositivoStampaEtichetta
    {
        /// <summary>
        /// Identificativo univoco del dispositivo
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Codice del dispositivo
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Descrizione del dispositivo
        /// </summary>
        public string Description { get; set; }
    }
}
