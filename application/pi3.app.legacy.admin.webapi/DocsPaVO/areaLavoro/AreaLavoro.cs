// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Xml.Serialization;
using System.Collections;

namespace DocsPaVO.areaLavoro
{
	/// <summary>
	/// </summary>
	public class AreaLavoro 
	{
		/// <summary>
		/// Numero totale di documenti trovati nell'area di lavoro.
		/// </summary>
		/// <remarks>
		/// Il numero riportato non � necessariamente il numero di documenti 
		/// nell'array lista (nel caso del paging lato server)
		/// </remarks>
		public int TotalRecs;

		[XmlArray()]
		[XmlArrayItem(typeof(DocsPaVO.documento.InfoDocumento))]
		[XmlArrayItem(typeof(DocsPaVO.fascicolazione.Fascicolo))]
		public System.Collections.ArrayList lista;

		/// <summary>
		/// </summary>
		public AreaLavoro()
		{
			lista = new System.Collections.ArrayList();
		}
	}
}