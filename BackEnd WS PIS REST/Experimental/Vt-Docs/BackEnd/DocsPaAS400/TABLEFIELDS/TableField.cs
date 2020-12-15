using System;

namespace DocsPaAS400.tableFields
{
	/// <summary>
	/// Summary description for TableField.
	/// </summary>
	public interface TableField
	{

		string getFieldValue(InsertContext context); 
  
		string getFieldSQLValue(InsertContext context);

		string getFieldName();

	}
}
