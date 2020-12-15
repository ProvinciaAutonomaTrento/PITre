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
		/// può effettuare diversi controlli di business.
		/// NB: In presenza di sole BrokenRules di livello "Warning", 
		/// il valore di ritorno deve essere sempre "true".
		/// </summary>
        public bool Value = true;

		/// <summary>
		/// Lista delle eventuali "BusinessRules" non validate
		/// </summary>
		[XmlArray()]
		[XmlArrayItem(typeof(BrokenRule))]
		public ArrayList BrokenRules=new ArrayList();
	}
}