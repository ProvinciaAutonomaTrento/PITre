// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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