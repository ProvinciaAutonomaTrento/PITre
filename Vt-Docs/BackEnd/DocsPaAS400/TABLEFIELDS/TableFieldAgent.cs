using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for TableFieldAgent.
	/// </summary>
	public class TableFieldAgent
	{
		public TableFieldAgent()
		{
		}

		public static TableField getTableFieldInstance(string fieldName)
		{
			if(fieldName.Equals(Constants.JOUCOD_FIELD_NAME))
			{
				return new Joucod();
			}
			if(fieldName.Equals(Constants.JOUDES_FIELD_NAME))
			{
				return new Joudes();
			}
			if(fieldName.Equals(Constants.JOUDOC_FIELD_NAME))
			{
				return new Joudoc();
			}
			if(fieldName.Equals(Constants.JOULEN_FIELD_NAME))
			{
				return new Joulen();
			}
			if(fieldName.Equals(Constants.JOUPRO_FIELD_NAME))
			{
				return new Joupro();
			}
			if(fieldName.Equals(Constants.JOUREC_FIELD_NAME))
			{
                return new Jourec();
			}
			if(fieldName.Equals(Constants.JOUTYP_FIELD_NAME))
			{
				return new Joutyp();
			}
			return null;
		}
	}
}
