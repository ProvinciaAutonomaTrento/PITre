using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for InsertContext.
	/// </summary>
	public class InsertContext
	{
		public InsertContext()
		{
		}

		public int numRow;
		public string operation;
		public string val;
		public DocsPaVO.documento.SchedaDocumento schedaDoc;
	}
}
