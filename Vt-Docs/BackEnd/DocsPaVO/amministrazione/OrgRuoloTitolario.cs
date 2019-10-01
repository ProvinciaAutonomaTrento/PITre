using System;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Rappresentazione l'associazione di un ruolo con 
	/// un nodo di titolario (mediante un registro) 
	/// </summary>
	public class OrgRuoloTitolario
	{
		public string ID=string.Empty;
		
		public string Codice=string.Empty;
		
		public string Descrizione=string.Empty;

		/// <summary>
		/// Se true, il ruolo ha la visibilità su un nodo di titolario
		/// </summary>
		public bool Associato=false;
	}
}
