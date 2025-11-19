// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Definizione oggetto Tipo Funzione 
	/// relativo alla funzionalit� Organigramma in Amministrazione.
	/// </summary>
	public class OrgTipoFunzione
	{
		public string IDTipoFunzione = string.Empty;

		public string Codice = string.Empty;

		public string Descrizione = string.Empty;

		public string IDAmministrazione = string.Empty;

		public string Associato = string.Empty;

		/// <summary>
		/// Singole funzioni associate al tipo funzione
		/// </summary>
		public OrgFunzione[] Funzioni=null;
	}
}
