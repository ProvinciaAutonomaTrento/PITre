using System;
using System.Xml.Serialization;

namespace DocsPaVO.Smistamento
{
	/// <summary>
	/// Dati riguardanti l'utente che ha inviato il documento durante lo smistamento
	/// </summary>
	public class MittenteSmistamento
	{
		#region Attributi persona

		// ID della persona in tabella People
		public string IDPeople=string.Empty;

		// ID dell'amministrazione
		public string IDAmministrazione=string.Empty;

		// ArrayList contenente i systemid dei registri
		// appartenenti al ruolo con cui è connesso
		// il mittente dello smistamento
		[XmlArray()]
		[XmlArrayItem(typeof(string))]
		public System.Collections.ArrayList RegistriAppartenenza=new System.Collections.ArrayList();

		// Indirizzo mail del mittente
		public string EMail=string.Empty;

		#endregion

		#region Attributi ruolo

		// ID del ruolo della persona in tabella corr_globali
		public string IDCorrGlobaleRuolo=string.Empty;

		// ID del ruolo della persona in tabella groups
		public string IDGroup=string.Empty;

		// Codice del livello del ruolo in tabella corr_globali
		public string LivelloRuolo=string.Empty;

		#endregion

	}
}
