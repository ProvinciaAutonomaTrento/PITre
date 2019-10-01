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
