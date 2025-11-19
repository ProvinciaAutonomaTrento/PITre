// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Tipo Funzione 
	/// relativo alla funzionalit� Organigramma in Amministrazione.
	/// </summary>
	[DataContract]
    public class OrgTipoFunzione
	{
		[DataMember]
        public string IDTipoFunzione { get; set; } = string.Empty;

        [DataMember]
        public string Codice { get; set; } = string.Empty;

        [DataMember]
        public string Descrizione { get; set; } = string.Empty;

        [DataMember]
        public string IDAmministrazione { get; set; } = string.Empty;

        [DataMember]
        public string Associato { get; set; } = string.Empty;

        /// <summary>
        /// Singole funzioni associate al tipo funzione
        /// </summary>
        [DataMember]
        public OrgFunzione[] Funzioni { get; set; } = null;
	}
}
