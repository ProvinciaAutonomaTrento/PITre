// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Unit� Organizzativa 
	/// relativo alla funzionalit� Organigramma in Amministrazione.
	/// </summary>
	[DataContract]
    public class OrgUO
	{
		[DataMember]
		public string IDCorrGlobale { get; set; } = string.Empty;

        [DataMember]
        public string Codice { get; set; } = string.Empty;

        [DataMember]
        public string CodiceRubrica { get; set; } = string.Empty;

        [DataMember]
        public string Descrizione { get; set; } = string.Empty;

        [DataMember]
        public string Livello { get; set; } = string.Empty;

        [DataMember]
        public string IDAmministrazione { get; set; } = string.Empty;

        [DataMember]
        public string CodiceRegistroInterop { get; set; } = string.Empty;

        [DataMember]
        public string Ruoli { get; set; } = string.Empty;

        [DataMember]
        public string SottoUo { get; set; } = string.Empty;

        [DataMember]
        public string IDParent { get; set; } = string.Empty;

        [DataMember]
        public OrgDettagliGlobali DettagliUo { get; set; } = null;

        [DataMember]
        public string IDPeso { get; set; } = string.Empty;

        [DataMember]
        public string Classifica { get; set; } = string.Empty;

        /// <summary>
        /// Id del registro utilizzato per l'interoperabilit� semplificata. Se null, significa che 
        /// la UO non � interoperante
        /// </summary>
        [DataMember]
        public String IdRegistroInteroperabilitaSemplificata { get; set; }

        /// <summary>
        /// Id dell'RF utilizzato per l'interoperabilit� semplificata. 
        /// </summary>
        [DataMember]
        public String IdRfInteroperabilitaSemplificata { get; set; }

	}
}
