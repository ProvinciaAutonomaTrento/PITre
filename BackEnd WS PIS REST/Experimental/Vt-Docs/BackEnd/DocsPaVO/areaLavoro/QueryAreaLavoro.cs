using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.areaLavoro
{
	/// <summary>
	/// </summary>
	[XmlType("AreaLavoroQueryAreaLavoro")]
	public class QueryAreaLavoro
	{
		public static Hashtable tipoDocString;
		public static Hashtable tipoFascString;

		/// <summary>
		/// </summary>
		public QueryAreaLavoro()
		{
			if(tipoDocString==null)
			{
				tipoDocString=new System.Collections.Hashtable();
				tipoDocString.Add(TipoDocumento.ARRIVO,"A");
				tipoDocString.Add(TipoDocumento.GRIGIO,"G");
				tipoDocString.Add(TipoDocumento.PARTENZA,"P");
				tipoDocString.Add(TipoDocumento.INTERNO,"I");
			}

			if(tipoFascString==null)
			{
				tipoFascString=new System.Collections.Hashtable();
				tipoFascString.Add(TipoFascicolo.GENERALE,"G");
				tipoFascString.Add(TipoFascicolo.PROCEDIMENTALE,"P");
			}
		}
	}
}