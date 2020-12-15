using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Joudes :TableField
	{
		static int JOUDES_LENGTH=255;
		static int JOUDES_NUM_WHITESPACES=20;
		static int JOUDES_NUM_ZEROES=3;
		static string WHITESPACE=" ";
		static string JOUDES_DATE_FORMAT="yyMMdd";
		static string JOUDES_HOUR_FORMAT="hhmmss";

		public static String getJoudesFirstValue(string operationValue)
		{
		   string result="";
           
		    //inserisce gli spazi bianchi nel result
			for(int i=0;i<Joudes.JOUDES_NUM_WHITESPACES;i++)
			{
               result=result+Joudes.WHITESPACE; 
			}
 
			//inserisce la data 
			DateTime systemDate=DateTime.Now;
			string date=systemDate.ToString(Joudes.JOUDES_DATE_FORMAT);
            string hour=systemDate.ToString(Joudes.JOUDES_HOUR_FORMAT);
            result=result+date+hour;

			//inserisce il parametro relativo all'operazione
            result=result+operationValue;

			//inserisce gli zeri
			for(int i=0;i<Joudes.JOUDES_NUM_ZEROES;i++)
			{
               result=result+"0";
			}

            //completa con spazi bianchi
			for(int i=result.Length;i<Joudes.JOUDES_LENGTH;i++)
			{
               result=result+Joudes.WHITESPACE;
			}

			return result;
		}

		public string getFieldValue(InsertContext context)
		{
			if(context.numRow==0) return getJoudesFirstValue(context.operation);
			if(context.val==null || context.val.Equals(""))
			{
				return getFilledValue(context);
			}
			return context.val;
		}

		public string getFieldSQLValue(InsertContext context)
		{
			return "'"+getFieldValue(context).Replace("'","''")+"'";
		}

		public string getFieldName()
		{
			return Constants.JOUDES_FIELD_NAME;
		}

		private string getFilledValue(InsertContext context)
		{
			string res="";
			if(Constants.JOUTYP_FIELD_VALUES[context.numRow].Equals("Z"))
			{
				for(int i=0;i<Constants.JOULEN_FIELD_VALUES[context.numRow];i++)
				{
                   res=res+"0";
				}
			}
			return res;
		}



	}

}
