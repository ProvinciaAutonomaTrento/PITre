// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.Validations
{
	/// <summary>
	/// Gestione validazione regole di business
	/// </summary>
	//[XmlInclude(typeof(BrokenRule))]
    [Serializable()]
	public class ValidationResultInfo
	{
		/// <summary>
		/// Valore di ritorno complessivo di una funzione, che
		/// pu� effettuare diversi controlli di business.
		/// NB: In presenza di sole BrokenRules di livello "Warning", 
		/// il valore di ritorno deve essere sempre "true".
		/// </summary>
        public bool Value = true;

		/// <summary>
		/// Lista delle eventuali "BusinessRules" non validate
		/// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(BrokenRule))]
		public BrokenRule[] BrokenRules =new BrokenRule[0];
	}
}