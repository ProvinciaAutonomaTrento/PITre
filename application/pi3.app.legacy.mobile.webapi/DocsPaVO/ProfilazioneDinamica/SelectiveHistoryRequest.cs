// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DocsPaVO.ProfilazioneDinamica
{
    /// <summary>
    /// Request relativa ai servizi di gestione dello storico dei campi profilati
    /// </summary>
    [Serializable()]
    [DataContract]
    public class SelectiveHistoryRequest
    {
        /// <summary>
        /// Lista delle informazioni sullo stato di abilitazione dello storico relativamente 
        /// ai campi di una tipologia
        /// </summary>
        [DataMember]
        public List<CustomObjHistoryState> CustomObjects { get; set; }

        /// <summary>
        /// Id della tipologia cui si riferiscono i campi contenuti nella proprietà <see cref="CustomObjects"/>
        /// </summary>
        [DataMember]
        public String TemplateId { get; set; }

        /// <summary>
        /// Enumerazione delle possibili tipologie documentali
        /// </summary>
        public enum ObjectType
        {
            Document,
            Folder
        }

        /// <summary>
        /// Tipo di oggetto cui è indirizzata questa request
        /// </summary>
        [DataMember]
        public ObjectType ObjType { get; set; }

        /// <summary>
        /// True se bisogna attivare lo storico su tutti i campi della tipologia
        /// </summary>
        [DataMember]
        public bool ActiveAllFields { get; set; }
    }
}
