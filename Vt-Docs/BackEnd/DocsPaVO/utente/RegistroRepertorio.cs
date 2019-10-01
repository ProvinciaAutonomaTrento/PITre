using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.ProfilazioneDinamica;

namespace DocsPaVO.utente
{
    /// <summary>
    /// Questo oggetto contiene le informazioni su un registro di repertorio e
    /// sulla pianificazione delle sue stampe
    /// </summary>
    [Serializable()]
    public class RegistroRepertorio
    {
        /// <summary>
        /// Id del registro di questo registro di repertorio
        /// </summary>
        public String RegistryId { get; set; }

        /// <summary>
        /// Descrizione del registro di repertorio
        /// </summary>
        public String RegistryDescription { get; set; }

        /// <summary>
        /// Descrizione della tipolgia documentale in cui è inserito questo registro di repertorio
        /// </summary>
        public String TipologyDescription { get; set; }

        /// <summary>
        /// Id del ruolo responsabile di questo registro di repertorio
        /// </summary>
        public String RoleId { get; set; }

        /// <summary>
        /// Frequenza della stampa automatica
        /// </summary>
        public Frequency PrintFrequency { get; set; }

        /// <summary>
        /// Descrizione lunga della frequenza di stampa
        /// </summary>
        public String LongPrintFrequency
        {
            set { }
            get
            {
                String longDescription = "Disattiva";
                switch (this.PrintFrequency)
                {
                    case Frequency.N:
                        longDescription = "Disattivata";
                        break;
                    case Frequency.D:
                        longDescription = "Giornaliera";
                        break;
                    case Frequency.W:
                        longDescription = "Settimanale";
                        break;
                    case Frequency.FD:
                        longDescription = "Quindicinale";
                        break;
                    case Frequency.M:
                        longDescription = "Mensile";
                        break;

                }

                return longDescription;
            }
        }

        /// <summary>
        /// Categoria documentale in cui è inserito questo contatore di repertorio
        /// </summary>
        public TipologyKind TipologyType { get; set; }

        /// <summary>
        /// Descrizione della categoria documentale
        /// </summary>
        public String LongTipologyType
        {
            set { }
            get
            {
                String longDescription = "Documenti";

                switch (this.TipologyType)
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
        /// Data in cui il processo di stampa automatica per questo registro di repertorio
        /// deve partire
        /// </summary>
        public DateTime DateAutomaticPrintStart { get; set; }

        /// <summary>
        /// Data limite dopo la quale il processo di stampa automatica per questo registro deve
        /// terminare
        /// </summary>
        public DateTime DateAutomaticPrintFinish { get; set; }

        /// <summary>
        /// Data prevosta per la prossima stampa automatica
        /// </summary>
        public DateTime DateNextAutomaticPrint {get; set; }

        /// <summary>
        /// Data dell'ultima stampa automatica
        /// </summary>
        public DateTime DateLastAutomaticPrint { get; set; }

        /// <summary>
        /// Id dell'amministrazione
        /// </summary>
        public String AdministrationId { get; set; }

        /// <summary>
        /// Enumerazione delle possibili frequenze di stampa automatica
        /// </summary>
        public enum Frequency
        {
            /// <summary>
            /// Stampa automatica
            /// </summary>
            N,
            /// <summary>
            /// Stampa giornaliera
            /// </summary>
            D,
            /// <summary>
            /// Stampa settimanale
            /// </summary>
            W,
            /// <summary>
            /// Stampa quindicinale
            /// </summary>
            FD,
            /// <summary>
            /// Stampa mensile
            /// </summary>
            M
        }

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
        
    }

}
