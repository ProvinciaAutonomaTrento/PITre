using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for Jourec.
	/// </summary>
	public class Jourec : TableField
	{
		public Jourec()
		{
		}

		public String getFieldValue(InsertContext context)
		{
           if(context.numRow==0) return "0";
		   if(context.numRow>0 && context.numRow<22) return "1";
		   return "2";
		}

		public string getFieldSQLValue(InsertContext context)
		{
			return "'"+getFieldValue(context)+"'";
		}

		public string getFieldName()
		{
			return Constants.JOUREC_FIELD_NAME;
		}

	}
}
