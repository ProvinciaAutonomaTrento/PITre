using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Joupro : TableField
	{

		public Joupro()
		{
		}

		public string getFieldValue(InsertContext context)
		{
			string res="";
			if(context.numRow<22)
			{
				res="0";
			}
			else
			{
				int temp=context.numRow-21;
                res=temp.ToString();
			}
			return res;
		}

		public string getFieldSQLValue(InsertContext context)
		{
			return getFieldValue(context);
		}

		public string getFieldName()
		{
			return Constants.JOUPRO_FIELD_NAME;
		}

        
	}
}
