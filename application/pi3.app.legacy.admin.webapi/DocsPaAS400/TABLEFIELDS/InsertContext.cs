// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
