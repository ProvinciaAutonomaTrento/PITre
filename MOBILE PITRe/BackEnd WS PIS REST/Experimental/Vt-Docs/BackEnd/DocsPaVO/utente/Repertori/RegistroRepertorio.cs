using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.ProfilazioneDinamica;

namespace DocsPaVO.utente.Repertori
{
    /// <summary>
    /// Questo oggetto contiene le informazioni su un registro di repertorio e
    /// sulla pianificazione delle sue stampe
    /// </summary>
    [Serializable()]
    public class RegistroRepertorio
    {
        /// <summary>
        /// Id di questo contatore
        /// </summary>
        public String CounterId { get; set; }

        /// <summary>
        /// Stato di abilitazione della tipologia documentale in cui è definito questo registro. (true se è attiva, false altrimenti)
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Descrizione del registro di repertorio
        /// </summary>
        public String CounterDescription { get; set; }

        /// <summary>
        /// Id della tipologia in cui è definito questo contatore
        /// </summary>
        public String TipologyId { get; set; }

        /// <summary>
        /// Descrizione della tipolgia documentale in cui è inserito questo registro di repertorio
        /// </summary>
        public String TipologyDescription { get; set; }

        /// <summary>
        /// Categoria documentale in cui è inserito questo contatore di repertorio
        /// </summary>
        public TipologyKind Tipology { get; set; }

        /// <summary>
        /// Descrizione della categoria documentale
        /// </summary>
        public String LongTipology
        {
            set { }
            get
            {
                String longDescription = "Documenti";

                switch (this.Tipology)
                {
                    case TipologyKind.D:
                        longDescription = "Documenti";
                        break;
                    case TipologyKind.F:
                        longDescription = "Fascicolo";
                        break;
                }

                return longDescription;
            }

        }

        /// <summary>
        /// Tipo di responsabile definito per questo registro di repertorio (Globale o Singolo per RF o Registro di AOO)
        /// </summary>
        public SettingsType Settings { get; set; }

        /// <summary>
        /// Id dell'amministrazione
        /// </summary>
        public String AdministrationId { get; set; }

        /// <summary>
        /// Impostazioni singole relative alle singole istanze di questo registro di repertorio
        /// </summary>
        public List<RegistroRepertorioSingleSettings> SingleSettings { get; set; }

        /// <summary>
        /// Categoria cui appartiene la tipologia dcocumentale in cui è contenuto il contatore
        /// </summary>
        public enum TipologyKind
        {
            /// <summary>
            /// Documenti
            /// </summary>
            D,
            /// <summary>
            /// Fascicoli
            /// </summary>
            F
        }

        /// <summary>
        /// Tipo di responsabile definito per questo registro di repertorio
        /// </summary>
        public enum SettingsType
        {
            /// <summary>
            /// Globale per tutte le eventuali istanze di questo registro
            /// </summary>
            G,
            /// <summary>
            /// Singolo per ogni istanza di questo registro
            /// </summary>
            S
        }
        
    }

}