// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento 
{
	/// <summary>
	/// </summary>
	[XmlType("DocumentoQueryOggetto")]
    [Serializable()]
	public class QueryOggetto 
	{
		public ArrayList idRegistri;
		public string idAmministrazione;
		public string queryDescrizione;
        public string queryCodice;
	}
}