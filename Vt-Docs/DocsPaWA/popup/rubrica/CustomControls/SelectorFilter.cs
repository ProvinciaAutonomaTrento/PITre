using System;

namespace DocsPAWA.popup.RubricaDocsPA.CustomControls
{
	/// <summary>
	/// Delegato per la gestione dell'evento SelectorFilter
	/// </summary>
	public delegate bool SelectorFilterHandler (object sender, SelectorFilterArgs e);

	/// <summary>
	/// Un'istanza di questa classe viene passata come argomento all'evento
	/// <see cref="RemoveItem"/>
	/// </summary>
	public class SelectorFilterArgs 
	{		
		/// <summary>
		/// L'elemento per cui decidere di visualizzare o meno il selettore
		/// </summary>
		/// 
		public string Codice;

		/// <summary>
		/// Il tipo dell'elemento per cui decidere di visualizzare o meno il selettore
		/// </summary>
		/// 
		public string TipoIE;

		public SelectorFilterArgs (string t, string c)
		{
			if (t.Length != 1 || (t != "I" && t != "E"))
				throw new ArgumentException ("SelectorFilterArgs - Argomento non valido");

			this.Codice = c;
			this.TipoIE = t;
		}
	}
}
