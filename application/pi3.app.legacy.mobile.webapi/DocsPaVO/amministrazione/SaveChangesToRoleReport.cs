// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Runtime.Serialization;
using DocsPaVO.Report;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Esito dell'operazione relativa al salvataggio di modifiche apportate ad un ruolo
    /// </summary>
    [Serializable()]
    [DataContract]
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
        [DataMember]
        public String Description { get; set; }

        /// <summary>
        /// Esito del passo attualmente in esecuzione
        /// </summary>
        private SaveChangesToRoleReportResult result = SaveChangesToRoleReportResult.Waiting;
        [PropertyToExport(Name="Esito", Type=typeof(String))]
        [DataMember]
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
        [DataMember]
        public String ImageUrl { get; set; }
    }
}
