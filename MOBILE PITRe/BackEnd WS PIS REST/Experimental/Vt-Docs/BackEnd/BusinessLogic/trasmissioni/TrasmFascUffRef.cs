using System;

namespace BusinessLogic.trasmissioni
{
	/// <summary>
	/// Summary description for TrasmFascUffRef.
	/// </summary>
	public class TrasmFascUffRef
	{
		/// <summary>
		/// Eccezione che indica che le ragioni trasmissioni dedicate alla protocollazione 
		/// interna non sono state trovate sul DB.
		/// </summary>
		public class RagioniNotFoundException : Exception
		{
			public override string Message
			{
				get	{ return "Ragione di trasmissione assente per l'Ufficio Referente."; }
			}
		}
		/// <summary>
		/// Verifica l'esistenza delle ragioni di trasmissione  la trasmissione a un ufficio referente
		/// </summary>
		/// <param name="idAmm">system_id dell'amministrazione</param>
		/// <returns>bool</returns>
		public bool VerificaRagioniUffReferente(string idAmm)
		{
			bool result;
			DocsPaDB.Query_DocsPAWS.Trasmissione vtr = new DocsPaDB.Query_DocsPAWS.Trasmissione();
			result = vtr.VerificaRagTrasmUffRef(idAmm);
			return result;
		}

	}
}
