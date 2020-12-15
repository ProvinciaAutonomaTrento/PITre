using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.filtri
{
	public class FilterVisibility
	{

		public TypeFilterVisibility Type
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		
	}
	/// <summary>
	/// </summary>
	[XmlType("FilterVisibility2")]
	public enum TypeFilterVisibility
	{
		USER,
		ROLE,
		DATE,
		DATE_FROM,
		DATE_TO,
		DATE_WEEK,
		DATE_MONTH,
		CAUSE,
		TYPE
	}


}
