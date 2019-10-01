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
    public sealed class TipoIstanzaConservazione
    {
        /// <summary>
        /// 
        /// </summary>
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        public static TipoIstanzaConservazione[] Tipi
        {
            get
            {
                List<TipoIstanzaConservazione> tipi = new List<TipoIstanzaConservazione>();

                tipi.Add(new TipoIstanzaConservazione { Codice = CONSERVAZIONE_CONSOLIDATA, Descrizione = "Conservazione consolidata" });
                tipi.Add(new TipoIstanzaConservazione { Codice = CONSERVAZIONE_NON_CONSOLIDATA, Descrizione = "Conservazione non consolidata" });
                //MEV CS 1.4 - ESIBIZIONE
                //esibizione rimossa dalle tipologie di conservazione selezionabili in area cons
                //tipi.Add(new TipoIstanzaConservazione { Codice = ESIBIZIONE, Descrizione = "Esibizione" });
                //fine MEV CS 1.4 - ESIBIZIONE
                //tipi.Add(new TipoIstanzaConservazione { Codice = CONSERVAZIONE_INTERNA, Descrizione = "Conservazione interna" });

                return tipi.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public const string CONSERVAZIONE_CONSOLIDATA = "CONSERVAZIONE_CONSOLIDATA";

        /// <summary>
        /// 
        /// </summary>
        public const string CONSERVAZIONE_NON_CONSOLIDATA = "CONSERVAZIONE_NON_CONSOLIDATA";

        /// <summary>
        /// 
        /// </summary>
        public const string ESIBIZIONE = "ESIBIZIONE";

        /// <summary>
        /// 
        /// </summary>
        public const string CONSERVAZIONE_INTERNA = "CONSERVAZIONE_INTERNA";

    }
}
