using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Joulen : TableField
	{

		//private int[] values;

		public Joulen()
		{
			//init();
		}

		public string getFieldValue(InsertContext context)
		{
			if(context.numRow>Constants.NUM_CODES) return "0";
            return Constants.JOULEN_FIELD_VALUES[context.numRow].ToString();
		}

		public string getFieldSQLValue(InsertContext context)
		{
			return getFieldValue(context);
		}

		public string getFieldName()
		{
			return Constants.JOULEN_FIELD_NAME;
		}

		/*private void init()
		{
            values=new int[]{0,7,9,5,1,5,1,1,5,5,5,37,16,9,28,30,50,117,10,4,1,2};
		}*/

        
	}
}

