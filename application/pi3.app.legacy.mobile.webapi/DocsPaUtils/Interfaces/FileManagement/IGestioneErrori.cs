// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;

namespace DocsPaUtils.Interfaces.FileManagement
{
	/// <summary>
	/// Interfaccia per la gestione degli errori con il documentale
	/// </summary>
	public interface IGestioneErrori
	{
		/// <summary>
		/// Ritorna l'ultimo codice di errore.
		/// </summary>
		/// <returns>
		/// Codice errore dell'ultima operazione (0 = nessun errore)
		/// </returns>
		int GetErrorCode();

		/// <summary>
		/// Ritorna l'ultimo codice di errore.
		/// </summary>
		/// <param name="customDescription">Eventuale descrizione custom.</param>
		/// <returns>
		/// Codice errore dell'ultima operazione (0 = nessun errore)
		/// </returns>
		int GetErrorCode(string customDescription);
	}
}
