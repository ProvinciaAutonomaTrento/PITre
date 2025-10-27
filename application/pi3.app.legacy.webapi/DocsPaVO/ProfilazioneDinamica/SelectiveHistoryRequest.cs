// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;

namespace DocsPaVO.ProfilazioneDinamica
{
    /// <summary>
    /// Request relativa ai servizi di gestione dello storico dei campi profilati
    /// </summary>
    [Serializable()]
    public class SelectiveHistoryRequest
    {
        /// <summary>
        /// Lista delle informazioni sullo stato di abilitazione dello storico relativamente 
        /// ai campi di una tipologia
        /// </summary>
        public List<CustomObjHistoryState> CustomObjects { get; set; }

        /// <summary>
        /// Id della tipologia cui si riferiscono i campi contenuti nella proprietà <see cref="CustomObjects"/>
        /// </summary>
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
        public ObjectType ObjType { get; set; }

        /// <summary>
        /// True se bisogna attivare lo storico su tutti i campi della tipologia
        /// </summary>
        public bool ActiveAllFields { get; set; }
    }
}
