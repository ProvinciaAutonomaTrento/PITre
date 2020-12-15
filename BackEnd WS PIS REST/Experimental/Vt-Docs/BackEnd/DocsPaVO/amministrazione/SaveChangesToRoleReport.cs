using System;
using DocsPaVO.Report;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Esito dell'operazione relativa al salvataggio di modifiche apportate ad un ruolo
    /// </summary>
    [Serializable()]
    public class SaveChangesToRoleReport
    {
        /// <summary>
        /// Enumerazione dei possibili risultati derivanti dall'esecuzione di una fase 
        /// della procedura di un ruolo
        /// </summary>
        public enum SaveChangesToRoleReportResult
        {
            OK,
            KO,
            Waiting
        }

        /// <summary>
        /// Descrizione dell'operazione
        /// </summary>
        [PropertyToExport(Name = "Descrizione", Type = typeof(String))]
        public String Description { get; set; }

        /// <summary>
        /// Esito del passo attualmente in esecuzione
        /// </summary>
        private SaveChangesToRoleReportResult result = SaveChangesToRoleReportResult.Waiting;
        [PropertyToExport(Name="Esito", Type=typeof(String))]
        public SaveChangesToRoleReportResult Result
        {
            get
            {
                return this.result;
            }

            set
            {
                this.result = value;
                switch (value)
                {
                    case SaveChangesToRoleReportResult.OK:
                        this.ImageUrl = "Images/completed.jpg";
                        break;
                    case SaveChangesToRoleReportResult.KO:
                        this.ImageUrl = "Images/failed.jpg";
                        break;
                    case SaveChangesToRoleReportResult.Waiting:
                        this.ImageUrl = "Images/wait.gif";
                        break;
                }
            }
        }

        /// <summary>
        /// Url dell'immagine da mostrare (Attesa, Ok, Fallimento)
        /// </summary>
        public String ImageUrl { get; set; }
    }
}
