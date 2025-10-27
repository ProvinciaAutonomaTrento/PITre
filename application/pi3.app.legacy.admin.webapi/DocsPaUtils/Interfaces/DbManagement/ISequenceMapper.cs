// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
