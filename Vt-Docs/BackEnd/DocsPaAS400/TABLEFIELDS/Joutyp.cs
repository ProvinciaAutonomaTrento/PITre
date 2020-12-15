using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Joutyp : TableField
	{
        //private string[] values;

		public Joutyp()
		{
			//init();
		}

		public string getFieldValue(InsertContext context)
		{
			if(context.numRow>Constants.NUM_CODES) return "A";
			return Constants.JOUTYP_FIELD_VALUES[context.numRow];
		}

		public string getFieldSQLValue(InsertContext context)
		{
			return "'"+getFieldValue(context)+"'";
		}

		public string getFieldName()
		{
            return Constants.JOUTYP_FIELD_NAME;
		}

		/*private void init()
		{
			values=new string[]{"","Z","Z","Z","A","A","A","A","Z","Z","A","A","A","Z","A","A","A","A","A","A","A","A"};
		}*/
	}
}
