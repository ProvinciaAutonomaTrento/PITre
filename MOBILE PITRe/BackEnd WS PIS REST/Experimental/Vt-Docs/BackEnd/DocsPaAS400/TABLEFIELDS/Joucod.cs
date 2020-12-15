using System;
using System.Collections;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for JOUCOD.
	/// </summary>
	public class Joucod : TableField
	{

		private int[] values;

		public Joucod()
		{
           init();
		}

		public string getFieldValue(InsertContext context)
		{
			if(context.numRow <= Constants.NUM_CODES)
			{
               return values[context.numRow].ToString();
			}
			else
			{
               return "10";
			}
		}

		public string getFieldSQLValue(InsertContext context)
		{
           return getFieldValue(context);
		}

		public string getFieldName()
		{
			return Constants.JOUCOD_FIELD_NAME;
		}

		private void init()
		{
			values=new int[]{0,1,2,3,5,10,11,12,13,14,27,30,31,32,33,35,40,45,50,55,60,65};
		}

           
	}
}
