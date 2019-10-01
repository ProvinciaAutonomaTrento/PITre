using System;
using DocsPaVO.Report;

namespace DocsPaVO.utente
{
    
    /// <summary>
    /// Item che descrive un istante della storia di un ruolo
    /// </summary>
    [Serializable()]
    public class RoleHistoryItem
    {
        /// <summary>
        /// Enumerazione delle possibili azioni registrate dalla storia
        /// </summary>
        [Serializable()]
        public enum AdmittedHistoryAction
        {
            /// <summary>
            /// Creazione del ruolo
            /// </summary>
            C,
            /// <summary>
            /// Storicizzazione del ruolo
            /// </summary>
            S,
            /// <summary>
            /// Modifica del ruolo
            /// </summary>
            M
        }

        /// <summary>
        /// Id corr globali del ruolo cui si riferisce l'istantanea
        /// </summary>
        public int OriginalCorrId { get; set; }

        /// <summary>
        /// Azione che ha portato alla creazione di una istantanea
        /// </summary>
        public AdmittedHistoryAction HistoryAction { get; set; }

        [PropertyToExport(Name = "Azione", Type = typeof(String))]
        public String ExtendedHistoryAction
        {
            get
            {
                String retVal = String.Empty;
                switch (this.HistoryAction)
                {
                    case AdmittedHistoryAction.C:
                        retVal = "Creazione";
                        break;
                    case AdmittedHistoryAction.S:
                        retVal = "Storicizzazione";
                        break;
                    case AdmittedHistoryAction.M:
                        retVal = "Modifica";
                        break;
                    
                }

                return retVal;
            }
        }

        /// <summary>
        /// Data in cui è stata compiuta l'azione
        /// </summary>
        [PropertyToExport(Name="Data Azione", Type=typeof(DateTime))]
        public DateTime ActionDate { get; set; }

        /// <summary>
        /// Descrizione del ruolo a seguito dell'applicazione dell'azione
        /// </summary>
        [PropertyToExport(Name="Descrizione Ruolo", Type=typeof(String))]
        public String RoleDescription { get; set; }

        /// <summary>
        /// Descrizione della UO cui apparteneva il ruolo nel momento successivo al compimento dell'azione
        /// </summary>
        [PropertyToExport(Name="Descrizione UO", Type=typeof(String))]
        public String UoDescription { get; set; }

        /// <summary>
        /// Descrizione del tipo ruolo cui apparteneva il ruolo nel momento successivo al compimento dell'azione
        /// </summary>
        [PropertyToExport(Name="Descrizione Tipo Ruolo", Type=typeof(String))]
        public String RoleTypeDescription { get; set; }

        /// <summary>
        /// Id del ruolo al momento in cui è stata creata l'istantanea
        /// </summary>
        public String RoleCorrGlobId { get; set; }

        /// <summary>
        /// Due istantanee sono uguali se fanno riferimento allo stesso ruolo, hanno stessa data di applicazione,
        /// e stessa azione
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            RoleHistoryItem item = obj as RoleHistoryItem;

            return item != null && item.OriginalCorrId == this.OriginalCorrId &&
                item.ActionDate.Equals(this.ActionDate) &&
                item.HistoryAction == this.HistoryAction;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + this.OriginalCorrId.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", this.HistoryAction, this.ActionDate);
        }
    }
}
