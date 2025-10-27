// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
		/// Se true, il ruolo ha la visibilit� su un nodo di titolario
		/// </summary>
		public bool Associato=false;
	}
}
