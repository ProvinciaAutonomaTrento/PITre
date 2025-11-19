// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
		// appartenenti al ruolo con cui � connesso
		// il mittente dello smistamento
		public string[] RegistriAppartenenza=new string[0];

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
