using System;

namespace DocsPaDocumentale.FullTextSearch
{
	/// <summary>
	/// Informazioni relative ad un file trovato dalla ricerca FullText
	/// </summary>
	public class FullTextResultInfo
	{
		public string Name=string.Empty;
		public string FileName=string.Empty;
		public string Rank=string.Empty;
		public string DocTitle=string.Empty;
		public string VPath=string.Empty;
		public string Characterization=string.Empty;
		public string Write=string.Empty;

		public FullTextResultInfo()
		{
		}
	}
}
