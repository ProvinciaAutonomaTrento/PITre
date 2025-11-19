// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.Validations
{
	/// <summary>
	/// Gestione validazione regole di business
	/// </summary>
	//[XmlInclude(typeof(BrokenRule))]
    [Serializable()]
	[DataContract]
	public class ValidationResultInfo
	{
		/// <summary>
		/// Valore di ritorno complessivo di una funzione, che
		/// pu� effettuare diversi controlli di business.
		/// NB: In presenza di sole BrokenRules di livello "Warning", 
		/// il valore di ritorno deve essere sempre "true".
		/// </summary>
		[DataMember]
        public bool Value { get; set; } = true;

		/// <summary>
		/// Lista delle eventuali "BusinessRules" non validate
		/// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(BrokenRule))]
        [DataMember]
        public ArrayList BrokenRules { get; set; } = new ArrayList();
	}
}