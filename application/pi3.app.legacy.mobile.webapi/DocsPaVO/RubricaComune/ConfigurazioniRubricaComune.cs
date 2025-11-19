// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DocsPaVO.RubricaComune
{
    /// <summary>
    /// Oggetto che mantiene le configurazioni docspa
    /// per la gestione del sistema esterno rubrica comune
    /// </summary>
    [Serializable()]
    [DataContract]
    public class ConfigurazioniRubricaComune
    {
        /// <summary>
        /// Flag, indica se la gestione è abilitata o meno
        /// per l'utente o per il contesto corrente
        /// </summary>
        [DataMember]
        public bool GestioneAbilitata { get; set; }

        /// <summary>
        /// Flag, indica se la gestione della rubrica comune 
        /// tool di amministrazione è abilitata o meno per il contesto corrente
        /// </summary>
        [DataMember]
        public bool GestioneAmministrazioneAbilitata { get; set; }

        /// <summary>
        /// BaseUrl in cui sono disponibili i servizi web
        /// esposti dal sistema rubrica comune
        /// </summary>
        [DataMember]
        public string ServiceRoot { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string SuperUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string SuperUserPwd { get; set; }
    }
}
