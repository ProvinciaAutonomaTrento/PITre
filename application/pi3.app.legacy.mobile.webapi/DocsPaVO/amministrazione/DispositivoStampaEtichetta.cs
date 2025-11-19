// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.amministrazione
{
    /// <summary>
    /// Classe per mantenere le informazioni relative ai dispositivi di stampa disponibili nel sistema
    /// </summary>
    [Serializable()]
    [DataContract]
    public class DispositivoStampaEtichetta
    {
        /// <summary>
        /// Identificativo univoco del dispositivo
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Codice del dispositivo
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Descrizione del dispositivo
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}
