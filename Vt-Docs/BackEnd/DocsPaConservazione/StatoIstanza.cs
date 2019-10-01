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
    public sealed class StatoIstanza
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
        public static StatoIstanza[] Stati
        {
            get
            {
                List<StatoIstanza> stati = new List<StatoIstanza>();

                stati.Add(new StatoIstanza { Codice = StatoIstanza.DA_INVIARE, Descrizione = "Da inviare" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.INVIATA, Descrizione = "Inviata" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.RIFIUTATA, Descrizione = "Rifiutata" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.IN_LAVORAZIONE, Descrizione = "In lavorazione" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.FIRMATA, Descrizione = "Firmata" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.CONSERVATA, Descrizione = "Conservata" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.CHIUSA, Descrizione = "Chiusa" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.IN_TRANSIZIONE, Descrizione = "In Transizione" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.IN_FASE_VERIFICA, Descrizione = "In fase di verifica" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.IN_VERIFICA, Descrizione = "Verifica formati in corso" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.VERIFICATA, Descrizione = "Verificata" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.IN_CONVERSIONE, Descrizione = "In conversione" });
                stati.Add(new StatoIstanza { Codice = StatoIstanza.ERRORE_CONVERSIONE, Descrizione = "In Errore di conversione formati" });
                return stati.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public const string DA_INVIARE = "N";

        /// <summary>
        /// 
        /// </summary>
        public const string INVIATA = "I";

        /// <summary>
        /// 
        /// </summary>
        public const string RIFIUTATA = "R";

        /// <summary>
        /// 
        /// </summary>
        public const string IN_LAVORAZIONE = "L";

        /// <summary>
        /// 
        /// </summary>
        public const string FIRMATA = "F";

        /// <summary>
        /// 
        /// </summary>
        public const string CONSERVATA = "V";

        /// <summary>
        /// 
        /// </summary>
        public const string CHIUSA = "C";

        /// <summary>
        /// Stato in cui si trova dopo la verifica leggibilità e prima della chiusura. Per il caricamento asincrono.
        /// </summary>
        public const string IN_TRANSIZIONE = "T";

        /// <summary>
        /// Stato in cui si trova durante le verifiche automatiche.
        /// </summary>
        public const string IN_FASE_VERIFICA = "Q";

        /// <summary>
        /// Stato transitorio in cui si trova un'istanza durante le verifiche asincrone  
        /// </summary>
        public const string IN_VERIFICA = "A";

        /// <summary>
        /// Stato  in cui si trova un'istanza alla fine delle verifiche asincrone  
        /// </summary>
        public const string VERIFICATA = "B";

        /// <summary>
        /// Stato transitorio in cui si trova un'istanza durante la conversione asincrona degli item e degli allegati  
        /// </summary>
        public const string IN_CONVERSIONE = "Y";

        /// <summary>
        /// Stato in cui si trova un'istanza nel caso in cui si verifichi un errore durante la conversione asincrona 
        /// degli item e degli allegati
        /// </summary>
        public const string ERRORE_CONVERSIONE = "Z";
    }
}
