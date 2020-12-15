using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaConservazione
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    class StatoIstanzaEsibizione
    {
        /// <summary>
        /// Codice dello stato
        /// </summary>
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dello stato
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public static StatoIstanzaEsibizione[] Stati
        {
            get
            {
                List<StatoIstanzaEsibizione> stati = new List<StatoIstanzaEsibizione>();

                stati.Add(new StatoIstanzaEsibizione { Codice = StatoIstanzaEsibizione.NUOVA, Descrizione = "Nuova" });
                stati.Add(new StatoIstanzaEsibizione { Codice = StatoIstanzaEsibizione.CHIUSA, Descrizione = "Chiusa" });
                stati.Add(new StatoIstanzaEsibizione { Codice = StatoIstanzaEsibizione.RIFIUTATA, Descrizione = "Rifiutata" });
                stati.Add(new StatoIstanzaEsibizione { Codice = StatoIstanzaEsibizione.IN_ATTESA_DI_CERTIFICAZIONE, Descrizione = "In attesa di certificazione" });
                stati.Add(new StatoIstanzaEsibizione { Codice = StatoIstanzaEsibizione.IN_TRANSIZIONE, Descrizione = "In Transizione" });
                
                return stati.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public const string NUOVA = "N";

        /// <summary>
        /// 
        /// </summary>
        public const string CHIUSA = "C";

        /// <summary>
        /// 
        /// </summary>
        public const string RIFIUTATA = "R";

        /// <summary>
        /// 
        /// </summary>
        public const string IN_ATTESA_DI_CERTIFICAZIONE = "I";

        /// <summary>
        /// Stato in cui si trova dopo la richiesta di certificazione e prima della chiusura o del rifiuto.
        /// </summary>
        public const string IN_TRANSIZIONE = "T";

    }
}
