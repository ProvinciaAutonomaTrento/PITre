// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for Joudoc.
	/// </summary>
	public class Joudoc : TableField
	{
		public Joudoc()
		{
		}

		public string getFieldValue(InsertContext context)
		{
            return context.schedaDoc.systemId;
		}

		public string getFieldSQLValue(InsertContext context)
		{
			return getFieldValue(context);
		}

		public string getFieldName()
		{
			return Constants.JOUDOC_FIELD_NAME;
		}
	}
}
