using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.Repertori
{
    /// <summary>
    /// Questa classe rappresenta le impostazioni relative ad un determinato registro di repertorio
    /// </summary>
    [Serializable()]
    public class RegistroRepertorioSingleSettings
    {
        /// <summary>
        /// Id dell'eventuale registro cui si riferiscono queste impostazioni (Registro di AOO).
        /// </summary>
        public String RegistryId { get; set; }

        /// <summary>
        /// Id dell'eventuale RF cui si riferiscono queste impostaizoni
        /// </summary>
        public String RFId { get; set; }

        /// <summary>
        /// Descrizione del registro o RF cui si riferiscono queste impostazioni
        /// </summary>
        public String RegistryOrRfDescription { get; set; }

        private String _roleRespId = null;
        /// <summary>
        /// Id del ruolo responsabile di un registro di repertorio per un particolare Registro o RF
        /// </summary>
        public String RoleRespId
        {
            get
            {
                return this._roleRespId;
            }
            set
            {
                int temp = 0;
                if (Int32.TryParse(value, out temp))
                    this._roleRespId = temp.ToString();

            }
        }

        /// <summary>
        /// Diritti da assegnare al ruolo responsabile del repertorio
        /// </summary>
        public ResponsableRight RoleRespRight { get; set; }

        private String _printerRoleId = null;
        /// <summary>
        /// Id del ruolo responsabile delle stampe
        /// </summary>
        public String PrinterRoleRespId
        {
            get
            {
                return this._printerRoleId;
            }
            set
            {
                int temp = 0;
                if (Int32.TryParse(value, out temp))
                    this._printerRoleId = temp.ToString();
            }
        }

        String _printerUserId = null;
        /// <summary>
        /// Id dell'utente responsabile delle stampe
        /// </summary>
        public String PrinterUserRespId
        {
            get
            {
                return this._printerUserId;
            }
            set
            {
                int temp = 0;
                if (Int32.TryParse(value, out temp))
                    this._printerUserId = temp.ToString();
            }
        }

        /// <summary>
        /// Descrizione estesa del ruolo e dell'utente responsabile del registro
        /// </summary>
        public String RoleAndUserDescription { get; set; }

        /// <summary>
        /// Stato del contatore di repertorio
        /// </summary>
        public RepertorioState CounterState { get; set; }

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
        public DateTime DateNextAutomaticPrint { get; set; }

        /// <summary>
        /// Data dell'ultima stampa
        /// </summary>
        public DateTime DateLastPrint { get; set; }

        /// <summary>
        /// Ultimo numero di repertorio stampato
        /// </summary>
        public int LastPrintedNumber { get; set; }

        /// <summary>
        /// Enumerazione delle possibili frequenze di stampa automatica
        /// </summary>
        public enum Frequency
        {
            /// <summary>
            /// Stampa disattivata
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
        /// Enumerazione dei possibili stati in cui si può trovare un repertorio
        /// </summary>
        public enum RepertorioState
        {
            /// <summary>
            /// Chiuso
            /// </summary>
            C,
            /// <summary>
            /// Aperto
            /// </summary>
            O
        }

        /// <summary>
        /// Enumerazione dei possibili diritti assegnabili ad un ruolo responsabile
        /// </summary>
        public enum ResponsableRight
        {
            /// <summary>
            /// Lettura
            /// </summary>
            R,
            /// <summary>
            /// Lettura / Scrittura
            /// </summary>
            RW
        }
    }
}
