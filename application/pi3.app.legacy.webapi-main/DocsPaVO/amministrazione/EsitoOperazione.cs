// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace DocsPaVO.amministrazione
{
	/// <summary>
	/// Summary description for EsitoOperazione.
	/// </summary>
	public class EsitoOperazione
	{
		public int Codice = 0;
		public string Descrizione = String.Empty;
        public int numDocCopiati = 0;
        public int numeroFascCopiati = 0;
        public int numeroDocinFascCopiati = 0;
    }
}
