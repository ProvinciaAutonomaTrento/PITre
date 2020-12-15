using System;

namespace DocsPaDocumentale_HUMMINGBIRD.HummingbirdLib.Tipi
{
	/// <summary>
	/// Valori da utilizzare nell'assegnazione delle proprietà dell'oggetto 
	/// documento del documentale
	/// </summary>
	public struct VersionDirectiveType
	{
		public const string AddAttachment = "%ADD_ATTACHMENT";
		public const string DeleteVersion = "%PCD_DELETE_VERSION";
		public const string NewVersion = "%PCD_NEW_VERSION";
		public const string NewSubversion = "%PCD_NEWSUBVERSION";
	}
}
