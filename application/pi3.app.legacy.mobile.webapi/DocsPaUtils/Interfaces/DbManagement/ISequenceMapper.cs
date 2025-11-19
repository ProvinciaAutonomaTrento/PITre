// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace DocsPaUtils.Interfaces.DbManagement
{
	/// <summary>
	/// Interfaccia per la gestione delle mappatura tableName - sequence per un motore di database
	/// </summary>
	public interface ISequenceMapper
	{
		string GetSequenceName(string tableName);
	}
}
