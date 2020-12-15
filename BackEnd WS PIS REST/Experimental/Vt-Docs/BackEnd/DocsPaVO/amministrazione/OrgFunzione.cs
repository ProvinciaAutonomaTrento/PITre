using System;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Rappresentazione di una funzione elementare in anagrafica amministrazione
	/// </summary>
	public class OrgFunzioneAnagrafica
	{
		public string Codice=string.Empty;

		public string Descrizione=string.Empty;

		public string TipoFunzione=string.Empty;
	}

	/// <summary>
	/// Rappresentazione del legame tra una funzione elementare e un tipo funzione
	/// </summary>
	public class OrgFunzione
	{
		/// <summary>
		/// ID della funzione
		/// </summary>
		public string ID=string.Empty;

		/// <summary>
		/// ID del tipo funzione
		/// </summary>
		public string IDTipoFunzione=string.Empty;

		/// <summary>
		/// Associazione con la corrispondente funzione in anagrafica
		/// </summary>
		public OrgFunzioneAnagrafica FunzioneAnagrafica=null;

		/// <summary>
		/// Flag, true se la funzione elementare è associata al tipo funzione
		/// </summary>
		public bool Associato=false;

		/// <summary>
		/// Stato della funzione, default: non modificato
		/// </summary>
		public StatoOrgFunzioneEnum StatoFunzione=StatoOrgFunzioneEnum.Unchanged;

		/// <summary>
		/// Definizione degli stati della funzione
		/// </summary>
		public enum StatoOrgFunzioneEnum
		{
			Unchanged,	// non modificato
			Inserted,	// inserito il legame
			Deleted		// cancellato il legame
		}

        /// <summary>
        /// idAmministrazione di riferimento
        /// </summary>
        public string IDAmministrazione=string.Empty;
	}
}