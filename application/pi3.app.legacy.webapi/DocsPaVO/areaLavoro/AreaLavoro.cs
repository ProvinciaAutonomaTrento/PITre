// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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

		public Object[] lista;

		/// <summary>
		/// </summary>
		public AreaLavoro()
		{
			lista = new Object[0];
		}
	}
}